using System;
using System.Collections;
using System.Threading;
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
[EditedObject("newsletter.emailtemplate", "templateid", "NewsletterTemplate_Frameset.aspx")]
public partial class CMSModules_Newsletters_Tools_Templates_NewsletterTemplate_Edit : CMSNewsletterTemplatesPage
{
    #region "Variables"

    protected int templateid = 0;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        rfvTemplateDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvTemplateName.ErrorMessage = GetString("NewsletterTemplate_Edit.ErrorEmptyName");

        ScriptHelper.RegisterSpellChecker(this);

        // Control initializations
        string varsScript = string.Format("var emptyNameMsg = '{0}'; \nvar emptyWidthMsg = '{1}'; \nvar emptyHeightMsg = '{2}'; \nvar spellURL = '{3}'; \n",
                                          GetString("NewsletterTemplate_Edit.EmptyNameMsg"),
                                          GetString("NewsletterTemplate_Edit.EmptyWidthMsg"),
                                          GetString("NewsletterTemplate_Edit.EmptyHeightMsg"),
                                          CMSContext.ResolveDialogUrl("~/CMSModules/Content/CMSDesk/Edit/SpellCheck.aspx"));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Script_" + ClientID, ScriptHelper.GetScript(varsScript));

        // Set fields to be checked by Spell Checker
        string spellCheckScript = string.Format("if (typeof(spellCheckFields)==='undefined') {{var spellCheckFields = new Array();}} spellCheckFields.push('{0}');",
                                                htmlTemplateBody.ClientID);
        ScriptHelper.RegisterStartupScript(this, typeof(string), ClientID, ScriptHelper.GetScript(spellCheckScript));

        string currentEmailTemplate = GetString("NewsletterTemplate_Edit.NewItemCaption");

        // Get edited object and its existence
        EmailTemplateInfo emailTemplateObj = (EmailTemplateInfo)EditedObject;
        templateid = emailTemplateObj.TemplateID;

        currentEmailTemplate = emailTemplateObj.TemplateDisplayName;

        // Display editable region section only for e-mail templates of type "Issue template"
        if (emailTemplateObj.TemplateType == EmailTemplateType.Issue)
        {
            pnlEditableRegion.Visible = true;
            plcThumb.Visible = true;
            ucThumbnail.Visible = true;
            ucThumbnail.ObjectID = emailTemplateObj.TemplateID;
            ucThumbnail.ObjectType = NewsletterObjectType.NEWSLETTERTEMPLATE;
            ucThumbnail.Category = MetaFileInfoProvider.OBJECT_CATEGORY_THUMBNAIL;
            ucThumbnail.SiteID = emailTemplateObj.TemplateSiteID;
        }
        else
        {
            plcSubject.Visible = true;
        }

        // Init CSS styles every time during page load
        htmlTemplateBody.EditorAreaCSS = EmailTemplateInfoProvider.GetStylesheetUrl(emailTemplateObj.TemplateName) + "&timestamp=" + DateTime.Now.Millisecond;

        // Initialize header actions
        InitHeaderActions(emailTemplateObj.TemplateID);

        // Initialize HTML editor
        InitHTMLEditor(emailTemplateObj);

        if (!RequestHelper.IsPostBack())
        {
            // Initialize dialog
            LoadData(emailTemplateObj);

            // Show that the emailTemplate was created successfully
            if (QueryHelper.GetBoolean("saved", false))
            {
                ShowChangesSaved();
            }
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Register client scripts
        string script = string.Format(@"
// Insert desired HTML at the current cursor position of the HTML editor
function InsertHTML(htmlString) {{
    // Get the editor instance that we want to interact with.
    var oEditor = CKEDITOR.instances['{0}'];
    // Check the active editing mode.
    if (oEditor.mode == 'wysiwyg') {{
        // Insert the desired HTML.
        oEditor.insertHtml(htmlString);
    }}
    else
        alert('You must be on WYSIWYG mode!');
    return false;
}}

function PasteImage(imageurl) {{
    imageurl = '<img src=""' + imageurl + '"" />';
    return InsertHTML(imageurl);
}}

function InsertEditableRegion() {{
    if (document.getElementById('{1}').value == '') {{
        alert(emptyNameMsg);
        return;
    }}

    var region = ""$$"";
    region += document.getElementById('{1}').value + "":"";
    region += document.getElementById('{2}').value + "":"";
    region += document.getElementById('{3}').value + ""$$"";
    InsertHTML(region);
}}
", htmlTemplateBody.ClientID, txtName.ClientID, txtWidth.ClientID, txtHeight.ClientID);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "InsertHTMLScripts_" + ClientID, ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    /// <param name="templateId">Template ID</param>
    protected void InitHeaderActions(int templateId)
    {
        bool isAuthorized = CurrentUser.IsAuthorizedPerResource("CMS.Newsletter", "ManageTemplates") && (EditedObject != null);
        string script = null;

        // Init save button
        CurrentMaster.HeaderActions.ActionsList.Add(new SaveAction(this)
        {
            ImageUrl = !isAuthorized ? GetImageUrl("CMSModules/CMS_Content/EditMenu/savedisabled.png") : string.Empty,
            Enabled = isAuthorized
        });

        // Init spellcheck button
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.LinkButton,
            Text = GetString("spellcheck.title"),
            Tooltip = GetString("spellcheck.title"),
            OnClientClick = "checkSpelling(spellURL); return false;",
            ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/spellcheck.png")
        });

        int attachCount = 0;
        if (isAuthorized)
        {
            // Get number of attachments
            InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(templateId, NewsletterObjectType.NEWSLETTERTEMPLATE, MetaFileInfoProvider.OBJECT_CATEGORY_TEMPLATE, null, null, "MetafileID", -1);
            attachCount = ds.Items.Count;

            script = @"
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
        }

        // Prepare metafile dialog URL
        string metaFileDialogUrl = ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
        string query = string.Format("?objectid={0}&objecttype={1}", templateId, NewsletterObjectType.NEWSLETTERTEMPLATE);
        metaFileDialogUrl += string.Format("{0}&category={1}&hash={2}", query, MetaFileInfoProvider.OBJECT_CATEGORY_TEMPLATE, QueryHelper.GetHash(query));

        // Init attachment button
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.LinkButton,
            Text = GetString("general.attachments") + string.Format("<span id='attachmentCount'>{0}</span>", (attachCount > 0) ? " (" + attachCount.ToString() + ")" : string.Empty),
            Tooltip = GetString("general.attachments"),
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}', 'Attachments', '700', '500');}}", metaFileDialogUrl) + " return false;",
            ImageUrl = isAuthorized ? GetImageUrl("Objects/CMS_MetaFile/attachment.png") : GetImageUrl("Objects/CMS_MetaFile/attachment_disabled.png"),
            Enabled = isAuthorized
        });

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
            CssClass = "MenuItemEdit Right"
        });

        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Load data of editing emailTemplate.
    /// </summary>
    /// <param name="emailTemplateObj">EmailTemplate object</param>
    protected void LoadData(EmailTemplateInfo emailTemplateObj)
    {
        if (emailTemplateObj != null)
        {
            htmlTemplateBody.ResolvedValue = emailTemplateObj.TemplateBody;
            txtTemplateName.Text = emailTemplateObj.TemplateName;
            txtTemplateHeader.Value = emailTemplateObj.TemplateHeader;
            txtTemplateFooter.Value = emailTemplateObj.TemplateFooter;
            txtTemplateDisplayName.Text = emailTemplateObj.TemplateDisplayName;
            txtTemplateStyleSheetText.Text = emailTemplateObj.TemplateStylesheetText;

            // Display temaplate subject only for 'subscription' and 'unsubscription' template types
            if (emailTemplateObj.TemplateType != EmailTemplateType.Issue)
            {
                txtTemplateSubject.Text = emailTemplateObj.TemplateSubject;
            }
        }
    }


    /// <summary>
    /// Initializes HTML editor's settings.
    /// </summary>
    protected void InitHTMLEditor(EmailTemplateInfo emailTemplateObj)
    {
        htmlTemplateBody.AutoDetectLanguage = false;
        htmlTemplateBody.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlTemplateBody.ToolbarSet = "Newsletter";
        htmlTemplateBody.LinkDialogConfig.UseFullURL = true;
        htmlTemplateBody.QuickInsertConfig.UseFullURL = true;
        if ((emailTemplateObj != null) && (emailTemplateObj.TemplateType == EmailTemplateType.DoubleOptIn))
        {
            htmlTemplateBody.ResolverName = "NewsletterOptInResolver";
        }
        else
        {
            htmlTemplateBody.ResolverName = "NewsletterResolver";
        }

        DialogConfiguration config = htmlTemplateBody.MediaDialogConfig;
        config.UseFullURL = true;
        config.MetaFileObjectID = (emailTemplateObj != null) ? emailTemplateObj.TemplateID : 0;
        config.MetaFileObjectType = NewsletterObjectType.NEWSLETTERTEMPLATE;
        config.MetaFileCategory = MetaFileInfoProvider.OBJECT_CATEGORY_TEMPLATE;
        config.HideAttachments = false;
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                // Check 'Manage templates' permission
                if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "managetemplates"))
                {
                    RedirectToCMSDeskAccessDenied("cms.newsletter", "managetemplates");
                }

                string errorMessage = null;
                // Check template code name
                if (!ValidationHelper.IsCodeName(txtTemplateName.Text))
                {
                    errorMessage = GetString("General.ErrorCodeNameInIdentifierFormat");
                }
                else
                {
                    // Check code and display name for emptiness
                    errorMessage = new Validator().NotEmpty(txtTemplateDisplayName.Text, GetString("general.requiresdisplayname")).NotEmpty(txtTemplateName.Text, GetString("NewsletterTemplate_Edit.ErrorEmptyName")).Result;
                }

                if (string.IsNullOrEmpty(errorMessage))
                {
                    // TemplateName must to be unique
                    EmailTemplateInfo emailTemplateObj = EmailTemplateInfoProvider.GetEmailTemplateInfo(txtTemplateName.Text.Trim(), CMSContext.CurrentSiteID);

                    // If templateName value is unique														
                    if ((emailTemplateObj == null) || (emailTemplateObj.TemplateID == templateid))
                    {
                        // If templateName value is unique -> determine whether it is update or insert 
                        if ((emailTemplateObj == null))
                        {
                            // Get EmailTemplate object by primary key
                            emailTemplateObj = EmailTemplateInfoProvider.GetEmailTemplateInfo(templateid);
                            if (emailTemplateObj == null)
                            {
                                // Create new item -> insert
                                emailTemplateObj = new EmailTemplateInfo();
                            }
                        }

                        // Check region names validity
                        bool isValidRegionName = true;
                        bool isValid = true;
                        string templateBody = htmlTemplateBody.ResolvedValue.Trim();

                        EmailTemplateHelper.ValidateEditableRegions(templateBody, out isValid, out isValidRegionName, null);
                        if (isValid)
                        {
                            if (isValidRegionName)
                            {
                                // Set template object
                                emailTemplateObj.TemplateBody = templateBody;
                                emailTemplateObj.TemplateName = txtTemplateName.Text.Trim();
                                emailTemplateObj.TemplateHeader = ValidationHelper.GetString(txtTemplateHeader.Value, "").Trim();
                                emailTemplateObj.TemplateFooter = ValidationHelper.GetString(txtTemplateFooter.Value, "").Trim();
                                emailTemplateObj.TemplateDisplayName = txtTemplateDisplayName.Text.Trim();
                                emailTemplateObj.TemplateStylesheetText = txtTemplateStyleSheetText.Text.Trim();

                                // Set temaplte subject only for 'subscription' and 'unsubscription' template types
                                if (plcSubject.Visible)
                                {
                                    emailTemplateObj.TemplateSubject = txtTemplateSubject.Text.Trim();
                                }

                                // Save the template object and display info message
                                EmailTemplateInfoProvider.SetEmailTemplateInfo(emailTemplateObj);
                                ShowChangesSaved();

                                // Reload header if changes were saved
                                if (TabMode)
                                {
                                    ScriptHelper.RefreshTabHeader(Page, null);
                                }
                            }
                            else
                            {
                                ShowError(GetString("NewsletterTemplate_Edit.EditableRegionNameError"));
                            }
                        }
                        else
                        {
                            ShowError(GetString("NewsletterTemplate_Edit.EditableRegionError"));
                        }
                    }
                    else
                    {
                        ShowError(GetString("NewsletterTemplate_Edit.TemplateNameExists"));
                    }
                }
                else
                {
                    ShowError(errorMessage);
                }
                break;
        }
    }

    #endregion
}