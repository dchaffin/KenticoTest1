using CMS.UIControls;
using CMS.SettingsProvider;

[CheckLicence(FeatureEnum.WorkflowVersioning)]
[Security(Resource = "CMS.MyDesk", UIElements = "CheckedOutDocs")]
public partial class CMSModules_MyDesk_CheckedOut_Frameset : CMSMyDeskPage
{
    protected override void OnLoad(System.EventArgs e)
    {
        base.OnLoad(e);

        frameset.FrameHeight = TabsFrameHeight;
    }
}
