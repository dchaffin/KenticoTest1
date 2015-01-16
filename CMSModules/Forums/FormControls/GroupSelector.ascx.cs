using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Forums_FormControls_GroupSelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            selectGroup.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return selectGroup.Value;
        }
        set
        {
            selectGroup.Value = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            selectGroup.IsLiveSite = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (DependsOnAnotherField)
        {
            selectGroup.DependsOnAnotherField = DependsOnAnotherField;
            selectGroup.SiteId = GetFormSiteId();
            selectGroup.Form = Form;
        }
    }


    /// <summary>
    /// Gets the SiteId if the SiteName field is available in the form.
    /// </summary>
    private int GetFormSiteId()
    {
        if (DependsOnAnotherField
            && (Form != null)
            && Form.IsFieldAvailable("SiteName"))
        {
            string siteName = ValidationHelper.GetString(Form.GetFieldValue("SiteName"), "");
            if (String.IsNullOrEmpty(siteName) || siteName.EqualsCSafe("##all##", true))
            {
                return -1;
            }
            else if (siteName.EqualsCSafe("##currentsite##", true))
            {
                siteName = CMSContext.CurrentSiteName;
            }

            SiteInfo siteObj = SiteInfoProvider.GetSiteInfo(siteName);
            if (siteObj != null)
            {
                return siteObj.SiteID;
            }
        }

        return CMSContext.CurrentSiteID;
    }
}