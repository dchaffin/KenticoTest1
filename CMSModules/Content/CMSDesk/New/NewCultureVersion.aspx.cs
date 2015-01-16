using System;
using System.Text;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.TranslationServices;
using CMS.SettingsProvider;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Content_CMSDesk_New_NewCultureVersion : CMSContentPage
{
    #region "Variables"

    TreeNode node = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the target culture code
    /// </summary>
    protected string RequiredCulture
    {
        get
        {
            return QueryHelper.GetString("culture", CMSContext.PreferredCultureCode);
        }
    }


    /// <summary>
    /// Gets a value that indicates if the page is loaded inside a split view.
    /// </summary>
    public bool IsInCompare
    {
        get
        {
            return QueryHelper.GetBoolean("compare", false);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Adds the script to the page
    /// </summary>
    /// <param name="script">JavaScript code</param>
    public override void AddScript(string script)
    {
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "NewCultureVersionScript", script, true);
    }


    protected override void OnInit(EventArgs e)
    {
        // Do not check changes
        DocumentManager.RegisterSaveChangesScript = false;

        // Do not redirect for non-existing document
        DocumentManager.RedirectForNonExistingDocument = false;

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register progress script
        ScriptHelper.RegisterProgress(Page);

        string culture = RequiredCulture;

        // Check if the culture is valid
        CheckPreferredCulture(true, culture);

        StringBuilder sb = new StringBuilder(); 
        sb.Append("var radCopyElem = document.getElementById('", radCopy.ClientID, "');\n",
                  "var radTranslations = document.getElementById('", radTranslate.ClientID, "');\n",
                  "function FramesRefresh(selectNodeId) { parent.RefreshTree(selectNodeId, selectNodeId); parent.SelectNode(selectNodeId); }");


        ltlScript.Text += ScriptHelper.GetScript(sb.ToString());

        // Setup page title text and image
        CultureInfo ci = CultureInfoProvider.GetCultureInfo(culture);
        string cult = (ci == null) ? null : " (" + HTMLHelper.HTMLEncode(ci.CultureName) + ")";
        CurrentMaster.Title.TitleText = GetString("Content.NewCultureVersionTitle") + cult;
        CurrentMaster.Title.TitleImage = UIHelper.GetFlagIconUrl(this, culture, "48x48");
        CurrentMaster.Title.HelpTopicName = "cms_content_newculture";

        lblInfo.Text = GetString("ContentNewCultureVersion.Info");
        radCopy.Text = GetString("ContentNewCultureVersion.Copy");
        radEmpty.Text = GetString("ContentNewCultureVersion.Empty");
        radTranslate.Text = GetString("ContentNewCultureVersion.Translate");

        radCopy.Attributes.Add("onclick", "ShowSelection();");
        radEmpty.Attributes.Add("onclick", "ShowSelection()");
        radTranslate.Attributes.Add("onclick", "ShowSelection()");

        btnCreateDocument.Text = GetString("ContentNewCultureVersion.Create");
        btnTranslate.Text = GetString("ContentNewCultureVersion.TranslateButton");
        btnTranslate.Click += btnTranslate_Click;
        btnCreateDocument.Click += btnCreateDocument_Click;

        if (NodeID > 0)
        {
            // Fill in the existing culture versions
            node = Tree.SelectSingleNode(NodeID, TreeProvider.ALL_CULTURES);
            if (node != null)
            {
                bool translationAllowed = SettingsKeyProvider.GetBoolValue(node.NodeSiteName + ".CMSEnableTranslations");
                if (translationAllowed)
                {
                    translationElem.TranslationSettings = new TranslationSettings
                                                              {
                                                                  TargetLanguage = culture
                                                              };
                    translationElem.NodeID = node.NodeID;
                }
                else
                {
                    translationElem.StopProcessing = true;
                    plcTranslationServices.Visible = false;
                }

                if (!CMSContext.CurrentUser.IsAuthorizedToCreateNewDocument(node.NodeParentID, node.NodeClassName))
                {
                    pnlNewVersion.Visible = false;
                    lblInfo.Visible = false;
                    ShowError(GetString("accessdenied.notallowedtocreatenewcultureversion"));
                }
                else
                {
                    SiteInfo si = SiteInfoProvider.GetSiteInfo(node.NodeSiteID);
                    if (si != null)
                    {
                        TreeNode originalNode = Tree.GetOriginalNode(node);
                        copyCulturesElem.UniSelector.DisplayNameFormat = "{% CultureName %}{% if (CultureCode == \"" + CultureHelper.GetDefaultCulture(si.SiteName) + "\") { \" \" +\"" + GetString("general.defaultchoice") + "\" } %}";
                        copyCulturesElem.AdditionalWhereCondition = "CultureCode IN (SELECT DocumentCulture FROM CMS_Document WHERE DocumentNodeID = " + originalNode.NodeID + ")";

                        if (!CMSContext.CurrentUser.IsCultureAllowed(culture, si.SiteName))
                        {
                            pnlNewVersion.Visible = false;
                            lblInfo.Visible = false;
                            ShowError(GetString("transman.notallowedcreate"));
                        }
                    }
                }
            }
            else
            {
                RedirectToInformation("editeddocument.notexists");
            }
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ShowSelection", "ShowSelection();", true);

        // Set title for new culture version in dialog mode
        if (RequiresDialog)
        {
            SetTitle("CMSModules/CMS_Content/Menu/New.png", GetString("content.newcultureversiontitle"), null, null);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide translations if no service is available
        if (!translationElem.AnyServiceAvailable)
        {
            plcTranslationServices.Visible = false;
        }
    }

    #endregion


    #region "Control events"

    protected void btnCreateDocument_Click(object sender, EventArgs e)
    {
        if (radCopy.Checked)
        {
            string sourceCulture = copyCulturesElem.Value.ToString();
            TreeNode actualSourceNode = DocumentHelper.GetDocument(NodeID, sourceCulture, Tree);
            TreeNode sourceNode = actualSourceNode.IsLink ? DocumentHelper.GetDocument(Tree.GetOriginalNode(actualSourceNode), Tree) : actualSourceNode;

            if (sourceNode != null)
            {
                if (chkSaveBeforeEditing.Checked && (node != null))
                {
                    // Create the version first
                    TreeNode newCulture = TreeNode.New(node.ClassName);
                    DocumentHelper.CopyNodeData(sourceNode, newCulture, new CopyNodeDataSettings(true, null));
                    NewCultureDocumentSettings settings = new NewCultureDocumentSettings
                                                                {
                                                                    Node = newCulture,
                                                                    CultureCode = RequiredCulture,
                                                                    CopyAttachments = true,
                                                                    CopyCategories = true
                                                                };
                    DocumentHelper.InsertNewCultureVersion(settings);

                    // Refresh page
                    if (RequiresDialog)
                    {
                        string url = URLHelper.ResolveUrl(DocumentURLProvider.GetUrl(newCulture.NodeAliasPath) + "?" + URLHelper.LanguageParameterName + "=" + RequiredCulture);
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "NewCultureRefreshAction", ScriptHelper.GetScript(" wopener.location = " + ScriptHelper.GetString(url) + "; CloseDialog();"));
                    }
                    else
                    {
                        ViewModeEnum mode = ViewModeEnum.Edit;
                        if (!TreePathUtils.IsMenuItemType(node.NodeClassName) && (CMSContext.ViewMode != ViewModeEnum.EditLive))
                        {
                            mode = ViewModeEnum.EditForm;
                        }
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "NewCultureRefreshAction", ScriptHelper.GetScript("if (FramesRefresh) { FramesRefresh(" + node.NodeID + ", '" + mode + "'); }"));
                    }
                }
                else
                {
                    var url = GetEditUrl(node);
                    url = URLHelper.AddParameterToUrl(url, "sourcedocumentid", sourceNode.DocumentID.ToString());
                    // Provide information about actual node
                    if ((actualSourceNode != null) && actualSourceNode.IsLink)
                    {
                        url = URLHelper.AddParameterToUrl(url, "sourcenodeid", actualSourceNode.NodeID.ToString());
                    }
                    Response.Redirect(url);
                }
            }
            else
            {
                ShowError(GetString("transman.notallowedcreate"));
            }
        }
        else
        {
            var url = GetEditUrl(node);
            Response.Redirect(url);
        }
    }


    protected void btnTranslate_Click(object sender, EventArgs e)
    {
        if (TranslationServiceHelper.IsAuthorizedToTranslateDocument(node, CMSContext.CurrentUser))
        {
            try
            {
                // Submits the document to translation service
                string err = translationElem.SubmitToTranslation();
                if (string.IsNullOrEmpty(err))
                {
                    // Refresh page
                    if (RequiresDialog)
                    {
                        string url = URLHelper.ResolveUrl(DocumentURLProvider.GetUrl(node.NodeAliasPath) + "?" + URLHelper.LanguageParameterName + "=" + RequiredCulture);
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "NewCultureRefreshAction", ScriptHelper.GetScript(" window.top.location = " + ScriptHelper.GetString(url)));
                    }
                    else
                    {
                        ViewModeEnum mode = ViewModeEnum.Edit;
                        if (!TreePathUtils.IsMenuItemType(node.NodeClassName) && (CMSContext.ViewMode != ViewModeEnum.EditLive))
                        {
                            mode = ViewModeEnum.EditForm;
                        }
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "NewCultureRefreshAction", ScriptHelper.GetScript("if (FramesRefresh) { FramesRefresh(" + node.NodeID + ", '" + mode + "'); }"));
                    }
                }
                else
                {
                    ShowError(err);
                }
            }
            catch (Exception ex)
            {
                ShowError(GetString("ContentRequest.TranslationFailed"), ex.Message, null);
                TranslationServiceHelper.LogEvent(ex);
            }
        }
        else
        {
            RedirectToCMSDeskAccessDenied("CMS.Content", "SubmitForTranslation");
        }
    }

    #endregion


    #region "Methods"

    private string GetEditUrl(TreeNode node)
    {
        string url = null;
        if (node.HasSKU && ModuleEntry.IsModuleLoaded(ModuleEntry.ECOMMERCE))
        {
            url = "~/CMSModules/Ecommerce/Pages/Content/Product/Product_Edit_General.aspx";
        }
        else
        {
            url = "~/CMSModules/Content/CMSDesk/Edit/Edit.aspx";
        }
        
        url = URLHelper.AddParameterToUrl(url, "nodeid", NodeID.ToString());
        url = URLHelper.AddParameterToUrl(url, "action", "newculture");
        url = URLHelper.AddParameterToUrl(url, "mode", Mode);
        url = URLHelper.AddParameterToUrl(url, "parentculture", RequiredCulture);
        
        if (IsInCompare)
        {
            url = URLHelper.AddParameterToUrl(url, "compare", "1");
        }

        if (RequiresDialog)
        {
            url = URLHelper.AddParameterToUrl(url, "dialog", "1");
        }
        
        return url;
    }

    #endregion
}