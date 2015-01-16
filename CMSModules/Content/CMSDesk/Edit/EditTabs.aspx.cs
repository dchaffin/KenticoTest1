using System;
using System.Web.UI;
using System.Web;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.Controls.Configuration;

public partial class CMSModules_Content_CMSDesk_Edit_EditTabs : CMSContentPage, ICallbackEventHandler
{
    #region "Variables"

    private CurrentUserInfo currentUser = null;

    private bool showProductTab = false;
    private bool isMasterPage = false;
    private bool isPortalPage = false;
    private bool designEnabled = false;
    private bool authorizedPerDesign = false;
    private bool designPermissionRequired = false;

    private bool isWireframe = false;
    private bool hasWireframe = false;

    private int pageTabIndex = -1;
    private int designTabIndex = -1;
    private int formTabIndex = -1;
    private int productTabIndex = -1;
    private int masterTabIndex = -1;
    private int propertiesTabIndex = -1;
    private int analyticsIndex = -1;
    private int wireframeTabIndex = -1;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        ManagersContainer = plcManagers;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterProgress(this);

        if (!RequestHelper.IsPostBack())
        {
            chkWebParts.Checked = PortalHelper.DisplayContentInDesignMode;
        }

        chkWebParts.Attributes.Add("onclick", "SaveSettings()");
        chkWebParts.Text = GetString("EditTabs.DisplayContent");

        string scripts =
@"
function SetTabsContext(mode) {
    if ((mode == 'design') || (mode == 'wireframe')) {
        document.getElementById('divDesign').style.display = 'block';
    } else {
        document.getElementById('divDesign').style.display = 'none';
    }
    parent.SetSelectedMode(mode);
}

function RefreshContent() {
    parent.frames['contenteditview'].document.location.replace(parent.frames['contenteditview'].document.location);
}

function SaveSettings() { 
    __theFormPostData = ''; 
    WebForm_InitCallback(); " +
    ClientScript.GetCallbackEventReference(this, "'save'", "RefreshContent", null) + @"; 
}";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "editTabsScripts", scripts, true);

        // Initialize tabs
        tabsModes.OnTabCreated += tabModes_OnTabCreated;
        tabsModes.OnCheckTabSecurity += tabsModes_OnCheckTabSecurity;
        tabsModes.SelectedTab = 0;
        tabsModes.UrlTarget = "contenteditview";

        // Process the page mode
        currentUser = CMSContext.CurrentUser;

        if (Node != null)
        {
            // Product tab
            showProductTab = Node.HasSKU;

            // Initialize required variables
            authorizedPerDesign = currentUser.IsAuthorizedPerResource("CMS.Design", "Design");

            isWireframe = Node.IsWireframe();
            hasWireframe = isWireframe || (Node.NodeWireframeTemplateID > 0);

            // Get page template information
            try
            {
                PageInfo pi = PageInfoProvider.GetPageInfo(CMSContext.CurrentSiteName, Node.NodeAliasPath, Node.DocumentCulture, Node.DocumentUrlPath, false);
                if ((pi != null) && (pi.DesignPageTemplateInfo != null))
                {
                    PageTemplateInfo pti = pi.DesignPageTemplateInfo;
                    isPortalPage = pti.IsPortal;
                    isMasterPage = isPortalPage && ((Node.NodeAliasPath == "/") || pti.ShowAsMasterTemplate);
                    designEnabled = ((pti.PageTemplateType == PageTemplateTypeEnum.Portal) || (pti.PageTemplateType == PageTemplateTypeEnum.AspxPortal));
                }
            }
            catch
            {
                // Page info not found - probably tried to display document from different site
            }

            // Do not show design tab for CMS.File
            if (Node.NodeClassName.EqualsCSafe("CMS.File", true))
            {
                designEnabled = false;
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        SelectTab();
    }

    #endregion


    #region "Tab events"

    protected bool tabsModes_OnCheckTabSecurity(ref string resource, ref string permission)
    {
        if (designPermissionRequired)
        {
            resource = "CMS.Content";
            permission = "Design";
        }
        return designPermissionRequired;
    }


    private TabItem tabModes_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        string mode = null;

        string elem = element.ElementName.ToLowerCSafe();
        
        // Hide other tabs for wireframe
        if (isWireframe && ((elem != "wireframe") && (elem != "editform")))
        {
            return null;
        }

        switch (elem)
        {
            case "page":
                {
                    pageTabIndex = tabIndex;
                    mode = "edit";
                }
                break;

            case "design":
                {
                    // Check if the design mode is enabled
                    if (!designEnabled)
                    {
                        return null;
                    }

                    if (authorizedPerDesign)
                    {
                        designTabIndex = tabIndex;
                    }
                    else
                    {
                        if (!authorizedPerDesign)
                        {
                            designPermissionRequired = true;
                        }
                        return null;
                    }
                    mode = "design";
                }
                break;

            case "wireframe":
                {
                    // Check if the wireframe mode is enabled
                    if (!hasWireframe)
                    {
                        return null;
                    }

                    wireframeTabIndex = tabIndex;
                    mode = "wireframe";
                }
                break;

            case "editform":
                // Keep edit form
                {
                    formTabIndex = tabIndex;
                    mode = "editform";
                }
                break;

            case "product":
                {
                    // Check if the product tab is enabled
                    if (showProductTab && LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.Ecommerce, ModuleEntry.ECOMMERCE))
                    {
                        productTabIndex = tabIndex;
                    }
                    else
                    {
                        return null;
                    }
                    mode = "product";
                }
                break;

            case "masterpage":
                {
                    // Check if master page tab is enabled
                    if (isMasterPage && authorizedPerDesign)
                    {
                        masterTabIndex = tabIndex;
                    }
                    else
                    {
                        if (!authorizedPerDesign)
                        {
                            designPermissionRequired = true;
                        }
                        return null;
                    }

                    mode = "masterpage";
                }
                break;

            case "properties":
                {
                    // Document properties
                    propertiesTabIndex = tabIndex;
                    mode = "properties";
                }
                break;

            case "analytics":
                {
                    // Analytics
                    if (isWireframe)
                    {
                        return null;
                    }

                    analyticsIndex = tabIndex;
                    mode = "analytics";
                }
                break;

            case "validation":
                return null;
        }

        // Script for refresh 'display web part content' button
        string script = "SetTabsContext('" + elem + "');";

        UIPageURLSettings settings = new UIPageURLSettings
                                         {
                                             Mode = mode,
                                             Node = Node,
                                             NodeID = Node.NodeID,
                                             Culture = Node.DocumentCulture,
                                             PreferredURL = tab.RedirectUrl
                                         };

        // Ensure correct URL
        tab.RedirectUrl = GetDocumentPageUrl(settings);

        // Add script for refresh web part content button (displayed only when design mode)
        tab.OnClientClick = script + tab.OnClientClick;

        return tab;
    }

    #endregion


    #region "Methods"

    private void SelectTab()
    {
        switch (Action)
        {
            // New document / new document culture
            case "new":
            case "newculture":
                tabsModes.SelectedTab = pageTabIndex;
                break;

            case "properties":
                tabsModes.SelectedTab = propertiesTabIndex;
                break;

            case "product":
                tabsModes.SelectedTab = productTabIndex;
                break;

            default:
                // Set the tab
                switch (CMSContext.ViewMode)
                {
                    case ViewModeEnum.Edit:
                        tabsModes.SelectedTab = pageTabIndex;
                        break;

                    case ViewModeEnum.Wireframe:
                        tabsModes.SelectedTab = wireframeTabIndex;
                        break;

                    case ViewModeEnum.Design:
                        tabsModes.SelectedTab = designTabIndex;
                        break;

                    case ViewModeEnum.EditForm:
                        tabsModes.SelectedTab = formTabIndex;
                        break;

                    case ViewModeEnum.MasterPage:
                        tabsModes.SelectedTab = masterTabIndex;
                        break;

                    case ViewModeEnum.Product:
                        tabsModes.SelectedTab = productTabIndex;
                        break;

                    case ViewModeEnum.Properties:
                        tabsModes.SelectedTab = propertiesTabIndex;
                        break;

                    case ViewModeEnum.Analytics:
                        tabsModes.SelectedTab = analyticsIndex;
                        break;
                }
                break;
        }

        // Selected tab (for first load)
        if (tabsModes.SelectedTab == -1)
        {
            tabsModes.SelectedTab = 0;
        }
    }

    #endregion


    #region "Callback handling"

    public void RaiseCallbackEvent(string eventArgument)
    {
        if (eventArgument == "save")
        {
            PortalHelper.DisplayContentInDesignMode = chkWebParts.Checked;
        }
    }


    public string GetCallbackResult()
    {
        return string.Empty;
    }

    #endregion
}