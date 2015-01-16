using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_PageLayouts_PageLayout_Edit : CMSModalDesignPage
{
    #region "Variables"

    private bool dialogMode = false;
    private bool tabMode = false;

    private PageTemplateInfo pti = null;
    private DeviceProfileInfo deviceProfile = null;
    private int templateId = 0;

    #endregion


    #region "Page methods"

    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        deviceProfile = CMSContext.CurrentDeviceProfile;
        templateId = QueryHelper.GetInteger("templateid", 0);
        pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);

        dialogMode = QueryHelper.GetBoolean("dialogmode", false);
        tabMode = QueryHelper.GetBoolean("tabmode", false);

        // Page template dialog in CMS Desk
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "Design.EditLayout"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Design.EditLayout");
        }

        if (dialogMode)
        {
            // Set proper master page for dialog mode
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalSimplePage.master";

            if (!tabMode)
            {
                CurrentMaster.Title.TitleText = GetTitleText();
                CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Layout/object.png");
                CurrentMaster.Title.TitleInformation = GetTitleInformation();

                CurrentMaster.Title.HelpTopicName = "CMS_Design_page_layout";
                CurrentMaster.Title.HelpName = "helpTopic";
            }
        }

        int layoutId = QueryHelper.GetInteger("layoutid", 0);
        int newSharedLayoutId = QueryHelper.GetInteger("newshared", -1);
        int oldSharedLayoutId = QueryHelper.GetInteger("oldshared", -1);
        int deviceProfileId = (deviceProfile != null) ? deviceProfile.ProfileID : 0;

        CMSContext.EditedObject = GetEditedObject(templateId, layoutId, newSharedLayoutId, oldSharedLayoutId, deviceProfileId);

        base.OnPreInit(e);
    }


    /// <summary>
    /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
    /// </summary>
    protected override void CreateChildControls()
    {
        if (dialogMode)
        {
            ucHierarchy.ShowPanelSeparator = tabMode;
            ucHierarchy.DialogMode = true;
            ucHierarchy.AddTitleToToolbar = !tabMode;
            ucHierarchy.ShowSaveButtons = false;

            ScriptHelper.RegisterWOpenerScript(Page);
        }

        // For edit template in properties->template->edit template - (special dialog mode), ignore session and load alias path first
        // Or for dialog mode in site manager, set ignoresessionvalues to false
        ucHierarchy.IgnoreSessionValues = (dialogMode && !String.IsNullOrEmpty(QueryHelper.GetString("aliaspath", String.Empty))) || QueryHelper.GetBoolean("dialog", false);
        ucHierarchy.StorePreviewScrollPosition = true;

        if ((pti != null) && (CMSContext.EditedObject is PageTemplateInfo))
        {
            ucHierarchy.PreviewObjectName = pti.PageTemplateId.ToString();
            ucHierarchy.CookiesPreviewStateName = CMSPreviewControl.PAGETEMPLATELAYOUT;
        }
        else if (CMSContext.EditedObject is LayoutInfo)
        {
            ucHierarchy.PreviewObjectName = (CMSContext.EditedObject as LayoutInfo).LayoutCodeName;
            ucHierarchy.CookiesPreviewStateName = CMSPreviewControl.PAGELAYOUT;
        }
        else
        {
            if (deviceProfile != null)
            {
                ucHierarchy.PreviewObjectName = deviceProfile.ProfileName;
            }
            ucHierarchy.CookiesPreviewStateName = CMSPreviewControl.DEVICELAYUOT;
        }

        // Pass the AllowTypeSwitching setting to the inner control
        ucHierarchy.LayoutPane.Values.Add(new UILayoutValue("AllowTypeSwitching", QueryHelper.GetBoolean("allowswitch", false)));

        base.CreateChildControls();
    }


    /// <summary>
    /// Init event handler
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register client ID for autoresize of codemirror
        ucHierarchy.RegisterEnvelopeClientID();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets the title text.
    /// </summary>
    private string GetTitleText()
    {
        string title = GetString(templateId > 0 ? "pagetemplate.layoutproperties" : "administration-pagelayout_new.editlayout");

        if (deviceProfile != null)
        {
            // Display info about the current device profile
            title += " - " + HTMLHelper.HTMLEncode(deviceProfile.ProfileDisplayName);
        }

        return title;
    }


    /// <summary>
    /// Gets the title text.
    /// </summary>
    private string GetTitleInformation()
    {
        if (deviceProfile != null)
        {
            PageTemplateDeviceLayoutInfo deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(templateId, deviceProfile.ProfileID);
            bool layoutIsMapped = (deviceLayout != null);

            // Check layout auto mapping
            if (!layoutIsMapped && (templateId > 0))
            {
                layoutIsMapped |= (DeviceProfileLayoutInfoProvider.GetTargetLayoutInfo(deviceProfile.ProfileID, pti.LayoutID) != null);
            }

            if (!layoutIsMapped)
            {
                // Device layout not defined
                return GetString("devicelayout.notdefined");
            }
        }

        return null;
    }


    /// <summary>
    /// Gets the edited object.
    /// </summary>
    /// <param name="templateId">The template id - if template/device layout is being edited</param>
    /// <param name="layoutId">The layout id - if shared layout is being edited</param>
    /// <param name="newSharedLayoutId">The new shared layout id - when changing shared layouts for template/device layouts</param>
    /// <param name="oldSharedLayoutId">The old shared layout id - when changing shared layouts for template/device layouts</param>
    /// <param name="deviceProfileId">The device profile id</param>
    private object GetEditedObject(int templateId, int layoutId, int newSharedLayoutId, int oldSharedLayoutId, int deviceProfileId)
    {
        object editedObject = null;

        if (templateId > 0)
        {
            PageTemplateInfo pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);
            DeviceProfileInfo profileInfo = DeviceProfileInfoProvider.GetDeviceProfileInfo(deviceProfileId);
            PageTemplateLayoutTypeEnum type = PageTemplateLayoutTypeEnum.PageTemplateLayout;

            // Standard page template layout
            if (pti != null)
            {
                // Check modify shared templates permission
                if (pti.IsReusable && !CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "Design.ModifySharedTemplates"))
                {
                    RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Design.ModifySharedTemplates");
                }

                // Default state - no radio buttons used
                if ((newSharedLayoutId == -1) && (oldSharedLayoutId == -1))
                {
                    editedObject = PageTemplateDeviceLayoutInfoProvider.GetLayoutObject(pti, profileInfo, out type);
                }
                else
                {
                    // If new shared layout is set, than it should be used as edited object
                    // This happens when switched from custom to a shared layout
                    if (newSharedLayoutId > 0)
                    {
                        // Standard page layout
                        editedObject = LayoutInfoProvider.GetLayoutInfo(newSharedLayoutId);
                    }
                    else if (newSharedLayoutId == 0)
                    {
                        // This means user switched from shared layout to custom
                        // Data has to be copied to PageTemplateInfo

                        PageTemplateDeviceLayoutInfo deviceLayout = null;
                        if (deviceProfileId > 0)
                        {
                            // Get the current device layout if exists
                            deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(templateId, deviceProfileId);
                            if (deviceLayout != null)
                            {
                                // Custom device layout (use old layout)
                                editedObject = PageTemplateDeviceLayoutInfoProvider.CloneInfoObject(deviceLayout, oldSharedLayoutId) as PageTemplateDeviceLayoutInfo;
                            }
                        }
                        else
                        {
                            // We have to work with the clone, because we need to change the data of the object 
                            // (copy from shared layout)
                            editedObject = PageTemplateDeviceLayoutInfoProvider.CloneInfoObject(pti, oldSharedLayoutId) as PageTemplateInfo;
                        }
                    }
                }
            }
        }
        else
        {
            // Load the object
            if (layoutId > 0)
            {
                editedObject = LayoutInfoProvider.GetLayoutInfo(layoutId);
            }
        }

        return editedObject;
    }

    #endregion
}