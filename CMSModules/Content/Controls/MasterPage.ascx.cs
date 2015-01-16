using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.DocumentEngine;
using CMS.CMSHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.GlobalHelper;
using CMS.ExtendedControls;
using CMS.SiteProvider;
using CMS.IO;
using CMS.ExtendedControls.ActionsConfig;
using CMS.VirtualPathHelper;
using CMS.PortalControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Content_Controls_MasterPage : CMSPreviewControl
{
    #region "Variables"

    protected string mSave = null;
    protected int nodeId = 0;
    protected TreeNode node = null;
    protected TreeProvider tree = null;
    protected CurrentUserInfo user = null;
    protected PageTemplateInfo pti = null;

    protected string mHead = null;
    protected string mBody = null;

    private int previewState = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessagePlaceholder;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "DivRegistration", ScriptHelper.GetScript("if (parent.InitScrollDivs != null) {parent.InitScrollDivs('" + ParentClientID + "');}"));

        // Register save event
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, (s, args) => SaveData());

        if (CMSContext.DisplaySplitMode)
        {
            ShowPreview = false;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterResizeHeaders();

        CMSContext.ViewMode = ViewModeEnum.MasterPage;
        previewState = GetPreviewStateFromCookies(MASTERPAGE);

        // Keep current user
        user = CMSContext.CurrentUser;

        // Get document node
        tree = new TreeProvider(user);
        node = CMSContext.EditedObject as TreeNode;

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);

        // Register save changes
        ScriptHelper.RegisterSaveChanges(Page);

        // Save changes support
        bool confirmChanges = SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSConfirmChanges");
        string script = string.Empty;
        if (confirmChanges)
        {
            script = "var confirmLeave='" + ResHelper.GetString("Content.ConfirmLeave", user.PreferredUICultureCode) + "'; \n";
        }
        else
        {
            script += "confirmChanges = false;";
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "saveChangesScript", ScriptHelper.GetScript(script));

        try
        {
            if (node != null)
            {
                CMSContext.CurrentPageInfo = PageInfoProvider.GetPageInfo(CMSContext.CurrentSiteName, node.NodeAliasPath, node.DocumentCulture, null, false);

                // Title
                string title = CMSContext.CurrentTitle;
                if (!string.IsNullOrEmpty(title))
                {
                    title = "<title>" + title + "</title>";
                }

                // Body class
                string bodyCss = CMSContext.CurrentBodyClass;
                if (bodyCss != null && bodyCss.Trim() != "")
                {
                    bodyCss = "class=\"" + bodyCss + "\"";
                }
                else
                {
                    bodyCss = "";
                }

                // Metadata
                string meta = "<meta http-equiv=\"pragma\" content=\"no-cache\" />";

                string description = CMSContext.CurrentDescription;
                if (description != "")
                {
                    meta += "<meta name=\"description\" content=\"" + description + "\" />";
                }

                string keywords = CMSContext.CurrentKeyWords;
                if (keywords != "")
                {
                    meta += "<meta name=\"keywords\"  content=\"" + keywords + "\" />";
                }

                // Site style sheet
                string cssSiteSheet = "";

                int stylesheetId = CMSContext.CurrentPageInfo.DocumentStylesheetID;

                CssStylesheetInfo cssInfo = CssStylesheetInfoProvider.GetCssStylesheetInfo((stylesheetId > 0) ? stylesheetId : CMSContext.CurrentSite.SiteDefaultStylesheetID);

                if (cssInfo != null)
                {
                    cssSiteSheet = CSSHelper.GetCSSFileLink(CSSHelper.GetStylesheetUrl(cssInfo.StylesheetName));
                }

                // Theme CSS files
                string themeCssFiles = "";
                if (cssInfo != null)
                {
                    try
                    {
                        string directory = URLHelper.GetPhysicalPath(string.Format("~/App_Themes/{0}/", cssInfo.StylesheetName));
                        if (Directory.Exists(directory))
                        {
                            foreach (string file in Directory.GetFiles(directory, "*.css"))
                            {
                                themeCssFiles += CSSHelper.GetCSSFileLink(CSSHelper.GetPhysicalCSSUrl(cssInfo.StylesheetName, Path.GetFileName(file)));
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                // Add values to page
                mHead = FormatHTML(HighlightHTML(title + meta + cssSiteSheet + themeCssFiles), 2);
                mBody = bodyCss;
            }
        }
        catch
        {
            ShowError(GetString("MasterPage.PageEditErr"));
        }


        LoadData();

        // Add save action
        SaveAction save = new SaveAction(Page);
        save.CommandArgument = ComponentEvents.SAVE_DATA;
        save.CommandName = ComponentEvents.SAVE_DATA;
        
        headerActions.ActionsList.Add(save);

        if (pti != null)
        {
            // Disable buttons for no-template
            bool actionsEnabled = (pti.PageTemplateId > 0);

            // Edit layout
            HeaderAction action = new HeaderAction
            {
                Text = GetString("content.ui.pagelayout"),
                Tooltip = GetString("pageplaceholder.editlayouttooltip"),
                OnClientClick = "EditLayout();return false;",
                ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/Edit.png"),
                Enabled = actionsEnabled

            };
            headerActions.ActionsList.Add(action);

            // Edit page properties action
            action = new HeaderAction
            {
                Text = GetString("PageProperties.EditTemplateProperties"),
                Tooltip = GetString("PageProperties.EditTemplateProperties"),
                OnClientClick = "modalDialog('" + ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Edit.aspx") + "?templateid=" + pti.PageTemplateId + "&nobreadcrumbs=1&dialog=1', 'TemplateSelection', 850, 680, false);return false;",
                ImageUrl = GetImageUrl("CMSModules/CMS_Content/Template/edit.png"),
                 Enabled = actionsEnabled
            };

            CMSPagePlaceholder.RegisterEditLayoutScript(this, pti.PageTemplateId, node.NodeAliasPath, null);
            headerActions.ActionsList.Add(action);

            // Preview
            HeaderAction preview = new HeaderAction
                                       {
                                           ControlType = HeaderActionTypeEnum.LinkButton,
                                           Text = GetString("general.preview"),
                                           OnClientClick = "performToolbarAction('split');return false;",
                                           ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/Preview.png"),
                                           SmallImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/16/Preview.png"),
                                           Visible = ((previewState == 0) && !CMSContext.DisplaySplitMode),
                                           Tooltip = GetString("preview.tooltip")
                                       };
            headerActions.ActionsList.Add(preview);

            headerActions.ActionPerformed += new CommandEventHandler(headerActions_ActionPerformed);
        }

        RegisterInitScripts(pnlBody.ClientID, pnlMenu.ClientID, false);
    }


    /// <summary>
    /// Action performed
    /// </summary>
    void headerActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName == ComponentEvents.SAVE_DATA)
        {
            SaveData();
        }
    }


    public void LoadData()
    {
        if (node != null)
        {
            string layoutText = String.Empty;

            // get template info
            pti = PageTemplateInfoProvider.GetPageTemplateInfo(node.GetUsedPageTemplateId());
            if ((pti == null) && (CMSContext.CurrentPageInfo != null) && (CMSContext.CurrentPageInfo.UsedPageTemplateInfo != null))
            {
                pti = CMSContext.CurrentPageInfo.UsedPageTemplateInfo;
            }

            if (pti != null)
            {
                PageTemplateLayoutTypeEnum type = PageTemplateLayoutTypeEnum.PageTemplateLayout;
                // Try get device layout
                object layoutObject = PageTemplateDeviceLayoutInfoProvider.GetLayoutObject(pti, CMSContext.CurrentDeviceProfile, out type);
                layoutText = pti.PageTemplateLayout;

                // Set layout text with dependence on current layout type
                switch (type)
                {
                    // Shared layouts
                    case PageTemplateLayoutTypeEnum.SharedLayout:
                    case PageTemplateLayoutTypeEnum.DeviceSharedLayout:
                    case PageTemplateLayoutTypeEnum.SharedLayoutMapped:
                        layoutText = (layoutObject as LayoutInfo).LayoutCode;
                        break;

                    // Custom device layout
                    case PageTemplateLayoutTypeEnum.DeviceLayout:
                        layoutText = (layoutObject as PageTemplateDeviceLayoutInfo).LayoutCode;
                        break;
                }
            }
            ltlLayoutCode.Text = HTMLHelper.EnsureLineEnding(HTMLHelper.HighlightHTML(layoutText), "<br />");
            ltlLayoutCode.Text = RegexHelper.GetRegex("[ ](?![^<>]*>)").Replace(ltlLayoutCode.Text, "&nbsp;");

            // Load node data
            if (!RequestHelper.IsPostBack())
            {
                txtBodyCss.Text = node.NodeBodyElementAttributes;
                txtDocType.Text = node.NodeDocType;
                txtHeadTags.Value = node.NodeHeadTags;
            }
        }

        lblAfterDocType.Text = HighlightHTML("<html>") + "<br />" + AddSpaces(1) + HighlightHTML("<head>");
        lblAfterHeadTags.Text = AddSpaces(1) + HighlightHTML("</head>");
        lblAfterLayout.Text = AddSpaces(1) + HighlightHTML("</body>") + "<br />" + HighlightHTML("</html>");
        lblBodyEnd.Text = HighlightHTML(">");
        lblBodyStart.Text = AddSpaces(1) + HighlightHTML("<body " + HttpUtility.HtmlDecode(mBody));
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Check whether virtual objects are allowed
        if (!VirtualPathHelper.VirtualObjectsAllowed)
        {
            ShowWarning(GetString("VirtualPathProvider.NotRunning"), null, null);
        }

        base.OnPreRender(e);
    }

    /// <summary>
    /// Format HTML text.
    /// </summary>
    /// <param name="inputHTML">Input HTML</param>
    /// <param name="level">Indentation level</param>
    public string FormatHTML(string inputHTML, int level)
    {
        return AddSpaces(level) + inputHTML.Replace(HttpUtility.HtmlEncode(">"), HttpUtility.HtmlEncode(">") + "<br />" + AddSpaces(level));
    }


    /// <summary>
    /// Add spaces.
    /// </summary>
    /// <param name="level">Indentation level</param>
    public string AddSpaces(int level)
    {
        string toReturn = "";
        for (int i = 0; i < level * 2; i++)
        {
            toReturn += "&nbsp;";
        }

        return toReturn;
    }


    /// <summary>
    /// Highlight HTML.
    /// </summary>
    /// <param name="inputHtml">Input HTML</param>
    public string HighlightHTML(string inputHtml)
    {
        return HTMLHelper.HighlightHTML(inputHtml);
    }


    private void SaveData()
    {
        if (node != null)
        {
            // Update fields
            node.NodeBodyElementAttributes = txtBodyCss.Text;
            node.NodeDocType = txtDocType.Text;
            node.NodeHeadTags = txtHeadTags.Value.ToString();

            // Update the node
            node.Update();

            // Update search index
            if ((node.PublishedVersionExists) && (SearchIndexInfoProvider.SearchEnabled))
            {
                SearchTaskInfoProvider.CreateTask(SearchTaskTypeEnum.Update, PredefinedObjectType.DOCUMENT, SearchHelper.ID_FIELD, node.GetSearchID());
            }

            // Log synchronization
            DocumentSynchronizationHelper.LogDocumentChange(node, TaskTypeEnum.UpdateDocument, tree);

            RegisterRefreshScript();

            // Empty variable for exitwithoutchanges dialog
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "SubmitAction", ScriptHelper.GetScript("NotChanged();"));

            // Clear cache
            PageInfoProvider.RemoveAllPageInfosFromCache();

            

           ShowChangesSaved();
        }
    }

    #endregion

}
