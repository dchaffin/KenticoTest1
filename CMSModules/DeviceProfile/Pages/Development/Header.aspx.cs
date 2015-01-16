using System;

using CMS.UIControls;
using CMS.PortalEngine;
using CMS.GlobalHelper;
using CMS.IO;

// Edited object
[EditedObject(PortalObjectType.DEVICEPROFILE, "profileid")]

//Title
[Title("Objects/CMS_DeviceProfile/object.png", "Device_Profile.Title", "Device_Profiles", HelpTopic = "device_profile_general")]
// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "Device_Profile.ItemListLink", "List.aspx", "_parent")]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, "Device_Profile.NewProfile", NewObject = true)]

[Tabs("Content")]
[Tab(0, "general.general", "General.aspx?profileid={?profileid?}", "SetHelpTopic('helpTopic', 'device_profile_general');")]
public partial class CMSModules_DeviceProfile_Pages_Development_Header : CMSDeviceProfilesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeMenu();
    }


    /// <summary>
    /// Initializes tabs.
    /// </summary>
    protected void InitializeMenu()
    {
        int tabIndex = 1;

        DeviceProfileInfo profileInfo = EditedObject as DeviceProfileInfo;
        // Don't display theme tab when using external storage
        if (!StorageHelper.IsExternalStorage(profileInfo.GetThemePath()))
        {
            // Theme tab
            SetTab(tabIndex++, GetString("Stylesheet.Theme"), "Theme.aspx?profileid=" + QueryHelper.GetInteger("profileid", 0), "SetHelpTopic('helpTopic', 'device_profile_theme');");
        }
        SetTab(tabIndex++, GetString("Device_Profile.PageLayouts"), "PageLayouts.aspx?profileid=" + QueryHelper.GetInteger("profileid", 0), "SetHelpTopic('helpTopic', 'device_profile_pagelayouts');");
    }
}