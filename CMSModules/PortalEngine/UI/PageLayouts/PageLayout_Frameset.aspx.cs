using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_PortalEngine_UI_PageLayouts_PageLayout_Frameset : CMSModalPage
{
    protected int headerHeight = 0;


    protected override void OnPreInit(System.EventArgs e)
    {
        base.OnPreInit(e);

        var isDialog = QueryHelper.GetBoolean("editonlycode", false);

        if (isDialog)
        {
            headerHeight = TabsFrameHeight;
        }
        else
        {
            CheckAccessToSiteManager();
            headerHeight = TabsBreadHeadFrameHeight;
        }
    }
}
