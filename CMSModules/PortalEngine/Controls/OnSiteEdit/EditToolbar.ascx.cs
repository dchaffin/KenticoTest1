using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

using CMS.Controls;
using CMS.UIControls;
using CMS.PortalControls;
using CMS.CMSHelper;
using CMS.PortalEngine;
using CMS.GlobalHelper;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.SettingsProvider;
using CMS.UIControls.UniMenuConfig;
using CMS.SiteProvider;
using CMS.URLRewritingEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_PortalEngine_Controls_OnSiteEdit_EditToolbar : CMSAbstractPortalUserControl, IPostBackEventHandler, ICallbackEventHandler
{
    #region "Variables"

    private MessagesPlaceHolder mMessagePlaceholder = null;
    private string preferredCultureCode = CMSContext.PreferredCultureCode;
    private bool isRTL = false;
    private CurrentUserInfo cui = null;
    private bool is404 = false;
    private bool largeCMSDeskButton = false;
    private string callbackResult = string.Empty;
    private string modalDialogScript = "modalDialog({0}, '{1}', '900', '85%');";
    private bool isRootDocument = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates whether toolbar is displayed for page not found mode
    /// </summary>
    protected bool IsPageNotFound
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the current view mode
    /// </summary>
    protected ViewModeEnum ViewMode
    {
        get
        {
            return CMSContext.ViewMode;
        }
    }


    /// <summary>
    /// Gets the messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            if (mMessagePlaceholder == null)
            {
                mMessagePlaceholder = new MessagesPlaceHolder();
                mMessagePlaceholder.UseRelativePlaceHolder = false;
                mMessagePlaceholder.OffsetY = 5;
                mMessagePlaceholder.OffsetX = 5;
                mMessagePlaceholder.ContainerCssClass = "OnSiteEdit";
                plcEdit.Controls.Add(mMessagePlaceholder);
            }
            return mMessagePlaceholder;
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        // Set the viewmode according the the URL param
        if ((ViewMode == ViewModeEnum.LiveSite) && QueryHelper.Contains("viewmode"))
        {
            ViewModeEnum queryStringViewMode = ViewModeCode.GetPageEnumFromString(QueryHelper.GetString("viewmode", "livesite"));
            if (queryStringViewMode == ViewModeEnum.EditLive)
            {
                SetViewMode(queryStringViewMode);
            }
        }

        if (ViewMode == ViewModeEnum.EditLive)
        {
            // Check if there is required a redirect to the specific document
            if (QueryHelper.Contains("onsitenodeid"))
            {
                int nodeId = QueryHelper.GetInteger("onsitenodeid", 0);
                TreeProvider treeProvider = new TreeProvider();
                TreeNode node = treeProvider.SelectSingleNode(nodeId);
                string url = URLHelper.ResolveUrl(DocumentURLProvider.GetUrl(node.NodeAliasPath, string.Empty /* ensure getting the link (not the linked document) */, node.NodeSiteName, URLHelper.CurrentUrlLangPrefix));
                URLHelper.Redirect(url);
            }
        }

        base.OnInit(e);
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Do not process control by default
        StopProcessing = true;

        // Keep frequent objects
        cui = CMSContext.CurrentUser;
        PageInfo pi = CMSContext.CurrentPageInfo;

        if (pi == null)
        {
            IsPageNotFound = true;
            pi = OnSiteEditHelper.PageInfoForPageNotFound;
        }

        ucUIToolbar.StopProcessing = true;
        largeCMSDeskButton = !cui.UserSiteManagerAdmin;

        // Check whether user is authorized to edit page
        if ((pi != null) && cui.IsAuthenticated() && cui.IsEditor && ((IsPageNotFound && pi.NodeID == 0) || cui.IsAuthorizedPerTreeNode(pi.NodeID, NodePermissionsEnum.Read) == AuthorizationResultEnum.Allowed))
        {
            // Enable processing
            StopProcessing = false;

            // Check whether the preferred culture is RTL
            isRTL = CultureHelper.IsUICultureRTL();

            // Add link to CSS file
            CSSHelper.RegisterCSSLink(Page, "Design", "OnSiteEdit.css");

            // Filter UI element buttons
            ucUIToolbar.OnButtonFiltered += ucUIToolbar_OnButtonFiltered;
            ucUIToolbar.OnButtonCreated += ucUIToolbar_OnButtonCreated;
            ucUIToolbar.OnButtonCreating += ucUIToolbar_OnButtonCreating;
            ucUIToolbar.OnGroupsCreated += ucUIToolbar_OnGroupsCreated;
            ucUIToolbar.IsRTL = isRTL;

            // Register edit script file
            RegisterEditScripts(pi);

            if (ViewMode == ViewModeEnum.EditLive)
            {
                popupHandler.Visible = true;
                IsLiveSite = false;
                MessagesPlaceHolder.IsLiveSite = false;
                MessagesPlaceHolder.Opacity = 100;

                // Display warning in the Safe mode
                if (PortalHelper.SafeMode)
                {
                    string safeModeText = GetString("onsiteedit.safemode") + "<br/><a href=\"" + URLHelper.RawUrl.Replace("safemode=1", "safemode=0") + "\">" + GetString("general.close") + "</a> " + GetString("contentedit.safemode2");
                    string safeModeDescription = GetString("onsiteedit.safemode") + "<br/>" + GetString("general.seeeventlog");

                    // Display the warning message
                    ShowWarning(safeModeText, safeModeDescription, "");
                }

                ucUIToolbar.StopProcessing = false;

                // Ensure document redirection
                if (!String.IsNullOrEmpty(pi.DocumentMenuRedirectUrl))
                {
                    string redirectUrl = CMSContext.ResolveMacros(pi.DocumentMenuRedirectUrl);
                    redirectUrl = URLHelper.ResolveUrl(redirectUrl);
                    ShowInformation(GetString("onsiteedit.redirectinfo") + " <a href=\"" + redirectUrl + "\">" + redirectUrl + "</a>");
                }
            }
            // Mode menu on live site
            else if (ViewMode == ViewModeEnum.LiveSite)
            {
                // Hide the edit panel, show only slider button
                pnlToolbarSpace.Visible = false;
                pnlToolbar.Visible = false;
                pnlSlider.Visible = true;

                imgSliderButton.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/OnSiteEdit/edit.png");
                imgSliderButton.ToolTip = GetString("onsiteedit.editmode");
                imgSliderButton.AlternateText = GetString("onsitedit.editmode");

                pnlButton.Attributes.Add("onclick", "OnSiteEdit_ChangeEditMode();");

                imgMaximize.Style.Add("display", "none");
                imgMaximize.AlternateText = GetString("general.maximize");
                imgMaximize.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/OnSiteEdit/ArrowDown.png");
                imgMinimize.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/OnSiteEdit/ArrowUp.png");
                imgMinimize.AlternateText = GetString("general.minimize");
                pnlMinimize.Attributes.Add("onclick", "OESlideSideToolbar();");

                // Hide the OnSite edit button when displayed in CMSDesk
                pnlSlider.Style.Add("display", "none");
            }
        }
        // Hide control actions for unauthorized users
        else
        {
            plcEdit.Visible = false;
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (ViewMode == ViewModeEnum.EditLive)
        {
            // Display workflow info message
            if (DocumentManager.Workflow != null)
            {
                string message = DocumentManager.GetDocumentInfo(true);
                if (!string.IsNullOrEmpty(message))
                {
                    ShowInformation(message);
                }
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Groups created event handler.
    /// </summary>
    protected void ucUIToolbar_OnGroupsCreated(object sender, List<Group> groups)
    {
        if (!IsPageNotFound)
        {
            // Replace Culture button
            Group culture = groups.Find(g => g.CssClass.Contains("OnSiteCultures"));
            if (culture != null)
            {
                // Hide culture selector when there is only one culture for the current site
                InfoDataSet<CultureInfo> sites = CultureInfoProvider.GetSiteCultures(CMSContext.CurrentSiteName);
                if ((sites.Tables[0].Rows.Count < 2)
                    || (IsPageNotFound && OnSiteEditHelper.PageInfoForPageNotFound.NodeID == 0))
                {
                    // Remove the culture button
                    groups.Remove(culture);
                }
                else
                {
                    culture.ControlPath = "~/CMSAdminControls/UI/UniMenu/OnSiteEdit/CultureMenu.ascx";
                }
            }

            // Replace Device profile button
            Group deviceProfile = groups.Find(g => g.CssClass.Contains("OnSiteDeviceProfile"));
            if (deviceProfile != null)
            {
                // Hide device profile selector when there is only one device defined or device profile module is disabled
                if (!DeviceProfileInfoProvider.IsDeviceProfilesEnabled(CMSContext.CurrentSiteName))
                {
                    // Remove the device profile button
                    groups.Remove(deviceProfile);
                }
                else
                {
                    deviceProfile.ControlPath = "~/CMSModules/DeviceProfile/Controls/ProfilesMenuControl.ascx";
                }
            }

            // Replace the Highlight and Hidden buttons
            Group otherGroup = groups.Find(g => g.CssClass.Contains("OnSiteOthers"));
            if (otherGroup != null)
            {
                otherGroup.ControlPath = "~/CMSAdminControls/UI/UniMenu/OnSiteEdit/OtherMenu.ascx";
            }

            // Use a specific css class for the large CMSDesk button
            if (largeCMSDeskButton)
            {
                Group adminsGroup = groups.Find(g => g.CssClass.Contains("OnSiteAdmins"));
                if (adminsGroup != null)
                {
                    adminsGroup.CssClass += " BigCMSDeskButton";
                }
            }
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// OnButtonCreating event handler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    protected void ucUIToolbar_OnButtonCreating(object sender, UniMenuArgs e)
    {
        if (e.UIElement != null)
        {
            switch (e.UIElement.ElementName.ToLowerCSafe())
            {
                case "onsitecmsdesk":
                    if (largeCMSDeskButton)
                    {
                        // Display big button for CMSDesk when SiteManager button is not visible
                        e.UIElement.ElementIconPath = "CMSModules/CMS_PortalEngine/OnSiteEdit/cmsdesk_large.png";
                        e.UIElement.ElementSize = UIElementSizeEnum.Large;
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// OnButtonCreated event handler.
    /// </summary>
    protected void ucUIToolbar_OnButtonCreated(object sender, UniMenuArgs e)
    {
        if ((e.UIElement == null) || (e.ButtonControl == null))
        {
            return;
        }

        switch (e.UIElement.ElementName.ToLowerCSafe())
        {
            case "onsitedelete":
                if (isRootDocument)
                {
                    DisableButton(e, GetString("onsitedit.deleteroot"));
                }
                break;

            case "onsiteclose":
            case "onsitesignout":
                {
                    string script = string.Empty;

                    // Show javascript confirmation when the document is not found or published
                    if (IsPageNotFound ||
                        ((CurrentPageInfo != null) && !CurrentPageInfo.IsPublished && URLRewriter.PageNotFoundForNonPublished(CMSContext.CurrentSiteName)))
                    {
                        script = "if (!confirm('" + GetString("onsiteedit.signout404confirmation") + "')) { return false; } ";
                        is404 = true;
                    }

                    // Sign out postback script
                    string eventCode = (e.UIElement.ElementName.ToLowerCSafe() == "onsitesignout") ? "signout" : "changeviewmode";
                    script += ControlsHelper.GetPostBackEventReference(this, eventCode);

                    e.ButtonControl.Attributes.Add("onclick", script);
                }
                break;
        }
    }


    /// <summary>
    /// OnButtonFiltered event handler.
    /// </summary>
    protected bool ucUIToolbar_OnButtonFiltered(object sender, UniMenuArgs e)
    {
        if (e.UIElement == null)
        {
            return false;
        }

        // Process only basic elements in page not found mode
        if (IsPageNotFound)
        {
            switch (e.UIElement.ElementName.ToLowerCSafe())
            {
                case "onsitesitemanager":
                case "onsitecmsdesk":
                case "onsitelist":
                case "onsiteclose":
                case "onsitesignout":
                    break;

                default:
                    return false;
            }
        }

        // Switch by element name
        switch (e.UIElement.ElementName.ToLowerCSafe())
        {
            // Site manager link
            case "onsitesitemanager":
                if (largeCMSDeskButton)
                {
                    return false;
                }
                break;
        }

        return true;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Registers script files for on-site editing
    /// </summary>
    /// <param name="pi">Current page info</param>
    private void RegisterEditScripts(PageInfo pi)
    {
        ScriptHelper.RegisterJQueryCookie(Page);
        StringBuilder script = new StringBuilder();

        // Edit toolbar scripts
        if (ViewMode == ViewModeEnum.EditLive)
        {
            // Dialog scripts
            ScriptHelper.RegisterDialogScript(Page);

            // General url settings
            UIPageURLSettings settings = new UIPageURLSettings();
            settings.AllowSplitview = false;
            settings.NodeID = pi.NodeID;
            settings.Culture = pi.DocumentCulture;
            settings.AdditionalQuery = "dialog=1";

            // Edit document
            settings.Mode = "editform";
            settings.Action = null;
            string editUrl = CMSDeskPage.GetUIPageURL(settings);
            settings.Mode = string.Empty;
            // Toolbar - Edit button script
            editUrl = URLHelper.RemoveParameterFromUrl(editUrl, "mode");
            string scriptEdit = GetModalDialogScript(editUrl, "editpage");
            // User contributions - Edit item script
            string scriptEditItem = GetModalDialogScript(AddItemUrlParameters(editUrl), "editpage");

            // Delete document
            settings.Action = "delete";
            string deleteUrl = CMSDeskPage.GetUIPageURL(settings);
            // Toolbar - Delete button script
            string scriptDelete = GetModalDialogScript(deleteUrl, "deletepage");
            // User contributions - Delete item script
            string scriptDeleteItem = GetModalDialogScript(AddItemUrlParameters(deleteUrl), "deletepage");

            // New document
            settings.Action = "new";
            settings.AdditionalQuery += "&reloadnewpage=1";
            string newUrl = CMSDeskPage.GetUIPageURL(settings);
            newUrl = AddNewItemUrlParameters(newUrl);
            // Toolbar - New button script
            string scriptNew = GetModalDialogScript(newUrl, "newpage");

            // Toolbar - Properties button script
            string scriptProperties = GetModalDialogScript(ResolveUrl("~/CMSModules/Content/CMSDesk/Properties/Properties_Frameset.aspx?mode=editlive&documentid=" + pi.DocumentID), "propertiespage");

            // Display items from current level by default
            int nodeId = pi.NodeParentID;
            // If current level is root display first level
            if (nodeId == 0)
            {
                isRootDocument = true;
                nodeId = pi.NodeID;
            }

            // In page not found mode display first level
            if (nodeId == 0)
            {
                TreeProvider tp = new TreeProvider(cui);
                TreeNode tn = tp.SelectSingleNode(CMSContext.CurrentSiteName, "/", TreeProvider.ALL_CULTURES);
                if (tn != null)
                {
                    nodeId = tn.NodeID;
                }
            }

            // Listing
            string listItemUrl = ResolveUrl("~/CMSModules/Content/CMSDesk/View/listing.aspx?dialog=1&wopenernodeid=" + pi.NodeID + "&nodeid=##id##");
            string scriptListItem = GetModalDialogScript(listItemUrl.Replace("##id##", nodeId.ToString()), "listingpage");

            // New culture
            string newCultureUrl = ResolveUrl("~/CMSModules/Content/CMSDesk/New/NewCultureVersion.aspx?nodeid=##id##&culture=##cult##&dialog=1");

            script.Append(@"
                var OEEditImagePath = '", GetImageUrl("CMSModules/CMS_PortalEngine/OnSiteEdit/webpart_edit.png"), @"';
                var OEConfigureImagePath = '", GetImageUrl("CMSModules/CMS_PortalEngine/OnSiteEdit/webpart_properties.png"), @"';
                var OEIsRTL = ", (isRTL ? "true" : "false"), @";
                var OECurrentNodeId = ", (CMSContext.CurrentPageInfo != null) ? CMSContext.CurrentPageInfo.NodeID : 0, @";
                var OEIsMobile = ", (CMSContext.CurrentDevice.IsMobile ? "true" : "false"), @";
                var OEHdnPostbackValue = null;

                function ConfigureWebPart(webPartZoneId, webPartId, aliasPath, instanceGuid, templateId, refreshOnCancel) {
                    modalDialog('", OnSiteEditHelper.WebPartPropertiesPageUrl, @"?aliaspath=' + aliasPath + '&zoneid=' + webPartZoneId + '&webpartid=' + webPartId + '&culture=' + ", preferredCultureCode, @" + '&instanceguid=' + instanceGuid + '&templateid=' + templateId + (!!refreshOnCancel ? '&refreshoncancel=true'  : ''), 'configurewebpart', 900, 638);
                }

                function NewDocument(parentId, classId, targetWindow)
                {
                    OEClearZIndex(OEActiveWebPart);

                    if (targetWindow == null) {
                        ", scriptNew, @";   
                    }
                    else {
                        targetWindow.location.href = '", newUrl, @"';
                    }
                }

                function EditDocument(nodeId, targetWindow)
                {
                    OEClearZIndex(OEActiveWebPart); 

                    // Edit item button in repeaters and datalists
                    var arg = 'editurl;' + nodeId;
                    ", Page.ClientScript.GetCallbackEventReference(this, "arg", "OECallbackHandler", null), @";
                   
                }

                function OECallbackHandler(arg, context) {
                    ", GetModalDialogScript("arg", "editpage", false), @"
                }

                function DeleteDocument(nodeId)
                {
                    OEClearZIndex(OEActiveWebPart); 
                    ", scriptDeleteItem, @" 
                }

                function OnSiteToolbarAction(name)
                { 
                    switch(name)
                    {
                        case 'edit':
                                ", scriptEdit, @"
                            break;

                        case 'properties':
                                ", scriptProperties, @"
                            break;

                        case 'new':
                                var parentId = OECurrentNodeId;
                                var classId = 0;
                                ", scriptNew, @"
                            break;

                        case 'delete':
                                ", scriptDelete, @"
                            break;

                        case 'list':
                                ", scriptListItem, @"
                            break;

                        default:
                            alert('Required action is not implemented.');
                    }
                }

                function SelectNode(nodeId, parentNodeId)
                {
                    var liu = '", listItemUrl, @"';
                    return liu.replace(/##id##/g, nodeId);
                }

                function NewDocumentCulture(nodeId, culture)
                {
                    var liu = '", newCultureUrl, @"';
                    liu = liu.replace(/##id##/g, nodeId);
                    liu = liu.replace(/##cult##/g, culture);
                    ", GetModalDialogScript("liu", "newculture", false), @"
                }

                function ToggleUpdateProgress(show) {
                    var loadingBg = jQuery('.OnSiteUPBg');
                    var loading = jQuery('#", updateProgress.ClientID, @"');
                
                    if (show) {
                        loading.css('display', 'inline');
                        loadingBg.show();
                    }
                    else {
                        loading.hide();
                        loadingBg.hide();
                    }
                }

                function OEEnsureHdnPostbackValue() {
                    if (OEHdnPostbackValue == null) {
                        OEHdnPostbackValue = document.getElementById('", hdnPostbackValue.ClientID, @"');
                    }
                }

                // Changes the device profile
                function ChangeDevice(device) {
                    OEEnsureHdnPostbackValue();
                    OEHdnPostbackValue.value = device;
                    ", ControlsHelper.GetPostBackEventReference(this, "changedeviceprofile"), @"
                } ");

            // Register jQuery
            ScriptHelper.RegisterJQueryDialog(Page);

            // Register OnSiteEdit script file
            ScriptHelper.RegisterScriptFile(Page, "DesignMode/OnSiteEdit.js");
            ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/jquery/jquery-url.js");
        }
        // Slider buttons scripts
        else if (ViewMode == ViewModeEnum.LiveSite)
        {
            bool sliderIsMinimized = ValidationHelper.GetBoolean(CookieHelper.GetValue(CookieName.OnSiteButtonMinimized), false);
            if (sliderIsMinimized)
            {
                // Renew the cookie
                CookieHelper.SetValue(CookieName.OnSiteButtonMinimized, "true", "/", DateTime.Now.AddDays(31), false);
            }

            script.Append(@"
                var OEIsMinimized = ", (sliderIsMinimized ? "true" : "false"), @";

                function OESlideSideToolbar() {
                    // Hide the slider button in CMSDesk
                    if (parent != this) {
                        return;
                    }

                    var toolbarEl = document.getElementById('", pnlSlider.ClientID, @"');
                    toolbarEl.style.display = ""block"";

                    if (!OEIsMinimized) {
                        // Show slider button
                        toolbarEl.style.top = ""0px"";
                        document.getElementById('", imgMinimize.ClientID, @"').style.display = ""inline"";
                        document.getElementById('", imgMaximize.ClientID, @"').style.display = ""none"";
                        jQuery.cookie('", CookieName.OnSiteButtonMinimized, @"', null, { path: '/' });
                    }
                    else {
                        // Hide slider button
                        var sliderButtonEl = document.getElementById('", pnlButton.ClientID, @"');
                        toolbarEl.style.top = ""-"" + (sliderButtonEl.offsetHeight + 1) + ""px"";
                        document.getElementById('", imgMinimize.ClientID, @"').style.display = ""none"";
                        document.getElementById('", imgMaximize.ClientID, @"').style.display = ""inline"";
                        jQuery.cookie('", CookieName.OnSiteButtonMinimized, @"', true, { path: '/', expires: 31 });
                    }

                    OEIsMinimized = !OEIsMinimized;
                }

                // Display slider button
                jQuery(document).ready(function() { OESlideSideToolbar(); });
                ");
        }

        script.Append(" function OnSiteEdit_ChangeEditMode(){", ControlsHelper.GetPostBackEventReference(this, "changeviewmode"), "} ");

        ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "OnSiteEditActions", ScriptHelper.GetScript(script.ToString()));
    }


    /// <summary>
    /// Disables the UniMenu button.
    /// </summary>
    /// <param name="e">The UniMenu item argument</param>
    /// <param name="disabledMessage">The message displayed in the tooltip when the user does not have sufficient permissions</param>
    private void DisableButton(UniMenuArgs e, string disabledMessage)
    {
        if (e.ImageControl != null)
        {
            // Replace the image with the disabled image (i.e: "new.png" -> "newDisabled.png")
            string imageUrl = e.ImageControl.ImageUrl;
            int extensionPosition = imageUrl.LastIndexOf('.');
            if (extensionPosition != -1)
            {
                imageUrl = imageUrl.Substring(0, extensionPosition) + "Disabled" + imageUrl.Substring(extensionPosition);
                e.ImageControl.ImageUrl = imageUrl;
            }
        }

        if (e.ButtonControl != null)
        {
            // Disable the menu button
            e.ButtonControl.Attributes.Remove("onclick");
            e.ButtonControl.CssClass = "BigButtonDisabled";
            e.ButtonControl.ToolTip = disabledMessage;
        }
    }


    /// <summary>
    /// Signs out the current user.
    /// </summary>
    private void SignOut()
    {
        string signOutUrl = URLHelper.CurrentURL;

        // Redirect to the root document when page not found
        if (is404)
        {
            signOutUrl = ResolveUrl("~/");
        }

        // LiveID sign out URL is set if this LiveID session (otherwise the CurrentURL is used)
        CMSPage.SignOut(ref signOutUrl);

        ScriptHelper.RegisterStartupScript(this, typeof(string), "livesiteScript", ScriptHelper.GetScript("location.replace('" + signOutUrl + "');"));
    }


    /// <summary>
    /// Sets the view mode.
    /// </summary>
    /// <param name="viewMode">The view mode</param>
    private void SetViewMode(ViewModeEnum viewMode)
    {
        if (PortalHelper.IsOnSiteEditingEnabled(CMSContext.CurrentSiteName) && UserInfoProvider.IsAuthenticated())
        {
            // Remove the "viewmode" param from url and redirect
            string returnUrl = URLHelper.RemoveParameterFromUrl(URLHelper.CurrentURL, "viewmode");

            CMSContext.ViewMode = viewMode;

            // Switch view mode
            if (viewMode == ViewModeEnum.EditLive)
            {
                //  Handle default alias path
                if (URLRewriter.CurrentPageInfoSource == PageInfoSource.DefaultAliasPath)
                {
                    string aliasPath = PageInfoProvider.GetDefaultAliasPath(URLHelper.GetCurrentDomain(), CMSContext.CurrentSiteName);
                    if (!String.IsNullOrEmpty(aliasPath))
                    {
                        string query = URLHelper.GetQuery(returnUrl);
                        returnUrl = URLHelper.ResolveUrl(DocumentURLProvider.GetUrl(aliasPath));
                        returnUrl = URLHelper.AppendQuery(returnUrl, query);
                    }
                }
            }
            else if (is404)
            {
                // Redirect to the root document when page not found
                returnUrl = ResolveUrl("~/");
            }

            // Redirect to the URL
            URLHelper.Redirect(returnUrl, false, null);
        }
    }


    /// <summary>
    /// Adds the general dialog URL parameters (used in the user contribution actions)
    /// </summary>
    /// <param name="url">The URL to modify</param>
    private string AddItemUrlParameters(string url)
    {
        // Edit item
        url = URLHelper.RemoveQuery(url);
        url = URLHelper.AddParameterToUrl(url, "dialog", "1");
        url = URLHelper.AddParameterToUrl(url, "nodeid", "' + nodeId + '");

        return url;
    }


    /// <summary>
    /// Adds the new item dialog URL parameters (used in the user contribution actions)
    /// </summary>
    /// <param name="url">The URL to modify</param>
    private string AddNewItemUrlParameters(string actionUrl)
    {
        actionUrl = URLHelper.RemoveQuery(actionUrl);
        actionUrl = URLHelper.AddParameterToUrl(actionUrl, "action", "new");
        actionUrl = URLHelper.AddParameterToUrl(actionUrl, "parentnodeid", "' + parentId + '");
        actionUrl = URLHelper.AddParameterToUrl(actionUrl, "classid", "' + classId + '");
        actionUrl = URLHelper.AddParameterToUrl(actionUrl, "dialog", "1");

        return actionUrl;
    }


    /// <summary>
    /// Gets the modal dialog script.
    /// </summary>
    /// <param name="url">The URL of the modal dialog</param>
    /// <param name="dialogName">Name of the dialog</param>
    private string GetModalDialogScript(string url, string dialogName)
    {
        return GetModalDialogScript(url, dialogName, true);
    }


    /// <summary>
    /// Gets the modal dialog script.
    /// </summary>
    /// <param name="url">The URL of the modal dialog</param>
    /// <param name="dialogName">Name of the dialog</param>
    /// <param name="useQuotesForUrl">If set to <c>TRUE</c>, wrap the given url into the single quotes.
    /// When FALSE, do not wrap the url -> in this case it can be used for javascript variables holding url itself</param>
    private string GetModalDialogScript(string url, string dialogName, bool useQuotesForUrl)
    {
        if (useQuotesForUrl)
        {
            url = "'" + url + "'";
        }

        return String.Format(modalDialogScript, url, dialogName);
    }

    #endregion


    #region IPostBackEventHandler Members

    /// <summary>
    /// Processes an event raised when a form is posted to the server.
    /// </summary>
    public void RaisePostBackEvent(string eventArgument)
    {
        eventArgument = eventArgument.ToLowerCSafe();

        switch (eventArgument)
        {
            case "changeviewmode":
                {
                    // Switch view mode
                    if (CMSContext.ViewMode == ViewModeEnum.EditLive)
                    {
                        SetViewMode(ViewModeEnum.LiveSite);
                    }
                    else
                    {
                        SetViewMode(ViewModeEnum.EditLive);
                    }
                }
                break;

            case "changedeviceprofile":
                {
                    // Set the device name
                    string deviceName = hdnPostbackValue.Value;
                    CMSContext.CurrentDeviceProfileName = deviceName;

                    // Refresh the document
                    URLHelper.Redirect(URLHelper.RawUrl);
                }
                break;

            case "signout":
                SignOut();
                break;

            default:
                break;
        }
    }

    #endregion


    #region "Callback handling"

    /// <summary>
    /// Raises the callback event.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        if ((eventArgument == null))
        {
            return;
        }

        string[] argumentParts = eventArgument.Split(';');

        if (argumentParts.Length > 0)
        {
            string action = argumentParts[0];

            switch (action)
            {
                // Get edit page url (also consider custom document type urls)
                case "editurl":
                    {
                        int nodeId = ValidationHelper.GetInteger(argumentParts[1], 0);

                        // Prepare url retriever settings
                        UIPageURLSettings settings = new UIPageURLSettings();
                        settings.AllowSplitview = false;
                        settings.NodeID = nodeId;
                        settings.AdditionalQuery = "dialog=1";
                        settings.Mode = "editform";
                        settings.Action = null;
                        settings.Culture = preferredCultureCode;

                        // Get edit page url
                        callbackResult = CMSDeskPage.GetUIPageURL(settings);
                    }
                    break;

                default:
                    break;
            }
        }
    }


    /// <summary>
    /// Prepares the callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        return callbackResult;
    }

    #endregion
}
