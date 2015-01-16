using System;
using System.Web.UI;

using CMS.UIControls;

public partial class CMSFormControls_LiveSelectors_InsertYouTubeVideo_Content : CMSLiveModalPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Page.Title = GetString("dialogs.youtube.inserttitle");
        CurrentMaster.Title.TitleText = GetString("dialogs.youtube.inserttitle");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Content/Dialogs/YouTube.png");
        CurrentMaster.Title.HelpTopicName = "dialogs_youtube";
        CurrentMaster.Body.Attributes.Add("onbeforeunload", "$j('.YouTubePreviewBox').remove();");

        string[,] action = new string[1,12];
        action[0, 0] = "hyperlink";
        action[0, 1] = GetString("dialogs.youtube.goto");
        action[0, 3] = "http://www.youtube.com";
        action[0, 5] = GetImageUrl("CMSModules/CMS_Content/Dialogs/opennew.png");
        action[0, 11] = "_blank";
        CurrentMaster.HeaderActions.Actions = action;
    }
}