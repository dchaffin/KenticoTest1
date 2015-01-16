using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.DocumentEngine;
using CMS.ExtendedControls;

public partial class CMSModules_Widgets_Controls_Dashboard : CMSUserControl
{
    #region "Variables"

    private bool mHighlightDropableAreas = true;
    private bool mActivateZoneBorder = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the dashboard site name.
    /// </summary>
    public string DashboardSiteName
    {
        get
        {
            return PortalContext.DashboardSiteName;
        }
        set
        {
            PortalContext.DashboardSiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the portal page instance.
    /// </summary>
    public PortalPage PortalPageInstance
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the literal control.
    /// </summary>
    public Literal TagsLiteral
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the element name.
    /// </summary>
    public string ResourceName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the element name.
    /// </summary>
    public string ElementName
    {
        get;
        set;
    }


    /// <summary>
    /// Dropable areas are highlighted when widget dragged.
    /// </summary>
    public bool HighlightDropableAreas
    {
        get
        {
            return mHighlightDropableAreas;
        }
        set
        {
            mHighlightDropableAreas = value;
        }
    }


    /// <summary>
    /// If true zone border can be activated (+add widget button).
    /// </summary>
    public bool ActivateZoneBorder
    {
        get
        {
            return mActivateZoneBorder;
        }
        set
        {
            mActivateZoneBorder = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Disable callback for webpart movement
        // Validation viewstate may cause errors when changing DOM of page and callback event afterwards (typically drag&drop widget(webpart)manipulation)
        String target = ValidationHelper.GetString(Request.Form["__CALLBACKID"], String.Empty);
        String argument = ValidationHelper.GetString(Request.Form["__CALLBACKPARAM"], String.Empty);
        string[] argumentParts = argument.Split('\n');
        if (argumentParts.Length > 0)
        {
            if ((target == manPortal.UniqueID) && (argumentParts[0].ToLowerCSafe() == "movewebpartasync"))
            {
                EnableViewState = false;
            }
        }
    }


    /// <summary>
    /// Check security.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterTitleScript(Page, GetString("mydesk.ui.mydashboard"));

        if (!String.IsNullOrEmpty(ResourceName) && !String.IsNullOrEmpty(ElementName))
        {
            // Check UIProfile
            if ((CMSContext.CurrentUser == null) || (!CMSContext.CurrentUser.IsAuthorizedPerUIElement(ResourceName, ElementName)))
            {
                URLHelper.Redirect(UIHelper.GetInformationUrl("uiprofile.uinotavailable"));
            }
        }

        if (!CheckHashCode())
        {
            RedirectToAccessDenied(GetString("dashboard.invalidparameters"));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Init the header tags
        TagsLiteral.Text = PortalPageInstance.HeaderTags;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Ensures dashboard initialization.
    /// </summary>
    public void SetupDashboard()
    {
        // Register placeholder for context menu
        ICMSPage page = Page as ICMSPage;
        if (page != null)
        {
            page.ContextMenuContainer = plcCtx;
        }

        if (PortalPageInstance == null)
        {
            throw new Exception("[DashboardControl.SetupDashboard] Portal page instance must be set.");
        }

        // Default settings for drag and drop for dashboard
        manPortal.HighlightDropableAreas = HighlightDropableAreas;
        manPortal.ActivateZoneBorder = ActivateZoneBorder;

        string dashboardName = QueryHelper.GetString("DashboardName", PersonalizationInfoProvider.UNDEFINEDDASHBOARD);

        // Set culture
        CultureInfo ci = CultureHelper.PreferredUICultureInfo;
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;

        // Init the page components
        PortalPageInstance.PageManager = manPortal;
        manPortal.SetMainPagePlaceholder(plc);

        string templateName = QueryHelper.GetString("templatename", String.Empty);

        PageTemplateInfo pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateName);
        if (pti != null)
        {
            if (pti.PageTemplateType != PageTemplateTypeEnum.Dashboard)
            {
                RedirectToAccessDenied("dashboard.invalidpagetemplate");
            }

            // Prepare virtual page info
            PageInfo pi = PageInfoProvider.GetVirtualPageInfo(pti.PageTemplateId);
            pi.DocumentNamePath = "/" + templateName;
            
            CMSContext.CurrentPageInfo = pi;

            // Set the design mode
            PortalContext.SetRequestViewMode(ViewModeEnum.DashboardWidgets);
            PortalContext.DashboardName = dashboardName;
            ContextHelper.Add("DisplayContentInDesignMode", "0", true, false, false, DateTime.MinValue);

            PortalPageInstance.ManagersContainer = plcManagers;
            PortalPageInstance.ScriptManagerControl = manScript;
        }
        else
        {
            RedirectToInformation(GetString("dashboard.notemplate"));
        }
    }


    /// <summary>
    /// Checks whether url parameters are valid.
    /// </summary>
    protected bool CheckHashCode()
    {
        // Get hashcode from querystring
        string hash = QueryHelper.GetString("hash", String.Empty);

        // Check whether url contains all reuired values
        if (QueryHelper.Contains("dashboardname") && !String.IsNullOrEmpty(hash))
        {
            // Try get custom hash values
            string hashValues = QueryHelper.GetString("hashvalues", String.Empty);

            string hashString = String.Empty;
            // Use default hash values
            if (String.IsNullOrEmpty(hashValues))
            {
                hashString = QueryHelper.GetString("dashboardname", String.Empty) + "|" + QueryHelper.GetString("templatename", String.Empty);
            }
            // Use custom hash values
            else
            {
                string[] values = hashValues.Split(';');
                foreach (string value in values)
                {
                    hashString += QueryHelper.GetString(value, String.Empty) + "|";
                }

                hashString = hashString.TrimEnd('|');
            }

            // Compare url hash with current hash
            return ((CMSString.Compare(hash, ValidationHelper.GetHashString(hashString), false) == 0));
        }
        return false;
    }

    #endregion
}