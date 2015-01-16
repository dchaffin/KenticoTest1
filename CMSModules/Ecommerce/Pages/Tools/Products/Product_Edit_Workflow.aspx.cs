using System;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.WorkflowEngine;

[Title("Objects/CMS_Workflow/object.png", "com.ui.productsworkflow", "cms_ecommerce_products_workflow")]
[Security(Resource = "CMS.Ecommerce", UIElements = "Products.Workflow")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Workflow : CMSProductsAdvancedPage
{
    #region "Variables"

    private WorkflowInfo workflow = null;

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Disable confirm changes checking
        DocumentManager.RegisterSaveChangesScript = false;

        // Init node
        workflowElem.Node = Node;

        workflow = DocumentManager.Workflow;
        if (workflow != null)
        {
            menuElem.OnClientStepChanged = ClientScript.GetPostBackEventReference(pnlUp, null);

            // Backward compatibility - Display Archive button for all steps
            menuElem.ForceArchive = workflow.IsBasic;
        }

        // Enable split mode
        EnableSplitMode = true;

        // Mark selected tab
        UIContext.ProductTab = ProductTabEnum.Workflow;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        pnlContainer.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (workflow != null)
        {
            // Backward compatibility
            if (workflow.WorkflowAutoPublishChanges)
            {
                string message = DocumentManager.GetDocumentInfo(true);
                if (!string.IsNullOrEmpty(message))
                {
                    message += "<br />";
                }
                message += GetString("WorfklowProperties.AutoPublishChanges");
                DocumentManager.DocumentInfo = message;
            }
        }

        // Register the scripts
        if (!DocumentManager.RefreshActionContent)
        {
            ScriptHelper.RegisterProgress(Page);
        }
    }

    #endregion
}
