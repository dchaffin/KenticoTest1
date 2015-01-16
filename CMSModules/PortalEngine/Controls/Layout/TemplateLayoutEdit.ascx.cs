using System;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.VirtualPathHelper;

public partial class CMSModules_PortalEngine_Controls_Layout_TemplateLayoutEdit : CMSPreviewControl
{
    #region "Variables"

    // Type of the edited object
    private enum EditedObjectTypeEnum
    {
        Layout = 0,
        Template = 1,
        DeviceLayout = 2
    }

    private HeaderAction createDeviceLayout = null;
    private HeaderAction removeDeviceLayout = null;

    protected bool startWithFullScreen = false;
    private int previewState = 0;
    private int mTemplateId = 0;
    private int? mDeviceProfileID = null;
    private BaseInfo mEditedObject = null;
    private EditedObjectTypeEnum? mEditedObjectType = null;
    private PageTemplateInfo mPageTemplateInfo = null;
    private DeviceProfileInfo mDeviceProfileInfo = null;

    private bool? mShowSharedLayoutWarnings = null;
    private bool requiresDialog = false;
    private bool dialog = false;
    private int sharedLayoutId = 0;

    private LayoutInfo actualLayoutInfo = null;
    private bool allowDeviceButtons = true;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets the edited object.
    /// </summary>
    private BaseInfo EditedObject
    {
        get
        {
            if (mEditedObject == null)
            {
                mEditedObject = CMSContext.EditedObject as BaseInfo;
            }

            return mEditedObject;
        }
    }


    /// <summary>
    /// Gets the type of the edited object.
    /// </summary>
    private EditedObjectTypeEnum EditedObjectType
    {
        get
        {
            if (mEditedObjectType == null)
            {
                mEditedObjectType = EditedObjectTypeEnum.Layout;

                if (EditedObject != null)
                {
                    switch (EditedObject.ObjectType)
                    {
                        case PredefinedObjectType.PAGETEMPLATE:
                            mEditedObjectType = EditedObjectTypeEnum.Template;
                            break;

                        case PortalObjectType.TEMPLATEDEVICELAYOUT:
                            mEditedObjectType = EditedObjectTypeEnum.DeviceLayout;
                            break;
                    }
                }
            }

            return mEditedObjectType.Value;
        }
    }


    /// <summary>
    /// Gets a value that indicates if warnings about the shared layouts should be visible.
    /// </summary>
    private bool ShowSharedLayoutWarnings
    {
        get
        {
            if (!mShowSharedLayoutWarnings.HasValue)
            {
                mShowSharedLayoutWarnings = QueryHelper.GetBoolean("sharedlayoutwarnings", true);
            }
            return mShowSharedLayoutWarnings.Value;
        }
    }


    /// <summary>
    /// Gets the template ID.
    /// </summary>
    private int TemplateID
    {
        get
        {
            if (mTemplateId == 0)
            {
                mTemplateId = QueryHelper.GetInteger("templateid", 0);
            }

            return mTemplateId;
        }
    }


    /// <summary>
    /// Gets the device profile ID.
    /// </summary>
    private int DeviceProfileID
    {
        get
        {
            if (mDeviceProfileID == null)
            {
                mDeviceProfileID = 0;

                DeviceProfileInfo deviceProfile = CMSContext.CurrentDeviceProfile;
                if (deviceProfile != null)
                {
                    mDeviceProfileID = deviceProfile.ProfileID;
                }
            }

            return mDeviceProfileID.Value;
        }
    }


    /// <summary>
    /// Gets the current page template info object.
    /// </summary>
    private PageTemplateInfo PageTemplateInfo
    {
        get
        {
            if (mPageTemplateInfo == null)
            {
                mPageTemplateInfo = PageTemplateInfoProvider.GetPageTemplateInfo(TemplateID);
            }

            return mPageTemplateInfo;
        }
    }


    /// <summary>
    /// Gets the current device profile info object.
    /// </summary>
    private DeviceProfileInfo DeviceProfileInfo
    {
        get
        {
            if (mDeviceProfileInfo == null)
            {
                mDeviceProfileInfo = DeviceProfileInfoProvider.GetDeviceProfileInfo(DeviceProfileID);
            }

            return mDeviceProfileInfo;
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Ensures correct displaying of info messages.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return editMenuElem.MessagesPlaceHolder;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            EditForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
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
            EditForm.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Actual form according to correct object type.
    /// </summary>
    public UIForm EditForm
    {
        get
        {
            switch (EditedObjectType)
            {
                case EditedObjectTypeEnum.Template:
                    return EditFormTemplate;

                case EditedObjectTypeEnum.DeviceLayout:
                    return EditFormDeviceLayout;

                case EditedObjectTypeEnum.Layout:
                default:
                    return EditFormLayout;
            }
        }
    }


    /// <summary>
    /// If true, for PageTemplates it is possible to switch between shared and custom layout.
    /// </summary>
    public bool AllowTypeSwitching
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowTypeSwitching"), false);
        }
        set
        {
            SetValue("AllowTypeSwitching", value);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Hide all UIForms
        EditFormTemplate.Visible = false;
        EditFormTemplate.StopProcessing = true;
        EditFormDeviceLayout.Visible = false;
        EditFormDeviceLayout.StopProcessing = true;
        EditFormLayout.Visible = false;
        EditFormLayout.StopProcessing = true;

        sharedLayoutId = QueryHelper.GetInteger("newshared", 0);

        PageTemplateDeviceLayoutInfo deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(TemplateID, DeviceProfileID);

        // Show UIForm for the current edited object type
        switch (EditedObjectType)
        {
            case EditedObjectTypeEnum.Template:
                EditFormTemplate.Visible = true;
                EditFormTemplate.StopProcessing = false;
                break;

            case EditedObjectTypeEnum.DeviceLayout:
                EditFormDeviceLayout.Visible = true;
                EditFormDeviceLayout.StopProcessing = false;
                break;

            case EditedObjectTypeEnum.Layout:
                EditFormLayout.Visible = true;
                EditFormLayout.StopProcessing = false;
                break;
        }

        if (AllowTypeSwitching && (EditedObjectType == EditedObjectTypeEnum.Layout))
        {
            if (deviceLayout != null)
            {
                // Force ObjectManager to work with PageTemplateDeviceLayout even though the EditedObject is layout
                editMenuElem.ObjectManager.ObjectType = PortalObjectType.TEMPLATEDEVICELAYOUT;
                editMenuElem.ObjectManager.ObjectID = deviceLayout.TemplateDeviceLayoutID;
            }
            else
            {
                // Force ObjectManager to work with PageTemplate even though the EditedObject is layout
                editMenuElem.ObjectManager.ObjectType = PredefinedObjectType.PAGETEMPLATE;
                editMenuElem.ObjectManager.ObjectID = TemplateID;
            }
        }

        pnlType.Visible = AllowTypeSwitching;
        requiresDialog = ((CMSDeskPage)Page).RequiresDialog;
        dialog = QueryHelper.GetBoolean("dialog", false);
    }


    /// <summary>
    /// Display info messages
    /// </summary>
    /// <param name="forceDisplay">If true, message is displayed even on postback</param>
    private void DisplayMessage(bool forceDisplay)
    {
        bool showMessage = editMenuElem.ObjectManager.IsObjectChecked() && (!RequestHelper.IsPostBack() || forceDisplay);
        // Setup Preview mode state and display shared template warning
        switch (EditedObjectType)
        {
            case EditedObjectTypeEnum.Template:
                {
                    previewState = GetPreviewStateFromCookies(CMSPreviewControl.PAGETEMPLATELAYOUT);
                    if (showMessage)
                    {
                        PageTemplateInfo pti = EditFormLayout.EditedObject as PageTemplateInfo;
                        ShowSharedTemplateWarningMessage(pti);
                    }
                }
                break;

            case EditedObjectTypeEnum.Layout:
                {
                    previewState = GetPreviewStateFromCookies(CMSPreviewControl.PAGELAYOUT);
                    LayoutInfo layoutInfo = EditFormLayout.EditedObject as LayoutInfo;
                    if (DialogMode && ShowSharedLayoutWarnings && showMessage && (layoutInfo != null))
                    {
                        ShowInformation(string.Format(GetString("layout.sharedwarning"), layoutInfo.LayoutDisplayName));
                    }
                }
                break;

            case EditedObjectTypeEnum.DeviceLayout:
                {
                    previewState = GetPreviewStateFromCookies(CMSPreviewControl.DEVICELAYUOT);
                    PageTemplateDeviceLayoutInfo deviceLayout = EditFormLayout.EditedObject as PageTemplateDeviceLayoutInfo;
                    if ((deviceLayout != null) && showMessage)
                    {
                        if (DialogMode && (deviceLayout.LayoutID > 0))
                        {
                            ShowInformation(GetString("layout.sharedwarning"));
                        }
                        else
                        {
                            PageTemplateInfo pti = PageTemplateInfoProvider.GetPageTemplateInfo(deviceLayout.PageTemplateID);
                            ShowSharedTemplateWarningMessage(pti);
                        }
                    }
                }
                break;
        }

        if (showMessage)
        {
            codeElem.ShowMessage();
            codeLayoutElem.ShowMessage();
            deviceCode.ShowMessage();
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        // Display the Create/Remove device layout buttons when the device is other than the default device
        allowDeviceButtons = string.IsNullOrEmpty(QueryHelper.GetString("devicename", null));

        base.OnLoad(e);

        DisplayMessage(false);
        if (AllowTypeSwitching && (EditedObject != null))
        {
            if (EditedObjectType == EditedObjectTypeEnum.DeviceLayout)
            {
                // Custom device layout
                radCustom.Checked = ((((PageTemplateDeviceLayoutInfo)EditedObject).LayoutID == 0) && (sharedLayoutId == 0));
                radShared.Checked = !radCustom.Checked;
            }
            else
            {
                // Standard layout
                radCustom.Checked = (EditedObjectType == EditedObjectTypeEnum.Template);
                radShared.Checked = !radCustom.Checked;
            }
        }

        ucFile.ObjectID = EditedObject.Generalized.ObjectID;
        ucFile.ObjectType = EditedObject.ObjectType;
        ucFile.SiteID = EditedObject.Generalized.ObjectSiteID;
        ucFile.Category = MetaFileInfoProvider.OBJECT_CATEGORY_THUMBNAIL;

        codeElem.Editor.AutoSize = true;
        codeElem.Editor.ParentElementID = ParentClientID;
        codeElem.FullscreenMode = (previewState != 0);

        codeLayoutElem.Editor.AutoSize = true;
        codeLayoutElem.Editor.ParentElementID = ParentClientID;
        codeLayoutElem.FullscreenMode = (previewState != 0);

        deviceCode.Editor.AutoSize = true;
        deviceCode.Editor.ParentElementID = ParentClientID;
        deviceCode.FullscreenMode = (previewState != 0);

        InitializeHeaderActions();
        RegisterScripts();

        editMenuElem.ObjectEditMenu.PreviewMode = true;
        editMenuElem.MenuPanel.CssClass = "PreviewMenu";
        editMenuElem.ObjectManager.OnAfterAction += new EventHandler<SimpleObjectManagerEventArgs>(ObjectManager_OnAfterAction);

        if (CMSObjectHelper.UseCheckinCheckout)
        {
            pnlContainer.CssClass = "PreviewDefaultContentLarge";
        }
        else
        {
            pnlContainer.CssClass = "PreviewDefaultContent";
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        HandleFullScreen();

        base.OnPreRender(e);

        if (AllowTypeSwitching && (EditedObject != null))
        {
            if (sharedLayoutId > 0)
            {
                drpLayout.Value = sharedLayoutId;
            }
            else if (EditedObjectType == EditedObjectTypeEnum.Layout)
            {
                drpLayout.Value = EditedObject.Generalized.ObjectID;
            }

            radCustom.Text = GetString("TemplateLayout.Custom");
            radCustom.Attributes.Add("onclick", "window.location = '" + URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(URLHelper.CurrentURL, "newshared", "0"), "oldshared", drpLayout.Value.ToString()) + "'");

            radShared.Text = GetString("TemplateLayout.Shared");
            if (drpLayout.UniSelector.HasData && drpLayout.UniSelector.HasValue)
            {
                radShared.Attributes.Add("onclick", "window.location = '" + URLHelper.AddParameterToUrl(URLHelper.CurrentURL, "newshared", drpLayout.Value + "") + "'");
            }

            // Get the current layout type
            bool radioButtonsEnabled = !CMSObjectHelper.UseCheckinCheckout;
            if (DeviceProfileID > 0)
            {
                // Device profile layout
                PageTemplateDeviceLayoutInfo dli = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(TemplateID, DeviceProfileID);
                if (dli != null)
                {
                    radioButtonsEnabled |= dli.Generalized.IsCheckedOutByUser(CMSContext.CurrentUser);
                }
            }
            else
            {
                // Page template layout
                radioButtonsEnabled |= PageTemplateInfo.Generalized.IsCheckedOutByUser(CMSContext.CurrentUser);
            }

            // Disable also radio buttons when the object is not checked out
            radShared.Enabled = radCustom.Enabled = radioButtonsEnabled;
        }

        if (AllowTypeSwitching || DialogMode)
        {
            pnlGeneral.Visible = false;
            pnlLayout.ResourceString = null;
        }

        SetActionVisiblity();

        RegisterInitScripts(pnlBody.ClientID, editMenuElem.MenuPanel.ClientID, startWithFullScreen);

        if (QueryHelper.GetBoolean("refreshParent", false))
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshParent", "window.refreshPageOnClose = true;", true);
        }

        HandleWebpartZonesCountWarning();

        if (DialogMode && QueryHelper.GetBoolean("wopenerrefresh", false) && !ValidationHelper.GetBoolean(hdnWOpenerRefreshed.Value, false))
        {
            RegisterWOpenerRefreshScript();
            hdnWOpenerRefreshed.Value = "1";
        }

        var deviceActions = QueryHelper.GetBoolean("deviceactions", true);
        if (!deviceActions)
        {
            if (createDeviceLayout != null)
            {
                createDeviceLayout.Visible = false;
            }
            if (removeDeviceLayout != null)
            {
                removeDeviceLayout.Visible = false;
            }
        }

        // Try to get page template
        EditForm.EnableByLockState();

        // Enable DDL and disable UIForm for shared layouts
        if (radShared.Checked)
        {
            codeElem.Enabled = false;
            cssEditor.Editor.Enabled = false;
            codeLayoutElem.Enabled = false;
            cssLayoutEditor.Editor.Enabled = false;
            deviceCode.Enabled = false;
            cssDeviceEditor.Editor.Enabled = false;
        }
        else
        {
            drpLayout.Enabled = false;
        }

        // Check whether virtual objects are allowed
        if (!VirtualPathHelper.VirtualObjectsAllowed)
        {
            ShowWarning(GetString("VirtualPathProvider.NotRunning"), null, null);
        }
    }


    /// <summary>
    /// Handles the OnAfterAction event of the ObjectManager control.
    /// </summary>
    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if ((e.ActionName == ComponentEvents.SAVE) || (e.ActionName == ComponentEvents.CHECKIN))
        {
            if (EditForm.ValidateData())
            {
                LayoutInfo li = EditedObject as LayoutInfo;

                if (radShared.Checked)
                {
                    // Get the current layout type
                    PageTemplateLayoutTypeEnum layoutType = PageTemplateLayoutTypeEnum.PageTemplateLayout;
                    PageTemplateDeviceLayoutInfoProvider.GetLayoutObject(PageTemplateInfo, DeviceProfileInfo, out layoutType);

                    switch (layoutType)
                    {
                        case PageTemplateLayoutTypeEnum.PageTemplateLayout:
                        case PageTemplateLayoutTypeEnum.SharedLayout:
                            {
                                int newLayoutId = ValidationHelper.GetInteger(drpLayout.Value, 0);

                                // We need to save also page template if shared template is used
                                if ((PageTemplateInfo != null) && (PageTemplateInfo.LayoutID != li.LayoutId))
                                {
                                    PageTemplateInfo.LayoutID = newLayoutId;
                                    PageTemplateInfoProvider.SetPageTemplateInfo(PageTemplateInfo);
                                }
                            }
                            break;

                        case PageTemplateLayoutTypeEnum.DeviceSharedLayout:
                        case PageTemplateLayoutTypeEnum.DeviceLayout:
                            {
                                // We need to save also template device layout if shared template is used
                                PageTemplateDeviceLayoutInfo deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(TemplateID, DeviceProfileID);
                                if (deviceLayout != null)
                                {
                                    deviceLayout.LayoutID = ValidationHelper.GetInteger(drpLayout.Value, 0);
                                    deviceLayout.LayoutCode = null;
                                    deviceLayout.LayoutCSS = null;
                                    PageTemplateDeviceLayoutInfoProvider.SetTemplateDeviceLayoutInfo(deviceLayout);
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }

                // Register refresh script
                string refreshScript = ScriptHelper.GetScript("if ((wopener != null) && (wopener.RefreshPage != null)) {wopener.RefreshPage();}");
                ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "pageTemplateRefreshScript", refreshScript);

                // Register preview refresh
                RegisterRefreshScript();

                // Close if required
                if (ValidationHelper.GetBoolean(hdnClose.Value, false))
                {
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseDialogPreviewScript", ScriptHelper.GetScript("CloseDialog();"));
                }

                // Load actual layout info (it was edited during saving by LayoutInfoProvider)
                if (li != null)
                {
                    actualLayoutInfo = LayoutInfoProvider.GetLayoutInfo(li.LayoutId);
                }
            }

            // Hide warning after save
            MessagesPlaceHolder.WarningText = "";
        }
        else if (e.ActionName == ComponentEvents.UNDO_CHECKOUT)
        {
            if (AllowTypeSwitching)
            {
                var url = URLHelper.CurrentURL;
                url = URLHelper.RemoveParameterFromUrl(url, "newshared");
                url = URLHelper.RemoveParameterFromUrl(url, "oldshared");
                url = URLHelper.AddParameterToUrl(url, "wopenerrefresh", "1");
                Response.Redirect(url);
            }
        }
        else if (e.ActionName == ComponentEvents.CHECKOUT)
        {
            DisplayMessage(true);
        }

        switch (e.ActionName)
        {
            case ComponentEvents.SAVE:
            case ComponentEvents.CHECKOUT:
            case ComponentEvents.CHECKIN:
            case ComponentEvents.UNDO_CHECKOUT:
                if (DialogMode)
                {
                    RegisterWOpenerRefreshScript();
                }
                else if (dialog)
                {
                    ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "parentWOpenerRefresh", ScriptHelper.GetScript("if (parent && parent.wopener && parent.wopener.refresh) { parent.wopener.refresh(); }"));
                }
                break;
        }

        if (!AllowTypeSwitching && (EditedObjectType == EditedObjectTypeEnum.Layout) && (e.ActionName != ComponentEvents.CHECKOUT) && !DialogMode)
        {
            ScriptHelper.RefreshTabHeader(Page, null);
        }
    }


    /// <summary>
    /// Layout DropDownlist change.
    /// </summary>
    protected void selectShared_Changed(object sender, EventArgs ea)
    {
        if (EditedObject != null)
        {
            Response.Redirect(URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(URLHelper.CurrentURL, "newshared", drpLayout.Value + ""), "oldshared", EditedObject.Generalized.ObjectID.ToString()));
        }
    }


    /// <summary>
    /// Ensures full screen mode for preview.
    /// </summary>
    private void HandleFullScreen()
    {
        startWithFullScreen = ((previewState != 0) && editMenuElem.ObjectManager.IsObjectChecked());

        // Wrong calculation for these browsers, when div is hidden.
        bool hide = (BrowserHelper.IsSafari() || BrowserHelper.IsChrome());
        pnlBody.Attributes["style"] = (startWithFullScreen && !hide) ? "display: none;" : "display: block;";
    }


    /// <summary>
    /// Shows warning if there is wrong web part zone count set.
    /// </summary>
    private void HandleWebpartZonesCountWarning()
    {
        if (actualLayoutInfo == null)
        {
            actualLayoutInfo = EditedObject as LayoutInfo;
        }
        FormEngineUserControl zonesCountControl = EditFormLayout.FieldControls["layoutzonecount"];
        if ((actualLayoutInfo == null) || (zonesCountControl == null) || (!zonesCountControl.Visible))
        {
            return;
        }

        // Handle difference between counted and entered number of web part zones
        if (actualLayoutInfo.LayoutZoneCountAutomatic != actualLayoutInfo.LayoutZoneCount)
        {
            string msg = (actualLayoutInfo.LayoutZoneCount >= 0) ? String.Format(ResHelper.GetString("pagelayout.webpartzonescountnotmatch"), actualLayoutInfo.LayoutZoneCount, actualLayoutInfo.LayoutZoneCountAutomatic) : ResHelper.GetString("pagelayout.webpartzonescountmissing");
            ShowWarning(msg, null, null);
        }
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitializeHeaderActions()
    {
        // Add preview action
        HeaderAction preview = new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.LinkButton,
            Text = GetString("general.preview"),
            OnClientClick = "performToolbarAction('split');return false;",
            ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/Preview.png"),
            SmallImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/16/Preview.png"),
            Visible = (previewState == 0),
            Tooltip = GetString("preview.tooltip")
        };
        editMenuElem.ObjectEditMenu.AddExtraAction(preview);

        // Custom device layout
        if (allowDeviceButtons && (DialogMode || requiresDialog) && (DeviceProfileID > 0))
        {
            // "Create device layout" button
            string dialogUrl = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageLayouts/PageLayout_CustomDeviceLayout.aspx?dialog=1");
            dialogUrl += "&templateid=" + TemplateID + "&deviceprofileid=" + DeviceProfileID;

            // Create header button
            createDeviceLayout = new HeaderAction()
            {
                ControlType = HeaderActionTypeEnum.LinkButton,
                Text = GetString("devicelayout.create"),
                Tooltip = GetString("devicelayout.create.tooltip"),
                OnClientClick = "modalDialog('" + dialogUrl + "', 'createCustomDeviceLayout', 450, 420); return false;",
                ImageUrl = GetImageUrl("Objects/CMS_DeviceProfile/add.png"),
                SmallImageUrl = GetImageUrl("Objects/CMS_DeviceProfile/add.png"),
                Visible = false
            };
            editMenuElem.ObjectEditMenu.AddExtraAction(createDeviceLayout);

            // "Remove device layout" button
            removeDeviceLayout = new HeaderAction()
            {
                ControlType = HeaderActionTypeEnum.LinkButton,
                OnClientClick = "javascript:if (confirm('" + GetString("devicelayout.remove.confirmation") + "')) { window.refreshPageOnClose = true; return true; } return false;",
                Text = GetString("devicelayout.remove"),
                Tooltip = GetString("devicelayout.remove.tooltip"),
                ImageUrl = GetImageUrl("Objects/CMS_DeviceProfile/delete.png"),
                SmallImageUrl = GetImageUrl("Objects/CMS_DeviceProfile/delete.png"),
                CommandName = "removelayout",
                Visible = false
            };
            editMenuElem.ObjectEditMenu.AddExtraAction(removeDeviceLayout);

            editMenuElem.ObjectEditMenu.HeaderActions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);
        }
    }


    /// <summary>
    /// Handles the ActionPerformed event of the HeaderActions control.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "removelayout":
                // Remove custom device layout
                PageTemplateDeviceLayoutInfo deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(TemplateID, DeviceProfileID);
                PageTemplateDeviceLayoutInfoProvider.DeleteTemplateDeviceLayoutInfo(deviceLayout);

                ShowConfirmation(GetString("devicelayout.removed"));

                // Refresh the current page
                string refreshUrl = URLHelper.AddParameterToUrl(URLHelper.RawUrl, "refreshParent", "1");
                URLHelper.Redirect(refreshUrl);
                break;
        }
    }


    /// <summary>
    /// Sets the visibility of actions according to checkout info
    /// </summary>
    private void SetActionVisiblity()
    {
        // Show "Create/Remove device layout" buttons
        if (allowDeviceButtons && (DialogMode || requiresDialog) && (DeviceProfileID > 0))
        {
            PageTemplateDeviceLayoutInfo deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(TemplateID, DeviceProfileID);
            if (deviceLayout != null)
            {
                removeDeviceLayout.Visible = true;
            }
            else
            {
                createDeviceLayout.Visible = true;
            }
        }
    }


    /// <summary>
    /// Registers script for header resize
    /// </summary>
    private void RegisterScripts()
    {
        // Register action script for dialog purposes
        if (DialogMode)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "PreviewHierarchyPerformAction", ScriptHelper.GetScript("function actionPerformed(action) { if (action == 'saveandclose') { document.getElementById('" + hdnClose.ClientID + "').value = '1'; }; " + editMenuElem.ObjectManager.GetJSFunction(ComponentEvents.SAVE, null, null) + "; }"));
        }
        if (RequestHelper.IsPostBack())
        {
            // Set timeout to prevent count header size when not properly initialized (f.e. saved action placeholder is not properly counted)
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "ResizeHeader", ScriptHelper.GetScript("window.setTimeout(function(){if (typeof (ResizeHeaderButtons) == 'function')ResizeHeaderButtons() ;},1);"));
        }
    }


    private void RegisterWOpenerRefreshScript()
    {
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "wOpenerRefresh", ScriptHelper.GetScript("if (wopener && wopener.refresh) { wopener.refresh(); }"));
    }


    /// <summary>
    /// Shows the shared template warning message for re-usable templates.
    /// </summary>
    /// <param name="pti">The page template info object</param>
    private void ShowSharedTemplateWarningMessage(PageTemplateInfo pti)
    {
        if ((pti != null) && pti.IsReusable && ShowSharedLayoutWarnings)
        {
            ShowInformation(GetString("template.shared.warning"));
        }
    }

    #endregion
}

