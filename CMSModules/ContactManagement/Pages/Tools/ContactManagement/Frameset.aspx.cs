using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;

public partial class CMSModules_ContactManagement_Pages_Tools_ContactManagement_Frameset : CMSContactManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        frm.FrameHeight = TabsFrameHeight;
    }
}