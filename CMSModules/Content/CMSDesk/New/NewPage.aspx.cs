using System;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.PortalControls;

public partial class CMSModules_Content_CMSDesk_New_NewPage : CMSContentPage
{
    #region "Variables"

    private string mode = null;

    private string pageClassName = DataClassInfoProvider.DefaultPageClassName;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        mode = QueryHelper.GetString("mode", string.Empty);
        if (mode.ToLowerCSafe() == "productssection")
        {
            string url = ResolveUrl("~/CMSModules/Content/CMSDesk/Edit/Edit.aspx" + URLHelper.Url.Query);
            URLHelper.Redirect(url);
        }

        // Do not use content class in dialog mode
        UseDialogContentClass = false;

        DocumentManager.OnValidateData += DocumentManager_OnValidateData;
        DocumentManager.OnSaveData += DocumentManager_OnSaveData;

        DocumentManager.Mode = FormModeEnum.Insert;
        DocumentManager.ParentNodeID = QueryHelper.GetInteger("parentnodeid", 0);
        DocumentManager.NewNodeCultureCode = QueryHelper.GetString("parentculture", null);
        DocumentManager.NewNodeClassID = QueryHelper.GetInteger("classId", 0);

        // Set up the document conversion
        int convertDocumentId = QueryHelper.GetInteger("convertdocumentid", 0);
        if (convertDocumentId > 0)
        {
            DocumentManager.SourceDocumentID = convertDocumentId;
            DocumentManager.Mode = FormModeEnum.Convert;
        }

        // Load proper class name
        DataClassInfo dci = DocumentManager.NewNodeClass;
        if (dci != null)
        {
            pageClassName = dci.ClassName;
        }

        // Check if user is allowed to create page under this node
        if (!CMSContext.CurrentUser.IsAuthorizedToCreateNewDocument(DocumentManager.ParentNodeID, pageClassName))
        {
            RedirectToAccessDenied(GetString("cmsdesk.notauthorizedtocreatedocument"));
        }

        // Check license limitations
        if (!LicenseHelper.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Documents, VersionActionEnum.Insert))
        {
            RedirectToAccessDenied(String.Format(GetString("cmsdesk.documentslicenselimits"), string.Empty));
        }

        // Set dialog title
        if (RequiresDialog)
        {
            SetTitle("CMSModules/CMS_Content/Menu/New.png", GetString("Content.NewTitle"), null, null);
        }
    }


    protected override void OnInit(EventArgs e)
    {
        // Load the root category of the selector
        DataClassInfo dci = DocumentManager.NewNodeClass;
        if (dci != null)
        {
            selTemplate.RootCategory = dci.ClassPageTemplateCategoryID;
            if (!RequestHelper.IsPostBack())
            {
                selTemplate.SetDefaultTemplate(dci.ClassDefaultPageTemplateID);
            }

            if (dci.ClassName.EqualsCSafe("cms.wireframe", true) && PortalHelper.IsWireframingEnabled(CMSContext.CurrentSiteName))
            {
                selTemplate.IsWireframe = true;
            }
        }

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        menuElem.OnGetClientValidationScript += menuElem_OnGetClientValidationScript;

        // Register script files
        ScriptHelper.RegisterProgress(this);
        ScriptHelper.RegisterScriptFile(this, "cmsedit.js");

        txtPageName.MaxLength = TreePathUtils.GetMaxNameLength(DocumentManager.NewNodeClassName);

        if (DocumentManager.ParentNode != null)
        {
            selTemplate.DocumentID = DocumentManager.ParentNode.DocumentID;
            selTemplate.ParentNodeID = DocumentManager.ParentNodeID;
        }

        // Hide error label
        lblError.Style.Add("display", "none");

        // Set default focus on page name field
        if (!RequestHelper.IsPostBack())
        {
            if (DocumentManager.Mode == FormModeEnum.Convert)
            {
                txtPageName.Text = DocumentManager.Node.DocumentName;
            }
            txtPageName.Focus();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // For blank page and page template selector - focus page name text box to proper ctrl+c function.
        if ((selTemplate.TemplateSelectionState == 1) || (selTemplate.TemplateSelectionState == 3))
        {
            txtPageName.Focus();
        }
    }

    #endregion


    #region "Other events"

    protected void menuElem_OnGetClientValidationScript(object sender, CMS.Controls.EditMenuEventArgs e)
    {
        switch (e.ActionName)
        {
            case ComponentEvents.SAVE:
                string jsValidation = "function ValidateNewPage(){" +
                   " var value = document.getElementById('" + txtPageName.ClientID + "').value;" +
                   " value = value.replace(/^\\s+|\\s+$/g, '');" +
                   " var errorLabel = document.getElementById('" + lblError.ClientID + "'); " +
                   " if (value == '') {" +
                   " errorLabel.style.display = ''; errorLabel.innerHTML  = " + ScriptHelper.GetString(GetString("newpage.nameempty")) + "; resizearea(); return false;}";

                jsValidation += selTemplate.GetValidationScript();

                jsValidation += " return true;}";

                // Register validate script
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ValidateNewPage", ScriptHelper.GetScript(jsValidation));

                e.ValidationScript = "ValidateNewPage()";
                break;
        }
    }


    protected void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        e.UpdateDocument = false;

        string errorMessage = null;
        string newPageName = txtPageName.Text.Trim();

        if (!String.IsNullOrEmpty(newPageName))
        {
            // Limit length
            newPageName = TreePathUtils.EnsureMaxNodeNameLength(newPageName, pageClassName);
        }

        TreeNode node = e.Node;
        node.DocumentName = newPageName;

        bool wireframe = node.IsWireframe() && PortalHelper.IsWireframingEnabled(CMSContext.CurrentSiteName);
        if (wireframe)
        {
            // Ensure the node GUID
            node.EnsureGUID();
            node.DocumentSearchExcluded = true;
            
            // Wireframe
            PageTemplateInfo pti = selTemplate.EnsureTemplate(node.DocumentName, node.NodeGUID, ref errorMessage);
            if (pti != null)
            {
                node.NodeWireframeTemplateID = pti.PageTemplateId;
            }
        }
        else
        {
            // Same template for all language versions by default
            PageTemplateInfo pti = selTemplate.EnsureTemplate(node.DocumentName, node.NodeGUID, ref errorMessage);
            if (pti != null)
            {
                node.SetDefaultPageTemplateID(pti.PageTemplateId);
            }
        }

        // Insert node if no error
        if (String.IsNullOrEmpty(errorMessage))
        {
            // Switch to design mode (the template is empty at this moment)
            if (!DocumentManager.CreateAnother && (CMSContext.ViewMode != ViewModeEnum.EditLive))
            {
                if (wireframe)
                {
                    // Wireframe mode for wireframes
                    CMSContext.ViewMode = ViewModeEnum.Wireframe;
                }
                else if (selTemplate.NewTemplateIsEmpty)
                {
                    // Design mode for empty template
                    CMSContext.ViewMode = ViewModeEnum.Design;
                }
            }

            // Insert the document
            if (DocumentManager.Mode == FormModeEnum.Convert)
            {
                DocumentManager.UpdateDocument(true);
            }
            else
            {
                // Ensures documents consistency (blog post hierarchy etc.)
                DocumentManager.EnsureDocumentsConsistency();
                DocumentHelper.InsertDocument(node, DocumentManager.ParentNodeID, DocumentManager.Tree);
            }

            // Create default SKU if configured
            if (ModuleEntry.CheckModuleLicense(ModuleEntry.ECOMMERCE, URLHelper.GetCurrentDomain(), FeatureEnum.Ecommerce, VersionActionEnum.Insert))
            {
                bool? skuCreated = node.CreateDefaultSKU();
                if (skuCreated.HasValue && !skuCreated.Value)
                {
                    AddError(GetString("com.CreateDefaultSKU.Error"), null);
                }
            }

            if (node.NodeSKUID > 0)
            {
                DocumentManager.UpdateDocument(true);
            }
        }
        else
        {
            e.IsValid = false;
            e.ErrorMessage = errorMessage;
        }
    }


    protected void DocumentManager_OnValidateData(object sender, DocumentManagerEventArgs e)
    {
        string newPageName = txtPageName.Text.Trim();
        if (String.IsNullOrEmpty(newPageName))
        {
            e.ErrorMessage = GetString("newpage.nameempty");
            e.IsValid = false;
        }
    }

    #endregion
}
