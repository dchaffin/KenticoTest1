using System;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.PortalEngine;

public partial class CMSModules_PortalEngine_UI_PageTemplates_DeviceLayout_Frameset : CMSEditTemplatePage
{
    protected int headerHeight = 0;

    protected string deviceLayoutHeaderUrl = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/DeviceLayout_Header.aspx?templateid={0}&deviceprofileid={1}");
    protected string deviceLayoutContentUrl = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageLayouts/PageLayout_Edit.aspx?allowswitch=1&templateid={0}&deviceprofileid={1}&devicename={2}&DeviceObjectLifeTime=request");


    protected override void OnPreInit(EventArgs e)
    {
        var isDialog = QueryHelper.GetBoolean("editonlycode", false);
        headerHeight = isDialog ? TabsFrameHeight : TabsBreadFrameHeight;

        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var deviceProfileId = QueryHelper.GetInteger("deviceprofileid", 0);
        if (deviceProfileId == 0)
        {
            var deviceLayoutId = QueryHelper.GetInteger("devicelayoutid", 0);
            var deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(deviceLayoutId);
            if (deviceLayout != null)
            {
                deviceProfileId = deviceLayout.ProfileID;
            }
        }

        var deviceProfile = DeviceProfileInfoProvider.GetDeviceProfileInfo(deviceProfileId);

        if (deviceProfile != null)
        {
            deviceLayoutHeaderUrl = string.Format(deviceLayoutHeaderUrl, PageTemplateID, deviceProfileId);
            deviceLayoutContentUrl = string.Format(deviceLayoutContentUrl, PageTemplateID, deviceProfileId, deviceProfile.ProfileName);
        }

        var editOnlyCode = QueryHelper.GetBoolean("editonlycode", false);
        if (editOnlyCode)
        {
            deviceLayoutHeaderUrl = URLHelper.AddParameterToUrl(deviceLayoutHeaderUrl, "editonlycode", "1");
            deviceLayoutContentUrl = URLHelper.AddParameterToUrl(deviceLayoutContentUrl, "dialogmode", "1");
            deviceLayoutContentUrl = URLHelper.AddParameterToUrl(deviceLayoutContentUrl, "tabmode", "1");
        }

        var deviceActions = QueryHelper.GetBoolean("deviceactions", true);
        deviceLayoutHeaderUrl = URLHelper.AddParameterToUrl(deviceLayoutHeaderUrl, "deviceactions", deviceActions ? "1" : "0");
        deviceLayoutContentUrl = URLHelper.AddParameterToUrl(deviceLayoutContentUrl, "deviceactions", deviceActions ? "1" : "0");

        // Include the AliasPath parameter to ensure path pre-selection in the preview control
        string aliasPath = QueryHelper.GetString("aliaspath", null);
        if (!string.IsNullOrEmpty(aliasPath))
        {
            deviceLayoutHeaderUrl = URLHelper.AddParameterToUrl(deviceLayoutHeaderUrl, "aliaspath", aliasPath);
            deviceLayoutContentUrl = URLHelper.AddParameterToUrl(deviceLayoutContentUrl, "aliaspath", aliasPath);
        }

        string aliasPathParam = (string.IsNullOrEmpty(aliasPath) ? string.Empty : "&aliaspath=" + aliasPath);

        string script = @"
function RefreshAfterDelete() {
    location.href = '" + URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_DeviceLayouts.aspx?templateid=" + PageTemplateID + aliasPathParam) + @"';
}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "refreshGrid", script, true);
    }
}
