using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.UIControls;

public partial class CMSModules_MyDesk_WaitingForApproval_WaitingForApproval : CMSDocumentsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.MyDesk", new string[] { "MyDeskDocuments", "Pending", "WaitingDocs" }, CMSContext.CurrentSiteName))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.MyDesk", "MyDeskDocuments;Pending;WaitingDocs");
        }

        ucWaitingForApproval.SiteName = CMSContext.CurrentSiteName;
    }
}