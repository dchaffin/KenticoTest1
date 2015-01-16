using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.SiteProvider;
using CMS.UIControls;
using CMS.PortalEngine;
using CMS.IO;

using System.Xml.Serialization;

// Edited object
[EditedObject(PortalObjectType.DEVICEPROFILE, "profileid")]

//Title
[Title("Objects/CMS_DeviceProfile/object.png", "Device_Profile.Title", "device_profiles")]

// Actions
[Actions(1)]
[Action(0, "Objects/CMS_DeviceProfile/add.png", "Device_Profile.NewProfile", "New.aspx")]
public partial class CMSModules_DeviceProfile_Pages_Development_List : CMSDeviceProfilesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSDeviceProfilesEnable";
        ucDisabledModule.InfoText = GetString("Device_Profile.ModuleDisabled");
    }
}