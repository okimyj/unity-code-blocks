using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;

using UnityEngine;


#if UNITY_EDITOR_OSX || UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif      //#if UNITY_EDITOR_OSX || UNITY_IOS

using Debug = UnityEngine.Debug;

/*
 https://docs.unity3d.com/kr/2019.3/ScriptReference/iOS.Xcode.PBXProject.html

https://xcodebuildsettings.com/ -xcode 세팅쪽 참고서
 */


public partial class DayBrixPostBuildProcessor
{
    private static string DayBrixTeamId = "7557U36CG5";
    static string TeamId
    {
        get
        {
            if (string.IsNullOrEmpty(PlayerSettings.iOS.appleDeveloperTeamID))
            {
                return DayBrixTeamId;
            }
            else
            {
                return PlayerSettings.iOS.appleDeveloperTeamID;
            }
        }
    }
    static string projectPath = null;
    static string plistPath = null;


#if UNITY_EDITOR_OSX || UNITY_IOS
    private static void PostBuild_iOS(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) return;

        Debug.Log("IOS 빌드 PostProcess 실행합니다.");
        projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);

        plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        
        ModifyUnityUnity_iPhone(ref pbxProject, pathToBuiltProject);
        
        ModifyNaverSDK(ref pbxProject, ref plist, pathToBuiltProject);

        ModifyUnityFramework(ref pbxProject);
        
        ModifyCapability(ref pbxProject);
        
        ModifyNativeGallery(ref pbxProject, ref plist);

        WriteExportPListSetting(pathToBuiltProject, "development");
        WriteExportPListSetting(pathToBuiltProject, "ad-hoc");
        WriteExportPListSetting(pathToBuiltProject, "app-store");

        // 기본 언어 설정 (영어)
        plist.root.SetString("CFBundleDevelopmentRegion", "en");
        ModifyLocalizations(ref pbxProject, pathToBuiltProject);

        // InfoPlist.strings 파일 활성화
        var localizedResources = GetOrCreatePlistArray(plist.root, "CFBundleLocalizations");
        localizedResources.AddString("en"); // 영어
        localizedResources.AddString("ko"); // 한국어    

        // AppSealing 관련 PrivacyInfo setting.
        ModifyPrivacyInfo(ref pbxProject, pathToBuiltProject);

        pbxProject.WriteToFile(projectPath);
        
        plist.WriteToFile(plistPath);

        Debug.Log("IOS Build 성공!");
        Debug.Log("ios projected path : " + pathToBuiltProject);
    }

    

    /// <summary>
    /// Modify Iphone Settings
    /// </summary>
    /// <param name="pbxProject"></param>
    public static void ModifyUnityUnity_iPhone(ref PBXProject pbxProject, string xcodeProjectPath)
    {
        Debug.Log("ModifyUnityUnity_iPhone - ");
        string targetGuid = pbxProject.GetUnityMainTargetGuid();
        

        string targetGuid2 = pbxProject.TargetGuidByName("UnityFramework");
        pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
        pbxProject.SetBuildProperty(targetGuid, "VALIDATE_WORKSPACE", "YES");
        pbxProject.SetBuildProperty(targetGuid, "DEVELOPMENT_TEAM", "DayBrix INC");
        

        string token = pbxProject.GetBuildPropertyForAnyConfig(targetGuid, "USYM_UPLOAD_AUTH_TOKEN");
        if (string.IsNullOrEmpty(token)) token = "FAKE_TOKEN";

        pbxProject.SetBuildProperty(targetGuid, "USYM_UPLOAD_AUTH_TOKEN", token);
        pbxProject.SetBuildProperty(targetGuid2, "USYM_UPLOAD_AUTH_TOKEN", token);
        pbxProject.SetBuildProperty(pbxProject.ProjectGuid(), "USYM_UPLOAD_AUTH_TOKEN", token);
        pbxProject.SetTeamId(targetGuid, TeamId);

        pbxProject.WriteToFile(projectPath);
        Debug.Log("ModifyUnityUnity_iPhone done.");
    }


    public static void ModifyCapability(ref PBXProject pbxProject)
    {
        Debug.Log("ModifyCapability - ");
        var entitlementsPath = "Entitlements.entitlements";
        var capabilityMgr = new ProjectCapabilityManager(projectPath, entitlementsPath, null, pbxProject.GetUnityMainTargetGuid());
        //manager.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
        capabilityMgr.AddInAppPurchase();
        capabilityMgr.AddSignInWithApple();
        capabilityMgr.AddPushNotifications(true);
#if DEVENV_DEV || DEVENV_QA
        capabilityMgr.Entitlements.root.SetString("aps-environment", "development");
#else
        capabilityMgr.Entitlements.root.SetString("aps-environment", "production");
#endif
        capabilityMgr.WriteToFile();
        var targetGuid = pbxProject.GetUnityMainTargetGuid();
        pbxProject.AddBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", entitlementsPath);
        pbxProject.WriteToFile(projectPath);
    }

    public static void ModifyUnityFramework(ref PBXProject pbxProject)
    {
        Debug.Log("ModifyUnityFramework - ");
        string targetGuid = pbxProject.GetUnityFrameworkTargetGuid();
        pbxProject.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
        pbxProject.SetBuildProperty(targetGuid, "VALIDATE_WORKSPACE", "YES");
        pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
        pbxProject.AddFrameworkToProject(targetGuid, "GameKit.framework", false);
        pbxProject.AddFrameworkToProject(targetGuid, "AuthenticationServices.framework", false);
        
    }

    private static void ModifyNativeGallery(ref PBXProject pbxProject, ref PlistDocument plist)
    {
        Debug.Log("ModifyNativeGallery - ");
        string targetGuid = pbxProject.GetUnityFrameworkTargetGuid();
        pbxProject.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-weak_framework PhotosUI");
        pbxProject.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-framework Photos");
        pbxProject.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-framework MobileCoreServices");
        pbxProject.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-framework ImageIO");
        
        PlistElementDict rootDict = plist.root;
        
        // 게임모드 설정
        rootDict.SetString("LSApplicationCategoryType", "public.app-category.action-games");
        rootDict.SetBoolean("GCSupportsGameMode", true);
        

        #region URL Types Settings. (Google Login)
        // URL Types 찾기/추가 
        PlistElementArray urlTypes;
        if (rootDict.values.ContainsKey("CFBundleURLTypes"))
        {
            urlTypes = rootDict["CFBundleURLTypes"].AsArray();
        }
        else
        {
            urlTypes = rootDict.CreateArray("CFBundleURLTypes");
        }
        // url scheme 추가
        PlistElementDict urlDict = urlTypes.AddDict();
        urlDict.SetString("CFBundleURLName", "GoogleLogin");
        PlistElementArray urlSchemes = urlDict.CreateArray("CFBundleURLSchemes");
        urlSchemes.AddString("com.googleusercontent.apps.172871007108-6enol5887pq3bidd8u045crfmbpkfimr"); // google cloud console의 ios 클라이언트 id.
        #endregion
        // 수출 규정 관련 문서가 누락됨 처리. 
        rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

        #region ironSource Setting.
        // rootDict.values.Add("NSAdvertisingAttributionReportEndpoint", "https://postbacks-is.com");
        rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://postbacks-is.com");
        // NSAppTransportSecurity 
        PlistElementDict atsDict = GetOrCreatePlistDict(rootDict, "NSAppTransportSecurity");
        // NSAllowsArbitraryLoads 설정 추가
        atsDict.SetBoolean("NSAllowsArbitraryLoads", true);
        PlistElementArray sdkAdNetworkItems = GetOrCreatePlistArray(rootDict, "SDKAdNetworkItems");
        sdkAdNetworkItems.AddDict();
        sdkAdNetworkItems.values[0].AsDict().SetString("SKAdNetworkIdentifier", "su67r6k2v3.skadnetwork");
        #endregion
    }

    public static void ModifyNaverSDK(ref PBXProject pbxProject, ref PlistDocument plist, string xcodeProjectPath)
    {
        Debug.Log("ModifyNaverSDK - start");
        string mainTargetGuid = pbxProject.GetUnityMainTargetGuid();
        var unityFrameworkPath = "UnityFramework.framework";
        // naver 라운지 버그 때문에 한번 뺐다가 다시 넣는다.
        if (pbxProject.ContainsFramework(mainTargetGuid, unityFrameworkPath))
        {
            pbxProject.RemoveFrameworkFromProject(mainTargetGuid, unityFrameworkPath);
        }
        pbxProject.AddFrameworkToProject(mainTargetGuid, unityFrameworkPath, true);

        // NaverLogin xcframework 추가
        string xcFrameworkPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../../external-lib/iOS/NaverLogin/NaverThirdPartyLogin.xcframework"));
        if (Directory.Exists(xcFrameworkPath))
        {
            string frameworkFolder = "Frameworks/NaverLogin";
            string destinationFolder = Path.Combine(xcodeProjectPath, frameworkFolder);
            string destinationPath = Path.Combine(destinationFolder, "NaverThirdPartyLogin.xcframework");

            Directory.CreateDirectory(destinationFolder);
            DirectoryCopy(xcFrameworkPath, destinationPath);

            string relativePath = Path.Combine(frameworkFolder, "NaverThirdPartyLogin.xcframework");

            string fileGuid = pbxProject.AddFile(relativePath, relativePath);
            pbxProject.AddFileToEmbedFrameworks(mainTargetGuid, fileGuid, mainTargetGuid);
            string linkPhaseGuid = pbxProject.GetFrameworksBuildPhaseByTarget(mainTargetGuid);
            pbxProject.AddFileToBuildSection(mainTargetGuid, linkPhaseGuid, fileGuid);

            // Set FRAMEWORK_SEARCH_PATHS
            pbxProject.AddBuildProperty(mainTargetGuid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks/NaverLogin");
            pbxProject.AddBuildProperty(mainTargetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            Debug.Log("NaverThirdPartyLogin.xcframework successfully added to Xcode project.");
        }
        else
        {
            Debug.LogError($"NaverThirdPartyLogin.xcframework not found at path: {xcFrameworkPath}");
        }
        
        string frameworkTargetGuid = pbxProject.GetUnityFrameworkTargetGuid();
        pbxProject.AddBuildProperty(frameworkTargetGuid, "OTHER_LDFLAGS", "-ObjC");
        pbxProject.AddBuildProperty(frameworkTargetGuid, "OTHER_LDFLAGS", "-lc++");
        pbxProject.AddFrameworkToProject(frameworkTargetGuid, "SafariServices.framework", false);
        PlistElementDict rootDict = plist.root;
        #region NaverGame Setting
        rootDict.SetString("NSCameraUsageDescription", "This app requires access to the camera for capturing photos and videos.");
        rootDict.SetString("NSPhotoLibraryUsageDescription", "This app requires access to the photo library to save and share images.");
        #endregion

        pbxProject.WriteToFile(projectPath);
        Debug.Log("ModifyNaverSDK - done");
    }

    private static void WriteExportPListSetting(string pathToBuiltProject, string _method_name)
    {
        var targetPath = $"{pathToBuiltProject}/{_method_name}-export.plist";
        System.IO.File.WriteAllText(targetPath, @$" 
                    <?xml version=""1.0"" encoding=""UTF-8""?>
                    <!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
                    <plist version=""1.0"">
                    <dict>
                    <key>method</key>
                    <string>{_method_name}</string>
                    </dict>
                    </plist> 
                    ");
    }

    private static void ModifyLocalizations(ref PBXProject pbxProject, string xcodeProjectPath)
    {
        Debug.Log("PostBuildProcessor_iOS::ModifyLocalizations - ");
        // Unity Plugins/iOS 폴더 경로
        string sourcePath = Path.Combine(Application.dataPath, "Plugins/iOS");

        if (!Directory.Exists(sourcePath))
        {
            Debug.LogWarning($"Localization source path does not exist: {sourcePath}");
            return;
        }

        string targetGuid = pbxProject.GetUnityMainTargetGuid(); // Unity-iPhone 타겟 GUID

        try
        {
            // Plugins/iOS 안의 모든 *.lproj 디렉토리 복사 및 프로젝트에 등록
            var directories = Directory.GetDirectories(sourcePath, "*.lproj");
            foreach (string directory in directories)
            {
                string folderName = Path.GetFileName(directory); // 예: "en.lproj"
                string destinationPath = Path.Combine(xcodeProjectPath, folderName);

                DirectoryCopy(directory, destinationPath);

                var files = Directory.GetFiles(destinationPath);
                if (files.Length <= 0)
                {
                    Debug.Log($"No files found in localization folder : {folderName}");
                }
                Debug.Log($"DirectoryCopy success. folderName : {folderName} files : {files.Length}.. pbxProject is null? {pbxProject == null}");
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string relativePath = Path.Combine(folderName, fileName);
                    string fileGuid = pbxProject.AddFile(file, relativePath);
                    pbxProject.AddFileToBuild(targetGuid, fileGuid);

                    Debug.Log($"Added file {fileName} relativePath : {relativePath}");
                }

            }

            Debug.Log("Localization files copied and added to Xcode project successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error processing localization files: {ex.Message}");
        }
    }

    private static void AddFileToLocalizationGroup(ref PBXProject pbxProject, string fileGuid, string folderName)
    {
        Debug.Log($"AddFileToLocalizationGroup - fileGuid : {fileGuid} folderName : {folderName} pbxProject is null? {pbxProject == null}");
        // Localization 그룹에 파일을 등록하기 위한 설정
        string languageCode = folderName.Replace(".lproj", ""); // "en.lproj" -> "en"

        // 파일에 현지화 속성 추가
        var guid = pbxProject.GetUnityMainTargetGuid();
        var configName = "INFOPLIST_KEY_LOCALIZATION";
        var property = pbxProject.GetBuildPropertyForConfig(guid, configName);
        Debug.Log($"AddFileToLocalizationGroup - 1 {pbxProject.GetUnityMainTargetGuid()} languageCode : {languageCode} guid : {guid} property : {property}");
        if (string.IsNullOrEmpty(property))
        {
            pbxProject.AddBuildPropertyForConfig(guid, configName, languageCode);
        }
        else
        {
            pbxProject.SetBuildPropertyForConfig(guid, configName, languageCode);
        }

        Debug.Log($"AddFileToLocalizationGroup - 2");
    }

    private static void ModifyPrivacyInfo(ref PBXProject pbxProject, string xcodeProjectPath)
    {
        // PrivacyInfo.xcprivacy 파일 경로 설정
        var privacyFileName = "PrivacyInfo.xcprivacy";
        string privacyInfoPath = Path.Combine(xcodeProjectPath, privacyFileName);
        CreatePrivacyInfoFile(privacyInfoPath);
        var targetGuid = pbxProject.GetUnityMainTargetGuid();
        pbxProject.AddFileToBuild(targetGuid, pbxProject.AddFile(privacyInfoPath, privacyFileName));
        
    }
    private static void CreatePrivacyInfoFile(string filePath)
    {
        // PrivacyInfo.xcprivacy 파일 생성. For Appsealing.
        var content = @$"<?xml version=""1.0"" encoding=""UTF-8""?>
        <!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
        <plist version=""1.0"">
        <dict>
            <key>NSPrivacyAccessedAPITypes</key>
            <array>
                <dict>
                    <key>NSPrivacyAccessedAPIType</key>
                    <string>NSPrivacyAccessedAPICategoryFileTimestamp</string>
                    <key>NSPrivacyAccessedAPITypeReasons</key>
                    <array>
                        <string>C617.1</string>
                    </array>
                </dict>
            </array>
        </dict>
        </plist>";

        File.WriteAllText(filePath, content);
    }

    private static PlistElementArray GetOrCreatePlistArray(PlistElementDict root, string key)
    {
        if (root.values.ContainsKey(key))
            return root[key].AsArray();
        else
            return root.CreateArray(key);
    }
    private static PlistElementDict GetOrCreatePlistDict(PlistElementDict root, string key)
    {
        if (root.values.ContainsKey(key))
            return root[key].AsDict();
        else
        {
            return root.CreateDict(key);
        }
    }
    private static void DirectoryCopy(string sourceDir, string destDir)
    {
    Debug.Log($"DirectoryCopy - sourceDir : {sourceDir} destDir : {destDir}");
        var dir = new DirectoryInfo(sourceDir);
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");
        }

        Directory.CreateDirectory(destDir);
        foreach (FileInfo file in dir.GetFiles())
        {
            string tempPath = Path.Combine(destDir, file.Name);
            file.CopyTo(tempPath, false);
        }

        foreach (DirectoryInfo subdir in dir.GetDirectories())
        {
            string tempPath = Path.Combine(destDir, subdir.Name);
            DirectoryCopy(subdir.FullName, tempPath);
        }
    }
#endif      //#if UNITY_EDITOR_OSX || UNITY_IOS
}