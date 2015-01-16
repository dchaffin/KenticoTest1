using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.SiteProvider;
using CMS.UIControls;

/// <summary>
/// Metafile WebDAV control.
/// </summary>
public partial class CMSModules_WebDAV_Controls_MetaFileWebDAVEditControl : WebDAVEditControl
{
    #region "Constructors"

    /// <summary>
    /// Creates instance.
    /// </summary>
    public CMSModules_WebDAV_Controls_MetaFileWebDAVEditControl()
    {
        mControlType = WebDAVControlTypeEnum.MetaFile;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Gets the meta file URL.
    /// </summary>
    protected override string GetUrl()
    {
        return MetaFileURLProvider.GetWebDAVUrl(MetaFileGUID, FileName, SiteName);
    }

    #endregion
}