using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSModules_Widgets_UI_Widget_Header : SiteManagerPage
{
    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("widgets.title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Widget/object.png");
        CurrentMaster.Title.HelpTopicName = "widgets_main";
        CurrentMaster.Title.HelpName = "helpTopic";
    }

    #endregion
}