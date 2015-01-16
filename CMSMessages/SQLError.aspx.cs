using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSMessages_SQLError : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        titleElem.TitleText = "SQL troubleshooting help";
        titleElem.TitleImage = GetImageUrl("Others/Messages/info.png");
    }
}