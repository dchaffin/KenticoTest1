using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

// Set edited object
[EditedObject("newsletter.issue", "issueid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Edit : CMSNewsletterNewslettersPage
{
    #region "Variables"

    private NewsletterInfo newsletter = null;

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Set update mode to ALWAYS
        CurrentMaster.HeaderActions.UpdatePanel.UpdateMode = UpdatePanelUpdateMode.Always;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Reset master page content CSS class
        CurrentMaster.PanelContent.CssClass = string.Empty;

        // Get edited issue object and check its existence
        IssueInfo issue = (IssueInfo)EditedObject;
        EditedObject = issue;

        // Get newsletter and check its existence
        newsletter = NewsletterInfoProvider.GetNewsletterInfo(issue.IssueNewsletterID);
        if ((newsletter != null) && (newsletter.NewsletterType == NewsletterType.Dynamic))
        {
            ShowError(GetString("Newsletter_Issue_Edit.CannotBeEdited"));
            editElem.StopProcessing = true;
            editElem.Visible = false;
            return;
        }

        // Get variant issue ID if A/B testing is ON
        int issueId = InitVariantSlider(issue.IssueID);

        // Initialize edit control
        editElem.IssueID = issueId;
        editElem.NewsletterID = issue.IssueNewsletterID;
        editElem.IsDialogMode = false;

        InitHeaderActions(issueId);
    }


    /// <summary>
    /// Updates user friendly info message according to current issue.
    /// </summary>
    /// <param name="issueId">Issue ID</param>
    /// <param name="editingIssueEnabled">TRUE if editing issue is allowed</param>
    /// <param name="variantSliderEnabled">TRUE if modifying variants is allowed</param>
    private void UpdateDialog(int issueId, out bool editingIssueEnabled, out bool variantSliderEnabled)
    {
        editingIssueEnabled = variantSliderEnabled = false;

        // Get issue
        IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueId);

        if (issue != null)
        {
            switch (issue.IssueStatus)
            {
                case IssueStatusEnum.Idle:
                case IssueStatusEnum.ReadyForSending:
                    bool isSent = (issue.IssueMailoutTime != DateTimeHelper.ZERO_TIME) && (issue.IssueMailoutTime < DateTime.Now);
                    editingIssueEnabled = true;
                    // Enable variant slider only if parent issue is idle
                    if (issue.IssueIsABTest)
                    {
                        if (issue.IssueIsVariant)
                        {
                            IssueInfo parentIssue = IssueInfoProvider.GetIssueInfo(issue.IssueVariantOfIssueID);
                            variantSliderEnabled = (parentIssue != null) && (parentIssue.IssueStatus == IssueStatusEnum.Idle);
                        }
                    }
                    break;
                case IssueStatusEnum.PreparingData:
                case IssueStatusEnum.TestPhase:
                case IssueStatusEnum.Sending:
                    variantSliderEnabled = editingIssueEnabled = false;
                    break;
                case IssueStatusEnum.Finished:
                    variantSliderEnabled = editingIssueEnabled = false;
                    if (!issue.IssueIsABTest && (newsletter != null))
                    {
                        editingIssueEnabled = newsletter.NewsletterEnableResending;
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    /// <param name="issueId">Issue ID</param>
    private void InitHeaderActions(int issueId)
    {
        // Show info message and get current issue state
        bool editingEnabled = false;
        bool variantSliderEnabled = false;
        UpdateDialog(issueId, out editingEnabled, out variantSliderEnabled);
        editElem.Enabled = editingEnabled;

        bool isAuthorized = CurrentUser.IsAuthorizedPerResource("CMS.Newsletter", "AuthorIssues");
        bool isIssueVariant = ucVariantSlider.Variants.Count > 0;
        string script = null;

        ucVariantSlider.Visible = isIssueVariant;
        ucVariantSlider.EditingEnabled = editingEnabled && variantSliderEnabled;

        ScriptHelper.RegisterDialogScript(Page);

        CurrentMaster.HeaderActions.ActionsList.Clear();

        // Init save button
        CurrentMaster.HeaderActions.ActionsList.Add(new SaveAction(this)
        {
            OnClientClick = "if (GetContent != null) {return GetContent();} else {return false;}",
            SmallImageUrl = isAuthorized && editingEnabled ? GetImageUrl("CMSModules/CMS_Content/EditMenu/16/save.png") : GetImageUrl("CMSModules/CMS_Content/EditMenu/16/savedisabled.png"),
            Enabled = isAuthorized && editingEnabled
        });

        // Ensure spell check action
        if (isAuthorized && editingEnabled)
        {
            CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction()
            {
                CssClass = "MenuItemEdit",
                Text = GetString("EditMenu.IconSpellCheck"),
                Tooltip = GetString("EditMenu.SpellCheck"),
                OnClientClick = "var frame = GetFrame(); if ((frame != null) && (frame.contentWindow.SpellCheck_" + ClientID + " != null)) {frame.contentWindow.SpellCheck_" + ClientID + "();} return false;",
                ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/spellcheck.png"),
                SmallImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/16/spellcheck.png"),
            });
        }

        // Init send draft button
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.LinkButton,
            Text = GetString("newsletterissue_content.senddraft"),
            Tooltip = GetString("newsletterissue_content.senddraft"),
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}?issueid={1}', 'SendDraft', '500', '300');}}", ResolveUrl(@"~/CMSModules/Newsletters/Tools/Newsletters/Newsletter_Issue_SendDraft.aspx"), issueId) + " return false;",
            ImageUrl = GetImageUrl("CMSModules/CMS_Newsletter/senddraft.png"),
            Enabled = isAuthorized
        });

        // Init preview button
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.Hyperlink,
            Text = GetString("general.preview"),
            Tooltip = GetString("general.preview"),
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}?issueid={1}', 'Preview', '90%', '90%');}}", ResolveUrl(@"~/CMSModules/Newsletters/Tools/Newsletters/Newsletter_Issue_Preview.aspx"), issueId) + " return false;",
            ImageUrl = GetImageUrl("CMSModules/CMS_Newsletter/preview.png")
        });

        int attachCount = 0;
        if (isAuthorized)
        {
            // Get number of attachments
            InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(issueId,
                (isIssueVariant ? NewsletterObjectType.NEWSLETTERISSUEVARIANT : NewsletterObjectType.NEWSLETTERISSUE), MetaFileInfoProvider.OBJECT_CATEGORY_ISSUE, null, null, "MetafileID", -1);
            attachCount = ds.Items.Count;

            script = @"
function UpdateAttachmentCount(count) {
    var counter = document.getElementById('attachmentCount');
    if (counter != null) {
        if (count > 0) { counter.innerHTML = ' (' + count + ')'; }
        else { counter.innerHTML = ''; }
    }
}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UpdateAttachmentScript_" + ClientID, script, true);
        }

        // Prepare metafile dialog URL
        string metaFileDialogUrl = ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
        string query = string.Format("?objectid={0}&objecttype={1}", issueId, (isIssueVariant? NewsletterObjectType.NEWSLETTERISSUEVARIANT : NewsletterObjectType.NEWSLETTERISSUE));
        metaFileDialogUrl += string.Format("{0}&category={1}&hash={2}", query, MetaFileInfoProvider.OBJECT_CATEGORY_ISSUE, QueryHelper.GetHash(query));

        // Init attachment button
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.LinkButton,
            Text = GetString("general.attachments") + string.Format("<span id='attachmentCount'>{0}</span>", (attachCount > 0) ? " (" + attachCount.ToString() + ")" : string.Empty),
            Tooltip = GetString("general.attachments"),
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}', 'Attachments', '700', '500');}}", metaFileDialogUrl) + " return false;",
            ImageUrl = GetImageUrl("Objects/CMS_MetaFile/attachment.png"),
            Enabled = isAuthorized
        });

        // Init create A/B test button - online marketing, open email and click through tracking are required
        if (!isIssueVariant)
        {
            IssueInfo issue = (IssueInfo)EditedObject;
            if (editingEnabled && (issue.IssueStatus == IssueStatusEnum.Idle) && NewsletterHelper.IsABTestingAvailable())
            {
                // Check that trackings are enabled
                bool trackingsEnabled = (newsletter != null) && newsletter.NewsletterTrackOpenEmails && newsletter.NewsletterTrackClickedLinks;

                CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction()
                    {
                        ControlType = HeaderActionTypeEnum.LinkButton,
                        Text = GetString("newsletterissue_content.createabtest"),
                        Tooltip = trackingsEnabled ? GetString("newsletterissue_content.createabtest") : GetString("newsletterissue_content.abtesttooltip"),
                        OnClientClick = isAuthorized && trackingsEnabled ? "ShowVariantDialog_" + ucVariantDialog.ClientID + "('addvariant', ''); return false;" : "return false;",
                        ImageUrl = isAuthorized && trackingsEnabled ? GetImageUrl("CMSModules/CMS_Newsletter/abtest.png") : GetImageUrl("CMSModules/CMS_Newsletter/abtest_disabled.png"),
                        Enabled = isAuthorized && trackingsEnabled
                    });
                ucVariantDialog.IssueID = issueId;
                ucVariantDialog.OnAddVariant += new EventHandler(ucVariantSlider_OnVariantAdded);
            }
        }

        // Init fullscreen button
        string imageOff = ResolveUrl(GetImageUrl("CMSModules/CMS_Newsletter/fullscreenoff.png"));
        string imageOn = ResolveUrl(GetImageUrl("CMSModules/CMS_Newsletter/fullscreenon.png"));

        // Create fullscreen toogle function
        script = string.Format(@"
function ToogleFullScreen(elem) {{
 if (window.maximized) {{
     window.maximized = false;
     $j(elem).find('img').attr('src','{0}');
     MaximizeAll(top.window);
 }} else {{
     window.maximized = true;
     $j(elem).find('img').attr('src','{1}');
     MinimizeAll(top.window);
 }}
}}", imageOff, imageOn);

        // Register fullscreen toogle function and necessary scripts
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ToogleFullScreen_" + ClientID, script, true);
        ScriptHelper.RegisterResizer(this);
        ScriptHelper.RegisterJQuery(this);
        
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.LinkButton,
            Text = GetString("general.fullscreen"),
            OnClientClick = "ToogleFullScreen(this);return false;",
            ImageUrl = GetImageUrl("CMSModules/CMS_Newsletter/fullscreenoff.png"),
            CssClass = !isIssueVariant ? "MenuItemEdit Right" : "MenuItemEdit RightABVariant"
        });

        // Init masterpage
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        CurrentMaster.DisplayControlsPanel = isIssueVariant;
        CurrentMaster.PanelContent.Attributes.Add("onmouseout", "if (RememberFocusedRegion) {RememberFocusedRegion();}");
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                // Check permission
                if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Newsletter", "AuthorIssues"))
                {
                    RedirectToCMSDeskAccessDenied("CMS.Newsletter", "AuthorIssues");
                }

                // Check issue status before saving (saving variant issue is allowed only in during "idle" and "ready for sending")
                IssueInfo issue = IssueInfoProvider.GetIssueInfo(editElem.IssueID);
                if ((issue != null) && issue.IssueIsABTest) 
                {
                    if ((issue.IssueStatus != IssueStatusEnum.Idle) && (issue.IssueStatus != IssueStatusEnum.ReadyForSending))
                    {
                        CurrentMaster.MessagesPlaceHolder.UseRelativePlaceHolder = false;
                        ShowError(GetString("newsletterissue.notallowedtosaveissue"));
                        break;
                    }
                }

                if (editElem.Save())
                {
                    // Show save message
                    ShowChangesSaved();

                    // Refresh breadcrumbs
                    ScriptHelper.RefreshTabHeader(this, null);
                }
                else if (!string.IsNullOrEmpty(editElem.ErrorMessage))
                {
                    CurrentMaster.MessagesPlaceHolder.UseRelativePlaceHolder = false;
                    ShowError(editElem.ErrorMessage);
                }
                break;
        }
    }

    #endregion


    #region "A/B test methods"

    /// <summary>
    /// Inits variant slider. Returns selected variant issue ID.
    /// </summary>
    /// <param name="issueId">Issue ID (currently edited)</param>
    private int InitVariantSlider(int issueId)
    {
        // Initialize variant slider
        ucVariantSlider.IssueID = issueId;
        bool isIssueVariant = ucVariantSlider.Variants.Count > 0;
        if (isIssueVariant)
        {
            int variantIndex = ucVariantSlider.CurrentVariant;
            if (variantIndex >= ucVariantSlider.Variants.Count)
            {
                variantIndex = ucVariantSlider.Variants.Count - 1;
            }
            IssueABVariantItem issueVariant = ucVariantSlider.Variants[(variantIndex < 0 ? 0 : variantIndex)];

            ucVariantSlider.OnVariantDeleted -= new EventHandler(ucVariantSlider_OnVariantDeleted);
            ucVariantSlider.OnVariantDeleted += new EventHandler(ucVariantSlider_OnVariantDeleted);
            ucVariantSlider.OnVariantAdded -= new EventHandler(ucVariantSlider_OnVariantAdded);
            ucVariantSlider.OnVariantAdded += new EventHandler(ucVariantSlider_OnVariantAdded);
            ucVariantSlider.OnSelectionChanged -= new EventHandler(ucVariantSlider_OnSelectionChanged);
            ucVariantSlider.OnSelectionChanged += new EventHandler(ucVariantSlider_OnSelectionChanged);
            return issueVariant.IssueID;
        }
        return issueId;
    }


    /// <summary>
    /// Reloads newly added variant to edit control.
    /// </summary>
    protected void ucVariantSlider_OnVariantAdded(object sender, EventArgs e)
    {
        int issueId = 0;
        if (sender == ucVariantDialog)
        {
            if (!(e is VariantEventArgs)) return;
            VariantEventArgs args = (VariantEventArgs)e;
            issueId = args.ID;
            InitVariantSlider(issueId);
            ucVariantSlider.SetVariant(issueId);
        }
        else
        {
            issueId = ucVariantSlider.IssueID;
        }

        editElem.IssueID = issueId;
        editElem.ReloadData(true);
        InitHeaderActions(issueId);
    }


    /// <summary>
    /// Additional actions after variant has been deleted.
    /// </summary>
    protected void ucVariantSlider_OnVariantDeleted(object sender, EventArgs e)
    {
        int issueId = 0;
        if (ucVariantSlider.Variants.Count > 1)
        {
            IssueABVariantItem issueVariant = ucVariantSlider.Variants[0];
            issueId = issueVariant.IssueID;
        }
        else
        {
            issueId = ucVariantSlider.OriginalIssueID;
        }
        editElem.IssueID = issueId;
        editElem.ReloadData(true);
        InitVariantSlider(issueId);
        ucVariantSlider.SetVariant(issueId);
        InitHeaderActions(issueId);
    }


    protected void ucVariantSlider_OnSelectionChanged(object sender, EventArgs e)
    {
        int issueId = QueryHelper.GetInteger("issueid", 0);
        int variantIndex = ucVariantSlider.CurrentVariant;
        if (variantIndex >= 0 && variantIndex < ucVariantSlider.Variants.Count)
        {
            IssueABVariantItem item = ucVariantSlider.Variants[variantIndex];
            issueId = item.IssueID;
        }

        editElem.IssueID = issueId;
        editElem.ReloadData(true);
        editElem.UpdateContent();
    }

    #endregion
}