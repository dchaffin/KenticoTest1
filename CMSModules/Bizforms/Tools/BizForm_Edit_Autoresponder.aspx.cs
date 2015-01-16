using System;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_BizForms_Tools_BizForm_Edit_Autoresponder : CMSBizFormPage
{
    #region "Variables"

    private int formId = 0;
    private DataClassInfo formClassObj = null;
    private CurrentUserInfo currentUser = null;
    protected SaveAction save = null;
    protected HeaderAction attachments = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates whether custom form layout is set or not.
    /// </summary>
    private bool IsLayoutSet
    {
        get
        {
            object obj = ViewState["IsLayoutSet"];
            return ValidationHelper.GetBoolean(obj, false);
        }
        set
        {
            ViewState["IsLayoutSet"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'ReadForm' and 'EditData' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.form", "ReadForm"))
        {
            RedirectToCMSDeskAccessDenied("cms.form", "ReadForm");
        }

        currentUser = CMSContext.CurrentUser;

        // Register for action
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, lnkSave_Click);

        // Get form id from URL
        formId = QueryHelper.GetInteger("formid", 0);
        BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(formId);
        EditedObject = bfi;

        if (bfi != null)
        {
            // Control initialization
            ltlConfirmDelete.Text = "<input type=\"hidden\" id=\"confirmdelete\" value=\"" + GetString("Bizform_Edit_Autoresponder.ConfirmDelete") + "\">";

            drpEmailField.SelectedIndexChanged += new EventHandler(drpEmailField_SelectedIndexChanged);

            // Init header actions
            InitHeaderActions();

            // Initialize HTML editor
            InitHTMLEditor();

            if (!RequestHelper.IsPostBack())
            {
                // Get bizform class object
                formClassObj = DataClassInfoProvider.GetDataClass(bfi.FormClassID);
                if (formClassObj != null)
                {
                    // Enable or disable form
                    EnableDisableForm(bfi.FormConfirmationTemplate);

                    // Fill list of available fields                    
                    FillFieldsList();

                    // Load dropdown list with form text fields   
                    FormInfo fi = FormHelper.GetFormInfo(formClassObj.ClassName, false);
                    drpEmailField.DataSource = fi.GetFields(FormFieldDataTypeEnum.Text);
                    drpEmailField.DataBind();
                    drpEmailField.Items.Insert(0, new ListItem(GetString("bizform_edit_autoresponder.emptyemailfield"), string.Empty));

                    // Try to select specified field
                    ListItem li = drpEmailField.Items.FindByValue(bfi.FormConfirmationEmailField);
                    if (li != null)
                    {
                        li.Selected = true;
                    }

                    // Load email subject and email from address
                    txtEmailFrom.Text = bfi.FormConfirmationSendFromEmail;
                    txtEmailSubject.Text = bfi.FormConfirmationEmailSubject;
                }
                else
                {
                    // Disable form by default
                    EnableDisableForm(null);
                }
            }
        }
    }


    protected void Page_PreRender(Object sender, EventArgs e)
    {
        btnInsertInput.OnClientClick = "InsertAtCursorPosition('$$value:' + document.getElementById('" + lstAvailableFields.ClientID + "').value + '$$'); return false;";
        btnInsertLabel.OnClientClick = "InsertAtCursorPosition('$$label:' + document.getElementById('" + lstAvailableFields.ClientID + "').value + '$$'); return false;";

        SetCustomLayoutVisibility(!string.IsNullOrEmpty(drpEmailField.SelectedValue));

        // Get save script
        string script = null;
        if (!pnlCustomLayout.Visible && IsLayoutSet)
        {
            script = "if(!ConfirmDelete()) { return false; } ";
        }

        // Refresh script
        save.OnClientClick = script;
        menu.ReloadData();

        // Register other scripts
        RegisterScripts();
    }


    private void drpEmailField_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetCustomLayoutVisibility(!string.IsNullOrEmpty(drpEmailField.SelectedValue));
    }


    /// <summary>
    /// Save button is clicked.
    /// </summary>
    protected void lnkSave_Click(object sender, EventArgs e)
    {
        string errorMessage = null;

        // Check 'EditForm' permission
        if (!currentUser.IsAuthorizedPerResource("cms.form", "EditForm"))
        {
            RedirectToCMSDeskAccessDenied("cms.form", "EditForm");
        }

        // Validate form
        errorMessage = new Validator().NotEmpty(txtEmailFrom.Text.Trim(), GetString("bizform_edit_autoresponder.emptyemail")).NotEmpty(txtEmailSubject.Text.Trim(), GetString("bizform_edit_autoresponder.emptysubject")).Result;

        // Check if from e-mail contains macro expression or e-mails separated by semicolon
        if (string.IsNullOrEmpty(errorMessage) && !MacroResolver.ContainsMacro(txtEmailFrom.Text.Trim()) && !ValidationHelper.IsEmail(txtEmailFrom.Text.Trim()))
        {
            errorMessage = GetString("bizform_edit_autoresponder.emptyemail");
        }

        if ((string.IsNullOrEmpty(errorMessage)) || (!pnlCustomLayout.Visible))
        {
            errorMessage = String.Empty;
            BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(formId);
            if (bfi != null)
            {
                // Save custom layout
                if (!string.IsNullOrEmpty(drpEmailField.SelectedValue))
                {
                    bfi.FormConfirmationTemplate = htmlEditor.ResolvedValue.Trim();
                    bfi.FormConfirmationEmailField = drpEmailField.SelectedValue;
                    bfi.FormConfirmationEmailSubject = txtEmailSubject.Text.Trim();
                    bfi.FormConfirmationSendFromEmail = txtEmailFrom.Text.Trim();

                    try
                    {
                        BizFormInfoProvider.SetBizFormInfo(bfi);
                        ShowChangesSaved();
                        EnableDisableForm(bfi.FormConfirmationTemplate);
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                    }
                }
                // Delete custom layout if exists
                else
                {
                    bfi.FormConfirmationTemplate = null;
                    bfi.FormConfirmationEmailField = drpEmailField.SelectedValue;
                    bfi.FormConfirmationEmailSubject = string.Empty;
                    bfi.FormConfirmationSendFromEmail = string.Empty;

                    // Delete all attachments
                    MetaFileInfoProvider.DeleteFiles(bfi.FormID, FormObjectType.BIZFORM, MetaFileInfoProvider.OBJECT_CATEGORY_FORM_LAYOUT);

                    try
                    {
                        BizFormInfoProvider.SetBizFormInfo(bfi);
                        if(IsLayoutSet)
                        {
                            ShowConfirmation(GetString("Bizform_Edit_Autoresponder.LayoutDeleted"));
                        }
                        else
                        {
                            ShowChangesSaved();
                        }
                        EnableDisableForm(bfi.FormConfirmationTemplate);
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes header actions.
    /// </summary>
    protected void InitHeaderActions()
    {
        menu.ActionsList.Clear();

        // Add save action
        save = new SaveAction(Page);
        menu.ActionsList.Add(save);

        bool isAuthorized = CurrentUser.IsAuthorizedPerResource("cms.form", "EditForm") && ((formId > 0) && (EditedObject != null));

        int attachCount = 0;
        if (isAuthorized)
        {
            // Get number of attachments
            InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(formId, FormObjectType.BIZFORM, MetaFileInfoProvider.OBJECT_CATEGORY_FORM_LAYOUT, null, null, "MetafileID", -1);
            attachCount = ds.Items.Count;

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
        }

        // Prepare metafile dialog URL
        string metaFileDialogUrl = ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
        string query = string.Format("?objectid={0}&objecttype={1}", formId, FormObjectType.BIZFORM);
        metaFileDialogUrl += string.Format("{0}&category={1}&hash={2}", query, MetaFileInfoProvider.OBJECT_CATEGORY_FORM_LAYOUT, QueryHelper.GetHash(query));

        // Init attachment button
        attachments = new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.LinkButton,
            Text = GetString("general.attachments") + string.Format("<span id='attachmentCount'>{0}</span>", (attachCount > 0) ? " (" + attachCount.ToString() + ")" : string.Empty),
            Tooltip = GetString("general.attachments"),
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}', 'Attachments', '700', '500');}}", metaFileDialogUrl) + " return false;",
            ImageUrl = isAuthorized ? GetImageUrl("Objects/CMS_MetaFile/attachment.png") : GetImageUrl("Objects/CMS_MetaFile/attachment_disabled.png"),
            Enabled = isAuthorized
        };
        menu.ActionsList.Add(attachments);
    }


    /// <summary>
    /// Initializes HTML editor's settings.
    /// </summary>
    protected void InitHTMLEditor()
    {
        htmlEditor.AutoDetectLanguage = false;
        htmlEditor.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlEditor.LinkDialogConfig.UseFullURL = true;
        htmlEditor.QuickInsertConfig.UseFullURL = true;
        htmlEditor.ToolbarSet = "BizForm";

        DialogConfiguration config = htmlEditor.MediaDialogConfig;
        config.UseFullURL = true;
        config.MetaFileObjectID = formId;
        config.MetaFileObjectType = FormObjectType.BIZFORM;
        config.MetaFileCategory = MetaFileInfoProvider.OBJECT_CATEGORY_FORM_LAYOUT;
        config.HideAttachments = false;
    }


    /// <summary>
    /// Sets visibility of custom layout form.
    /// </summary>
    private void SetCustomLayoutVisibility(bool visible)
    {
        pnlCustomLayout.Visible = visible;
        attachments.Enabled = visible;

        if (visible)
        {
            // Reload HTML editor content
            BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(formId);
            if (bfi != null && bfi.FormConfirmationTemplate != null)
            {
                htmlEditor.ResolvedValue = bfi.FormConfirmationTemplate;
            }
        }
    }


    /// <summary>
    /// Fills list of available fields.
    /// </summary>
    private void FillFieldsList()
    {
        if (formClassObj != null)
        {
            // load form definition and get visible fields
            var fi = FormHelper.GetFormInfo(formClassObj.ClassName, false);
            var fields = fi.GetFields(true, true);

            lstAvailableFields.Items.Clear();

            if (fields != null)
            {
                // add visible fields to the list
                foreach (FormFieldInfo ffi in fields)
                {
                    lstAvailableFields.Items.Add(new ListItem(ffi.Name, ffi.Name));
                }
            }
            lstAvailableFields.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// Enables or disables form according to the confirmation email template text is defined or not.
    /// </summary>
    /// <param name="formLayout">Autoresponder layout</param>
    protected void EnableDisableForm(string formLayout)
    {
        if (RequestHelper.IsPostBack())
        {
            InitHeaderActions();
        }

        if (!string.IsNullOrEmpty(formLayout))
        {
            // Enable layout editing                                
            pnlCustomLayout.Visible = true;

            // Set confirmation email template text to the editable window of the HTML editor
            htmlEditor.ResolvedValue = formLayout;

            // Save info to viewstate 
            IsLayoutSet = true;
        }
        else
        {
            // Layout editing is not enabled by default        
            pnlCustomLayout.Visible = false;

            htmlEditor.ResolvedValue = string.Empty;

            // Save info to viewstate
            IsLayoutSet = false;
        }
    }


    /// <summary>
    /// Registers JS scripts.
    /// </summary>
    protected void RegisterScripts()
    {
        string script =
string.Format(@"// Insert desired HTML at the current cursor position of the CK editor
function InsertHTML(htmlString) {{
    // Get the editor instance that we want to interact with
    var oEditor = CKEDITOR.instances['{0}'];

    // Check the active editing mode
    if (oEditor.mode == 'wysiwyg') {{
        // Insert the desired HTML.
        oEditor.insertHtml(htmlString);
    }}
    else alert('You must be on WYSIWYG mode!');
}}

// Set content of the CK editor - replace the actual one
function SetContent(newContent) {{
    // Get the editor instance that we want to interact with.
    var oEditor = CKEDITOR.instances['{0}'];

    // Set the editor content (replace the actual one).
    oEditor.setData(newContent);
}}

function PasteImage(imageurl) {{
    imageurl = '<img src=""' + imageurl + '"" />';
    return InsertHTML(imageurl);
}}

// Returns HTML code with standard table layout
function GenerateTableLayout() {{
    var tableLayout = """";

    // indicates whether any row definition was added to the table
    var rowAdded = false;

    // list of attributes
    var list = document.getElementById('{1}');

    // attributes count
    var optionsCount = 0;
    if (list != null) {{
        optionsCount = list.options.length;
    }}

    for (var i = 0; i < optionsCount; i++) {{
        tableLayout += ""<tr><td>$$label:"" + list.options[i].value + ""$$</td><td>$$value:"" + list.options[i].value + ""$$</td></tr>"";
        rowAdded = true;
    }}

    if (rowAdded) {{
        tableLayout = ""<table><tbody>"" + tableLayout + ""</tbody></table>"";
    }}

    return tableLayout;
}}

// Insert desired HTML at the current cursor position of the CK editor if it is not already inserted 
function InsertAtCursorPosition(htmlString) {{
    InsertHTML(htmlString);
}}

function ConfirmDelete() {{
    return confirm(document.getElementById('confirmdelete').value);
}}", htmlEditor.ClientID, lstAvailableFields.ClientID);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Autoresponder_" + ClientID, script, true);
    }

    #endregion
}