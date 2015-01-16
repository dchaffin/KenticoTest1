using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSMessages_KickedUser : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        titleElem.TitleText = GetString("kicked.header");
        Page.Title = GetString("kicked.header");
        titleElem.TitleImage = GetImageUrl("Others/Messages/denied.png");

        lblInfo.Text = String.Format(GetString("kicked.info"), SettingsKeyProvider.GetIntValue("CMSDenyLoginInterval"));

        // Back link
        lnkBack.Text = GetString("general.Back");
        lnkBack.NavigateUrl = "~/";
    }
}