using System;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.PortalEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

[RegisterTitle("content.ui.propertiesurls")]
public partial class CMSModules_Content_CMSDesk_Properties_Alias_List : CMSPropertiesPage
{
    #region "Private variables"

    bool isRoot = false;
    String oldAliasPath = String.Empty;

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        // Culture independent data
        SplitModeAllwaysRefresh = true;

        // Non-version data is modified
        DocumentManager.UseDocumentHelper = false;

        base.OnInit(e);

        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.URLs"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Properties.URLs");
        }

        // Redirect to information page when no UI elements displayed
        if (pnlUIDocumentAlias.IsHidden && pnlUIExtended.IsHidden && pnlUIPath.IsHidden)
        {
            RedirectToUINotAvailable();
        }

        // Enable split mode
        EnableSplitMode = true;

        // Init document manager events
        DocumentManager.OnSaveData += DocumentManager_OnSaveData;
        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;

        // Initialize node
        isRoot = (Node.NodeClassName.ToLowerCSafe() == "cms.root");

        UniGridAlias.StopProcessing = pnlUIDocumentAlias.IsHidden;

        // Register the scripts
        ScriptHelper.RegisterProgress(Page);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        UIContext.PropertyTab = PropertyTabEnum.URLs;

        // Set where condition - show nothing when nodeId is zero
        UniGridAlias.WhereCondition = "AliasNodeID = " + NodeID;

        if (Node != null)
        {
            imgNewAlias.ImageUrl = GetImageUrl("CMSModules/CMS_Content/Properties/adddocumentalias.png");
            imgNewAlias.DisabledImageUrl = GetImageUrl("CMSModules/CMS_Content/Properties/adddocumentaliasdisabled.png");

            if (Node.NodeAliasPath == "/")
            {
                valAlias.Visible = false;
            }

            // Check modify permissions
            if ((CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied))
            {
                pnlAlias.Enabled = false;
                menuElem.Enabled = false;
                UniGridAlias.GridView.Enabled = false;
                lnkNewAlias.Disabled = true;
                imgNewAlias.Enabled = false;

                chkCustomExtensions.Enabled = false;
                ctrlURL.Enabled = false;
            }
            else
            {
                ltlScript.Text = ScriptHelper.GetScript("var node = " + NodeID + "; \n var deleteMsg = '" + GetString("DocumentAlias.DeleteMsg") + "';");

                UniGridAlias.OnAction += UniGridAlias_OnAction;
                UniGridAlias.OnExternalDataBound += UniGridAlias_OnExternalDataBound;
                lnkNewAlias.HRef = URLHelper.ResolveUrl("Alias_Edit.aspx?nodeid=" + NodeID);
            }

            Literal ltl = new Literal();
            ltl.Text = GetString("doc.urls.addnewalias");
            lnkNewAlias.Controls.Add(ltl);

            ltl = new Literal();
            ltl.Text = GetString("doc.urls.aliaslist");

            lblAlias.Text = GetString("GeneralProperties.Alias");

            chkCustomExtensions.Text = GetString("GeneralProperties.UseCustomExtensions");
            valAlias.ErrorMessage = GetString("GeneralProperties.RequiresAlias");

            lblExtensions.Text = GetString("doc.urls.urlextensions") + ResHelper.Colon;

            pnlURLPath.GroupingText = GetString("GeneralProperties.UrlPath");
            pnlAlias.GroupingText = GetString("GeneralProperties.DocumentAlias");

            pnlDocumentAlias.GroupingText = GetString("doc.urls.documentalias");
            pnlExtended.GroupingText = GetString("doc.urls.extendedproperties");

            if (!isRoot)
            {
                txtAlias.Enabled = !TreePathUtils.AutomaticallyUpdateDocumentAlias(CMSContext.CurrentSiteName);
            }

            if (!RequestHelper.IsPostBack())
            {
                ReloadData();
            }

            ctrlURL.AutomaticURLPath = TreePathUtils.GetUrlPathFromNamePath(Node.DocumentNamePath, Node.NodeLevel, CMSContext.CurrentSiteName);

            // Reflect processing action
            pnlPageContent.Enabled = DocumentManager.AllowSave;
        }
    }


    protected object UniGridAlias_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "culture":
                return UniGridFunctions.CultureDisplayName(parameter);

            case "urlpath":
                {
                    // Parse the URL path
                    string urlPath = ValidationHelper.GetString(parameter, "");

                    return TreePathUtils.GetURLPathDisplayName(urlPath);
                }
        }

        return parameter;
    }


    private void UniGridAlias_OnAction(string actionName, object actionArgument)
    {
        if (Node != null)
        {
            // Check modify permissions
            if (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                return;
            }

            string action = DataHelper.GetNotEmpty(actionName, String.Empty).ToLowerCSafe();

            switch (action)
            {
                case "edit":
                    // Edit action
                    URLHelper.Redirect(URLHelper.ResolveUrl("Alias_Edit.aspx?nodeid=" + NodeID + "&aliasid=" + Convert.ToString(actionArgument)));
                    break;

                case "delete":
                    // Delete action
                    int aliasId = ValidationHelper.GetInteger(actionArgument, 0);
                    if (aliasId > 0)
                    {
                        // Delete
                        DocumentAliasInfoProvider.DeleteDocumentAliasInfo(aliasId);

                        // Log synchronization
                        DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, DocumentManager.Tree);
                    }
                    break;
            }
        }
    }


    protected void chkCustomExtensions_CheckedChanged(object sender, EventArgs e)
    {
        txtExtensions.Enabled = chkCustomExtensions.Checked;
        if (!chkCustomExtensions.Checked)
        {
            txtExtensions.Text = Node.DocumentExtensions;
        }
    }


    private void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        txtAlias.Text = Node.NodeAlias;

        // If alias was changed, update all related MVTests
        if (Node.NodeAliasPath != oldAliasPath)
        {
            ModuleCommands.OnlineMarketingMoveMVTests(Node.NodeAliasPath, oldAliasPath, CMSContext.CurrentSiteID);
        }

        // Load the URL path
        LoadURLPath(Node);

        UniGridAlias.ReloadData();
    }


    private void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        e.IsValid = !string.IsNullOrEmpty(txtAlias.Text.Trim()) || isRoot;
        if (e.IsValid)
        {
            // PATH group is displayed
            if (!pnlUIPath.IsHidden)
            {
                // Validate URL path
                if (!ctrlURL.IsValid())
                {
                    e.IsValid = false;
                    e.ErrorMessage = ctrlURL.ValidationError;
                    return;
                }

                string nodeAlias = txtAlias.Text.Trim();
                bool aliasChanged = (Node.NodeAlias != nodeAlias) || (ctrlURL.URLPath != Node.DocumentUrlPath);
                oldAliasPath = Node.NodeAliasPath;
                Node.NodeAlias = nodeAlias;
                Node.DocumentUseNamePathForUrlPath = !ctrlURL.IsCustom;
                if (Node.DocumentUseNamePathForUrlPath)
                {
                    string urlPath = TreePathUtils.GetUrlPathFromNamePath(Node.DocumentNamePath, Node.NodeLevel, CMSContext.CurrentSiteName);
                    Node.DocumentUrlPath = urlPath;
                }
                else
                {
                    Node.DocumentUrlPath = TreePathUtils.GetSafeUrlPath(ctrlURL.URLPath, CMSContext.CurrentSiteName, true);
                }

                if (aliasChanged && (CMSContext.ViewMode == ViewModeEnum.EditLive))
                {
                    // Redirect the parent page to the new document alias
                    string newAliasPath = string.Empty;
                    if (Node.Parent != null)
                    {
                        newAliasPath = Node.Parent.NodeAliasPath.TrimEnd('/');
                    }
                    newAliasPath += "/" + Node.NodeAlias;

                    // Get the updated document url
                    string url = URLHelper.ResolveUrl(DocumentURLProvider.GetUrl(newAliasPath, Node.DocumentUrlPath, Node.NodeSiteName, URLHelper.CurrentUrlLangPrefix));

                    // Register redirect script
                    string reloadScript = "if (parent.parent.frames['header'] != null) { parent.parent.frames['header'].reloadPageUrl =" + ScriptHelper.GetString(url) + "; }";
                    ScriptHelper.RegisterStartupScript(this, typeof(string), "reloadScript", reloadScript, true);
                }
            }

            // EXTENDED group is displayed
            if (!pnlUIExtended.IsHidden)
            {
                Node.DocumentUseCustomExtensions = chkCustomExtensions.Checked;
                if (Node.DocumentUseCustomExtensions)
                {
                    Node.DocumentExtensions = txtExtensions.Text;
                }
            }
        }
        else
        {
            e.ErrorMessage = GetString("general.errorvalidationerror");
        }
    }


    private void ReloadData()
    {
        if (Node != null)
        {
            // Check modify permissions
            if (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                pnlNewAlias.Enabled = false;
                pnlAlias.Enabled = false;
                pnlURLPath.Enabled = false;
                pnlExtended.Enabled = false;
                pnlDocumentAlias.Enabled = false;
            }

            ctrlURL.IsCustom = !Node.DocumentUseNamePathForUrlPath;
            chkCustomExtensions.Checked = Node.DocumentUseCustomExtensions;

            txtExtensions.Text = Node.DocumentExtensions;
            txtAlias.Text = Node.NodeAlias;

            // Load the URL path
            LoadURLPath(Node);
        }
    }


    /// <summary>
    /// Loads the URL path to the UI
    /// </summary>
    private void LoadURLPath(TreeNode treeNode)
    {
        ctrlURL.URLPath = treeNode.DocumentUrlPath;

        txtExtensions.Enabled = chkCustomExtensions.Checked;

        if (isRoot)
        {
            txtAlias.Enabled = false;
            valAlias.Enabled = false;

            ctrlURL.Enabled = false;

            chkCustomExtensions.Enabled = false;
        }

        if (treeNode.IsLink)
        {
            ctrlURL.Enabled = false;
        }
    }

    #endregion
}