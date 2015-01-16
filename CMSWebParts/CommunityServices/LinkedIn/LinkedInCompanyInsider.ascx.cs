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

public partial class CMSWebParts_CommunityServices_LinkedIn_LinkedInCompanyInsider: CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Company ID.
    /// </summary>
    public string CompanyID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CompanyID"), string.Empty);
        }
        set
        {
            SetValue("CompanyID", value);
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
    /// Defines whether to show user's network.
    /// </summary>
    public bool ShowNetwork
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowNetwork"), false);
        }
        set
        {
            SetValue("ShowNetwork", value);
        }
    }


    /// <summary>
    /// Defines whether to show new hires.
    /// </summary>
    public bool ShowNewHires
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowNewHires"), false);
        }
        set
        {
            SetValue("ShowNewHires", value);
        }
    }


    /// <summary>
    /// Defines whether to show promotions and changes.
    /// </summary>
    public bool ShowPromotions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPromotions"), false);
        }
        set
        {
            SetValue("ShowPromotions", value);
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
            string dataModules = String.Empty;
            string apiKey = LinkedInHelper.GetLinkedInApiKey(CMSContext.CurrentSiteName);
            
            //Check optional parameters
            if (ShowNetwork)
            {
                dataModules += ",innetwork";
            }

            if (ShowNewHires)
            {
                dataModules += ",newhires";
            }

            if (ShowPromotions)
            {
                dataModules += ",jobchanges";
            }

            // Build plugin code
            string output = "<div style=\"overflow: hidden; width: {0}px;\"><script src=\"{1}\" type=\"text/javascript\">api_key: {4}</script><script type=\"IN/CompanyInsider\" data-id=\"{2}\" data-modules=\"{3}\"></script></div>";
            ltlPluginCode.Text = String.Format(output, Width.ToString(), src, CompanyID, dataModules.TrimStart(','), apiKey);
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