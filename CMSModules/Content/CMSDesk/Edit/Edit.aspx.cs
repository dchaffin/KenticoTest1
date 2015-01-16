using System;
using System.Linq;
using System.Data;

using CMS.CMSHelper;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.ExtendedControls;

public partial class CMSModules_Content_CMSDesk_Edit_Edit : CMSContentPage
{
    #region "Variables"

    protected bool newdocument = false;
    protected bool newculture = false;
    protected bool mShowToolbar = false;

    protected DataClassInfo ci = null;

    #endregion


    #region "Protected variables"

    /// <summary>
    /// Local page message placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            // Ensure default offset in special case of edit page
            MessagesPlaceHolder plc = base.MessagesPlaceHolder;
            if (plc != null)
            {
                plc.WrapperControlID = "pnlContent";
                plc.OffsetY = 7;
            }
            return plc;
        }
    }


    /// <summary>
    /// Class identifier for new documents.
    /// </summary>
    protected int ClassID
    {
        get
        {
            return QueryHelper.GetInteger("classid", 0);
        }
    }


    /// <summary> 
    /// TemplateID, used when Use template selection is enabled for class of newly created document.
    /// </summary>
    protected int TemplateID
    {
        get
        {
            return QueryHelper.GetInteger("templateid", -1);
        }
    }


    /// <summary>
    /// Identifier of parent document. (For newly created documents.)
    /// </summary>
    protected int ParentNodeID
    {
        get
        {
            return QueryHelper.GetInteger("parentnodeid", 0);
        }
    }


    /// <summary>
    /// Culture of parent document. (For newly created documents.)
    /// </summary>
    protected string ParentCulture
    {
        get
        {
            return QueryHelper.GetString("parentculture", CMSContext.PreferredCultureCode);
        }
    }


    /// <summary>
    /// Indicates if e-commerce product section is edited.
    /// </summary>
    protected bool ProductSection
    {
        get
        {
            return (Mode != null) && (Mode.ToLowerCSafe() == "productssection") && (ci != null) && (ci.ClassIsProductSection);
        }
    }


    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Do not redirect for non-existing document if new culture version is being created
        DocumentManager.RedirectForNonExistingDocument = (Action != "newculture");

        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        CurrentUserInfo currentUser = CMSContext.CurrentUser;

        // Check UIProfile
        if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", "EditForm"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "EditForm");
        }

        base.OnInit(e);

        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;
        DocumentManager.OnLoadData += DocumentManager_OnLoadData;

        // Register scripts
        string script = "function " + formElem.ClientID + "_RefreshForm(){" + Page.ClientScript.GetPostBackEventReference(btnRefresh, "") + " }";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), formElem.ClientID + "_RefreshForm", ScriptHelper.GetScript(script));

        ScriptHelper.RegisterCompletePageScript(this);
        ScriptHelper.RegisterProgress(this);
        ScriptHelper.RegisterDialogScript(this);

        formElem.OnBeforeBasicFormDataLoad += formElem_OnBeforeDataLoad;
        formElem.OnAfterDataLoad += formElem_OnAfterDataLoad;

        // Analyze the action parameter
        switch (Action)
        {
            case "new":
            case "convert":
                {
                    newdocument = true;

                    // Check if document type is allowed under parent node
                    if ((ParentNodeID > 0) && (ClassID > 0))
                    {
                        // Check class
                        ci = DataClassInfoProvider.GetDataClass(ClassID);
                        if (ci == null)
                        {
                            throw new Exception("[Content/Edit.aspx]: Class ID '" + ClassID + "' not found.");
                        }

                        if (ci.ClassName.ToLowerCSafe() == "cms.blog")
                        {
                            if (!LicenseHelper.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Blogs, VersionActionEnum.Insert))
                            {
                                RedirectToAccessDenied(String.Format(GetString("cmsdesk.bloglicenselimits"), ""));
                            }
                        }

                        if (!LicenseHelper.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Documents, VersionActionEnum.Insert))
                        {
                            RedirectToAccessDenied(String.Format(GetString("cmsdesk.documentslicenselimits"), ""));
                        }

                        // Check if need template selection, if so, then redirect to template selection page
                        int templateId = TemplateID;
                        if (!ProductSection && ci.ClassShowTemplateSelection && (templateId < 0) && !ci.ClassName.EqualsCSafe(DataClassInfoProvider.DefaultPageClassName, true))
                        {
                            URLHelper.Redirect("~/CMSModules/Content/CMSDesk/TemplateSelection.aspx" + URLHelper.Url.Query);
                        }

                        // Set default template ID
                        formElem.DefaultPageTemplateID = (templateId > 0) ? templateId : ci.ClassDefaultPageTemplateID;

                        string newClassName = ci.ClassName;
                        formElem.FormName = newClassName + ".default";

                        DocumentManager.Mode = FormModeEnum.Insert;
                        DocumentManager.ParentNodeID = ParentNodeID;
                        DocumentManager.NewNodeCultureCode = ParentCulture;

                        // Set up the document conversion
                        int convertDocumentId = QueryHelper.GetInteger("convertdocumentid", 0);
                        if (convertDocumentId > 0)
                        {
                            DocumentManager.SourceDocumentID = convertDocumentId;
                            DocumentManager.Mode = FormModeEnum.Convert;
                        }

                        DocumentManager.NewNodeClassID = ClassID;

                        // Check allowed document type
                        TreeNode parentNode = DocumentManager.ParentNode;
                        if ((parentNode == null) || !DataClassInfoProvider.IsChildClassAllowed(parentNode.GetIntegerValue("NodeClassID", 0), ClassID))
                        {
                            AddNotAllowedScript("child");
                        }

                        if (!currentUser.IsAuthorizedToCreateNewDocument(DocumentManager.ParentNode, DocumentManager.NewNodeClassName))
                        {
                            AddNotAllowedScript("new");
                        }
                    }

                    if (RequiresDialog)
                    {
                        SetTitle("CMSModules/CMS_Content/Menu/New.png", GetString("Content.NewTitle"), null, null);
                    }
                }
                break;

            case "newculture":
                {
                    newculture = true;
                    int nodeId = QueryHelper.GetInteger("nodeid", 0);
                    DocumentManager.Mode = FormModeEnum.InsertNewCultureVersion;
                    formElem.NodeID = nodeId;

                    // Check permissions
                    bool authorized = false;
                    if (nodeId > 0)
                    {
                        // Get the node                    
                        TreeNode treeNode = DocumentManager.Tree.SelectSingleNode(nodeId);
                        if (treeNode != null)
                        {
                            DocumentManager.NewNodeClassID = treeNode.GetIntegerValue("NodeClassID", 0);
                            DocumentManager.ParentNodeID = ParentNodeID;
                            DocumentManager.NewNodeCultureCode = ParentCulture;
                            DocumentManager.SourceDocumentID = QueryHelper.GetInteger("sourcedocumentid", 0);
                            authorized = currentUser.IsAuthorizedToCreateNewDocument(treeNode.NodeParentID, treeNode.NodeClassName);

                            if (authorized)
                            {
                                string className = DocumentManager.NewNodeClassName;
                                if (className.ToLowerCSafe() == "cms.blog")
                                {
                                    if (!LicenseHelper.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Blogs, VersionActionEnum.Insert))
                                    {
                                        RedirectToAccessDenied(String.Format(GetString("cmsdesk.bloglicenselimits"), ""));
                                    }
                                }

                                if (!LicenseHelper.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Documents, VersionActionEnum.Insert))
                                {
                                    RedirectToAccessDenied(String.Format(GetString("cmsdesk.documentslicenselimits"), ""));
                                }

                                ci = DataClassInfoProvider.GetDataClass(className);
                                formElem.FormName = className + ".default";
                            }
                        }
                    }

                    if (!authorized)
                    {
                        AddNotAllowedScript("newculture");
                    }

                    if (RequiresDialog)
                    {
                        SetTitle("CMSModules/CMS_Content/Menu/New.png", GetString("content.newcultureversiontitle"), null, null);
                    }
                }
                break;

            default:
                {
                    TreeNode node = Node;
                    if (node == null)
                    {
                        RedirectToNewCultureVersionPage();
                    }
                    else
                    {
                        // Update view mode
                        if (!RequiresDialog)
                        {
                            CMSContext.ViewMode = ViewModeEnum.EditForm;
                        }

                        EnableSplitMode = true;
                        DocumentManager.Mode = FormModeEnum.Update;
                        ci = DataClassInfoProvider.GetDataClass(node.NodeClassName);
                        if (RequiresDialog)
                        {
                            menuElem.ShowSaveAndClose = true;

                            // Add the document name to the properties header title
                            string nodeName = node.GetDocumentName();
                            // Get name for root document
                            if (node.NodeClassName.ToLowerCSafe() == "cms.root")
                            {
                                nodeName = CMSContext.CurrentSite.DisplayName;
                            }

                            SetTitle("CMSModules/CMS_Content/Menu/New.png", GetString("Content.EditTitle") + " \"" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(nodeName)) + "\"", null, null);
                        }
                    }
                }
                break;
        }

        formElem.Visible = true;

        // Display / hide the CK editor toolbar area
        FormInfo fi = FormHelper.GetFormInfo(ci.ClassName, false);

        if (fi.UsesHtmlArea())
        {
            // Add script to display toolbar
            if (formElem.HtmlAreaToolbarLocation.ToLowerCSafe() == "out:cktoolbar")
            {
                mShowToolbar = true;
            }
        }

        // Init form for product section edit
        if (ProductSection)
        {
            // Form prefix
            formElem.FormPrefix = "ecommerce";
        }

        if (RequiresDialog)
        {
            plcCKFooter.Visible = false;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Register script files
        ScriptHelper.RegisterScriptFile(this, "cmsedit.js");
        ScriptHelper.RegisterTooltip(this);

        formElem.MessagesPlaceHolder.ClearLabels();

        InitBindSkuAction();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterStartupScript(this, typeof(string), "InitializePage", ScriptHelper.GetScript("InitializePage()"));

        // Title and breadcrumbs for product section edit
        if (ProductSection)
        {
            bool isNew = Action == "new";
            TreeNode node = DocumentManager.Node;
            string className = (node != null) ? node.ClassName : ci.ClassName;

            // Title
            CurrentMaster.Title.TitleImage = GetDocumentTypeIconUrl(className, "48x48");
            CurrentMaster.Title.TitleText = GetString(isNew ? "com.productsection.new" : "com.productsection.properties");
            CurrentMaster.Title.HelpTopicName = "new_edit_product_section";

            // Get products starting path
            string key = SettingsKeyProvider.GetFullKeyName(CurrentSiteName, "CMSStoreProductsStartingPath");
            string productsStartingPath = SettingsKeyProvider.GetStringValue(key);

            // Create and set breadcrumbs
            TreeNode breadcrumbNode = isNew ? DocumentManager.ParentNode : node;
            if (breadcrumbNode != null)
            {
                CurrentMaster.Title.Breadcrumbs = CreateBreadcrumbs(breadcrumbNode, "/" + productsStartingPath.Trim('%').Trim('/') + "%");
            }
        }

        if (!newdocument && !newculture)
        {
            formElem.Enabled = DocumentManager.AllowSave;

            imgBindSku.ImageUrl = DocumentManager.AllowSave ? GetImageUrl("Objects/Ecommerce_SKU/bind.png") : GetImageUrl("Objects/Ecommerce_SKU/binddisabled.png");
            btnBindSku.Enabled = DocumentManager.AllowSave;
        }
        if (!RequiresDialog)
        {
            // Set dialog title
            ScriptHelper.RegisterTitleScript(this, GetString("content.ui.form"));
        }
    }

    #endregion


    #region "Document manager events"

    protected void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        TreeNode node = e.Node;

        int newNodeId = node.NodeID;
        if (newdocument || newculture)
        {
            // Store error text
            if (!string.IsNullOrEmpty(formElem.MessagesPlaceHolder.ErrorText))
            {
                SessionHelper.SetValue("FormErrorText|" + newNodeId, formElem.MessagesPlaceHolder.ErrorText);
            }
        }
        else if (e.ActionName == ComponentEvents.UNDO_CHECKOUT)
        {
            // Reload the values in the form
            formElem.BasicForm.LoadControlValues();
        }
    }


    protected void DocumentManager_OnLoadData(object sender, DocumentManagerEventArgs e)
    {
        formElem.BasicForm.LoadControlValues();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Adds script for redirecting to NotAllowed page.
    /// </summary>
    /// <param name="action">Action string</param>
    private void AddNotAllowedScript(string action)
    {
        AddScript("window.location.replace('../NotAllowed.aspx?action=" + action + "')");
    }


    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), script.GetHashCode().ToString(), ScriptHelper.GetScript(script));
    }


    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        TreeNode node = DocumentManager.Node;

        // Check permission to modify document
        if (CMSContext.CurrentUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed)
        {
            // Ensure version for later detection whether node is published
            node.VersionManager.EnsureVersion(node, node.IsPublished);

            // Tree refresh is needed only if node was archived or published
            WorkflowStepInfo currentStep = node.WorkflowStep;
            bool refreshTree = (currentStep != null) && (currentStep.StepIsArchived || currentStep.StepIsPublished);

            // Move to edit step
            node.MoveToFirstStep(null);

            // Refresh tree
            if (refreshTree)
            {
                ScriptHelper.RefreshTree(this, node.NodeID, node.NodeID);
            }

            // Reload form
            formElem.LoadForm(true);

            if (DocumentManager.SaveChanges)
            {
                ScriptHelper.RegisterStartupScript(this, typeof(string), "moveToEditStepChange", ScriptHelper.GetScript("Changed();"));
            }
        }
    }


    protected void formElem_OnBeforeDataLoad(object sender, EventArgs e)
    {
        bool hasCategories = (formElem.BasicForm != null) && formElem.BasicForm.FormInformation.ItemsList.OfType<FormCategoryInfo>().Any();
        if (hasCategories)
        {
            formElem.DefaultFormLayout = FormLayoutEnum.FieldSets;
            formElem.DefaultCategoryName = ResHelper.GetString("general.general");
        }
    }


    protected void formElem_OnAfterDataLoad(object sender, EventArgs e)
    {
        TreeNode node = DocumentManager.Node;
        if (node != null)
        {
            // Show stored error message
            string frmError = SessionHelper.GetValue("FormErrorText|" + node.NodeID) as string;
            if (!string.IsNullOrEmpty(frmError))
            {
                formElem.ShowError(frmError);
                // Remove error message
                SessionHelper.Remove("FormErrorText|" + node.NodeID);
            }
        }
    }


    private void InitBindSkuAction()
    {
        if (RequiresDialog)
        {
            return;
        }

        if ((DocumentManager.NodeID > 0) && (DocumentManager.Node != null) && (Action != "newculture"))
        {
            var dataClass = DataClassInfoProvider.GetDataClass(DocumentManager.Node.ClassName);
            if ((dataClass != null) && ModuleEntry.IsModuleLoaded(ModuleEntry.ECOMMERCE))
            {
                plcSkuBinding.Visible = dataClass.ClassIsProduct;

                btnBindSku.Click += (sender, args) =>
                {
                    string url = "~/CMSModules/Ecommerce/Pages/Content/Product/Product_Edit_General.aspx";
                    url = URLHelper.AddParameterToUrl(url, "nodeid", DocumentManager.NodeID.ToString());
                    url = URLHelper.AddParameterToUrl(url, "action", "bindsku");
                    if (RequiresDialog)
                    {
                        url = URLHelper.AddParameterToUrl(url, "dialog", "1");
                    }
                    Response.Redirect(url);
                };
            }
        }
    }


    /// <summary>
    /// Creates breadcrumbs array leading to specified node using given tree provider and starting at given starting path.
    /// </summary>
    /// <param name="node">Tree node to generate breadcrumbs for.</param>
    /// <param name="startingPath">Path where to start to generate.</param>
    protected virtual string[,] CreateBreadcrumbs(TreeNode node, string startingPath)
    {
        string[,] breadcrumbs = null;

        if (node != null)
        {
            const string columns = "NodeID, NodeAliasPath, NodeSiteID, NodeOwner, DocumentName, DocumentLastVersionName, DocumentMenuCaption, DocumentCulture, SiteName, ClassName, NodeACLID";

            // Prepare the where condition
            string where = SqlHelperClass.AddWhereCondition(null, TreeProvider.GetNodesOnPathWhereCondition(node.NodeAliasPath, true, true));
            DataSet ds = DocumentHelper.GetDocuments(CurrentSiteName, startingPath, TreeProvider.ALL_CULTURES, true, null, where, "NodeAliasPath ASC", -1, false, 0, columns, node.TreeProvider);

            ds = TreeSecurityProvider.FilterDataSetByPermissions(ds, NodePermissionsEnum.Read, CMSContext.CurrentUser);

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                // Initialize resolver
                ContextResolver currentResolver = CMSContext.CurrentResolver.CreateContextChild();
                currentResolver.EncodeResolvedValues = true;

                DataRow[] resolverData = new DataRow[1];
                DataTable table = ds.Tables[0];

                // Prepare breadcrumbs
                breadcrumbs = new string[table.Rows.Count + 1, 4];

                int i = 0;

                foreach (DataRow dr in table.Rows)
                {
                    // Add current datarow to the resolver
                    resolverData[0] = dr;
                    currentResolver.SourceData = resolverData;

                    // Prepare the item name. Disable encoding.
                    currentResolver.EncodeResolvedValues = false;
                    string linkName = currentResolver.ResolveMacros(TreePathUtils.GetMenuCaption(ValidationHelper.GetString(dr["DocumentMenuCaption"], String.Empty), ValidationHelper.GetString(dr["DocumentName"], String.Empty)));
                    currentResolver.EncodeResolvedValues = true;

                    // Use site name for root node
                    linkName = string.IsNullOrEmpty(linkName) ? CMSContext.CurrentSite.DisplayName : linkName;

                    // Create breadcrumb
                    breadcrumbs[i, 0] = HTMLHelper.HTMLEncode(linkName);
                    breadcrumbs[i, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_List.aspx?nodeid=" + dr["NodeID"];
                    breadcrumbs[i, 3] = string.Format("EditDocument({0}); RefreshTree({0},{0});", dr["NodeID"]);

                    // Increment index
                    i++;
                }

                // Add 'properties' breadcrumb
                breadcrumbs[i, 0] = HTMLHelper.HTMLEncode(GetString((Action == "new") ? "com.productsection.new" : "com.productsection.properties"));
                breadcrumbs[i, 1] = "";
                breadcrumbs[i, 2] = "";
            }
        }

        return breadcrumbs;
    }

    #endregion
}