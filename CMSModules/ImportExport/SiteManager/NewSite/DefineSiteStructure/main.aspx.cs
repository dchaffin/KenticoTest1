using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.DocumentEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

public partial class CMSModules_ImportExport_SiteManager_NewSite_DefineSiteStructure_main : SiteManagerPage
{
    #region "Variables"

    private TreeProvider tree = null;
    private TreeNode node = null;

    private string action = string.Empty;
    private string siteName = string.Empty;
    private int nodeId = 0;

    private string pageScript = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        tree = new TreeProvider(CMSContext.CurrentUser);

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this);

        // Initialize resources
        btnInherit.Text = GetString("DefineSiteStructure.InheritFromParent");
        reqItemName.ErrorMessage = GetString("DefineSiteStructure.PageNameRequired");

        siteName = QueryHelper.GetString("sitename", string.Empty);
        nodeId = QueryHelper.GetInteger("nodeid", 0);

        // If action not specified, edit mode
        action = QueryHelper.GetString("action", "edit");

        if (nodeId > 0)
        {
        }
        else
        {
            TreeNode rootNode = tree.SelectSingleNode(siteName, "/", TreeProvider.ALL_CULTURES);
            if (rootNode != null)
            {
                nodeId = rootNode.NodeID;
                AddScript("SetSelectedNodeId(" + nodeId + ")");
                action = "edit";
            }
        }

        bool isNewPage = false;

        if (!RequestHelper.IsPostBack())
        {
            switch (action)
            {
                case "edit":
                    // Initialize PageTitle
                    SetTitle("CMSModules/CMS_ImportExport/PageProperties.png", GetString("DefineSiteStructure.PagePropertiesCaption"), null, null);

                    node = tree.SelectSingleNode(nodeId);
                    if (node != null)
                    {
                        txtPageName.Text = node.DocumentName;
                        hdnSelected.Value = node.GetUsedPageTemplateId().ToString();

                        // If root node is selected
                        if (node.NodeParentID == 0)
                        {
                            btnInherit.Text = GetString("DefineSiteStructure.RemoveTemplate");
                            txtPageTemplate.Text = GetString("Template.SelectorNoTemplate");

                            txtPageName.Enabled = false;
                            reqItemName.Enabled = false;
                        }
                        else
                        {
                            txtPageTemplate.Text = GetString("Template.Inherited");
                        }
                    }
                    break;

                default:
                    // Initialize PageTitle
                    SetTitle("CMSModules/CMS_ImportExport/PageProperties.png", GetString("DefineSiteStructure.NewPagePropertiesCaption"), null, null);

                    txtPageName.Text = "";
                    txtPageTemplate.Text = GetString("Template.Inherited");
                    isNewPage = true;
                    break;
            }
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SelectTemplate", ScriptHelper.GetScript(
@"
function OnSelectPageTemplate(templateId) {
    document.getElementById('" + hdnSelected.ClientID + @"').value = templateId;
    " + ClientScript.GetPostBackEventReference(btnSelect, null) + @"
}
"
));

        // Modal dialog
        btnSelect.Attributes.Add("onclick", "modalDialog('" + ResolveUrl("~/CMSModules/PortalEngine/UI/Layout/PageTemplateSelector.aspx?isnewpage=" + ValidationHelper.GetString(isNewPage, "") + "&nodeid=" + ValidationHelper.GetString(nodeId, "0")) + "', 'PageTemplateSelection', '90%', '85%');");
    }

    #endregion


    #region "Button handling"

    /// <summary>
    /// Fires after the template selection
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
    }


    /// <summary>
    /// Fires after the template selection
    /// </summary>
    protected void btnInherit_Click(object sender, EventArgs e)
    {
        hdnSelected.Value = "";

        node = tree.SelectSingleNode(nodeId);
        if (node != null)
        {
            // If root node is selected
            if (node.NodeParentID == 0)
            {
                txtPageTemplate.Text = GetString("Template.SelectorNoTemplate");
            }
            else
            {
                txtPageTemplate.Text = GetString("Template.Inherited");
            }
        }
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        int templateId = ValidationHelper.GetInteger(hdnSelected.Value, 0);
        if (templateId > 0)
        {
            btnInherit.Enabled = true;
            txtPageTemplate.Enabled = true;

            PageTemplateInfo pi = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);
            txtPageTemplate.Text = pi.DisplayName;
        }
        else
        {
            btnInherit.Enabled = false;
            txtPageTemplate.Enabled = false;
        }

        if (!String.IsNullOrEmpty(pageScript))
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "Script", pageScript);
        }
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        int selectedPageTemplateId = ValidationHelper.GetInteger(hdnSelected.Value, 0);

        // Inserts new document
        if (action == "new")
        {
            try
            {
                string name = txtPageName.Text;
                node = TreeNode.New(DataClassInfoProvider.DefaultPageClassName);
                node.DocumentName = name;
                node.SetDefaultPageTemplateID(selectedPageTemplateId);
                node.DocumentCulture = CultureHelper.GetDefaultCulture(siteName);

                DocumentHelper.InsertDocument(node, nodeId, tree);

                AddScript("RefreshNode(" + node.NodeParentID + "," + node.NodeID + "); SelectNode(" + node.NodeID + ");");

                ShowChangesSaved();
            }
            catch (Exception ex)
            {
                AddAlert(GetString("DefineSiteStructure.InsertFailed") + " : " + ex.Message);
            }
        }
        else if (action == "edit")
        {
            // Updates node
            try
            {
                node = tree.SelectSingleNode(nodeId);
                node.SetValue("DocumentName", txtPageName.Text);

                node.SetDefaultPageTemplateID(selectedPageTemplateId);

                DocumentHelper.UpdateDocument(node, tree);

                AddScript("RefreshNode(" + node.NodeParentID + "," + node.NodeID + "); SelectNode(" + node.NodeID + ");");

                ShowChangesSaved();
            }
            catch (Exception ex)
            {
                AddAlert(GetString("DefineSiteStructure.UpdateFailed") + " : " + ex.Message);
            }
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Adds the alert message to the output request window.
    /// </summary>
    /// <param name="message">Message to display</param>
    private void AddAlert(string message)
    {
        pageScript += ScriptHelper.GetAlertScript(message);
    }


    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        pageScript += ScriptHelper.GetScript(script);
    }

    #endregion
}