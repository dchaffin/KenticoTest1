using System;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.VirtualPathHelper;

public partial class CMSModules_PortalEngine_Controls_Layout_General : CMSPreviewControl
{
    #region "Variables"

    protected string aliasPath = QueryHelper.GetString("aliaspath", "");
    protected string webpartId = QueryHelper.GetString("webpartid", "");
    protected string zoneId = QueryHelper.GetString("zoneid", "");
    protected Guid instanceGuid = QueryHelper.GetGuid("instanceguid", Guid.Empty);
    protected int templateId = QueryHelper.GetInteger("templateid", 0);
    protected bool isNewVariant = QueryHelper.GetBoolean("isnewvariant", false);
    protected int variantId = QueryHelper.GetInteger("variantid", 0);
    protected int zoneVariantId = QueryHelper.GetInteger("zonevariantid", 0);
    protected string culture = QueryHelper.GetString("culture", CMSContext.PreferredCultureCode);
    protected VariantModeEnum variantMode = VariantModeFunctions.GetVariantModeEnum(QueryHelper.GetString("variantmode", string.Empty));
    protected bool isSiteManager = false;
    bool isNew = false;
    bool isDefault = false;

    protected bool startWithFullScreen = true;
    CurrentUserInfo currentUser = null;

    private WebPartInfo webPartInfo = null;

    /// <summary>
    /// Current page info.
    /// </summary>
    private PageInfo pi = null;

    /// <summary>
    /// Page template info.
    /// </summary>
    private PageTemplateInfo pti = null;


    WebPartLayoutInfo wpli = null;
    WebPartInfo wpi = null;

    /// <summary>
    /// Current web part.
    /// </summary>
    private WebPartInstance webPart = null;

    private string mLayoutCodeName = null;
    int previewState = 0;
    int layoutID = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets code name of edited layout.
    /// </summary>
    private string LayoutCodeName
    {
        get
        {
            return mLayoutCodeName ?? (mLayoutCodeName = QueryHelper.GetString("layoutcodename", string.Empty));
        }
        set
        {
            mLayoutCodeName = value;
        }
    }


    /// <summary>
    /// Returns true if object is checked out or checkin/out is not used 
    /// </summary>
    public bool IsChecked
    {
        get
        {
            CMSObjectManager om = ControlsHelper.GetChildControl(Page, typeof(CMSObjectManager)) as CMSObjectManager;
            if (om != null)
            {
                return om.IsObjectChecked();
            }

            return false;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return editMenuElem.MessagesPlaceHolder;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnLoad override, setup access denied page with dependence on current usage.
    /// </summary>    
    protected override void OnLoad(EventArgs e)
    {
        if (ShowPreview)
        {
            previewState = GetPreviewStateFromCookies(WEBPARTLAYOUT);
        }

        if (!RequestHelper.IsPostBack() && IsChecked)
        {
            ShowMessage();
        }


        if (previewState != 0)
        {
            etaCode.TopOffset = 40;
        }

        // Add preview action
        if (!isNew)
        {
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
        }

        editMenuElem.ObjectEditMenu.PreviewMode = true;
        editMenuElem.MenuPanel.CssClass = "PreviewMenu";
        editMenuElem.ObjectManager.OnBeforeAction += ObjectManager_OnBeforeAction;
        editMenuElem.ObjectManager.OnAfterAction += ObjectManager_OnAfterAction;
        editMenuElem.ObjectManager.OnSaveData += ObjectManager_OnSaveData;

        if (!currentUser.IsAuthorizedPerResource("CMS.Design", "EditCode"))
        {
            editMenuElem.MessagesPlaceHolder.WrapperControlClientID = pnlBody.ClientID;

            etaCode.Editor.ReadOnly = true;
        }

        base.OnLoad(e);
    }


    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        // Do not check changes
        DocumentManager.RegisterSaveChangesScript = false;

        EditForm.OnBeforeSave += EditForm_OnBeforeSave;

        etaCode.Editor.Language = LanguageEnum.HTML;
        etaCSS.Editor.Language = LanguageEnum.CSS;

        wpli = CMSContext.EditedObject as WebPartLayoutInfo;

        layoutID = QueryHelper.GetInteger("layoutid", 0);

        isSiteManager = ((CMSContext.CurrentUser.IsGlobalAdministrator && layoutID != 0) || QueryHelper.GetBoolean("sitemanager", false));
        isNew = (LayoutCodeName == "|new|");
        isDefault = (LayoutCodeName == "|default|") || (!isSiteManager && string.IsNullOrEmpty(LayoutCodeName));

        if ((wpli == null) && isSiteManager)
        {
            isNew = true;
        }

        if (wpli == null)
        {
            editMenuElem.ObjectManager.ObjectType = PredefinedObjectType.WEBPARTLAYOUT;
        }

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "PreviewHierarchyPerformAction", ScriptHelper.GetScript("function actionPerformed(action) { if (action == 'saveandclose') { document.getElementById('" + hdnClose.ClientID + "').value = '1'; } " + editMenuElem.ObjectManager.GetJSFunction(ComponentEvents.SAVE, null, null) + "; }"));

        RegisterResizeHeaders();

        currentUser = CMSContext.CurrentUser;

        // Get webpart instance (if edited in CMSDesk)
        if ((webpartId != "") && !isSiteManager)
        {
            // Get pageinfo
            pi = CMSWebPartPropertiesPage.GetPageInfo(aliasPath, templateId, culture);
            if (pi == null)
            {
                ShowInformation(GetString("WebPartProperties.WebPartNotFound"), null, false);
            }

            // Get page template
            pti = pi.UsedPageTemplateInfo;
            if ((pti != null) && ((pti.TemplateInstance != null)))
            {
                webPart = pti.TemplateInstance.GetWebPart(instanceGuid, zoneVariantId, variantId) ?? pti.GetWebPart(webpartId);
            }
        }

        // If the web part is not found, try webpart ID
        if (webPart == null)
        {
            // Site manager
            wpi = WebPartInfoProvider.GetWebPartInfo(ValidationHelper.GetInteger(webpartId, 0));
            if (wpi == null)
            {
                ShowError(GetString("WebPartProperties.WebPartNotFound"), null, null);
                startWithFullScreen = false;
                return;
            }
        }
        else
        {
            // CMS desk
            wpi = WebPartInfoProvider.GetWebPartInfo(webPart.WebPartType);
            if (string.IsNullOrEmpty(LayoutCodeName))
            {
                // Get the current layout name
                LayoutCodeName = ValidationHelper.GetString(webPart.GetValue("WebPartLayout"), "");
            }
        }

        if (wpi != null)
        {
            if (CMSObjectHelper.UseCheckinCheckout && (!isNew || isSiteManager))
            {
                pnlFormArea.CssClass = "PreviewDefaultContentLarge";
            }
            else
            {
                pnlFormArea.CssClass = "PreviewDefaultContent";
            }

            // Load the web part information
            webPartInfo = wpi;
            bool loaded = false;

            if (!RequestHelper.IsPostBack())
            {
                if (wpli != null)
                {
                    editMenuElem.MenuPanel.Visible = true;

                    // Read-only code text area
                    etaCode.Editor.ReadOnly = false;
                    loaded = true;
                }

                if (!loaded)
                {
                    string fileName = webPartInfo.WebPartFileName;

                    if (webPartInfo.WebPartParentID > 0)
                    {
                        WebPartInfo pwpi = WebPartInfoProvider.GetWebPartInfo(webPartInfo.WebPartParentID);
                        if (pwpi != null)
                        {
                            fileName = pwpi.WebPartFileName;
                        }
                    }

                    if (!fileName.StartsWithCSafe("~"))
                    {
                        fileName = "~/CMSWebparts/" + fileName;
                    }

                    // Check if filename exist
                    if (!FileHelper.FileExists(fileName))
                    {
                        ShowError(GetString("WebPartProperties.FileNotExist"), null, null);
                        plcContent.Visible = false;
                    }
                    else
                    {
                        // Load default web part layout code
                        etaCode.Text = File.ReadAllText(Server.MapPath(fileName));

                        // Load default web part CSS
                        etaCSS.Text = wpi.WebPartCSS;
                        startWithFullScreen = false;
                    }
                }
            }
        }

        if ((wpli == null) && isSiteManager)
        {
            pnlFormArea.CssClass += " NewPreviewContent";

            var breadcrumbs = new string[2, 3];
            breadcrumbs[0, 0] = GetString("WebParts.Layout");
            breadcrumbs[0, 1] = ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/Development/WebPart_Edit_Layout.aspx?webpartid=" + QueryHelper.GetInteger("webpartid", 0));
            breadcrumbs[1, 0] = GetString("webparts_layout_newlayout");
            editMenuElem.PageTitleBreadcrumbs = breadcrumbs;
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ApplyButton", ScriptHelper.GetScript(
            "function SetRefresh(refreshpage) { document.getElementById('" + hidRefresh.ClientID + @"').value = refreshpage; }
             function OnApplyButton(refreshpage) { SetRefresh(refreshpage); actionPerformed('save');refreshPreview(); }  
             function OnOKButton(refreshpage) { SetRefresh(refreshpage); actionPerformed('saveandclose'); } "));

        InitLayoutForm();

        plcCssLink.Visible = string.IsNullOrEmpty(etaCSS.Text.Trim());

        base.OnInit(e);
    }


    /// <summary>
    /// Display info message
    /// </summary>
    public void ShowMessage()
    {
        // Check the edit code permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Design", "EditCode"))
        {
            ShowWarning(GetString("EditCode.NotAllowed"), null, null);
        }
    }


    /// <summary>
    /// Handles the OnBeforeAction event of the ObjectManager control.
    /// </summary>
    protected void ObjectManager_OnBeforeAction(object sender, SimpleObjectManagerEventArgs e)
    {
        switch (e.ActionName)
        {
            case ComponentEvents.SAVE:
                if ((webPart != null) && isDefault)
                {
                    if (!isSiteManager)
                    {
                        SetCurrentLayout(true);

                        // Reload the parent page after save
                        EnsureParentPageRefresh();

                        if (ValidationHelper.GetBoolean(hdnClose.Value, false))
                        {
                            // If window to close, register close script
                            CloseDialog();
                        }
                        else
                        {
                            ShowChangesSaved();
                        }
                    }

                    // Do not save default layout
                    e.IsValid = false;
                }
                break;

            case ComponentEvents.CHECKOUT:
                ShowMessage();
                break;
        }
    }


    /// <summary>
    /// Handles the OnAfterAction event of the ObjectManager control.
    /// </summary>
    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        wpli = EditForm.EditedObject as WebPartLayoutInfo;

        if (wpli != null)
        {
            LayoutCodeName = wpli.WebPartLayoutCodeName;
        }

        if (e.ActionName == ComponentEvents.SAVE)
        {
            if (EditForm.ValidateData())
            {
                if (!isSiteManager)
                {
                    SetCurrentLayout(true);
                }

                if (ValidationHelper.GetBoolean(hdnClose.Value, false))
                {
                    // If window to close, register close script
                    CloseDialog();
                }
                else
                {
                    // Redirect parent for new
                    if (isNew)
                    {
                        if (isSiteManager)
                        {
                            URLHelper.Redirect("WebPart_Edit_Layout_Frameset.aspx?layoutID=" + wpli.WebPartLayoutID + "&webpartID=" + webPartInfo.WebPartID);
                        }
                        else
                        {
                            var codeName = (wpli != null) ? wpli.WebPartLayoutCodeName : string.Empty;
                            var redirectUrl = ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/WebPartProperties_layout_frameset.aspx") + URLHelper.UpdateParameterInUrl(URLHelper.Url.Query, "layoutcodename", codeName);
                            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "RefreshHeader", ScriptHelper.GetScript("parent.location ='" + redirectUrl + "'"));
                        }
                    }
                    else
                    {
                        if (!isSiteManager)
                        {
                            // Reload the parent page after save
                            EnsureParentPageRefresh();
                        }

                        // If all ok show changes saved
                        ShowChangesSaved();
                        RegisterRefreshScript();
                    }
                }
            }

            // Clear warning text
            editMenuElem.MessagesPlaceHolder.WarningText = "";
        }

        if (DialogMode)
        {
            switch (e.ActionName)
            {
                case ComponentEvents.SAVE:
                case ComponentEvents.CHECKOUT:
                case ComponentEvents.UNDO_CHECKOUT:
                case ComponentEvents.CHECKIN:
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "wopenerRefresh", ScriptHelper.GetScript("if (wopener && wopener.refresh) { wopener.refresh(); }"));
                    break;
            }
        }

        if (isSiteManager && (e.ActionName != ComponentEvents.CHECKOUT))
        {
            ScriptHelper.RefreshTabHeader(Page, null);
        }
    }


    /// <summary>
    /// Handles the OnBeforeSave event of the EditForm control.
    /// </summary>
    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Remove "." due to virtual path provider replacement            
        txtCodeName.Text = txtCodeName.Text.Replace(".", "");

        WebPartLayoutInfo li = EditForm.EditedObject as WebPartLayoutInfo;
        if (li != null)
        {
            if (!currentUser.IsAuthorizedPerResource("CMS.Design", "EditCode"))
            {
                li.WebPartLayoutCode = GetDefaultCode();
            }

            if (webPartInfo != null)
            {
                li.WebPartLayoutWebPartID = webPartInfo.WebPartID;
            }
        }
    }


    /// <summary>
    /// Handles the OnSaveData event of the ObjectManager control.
    /// </summary>
    protected void ObjectManager_OnSaveData(object sender, SimpleObjectManagerEventArgs e)
    {
        try
        {
            ComponentEvents.RequestEvents.RaiseComponentEvent(this, e, ComponentName, DocumentComponentEvents.SAVE_DATA, e.ActionName);
        }
        catch (WebPartLayoutException ex)
        {
            ShowError(ex.Message);
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        startWithFullScreen = ((previewState != 0) && editMenuElem.ObjectManager.IsObjectChecked());

        // Wrong calculation for these browsers, when div is hidden.
        bool hide = (BrowserHelper.IsSafari() || BrowserHelper.IsChrome());
        pnlBody.Attributes["style"] = (startWithFullScreen && !hide) ? "display:none" : "display:block";

        // Check whether virtual objects are allowed
        if (!VirtualPathHelper.VirtualObjectsAllowed)
        {
            ShowWarning(GetString("VirtualPathProvider.NotRunning"), null, null);
        }

        RegisterInitScripts(pnlBody.ClientID, editMenuElem.MenuPanel.ClientID, startWithFullScreen);
    }


    /// <summary>
    /// Selected index changed.
    /// </summary>
    private void InitLayoutForm()
    {
        if (wpli == null)
        {
            if (!RequestHelper.IsPostBack())
            {
                // Prefill with default layout
                etaCode.Text = GetDefaultCode();
            }

            if ((LayoutCodeName == "|new|") || isSiteManager)
            {
                // New layout
                plcDescription.Visible = true;
                plcValues.Visible = true;
                etaCode.Editor.ReadOnly = false;
                etaCSS.Editor.ReadOnly = false;
                etaCode.Editor.Height = Unit.Pixel(200);
                editMenuElem.MenuPanel.Visible = true;
                lnkStyles.Visible = true;

                if (!RequestHelper.IsPostBack())
                {
                    etaCSS.Text = webPartInfo.WebPartCSS;
                }
            }
            else
            {
                // Default layout
                etaCode.Editor.ReadOnly = true;
                etaCSS.Editor.ReadOnly = true;
                lnkStyles.Visible = false;
                editMenuElem.MenuPanel.Visible = false;
                pnlFormArea.Attributes["style"] = "";

                // Tell parent preview hierarchy control to hide preview
                ShowPreview = false;
                previewState = 0;
                pnlFormArea.CssClass = "";
                etaCode.Editor.Height = Unit.Pixel(300);
            }
        }
        else
        {
            etaCode.Editor.Height = Unit.Pixel(300);
            plcDescription.Visible = true;
            plcValues.Visible = (layoutID != 0);
            etaCode.Editor.ReadOnly = false;
            editMenuElem.MenuPanel.Visible = true;

            if (wpli != null)
            {
                etaCSS.Editor.ReadOnly = false;
            }
        }

        // Ensure that hidden fields are truly hidden and not validated
        foreach (var field in EditForm.Fields)
        {
            var formControl = EditForm.FieldControls[field];
            if (!formControl.Visible)
            {
                EditForm.FieldsToHide.Add(field);
            }
        }
    }


    /// <summary>
    /// Gets the default layout code for the web part
    /// </summary>
    protected string GetDefaultCode()
    {
        string fileName = webPartInfo.WebPartFileName;
        if (webPartInfo.WebPartParentID > 0)
        {
            WebPartInfo pwpi = WebPartInfoProvider.GetWebPartInfo(webPartInfo.WebPartParentID);
            if (pwpi != null)
            {
                fileName = pwpi.WebPartFileName;
            }
        }

        if (!fileName.StartsWithCSafe("~"))
        {
            fileName = "~/CMSWebparts/" + fileName;
        }

        string fileFullPath = Server.MapPath(fileName);
        if (File.Exists(fileFullPath))
        {
            return File.ReadAllText(fileFullPath);
        }

        return null;
    }


    /// <summary>
    /// Sets current layout.
    /// </summary>
    protected void SetCurrentLayout(bool saveToWebPartInstance)
    {
        if ((webPart != null) && (LayoutCodeName != "|new|"))
        {
            if (saveToWebPartInstance)
            {
                if (LayoutCodeName == "|default|")
                {
                    webPart.SetValue("WebPartLayout", "");
                }
                else
                {
                    webPart.SetValue("WebPartLayout", LayoutCodeName);
                }

                bool isWebPartVariant = (variantId > 0) || (zoneVariantId > 0) || isNewVariant;
                if (!isWebPartVariant)
                {
                    // Update page template
                    PageTemplateInfoProvider.SetPageTemplateInfo(pti);
                }
                else
                {
                    // Save the variant properties
                    if ((webPart != null)
                        && (webPart.ParentZone != null)
                        && (webPart.ParentZone.ParentTemplateInstance != null)
                        && (webPart.ParentZone.ParentTemplateInstance.ParentPageTemplate != null))
                    {
                        XmlDocument doc = new XmlDocument();
                        XmlNode xmlWebParts = null;

                        if (zoneVariantId > 0)
                        {
                            // This webpart is in a zone variant therefore save the whole variant webparts
                            xmlWebParts = webPart.ParentZone.GetXmlNode(doc);
                            if (webPart.VariantMode == VariantModeEnum.MVT)
                            {
                                ModuleCommands.OnlineMarketingSaveMVTVariantWebParts(zoneVariantId, xmlWebParts);
                            }
                            else if (webPart.VariantMode == VariantModeEnum.ContentPersonalization)
                            {
                                ModuleCommands.OnlineMarketingSaveContentPersonalizationVariantWebParts(zoneVariantId, xmlWebParts);
                            }
                        }
                        else if (variantId > 0)
                        {
                            // This webpart is a web part variant
                            xmlWebParts = webPart.GetXmlNode(doc);
                            if (webPart.VariantMode == VariantModeEnum.MVT)
                            {
                                ModuleCommands.OnlineMarketingSaveMVTVariantWebParts(variantId, xmlWebParts);
                            }
                            else if (webPart.VariantMode == VariantModeEnum.ContentPersonalization)
                            {
                                ModuleCommands.OnlineMarketingSaveContentPersonalizationVariantWebParts(variantId, xmlWebParts);
                            }
                        }
                    }
                }
            }

            string parameters = aliasPath + "/" + zoneId + "/" + webpartId;
            string cacheName = "CMSVirtualWebParts|" + parameters.ToLowerCSafe().TrimStart('/');

            CacheHelper.Remove(cacheName);
        }
    }


    /// <summary>
    /// Registers the client script to close the dialog.
    /// </summary>
    private void CloseDialog()
    {
        bool refresh = ValidationHelper.GetBoolean(hidRefresh.Value, false);

        var script = new StringBuilder();
        if (refresh)
        {
            // Reload the parent page after save
            script.AppendLine("SendEvent('setrefreshpage');");
        }
        script.AppendLine("SendEvent('close');");

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "closeDialog", script.ToString(), true);
    }


    /// <summary>
    /// Refreshes the parent page.
    /// </summary>
    private void EnsureParentPageRefresh()
    {
         ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "LayoutRefreshParent", "SendEvent('setrefreshpage');", true);
    }

    #endregion
}
