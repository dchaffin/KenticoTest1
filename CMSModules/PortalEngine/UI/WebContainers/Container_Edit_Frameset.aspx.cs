using System;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebContainers_Container_Edit_Frameset : CMSModalDesignPage
{
    protected int mHeight = 0;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        RequireSite = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.GetBoolean("editonlycode", false))
        {
            mHeight = TabsFrameHeight;
        }
        else
        {
            mHeight = TabsBreadHeadFrameHeight;
            CheckAccessToSiteManager();
        }
    }
}