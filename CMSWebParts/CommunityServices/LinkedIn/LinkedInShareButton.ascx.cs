using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.MembershipProvider;

public partial class CMSWebParts_CommunityServices_LinkedIn_LinkedInShareButton: CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Url to share.
    /// </summary>
    public string UrlToShare
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UrlToShare"), string.Empty);
        }
        set
        {
            SetValue("UrlToShare", value);
        }
    }


    /// <summary>
    /// Count box position.
    /// </summary>
    public string CountBox
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CountBox"), string.Empty);
        }
        set
        {
            SetValue("CountBox", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 110);
        }
        set
        {
            SetValue("Width", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Build plugin code
            string src = "http://platform.linkedin.com/in.js";
            string apiKey = LinkedInHelper.GetLinkedInApiKey(CMSContext.CurrentSiteName);
            
            if (string.IsNullOrEmpty(UrlToShare))
            {
                UrlToShare = URLHelper.CurrentURL;
            }
            UrlToShare = URLHelper.GetAbsoluteUrl(ResolveUrl(HTMLHelper.HTMLEncode(UrlToShare)));

            string output = "<div style=\"overflow: hidden; width: {0}px;\"><script src=\"{1}\" type=\"text/javascript\">api_key: {4}</script><script type=\"IN/Share\" data-url=\"{2}\" data-counter=\"{3}\"></script></div>";
            ltlButtonCode.Text = String.Format(output, Width.ToString(), src, UrlToShare, CountBox, apiKey);
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}