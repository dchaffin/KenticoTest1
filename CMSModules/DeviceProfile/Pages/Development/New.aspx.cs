using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "device_profile.title", "List.aspx", null)]
[Breadcrumb(1, "device_profile.new")]

// Title
[Title("Objects/CMS_DeviceProfile/new.png", "Device_Profile_New.HeaderCaption", "new_device_profile")]
public partial class CMSModules_DeviceProfile_Pages_Development_New : CMSDeviceProfilesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
}