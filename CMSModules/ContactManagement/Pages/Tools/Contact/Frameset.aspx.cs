using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;

public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Frameset : CMSContactManagementContactsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Modify high of the first row...
        if (QueryHelper.GetBoolean("dialogmode", false))
        {
            //  in modal dialog
            frm.FrameHeight = TabsFrameHeight;
        }
        else 
        {
            frm.FrameHeight = TabsBreadFrameHeight;
        }
        frm.HeaderUrl = URLHelper.AddParameterToUrl(frm.HeaderUrl, "siteid", SiteID.ToString());
        if (IsSiteManager)
        {
            frm.HeaderUrl = URLHelper.AddParameterToUrl(frm.HeaderUrl, "issitemanager", "1");
        }
    }
}