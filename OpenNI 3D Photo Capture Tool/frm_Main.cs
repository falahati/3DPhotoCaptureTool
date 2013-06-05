using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenNIWrapper;
using _3DImageGenerator;

namespace OpenNI_3D_Photo_Capture_Tool
{
    public partial class frm_Main : Form
    {
        public frm_Main()
        {
            InitializeComponent();
        }

        private bool HandleError(OpenNI.Status status)
        {
            if (status == OpenNI.Status.OK)
                return true;
            MessageBox.Show("Error: " + status.ToString() + " - " + OpenNI.LastError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return false;
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {
            HandleError(OpenNI.Initialize());
            OpenNI.onDeviceConnected += new OpenNI.DeviceConnectionStateChanged(OpenNI_onDeviceConnectionStateChanged);
            OpenNI.onDeviceDisconnected += new OpenNI.DeviceConnectionStateChanged(OpenNI_onDeviceConnectionStateChanged);
            UpdateDevicesList();
        }

        void OpenNI_onDeviceConnectionStateChanged(DeviceInfo Device)
        {
            this.BeginInvoke((Action)delegate
            {
                UpdateDevicesList();
            });
        }

        private void UpdateDevicesList()
        {
            DeviceInfo[] devices = OpenNI.EnumerateDevices();
            cb_devices.Items.Clear();
            cb_devices.Items.Add("None");
            cb_devices.Enabled = devices.Length != 0;
            for (int i = 0; i < devices.Length; i++)
                cb_devices.Items.Add(devices[i]);
            if (cb_devices.Items.Count > 0)
                cb_devices.SelectedIndex = 0;
        }

        Device selectedDevice = null;
        VideoStream depthStream = null;
        VideoStream colorStream = null;
        bool isUse960asHD = false;
        private void DeviceChanged()
        {
            but_anag.Enabled = false;
            but_stereo.Enabled = false;
            if (selectedDevice != null)
                selectedDevice.Close();
            if (cb_devices.Items.Count < 1)
            {
                selectedDevice = null;
                return;
            }
            if (cb_devices.SelectedItem == null)
            {
                selectedDevice = null;
                return;
            }
            if (cb_devices.SelectedItem is string && cb_devices.SelectedItem.ToString() == "None")
            {
                selectedDevice = null;
                return;
            }
            if (!(cb_devices.SelectedItem is DeviceInfo))
            {
                selectedDevice = null;
                MessageBox.Show("Selected item is not a device.", "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                selectedDevice = (cb_devices.SelectedItem as DeviceInfo).OpenDevice();
            }
            catch (Exception)
            {
                selectedDevice = null;
                MessageBox.Show("Can not open selected device.", "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!selectedDevice.hasSensor(Device.SensorType.COLOR))
            {
                selectedDevice.Close();
                selectedDevice = null;
                MessageBox.Show("Selected device can not offer depth stream.", "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!selectedDevice.hasSensor(Device.SensorType.DEPTH))
            {
                selectedDevice.Close();
                selectedDevice = null;
                MessageBox.Show("Selected device can not offer depth stream.", "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                depthStream = selectedDevice.CreateVideoStream(Device.SensorType.DEPTH);
                colorStream = selectedDevice.CreateVideoStream(Device.SensorType.COLOR);
            }
            catch (Exception)
            {
                selectedDevice.Close();
                selectedDevice = null;
                depthStream = null;
                colorStream = null;
                MessageBox.Show("Can not create Depth and Color streams.", "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            cb_hd.Enabled = false;
            foreach (VideoMode vm in colorStream.SensorInfo.getSupportedVideoModes())
            {
                if (vm.Resolution.Equals(new Size(1280, 1024)) || vm.Resolution.Equals(new Size(1280, 960)))
                {
                    cb_hd.Enabled = true;
                    isUse960asHD = vm.Resolution.Height == 960;
                    break;
                }
            }
            VideoMode depthMode = new VideoMode();
            depthMode.Resolution = new Size(640, 480);
            depthMode.FPS = 30;
            depthMode.DataPixelFormat = VideoMode.PixelFormat.DEPTH_1MM;
            VideoMode colorMode = new VideoMode();
            colorMode.Resolution = new Size(640, 480);
            colorMode.FPS = 30;
            colorMode.DataPixelFormat = VideoMode.PixelFormat.RGB888;
            if (cb_hd.Enabled && cb_hd.Checked)
                colorMode.Resolution = ((isUse960asHD) ? new Size(1280, 960) : new Size(1280, 1024));
            try
            {
                depthStream.VideoMode = depthMode;
                colorStream.VideoMode = colorMode;
            }
            catch (Exception)
            {
                selectedDevice.Close();
                selectedDevice = null;
                depthStream = null;
                colorStream = null;
                MessageBox.Show("Can not set Depth and Color streams video mode to 640x480@30fps. This application need at least this resolution.", "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                selectedDevice.ImageRegistration = Device.ImageRegistrationMode.DEPTH_TO_COLOR;
                lbl_registration.Visible = false;
            }
            catch (Exception)
            {
                lbl_registration.Visible = true;
            }
            try
            {
                //selectedDevice.DepthColorSyncEnabled = true;
            }
            catch (Exception) { }
            if (!HandleError(depthStream.Start()) || !HandleError(colorStream.Start()))
            {
                selectedDevice.Close();
                selectedDevice = null;
                depthStream = null;
                colorStream = null;
                MessageBox.Show("Can not start depth and color streams.", "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            depthStream.onNewFrame += new VideoStream.VideoStreamNewFrame(depthStream_onNewFrame);
            colorStream.onNewFrame += new VideoStream.VideoStreamNewFrame(colorStream_onNewFrame);
            but_anag.Enabled = true;
            but_stereo.Enabled = true;
        }
        Bitmap colorBitmap;
        void colorStream_onNewFrame(VideoStream vStream)
        {
            if (vStream.isValid && vStream.isFrameAvailable())
            {
                using (VideoFrameRef frame = vStream.readFrame())
                {
                    if (frame.isValid)
                    {
                        if (colorBitmap == null)
                            colorBitmap = new Bitmap(1, 1);
                        lock (colorBitmap)
                        {
                            try
                            {
                                frame.updateBitmap(colorBitmap, VideoFrameRef.copyBitmapOptions.Force24BitRGB);
                            }
                            catch (Exception)
                            {
                                colorBitmap = frame.toBitmap(VideoFrameRef.copyBitmapOptions.Force24BitRGB);
                            }
                        }
                        this.BeginInvoke((Action)delegate
                        {
                            lock (colorBitmap)
                            {
                                if (p_image.Image != null)
                                    p_image.Image.Dispose();
                                p_image.Image = new Bitmap(colorBitmap);
                                p_image.Refresh();
                            }
                        });
                    }
                }
            }
        }
        Bitmap depthBitmap;
        void depthStream_onNewFrame(VideoStream vStream)
        {
            if (vStream.isValid && vStream.isFrameAvailable())
            {
                using (VideoFrameRef frame = vStream.readFrame())
                {
                    if (frame.isValid)
                    {
                        if (depthBitmap == null)
                            depthBitmap = new Bitmap(1, 1);
                        lock (depthBitmap)
                        {
                            try
                            {
                                frame.updateBitmap(depthBitmap,
                                    VideoFrameRef.copyBitmapOptions.Force24BitRGB |
                                    VideoFrameRef.copyBitmapOptions.DepthInvert |
                                    VideoFrameRef.copyBitmapOptions.DepthFillShadow |
                                    VideoFrameRef.copyBitmapOptions.DepthHistogramEqualize |
                                    VideoFrameRef.copyBitmapOptions.DepthFillRigthBlack);
                            }
                            catch (Exception)
                            {
                                depthBitmap = frame.toBitmap(
                                    VideoFrameRef.copyBitmapOptions.Force24BitRGB |
                                    VideoFrameRef.copyBitmapOptions.DepthInvert |
                                    VideoFrameRef.copyBitmapOptions.DepthFillShadow |
                                    VideoFrameRef.copyBitmapOptions.DepthHistogramEqualize |
                                    VideoFrameRef.copyBitmapOptions.DepthFillRigthBlack);
                            }
                        }
                        this.BeginInvoke((Action)delegate
                        {
                            lock (depthBitmap)
                            {
                                if (p_depth.Image != null)
                                    p_depth.Image.Dispose();
                                p_depth.Image = new Bitmap(depthBitmap);
                                p_depth.Refresh();
                            }
                        });
                    }
                }
            }
        }

        private void frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (selectedDevice != null)
                    selectedDevice.Close();
            }
            catch (Exception) { }
            try
            {
                OpenNI.Shutdown();
            }
            catch (Exception) { }
            Environment.Exit(0);
        }

        private void cb_devices_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeviceChanged();
        }

        private void cb_hd_CheckedChanged(object sender, EventArgs e)
        {
            DeviceChanged();
        }
        c_3DGenerator anaggen3 = null;
        bool lastStereo = false;
        private void but_anag_Click(object sender, EventArgs e)
        {
            Bitmap c;
            Bitmap d;
            lock (colorBitmap)
            {
                lock (depthBitmap)
                {
                    c = colorBitmap.Clone(new Rectangle(new Point(0, 0), colorBitmap.Size), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    d = depthBitmap.Clone(new Rectangle(new Point(0, 0), depthBitmap.Size), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    if (anaggen3 == null)
                    {
                        anaggen3 = new c_3DGenerator();
                        anaggen3.AnaglyphComplete += new c_3DGenerator.AnaglyphCompleteEventHandler(gen3_AnagComplete);
                    }
                    but_anag.Enabled = false;
                    but_stereo.Enabled = false;
                    cb_devices.Enabled = false;
                    l_save.Enabled = false;
                    anaggen3.SwapRightLeft = cb_swap.Checked;
                    anaggen3.Smoothing = cb_smoothing.Checked;
                    anaggen3.MaxPixelDisplacement = (int)nud_maxdisp.Value;
                    if (lbl_registration.Visible)
                        d = DepthFix(depthBitmap);
                    if (colorBitmap.Size.Equals(new Size(1280, 1024)) || colorBitmap.Size.Equals(new Size(1280,960)))
                    {
                        c = colorBitmap.Clone(new Rectangle(0, ((isUse960asHD) ? 0 : 32), 1280, 960), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        d = new Bitmap(d, 1280, 960);
                    }
                    else if (!colorBitmap.Size.Equals(depthBitmap.Size))
                    {
                        c = new Bitmap(colorBitmap, 640, 480);
                        d = new Bitmap(d, 640, 480);
                    }
                }
            }
            Bitmap p = d.Clone(new Rectangle(new Point(0, 0), d.Size), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            anaggen3.GenerateAnaglyphAsync(p, d, new Bitmap[] { c, d });
        }

        void gen3_AnagComplete(bool Success, object userCode)
        {
            if (userCode is bool && (bool)userCode == true)
            {
                if (Success)
                {
                    but_anag.Enabled = true;
                    but_stereo.Enabled = true;
                    l_save.Enabled = true;
                    lastStereo = false;
                    p_image3d.Image = anaggen3.Anaglyph;
                }
                cb_devices.Enabled = true;
            }
            else
            {
                if (Success)
                {
                    p_depth3d.Image = anaggen3.Anaglyph;
                    anaggen3.GenerateAnaglyphAsync((userCode as Bitmap[])[0], (userCode as Bitmap[])[1], true);
                }
                else
                {
                    but_anag.Enabled = true;
                    but_stereo.Enabled = true;
                    cb_devices.Enabled = true;
                }
            }
        }
        c_3DGenerator strggen3 = null;
        private void but_stereo_Click(object sender, EventArgs e)
        {
            Bitmap c;
            Bitmap d;
            lock (colorBitmap)
            {
                lock (depthBitmap)
                {
                    c = colorBitmap.Clone(new Rectangle(new Point(0, 0), colorBitmap.Size), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    d = depthBitmap.Clone(new Rectangle(new Point(0, 0), depthBitmap.Size), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    if (strggen3 == null)
                    {
                        strggen3 = new c_3DGenerator();
                        strggen3.AnaglyphComplete += new c_3DGenerator.AnaglyphCompleteEventHandler(gen3_StereoComplete);
                        strggen3.StereoscopicComplete += new c_3DGenerator.StereoscopicCompleteEventHandler(gen3_StereoComplete);
                    }
                    l_save.Enabled = false;
                    but_anag.Enabled = false;
                    but_stereo.Enabled = false;
                    strggen3.SwapRightLeft = cb_swap.Checked;
                    strggen3.Smoothing = cb_smoothing.Checked;
                    strggen3.MaxPixelDisplacement = (int)nud_maxdisp.Value;
                    if (lbl_registration.Visible)
                        d = DepthFix(depthBitmap);
                    if (colorBitmap.Size.Equals(new Size(1280, 1024)) || colorBitmap.Size.Equals(new Size(1280, 960)))
                    {
                        c = colorBitmap.Clone(new Rectangle(0, ((isUse960asHD) ? 0 : 32), 1280, 960), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        d = new Bitmap(d, 1280, 960);
                    }
                    else if (!colorBitmap.Size.Equals(depthBitmap.Size))
                    {
                        c = new Bitmap(colorBitmap, 640, 480);
                        d = new Bitmap(d, 640, 480);
                    }
                }
            }
            Bitmap p = d.Clone(new Rectangle(new Point(0, 0), d.Size), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            strggen3.GenerateAnaglyphAsync(p, d, new Bitmap[] { c, d });
        }

        void gen3_StereoComplete(bool Success, object userCode)
        {
            if (userCode is bool && (bool)userCode == true)
            {
                if (Success)
                {
                    but_anag.Enabled = true;
                    but_stereo.Enabled = true;
                    l_save.Enabled = true;
                    lastStereo = true;
                    p_image3d.Image = strggen3.Stereoscopic_SideBySide;
                }
                cb_devices.Enabled = true;
            }
            else
            {
                if (Success)
                {
                    p_depth3d.Image = strggen3.Anaglyph;
                    strggen3.GenerateStereoscopicAsync((userCode as Bitmap[])[0], (userCode as Bitmap[])[1], true);
                }
                else
                {
                    but_anag.Enabled = true;
                    but_stereo.Enabled = true;
                    cb_devices.Enabled = true;
                }
            }
        }


        private Bitmap DepthFix(Bitmap depthBitmap)
        {
            Bitmap x = new Bitmap(640, 480);
            Graphics g = Graphics.FromImage(x);
            g.DrawImage(depthBitmap, new Rectangle(30, 8, 600, 450));
            g.Flush();
            g.Dispose();
            return x;
        }


        private void l_save_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!lastStereo)
            {
                SaveFileDialog.Title = "Save Anaglyph";
                SaveFileDialog.FileName = "";
                SaveFileDialog.Filter = "Jpg File|*.jpg|BMP File|*.bmp|PNG File|*.png";
                if (SaveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    switch (System.IO.Path.GetExtension(SaveFileDialog.FileName).ToLower().Trim())
                    {
                        case "png":
                            anaggen3.SaveAnaglyph(SaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case "bmp":
                            anaggen3.SaveAnaglyph(SaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                        default:
                            anaggen3.SaveAnaglyph(SaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg, 100);
                            break;
                    }
                }
            }
            else
            {
                SaveFileDialog.Title = "Save StereoScopic";
                SaveFileDialog.FileName = "";
                SaveFileDialog.Filter = "Jps File|*.jps";
                if (SaveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    strggen3.SaveStereoscopic(SaveFileDialog.FileName, 100);
                }
            }
        }

        private void l_website_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://falahati.net");
        }
    }
}
