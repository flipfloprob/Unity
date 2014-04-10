#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System;

public class AkUnityAssetsInstaller
{
	protected string m_platform = "Undefined";
	protected string m_assetsDir = Application.dataPath;
	protected string m_pluginDir = Path.Combine(Application.dataPath, "Plugins");
	protected string[] m_excludes = new string[] {};

	// Copy file to destination directory and create the directory when none exists.
	public static bool CopyFileToDirectory(string srcFilePath, string destDir)
	{
		FileInfo fi = new FileInfo(srcFilePath);
		if ( ! fi.Exists )
        {
        	UnityEngine.Debug.LogError(string.Format("Wwise: Failed to copy. Source is missing: {0}.", srcFilePath));
    		return false;
        }

		DirectoryInfo di = new DirectoryInfo(destDir);
		
		if ( ! di.Exists )
        {
            di.Create();
        }

        const bool IsToOverwrite = true;
        try
        {
        	fi.CopyTo(Path.Combine(di.FullName, fi.Name), IsToOverwrite);		
    	}
    	catch (Exception ex)
        {
			UnityEngine.Debug.LogError(string.Format("Wwise: Error during installation: {0}.", ex.Message));
			return false;
        }

        return true;
	}

	// Copy or overwrite destination file with source file.
	public static bool OverwriteFile(string srcFilePath, string destFilePath)
	{
		FileInfo fi = new FileInfo(srcFilePath);
		if ( ! fi.Exists )
        {
        	UnityEngine.Debug.LogError(string.Format("Wwise: Failed to overwrite. Source is missing: {0}.", srcFilePath));
    		return false;
        }

		DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(destFilePath));
		
		if ( ! di.Exists )
        {
            di.Create();
        }

        const bool IsToOverwrite = true;
        try
        {
        	fi.CopyTo(destFilePath, IsToOverwrite);		
    	}
    	catch (Exception ex)
        {
			UnityEngine.Debug.LogError(string.Format("Wwise: Error during installation: {0}.", ex.Message));
			return false;
        }

        return true;
	}	

	// Move file to destination directory and create the directory when none exists.
	public static void MoveFileToDirectory(string srcFilePath, string destDir)
	{
		FileInfo fi = new FileInfo(srcFilePath);
		if ( ! fi.Exists )
        {
        	UnityEngine.Debug.LogError(string.Format("Wwise: Failed to move. Source is missing: {0}.", srcFilePath));
    		return;
        }

		DirectoryInfo di = new DirectoryInfo(destDir);
		
		if ( ! di.Exists )
        {
            di.Create();
        }

        string destFilePath = Path.Combine(di.FullName, fi.Name);
        try
        {
        	fi.MoveTo(destFilePath);
        }
        catch (Exception ex)
        {
			UnityEngine.Debug.LogError(string.Format("Wwise: Error during installation: {0}.", ex.Message));
			return;
        }

        return;
	}

	// Recursively copy a directory to its destination.
	public static bool RecursiveCopyDirectory(DirectoryInfo srcDir, DirectoryInfo destDir, string[] excludeExtensions = null)
    {
    	if ( ! srcDir.Exists )
    	{
    		UnityEngine.Debug.LogError(string.Format("Wwise: Failed to copy. Source is missing: {0}.", srcDir));
    		return false;
    	}

        if ( ! destDir.Exists )
        {
            destDir.Create();
        }

        // Copy all files.
        FileInfo[] files = srcDir.GetFiles();
        foreach (FileInfo file in files)
        {
			if (excludeExtensions != null)
			{
				string fileExt = Path.GetExtension(file.Name);
				bool isFileExcluded = false;
				foreach (string ext in excludeExtensions)
				{
					if (fileExt.ToLower() == ext)
					{
						isFileExcluded = true;
						break;
					}
				}
				
				if (isFileExcluded)
				{
					continue;
				}
			}
			
			const bool IsToOverwrite = true;
			try
			{
            	file.CopyTo(Path.Combine(destDir.FullName, file.Name), IsToOverwrite);
            }
            catch (Exception ex)
	        {
				UnityEngine.Debug.LogError(string.Format("Wwise: Error during installation: {0}.", ex.Message));
				return false;
	        }
        }

        // Process subdirectories.
        DirectoryInfo[] dirs = srcDir.GetDirectories();
        foreach (DirectoryInfo dir in dirs)
        {
            // Get destination directory.
            string destFullPath = Path.Combine(destDir.FullName, dir.Name);

            // Recurse
            bool isSuccess = RecursiveCopyDirectory(dir, new DirectoryInfo(destFullPath), excludeExtensions);
            if ( ! isSuccess )
            	return false;
        }

        return true;
    }

}

public class AkUnityPluginInstallerBase : AkUnityAssetsInstaller
{
	private string m_progTitle = "Wwise: Plugin Installation Progress";

	public void InstallPluginByConfig(string config)
	{
		string pluginSrc = GetPluginSrcPathByConfig(config);
		string pluginDest = GetPluginDestPath("");

		string progMsg = string.Format("Installing plugin for {0} ({1}) from {2} to {3}.", m_platform, config, pluginSrc, pluginDest);
		EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 0.5f);

		bool isSuccess = RecursiveCopyDirectory(new DirectoryInfo(pluginSrc), new DirectoryInfo(pluginDest), m_excludes);
		if ( ! isSuccess )
		{
			UnityEngine.Debug.LogError(string.Format("Wwise: Failed to install plugin for {0} ({1}) from {2} to {3}.", m_platform, config, pluginSrc, pluginDest));
			EditorUtility.ClearProgressBar();
			return;
		}
		
		EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 1.0f);
		AssetDatabase.Refresh();

		EditorUtility.ClearProgressBar();
		UnityEngine.Debug.Log(string.Format("Wwise: Plugin for {0} {1} installed from {2} to {3}.", m_platform, config, pluginSrc, pluginDest));
	}

	public virtual void InstallPluginByArchConfig(string arch, string config)
	{
		string pluginSrc = GetPluginSrcPathByArchConfig(arch, config);
		string pluginDest = GetPluginDestPath(arch);

		string progMsg = string.Format("Installing plugin for {0} ({1}, {2}) from {3} to {4}.", m_platform, arch, config, pluginSrc, pluginDest);
		EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 0.5f);

		bool isSuccess = RecursiveCopyDirectory(new DirectoryInfo(pluginSrc), new DirectoryInfo(pluginDest), m_excludes);
		if ( ! isSuccess )
		{
			UnityEngine.Debug.LogError(string.Format("Failed to install plugin for {0} ({1}, {2}) from {3} to {4}.", m_platform, arch, config, pluginSrc, pluginDest));
			EditorUtility.ClearProgressBar();
			return;
		}
		
		EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 1.0f);
		AssetDatabase.Refresh();

		EditorUtility.ClearProgressBar();
		UnityEngine.Debug.Log(string.Format("Wwise: Plugin for {0} {1} {2} installed from {3} to {4}.", m_platform, arch, config, pluginSrc, pluginDest));

	}

	protected string GetPluginSrcPathByConfig(string config)
	{
		return Path.Combine(Path.Combine(Path.Combine(Path.Combine(Path.Combine(m_assetsDir, "Wwise"), "Deployment"), "Plugins"), m_platform), config);
	}

	protected string GetPluginSrcPathByArchConfig(string arch, string config)
	{
		return Path.Combine(Path.Combine(Path.Combine(Path.Combine(Path.Combine(Path.Combine(m_assetsDir, "Wwise"), "Deployment"), "Plugins"), m_platform), arch), config);
	}
	
	protected virtual string GetPluginDestPath(string arch)
	{
		return m_pluginDir;
	}

}
#endif // #if UNITY_EDITOR