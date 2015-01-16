using System;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Layout_Frameset : CMSModalPage
{
    protected int headHeight = 0;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        var isDialog = QueryHelper.GetBoolean("editonlycode", false);

        if (isDialog)
        {
            headHeight = TabsFrameHeight;
        }
        else
        {
            CheckAccessToSiteManager();
            headHeight = TabsBreadFrameHeight;
        }
    }
}
