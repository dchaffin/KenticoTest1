using System;
using System.Data;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;

[RegisterTitle("content.ui.propertiestemplate")]

[Security("CMS.Content", null, "Properties.Template")]
[Security("CMS.Design", "Design", null)]
public partial class CMSModules_Content_CMSDesk_Properties_Template : CMSPropertiesPage
{
    #region "Variables & constants"

    TreeNode node = null;

    protected string mSave = null;
    protected string mClone = null;
    protected string mEditTemplateProperties = null;

    protected CurrentUserInfo currentUser = null;

    protected bool hasModifyPermission = false;
    protected bool selectorEnabled = true;

    protected PageTemplateInfo pti = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Selected template ID
    /// </summary>
    public int SelectedTemplateID
    {
        get
        {
            return ValidationHelper.GetInteger(hdnSelected.Value, 0);
        }
        set
        {
            hdnSelected.Value = value.ToString();
        }
    }

    #endregion


    #region "Page methods"

    protected override void OnInit(EventArgs e)
    {
        // Culture independent data
        SplitModeAllwaysRefresh = true;

        // Non-version data is modified
        DocumentManager.UseDocumentHelper = false;

        base.OnInit(e);

        // Keep current user info
        currentUser = CMSContext.CurrentUser;
        
        // Keep node instance
        node = Node;
        
        if (node.IsRoot())
        {
            plcUILevels.Visible = false;
        }

        // Init document manager events
        DocumentManager.OnValidateData += DocumentManager_OnValidateData;
        DocumentManager.OnSaveData += DocumentManager_OnSaveData;
        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;

        EnableSplitMode = true;

        // Register the scripts
        ScriptHelper.RegisterProgress(this);
        ScriptHelper.RegisterDialogScript(this);
    }


    protected void Page_Load(object sender, EventArgs e)
    {

        UIContext.PropertyTab = PropertyTabEnum.Template;

        inheritElem.DocumentSettings = true;

        // Init the control properties
        pnlInherits.GroupingText = GetString("PageProperties.InheritLevels");
        pnlActions.GroupingText = GetString("PageProperties.Template");

        imgClone.ImageUrl = GetImageUrl("CMSModules/CMS_Content/Template/clone.png");
        imgSave.ImageUrl = GetImageUrl("CMSModules/CMS_Content/Template/save.png");
        imgEditTemplateProperties.ImageUrl = GetImageUrl("CMSModules/CMS_Content/Template/edit.png");
        imgClone.DisabledImageUrl = GetImageUrl("CMSModules/CMS_Content/Template/clonedisabled.png");
        imgSave.DisabledImageUrl = GetImageUrl("CMSModules/CMS_Content/Template/savedisabled.png");
        imgEditTemplateProperties.DisabledImageUrl = GetImageUrl("CMSModules/CMS_Content/Template/editdisabled.png");

        // Keep information whether current user has modify permission
        if (node != null)
        {
            hasModifyPermission = CheckDocumentPermissions(node, PermissionsEnum.Modify);
            btnSelect.OnClientClick = "modalDialog('" + ResolveUrl("~/CMSModules/PortalEngine/UI/Layout/PageTemplateSelector.aspx") + "?documentid=" + node.DocumentID + "&nodeguid=" + node.NodeGUID + "', 'PageTemplateSelection', '90%', '85%'); return false;";
        }

        btnSelect.Text = GetString("PageProperties.Select");
        btnClone.OnClientClick = "return confirm('" + GetString("Template.ConfirmClone") + "');";


        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SelectTemplate", ScriptHelper.GetScript(
@"
function OnSelectPageTemplate(templateId) {
    document.getElementById('" + hdnSelected.ClientID + @"').value = templateId;
    " + ClientScript.GetPostBackEventReference(btnSelect, null) + @"
}
"
));

        // Reflect processing action
        pnlInherits.Enabled = DocumentManager.AllowSave;

        if (!RequestHelper.IsPostBack())
        {
            LoadData();
        }

        ReloadControls();

        // Ensure visibility
        EnsureEditingForm(hasModifyPermission && !DocumentManager.ProcessingAction);
    }

    #endregion


    #region "Methods"

    protected void RadChanged(object sender, EventArgs e)
    {
        ReloadControls();
    }


    /// <summary>
    /// Gets the inherited page template from the parent node
    /// </summary>
    /// <param name="node">Document node</param>
    protected int GetInheritedPageTemplateId(TreeNode node)
    {
        string aliasPath = node.NodeAliasPath;

        // For root, there is no inheritance possible
        if (String.IsNullOrEmpty(aliasPath) || (aliasPath == "/"))
        {
            return 0;
        }

        aliasPath = TreePathUtils.GetParentPath(aliasPath);

        // Get the page info
        PageInfo pi = PageInfoProvider.GetPageInfo(node.NodeSiteName, aliasPath, node.DocumentCulture, node.DocumentUrlPath, node.NodeParentID, true);
        if (pi != null)
        {
            // Get template used by the page info
            pti = pi.UsedPageTemplateInfo;
            if (pti != null)
            {
                return pti.PageTemplateId;
            }
        }

        return 0;
    }


    /// <summary>
    /// Reloads the controls on the page to the appropriate state
    /// </summary>
    protected void ReloadControls()
    {
        TreeNode node = DocumentManager.Node;

        if (node.NodeAliasPath == "/")
        {
            // For root, inherit option means no page template
            radInherit.ResourceString = "Template.NoTemplate";
        }

        // Get the template ID
        int templateId = SelectedTemplateID;
        string suffix = null;
        bool inherit = radInherit.Checked;

        plcUIClone.Visible = false;
        plcUIEdit.Visible = false;
        plcUISave.Visible = false;

        if (inherit)
        {
            // Inherited
            selectorEnabled = false;

            // Inherit
            templateId = GetInheritedPageTemplateId(node);

            if (templateId > 0)
            {
                suffix = " " + GetString("Template.Inherited");
            }
        }
        else
        {
            // Standard selection
            selectorEnabled = true;
        }

        // Set modal dialogs
        btnSave.OnClientClick = "modalDialog('" + ResolveUrl("~/CMSModules/PortalEngine/UI/Layout/SaveNewPageTemplate.aspx") + "?templateId=" + templateId + "&siteid=" + CMSContext.CurrentSiteID + "', 'SaveNewTemplate', 600, 400); return false;";
        btnEditTemplateProperties.OnClientClick = "modalDialog('" + ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Edit.aspx") + "?templateid=" + templateId + "&aliaspath=" + node.NodeAliasPath + "&dialog=1', 'TemplateSelection', 850, 680); return false;";

        // Load the page template name
        pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);
        if (pti != null)
        {
            txtTemplate.Text = ResHelper.LocalizeString(pti.DisplayName);

            plcUISave.Visible = true;
            plcUIEdit.Visible = (!pti.IsReusable || currentUser.IsAuthorizedPerUIElement("CMS.Content", "Template.ModifySharedTemplates"));
            plcUIClone.Visible = pti.IsReusable || inherit;
        }
        else
        {
            txtTemplate.Text = GetString("Template.SelectorNoTemplate");
        }

        txtTemplate.Text += suffix;
    }


    /// <summary>
    /// Ensures visibility of the displayed controls
    /// </summary>
    /// <param name="modify">Indicates whether current user has modify permission</param>
    private void EnsureEditingForm(bool modify)
    {
        // Template selector
        txtTemplate.Enabled = modify && selectorEnabled;
        btnSelect.Enabled = txtTemplate.Enabled;

        // Clone as ad-hoc
        btnClone.Enabled = modify;

        if (CMSObjectHelper.UseCheckinCheckout)
        {
            var pageTemplate = PageTemplateInfoProvider.GetPageTemplateInfo(Node.GetUsedPageTemplateId());
            if ((pageTemplate != null) && !pageTemplate.Generalized.IsCheckedOutByUser(CMSContext.CurrentUser))
            {
                btnClone.Enabled = false;

                var objectType = GetString(TypeHelper.GetObjectTypeResourceKey(pageTemplate.ObjectType));
                var objectName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(pageTemplate.Generalized.ObjectDisplayName));
                btnClone.ToolTip = HTMLHelper.StripTags(string.Format(GetString("ObjectEditMenu.CheckOutToEdit"), objectType, objectName));
            }
        }

        if (!btnClone.Enabled)
        {
            // Remove client click action
            btnClone.OnClientClick = "return false;";
        }

        lblClone.Enabled = btnClone.Enabled;

        // Edit menu
        menuElem.Enabled = modify;

        // Inherits
        pnlInherits.Visible = modify;

        // Radio buttons
        radInherit.Enabled = modify;
        if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", "Template.Inherit"))
        {
            radInherit.Attributes.Add("disabled", "disabled");
        }
        radAllCultures.Enabled = modify;
        radThisCulture.Enabled = modify;
    }


    /// <summary>
    /// Loads the initial data from the document
    /// </summary>
    private void LoadData()
    {
        TreeNode node = Node;
        if (node != null)
        {
            if (node.IsRoot())
            {
                // Hide inheritance options for root node
                pnlInherits.Visible = false;
            }
            else 
            {
                inheritElem.Value = Node.NodeInheritPageLevels;

                // Try get info whether exist linked document in path
                DataSet ds = DocumentManager.Tree.SelectNodes(CMSContext.CurrentSiteName, "/%", node.DocumentCulture, false, null, "NodeLinkedNodeID IS NOT NULL AND (N'" + SqlHelperClass.GetSafeQueryString(Node.NodeAliasPath) + "' LIKE NodeAliasPath + '%')", null, -1, false, 1, "Count(*) AS NumOfDocs");

                // If node is not link or none of parent documents is not linked document use document name path
                if (!node.IsLink && ValidationHelper.GetInteger(DataHelper.GetDataRowValue(ds.Tables[0].Rows[0], "NumOfDocs"), 0) == 0)
                {
                    inheritElem.TreePath = TreePathUtils.GetParentPath("/" + Node.DocumentNamePath);
                }
                else
                {
                    // Otherwise use alias path
                    inheritElem.TreePath = TreePathUtils.GetParentPath("/" + Node.NodeAliasPath);
                }
            }

            if (node.NodeInheritPageTemplate)
            {
                // Document inherits template
                radInherit.Checked = true;
            }
            else
            {
                // Document has its own template
                int templateId = node.GetUsedPageTemplateId();

                if (node.NodeTemplateForAllCultures)
                {
                    radAllCultures.Checked = true;
                }
                else
                {
                    radThisCulture.Checked = true;
                }

                // Set selected template ID
                SelectedTemplateID = templateId;
            }

            ReloadControls();
        }
    }

    #endregion


    #region "Button handling"

    /// <summary>
    /// Fires after the template selection
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        ReloadControls();
    }


    /// <summary>
    /// Clone button event handler
    /// </summary>
    protected void btnClone_Click(object sender, EventArgs e)
    {
        if ((pti != null) && hasModifyPermission)
        {
            int templateId = pti.PageTemplateId;

            TreeNode node = Node;

            // Clone the info
            string docName = node.GetDocumentName();
            string displayName = "Ad-hoc: " + docName;

            PageTemplateInfo newInfo = PageTemplateInfoProvider.CloneTemplateAsAdHoc(pti, displayName, CMSContext.CurrentSiteID, node.NodeGUID);

            newInfo.Description = String.Format(GetString("PageTemplate.AdHocDescription"), Node.DocumentNamePath);
            PageTemplateInfoProvider.SetPageTemplateInfo(newInfo);

            // Save the MVT/Content personalization variants of this page template
            if (LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.MVTesting))
            {
                ModuleCommands.OnlineMarketingCloneTemplateMVTVariants(templateId, newInfo.PageTemplateId);
            }
            if (LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.ContentPersonalization))
            {
                ModuleCommands.OnlineMarketingCloneTemplateContentPersonalizationVariants(templateId, newInfo.PageTemplateId);
            }

            // Assign the selected template for all cultures and save
            SelectedTemplateID = newInfo.PageTemplateId;

            if (radInherit.Checked)
            {
                radAllCultures.Checked = true;
                radInherit.Checked = false;
            }

            DocumentManager.SaveDocument();
        }
    }


    protected void DocumentManager_OnValidateData(object sender, DocumentManagerEventArgs e)
    {
        if (!radInherit.Checked)
        {
            // Set the selected template ID
            int templateId = SelectedTemplateID;
            if (templateId <= 0)
            {
                e.IsValid = false;
                e.ErrorMessage = GetString("newpage.templateerror");
            }
        }
    }


    private void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        node = Node;

        if (radInherit.Checked)
        {
            // Set 0 as inherited
            SetTemplateId(0);

            node.NodeInheritPageTemplate = true;
        }
        else
        {
            // Set the selected template ID
            int templateId = SelectedTemplateID;
            SetTemplateId(templateId);

            bool templateSelected = (templateId > 0);

            node.NodeInheritPageTemplate = !templateSelected;

            if (!templateSelected)
            {
                radInherit.Checked = true;
                radThisCulture.Checked = false;
                radAllCultures.Checked = false;

                txtTemplate.Enabled = false;
                btnSelect.Enabled = false;
            }
        }

        node.SetValue("NodeInheritPageLevels", inheritElem.Value);
    }


    private void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        // Ensure default combination if page template changed
        if (SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSMVTEnabled")
            && LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.MVTesting)
            && (ModuleCommands.OnlineMarketingContainsMVTest(Node.NodeAliasPath, Node.NodeSiteID, Node.DocumentCulture, false)))
        {
            int templateId = Node.GetUsedPageTemplateId();

            ModuleCommands.OnlineMarketingEnsureDefaultCombination(templateId);
        }

        ReloadControls();
    }


    /// <summary>
    /// Sets the template id to the given document
    /// </summary>
    /// <param name="node">Document node</param>
    private void SetTemplateId(int templateId)
    {
        if (radAllCultures.Checked)
        {
            Node.NodeTemplateID = templateId;
        }
        else
        {
            Node.NodeTemplateID = 0;
        }

        Node.DocumentPageTemplateID = templateId;
        Node.NodeTemplateForAllCultures = radAllCultures.Checked;
    }

    #endregion
}