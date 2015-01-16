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
using CMS.EmailEngine;

public partial class CMSModules_EmailTemplates_FormControls_EmailTemplateSelector : FormEngineUserControl
{
    #region "Private variables"

    private int mSiteId = 0;
    private string mReturnColumnName = "EmailTemplateName";
    private string mNewDialogUrl = "~/CMSModules/EmailTemplates/Pages/Edit.aspx";
    private string mEditDialogUrl = "~/CMSModules/EmailTemplates/Pages/Frameset.aspx";

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
            usTemplate.Enabled = value;
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
            return usTemplate;
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
            return usTemplate.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets or sets template name.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return usTemplate.Value;
        }
        set
        {
            EnsureChildControls();
            usTemplate.Value = value;
        }
    }


    /// <summary>
    /// Gets ClientID of the dropdownlist with stylesheets.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            EnsureChildControls();
            return usTemplate.ClientID;
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
            usTemplate.IsLiveSite = value;
        }
    }


    /// <summary>
    /// E-mail template type. (null represents all types)
    /// </summary>
    public EmailTemplateMacros.EmailType? TemplateType
    {
        get
        {
            EmailTemplateMacros.EmailType? emailType = GetValue("TemplateType") as EmailTemplateMacros.EmailType?;
            if(emailType.HasValue)
            {
                return emailType.Value;
            }

            string val = ValidationHelper.GetString(GetValue("TemplateType"), null);
            if (val != null)
            {
                return EmailTemplateMacros.GetEmailTypeEnum(val);
            }

            return null;
        }
        set
        {
            SetValue("TemplateType", value);
        }
    }

    /// <summary>
    /// Gets or sets the site id. If set, only templates of the site are displayed.
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
    /// Gets or sets the site name. If set, only templates of the site are displayed.
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
    /// URL of creation dialog for uniselector.
    /// </summary>
    public string NewDialogUrl
    {
        get
        {
            return mNewDialogUrl;
        }
        set
        {
            mNewDialogUrl = value;
        }
    }


    /// <summary>
    /// URL of edit dialog for uniselector.
    /// </summary>
    public string EditDialogUrl
    {
        get
        {
            return mEditDialogUrl;
        }
        set
        {
            mEditDialogUrl = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        usTemplate.WhereCondition = SqlHelperClass.AddWhereCondition(usTemplate.WhereCondition, (SiteId > 0) ? "EmailTemplateSiteID = " + SiteId : "EmailTemplateSiteID IS NULL");

        // Filter type
        if (TemplateType.HasValue)
        {
            usTemplate.WhereCondition = SqlHelperClass.AddWhereCondition(usTemplate.WhereCondition, "EmailTemplateType = '" + SqlHelperClass.GetSafeQueryString(EmailTemplateMacros.GetEmailTypeString(TemplateType.Value)) + "'");
        }

        // Edit button initialization
        if (CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.EmailTemplates", "Modify"))
        {
            string templateType = null;
            if (TemplateType != null)
            {
                templateType = "&templatetype=" + EmailTemplateMacros.GetEmailTypeString(TemplateType.Value);
            }

            if (!String.IsNullOrEmpty(EditDialogUrl))
            {
                string url = EditDialogUrl + "?name=##ITEMID##&tabmode=1&editonlycode=1" + templateType;
                url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash("?editonlycode=1" + templateType));
                usTemplate.EditItemPageUrl = url;
            }

            if (!String.IsNullOrEmpty(NewDialogUrl))
            {
                string url = NewDialogUrl + "?editonlycode=1" + templateType;
                url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash("?editonlycode=1" + templateType));
                usTemplate.NewItemPageUrl = url;
            }
        }

        usTemplate.ButtonClear.Visible = false;
        usTemplate.ReturnColumnName = ReturnColumnName;
        CheckRegularExpression = true;
    }


    /// <summary>
    /// Reloads the selector's data.
    /// </summary>
    /// <param name="forceReload">Indicates whether data should be forcibly reloaded</param>
    public void Reload(bool forceReload)
    {
        usTemplate.Reload(forceReload);
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load update panel container
        if (usTemplate == null)
        {
            pnlUpdate.LoadContainer();
        }

        // Call base method
        base.CreateChildControls();
    }

    #endregion
}