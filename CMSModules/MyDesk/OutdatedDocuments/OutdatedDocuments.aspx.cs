using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.UIControls;

public partial class CMSModules_MyDesk_OutdatedDocuments_OutdatedDocuments : CMSDocumentsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if ((CMSContext.CurrentUser == null) || (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.MyDesk", "OutdatedDocs")))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.MyDesk", "OutdatedDocs");
        }

        // Setup page title text and image
        CurrentMaster.Title.TitleText = GetString("MyDesk.OutdatedDocumentsTitle");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_MyDesk/OutdatedDocuments/module.png");

        CurrentMaster.Title.HelpTopicName = "outdated_documents";
        CurrentMaster.Title.HelpName = "helpTopic";

        ucOutdatedDocuments.SiteName = CMSContext.CurrentSiteName;
    }
}