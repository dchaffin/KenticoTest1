using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSPages_DevicePreview : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = "";
        ucView.RotateDevice = ValidationHelper.GetBoolean(CookieHelper.GetValue("CMSDeviceProfileRotate"), false);       
    }

    protected override void OnPreRender(EventArgs e)
    {
        HeaderAction rotate = new HeaderAction
        {
            ControlType = HeaderActionTypeEnum.LinkButton,
            Text = GetString("devicepreview.rotate"),
            ImageUrl = GetImageUrl("Objects/CMS_DeviceProfile/rotate.png"),
            SmallImageUrl = GetImageUrl("Objects/CMS_DeviceProfile/rotate.png"),
            OnClientClick="CMSViewValidate.RotateDevice();return false;",
            Visible = true
        };

        CurrentMaster.HeaderActions.ActionsList.Add(rotate);
        CurrentMaster.HeaderActions.IsLiveSite = false;

        base.OnPreRender(e);
    }
}
