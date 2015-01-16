using System;
using System.Web.UI;

using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSFormControls_Selectors_InsertYouTubeVideo_Content : CMSModalPage
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

        HeaderAction action = new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.Hyperlink,
            Text = GetString("dialogs.youtube.goto"),
            Target = "_blank",
            ImageUrl = GetImageUrl("CMSModules/CMS_Content/Dialogs/opennew.png"),
            RedirectUrl = "http://www.youtube.com"
        };

        CurrentMaster.HeaderActions.AddAction(action);
    }
}