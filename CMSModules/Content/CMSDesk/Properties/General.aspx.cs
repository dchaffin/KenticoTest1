using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.PortalControls;

using TimeZoneInfo = CMS.SiteProvider.TimeZoneInfo;
using TreeNode = CMS.DocumentEngine.TreeNode;

[RegisterTitle("content.ui.propertiesgeneral")]
public partial class CMSModules_Content_CMSDesk_Properties_General : CMSPropertiesPage
{
    #region "Variables"

    protected string mForums = null;
    protected string mMessageBoards = null;
    protected string mEditableContent = null;
    protected string mWireframe = null;

    protected bool canEditOwner = false;
    protected bool canEdit = true;
    protected bool clearCache = false;

    private bool hasAdHocBoard = false;
    private bool hasAdHocForum = false;

    protected FormEngineUserControl usrOwner = null;
    protected FormEngineUserControl fcDocumentGroupSelector = null;

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        // Culture independent data
        SplitModeAllwaysRefresh = true;

        // Non-versioned data are modified
        DocumentManager.UseDocumentHelper = false;

        base.OnInit(e);

        // Check UI element permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.General"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Properties.General");
        }

        // Redirect to information page when no UI elements displayed
        if (pnlUIAdvanced.IsHidden && pnlUICache.IsHidden && pnlUIDesign.IsHidden &&
            pnlUIOther.IsHidden && pnlUIOwner.IsHidden)
        {
            RedirectToUINotAvailable();
        }

        // Init document manager events
        DocumentManager.OnSaveData += DocumentManager_OnSaveData;
        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;

        EnableSplitMode = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        UIContext.PropertyTab = PropertyTabEnum.General;

        // Register the scripts
        ScriptHelper.RegisterProgress(Page);
        ScriptHelper.RegisterTooltip(Page);
        ScriptHelper.RegisterDialogScript(this);

        // Set user control properties
        usrOwner = Page.LoadUserControl("~/CMSModules/Membership/FormControls/Users/selectuser.ascx") as FormEngineUserControl;
        if (usrOwner != null)
        {
            usrOwner.ID = "ctrlUsrOwner";
            usrOwner.IsLiveSite = false;
            usrOwner.SetValue("ShowSiteFilter", false);
            usrOwner.StopProcessing = pnlUIOwner.IsHidden;
            plcUsrOwner.Controls.Add(usrOwner);
        }

        // Init strings
        pnlDesign.GroupingText = GetString("GeneralProperties.DesignGroup");
        pnlCache.GroupingText = GetString("GeneralProperties.CacheGroup");
        pnlOther.GroupingText = GetString("GeneralProperties.OtherGroup");
        pnlAdvanced.GroupingText = GetString("GeneralProperties.AdvancedGroup");
        pnlOwner.GroupingText = GetString("GeneralProperties.OwnerGroup");

        // Advanced section
        mEditableContent = GetString("GeneralProperties.EditableContent");
        mForums = GetString("PageProperties.AdHocForum");
        mMessageBoards = GetString("PageProperties.MessageBoards");

        lnkEditableContent.OnClientClick = "ShowEditableContent(); return false;";
        lnkMessageBoards.OnClientClick = "ShowMessageBoards(); return false;";
        lnkForums.OnClientClick = "ShowForums(); return false;";

        imgEditableContent.ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditableContent/editablecontent.png");
        imgMessageBoards.ImageUrl = GetImageUrl("CMSModules/CMS_MessageBoards/module.png");
        imgForums.ImageUrl = GetImageUrl("CMSModules/CMS_Forums/module.png");

        // Get strings for radio buttons
        lblCacheMinutes.Text = GetString("GeneralProperties.cacheMinutes");

        // Get strings for labels
        lblNameTitle.Text = GetString("GeneralProperties.Name");
        lblNamePathTitle.Text = GetString("GeneralProperties.NamePath");
        lblAliasPathTitle.Text = GetString("GeneralProperties.AliasPath");
        lblTypeTitle.Text = GetString("GeneralProperties.Type");
        lblNodeIDTitle.Text = GetString("GeneralProperties.NodeID");
        lblLastModifiedByTitle.Text = GetString("GeneralProperties.LastModifiedBy");
        lblLastModifiedTitle.Text = GetString("GeneralProperties.LastModified");
        lblLiveURLTitle.Text = GetString("GeneralProperties.LiveURL");
        lblPreviewURLTitle.Text = GetString("GeneralProperties.PreviewURL");
        lblGUIDTitle.Text = GetString("GeneralProperties.GUID");
        lblDocGUIDTitle.Text = GetString("GeneralProperties.DocumentGUID");
        lblDocIDTitle.Text = GetString("GeneralProperties.DocumentID");
        lblCultureTitle.Text = GetString("GeneralProperties.Culture");
        lblCreatedByTitle.Text = GetString("GeneralProperties.CreatedBy");
        lblCreatedTitle.Text = GetString("GeneralProperties.Created");
        lblOwnerTitle.Text = GetString("GeneralProperties.Owner");
        lblCssStyle.Text = GetString("PageProperties.CssStyle");
        lblPublishedTitle.Text = GetString("PageProperties.Published");

        chkCssStyle.Text = GetString("Metadata.Inherit");
        pnlOnlineMarketing.GroupingText = GetString("general.onlinemarketing");

        // Set default item value
        string defaultStyleSheet = "-1";
        CssStylesheetInfo cssInfo = CMSContext.CurrentSiteStylesheet;

        // If current site default style sheet defined, choose it
        if (cssInfo != null)
        {
            defaultStyleSheet = "default";
        }
        ctrlSiteSelectStyleSheet.CurrentSelector.SpecialFields = new string[,] { { GetString("general.defaultchoice"), defaultStyleSheet } };
        ctrlSiteSelectStyleSheet.ReturnColumnName = "StyleSheetID";
        ctrlSiteSelectStyleSheet.SiteId = CMSContext.CurrentSiteID;

        if ((CMSContext.CurrentSite != null) && (usrOwner != null))
        {
            usrOwner.SetValue("SiteID", CMSContext.CurrentSite.SiteID);
        }

        int documentId = 0;

        string script = null;

        TreeNode node = Node;
        if (node != null)
        {
            // Create wireframe option
            if (node.NodeWireframeTemplateID <= 0)
            {
                mWireframe = GetString("Wireframe.Create");

                string createUrl = URLHelper.ResolveUrl(String.Format("~/CMSModules/Content/CMSDesk/Properties/CreateWireframe.aspx?nodeid={0}&culture={1}", node.NodeID, node.DocumentCulture));
                lnkWireframe.OnClientClick = "parent.location.replace('" + createUrl + "'); return false;";

                imgWireframe.ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/createwireframe.png");
            }
            else
            {
                mWireframe = GetString("Wireframe.Remove");
                imgWireframe.ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/removewireframe.png");

                lnkWireframe.OnClientClick = "return confirm('" + GetString("Wireframe.ConfirmRemove") + "')";
                lnkWireframe.Click += new EventHandler(lnkWireframe_Click);
            }

            plcWireframe.Visible = PortalHelper.IsWireframingEnabled(CMSContext.CurrentSiteName);

            documentId = node.DocumentID;
            canEditOwner = (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.ModifyPermissions) == AuthorizationResultEnum.Allowed);
            ctrlSiteSelectStyleSheet.AliasPath = node.NodeAliasPath;

            ReloadData();

            // Check ad-hoc forum counts
            hasAdHocForum = (ModuleCommands.ForumsGetDocumentForumsCount(node.DocumentID) > 0);

            // Ad-Hoc message boards check
            hasAdHocBoard = (ModuleCommands.MessageBoardGetDocumentBoardsCount(node.DocumentID) > 0);

            script += "function ShowEditableContent() { modalDialog('" + ResolveUrl("Advanced/EditableContent/default.aspx") + "?nodeid=" + node.NodeID + "', 'EditableContent', 1015, 700); } \n";
        }

        // Generate executive script
        if (hasAdHocBoard)
        {
            plcAdHocBoards.Visible = true;
            script += "function ShowMessageBoards() { modalDialog('" + ResolveUrl("~/CMSModules/MessageBoards/Content/Properties/default.aspx") + "?documentid=" + documentId + "', 'MessageBoards', 1020, 680); } \n";
        }

        if (hasAdHocForum)
        {
            plcAdHocForums.Visible = true;
            script += "function ShowForums() { modalDialog('" + ResolveUrl("~/CMSModules/Forums/Content/Properties/default.aspx") + "?documentid=" + documentId + "', 'Forums', 1130, 680); } \n";
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ModalDialogsToAdvancedSection", ScriptHelper.GetScript(script));

        // Reflect processing action
        pnlContent.Enabled = DocumentManager.AllowSave;

        if (chkCssStyle.Checked)
        {
            // Enable the edit button
            ctrlSiteSelectStyleSheet.ButtonEditEnabled = true;
        }
    }


    protected void lnkWireframe_Click(object sender, EventArgs e)
    {
        // Remove the wireframe
        TreeNode node = DocumentManager.Node;

        DocumentManager.RemoveWireframe();

        ScriptHelper.RegisterStartupScript(this, typeof(string), "Refresh", ScriptHelper.GetScript(String.Format(
            "parent.RefreshTree({0}, {0}); parent.SelectNode({0});",
            node.NodeID
        )));
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (RequestHelper.IsPostBack())
        {
            ReloadData();
        }
        chkPageVisitInherit.Visible = (Node != null) && (Node.NodeParentID > 0);
    }


    protected void chkPageVisitInherit_CheckedChanged(object sender, EventArgs e)
    {
        chkLogPageVisit.Enabled = !chkPageVisitInherit.Checked;
        if (chkPageVisitInherit.Checked && (Node != null))
        {
            string siteName = CMSContext.CurrentSiteName;
            if (!String.IsNullOrEmpty(siteName))
            {
                chkLogPageVisit.Checked = ValidationHelper.GetBoolean(Node.GetInheritedValue("DocumentLogVisitActivity", SiteInfoProvider.CombineWithDefaultCulture(siteName)), false);
            }
        }
    }

    #endregion


    #region "Private methods"

    private void ReloadData()
    {
        if (Node != null)
        {
            // Log activities checkboxes
            if (!RequestHelper.IsPostBack())
            {
                bool? logVisit = Node.DocumentLogVisitActivity;
                chkLogPageVisit.Checked = (logVisit == true);
                if (Node.NodeParentID > 0)  // Init "inherit" option for child nodes (and hide option for root)
                {
                    chkPageVisitInherit.Checked = (logVisit == null);
                    if (logVisit == null)
                    {
                        chkPageVisitInherit_CheckedChanged(null, EventArgs.Empty);
                    }
                }
                chkLogPageVisit.Enabled = !chkPageVisitInherit.Checked;
            }

            // Check modify permission
            canEdit = (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) != AuthorizationResultEnum.Denied);

            // Show document group owner selector
            if (ModuleEntry.IsModuleLoaded(ModuleEntry.COMMUNITY) && canEditOwner && LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.Groups))
            {
                plcOwnerGroup.Controls.Clear();
                // Initialize table
                TableRow rowOwner = new TableRow();
                TableCell cellTitle = new TableCell();
                TableCell cellSelector = new TableCell();

                // Initialize caption
                LocalizedLabel lblOwnerGroup = new LocalizedLabel();
                lblOwnerGroup.EnableViewState = false;
                lblOwnerGroup.ResourceString = "community.group.documentowner";
                lblOwnerGroup.ID = "lblOwnerGroup";
                cellTitle.Controls.Add(lblOwnerGroup);

                // Initialize selector
                fcDocumentGroupSelector = (FormEngineUserControl)Page.LoadUserControl("~/CMSAdminControls/UI/Selectors/DocumentGroupSelector.ascx");
                fcDocumentGroupSelector.ID = "fcDocumentGroupSelector";
                fcDocumentGroupSelector.StopProcessing = pnlUIOwner.IsHidden;
                cellSelector.Controls.Add(fcDocumentGroupSelector);
                fcDocumentGroupSelector.Value = ValidationHelper.GetInteger(Node.GetValue("NodeGroupID"), 0);
                fcDocumentGroupSelector.SetValue("siteid", CMSContext.CurrentSiteID);
                fcDocumentGroupSelector.SetValue("nodeid", Node.NodeID);

                // Add controls to containers
                rowOwner.Cells.Add(cellTitle);
                rowOwner.Cells.Add(cellSelector);
                plcOwnerGroup.Controls.Add(rowOwner);
                plcOwnerGroup.Visible = true;
            }

            // Show owner editing only when authorized to change the permissions
            if (canEditOwner)
            {
                lblOwner.Visible = false;
                usrOwner.Visible = true;
                usrOwner.SetValue("AdditionalUsers", new int[] { Node.NodeOwner });
            }
            else
            {
                usrOwner.Visible = false;
            }

            if (!RequestHelper.IsPostBack())
            {
                if (canEditOwner)
                {
                    usrOwner.Value = Node.GetValue("NodeOwner");
                }
            }

            // Load the data
            lblName.Text = HttpUtility.HtmlEncode(Node.GetDocumentName());
            lblNamePath.Text = HttpUtility.HtmlEncode(Convert.ToString(Node.GetValue("DocumentNamePath")));
            lblAliasPath.Text = Convert.ToString(Node.NodeAliasPath);
            string typeName = DataClassInfoProvider.GetDataClass(Node.NodeClassName).ClassDisplayName;
            lblType.Text = HttpUtility.HtmlEncode(ResHelper.LocalizeString(typeName));
            lblNodeID.Text = Convert.ToString(Node.NodeID);

            // Modifier
            SetUserLabel(lblLastModifiedBy, "DocumentModifiedByUserId");

            // Get modified time
            TimeZoneInfo usedTimeZone = null;
            DateTime lastModified = ValidationHelper.GetDateTime(Node.GetValue("DocumentModifiedWhen"), DateTimeHelper.ZERO_TIME);
            lblLastModified.Text = TimeZoneHelper.GetCurrentTimeZoneDateTimeString(lastModified, CMSContext.CurrentUser, CMSContext.CurrentSite, out usedTimeZone);
            ScriptHelper.AppendTooltip(lblLastModified, TimeZoneHelper.GetGMTLongStringOffset(usedTimeZone), "help");

            if (!canEditOwner)
            {
                // Owner
                SetUserLabel(lblOwner, "NodeOwner");
            }

            // Creator
            SetUserLabel(lblCreatedBy, "DocumentCreatedByUserId");
            DateTime createdWhen = ValidationHelper.GetDateTime(Node.GetValue("DocumentCreatedWhen"), DateTimeHelper.ZERO_TIME);
            lblCreated.Text = TimeZoneHelper.GetCurrentTimeZoneDateTimeString(createdWhen, CMSContext.CurrentUser, CMSContext.CurrentSite, out usedTimeZone);
            ScriptHelper.AppendTooltip(lblCreated, TimeZoneHelper.GetGMTLongStringOffset(usedTimeZone), "help");


            // URL
            string liveUrl = Node.IsLink ? CMSContext.GetUrl(Node.NodeAliasPath, null) : CMSContext.GetUrl(Node.NodeAliasPath, Node.DocumentUrlPath);
            lnkLiveURL.Text = URLHelper.ResolveUrl(liveUrl, true, false);
            lnkLiveURL.NavigateUrl = URLHelper.ResolveUrl(liveUrl);

            bool isRoot = (Node.NodeClassName.ToLowerCSafe() == "cms.root");

            // Preview URL
            if (!isRoot)
            {
                plcPreview.Visible = true;
                string path = canEdit ? "/CMSModules/CMS_Content/Properties/resetlink.png" : "/CMSModules/CMS_Content/Properties/resetlinkdisabled.png";
                btnResetPreviewGuid.ImageUrl = GetImageUrl(path);
                btnResetPreviewGuid.ToolTip = GetString("GeneralProperties.InvalidatePreviewURL");
                btnResetPreviewGuid.ImageAlign = ImageAlign.AbsBottom;
                btnResetPreviewGuid.Click += btnResetPreviewGuid_Click;
                btnResetPreviewGuid.OnClientClick = "if(!confirm('" + GetString("GeneralProperties.GeneratePreviewURLConf") + "')){return false;}";

                InitPreviewUrl();
            }

            lblGUID.Text = Convert.ToString(Node.NodeGUID);
            lblDocGUID.Text = (Node.DocumentGUID == Guid.Empty) ? ResHelper.Dash : Node.DocumentGUID.ToString();
            lblDocID.Text = Convert.ToString(Node.DocumentID);

            // Culture
            CultureInfo ci = CultureInfoProvider.GetCultureInfo(Node.DocumentCulture);
            lblCulture.Text = ((ci != null) ? ResHelper.LocalizeString(ci.CultureName) : Node.DocumentCulture);

            lblPublished.Text = (Node.IsPublished ? "<span class=\"DocumentPublishedYes\">" + GetString("General.Yes") + "</span>" : "<span class=\"DocumentPublishedNo\">" + GetString("General.No") + "</span>");

            if (!RequestHelper.IsPostBack())
            {
                // Init radio buttons for cache settings
                if (isRoot)
                {
                    radInherit.Visible = false;
                    radFSInherit.Visible = false;
                    chkCssStyle.Visible = false;
                }

                string cacheMinutes = "";

                switch (Node.NodeCacheMinutes)
                {
                    case -1:
                        // Cache is off
                        {
                            radNo.Checked = true;
                            radYes.Checked = false;
                            radInherit.Checked = false;
                            if (!isRoot)
                            {
                                radInherit.Checked = true;
                                radNo.Checked = false;
                            }
                        }
                        break;

                    case 0:
                        // Cache is off
                        radNo.Checked = true;
                        radYes.Checked = false;
                        radInherit.Checked = false;
                        break;

                    default:
                        // Cache is enabled
                        radNo.Checked = false;
                        radYes.Checked = true;
                        radInherit.Checked = false;
                        cacheMinutes = Node.NodeCacheMinutes.ToString();
                        break;
                }

                // Set secured radio buttons
                switch (Node.NodeAllowCacheInFileSystem)
                {
                    case 0:
                        radFSNo.Checked = true;
                        break;

                    case 1:
                        radFSYes.Checked = true;
                        break;

                    default:
                        if (!isRoot)
                        {
                            radFSInherit.Checked = true;
                        }
                        else
                        {
                            radFSYes.Checked = true;
                        }
                        break;
                }

                txtCacheMinutes.Text = cacheMinutes;

                if (!radYes.Checked)
                {
                    txtCacheMinutes.Enabled = false;
                }

                if (Node.GetValue("DocumentStylesheetID") == null)
                {
                    // If default site not exist edit is set to -1 - disabled
                    if (CMSContext.CurrentSiteStylesheet != null)
                    {
                        ctrlSiteSelectStyleSheet.Value = "default";
                    }
                    else
                    {
                        ctrlSiteSelectStyleSheet.Value = -1;
                    }
                }
                else
                {
                    // If stylesheet is inherited from parent document
                    if (ValidationHelper.GetInteger(Node.GetValue("DocumentStylesheetID"), 0) == -1)
                    {
                        if (!isRoot)
                        {
                            chkCssStyle.Checked = true;

                            // Get parent stylesheet
                            string value = PageInfoProvider.GetParentProperty(CMSContext.CurrentSite.SiteID, Node.NodeAliasPath, "(DocumentStylesheetID <> -1 OR DocumentStylesheetID IS NULL) AND DocumentCulture = N'" + SqlHelperClass.GetSafeQueryString(Node.DocumentCulture, false) + "'", "DocumentStylesheetID");

                            if (String.IsNullOrEmpty(value))
                            {
                                // If default site stylesheet not exist edit is set to -1 - disabled
                                if (CMSContext.CurrentSiteStylesheet != null)
                                {
                                    ctrlSiteSelectStyleSheet.Value = "default";
                                }
                                else
                                {
                                    ctrlSiteSelectStyleSheet.Value = -1;
                                }
                            }
                            else
                            {
                                // Set parent stylesheet to current document
                                ctrlSiteSelectStyleSheet.Value = value;
                            }
                        }
                    }
                    else
                    {
                        ctrlSiteSelectStyleSheet.Value = Node.GetValue("DocumentStylesheetID");
                    }
                }
            }

            // Disable new button if document inherit stylesheet
            bool disableCssSelector = (!isRoot && chkCssStyle.Checked);
            ctrlSiteSelectStyleSheet.Enabled = !disableCssSelector;
            ctrlSiteSelectStyleSheet.ButtonNewEnabled = !disableCssSelector;

            // Initialize Rating control
            RefreshCntRatingResult();

            double rating = 0.0f;
            if (Node.DocumentRatings > 0)
            {
                rating = Node.DocumentRatingValue / Node.DocumentRatings;
            }
            ratingControl.MaxRating = 10;
            ratingControl.CurrentRating = rating;
            ratingControl.Visible = true;
            ratingControl.Enabled = false;

            // Initialize Reset button for rating
            btnResetRating.Text = GetString("general.reset");
            btnResetRating.OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("GeneralProperties.ResetRatingConfirmation")) + ")) return false;";

            object[] param = new object[1];
            param[0] = Node.DocumentID;

            plcAdHocForums.Visible = hasAdHocForum;
            plcAdHocBoards.Visible = hasAdHocBoard;

            if (!canEdit)
            {
                // Disable form editing                                                            
                DisableFormEditing();
            }
        }
        else
        {
            btnResetRating.Visible = false;
        }
    }


    /// <summary>
    /// Initializes the label with specified user text.
    /// </summary>
    private void SetUserLabel(Label label, string columnName)
    {
        // Get the user ID
        int userId = ValidationHelper.GetInteger(Node.GetValue(columnName), 0);
        if (userId > 0)
        {
            // Get the user object
            UserInfo ui = null;
            string key = "user_" + userId;
            object userObject = RequestStockHelper.GetItem(key);
            if (userObject != null)
            {
                ui = (UserInfo)userObject;
            }
            else
            {
                // Get user object from database
                ui = UserInfoProvider.GetUserInfo(userId);
                RequestStockHelper.Add(key, ui);
            }

            if (ui != null)
            {
                label.Text = HTMLHelper.HTMLEncode(ui.FullName);
            }
        }
        else
        {
            label.Text = GetString("general.selectnone");
        }
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check modify permissions
            if (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                return;
            }

            // Clear the output cache with the children
            Node.ClearOutputCache(true, true);

            ShowConfirmation(GetString("GeneralProperties.CacheCleared"));
        }
    }


    private void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        // Clear cache if cache settings changed
        if (clearCache)
        {
            Node.ClearOutputCache(true, true);
        }
    }


    private void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        TreeNode node = e.Node;

        // OWNER group is displayed by UI profile
        if (!pnlUIOwner.IsHidden)
        {
            // Set owner
            int ownerId = ValidationHelper.GetInteger(usrOwner.Value, 0);
            node.SetValue("NodeOwner", (ownerId > 0) ? usrOwner.Value : null);
        }


        // DESIGN group is displayed by UI profile
        if (!pnlUIDesign.IsHidden)
        {
            node.SetValue("DocumentStylesheetID", -1);
            if (!chkCssStyle.Checked)
            {
                // Set style sheet
                int selectedCssId = ValidationHelper.GetInteger(ctrlSiteSelectStyleSheet.Value, 0);
                if (selectedCssId < 1)
                {
                    node.SetValue("DocumentStylesheetID", null);
                }
                else
                {
                    node.SetValue("DocumentStylesheetID", selectedCssId);
                }

                ctrlSiteSelectStyleSheet.CurrentDropDown.Enabled = true;
            }
            else
            {
                ctrlSiteSelectStyleSheet.CurrentDropDown.Enabled = false;
            }
        }

        // CACHE group is displayed by UI profile
        if (!pnlUICache.IsHidden)
        {
            // Cache minutes
            int cacheMinutes = 0;
            if (radNo.Checked)
            {
                cacheMinutes = 0;
                txtCacheMinutes.Text = "";
            }
            else if (radYes.Checked)
            {
                cacheMinutes = ValidationHelper.GetInteger(txtCacheMinutes.Text, -5);
                if (cacheMinutes <= 0)
                {
                    e.IsValid = false;
                }
            }
            else if (radInherit.Checked)
            {
                cacheMinutes = -1;
                txtCacheMinutes.Text = "";
            }

            // Set cache minutes                
            if (cacheMinutes != node.NodeCacheMinutes)
            {
                node.NodeCacheMinutes = cacheMinutes;
                clearCache = true;
            }

            // Allow file system cache
            int allowFs = Node.NodeAllowCacheInFileSystem;

            if (radFSYes.Checked)
            {
                allowFs = 1;
            }
            else if (radFSNo.Checked)
            {
                allowFs = 0;
            }
            else if (radInherit.Checked)
            {
                allowFs = -1;
            }

            Node.NodeAllowCacheInFileSystem = allowFs;
        }

        if (e.IsValid)
        {
            // Check UI element permission
            if (CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "General.OnlineMarketing"))
            { 
                node.DocumentLogVisitActivity = (chkPageVisitInherit.Checked ? (bool?)null : chkLogPageVisit.Checked);
            }
        }
        else
        {
            // Show error message
            e.ErrorMessage = GetString("GeneralProperties.BadCacheMinutes");
        }
    }


    private void btnResetPreviewGuid_Click(object sender, ImageClickEventArgs e)
    {
        if (Node != null)
        {
            // Check modify permissions
            if (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                return;
            }

            Node.DocumentWorkflowCycleGUID = Guid.NewGuid();
            Node.Update();

            ShowConfirmation(ResHelper.GetString("GeneralProperties.PreviewLinkGenerated"));
            InitPreviewUrl();
        }
    }


    /// <summary>
    /// Disables form editing.
    /// </summary>
    protected void DisableFormEditing()
    {
        // Disable all panels
        pnlDesign.Enabled = false;
        pnlCache.Enabled = false;
        pnlOwner.Enabled = false;
        pnlOnlineMarketing.Enabled = false;

        // Disable 'save button'
        menuElem.Enabled = false;

        // Disable rating and owner selector
        btnResetPreviewGuid.Enabled = false;
        btnResetPreviewGuid.CssClass = "Disabled";
        btnResetRating.Enabled = false;
        btnClear.Enabled = false;
        usrOwner.Enabled = false;
        if (fcDocumentGroupSelector != null)
        {
            fcDocumentGroupSelector.Enabled = false;
        }

        ctrlSiteSelectStyleSheet.Enabled = false;
        ctrlSiteSelectStyleSheet.ButtonNewEnabled = false;
    }


    protected void radInherit_CheckedChanged(object sender, EventArgs e)
    {
        txtCacheMinutes.Enabled = false;

        // Enable textbox for cache minutes
        if (radYes.Checked)
        {
            txtCacheMinutes.Enabled = true;
        }
    }


    protected void chkCssStyle_CheckedChanged(object sender, EventArgs e)
    {
        if (chkCssStyle.Checked)
        {
            // Set stylesheet to stylesheet selector
            ctrlSiteSelectStyleSheet.CurrentDropDown.Enabled = false;
            ctrlSiteSelectStyleSheet.ButtonNewEnabled = false;

            string value = PageInfoProvider.GetParentProperty(CMSContext.CurrentSite.SiteID, Node.NodeAliasPath, "(DocumentStylesheetID <> -1 OR DocumentStylesheetID IS NULL) AND DocumentCulture = N'" + SqlHelperClass.GetSafeQueryString(Node.DocumentCulture, false) + "'", "DocumentStylesheetID");
            if (String.IsNullOrEmpty(value))
            {
                // If default site stylesheet not exist edit is set to -1 - disabled
                if (CMSContext.CurrentSiteStylesheet != null)
                {
                    ctrlSiteSelectStyleSheet.CurrentDropDown.SelectedValue = "default";
                }
                else
                {
                    ctrlSiteSelectStyleSheet.CurrentDropDown.SelectedValue = "-1";
                }
            }
            else
            {
                try
                {
                    ctrlSiteSelectStyleSheet.CurrentDropDown.SelectedValue = value;
                }
                catch
                {
                }
            }
        }
        else
        {
            ctrlSiteSelectStyleSheet.CurrentDropDown.Enabled = true;
            ctrlSiteSelectStyleSheet.ButtonNewEnabled = true;
        }
    }


    /// <summary>
    /// Refreshes current rating result.
    /// </summary>
    protected void RefreshCntRatingResult()
    {
        string msg = null;

        // Avoid division by zero
        if ((Node != null) && (Node.DocumentRatings > 0))
        {
            msg = String.Format(GetString("GeneralProperties.ContentRatingResult"), (Node.DocumentRatingValue * 10) / Node.DocumentRatings, Node.DocumentRatings);
        }

        // Document wasn't rated
        if (msg == null)
        {
            msg = GetString("general.na");
        }

        lblContentRatingResult.Text = msg;
    }


    /// <summary>
    /// Resets content rating score.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Args</param>
    protected void btnResetRating_Click(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check modify permissions
            if (CMSContext.CurrentUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                return;
            }

            // Reset rating
            TreeProvider.ResetRating(Node);
            RefreshCntRatingResult();
            ratingControl.CurrentRating = 0.0;
            ratingControl.ReloadData();

            ShowChangesSaved();
        }
    }


    private void InitPreviewUrl()
    {
        if (Node.DocumentWorkflowCycleGUID != Guid.Empty)
        {
            lnkPreviewURL.Visible = true;
            lblNoPreviewGuid.Visible = false;
            lnkPreviewURL.Text = ResHelper.GetString("GeneralProperties.ShowPreview");
            bool isFile = CMSString.Equals(Node.NodeClassName, "cms.file", true);
            lnkPreviewURL.NavigateUrl = Node.GetPreviewLink(CurrentUser.UserName, isFile);
        }
        else
        {
            lnkPreviewURL.Visible = false;
            lblNoPreviewGuid.Visible = true;
            lblNoPreviewGuid.Text = GetString("GeneralProperties.NoPreviewGuid");
        }
    }

    #endregion
}