SetCompressor /SOLID lzma

!define KRB_URL "http://download.microsoft.com/download/A/3/5/A35E3B22-BE74-40AF-A46B-229E071452C1/KinectRuntime-v1.7-Setup.exe"

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"

   ; include for some of the windows messages defines
   !include "winmessages.nsh"

;--------------------------------
;General
  !include "x64.nsh"
  ;Name and file
  Name "OpenNI 3D Photo Capture Tool for Windows v1.1.0.0"
  OutFile "../OpenNI-3D-Photo-Capture-Tool-Win-v1.1.exe"

  ;Default installation folder
  InstallDir "$PROGRAMFILES\OpenNI 3D Photo Capture Tool"

  ;Get installation folder from registry if available
  InstallDirRegKey HKLM "Software\OpenNI 3D Photo Capture Tool" ""

  ;Request application privileges for Windows Vista
  RequestExecutionLevel admin ;Require admin rights on NT6+ (When UAC is turned on)

  !include LogicLib.nsh

  BrandingText "Soroush Falahati (Falahati.net)"

;--------------------------------
;Interface Configuration

  !define MUI_HEADERIMAGE
  !define MUI_HEADERIMAGE_BITMAP "Setup.bmp" ; optional
  !define MUI_ABORTWARNING

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_LICENSE "GPL.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
    !define MUI_FINISHPAGE_RUN "$INSTDIR\OpenNI 3D Photo Capture Tool.exe"
    !define MUI_FINISHPAGE_RUN_TEXT "Launch OpenNI 3D Photo Capture Tool"

  !insertmacro MUI_PAGE_FINISH

  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES


;--------------------------------
;Languages

  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "Main Application" SecMain
  SectionIn RO
  SetOutPath "$INSTDIR"
  File "GPL.txt"
  File "..\Release\*.ini"
  File "..\Release\*.dll"
  File "..\Release\OpenNI 3D Photo Capture Tool.exe"

  CreateDirectory "$INSTDIR\Primesense Drivers"
  SetOutPath "$INSTDIR\Primesense Drivers"
  File "Primesense Drivers\*"

  CreateDirectory "$INSTDIR\Primesense Drivers\amd64"
  SetOutPath "$INSTDIR\Primesense Drivers\amd64"
  File "Primesense Drivers\amd64\*"

  CreateDirectory "$INSTDIR\Primesense Drivers\x86"
  SetOutPath "$INSTDIR\Primesense Drivers\x86"
  File "Primesense Drivers\x86\*"

  CreateDirectory "$INSTDIR\OpenNI2"
  CreateDirectory "$INSTDIR\OpenNI2\Drivers"
  SetOutPath "$INSTDIR\OpenNI2\Drivers"
  File "..\Release\OpenNI2\Drivers\*"

  ${If} ${RunningX64}
     CreateDirectory "$PROGRAMFILES64\Microsoft SDKs\Kinect\v1.6\Assemblies"
     CreateDirectory "$PROGRAMFILES32\Microsoft SDKs\Kinect\v1.6\Assemblies"
     WriteRegStr HKLM "SOFTWARE\Microsoft\Kinect" "SDKInstallPath" "$PROGRAMFILES64\Microsoft SDKs\Kinect"
     WriteRegStr HKLM "SOFTWARE\Wow6432Node\Microsoft\Kinect" "SDKInstallPath" "$PROGRAMFILES32\Microsoft SDKs\Kinect"
     ExecWait '$INSTDIR\Primesense Drivers\dpinst-amd64.exe /SW /LM'
  ${Else}
     CreateDirectory "$PROGRAMFILES\Microsoft SDKs\Kinect\v1.6\Assemblies"
     WriteRegStr HKLM "SOFTWARE\Microsoft\Kinect" "SDKInstallPath" "$PROGRAMFILES\Microsoft SDKs\Kinect"
     ExecWait '$INSTDIR\Primesense Drivers\dpinst-x86.exe /SW /LM'
  ${EndIf}
  SetOutPath "$INSTDIR"
  ;Store installation folder
  WriteRegStr HKLM "Software\OpenNI 3D Photo Capture Tool" "InstallDir" $INSTDIR
  WriteRegStr HKLM "Software\OpenNI 3D Photo Capture Tool" "Version" "1.1.0"

  CreateDirectory "$SMPROGRAMS\OpenNI 3D Photo Capture Tool"
  CreateShortCut "$SMPROGRAMS\OpenNI 3D Photo Capture Tool\OpenNI 3D Photo Capture Tool.lnk" "$INSTDIR\OpenNI 3D Photo Capture Tool.exe"
  CreateShortCut "$SMPROGRAMS\OpenNI 3D Photo Capture Tool\Uninstall.lnk" "$INSTDIR\Uninstall OpenNI 3D Photo Capture Tool.exe"

  ;Create uninstaller
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\OpenNI 3D Photo Capture Tool 1.1 for Windows" "DisplayName" "OpenNI 3D Photo Capture Tool 1.1 for Windows (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\OpenNI 3D Photo Capture Tool 1.1 for Windows" "UninstallString" "$INSTDIR\Uninstall OpenNI 3D Photo Capture Tool.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\OpenNI 3D Photo Capture Tool 1.1 for Windows" "Publisher" "Soroush Falahati (falahati.net)"

  WriteUninstaller "$INSTDIR\Uninstall OpenNI 3D Photo Capture Tool.exe"


SectionEnd
Function GetKRB
        StrCpy $2 "$TEMP\Kinect Runtime Binaries 1.7.exe"
        nsisdl::download /TIMEOUT=30000 ${KRB_URL} $2
        Pop $R0 ;Get the return value
                StrCmp $R0 "success" +3 0
                MessageBox MB_OK|MB_ICONEXCLAMATION "Download Failed: $R0"
		Goto +2
        ExecWait $2
	Delete $2
FunctionEnd
Section "Microsoft Kinect Drivers" SecKinect
	CALL GetKRB
SectionEnd
Function .onInit
	SectionSetSize "${SecKinect}" 225280
	UserInfo::GetAccountType
	pop $0
	${If} $0 != "admin" ;Require admin rights on NT4+
		MessageBox mb_iconstop "Administrator rights required!"
		SetErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
		Quit
	${EndIf}
FunctionEnd
;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecMain ${LANG_ENGLISH} "OpenNI 3D Photo Capture Tool along with Primesense Sensor and Asus Xtion Drivers."
  LangString DESC_SecKinect ${LANG_ENGLISH} "Download and Install Microsoft Kinect Runtime 1.7 including Kinect for Windows and Kinect for Xbox Drivers."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecMain} $(DESC_SecMain)
    !insertmacro MUI_DESCRIPTION_TEXT ${SecKinect} $(DESC_SecKinect)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"
  RMDir /r "$INSTDIR\*.*"
  RMDir "$INSTDIR"
  Delete "$SMPROGRAMS\OpenNI 3D Photo Capture Tool\*.*"
  RmDir  "$SMPROGRAMS\OpenNI 3D Photo Capture Tool"


  DeleteRegKey /ifempty HKLM "Software\OpenNI 3D Photo Capture Tool"
  DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\OpenNI 3D Photo Capture Tool 1.1 for Windows"
SectionEnd