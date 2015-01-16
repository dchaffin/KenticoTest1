using System;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

[Tabs("deviceLayoutContent")]
public partial class CMSModules_PortalEngine_UI_PageTemplates_DeviceLayout_Header : CMSEditTemplatePage
{
    private int deviceProfileId = 0;
    private bool isDialog = false;


    protected override void OnPreInit(EventArgs e)
    {
        deviceProfileId = QueryHelper.GetInteger("deviceprofileid", 0);
        var deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(PageTemplateID, deviceProfileId);

        // Check if the object exists - if not -> redirect and inform the user
        if (deviceLayout != null)
        {
            EditedObject = deviceLayout;
        }

        isDialog = QueryHelper.GetBoolean("editonlycode", false);

        if (isDialog)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/TabsHeader.master";
        }

        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (PageTemplate == null)
        {
            return;
        }

        var deviceProfile = DeviceProfileInfoProvider.GetDeviceProfileInfo(deviceProfileId);
        if (deviceProfile == null)
        {
            return;
        }

        var currentDeviceProfile = HTMLHelper.HTMLEncode(deviceProfile.ProfileDisplayName);
        var aliasPath = QueryHelper.GetString("aliaspath", null);
        var aliasPathParam = string.IsNullOrEmpty(aliasPath) ? string.Empty : "&aliaspath=" + aliasPath;

        if (isDialog)
        {
            CurrentMaster.Title.TitleText = GetString("objecttype.cms_templatedevicelayout");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_DeviceProfile/object.png");
        }
        else
        {
            var breadcrumbs = new string[2, 3];

            breadcrumbs[0, 0] = GetString("devicelayout.devicelayouts");
            breadcrumbs[0, 1] = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_DeviceLayouts.aspx?templateid=" + PageTemplateID + aliasPathParam);
            breadcrumbs[0, 2] = "_parent";

            breadcrumbs[1, 0] = HTMLHelper.HTMLEncode(currentDeviceProfile);
            breadcrumbs[1, 1] = "";
            breadcrumbs[1, 2] = "";

            CurrentMaster.Title.Breadcrumbs = breadcrumbs;
        }

        CurrentMaster.Title.HelpTopicName = "page_template_device_layout";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Initialize menu
        var i = 0;

        // Layout tab
        var layoutUrl = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageLayouts/PageLayout_Edit.aspx?allowswitch=1&templateid=" + PageTemplateID + "&deviceprofileid=" + deviceProfileId + "&devicename=" + deviceProfile.ProfileName + "&DeviceObjectLifeTime=request" + aliasPathParam);
        layoutUrl = EnsureDialogModeUrlParameter(layoutUrl);
        SetTab(i++, GetString("Administration-PageTemplate_Header.Layouts"), layoutUrl, "SetHelpTopic('helpTopic', 'page_template_device_layout');");

        if (!isDialog)
        {
            // Design tab
            var designUrl = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Design.aspx?templateid=" + PageTemplateID + "&deviceprofileid=" + deviceProfileId + "&devicename=" + deviceProfile.ProfileName + "&DeviceObjectLifeTime=request" + aliasPathParam);
            SetTab(i++, GetString("edittabs.design"), designUrl, "SetHelpTopic('helpTopic', 'page_template_device_design');");
        }
    }


    private string EnsureDialogModeUrlParameter(string url)
    {
        if (isDialog)
        {
            url = URLHelper.AddParameterToUrl(url, "dialogmode", "1");
            url = URLHelper.AddParameterToUrl(url, "tabmode", "1");
        }

        var deviceActions = QueryHelper.GetBoolean("deviceactions", true);
        url = URLHelper.AddParameterToUrl(url, "deviceactions", deviceActions ? "1" : "0");

        return url;
    }
}
