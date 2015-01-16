using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

[Title("CMSModules/CMS_Content/Menu/Listing.png", "Content.ListingTitle", "list_tab")]
public partial class CMSModules_Content_CMSDesk_View_Listing : CMSContentPage
{
    #region "Variables"

    protected string editToolTip = null;
    protected string deleteToolTip = null;
    protected bool isAdvancedMode = false;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Disable permissions check
        CheckDocPermissions = false;

        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        // Do not check changes
        DocumentManager.RegisterSaveChangesScript = false;

        base.OnInit(e);

        docList.Grid.GridName = "Listing.xml";

        // Ensure dialog parameters
        string dialogParameter = String.Empty;
        if (RequiresDialog)
        {
            dialogParameter = "&dialog=1";
            docList.Grid.GridName = "ListingDialog.xml";
        }

        docList.NodeID = NodeID;
        docList.SelectItemJSFunction = "SelectItem";

        docList.DeleteReturnUrl = "~/CMSModules/Content/CMSDesk/Delete.aspx?multiple=true" + dialogParameter;
        docList.PublishReturnUrl = "~/CMSModules/Content/CMSDesk/PublishArchive.aspx?multiple=true" + dialogParameter;
        docList.ArchiveReturnUrl = "~/CMSModules/Content/CMSDesk/PublishArchive.aspx?multiple=true" + dialogParameter;
        docList.TranslateReturnUrl = "~/CMSModules/Translations/Pages/TranslateDocuments.aspx" + dialogParameter.Replace("&", "?");

        docList.RequiresDialog = RequiresDialog;
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentUserInfo currentUserInfo = CMSContext.CurrentUser;

        string[,] actions = new string[1, 6];

        if (!RequiresDialog)
        {
            ScriptHelper.RegisterScriptFile(this, @"~/CMSModules/Content/CMSDesk/View/Listing.js");
        }

        if (docList.NodeID > 0)
        {
            TreeNode node = docList.Node;
            // Set edited document
            EditedDocument = node;

            if (node != null)
            {
                if (currentUserInfo.IsAuthorizedPerDocument(node, NodePermissionsEnum.ExploreTree) != AuthorizationResultEnum.Allowed)
                {
                    RedirectToCMSDeskAccessDenied("CMS.Content", "exploretree");
                }

                if (RequiresDialog)
                {
                    ScriptHelper.RegisterScriptFile(this, @"~/CMSModules/Content/CMSDesk/View/ListingDialog.js");

                    // Set JavaScript for new button
                    actions[0, 3] = "javascript:AddItem(" + node.NodeID + ");";
                    actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
                    actions[0, 1] = GetString("content.newtitle");
                    actions[0, 5] = GetImageUrl("CMSModules/CMS_Content/Listing/16/New.png");

                    CurrentMaster.Title.HelpTopicName = "ose_documents";

                    // Ensure breadcrumbs
                    EnsureBreadcrumbs(node);
                }
                else
                {
                    // Setup the link to the parent document
                    if ((node.NodeClassName.ToLowerCSafe() != "cms.root") && (currentUserInfo.UserStartingAliasPath.ToLowerCSafe() != node.NodeAliasPath.ToLowerCSafe()))
                    {
                        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
                        actions[0, 1] = GetString("Listing.ParentDirectory");
                        actions[0, 5] = GetImageUrl("CMSModules/CMS_Content/Listing/parent.png");
                        actions[0, 3] = "javascript:SelectItem(" + node.NodeParentID + ");";
                    }
                    else
                    {
                        CurrentMaster.HeaderActions.Visible = false;
                        CurrentMaster.PanelBody.FindControl("pnlActions").Visible = false;
                    }
                }

                // Define target window for modal dialogs (used for mobile Android browser which cannot open more than one modal dialog window at a time).
                // If set: The target window will be used for opening the new dialog in the following way: targetWindow.location.href = '...new dialog url...';
                // If not set: New modal dialog window will be opened 
                string actionTargetWindow = "var targetWindow = " + (CMSContext.CurrentDevice.IsMobile ? "this" : "null");
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "listingScript", actionTargetWindow, true);
            }
        }

        CurrentMaster.HeaderActions.Actions = actions;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Ensures dialog breadcrumbs
    /// </summary>
    /// <param name="node">Current node</param>
    private void EnsureBreadcrumbs(TreeNode node)
    {
        if (node != null)
        {
            // Loop thru all levels and generate breadcrumbs
            string[,] breadcrumbs = new String[node.NodeLevel + 1, 4];

            int parentNodeId = 0;

            for (int i = node.NodeLevel; i >= 0; i--)
            {
                if (node == null)
                {
                    // Document is not translated in the current culture -> get parent node from the default culture
                    TreeProvider treeProvider = new TreeProvider();
                    TreeNode parentNode = treeProvider.SelectSingleNode(parentNodeId, CultureCode, true);
                    node = parentNode;
                }

                if (node != null)
                {
                    breadcrumbs[i, 0] = DataHelper.GetNotEmpty(node.NodeName, CurrentSiteName);
                    breadcrumbs[i, 1] = "#";
                    breadcrumbs[i, 3] = "SelectItem(" + node.NodeID + "); return false;";

                    parentNodeId = node.NodeParentID;
                    node = node.Parent;
                }
            }

            // Set breadcrumb
            CurrentMaster.Title.Breadcrumbs = breadcrumbs;

            // Add additional css class for correct design
            CurrentMaster.PanelHeader.CssClass += " SimpleHeader";
        }
    }

    #endregion
}
