using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;

public partial class CMSModules_ContactManagement_Pages_Tools_ContactGroup_Frameset : CMSContactManagementContactGroupsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        frm.FrameHeight = TabsBreadHeadFrameHeight;
        frm.HeaderUrl = URLHelper.AddParameterToUrl(frm.HeaderUrl, "siteid", SiteID.ToString());
        if (IsSiteManager)
        {
            frm.FrameHeight = TabsBreadFrameHeight;
            frm.HeaderUrl = URLHelper.AddParameterToUrl(frm.HeaderUrl, "issitemanager", "1");
        }
    }
}