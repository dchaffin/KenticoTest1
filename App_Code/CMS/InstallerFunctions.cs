using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;

/// <summary>
/// Installer methods.
/// </summary>
public static class InstallerFunctions
{
    #region "Public methods"

    /// <summary>
    /// Redirects user to the installation page if connectionString not set.
    /// </summary>
    /// <param name="forceRedirect">If true, the redirect is forced</param>
    public static bool InstallRedirect(bool forceRedirect)
    {
        // Check if the connection string is initialized
        if (!SqlHelperClass.IsConnectionStringInitialized || forceRedirect)
        {
            string relativePath = GetRelativePath();
            string currentFile = Path.GetFileName(relativePath);

            if (CMSString.Compare(currentFile, "install.aspx", true) == 0)
            {
                return true;
            }

            string fileExtension = Path.GetExtension(currentFile);
            if ((CMSString.Compare(fileExtension, ".aspx", true) == 0) || currentFile == String.Empty || currentFile == "/")
            {
                if (!IsInstallerExcluded(relativePath))
                {
                    if (HttpContext.Current != null)
                    {
                        URLHelper.Redirect("~/cmsinstall/install.aspx");
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Returns true if install is in progress (current page is install.aspx). Otherwise false.
    /// </summary>
    public static bool IsInstallInProgress()
    {
        string currentFile = Path.GetFileName(GetRelativePath());

        if (CMSString.Compare(currentFile, "install.aspx", true) == 0)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Checks if all folders of the given path exist and if not, it creates them.
    /// </summary>
    /// <param name="path">Full disk path</param>
    public static void EnsureDiskPath(string path)
    {
        // Get the folder path
        string folderPath = Path.GetDirectoryName(path);

        // Create the directory
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns true if the path is excluded for the installer process.
    /// </summary>
    /// <param name="path">Path to check</param>
    private static bool IsInstallerExcluded(string path)
    {
        if (path.StartsWithCSafe("/cmsmessages") || path.StartsWithCSafe("/cmspages/getresource.ashx"))
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Gets relative path of the current request path.
    /// </summary>
    private static string GetRelativePath()
    {
        string currentPath = "";

        if (HttpContext.Current != null)
        {
            currentPath = HttpContext.Current.Request.Path.ToLowerCSafe();
        }

        return URLHelper.RemoveApplicationPath(currentPath);
    }

    #endregion
}