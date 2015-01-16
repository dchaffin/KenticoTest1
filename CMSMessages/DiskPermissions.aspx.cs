using System;
using System.Data;
using System.Collections;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSMessages_DiskPermissions : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string message = GetString("CMSSiteManager.DiskPermissions");

        message = message.Replace("<user>", WindowsIdentity.GetCurrent().Name);
        message = message.Replace("<folder>", Server.MapPath("~/."));

        LabelMessage.Text = message;
        titleElem.TitleText = GetString("CMSSiteManager.InvalidDiskPermissions");
        titleElem.TitleImage = GetImageUrl("Others/Messages/info.png");
    }
}