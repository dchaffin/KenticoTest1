using CMS.SettingsProvider;
using CMS.UIControls;

[CheckLicence(FeatureEnum.WorkflowVersioning)]
[Security(Resource = "CMS.MyDesk", UIElements = "CheckedOutDocs;CheckedOutObjects")]
public partial class CMSModules_MyDesk_CheckedOut_Objects : CMSMyDeskPage
{
    protected override void OnLoad(System.EventArgs e)
    {
        base.OnLoad(e);

        ucDisabledModuleInfo.InfoText = GetString("teamdevelopment.checkoutnotenabled");
        ucDisabledModuleInfo.ParentPanel = pnlDisabled;
    }
}
