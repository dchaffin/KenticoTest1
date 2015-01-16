using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.PortalEngine;
using CMS.GlobalHelper;

// Edited object
[EditedObject(PortalObjectType.DEVICEPROFILE, "profileid")]
public partial class CMSModules_DeviceProfile_Pages_Development_Theme : CMSDeviceProfilesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the file system browser
        if (EditedObject != null)
        {
            DeviceProfileInfo profileInfo = EditedObject as DeviceProfileInfo;
            if (profileInfo != null)
            {
                // Ensure the theme folder
                themeElem.Path = profileInfo.GetThemePath();
            }
            else
            {
                themeElem.StopProcessing = true;
                themeElem.Visible = false;
            }
        }
        else
        {
            themeElem.StopProcessing = true;
            themeElem.Visible = false;
        }
    }
}