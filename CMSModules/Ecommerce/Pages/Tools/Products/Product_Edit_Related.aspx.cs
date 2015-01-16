using System;
using System.Data;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

[Title("Objects/CMS_RelationshipName/object.png", "com.ui.productsrelated", "cms_ecommerce_products_related")]
[Security(Resource = "CMS.Ecommerce", UIElements = "Products.RelatedProducts")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Related : CMSProductsAdvancedPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Rememeber selected tab
        UIContext.ProductTab = ProductTabEnum.RelatedProducts;

        // Get default relationship name from settings
        string defaultRelName = ECommerceSettings.RelatedProductsRelationshipName(CMSContext.CurrentSiteName);

        // Check if relationship exists
        bool anyRelationshipsFound = true;
        RelationshipNameInfo defaultRelNameInfo = RelationshipNameInfoProvider.GetRelationshipNameInfo(defaultRelName);
        if (defaultRelNameInfo != null)
        {
            relatedDocuments.RelationshipName = defaultRelName;
        }
        else
        {
            // Check if any relationship exists
            DataSet dsRel = RelationshipNameInfoProvider.GetRelationshipNames("RelationshipNameID", "RelationshipAllowedObjects LIKE '%" + CMSObjectHelper.GROUP_DOCUMENTS + "%' AND RelationshipNameID IN (SELECT RelationshipNameID FROM CMS_RelationshipNameSite WHERE SiteID = " + CMSContext.CurrentSiteID + ")", null, 1);
            if (DataHelper.DataSourceIsEmpty(dsRel))
            {
                relatedDocuments.Visible = false;
                ShowInformation(GetString("relationship.norelationship"));

                anyRelationshipsFound = false;
            }
        }

        if (anyRelationshipsFound && (Node != null))
        {
            // Check read permissions
            if (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
            {
                RedirectToAccessDenied(string.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), Node.NodeAliasPath));
            }
            // Check modify permissions
            else if (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                pnlDocInfo.Label.Text = string.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), Node.DocumentName);
                relatedDocuments.Enabled = false;
                CurrentMaster.HeaderActions.Enabled = false;
            }
        }

        // Set tree node
        relatedDocuments.TreeNode = Node;

        // Set starting path
        if (!string.IsNullOrEmpty(ProductsStartingPath))
        {
            relatedDocuments.StartingPath = ProductsStartingPath;
        }

        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("Relationship.AddRelatedDocument"),
            ImageUrl = GetImageUrl(string.Format("CMSModules/CMS_Content/Properties/{0}.png", CurrentMaster.HeaderActions.Enabled ? "addrelationship" : "addrelationshipdisabled")),
            OnClientClick = relatedDocuments.GetAddRelatedDocumentScript()
        });
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ScriptHelper.RegisterDialogScript(Page);
    }
}
