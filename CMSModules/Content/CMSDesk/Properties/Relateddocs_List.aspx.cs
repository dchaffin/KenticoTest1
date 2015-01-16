using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.SettingsProvider;

public partial class CMSModules_Content_CMSDesk_Properties_Relateddocs_List : CMSPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.RelatedDocs"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Properties.RelatedDocs");
        }

        // Initialize node
        relatedDocuments.TreeNode = Node;

        CurrentMaster.PanelContent.CssClass = "";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        UIContext.PropertyTab = PropertyTabEnum.RelatedDocs;

        // Check if any relationship exists
        DataSet dsRel = RelationshipNameInfoProvider.GetRelationshipNames("RelationshipNameID", "RelationshipAllowedObjects LIKE '%" + CMSObjectHelper.GROUP_DOCUMENTS + "%' AND RelationshipNameID IN (SELECT RelationshipNameID FROM CMS_RelationshipNameSite WHERE SiteID = " + CMSContext.CurrentSiteID + ")", null, 1);
        if (DataHelper.DataSourceIsEmpty(dsRel))
        {
            pnlNewItem.Visible = false;
            relatedDocuments.Visible = false;
            ShowInformation(ResHelper.GetString("relationship.norelationship"));
        }
        else
        {
            if (Node != null)
            {
                // Check modify permissions
                if (!CheckDocumentPermissions(Node, PermissionsEnum.Modify))
                {
                    DisableForm();
                }

                // Initialize controls
                lnkNewRelationship.NavigateUrl = "~/CMSModules/Content/CMSDesk/Properties/Relateddocs_Add.aspx?nodeid=" + NodeID;
                imgNewRelationship.ImageUrl = GetImageUrl("CMSModules/CMS_Content/Properties/addrelationship.png");
                imgNewRelationship.DisabledImageUrl = GetImageUrl("CMSModules/CMS_Content/Properties/addrelationshipdisabled.png");
            }
        }

        pnlContent.Enabled = !DocumentManager.ProcessingAction;
    }


    private void DisableForm()
    {
        relatedDocuments.Enabled = false;
        lnkNewRelationship.Enabled = false;
        imgNewRelationship.Enabled = false;
    }

    #endregion
}