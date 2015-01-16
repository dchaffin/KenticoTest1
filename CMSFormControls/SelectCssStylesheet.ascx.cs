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
using CMS.UIControls;

public partial class CMSFormControls_SelectCssStylesheet : FormEngineUserControl
{
    #region "Private variables"

    private string mStylesheetCodeName = String.Empty;
    private int mSiteId = 0;
    private bool mAllowEditButtons = true;
    private string mReturnColumnName = "StylesheetName";

    #endregion


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
            EnsureChildControls();
            base.Enabled = value;
            usStyleSheet.Enabled = value;
        }
    }


    /// <summary>
    /// Adds aliaspath parameter to edit url. Used for preview.
    /// </summary>
    public String AliasPath
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether "default" record should be added to the dropdown list.
    /// </summary>
    public bool AddNoneRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("adddefaultrecord"), true);
        }
        set
        {
            SetValue("adddefaultrecord", value);
        }
    }


    /// <summary>
    /// Gets the current UniSelector instance.
    /// </summary>
    public UniSelector CurrentSelector
    {
        get
        {
            EnsureChildControls();
            return usStyleSheet;
        }
    }


    /// <summary>
    /// If true edit buttons are shown.
    /// </summary>
    public bool AllowEditButtons
    {
        get
        {
            return mAllowEditButtons;
        }
        set
        {
            mAllowEditButtons = value;
        }
    }


    /// <summary>
    /// CSS stylesheet code name.
    /// </summary>
    public string StylesheetCodeName
    {
        get
        {
            EnsureChildControls();
            return Convert.ToString(usStyleSheet.Value);
        }
        set
        {
            EnsureChildControls();
            usStyleSheet.Value = value;
        }
    }


    /// <summary>
    /// Gets the current drop down control.
    /// </summary>
    public DropDownList CurrentDropDown
    {
        get
        {
            EnsureChildControls();
            return usStyleSheet.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets or sets stylesheet name.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return Convert.ToString(usStyleSheet.Value);
        }
        set
        {
            EnsureChildControls();
            usStyleSheet.Value = value;
        }
    }


    /// <summary>
    /// Gets ClientID of the dropdown list with stylesheets.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            EnsureChildControls();
            return usStyleSheet.ClientID;
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
            usStyleSheet.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the site id. If set, only stylesheets of the site are displayed.
    /// </summary>
    public int SiteId
    {
        get
        {
            if ((mSiteId == 0) && !String.IsNullOrEmpty(SiteName))
            {
                string siteName = SiteName.ToLowerCSafe();
                if (siteName == "##all##")
                {
                    mSiteId = -1;
                }
                else if (siteName == "##currentsite##")
                {
                    mSiteId = CMSContext.CurrentSiteID;
                }
                else
                {
                    // Get site id from site name if sets.
                    mSiteId = SiteInfoProvider.GetSiteID(siteName);
                }
            }
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name. If set, only stylesheets of the site are displayed.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), String.Empty);
        }
        set
        {
            SetValue("SiteName", value);
        }
    }


    /// <summary>
    /// Name of the column used for return value.
    /// </summary>
    public string ReturnColumnName
    {
        get
        {
            return mReturnColumnName;
        }
        set
        {
            mReturnColumnName = value;
        }
    }


    /// <summary>
    /// Sets a value indicating whether the New button should be enabled.
    /// </summary>
    public bool ButtonNewEnabled
    {
        set
        {
            usStyleSheet.ButtonNewEnabled = value;
        }
    }


    /// <summary>
    /// Sets a value indicating whether the Edit button should be enabled.
    /// </summary>
    public bool ButtonEditEnabled
    {
        set
        {
            usStyleSheet.ButtonEditEnabled = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Add "none record" if required
        if (AddNoneRecord && (usStyleSheet.SpecialFields == null))
        {
            usStyleSheet.SpecialFields = new string[1,2] { { GetString("SelectCssStylesheet.NoneRecord"), String.Empty } };
        }

        // If site specified, restrict to stylesheets assigned to the site
        if (SiteId > 0)
        {
            usStyleSheet.WhereCondition = SqlHelperClass.AddWhereCondition(usStyleSheet.WhereCondition, "StylesheetID IN (SELECT StylesheetID FROM CMS_CssStylesheetSite WHERE SiteID = " + SiteId + ")");
        }

        usStyleSheet.ReturnColumnName = ReturnColumnName;

        // Check if user can edit the stylesheet
        CurrentUserInfo currentUser = CMSContext.CurrentUser;
        bool design = currentUser.IsAuthorizedPerResource("CMS.Design", "Design");
        bool uiElement = currentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "Properties", "Properties.General", "General.Design" }, CMSContext.CurrentSiteName);

        if ((AllowEditButtons) && (design) && (uiElement) && (usStyleSheet.ReturnColumnName.EqualsCSafe("StylesheetID", true)))
        {
            bool uiEditStylesheet = currentUser.IsAuthorizedPerUIElement("CMS.Content", "Design.EditCSSStylesheets", CMSContext.CurrentSiteName);
            bool uiNewStylesheet = currentUser.IsAuthorizedPerUIElement("CMS.Content", "Design.NewCSSStylesheets", CMSContext.CurrentSiteName);
            usStyleSheet.DropDownSingleSelect.CssClass = "SelectorDropDown";

            // Check UI permissions for editing stylesheet
            if (uiEditStylesheet)
            {
                string url = "~/CMSModules/CssStylesheets/Pages/CssStylesheet_General.aspx?editonlycode=true";
                url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash("?editonlycode=true"));
                url = URLHelper.AddParameterToUrl(url, "cssstylesheetid", "##ITEMID##");
                url = URLHelper.AddParameterToUrl(url, "siteid", SiteId.ToString());

                if (!String.IsNullOrEmpty(AliasPath))
                {
                    url = URLHelper.AddParameterToUrl(url, "aliaspath", HttpUtility.UrlEncode(AliasPath));
                }

                usStyleSheet.EditItemPageUrl = url;
            }

            // Check UI permissions for creating stylesheet
            if (uiNewStylesheet)
            {
                string url = "~/CMSModules/CssStylesheets/Pages/CssStylesheet_New.aspx?usedialog=1";

                if (SiteId > 0)
                {
                    url = URLHelper.AddParameterToUrl(url, "siteid", SiteId.ToString());
                }

                if (!String.IsNullOrEmpty(AliasPath))
                {
                    url = URLHelper.AddParameterToUrl(url, "aliaspath", HttpUtility.UrlEncode(AliasPath));
                }

                url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url));
                usStyleSheet.NewItemPageUrl = url;
            }
        }
    }


    /// <summary>
    /// Reloads the selector's data.
    /// </summary>
    /// <param name="forceReload">Indicates whether data should be forcibly reloaded</param>
    public void Reload(bool forceReload)
    {
        usStyleSheet.Reload(forceReload);
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load update panel container
        if (usStyleSheet == null)
        {
            pnlUpdate.LoadContainer();
        }

        // Call base method
        base.CreateChildControls();
    }

    #endregion
}