#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System;


public class AkWwiseMenu_Windows : MonoBehaviour {

	private static AkUnityPluginInstaller_Windows m_installer = new AkUnityPluginInstaller_Windows();

	private static AkUnityIntegrationBuilder_Windows m_rebuilder = new AkUnityIntegrationBuilder_Windows();

	[MenuItem("Wwise/Install Plugins/Windows/Win32/Debug")]
	public static void InstallPlugin_Win32_Debug () {
		m_installer.InstallPluginByArchConfig("Win32", "Debug");
	}

	[MenuItem("Wwise/Install Plugins/Windows/Win32/Profile")]
	public static void InstallPlugin_Win32_Profile () {
		m_installer.InstallPluginByArchConfig("Win32", "Profile");
	}

	[MenuItem("Wwise/Install Plugins/Windows/Win32/Release")]
	public static void InstallPlugin_Win32_Release () {
		m_installer.InstallPluginByArchConfig("Win32", "Release");
	}

	[MenuItem("Wwise/Install Plugins/Windows/x64/Debug")]
	public static void InstallPlugin_x64_Debug () {
		m_installer.InstallPluginByArchConfig("x64", "Debug");
	}

	[MenuItem("Wwise/Install Plugins/Windows/x64/Profile")]
	public static void InstallPlugin_x64_Profile () {
		m_installer.InstallPluginByArchConfig("x64", "Profile");
	}

	[MenuItem("Wwise/Install Plugins/Windows/x64/Release")]
	public static void InstallPlugin_x64_Release () {
		m_installer.InstallPluginByArchConfig("x64", "Release");
	}

	[MenuItem("Wwise/Rebuild Integration/Windows/Win32/Debug")]
	public static void RebuildIntegration_Debug_Win32() {
		m_rebuilder.BuildByConfig("Debug", "Win32");
	}

	[MenuItem("Wwise/Rebuild Integration/Windows/Win32/Profile")]
	public static void RebuildIntegration_Profile_Win32() {
		m_rebuilder.BuildByConfig("Profile", "Win32");
	}

	[MenuItem("Wwise/Rebuild Integration/Windows/Win32/Release")]
	public static void RebuildIntegration_Release_Win32() {
		m_rebuilder.BuildByConfig("Release", "Win32");
	}

	[MenuItem("Wwise/Rebuild Integration/Windows/x64/Debug")]
	public static void RebuildIntegration_Debug_x64() {
		m_rebuilder.BuildByConfig("Debug", "x64");
	}

	[MenuItem("Wwise/Rebuild Integration/Windows/x64/Profile")]
	public static void RebuildIntegration_Profile_x64() {
		m_rebuilder.BuildByConfig("Profile", "x64");
	}

	[MenuItem("Wwise/Rebuild Integration/Windows/x64/Release")]
	public static void RebuildIntegration_Release_x64() {
		m_rebuilder.BuildByConfig("Release", "x64");
	}

	[MenuItem("Wwise/Help")]
	public static void OpenDoc () {
		string docPath = string.Format("{0}/Wwise/Documentation/WwiseUnityIntegration_Standard.html", Application.dataPath);
		FileInfo fi = new FileInfo(docPath);
		if ( ! fi.Exists )
		{
			UnityEngine.Debug.LogError(string.Format("Wwise: Failed to find documentation: {0}. Aborted.", docPath));
			return;
		}

		string docUrl = string.Format("file:///{0}", docPath);
		
		Application.OpenURL(docUrl);
	}

}


public class AkUnityPluginInstaller_Windows : AkUnityPluginInstallerBase
{
	public AkUnityPluginInstaller_Windows()
	{
		m_platform = "Windows";
		m_excludes = new string[] {".pdb", ".exp", ".lib"};
	}
}


public class AkUnityIntegrationBuilder_Windows : AkUnityIntegrationBuilderBase
{
	public AkUnityIntegrationBuilder_Windows()
	{
		m_platform = "Windows";
	}
}
#endif // #if UNITY_EDITOR