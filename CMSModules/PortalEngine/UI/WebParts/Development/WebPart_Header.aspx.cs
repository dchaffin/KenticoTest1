using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Header : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("Development-WebPart_Header.WebPartTitle");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_WebPart/object.png");
        CurrentMaster.Title.HelpTopicName = "web_parts_main";
        CurrentMaster.Title.HelpName = "helpTopic";
    }
}