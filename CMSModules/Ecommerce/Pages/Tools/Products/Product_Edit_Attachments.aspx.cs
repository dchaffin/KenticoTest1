using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

[RegisterTitle("com.ui.productsattachments")]
[Security(Resource = "CMS.Ecommerce", UIElements = "Products.Attachments")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Attachments : CMSProductPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Enable split mode
        EnableSplitMode = true;

        // Ensure correct padding
        CurrentMaster.PanelContent.CssClass = "";

        // Mark selected tab
        UIContext.ProductTab = ProductTabEnum.Attachments;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check read permissions
            if (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
            {
                RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), menuElem.Node.NodeAliasPath));
            }

            // Register WebDAV script
            if (CMSContext.IsWebDAVEnabled(CMSContext.CurrentSiteName) && RequestHelper.IsWindowsAuthentication())
            {
                // Register scripts
                string script = "function RefreshForm(){" + Page.ClientScript.GetPostBackEventReference(btnRefresh, "") + " }";
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshForm", ScriptHelper.GetScript(script));

                ScriptHelper.RegisterWebDAV(Page);
            }
            
            ucAttachments.Container.CssClass = "PageContent";
            ucAttachments.DocumentID = Node.DocumentID;
            ucAttachments.NodeParentNodeID = Node.NodeParentID;
            ucAttachments.NodeClassName = Node.NodeClassName;

            // Resize attachment due to site settings
            string siteName = CMSContext.CurrentSiteName;
            ucAttachments.ResizeToHeight = ImageHelper.GetAutoResizeToHeight(siteName);
            ucAttachments.ResizeToWidth = ImageHelper.GetAutoResizeToWidth(siteName);
            ucAttachments.ResizeToMaxSideSize = ImageHelper.GetAutoResizeToMaxSideSize(siteName);
            ucAttachments.PageSize = "10,25,50,100,##ALL##";
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Node != null)
        {
            ucAttachments.Enabled = DocumentManager.AllowSave;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Refresh button click event handler.
    /// </summary>
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check permission to modify document
            if (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed)
            {
                // Ensure version for later detection whether node is published
                Node.VersionManager.EnsureVersion(Node, Node.IsPublished);

                // Tree refresh is needed only if node was archived or published
                WorkflowManager wm = WorkflowManager.GetInstance(Tree);
                WorkflowStepInfo currentStep = wm.GetStepInfo(Node);
                bool refreshTree = (currentStep != null) && (currentStep.StepIsArchived || currentStep.StepIsPublished);

                // Move to edit step
                wm.MoveToFirstStep(Node, null);

                // Refresh frames and tree
                string script = "if(window.FramesRefresh){FramesRefresh(" + refreshTree.ToString().ToLowerCSafe() + ", " + Node.NodeID + ");}";
                ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshAction", ScriptHelper.GetScript(script));
            }
        }
    }

    #endregion
}
