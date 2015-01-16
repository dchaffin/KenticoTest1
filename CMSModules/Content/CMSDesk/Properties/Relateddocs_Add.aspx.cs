using System;
using System.Web;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.UIControls;

public partial class CMSModules_Content_CMSDesk_Properties_Relateddocs_Add : CMSPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.RelatedDocs"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Properties.RelatedDocs");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initializes page breadcrumbs
        string[,] pageTitleTabs = new string[2,3];
        pageTitleTabs[0, 0] = GetString("Relationship.RelatedDocs");
        pageTitleTabs[0, 1] = "~/CMSModules/Content/CMSDesk/Properties/Relateddocs_List.aspx?nodeid=" + NodeID;
        pageTitleTabs[0, 2] = "propedit";
        pageTitleTabs[1, 0] = GetString("Relationship.AddRelatedDocs");
        pageTitleTabs[1, 1] = string.Empty;
        pageTitleTabs[1, 2] = string.Empty;

        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;

        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "related_docs_edit";

        // Set edited document
        addRelatedDocument.TreeNode = Node;
    }

    #endregion
}