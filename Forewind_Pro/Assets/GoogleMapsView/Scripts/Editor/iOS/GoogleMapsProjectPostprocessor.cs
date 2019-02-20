#pragma warning disable 0162

namespace DeadMosquito.IosGoodies.Editor
{
	using System;
	using System.IO;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode.GoogleMapsView;
	using UnityEngine;

	/// <summary>
	/// See https://developers.google.com/maps/documentation/ios-sdk/start
	/// </summary>
	public static class GoogleMapsProjectPostprocessor
	{
		[PostProcessBuild(2000)]
		public static void OnPostProcessBuild(BuildTarget target, string path)
		{
			if (target == BuildTarget.iOS)
			{
#if DISABLE_IOS_GOOGLE_MAPS
				Debug.LogWarning("Google Maps View for iOS is disabled, skipping iOS Project postprocessing");
				return;
#endif
				
				Debug.Log("Google Maps View is now postprocessing iOS Project");
				string projectPath = PBXProject.GetPBXProjectPath(path);
				PBXProject project = new PBXProject();

				project.ReadFromString(File.ReadAllText(projectPath));
				string targetName = PBXProject.GetUnityTargetName();
				string targetGUID = project.TargetGuidByName(targetName);

				AddFrameworks(project, targetGUID);
				
#if UNITY_2018_3_OR_NEWER
				AddGoogleMapsBundleToProjectResources(path, project, targetGUID, "Frameworks/GoogleMaps.framework/Resources/GoogleMaps.bundle", "GoogleMaps.bundle");
#else
				AddGoogleMapsBundleToProjectResources(path, project, targetGUID, "Frameworks/Plugins/iOS/GoogleMaps.framework/Resources/GoogleMaps.bundle", "GoogleMaps.bundle");
#endif
				// Remove the line below if you don't use places API
				AddPlacesBundlesToProjects(path, project, targetGUID);

				File.WriteAllText(projectPath, project.WriteToString());

				ModifyPlist(path, AddLocationPrivacyEntry);

				Debug.Log("Google Maps View has finished postprocessing iOS Project");
			}
		}

		static void AddPlacesBundlesToProjects(string path, PBXProject project, string targetGUID)
		{
#if UNITY_2018_3_OR_NEWER
			AddGoogleMapsBundleToProjectResources(path, project, targetGUID, "Frameworks/GooglePlacePicker.framework/Resources/GooglePlacePicker.bundle", "GooglePlacePicker.bundle");
			AddGoogleMapsBundleToProjectResources(path, project, targetGUID, "Frameworks/GooglePlaces.framework/Resources/GooglePlaces.bundle", "GooglePlaces.bundle");
#else
			AddGoogleMapsBundleToProjectResources(path, project, targetGUID, "Frameworks/Plugins/iOS/GooglePlacePicker.framework/Resources/GooglePlacePicker.bundle", "GooglePlacePicker.bundle");
			AddGoogleMapsBundleToProjectResources(path, project, targetGUID, "Frameworks/Plugins/iOS/GooglePlaces.framework/Resources/GooglePlaces.bundle", "GooglePlaces.bundle");
#endif
		}

		static void AddGoogleMapsBundleToProjectResources(string path, PBXProject project, string targetGUID, string sourcePath, string destPath)
		{
			var resourceBundlePath = Path.Combine(path, sourcePath);
			var addFolderReference = project.AddFolderReference(resourceBundlePath, destPath, PBXSourceTree.Absolute);
			project.AddFileToBuild(targetGUID, addFolderReference);
		}

		static void AddFrameworks(PBXProject project, string targetGuid)
		{
			project.AddFrameworkToProject(targetGuid, "libc++.tbd", false);
			project.AddFrameworkToProject(targetGuid, "libz.tbd", false);
			project.AddFrameworkToProject(targetGuid, "Accelerate.framework", false);
			project.AddFrameworkToProject(targetGuid, "CoreData.framework", false);
			project.AddFrameworkToProject(targetGuid, "CoreImage.framework", false);
			project.AddFrameworkToProject(targetGuid, "CoreGraphics.framework", false);
			project.AddFrameworkToProject(targetGuid, "CoreLocation.framework", false);
			project.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", false);
			project.AddFrameworkToProject(targetGuid, "CoreText.framework", false);
			project.AddFrameworkToProject(targetGuid, "GLKit.framework", false);
			project.AddFrameworkToProject(targetGuid, "ImageIO.framework", false);
			project.AddFrameworkToProject(targetGuid, "OpenGLES.framework", false);
			project.AddFrameworkToProject(targetGuid, "QuartzCore.framework", false);
			project.AddFrameworkToProject(targetGuid, "SystemConfiguration.framework", false);
			project.AddFrameworkToProject(targetGuid, "UIKit.framework", false);
			project.AddFrameworkToProject(targetGuid, "Security.framework", false);

			// Add `-ObjC` to "Other Linker Flags".
			project.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
		}

		static void ModifyPlist(string projectPath, Action<PlistDocument> modifier)
		{
			try
			{
				var plistInfoFile = new PlistDocument();

				var infoPlistPath = Path.Combine(projectPath, "Info.plist");
				plistInfoFile.ReadFromString(File.ReadAllText(infoPlistPath));

				modifier(plistInfoFile);

				File.WriteAllText(infoPlistPath, plistInfoFile.WriteToString());
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		static bool HasRootElement(this PlistDocument plist, string key)
		{
			return plist.root.values.ContainsKey(key);
		}

		static void AddLocationPrivacyEntry(PlistDocument plist)
		{
			const string NSLocationWhenInUseUsageDescription = "NSLocationWhenInUseUsageDescription";

			if (!plist.HasRootElement(NSLocationWhenInUseUsageDescription))
			{
				plist.root.AsDict().SetString(NSLocationWhenInUseUsageDescription, "Plist entry Added by GoogleMapsProjectPostprocessor.cs script");
			}
		}
	}
}

#pragma warning restore 0162