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

public partial class CMSWebParts_CommunityServices_LinkedIn_LinkedInMemberProfile: CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Public profile URL of LinkedIn member.
    /// </summary>
    public string ProfileUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ProfileUrl"), string.Empty);
        }
        set
        {
            SetValue("ProfileUrl", value);
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


    /// <summary>
    /// Display mode.
    /// </summary>
    public string DisplayMode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisplayMode"), string.Empty);
        }
        set
        {
            SetValue("DisplayMode", value);
        }
    }


    /// <summary>
    /// Behavior.
    /// </summary>
    public string Behavior
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Behavior"), string.Empty);
        }
        set
        {
            SetValue("Behavior", value);
        }
    }


    /// <summary>
    /// Profile name.
    /// </summary>
    public string ProfileName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DataText"), string.Empty);
        }
        set
        {
            SetValue("DataText", value);
        }
    }


    /// <summary>
    /// Show connections.
    /// </summary>
    public bool ShowConnections
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowConnections"), false);
        }
        set
        {
            SetValue("ShowConnections", value);
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
            // Initialize variables
            string src = "http://platform.linkedin.com/in.js";
            string dataFormat = null, dataText = null;
            string apiKey = LinkedInHelper.GetLinkedInApiKey(CMSContext.CurrentSiteName);
            
            // Check optional parameters
            if (DisplayMode.EqualsCSafe("inline"))
            {
                dataFormat = DisplayMode;
            }
            else
            {
                dataFormat = Behavior;
            }

            if (DisplayMode.EqualsCSafe("iconname"))
            {
                dataText = " data-text=\"" + ProfileName + "\"";
            }
            else
            {
                dataText = String.Empty;
            }

            // Build plugin code
            string output = "<div style=\"overflow: hidden; width: {0}px;\"><script src=\"{1}\" type=\"text/javascript\">api_key: {6}</script><script type=\"IN/MemberProfile\" data-id=\"{2}\" data-format=\"{3}\" data-related=\"{4}\"{5}></script></div>";
            ltlPluginCode.Text = String.Format(output, Width.ToString(), src, ProfileUrl, dataFormat, ShowConnections.ToString().ToLowerCSafe(), dataText, apiKey);
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