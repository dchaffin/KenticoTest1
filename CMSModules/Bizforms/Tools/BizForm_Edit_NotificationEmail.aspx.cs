using System;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_BizForms_Tools_BizForm_Edit_NotificationEmail : CMSBizFormPage
{
    #region "Variables"

    private int formId = 0;
    private DataClassInfo formClassObj = null;
    protected SaveAction save = null;

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
            return (obj == null) ? false : (bool)obj;
        }
        set
        {
            ViewState["IsLayoutSet"] = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'ReadForm' and 'EditData' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.form", "ReadForm"))
        {
            RedirectToCMSDeskAccessDenied("cms.form", "ReadForm");
        }

        // Get form id from URL
        formId = QueryHelper.GetInteger("formid", 0);

        // Add save action
        save = new SaveAction(Page);
        menu.AddAction(save);

        // Register for action
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, lnkSave_Click);
    
        // Control initialization
        ltlConfirmDelete.Text = "<input type=\"hidden\" id=\"confirmdelete\" value=\"" + GetString("Bizform_Edit_Notificationemail.ConfirmDelete") + "\">";

        chkSendToEmail.Text = GetString("BizFormGeneral.chkSendToEmail");
        chkAttachDocs.Text = GetString("BizForm_Edit_NotificationEmail.AttachUploadedDocs");

        chkCustomLayout.Text = GetString("BizForm_Edit_NotificationEmail.CustomLayout");

        // Initialize HTML editor
        InitHTMLEditor();

        BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(formId);
        EditedObject = bfi;

        if (!RequestHelper.IsPostBack())
        {
            if (bfi != null)
            {
                // Get form class object
                formClassObj = DataClassInfoProvider.GetDataClass(bfi.FormClassID);

                // Fill list of available fields                    
                FillFieldsList();

                // Load email from/to address and email subject
                txtFromEmail.Text = ValidationHelper.GetString(bfi.FormSendFromEmail, "");
                txtToEmail.Text = ValidationHelper.GetString(bfi.FormSendToEmail, "");
                txtSubject.Text = ValidationHelper.GetString(bfi.FormEmailSubject, "");
                chkAttachDocs.Checked = bfi.FormEmailAttachUploadedDocs;
                chkSendToEmail.Checked = ((txtFromEmail.Text + txtToEmail.Text) != "");
                if (!chkSendToEmail.Checked)
                {
                    txtFromEmail.Enabled = false;
                    txtToEmail.Enabled = false;
                    txtSubject.Enabled = false;
                    chkAttachDocs.Enabled = false;
                    chkCustomLayout.Visible = false;
                    pnlCustomLayout.Visible = false;
                }
                else
                {
                    // Enable or disable form
                    EnableDisableForm(bfi.FormEmailTemplate);
                }
            }
            else
            {
                // Disable form by default
                EnableDisableForm(null);
            }
        }
    }


    protected void Page_PreRender(Object sender, EventArgs e)
    {
        if (pnlCustomLayout.Visible)
        {
            btnInsertInput.OnClientClick = "InsertAtCursorPosition('$$value:' + document.getElementById('" + lstAvailableFields.ClientID + "').value + '$$'); return false;";
            btnInsertLabel.OnClientClick = "InsertAtCursorPosition('$$label:' + document.getElementById('" + lstAvailableFields.ClientID + "').value + '$$'); return false;";
        }

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


    /// <summary>
    /// On chkSendToEmail checked event handler.
    /// </summary>
    protected void chkSendToEmail_CheckedChanged(object sender, EventArgs e)
    {
        txtFromEmail.Enabled = chkSendToEmail.Checked;
        txtToEmail.Enabled = chkSendToEmail.Checked;
        txtSubject.Enabled = chkSendToEmail.Checked;
        chkAttachDocs.Enabled = chkSendToEmail.Checked;
        if (chkSendToEmail.Checked)
        {
            chkCustomLayout.Visible = true;
            if (chkCustomLayout.Checked)
            {
                pnlCustomLayout.Visible = true;

                // Reload HTML editor content
                BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(formId);
                if (bfi != null && bfi.FormEmailTemplate != null)
                {
                    htmlEditor.ResolvedValue = bfi.FormEmailTemplate;
                }
            }
        }
        else
        {
            chkCustomLayout.Visible = false;
            pnlCustomLayout.Visible = false;
        }
    }


    /// <summary>
    /// Custom layout checkbox checked changed.
    /// </summary>
    protected void chkCustomLayout_CheckedChanged(object sender, EventArgs e)
    {
        pnlCustomLayout.Visible = !pnlCustomLayout.Visible;

        if (chkCustomLayout.Checked)
        {
            BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(formId);
            if (bfi != null && bfi.FormEmailTemplate != null)
            {
                htmlEditor.ResolvedValue = bfi.FormEmailTemplate;
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
            // Load form definition and get visible fields
            var fi = FormHelper.GetFormInfo(formClassObj.ClassName, false);
            var fields = fi.GetFields(true, true);

            lstAvailableFields.Items.Clear();

            if (fields != null)
            {
                // Add visible fields to the list
                foreach (FormFieldInfo ffi in fields)
                {
                    lstAvailableFields.Items.Add(new ListItem(ffi.Name, ffi.Name));
                }
            }
            lstAvailableFields.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// Enables or disables form according to form layout is defined or not.
    /// </summary>
    protected void EnableDisableForm(string formLayout)
    {
        // if form layout is set
        if (formLayout != null)
        {
            //enable form editing                    
            chkCustomLayout.Checked = true;
            pnlCustomLayout.Visible = true;

            // set text (form layout) to the editable window of the HTML editor
            htmlEditor.ResolvedValue = formLayout;

            // save info to viewstate 
            IsLayoutSet = true;
        }
        else
        {
            // form is not enabled by default        
            chkCustomLayout.Checked = false;
            pnlCustomLayout.Visible = false;

            htmlEditor.Value = string.Empty;

            // save info to viewstate
            IsLayoutSet = false;
        }
    }


    /// <summary>
    /// Initializes HTML editor's settings.
    /// </summary>
    protected void InitHTMLEditor()
    {
        htmlEditor.AutoDetectLanguage = false;
        htmlEditor.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlEditor.ToolbarSet = "BizForm";
        htmlEditor.MediaDialogConfig.UseFullURL = true;
        htmlEditor.LinkDialogConfig.UseFullURL = true;
        htmlEditor.QuickInsertConfig.UseFullURL = true;
    }


    /// <summary>
    /// Save button is clicked.
    /// </summary>
    protected void lnkSave_Click(object sender, EventArgs e)
    {
        // Check 'EditForm' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.form", "EditForm"))
        {
            RedirectToCMSDeskAccessDenied("cms.form", "EditForm");
        }

        string errorMessage = null;

        BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(formId);
        if (bfi != null)
        {
            if (chkSendToEmail.Checked)
            {
                // Validate form
                errorMessage = new Validator().NotEmpty(txtFromEmail.Text, GetString("BizFormGeneral.EmptyFromEmail"))
                    .NotEmpty(txtToEmail.Text, GetString("BizFormGeneral.EmptyToEmail"))
                    .NotEmpty(txtSubject.Text, GetString("BizFormGeneral.EmptyEmailSubject")).Result;

                // Check if to e-mail contains macro expression or e-mails separated by semicolon
                if (string.IsNullOrEmpty(errorMessage) && !MacroResolver.ContainsMacro(txtToEmail.Text.Trim()) && !ValidationHelper.AreEmails(txtToEmail.Text.Trim()))
                {
                    errorMessage = GetString("BizFormGeneral.EmptyToEmail");
                }

                // Check if from e-mail contains macro expression or e-mails separated by semicolon
                if (string.IsNullOrEmpty(errorMessage) && !MacroResolver.ContainsMacro(txtFromEmail.Text.Trim()) && !ValidationHelper.IsEmail(txtFromEmail.Text.Trim()))
                {
                    errorMessage = GetString("BizFormGeneral.EmptyFromEmail");
                }

                if (string.IsNullOrEmpty(errorMessage))
                {
                    bfi.FormSendFromEmail = txtFromEmail.Text.Trim();
                    bfi.FormSendToEmail = txtToEmail.Text.Trim();
                    bfi.FormEmailSubject = txtSubject.Text.Trim();
                    bfi.FormEmailAttachUploadedDocs = chkAttachDocs.Checked;
                    if (chkCustomLayout.Checked)
                    {
                        bfi.FormEmailTemplate = htmlEditor.ResolvedValue.Trim();
                    }
                    else
                    {
                        bfi.FormEmailTemplate = null;
                    }
                }
            }
            else
            {
                bfi.FormSendFromEmail = null;
                bfi.FormSendToEmail = null;
                bfi.FormEmailSubject = null;
                bfi.FormEmailTemplate = null;
                txtToEmail.Text = string.Empty;
                txtFromEmail.Text = string.Empty;
                txtSubject.Text = string.Empty;
                chkAttachDocs.Checked = true;
                htmlEditor.ResolvedValue = string.Empty;
            }

            if (string.IsNullOrEmpty(errorMessage))
            {
                try
                {
                    BizFormInfoProvider.SetBizFormInfo(bfi);
                    ShowChangesSaved();
                    EnableDisableForm(bfi.FormEmailTemplate);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShowError(errorMessage);
            }
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
    // Get the editor instance that we want to interact with.
    var oEditor = CKEDITOR.instances['{0}'];

    // Check the active editing mode.
    if (oEditor.mode == 'wysiwyg') {{
        // Insert the desired HTML.
        oEditor.insertHtml(htmlString);
    }}
    else
        alert('You must be on WYSIWYG mode!');
}}

// Set content of the CK editor - replace the actual one
function SetContent(newContent) {{
    // Get the editor instance that we want to interact with.
    var oEditor = CKEDITOR.instances['{0}'];

    // Set the editor content (replace the actual one).
    oEditor.setData(newContent);
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
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "NotificationEmail_" + ClientID, script, true);
    }

    #endregion
}