; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "WinBLP"
#define MyAppVersion "3.0"
#define MyAppPublisher "Echolocation"
#define MyAppURL "http://echolocation.org.uk"
#define MyAppExeName "WinBLP.exe"
#define MyDistFolder "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Dist 

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{66D60D56-849E-413D-859F-1BD4F8C8EF91}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppPublisher}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\InnoPublish
OutputBaseFilename=WinBLP-Setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\WinBLP.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\BatReferenceXMLFile.xml"; DestDir: "{userappdata}\Echolocation\WinBLP"; Flags: ignoreversion createallsubdirs recursesubdirs uninsneveruninstall
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\BatReferenceXMLFile.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\Release.log.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\WinBLP.application"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\WinBLP.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\WinBLP.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\WinBLP.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\WinBLP.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\WinBLP.vshost.application"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\WinBLP.vshost.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\WinBLP.vshost.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\WinBLP.vshost.exe.manifest"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Documents\Visual Studio 2015\Projects\WinBLP\WinBLP\bin\Release\WinBLP.XML"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyDistFolder}\dotNetFx40_Full_setup.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: not IsRequiredDotNetDetected
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: {tmp}\dotNetFx40_Full_setup.exe; Parameters: "/q:a /c:""install /l /q"""; Check: not IsRequiredDotNetDetected; StatusMsg: Microsoft Framework 4.0 is be�ng installed. Please wait... 
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key: string;
    install, release, serviceCount: cardinal;
    check45, success: boolean;
var reqNetVer : string;
begin
    // .NET 4.5 installs as update to .NET 4.0 Full
    if version = 'v4.5' then begin
        version := 'v4\Full';
        check45 := true;
    end else
        check45 := false;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0/4.5 uses value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 uses additional value Release
    if check45 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= 378389);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;

function IsRequiredDotNetDetected(): Boolean;  
begin
    result := IsDotNetDetected('v4\Full', 0);
end;

function InitializeSetup(): Boolean;
begin
    if not IsDotNetDetected('v4\Full', 0) then begin
        MsgBox('{#MyAppName} requires Microsoft .NET Framework 4.0 Client Profile.'#13#13 'The installer will attempt to install it', mbInformation, MB_OK);        
    end;
    
    result := true;
end;
