#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

using UnityEditor.iOS.Xcode;

public class xPostProcessBuild : IPostprocessBuildWithReport
{
	public int callbackOrder => 1;

	public void OnPostprocessBuild(BuildReport report)
	{
		var target = report.summary.platform;
		var buildPath = report.summary.outputPath;

		Debug.Log("xPostProcessBuild OnPostprocessBuild " + target + " " + buildPath);

		if (target == BuildTarget.iOS)
		{
			var pbxProjectPath = PBXProject.GetPBXProjectPath(buildPath);
			var plistPath = Path.Combine(buildPath, "Info.plist");

			var pbxProject = new PBXProject();
			pbxProject.ReadFromString(File.ReadAllText(pbxProjectPath));

			var targetGUID = pbxProject.GetUnityMainTargetGuid();

			//pbxProject.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-weak_framework PhotosUI");
			pbxProject.AddFrameworkToProject(targetGUID, "AdSupport.framework", true);
			//pbxProject.RemoveFrameworkFromProject(targetGUID, "Photos.framework");
			pbxProject.AddCapability(targetGUID, PBXCapabilityType.InAppPurchase);
			pbxProject.AddCapability(targetGUID, PBXCapabilityType.SignInWithApple);
			pbxProject.AddCapability(targetGUID, PBXCapabilityType.PushNotifications);
			pbxProject.WriteToFile(pbxProjectPath);

			var plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			var rootDict = plist.root;

			var NSAppTransportSecurity = rootDict.CreateDict("NSAppTransportSecurity");
			NSAppTransportSecurity.SetBoolean("NSAllowsArbitraryLoads", true);

			var SKAdNetworkItemsArray = rootDict.CreateArray("SKAdNetworkItems");
			SKAdNetworkItemsArray.AddDict().SetString("SKAdNetworkIdentifier", "su67r6k2v3.skadnetwork");

			plist.WriteToFile(plistPath);
		}
	}
}
#endif