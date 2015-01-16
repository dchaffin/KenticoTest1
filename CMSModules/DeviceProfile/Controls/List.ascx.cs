using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.PortalEngine;
using CMS.GlobalHelper;

using CMS.IO;
using System.Data;

public partial class CMSModules_DeviceProfile_Controls_List : CMSAdminEditControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        gridProfiles.OnAction += gridProfiles_OnAction;
    }


    void gridProfiles_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "moveup":
                DeviceProfileInfoProvider.MoveProfileUp(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "movedown":
                DeviceProfileInfoProvider.MoveProfileDown(ValidationHelper.GetInteger(actionArgument, 0));
                break;
        }
    }
}