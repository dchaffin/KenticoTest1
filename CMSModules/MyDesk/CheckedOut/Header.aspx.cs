using CMS.UIControls;
using CMS.SettingsProvider;

[CheckLicence(FeatureEnum.WorkflowVersioning)]
[Security(Resource = "CMS.MyDesk", UIElements = "CheckedOutDocs")]
[Tabs("CMS.MyDesk", "CheckedOutDocs", "content")]
public partial class CMSModules_MyDesk_CheckedOut_Header : CMSMyDeskPage
{
    protected override void OnLoad(System.EventArgs e)
    {
        base.OnLoad(e);

        var title = GetString("MyDesk.CheckedOutTitle");
        Title = title;
        CurrentMaster.Title.TitleText = title;
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_MyDesk/CheckedOut/module.png");

        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "checked_out_by_me";
    }
}
