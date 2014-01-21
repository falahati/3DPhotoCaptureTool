/*  
    Copyright (C) 2013  Soroush Falahati - soroush@falahati.net

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see [http://www.gnu.org/licenses/].
    */
namespace OpenNI_3D_Photo_Capture_Tool
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    #region

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Forms;

    using OpenNIWrapper;

    using _3DImageGenerator;

    #endregion

    // ReSharper disable once InconsistentNaming
    public partial class frm_Main : Form
    {
        #region Fields

        private c_3DGenerator anaggen3;

        private Bitmap colorBitmap;

        private VideoStream colorStream;

        private Bitmap depthBitmap;

        private VideoStream depthStream;

        private bool isUse960asHD;

        private bool lastStereo;

        private Device selectedDevice;

        private c_3DGenerator strggen3;

        #endregion

        #region Constructors and Destructors

        public frm_Main()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        //private Bitmap DepthFix(Bitmap depthBitmap)
        //{
        //    Bitmap x = new Bitmap(640, 480);
        //    Graphics g = Graphics.FromImage(x);
        //    g.DrawImage(depthBitmap, new Rectangle(30, 8, 600, 450));
        //    g.Flush();
        //    g.Dispose();
        //    return x;
        //}

        private void DeviceChanged()
        {
            this.but_anag.Enabled = false;
            this.but_stereo.Enabled = false;
            this.but_saveall.Enabled = false;
            if (this.selectedDevice != null)
            {
                this.selectedDevice.Close();
            }

            if (this.cb_devices.Items.Count < 1)
            {
                this.selectedDevice = null;
                return;
            }

            if (this.cb_devices.SelectedItem == null)
            {
                this.selectedDevice = null;
                return;
            }

            if (this.cb_devices.SelectedItem is string && this.cb_devices.SelectedItem.ToString() == "None")
            {
                this.selectedDevice = null;
                return;
            }

            if (!(this.cb_devices.SelectedItem is DeviceInfo))
            {
                this.selectedDevice = null;
                MessageBox.Show(
                    "Selected item is not a device.", 
                    "Device Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                this.selectedDevice = (this.cb_devices.SelectedItem as DeviceInfo).OpenDevice();
            }
            catch (Exception)
            {
                this.selectedDevice = null;
                MessageBox.Show(
                    "Can not open selected device.", 
                    "Device Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            if (!this.selectedDevice.hasSensor(Device.SensorType.COLOR))
            {
                this.selectedDevice.Close();
                this.selectedDevice = null;
                MessageBox.Show(
                    "Selected device can not offer depth stream.", 
                    "Device Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            if (!this.selectedDevice.hasSensor(Device.SensorType.DEPTH))
            {
                this.selectedDevice.Close();
                this.selectedDevice = null;
                MessageBox.Show(
                    "Selected device can not offer depth stream.", 
                    "Device Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                this.depthStream = this.selectedDevice.CreateVideoStream(Device.SensorType.DEPTH);
                this.colorStream = this.selectedDevice.CreateVideoStream(Device.SensorType.COLOR);
            }
            catch (Exception)
            {
                this.selectedDevice.Close();
                this.selectedDevice = null;
                this.depthStream = null;
                this.colorStream = null;
                MessageBox.Show(
                    "Can not create Depth and Color streams.", 
                    "Device Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            this.cb_hd.Enabled = false;
            foreach (VideoMode vm in this.colorStream.SensorInfo.getSupportedVideoModes())
            {
                if (vm.Resolution.Equals(new Size(1280, 1024)) || vm.Resolution.Equals(new Size(1280, 960)))
                {
                    this.cb_hd.Enabled = true;
                    this.isUse960asHD = vm.Resolution.Height == 960;
                    break;
                }
            }

            VideoMode depthMode = new VideoMode
                                      {
                                          Resolution = new Size(640, 480),
                                          FPS = 30,
                                          DataPixelFormat = VideoMode.PixelFormat.DEPTH_1MM
                                      };
            VideoMode colorMode = new VideoMode
                                      {
                                          Resolution = new Size(640, 480),
                                          FPS = 30,
                                          DataPixelFormat = VideoMode.PixelFormat.RGB888
                                      };
            if (this.cb_hd.Enabled && this.cb_hd.Checked)
            {
                colorMode.Resolution = this.isUse960asHD ? new Size(1280, 960) : new Size(1280, 1024);
            }

            try
            {
                this.depthStream.VideoMode = depthMode;
                this.colorStream.VideoMode = colorMode;
            }
            catch (Exception)
            {
                this.selectedDevice.Close();
                this.selectedDevice = null;
                this.depthStream = null;
                this.colorStream = null;
                MessageBox.Show(
                    "Can not set Depth and Color streams video mode to 640x480@30fps. This application need at least this resolution.", 
                    "Device Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                this.selectedDevice.ImageRegistration = Device.ImageRegistrationMode.DEPTH_TO_COLOR;
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "We failed to register image over depth map.",
                    "Device Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }

            try
            {
                // this.selectedDevice.DepthColorSyncEnabled = true;
            }
            catch (Exception)
            {
            }

            if (!this.HandleError(this.depthStream.Start()) || !this.HandleError(this.colorStream.Start()))
            {
                this.selectedDevice.Close();
                this.selectedDevice = null;
                this.depthStream = null;
                this.colorStream = null;
                MessageBox.Show(
                    "Can not start depth and color streams.", 
                    "Device Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            this.depthStream.onNewFrame += this.depthStream_onNewFrame;
            this.colorStream.onNewFrame += this.colorStream_onNewFrame;
            this.but_anag.Enabled = true;
            this.but_stereo.Enabled = true;
            this.but_saveall.Enabled = true;
        }

        private bool HandleError(OpenNI.Status status)
        {
            if (status == OpenNI.Status.OK)
            {
                return true;
            }

            MessageBox.Show(
                "Error: " + status + " - " + OpenNI.LastError, 
                "Error", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Asterisk);
            return false;
        }

        private void OpenNI_onDeviceConnectionStateChanged(DeviceInfo Device)
        {
            this.BeginInvoke((Action)this.UpdateDevicesList);
        }

        private void UpdateDevicesList()
        {
            DeviceInfo[] devices = OpenNI.EnumerateDevices();
            this.cb_devices.Items.Clear();
            this.cb_devices.Items.Add("None");
            this.cb_devices.Enabled = devices.Length != 0;
            foreach (DeviceInfo device in devices)
            {
                this.cb_devices.Items.Add(device);
            }

            if (this.cb_devices.Items.Count > 0)
            {
                this.cb_devices.SelectedIndex = 0;
            }
        }

        private void but_anag_Click(object sender, EventArgs e)
        {
            Bitmap c;
            Bitmap d;
            lock (this.colorBitmap)
            {
                lock (this.depthBitmap)
                {
                    c = this.colorBitmap.Clone(
                        new Rectangle(new Point(0, 0), this.colorBitmap.Size), 
                        PixelFormat.Format24bppRgb);
                    d = this.depthBitmap.Clone(
                        new Rectangle(new Point(0, 0), this.depthBitmap.Size), 
                        PixelFormat.Format24bppRgb);

                    if (this.anaggen3 == null)
                    {
                        this.anaggen3 = new c_3DGenerator();
                        this.anaggen3.AnaglyphComplete += this.gen3_AnagComplete;
                    }

                    this.but_anag.Enabled = false;
                    this.but_stereo.Enabled = false;
                    this.cb_devices.Enabled = false;
                    this.l_save.Enabled = false;
                    this.l_saveanag.Enabled = false;
                    this.anaggen3.SwapRightLeft = this.cb_swap.Checked;
                    this.anaggen3.Smoothing = this.cb_smoothing.Checked;
                    this.anaggen3.MaxPixelDisplacement = (int)this.nud_maxdisp.Value;

                    // if (lbl_registration.Visible)
                    // {
                    // d = this.DepthFix(this.depthBitmap);
                    // }
                    if (this.colorBitmap.Size.Equals(new Size(1280, 1024))
                        || this.colorBitmap.Size.Equals(new Size(1280, 960)))
                    {
                        c = this.colorBitmap.Clone(
                            new Rectangle(0, this.isUse960asHD ? 0 : 32, 1280, 960), 
                            PixelFormat.Format24bppRgb);
                        d = new Bitmap(d, 1280, 960);
                    }
                    else if (!this.colorBitmap.Size.Equals(this.depthBitmap.Size))
                    {
                        c = new Bitmap(this.colorBitmap, 640, 480);
                        d = new Bitmap(d, 640, 480);
                    }
                }
            }

            Bitmap p = d.Clone(new Rectangle(new Point(0, 0), d.Size), PixelFormat.Format24bppRgb);
            this.anaggen3.GenerateAnaglyphAsync(p, d, new[] { c, d });
        }

        private void but_stereo_Click(object sender, EventArgs e)
        {
            Bitmap c;
            Bitmap d;
            lock (this.colorBitmap)
            {
                lock (this.depthBitmap)
                {
                    c = this.colorBitmap.Clone(
                        new Rectangle(new Point(0, 0), this.colorBitmap.Size), 
                        PixelFormat.Format24bppRgb);
                    d = this.depthBitmap.Clone(
                        new Rectangle(new Point(0, 0), this.depthBitmap.Size), 
                        PixelFormat.Format24bppRgb);

                    if (this.strggen3 == null)
                    {
                        this.strggen3 = new c_3DGenerator();
                        this.strggen3.AnaglyphComplete += this.gen3_StereoComplete;
                        this.strggen3.StereoscopicComplete += this.gen3_StereoComplete;
                    }

                    this.l_save.Enabled = false;
                    this.l_saveanag.Enabled = false;
                    this.but_anag.Enabled = false;
                    this.but_stereo.Enabled = false;
                    this.strggen3.SwapRightLeft = this.cb_swap.Checked;
                    this.strggen3.Smoothing = this.cb_smoothing.Checked;
                    this.strggen3.MaxPixelDisplacement = (int)this.nud_maxdisp.Value;

                    // if (lbl_registration.Visible)
                    // {
                    // d = this.DepthFix(this.depthBitmap);
                    // }
                    if (this.colorBitmap.Size.Equals(new Size(1280, 1024))
                        || this.colorBitmap.Size.Equals(new Size(1280, 960)))
                    {
                        c = this.colorBitmap.Clone(
                            new Rectangle(0, this.isUse960asHD ? 0 : 32, 1280, 960), 
                            PixelFormat.Format24bppRgb);
                        d = new Bitmap(d, 1280, 960);
                    }
                    else if (!this.colorBitmap.Size.Equals(this.depthBitmap.Size))
                    {
                        c = new Bitmap(this.colorBitmap, 640, 480);
                        d = new Bitmap(d, 640, 480);
                    }
                }
            }

            Bitmap p = d.Clone(new Rectangle(new Point(0, 0), d.Size), PixelFormat.Format24bppRgb);
            this.strggen3.GenerateAnaglyphAsync(p, d, new[] { c, d });
        }

        private void cb_devices_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DeviceChanged();
        }

        private void cb_hd_CheckedChanged(object sender, EventArgs e)
        {
            this.DeviceChanged();
        }

        private void colorStream_onNewFrame(VideoStream vStream)
        {
            if (vStream.isValid && vStream.isFrameAvailable())
            {
                using (VideoFrameRef frame = vStream.readFrame())
                {
                    if (frame.isValid)
                    {
                        if (this.colorBitmap == null)
                        {
                            this.colorBitmap = new Bitmap(1, 1);
                        }

                        lock (this.colorBitmap)
                        {
                            try
                            {
                                frame.updateBitmap(this.colorBitmap, VideoFrameRef.copyBitmapOptions.Force24BitRGB);
                            }
                            catch (Exception)
                            {
                                this.colorBitmap = frame.toBitmap(VideoFrameRef.copyBitmapOptions.Force24BitRGB);
                            }
                        }

                        this.BeginInvoke(
                            (Action)delegate
                                {
                                    lock (this.colorBitmap)
                                    {
                                        if (this.p_image.Image != null)
                                        {
                                            this.p_image.Image.Dispose();
                                        }

                                        this.p_image.Image = new Bitmap(this.colorBitmap);
                                        this.p_image.Refresh();
                                    }
                                });
                    }
                }
            }
        }

        private void depthStream_onNewFrame(VideoStream vStream)
        {
            if (vStream.isValid && vStream.isFrameAvailable())
            {
                using (VideoFrameRef frame = vStream.readFrame())
                {
                    if (frame.isValid)
                    {
                        if (this.depthBitmap == null)
                        {
                            this.depthBitmap = new Bitmap(1, 1);
                        }

                        lock (this.depthBitmap)
                        {
                            try
                            {
                                frame.updateBitmap(
                                    this.depthBitmap,
                                    VideoFrameRef.copyBitmapOptions.Force24BitRGB
                                    | VideoFrameRef.copyBitmapOptions.DepthInvert
                                    | VideoFrameRef.copyBitmapOptions.DepthFillShadow
                                    | VideoFrameRef.copyBitmapOptions.DepthHistogramEqualize
                                    | VideoFrameRef.copyBitmapOptions.DepthFillRigthBlack);
                            }
                            catch (Exception)
                            {
                                this.depthBitmap =
                                    frame.toBitmap(
                                        VideoFrameRef.copyBitmapOptions.Force24BitRGB
                                        | VideoFrameRef.copyBitmapOptions.DepthInvert
                                        | VideoFrameRef.copyBitmapOptions.DepthFillShadow
                                        | VideoFrameRef.copyBitmapOptions.DepthHistogramEqualize
                                        | VideoFrameRef.copyBitmapOptions.DepthFillRigthBlack);
                            }
                        }

                        this.BeginInvoke(
                            (Action)delegate
                                {
                                    lock (this.depthBitmap)
                                    {
                                        if (this.p_depth.Image != null)
                                        {
                                            this.p_depth.Image.Dispose();
                                        }

                                        this.p_depth.Image = new Bitmap(this.depthBitmap);
                                        this.p_depth.Refresh();
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
                if (this.selectedDevice != null)
                {
                    this.selectedDevice.Close();
                }
            }
            catch (Exception)
            {
            }

            try
            {
                OpenNI.Shutdown();
            }
            catch (Exception)
            {
            }

            Environment.Exit(0);
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {
            this.HandleError(OpenNI.Initialize());
            OpenNI.onDeviceConnected += this.OpenNI_onDeviceConnectionStateChanged;
            OpenNI.onDeviceDisconnected += this.OpenNI_onDeviceConnectionStateChanged;
            this.UpdateDevicesList();
        }

        private void gen3_AnagComplete(bool Success, object userCode)
        {
            if (userCode is bool && (bool)userCode)
            {
                if (Success)
                {
                    this.but_anag.Enabled = true;
                    this.but_stereo.Enabled = true;
                    this.l_save.Enabled = true;
                    this.l_saveanag.Enabled = true;
                    this.lastStereo = false;
                    this.p_image3d.Image = this.anaggen3.Anaglyph;
                }

                this.cb_devices.Enabled = true;
            }
            else
            {
                if (Success)
                {
                    this.p_depth3d.Image = this.anaggen3.Anaglyph;
                    this.anaggen3.GenerateAnaglyphAsync((userCode as Bitmap[])[0], (userCode as Bitmap[])[1], true);
                }
                else
                {
                    this.but_anag.Enabled = true;
                    this.but_stereo.Enabled = true;
                    this.cb_devices.Enabled = true;
                }
            }
        }

        private void gen3_StereoComplete(bool Success, object userCode)
        {
            if (userCode is bool && (bool)userCode)
            {
                if (Success)
                {
                    this.but_anag.Enabled = true;
                    this.but_stereo.Enabled = true;
                    this.l_save.Enabled = true;
                    this.l_saveanag.Enabled = true;
                    this.lastStereo = true;
                    this.p_image3d.Image = this.strggen3.Stereoscopic_SideBySide;
                }

                this.cb_devices.Enabled = true;
            }
            else
            {
                if (Success)
                {
                    this.p_depth3d.Image = this.strggen3.Anaglyph;
                    this.strggen3.GenerateStereoscopicAsync((userCode as Bitmap[])[0], (userCode as Bitmap[])[1], true);
                }
                else
                {
                    this.but_anag.Enabled = true;
                    this.but_stereo.Enabled = true;
                    this.cb_devices.Enabled = true;
                }
            }
        }

        private void l_save_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!this.lastStereo)
            {
                this.SaveFileDialog.Title = "Save Anaglyph";
                this.SaveFileDialog.FileName = string.Empty;
                this.SaveFileDialog.Filter = "Jpg File|*.jpg|BMP File|*.bmp|PNG File|*.png";
                if (this.SaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    switch (Path.GetExtension(this.SaveFileDialog.FileName).ToLower().Trim())
                    {
                        case "png":
                            this.anaggen3.SaveAnaglyph(this.SaveFileDialog.FileName, ImageFormat.Png);
                            break;
                        case "bmp":
                            this.anaggen3.SaveAnaglyph(this.SaveFileDialog.FileName, ImageFormat.Bmp);
                            break;
                        default:
                            this.anaggen3.SaveAnaglyph(this.SaveFileDialog.FileName, ImageFormat.Jpeg, 100);
                            break;
                    }
                }
            }
            else
            {
                this.SaveFileDialog.Title = "Save StereoScopic";
                this.SaveFileDialog.FileName = string.Empty;
                this.SaveFileDialog.Filter = "Jps File|*.jps";
                if (this.SaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.strggen3.SaveStereoscopic(this.SaveFileDialog.FileName, 100);
                }
            }
        }

        private void l_website_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://falahati.net");
        }

        #endregion

        private void but_saveall_Click(object sender, EventArgs e)
        {
            Bitmap c;
            Bitmap d;
            lock (this.colorBitmap)
            {
                lock (this.depthBitmap)
                {
                    c = this.colorBitmap.Clone(
                        new Rectangle(new Point(0, 0), this.colorBitmap.Size),
                        PixelFormat.Format24bppRgb);
                    d = this.depthBitmap.Clone(
                        new Rectangle(new Point(0, 0), this.depthBitmap.Size),
                        PixelFormat.Format24bppRgb);

                    if (this.colorBitmap.Size.Equals(new Size(1280, 1024))
                        || this.colorBitmap.Size.Equals(new Size(1280, 960)))
                    {
                        c = this.colorBitmap.Clone(
                            new Rectangle(0, this.isUse960asHD ? 0 : 32, 1280, 960),
                            PixelFormat.Format24bppRgb);
                        d = new Bitmap(d, 1280, 960);
                    }
                    else if (!this.colorBitmap.Size.Equals(this.depthBitmap.Size))
                    {
                        c = new Bitmap(this.colorBitmap, 640, 480);
                        d = new Bitmap(d, 640, 480);
                    }
                }
            }

            Bitmap p = d.Clone(new Rectangle(new Point(0, 0), d.Size), PixelFormat.Format24bppRgb);

            this.SaveFileDialog.Title = "Save Image";
            this.SaveFileDialog.FileName = string.Empty;
            this.SaveFileDialog.Filter = "Jpg File|*.jpg|BMP File|*.bmp|PNG File|*.png";
            if (this.SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(this.SaveFileDialog.FileName);
                if (extension != null)
                {
                    switch (extension.ToLower().Trim())
                    {
                        case "png":
                            c.Save(this.SaveFileDialog.FileName, ImageFormat.Png);
                            break;
                        case "bmp":
                            c.Save(this.SaveFileDialog.FileName, ImageFormat.Bmp);
                            break;
                        default:
                            c.Save(
                                    this.SaveFileDialog.FileName,
                                    ImageCodecInfo.GetImageEncoders().First(x => x.FormatID == ImageFormat.Jpeg.Guid),
                                    new EncoderParameters()
                                    {
                                        Param = new[]
                                            {
                                                new EncoderParameter(Encoder.Quality, (long)100)
                                            }
                                    });
                            break;
                    }
                }
            }

            this.SaveFileDialog.Title = "Save Depth";
            this.SaveFileDialog.FileName = string.Empty;
            if (this.SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(this.SaveFileDialog.FileName);
                if (extension != null)
                {
                    switch (extension.ToLower().Trim())
                    {
                        case "png":
                            p.Save(this.SaveFileDialog.FileName, ImageFormat.Png);
                            break;
                        case "bmp":
                            p.Save(this.SaveFileDialog.FileName, ImageFormat.Bmp);
                            break;
                        default:
                            p.Save(
                                    this.SaveFileDialog.FileName,
                                    ImageCodecInfo.GetImageEncoders().First(x => x.FormatID == ImageFormat.Jpeg.Guid),
                                    new EncoderParameters()
                                    {
                                        Param = new[]
                                            {
                                                new EncoderParameter(Encoder.Quality, (long)100)
                                            }
                                    });
                            break;
                    }
                }
            }
        }

        private void l_saveanag_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.SaveFileDialog.FileName = string.Empty;
            this.SaveFileDialog.Filter = "Jpg File|*.jpg|BMP File|*.bmp|PNG File|*.png";
            this.SaveFileDialog.Title = "Save Anaglyph Depth";
            if (this.SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(this.SaveFileDialog.FileName);
                if (extension != null)
                {
                    if (this.lastStereo)
                    {
                        switch (extension.ToLower().Trim())
                        {
                            case "png":
                                this.strggen3.SaveAnaglyph(this.SaveFileDialog.FileName, ImageFormat.Png);
                                break;
                            case "bmp":
                                this.strggen3.SaveAnaglyph(this.SaveFileDialog.FileName, ImageFormat.Bmp);
                                break;
                            default:
                                this.strggen3.SaveAnaglyph(this.SaveFileDialog.FileName, ImageFormat.Jpeg, 100);
                                break;
                        }
                    }
                    else
                    {
                        switch (extension.ToLower().Trim())
                        {
                            case "png":
                                this.p_depth3d.Image.Save(this.SaveFileDialog.FileName, ImageFormat.Png);
                                break;
                            case "bmp":
                                this.p_depth3d.Image.Save(this.SaveFileDialog.FileName, ImageFormat.Bmp);
                                break;
                            default:
                                this.p_depth3d.Image.Save(
                                    this.SaveFileDialog.FileName,
                                    ImageCodecInfo.GetImageEncoders().First(x => x.FormatID == ImageFormat.Jpeg.Guid),
                                    new EncoderParameters()
                                        {
                                            Param = new[]
                                            {
                                                new EncoderParameter(Encoder.Quality, (long)100)
                                            }
                                        });
                                break;
                        }
                    }
                }
            }
        }
    }
}