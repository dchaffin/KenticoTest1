using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.EmailEngine;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.FormControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Controls;

public partial class CMSModules_EmailTemplates_Controls_EmailTemplateEdit : CMSAdminEditControl
{
    #region "Private Variables"

    private EmailTemplateInfo templateInfo = null;
    private int mSiteId = 0;
    private int mEmailTemplateId = 0;
    private bool mGlobalTemplate = false;
    private string mRefreshPageURL = "~/CMSModules/EmailTemplates/Pages/Edit.aspx";

    private HeaderAction attachmentsAction = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether the e-mail template is global (independent on site).
    /// </summary>
    public bool GlobalTemplate
    {
        get
        {
            return mGlobalTemplate;
        }
        set
        {
            mGlobalTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the site ID for which the e-mail should be displayed.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets the e-mail template ID of currently processed e-mail template.
    /// </summary>
    public int EmailTemplateID
    {
        get
        {
            return mEmailTemplateId;
        }
        set
        {
            mEmailTemplateId = value;
        }
    }


    /// <summary>
    /// Gets or sets the e-mail template type of currently processed e-mail.
    /// It is used to limit options when in selector dialog mode.
    /// </summary>
    /// <remarks>
    /// null represents all types.
    /// </remarks>
    public EmailTemplateMacros.EmailType? TemplateType
    {
        get;
        set;
    }


    /// <summary>
    /// Gets whether the contorl is in edit mode.
    /// </summary>
    private bool EditMode
    {
        get
        {
            return EmailTemplateID > 0;
        }
    }


    /// <summary>
    /// Gets or sets whether the control is in dialog mode.
    /// </summary>
    public bool DialogMode
    {
        get;
        set;
    }


    /// <summary>
    /// Page that should be refreshed after button click.
    /// </summary>
    public string RefreshPageURL
    {
        get
        {
            return mRefreshPageURL;
        }
        set
        {
            mRefreshPageURL = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize HTML editor
        txtText.Editor.EnableViewState = false;
        txtText.Editor.EditorMode = EditorModeEnum.Advanced;
        txtText.Editor.Language = LanguageEnum.HTMLMixed;
        txtText.Editor.Height = new Unit("400px");
        txtText.Editor.Width = new Unit("100%");

        // Initialize Plain Text editor
        txtPlainText.Editor.EnableViewState = false;
        txtPlainText.Editor.EditorMode = EditorModeEnum.Advanced;
        txtPlainText.Editor.Language = LanguageEnum.Text;
        txtPlainText.Editor.Height = new Unit("400px");
        txtPlainText.Editor.Width = new Unit("100%");

        txtPlainText.Editor.FullScreenParentElementID = txtText.Editor.FullScreenParentElementID = "divContent";

        if (TemplateType == null)
        {
            drpEmailType.DropDown.AutoPostBack = true;
        }
        else
        {
            drpEmailType.DropDown.Enabled = false;
            drpEmailType.Value = EmailTemplateMacros.GetEmailTypeString(TemplateType.Value);
        }

        // Initialize required field validators
        RequiredFieldValidatorDisplayName.ErrorMessage = GetString("EmailTemplate_Edit.FillDisplayNameField");
        RequiredFieldValidatorCodeName.ErrorMessage = GetString("EmailTemplate_Edit.FillCodeNameField");

        // Register save button handler
        pnlObjectLocking.ObjectManager.OnSaveData += new EventHandler<SimpleObjectManagerEventArgs>(ObjectManager_OnSaveData);
        pnlObjectLocking.ObjectManager.OnAfterAction += new EventHandler<SimpleObjectManagerEventArgs>(ObjectManager_OnAfterAction);

        if (EmailTemplateID > 0)
        {
            // Get email template info of specified 'templateid'
            templateInfo = EmailTemplateProvider.GetEmailTemplate(EmailTemplateID);
            EditedObject = templateInfo;

            // Get SiteID of the template
            SiteID = templateInfo.TemplateSiteID;

            // Check if user is assigned to web site
            if (!CMSContext.CurrentUser.IsGlobalAdministrator)
            {
                if (!CMSContext.CurrentUser.IsInSite(SiteInfoProvider.GetSiteName(SiteID)))
                {
                    // Disable object for user
                    EditedObject = null;
                }
            }

            // Fill the form
            if (!RequestHelper.IsPostBack())
            {
                // Load data to the form
                LoadData(templateInfo);

                // Show message that the email template was created or updated successfully.
                if (QueryHelper.GetBoolean("saved", false))
                {
                    ShowChangesSaved();

                    // Reload header if changes were saved
                    if ((Page is CMSPage) && ((CMSPage)Page).TabMode)
                    {
                        ScriptHelper.RefreshTabHeader(Page, null);
                    }
                }
            }

            // Register paste attachment script
            RegisterPasteScript();
        }
        else
        {
            pnlObjectLocking.ObjectManager.ObjectType = PredefinedObjectType.EMAILTEMPLATE;
        }

        // Get correct MacroResolver
        ContextResolver resolver = EmailTemplateMacros.GetEmailTemplateResolver(EmailTemplateMacros.GetEmailTypeEnum(ValidationHelper.GetString(drpEmailType.Value, "")));

        txtText.Resolver = resolver;
        txtPlainText.Resolver = resolver;

        // Disable display name and code name editation when editing item from selector
        txtDisplayName.Enabled = txtCodeName.Enabled = !EditMode || !DialogMode;

        macroSelectorElm.Resolver = resolver;
        macroSelectorElm.ShowMacroTreeAbove = true;
        macroSelectorElm.ExtendedTextAreaElem = txtText.Editor.EditorID;
        macroSelectorElm.TextAreaID = txtText.Editor.ClientID;

        macroSelectorPlain.Resolver = resolver;
        macroSelectorPlain.ShowMacroTreeAbove = true;
        macroSelectorPlain.ExtendedTextAreaElem = txtPlainText.Editor.EditorID;
        macroSelectorPlain.TextAreaID = txtPlainText.Editor.ClientID;

        InitHeaderActions(EmailTemplateID, SiteID);
    }


    /// <summary>
    /// Initializes header actions.
    /// </summary>
    /// <param name="templateId">Email template ID</param>
    /// <param name="siteId">Site ID</param>
    protected void InitHeaderActions(int templateId, int siteId)
    {
        if (templateId > 0)
        {
            // Get number of attachments
            InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(templateId, PredefinedObjectType.EMAILTEMPLATE, MetaFileInfoProvider.OBJECT_CATEGORY_TEMPLATE,
                siteId > 0 ? "MetaFileSiteID=" + siteId : "MetaFileSiteID IS NULL", null, "MetafileID", -1);
            int attachCount = ds.Items.Count;

            string script = @"
function UpdateAttachmentCount(count) {
    var counter = document.getElementById('attachmentCount');
    if (counter != null) {
        if (count > 0) { counter.innerHTML = ' (' + count + ')'; }
        else { counter.innerHTML = ''; }
    }
}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UpdateAttachmentScript_" + this.ClientID, script, true);

            // Register dialog scripts
            ScriptHelper.RegisterDialogScript(Page);

            // Prepare metafile dialog URL
            string metaFileDialogUrl = ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
            string query = string.Format("?objectid={0}&objecttype={1}&siteid={2}", templateId, PredefinedObjectType.EMAILTEMPLATE, siteId);
            metaFileDialogUrl += string.Format("{0}&category={1}&hash={2}", query, MetaFileInfoProvider.OBJECT_CATEGORY_TEMPLATE, QueryHelper.GetHash(query));

            // Init attachment button
            ObjectEditMenu menu = ControlsHelper.GetChildControl(Page, typeof(ObjectEditMenu)) as ObjectEditMenu;
            if (menu != null)
            {
                attachmentsAction = new HeaderAction()
                {
                    ControlType = HeaderActionTypeEnum.LinkButton,
                    Text = GetString("general.attachments") + string.Format("<span id='attachmentCount'>{0}</span>", (attachCount > 0) ? " (" + attachCount.ToString() + ")" : string.Empty),
                    Tooltip = GetString("general.attachments"),
                    OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}', 'Attachments', '700', '500');}}", metaFileDialogUrl) + " return false;",
                    ImageUrl = GetImageUrl("Objects/CMS_MetaFile/attachment.png"),
                    Visible = !pnlObjectLocking.IsObjectLocked()
                };

                menu.AddExtraAction(attachmentsAction);
            }
        }
    }


    /// <summary>
    /// Load data of edited email template from DB into textboxes.
    /// </summary>
    /// <param name="templateInfo">EmailTemplateInfo object</param>
    protected void LoadData(EmailTemplateInfo templateInfo)
    {
        txtDisplayName.Text = templateInfo.TemplateDisplayName;
        txtCodeName.Text = templateInfo.TemplateName;
        txtSubject.Text = templateInfo.TemplateSubject;
        txtBcc.Text = templateInfo.TemplateBcc;
        txtCc.Text = templateInfo.TemplateCc;
        txtFrom.Text = templateInfo.TemplateFrom;
        txtText.Text = templateInfo.TemplateText;
        txtPlainText.Text = templateInfo.TemplatePlainText;
        drpEmailType.Value = templateInfo.TemplateType;
    }


    /// <summary>
    /// Registers script for pasting attached images into email body.
    /// </summary>
    protected void RegisterPasteScript()
    {
        string script =
string.Format(@"function PasteImage(imageurl) {{
    if (imageurl.substr(0, 1) === '/') {{
        var imageurl = '~' + imageurl;
    }}
    var imageHtml = '<img src=""' + imageurl + '"" alt="""" />';
    if (InsertMacroExtended != null) {{
        InsertMacroExtended(imageHtml, (typeof({0}) != 'undefined' ? {0} : null), '{1}');
    }}
}}", txtText.Editor.EditorID, txtText.Editor.ClientID);

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "PasteImage_" + ClientID, script, true);
    }


    /// <summary>
    /// Reloads data after save.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if (e.ActionName != ComponentEvents.SAVE)
        {
            LoadData((EmailTemplateInfo)EditedObject);
        }

        if (attachmentsAction != null)
        {
            attachmentsAction.Visible = !pnlObjectLocking.IsObjectLocked();
        }

        if (DialogMode)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "parentWOpenerRefresh", ScriptHelper.GetScript("if (parent && parent.wopener && parent.wopener.refresh) { parent.wopener.refresh(); }"));
        }
    }


    /// <summary>
    /// Handle btnOK's OnClick event.
    /// </summary>
    protected void ObjectManager_OnSaveData(object sender, SimpleObjectManagerEventArgs e)
    {
        Save(false);
    }


    public void Save(bool closeOnSave)
    {
        // Check modify permission
        RaiseOnCheckPermissions(PERMISSION_MODIFY, this);

        txtDisplayName.Text = txtDisplayName.Text.Trim();
        txtCodeName.Text = txtCodeName.Text.Trim();
        txtSubject.Text = txtSubject.Text.Trim();

        // Find whether required fields are not empty
        string result = new Validator().NotEmpty(txtDisplayName.Text, GetString("EmailTemplate_Edit.FillDisplayNameField"))
            .NotEmpty(txtCodeName.Text, GetString("EmailTemplate_Edit.FillCodeNameField"))
            .IsCodeName(txtCodeName.Text, GetString("general.invalidcodename"))
            .Result;

        // Check validity of entered e-mails
        if (!String.IsNullOrEmpty(txtFrom.Text) && !ValidationHelper.AreEmails(txtFrom.Text))
        {
            result = GetString("EmailTemplate_Edit.InvalidFrom");
        }
        else if (!String.IsNullOrEmpty(txtBcc.Text) && !ValidationHelper.AreEmails(txtBcc.Text))
        {
            result = GetString("EmailTemplate_Edit.InvalidBcc");
        }
        else if (!String.IsNullOrEmpty(txtCc.Text) && !ValidationHelper.AreEmails(txtCc.Text))
        {
            result = GetString("EmailTemplate_Edit.InvalidCc");
        }
        
        // Check max length if macros are entered
        if (MacroResolver.ContainsMacro(txtDisplayName.Text) && (txtDisplayName.Text.Length > txtDisplayName.MaxLength))
        {
            result = ResHelper.LocalizeString("{$general.displayname$}: {$cmstextbox.macrotoolong$}");
        }
        else if (MacroResolver.ContainsMacro(txtSubject.Text) && (txtSubject.Text.Length > txtSubject.MaxLength))
        {
            result = ResHelper.LocalizeString("{$general.subject$}: {$cmstextbox.macrotoolong$}");
        }

        if (String.IsNullOrEmpty(result))
        {
            string siteName = null;
            if (SiteID != 0)
            {
                // Get site name for non-global templates
                SiteInfo site = SiteInfoProvider.GetSiteInfo(SiteID);
                if (site != null)
                {
                    siteName = site.SiteName;
                }
            }
            // Try to get template by template name and site name
            EmailTemplateInfo templateInfo = EmailTemplateProvider.GetEmailTemplate(txtCodeName.Text, siteName);

            // Find if codename of the email template is unique for the site
            if ((templateInfo == null) || (templateInfo.TemplateID == EmailTemplateID) || ((templateInfo.TemplateSiteID == 0) && (SiteID > 0)))
            {
                bool isInsert = false;

                // Get object
                if (templateInfo == null)
                {
                    templateInfo = EmailTemplateProvider.GetEmailTemplate(EmailTemplateID);
                    if (templateInfo == null)
                    {
                        templateInfo = new EmailTemplateInfo();
                        isInsert = true;
                    }
                }

                templateInfo.TemplateID = EmailTemplateID;
                templateInfo.TemplateDisplayName = txtDisplayName.Text;
                templateInfo.TemplateName = txtCodeName.Text;
                templateInfo.TemplateSubject = txtSubject.Text;
                templateInfo.TemplateFrom = txtFrom.Text;
                templateInfo.TemplateBcc = txtBcc.Text;
                templateInfo.TemplateCc = txtCc.Text;
                templateInfo.TemplateText = txtText.Text;
                templateInfo.TemplatePlainText = txtPlainText.Text;
                templateInfo.TemplateSiteID = SiteID;
                templateInfo.TemplateType = ValidationHelper.GetString(drpEmailType.Value, null);

                // Save (insert/update) EmailTemplateInfo object
                EmailTemplateProvider.SetEmailTemplate(templateInfo);

                CMSContext.EditedObject = templateInfo;

                if (isInsert)
                {
                    pnlObjectLocking.ObjectManager.ObjectType = null;
                    CMSObjectManager.CheckOutNewObject(this.Page);
                }

                // Handle redirection
                RedirectOnSave(templateInfo, closeOnSave);
            }
            else
            {
                ShowError(GetString("EmailTemplate_Edit.UniqueCodeName"));
            }
        }
        else
        {
            ShowError(result);
        }
    }


    private void RedirectOnSave(EmailTemplateInfo templateInfo, bool closeOnSave)
    {
        StringBuilder redirectUrl = new StringBuilder();
        redirectUrl.Append(URLHelper.ResolveUrl(RefreshPageURL), "?saved=1&tabmode=", QueryHelper.GetInteger("tabmode", 0));

        if (GlobalTemplate)
        {
            redirectUrl.Append("&selectedsiteid=0");
        }
        else if (QueryHelper.GetInteger("selectedsiteid", 0) != 0)
        {
            redirectUrl.Append("&selectedsiteid=", QueryHelper.GetInteger("selectedsiteid", 0));
        }
        else if (SiteID > 0)
        {
            redirectUrl.Append("&siteid=" + SiteID);
        }

        // Selector ID in query means dialog mode
        string selector = QueryHelper.GetString("selectorid", string.Empty);
        if (!string.IsNullOrEmpty(selector))
        {
            // Add dialog specific query parameters
            redirectUrl.Append("&editonlycode=1");
            redirectUrl.Append("&name=", templateInfo.TemplateName, "&selectorid=", selector);

            string type = null;
            if (TemplateType != null)
            {
                type = "&templatetype=" + EmailTemplateMacros.GetEmailTypeString(TemplateType.Value);
                redirectUrl.Append(type);
            }
            redirectUrl.Append("&hash=", QueryHelper.GetHash("?editonlycode=1" + type));

            ScriptHelper.RegisterWOpenerScript(Page);

            StringBuilder script = new StringBuilder();
            if (!EditMode)
            {
                // Add selector refresh
                script.Append(@"
if (wopener) {{                        
    wopener.US_SelectNewValue_", selector, "('", templateInfo.TemplateName, @"');
}}");
            }

            if (closeOnSave)
            {
                script.AppendLine("CloseDialog();");
            }
            else
            {
                script.Append(@"
window.name = '", selector, @"';
window.open('", URLHelper.UrlEncodeQueryString(redirectUrl.ToString()), "', window.name);");
            }

            ScriptHelper.RegisterStartupScript(this, GetType(), "UpdateSelector", script.ToString(), true);
        }
        else if (DialogMode)
        {
            if (closeOnSave)
            {
                ScriptHelper.RegisterWOpenerScript(Page);
                ScriptHelper.RegisterStartupScript(this, GetType(), "closeDialog", ScriptHelper.GetScript("CloseDialog();"));
            }
            else
            {
                ShowChangesSaved();
            }
        }
        else
        {
            // Add UI specific query parameters
            redirectUrl.Append("&templateid=", templateInfo.TemplateID);
            URLHelper.Redirect(URLHelper.UrlEncodeQueryString(redirectUrl.ToString()));
        }
    }

    #endregion
}