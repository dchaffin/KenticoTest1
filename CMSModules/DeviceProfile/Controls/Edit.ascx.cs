using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.PortalEngine;
using CMS.CMSHelper;
using System.Collections;
using CMS.GlobalHelper;

public partial class CMSModules_DeviceProfile_Controls_Edit : CMSAdminEditControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        editForm.OnBeforeSave += editForm_OnBeforeSave;
    }


    protected void editForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Initialize order for new profile
        if (editForm.Mode == FormModeEnum.Insert)
        {
            editForm.Data["ProfileOrder"] = DeviceProfileInfoProvider.GetLastProfileOrder() + 1;
        }
    }
}