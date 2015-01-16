using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;

[RegisterTitle("content.ui.propertiescategories")]
public partial class CMSModules_Content_CMSDesk_Properties_Categories : CMSPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.Categories"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Properties.Categories");
        }

        // Enable split mode
        EnableSplitMode = true;

        UIContext.PropertyTab = PropertyTabEnum.Categories;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check modify permissions
            if (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                // Disable selector
                categoriesElem.Enabled = false;
            }

            // Display all global categories in administration UI
            categoriesElem.UserID = CMSContext.CurrentUser.UserID;
            categoriesElem.DocumentID = Node.DocumentID;
        }

        // UI settings
        categoriesElem.DisplaySavedMessage = false;
        categoriesElem.OnAfterSave += categoriesElem_OnAfterSave;
        categoriesElem.UniSelector.OnSelectionChanged += categoriesElem_OnSelectionChanged;

        // Register the scripts
        ScriptHelper.RegisterProgress(Page);

        pnlContent.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display 'The changes were saved' message
        if (QueryHelper.GetBoolean("saved", false))
        {
            ShowChangesSaved();
        }
    }

    #endregion


    #region "Handlers"

    private void categoriesElem_OnAfterSave()
    {
        if (DocumentManager.AllowSave)
        {
            // Log the synchronization
            DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, DocumentManager.Tree);
        }

        ShowChangesSaved();
    }


    private void categoriesElem_OnSelectionChanged(object sender, EventArgs e)
    {
        if (DocumentManager.AllowSave)
        {
            categoriesElem.Save();
        }
    }

    #endregion
}