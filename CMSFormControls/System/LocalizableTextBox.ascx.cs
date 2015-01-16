using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.ResourceManager;
using CMS.SettingsProvider;
using CMS.SiteProvider;

[ValidationPropertyAttribute("Value")]
public partial class CMSFormControls_System_LocalizableTextBox : FormEngineUserControl, ICallbackEventHandler
{
    #region "Macros and variables"

    /// <summary>
    /// Localization macro starts with '{$' characters.
    /// </summary>
    public const string MACRO_START = "{$";

    /// <summary>
    /// Localization macro starts with '$}' characters.
    /// </summary>
    public const string MACRO_END = "$}";

    /// <summary>
    /// In-place localization macro starts with '{$=' characters and should not be localized in localizable textbox!
    /// </summary>
    protected const string INPLACE_MACRO_START = "{$=";

    /// <summary>
    /// URL of field localization modal dialog.
    /// </summary>
    public const string LOCALIZE_FIELD = "~/CMSFormControls/Selectors/LocalizableTextBox/LocalizeField.aspx";

    /// <summary>
    /// URL of string localization modal dialog.
    /// </summary>
    public const string LOCALIZE_STRING = "~/CMSFormControls/Selectors/LocalizableTextBox/LocalizeString.aspx";

    /// <summary>
    /// Default prefix for keys created in development mode.
    /// </summary>
    private const string PREFIX = "test.";

    /// <summary>
    /// Maximum resource string key length.
    /// </summary>
    private const int MAX_KEY_LENGTH = 200;

    /// <summary>
    /// Default button style.
    /// </summary>
    private const string BUTTON_STYLE = "background-image: url('{0}');";

    // Indicates if changes to resource string should be performed immediately after each PostBack.
    private bool mAutoSave = true;

    // Value returned by callback
    private string callbackReturnVal = null;

    // Resource key prefix
    private string mResourceKeyPrefix = String.Empty;
    private bool prefixIsSet = false;
    private bool? mLocalizationExists = null;

    private bool mSaveAutomatically = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets whether the control is read only
    /// </summary>
    public bool ReadOnly
    {
        get
        {
            return textbox.ReadOnly;
        }
        set
        {
            textbox.ReadOnly = value;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            if (textbox != null)
            {
                textbox.Enabled = value;
                btnLocalize.Enabled = value;
                btnOtherLanguages.Enabled = value;
                btnRemoveLocalization.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Return macro contained in hidden field if text is macro
            if (IsMacro)
            {
                if (LocalizationExists)
                {
                    return hdnValue.Value;
                }
                else
                {
                    return MACRO_START + textbox.Text + MACRO_END;
                }
            }
            // Return plain text contained in textbox
            else
            {
                return textbox.Text;
            }
        }
        set
        {
            if (mSaveAutomatically)
            {
                // Save if configured to save automatically
                Save();

                mSaveAutomatically = false;
            }

            string valueStr = hdnValue.Value = ValidationHelper.GetString(value, null);

            // Check if value is localization macro
            if (!String.IsNullOrEmpty(valueStr) && !valueStr.StartsWithCSafe(INPLACE_MACRO_START) && valueStr.StartsWithCSafe(MACRO_START) && valueStr.EndsWithCSafe(MACRO_END))
            {
                IsMacro = true;
                if (!RequestHelper.IsPostBack())
                {
                    textbox.Text = ResHelper.LocalizeString(valueStr);
                }
            }
            // Value is plain text
            else
            {
                IsMacro = false;
                textbox.Text = valueStr;

                if (!string.IsNullOrEmpty(valueStr) && pnlButtons.Visible)
                {
                    // Hide localization buttons if in-place macro is edited
                    pnlButtons.Visible = !valueStr.StartsWithCSafe(INPLACE_MACRO_START);
                }
            }
        }
    }


    /// <summary>
    /// Returns client ID of the textbox.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return textbox.ClientID;
        }
    }


    /// <summary>
    /// Gets value indicating if localization of key exists.
    /// </summary>
    private bool LocalizationExists
    {
        get
        {
            // Determine if translation exists
            if (mLocalizationExists == null)
            {
                bool translationFound = false;
                ResHelper.LocalizeString(hdnValue.Value, out translationFound);
                mLocalizationExists = translationFound;
            }
            if (mLocalizationExists == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    /// <summary>
    /// Publicly visible textbox which contains translated string or plain text.
    /// </summary>
    public TextBox TextBox
    {
        get
        {
            return textbox;
        }
    }


    /// <summary>
    /// TextMode of textbox.
    /// </summary>
    public TextBoxMode TextMode
    {
        get
        {
            return textbox.TextMode;
        }
        set
        {
            textbox.TextMode = value;
        }
    }


    /// <summary>
    /// Number of columns of textbox in multiline mode.
    /// </summary>
    public int Columns
    {
        get
        {
            return textbox.Columns;
        }
        set
        {
            textbox.Columns = value;
        }
    }


    /// <summary>
    /// Number of rows of textbox in multiline mode.
    /// </summary>
    public int Rows
    {
        get
        {
            return textbox.Rows;
        }
        set
        {
            textbox.Rows = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if control should save changes to resource string immediately after each PostBack. Default true.
    /// </summary>
    public bool AutoSave
    {
        get
        {
            return mAutoSave;
        }
        set
        {
            mAutoSave = value;
        }
    }


    /// <summary>
    /// Indicates if text contained in textbox is resolved resource string or if it is just plain text.
    /// </summary>
    private bool IsMacro
    {
        get
        {
            return ValidationHelper.GetBoolean(hdnIsMacro.Value, false);
        }
        set
        {
            hdnIsMacro.Value = value.ToString();
        }
    }


    /// <summary>
    /// Modal dialog identifier.
    /// </summary>
    private string Identifier
    {
        get
        {
            // Try to load data from control ViewState
            string identifier = ValidationHelper.GetString(Request.Params[hdnIdentifier.UniqueID], String.Empty);

            // Create new GUID for identifier
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = Guid.NewGuid().ToString();
            }

            // Assign identifier to hidden field
            if (String.IsNullOrEmpty(hdnIdentifier.Value))
            {
                hdnIdentifier.Value = identifier;
            }

            return identifier;
        }
    }


    /// <summary>
    /// Maximum length of plain text or resource string key. Validates in IsValid() method.
    /// </summary>
    public int MaxLength
    {
        get
        {
            return textbox.MaxLength;
        }
        set
        {
            textbox.MaxLength = value;
        }
    }


    /// <summary>
    /// Resource key prefix. Default value is empty for DevelopmentMode, 'custom.' value otherwise.
    /// </summary>
    public string ResourceKeyPrefix
    {
        get
        {
            // If user set prefix
            if (prefixIsSet)
            {
                return mResourceKeyPrefix;
            }
            // If in DevelopmentMode
            else if (SettingsKeyProvider.DevelopmentMode)
            {
                return "test.";
            }
            // Otherwise return "custom."
            else
            {
                return "custom.";
            }
        }
        set
        {
            mResourceKeyPrefix = value;
            prefixIsSet = true;
        }
    }


    /// <summary>
    /// If TRUE then resource string key selection is skipped. Instead resource string key is automaticaly created from entered text.
    /// Also when removing localization it also deletes resource string key assigned.
    /// </summary>
    public bool AutomaticMode
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if control is used on live site. Default value is FALSE for localizable text box.
    /// </summary>
    public override bool IsLiveSite
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set controls
        btnOtherLanguages.ToolTip = GetString("localizable.otherlanguages");
        btnOtherLanguages.OnClientClick = "LocalizeString('" + hdnValue.ClientID + "', '" + textbox.ClientID + "'); return false;";
        btnLocalize.ToolTip = GetString("localizable.localize");
        btnRemoveLocalization.Click += btnRemoveLocalization_Click;
        btnRemoveLocalization.ToolTip = GetString("localizable.remove");
        string buttonScript =
@"var confirmValue = confirm('" + GetString("localizable.removelocalization") + @"');
if (!confirmValue) return false;";
        btnRemoveLocalization.OnClientClick = buttonScript;

        // In automatic mode resource string key is generated from plain text
        if (AutomaticMode)
        {
            btnLocalize.Click += btnLocalize_Click;
        }
        // Otherwise user has option to select resource string key
        else
        {
            btnLocalize.OnClientClick = "LocalizationDialog" + ClientID + "('$|' + document.getElementById('" + textbox.ClientID + "').value); return false;";
        }

        if (textbox.TextMode == TextBoxMode.MultiLine)
        {
            pnlButtons.CssClass = "LocalizablePanel LocalizableTop";
        }

        // Show/hide localization controls
        pnlButtons.Visible =
            CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Localization", "LocalizeStrings") &&
            !IsLiveSite &&
            (SettingsKeyProvider.DevelopmentMode || (UICultureInfoProvider.NumberOfUICultures > 1)) &&
            !textbox.Text.StartsWithCSafe(INPLACE_MACRO_START);

        // Apply CSS style
        if (!String.IsNullOrEmpty(CssClass))
        {
            textbox.CssClass = CssClass;
            CssClass = null;
        }

        // Register event handler for saving data in BasicForm
        if (Form != null)
        {
            Form.OnAfterSave += Form_OnAfterSave;
        }
        // Save changes after each PostBack if set so
        else if (RequestHelper.IsPostBack() && Visible && AutoSave && IsMacro && !String.IsNullOrEmpty(textbox.Text.Trim()))
        {
            mSaveAutomatically = true;
        }
        SetClientSideMaxLength();
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (mSaveAutomatically)
        {
            Save();
        }
                
        // Ensure the text in textbox
        if (IsMacro && RequestHelper.IsPostBack())
        {
            if (LocalizationExists)
            {
                textbox.Text = ResHelper.LocalizeString(hdnValue.Value);
            }
            else
            {
                textbox.Text = hdnValue.Value.Substring(MACRO_START.Length, hdnValue.Value.Length - (MACRO_END.Length + MACRO_START.Length));
            }
        }

        Reload();

        // Ensure Enabled property so all inner controls are set properly
        Enabled = Enabled;

        // Register the scripts
        if (pnlButtons.Visible)
        {
            RegisterScripts();
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Validates control.
    /// </summary>
    public override bool IsValid()
    {
        // Check for maximum length
        if (textbox.MaxLength > 0)
        {
            return (hdnValue.Value.Length <= MaxLength) && (textbox.Text.Length <= MaxLength);
        }
        else
        {
            return true;
        }
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void Reload()
    {
        if (pnlButtons.Visible)
        {
            string iconLocalize = GetImageUrl("/Objects/CMS_UICulture/add.png");
            string iconOther = GetImageUrl("Objects/CMS_UICulture/list.png");
            string iconRemove = GetImageUrl("Design/Controls/UniGrid/Actions/Delete.png");

            // Textbox contains translated macro
            if (IsMacro && LocalizationExists)
            {
                btnLocalize.Attributes.Add("style", string.Format(BUTTON_STYLE, iconLocalize) + "display: none;");
                btnOtherLanguages.Attributes.Add("style", string.Format(BUTTON_STYLE, iconOther) + "display: inline;");
                btnRemoveLocalization.Attributes.Add("style", string.Format(BUTTON_STYLE, iconRemove) + "display: inline;");

                if (!this.textbox.CssClass.EndsWith(" TextBoxLocalized"))
                {
                    this.textbox.CssClass += " TextBoxLocalized";
                }
                this.textbox.ToolTip = GetString("localizable.localized");
            }
            // Textbox contains only plain text
            else
            {
                if (this.textbox.CssClass.EndsWith(" TextBoxLocalized"))
                {
                    this.textbox.CssClass = this.textbox.CssClass.Substring(0, this.textbox.CssClass.Length - 17);
                }

                btnOtherLanguages.Attributes.Add("style", string.Format(BUTTON_STYLE, iconOther) + "display: none;");
                btnRemoveLocalization.Attributes.Add("style", string.Format(BUTTON_STYLE, iconRemove) + "display: none;");
                btnLocalize.Attributes.Add("style", string.Format(BUTTON_STYLE, iconLocalize) + "display:inline;");
            }
        }
    }


    /// <summary>
    /// Registers JS.
    /// </summary>
    private void RegisterScripts()
    {
        // Register function to set translation string key from dialog window
        ScriptHelper.RegisterDialogScript(Page);
        StringBuilder script = new StringBuilder();
        script.Append(
            @"
function Get(id) {
    return document.getElementById(id);
}

function SetResource(hdnValue, value, textbox, textboxvalue, hdnIsMacro, btnLocalizeField, btnLocalizeString, btnRemoveLocalization) {
    SetProperties(hdnValue, value, textbox, textboxvalue, hdnIsMacro, btnLocalizeField, btnLocalizeString, btnRemoveLocalization);
    return false;
}

function SetResourceAndOpen(hdnValue, value, textbox, textboxvalue, hdnIsMacro, btnLocalizeField, btnLocalizeString, btnRemoveLocalization) { 
    SetResource(hdnValue, value, textbox, textboxvalue, hdnIsMacro, btnLocalizeField, btnLocalizeString, btnRemoveLocalization);
    OpenLocalize(hdnValue, textbox);
    return false;
}

function SetProperties(hdnValue, value, textbox, textboxvalue, hdnIsMacro, btnLocalizeField, btnLocalizeString, btnRemoveLocalization) {
    Get(hdnValue).value = '", MACRO_START, @"' + value + '", MACRO_END, @"';
    var txt = Get(textbox);
    txt.value = textboxvalue;
    txt.className += ' TextBoxLocalized';
    Get(hdnIsMacro).value = 'true';
    Get(btnLocalizeField).style.display='none';
    Get(btnLocalizeString).style.display='inline';
    Get(btnRemoveLocalization).style.display='inline';
    return false;
}

function OpenLocalize(hdnValue, textbox) {
    window.setTimeout(function() { LocalizeString(hdnValue, textbox); }, 1);
    return false
}

function SetTranslation(textbox, textboxvalue, hdnValue, value) {
    Get(textbox).value = textboxvalue;
    Get(hdnValue).value = '", MACRO_START, @"' + value + '", MACRO_END, @"';
}

function LocalizeFieldReady(rvalue, context) {
    modalDialog(context, 'localizableField', 500, 350, null, null, true); 
    return false;
}

function LocalizeString(hiddenValueControl, textboxControl) {
    var stringKey = Get(hiddenValueControl).value;
    stringKey = stringKey.substring(", MACRO_START.Length, @",stringKey.length-", MACRO_END.Length, @");
    modalDialog('", ResolveUrl(LOCALIZE_STRING), @"?hiddenValueControl=' + hiddenValueControl + '&stringKey=' + escape(stringKey) + '&parentTextbox=' + textboxControl, 'localizableString', 600, 635, null, null, true);
    return false;
}
");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "LocalizationDialogFunction", ScriptHelper.GetScript(script.ToString()));

        // Register callback to send current plain text to modal window for localization
        string url = LOCALIZE_FIELD + "?params=" + Identifier;
        url += "&hash=" + QueryHelper.GetHash(url, false);

        script = new StringBuilder();
        script.Append(
            @"
function LocalizationDialog", ClientID, @"(values) {
    ", Page.ClientScript.GetCallbackEventReference(this, "values", "LocalizeFieldReady", "'" + ResolveUrl(url) + "'"), @"
}
"
            );

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "LocalizationDialog" + ClientID, ScriptHelper.GetScript(script.ToString()));
    }


    /// <summary>
    /// Saves translation for given resource string.
    /// </summary>
    /// <returns>Returns TRUE if resource string was successfully modified</returns>
    public bool Save()
    {
        // Save changes only when macro is edited
        if (IsMacro)
        {
            if (!textbox.Text.Trim().StartsWithCSafe(INPLACE_MACRO_START))
            {
                // Update key
                string key = hdnValue.Value.Substring(MACRO_START.Length, hdnValue.Value.Length - (MACRO_END.Length + MACRO_START.Length));
                var ri = SqlResourceManager.GetResourceStringInfo(key) ?? new ResourceStringInfo { StringKey = key };

                ri.TranslationText = textbox.Text.Trim();
                if (SqlResourceManager.GetUICultureID(CultureHelper.PreferredUICulture) != 0)
                {
                    ri.UICultureCode = CultureHelper.PreferredUICulture;
                }
                else
                {
                    ri.UICultureCode = CultureHelper.DefaultUICulture;
                }
                SqlResourceManager.SetResourceStringInfo(ri);
                return true;
            }
            else
            {
                // Remove localization if in-place macro was inserted
                RemoveLocalization();
            }
        }
        return false;
    }


    /// <summary>
    /// Removes localization from the textbox.
    /// </summary>
    private void RemoveLocalization()
    {
        // In automatic mode remove resource string key 
        if (AutomaticMode)
        {
            string key = hdnValue.Value.Substring(MACRO_START.Length, hdnValue.Value.Length - (MACRO_END.Length + MACRO_START.Length));
            SqlResourceManager.DeleteResourceStringInfo(key, CultureHelper.DefaultUICulture);
        }

        hdnValue.Value = textbox.Text;
        IsMacro = false;
    }


    /// <summary>
    /// Sets the dialog parameters to the context.
    /// </summary>
    protected void SetFieldDialog(string textboxValue)
    {
        Hashtable parameters = new Hashtable();
        parameters["TextBoxValue"] = textboxValue;
        parameters["HiddenValue"] = hdnValue.ClientID;
        parameters["TextBoxID"] = textbox.ClientID;
        parameters["HiddenIsMacro"] = hdnIsMacro.ClientID;
        parameters["ButtonLocalizeField"] = btnLocalize.ClientID;
        parameters["ButtonLocalizeString"] = btnOtherLanguages.ClientID;
        parameters["ButtonRemoveLocalization"] = btnRemoveLocalization.ClientID;
        parameters["ResourceKeyPrefix"] = ResourceKeyPrefix;

        WindowHelper.Add(Identifier, parameters);
    }


    /// <summary>
    /// Sets client-side max length of the textbox control.
    /// </summary>
    private void SetClientSideMaxLength()
    {
        if (FieldInfo != null)
        {
            switch (FieldInfo.DataType)
            {
                case (FormFieldDataTypeEnum.Text):
                    textbox.MaxLength = FieldInfo.Size;
                    break;

                case (FormFieldDataTypeEnum.Integer):
                case (FormFieldDataTypeEnum.LongInteger):
                    if (string.IsNullOrEmpty(FieldInfo.MaxValue) || string.IsNullOrEmpty(FieldInfo.MinValue))
                    {
                        // One of the limit value is not set => set maxint/maxlong length
                        textbox.MaxLength = (FieldInfo.DataType == FormFieldDataTypeEnum.Integer) ?
                                                                                                      ValidationHelper.MAX_INT_LENGTH :
                                                                                                                                          ValidationHelper.MAX_LONGINT_LENGTH;
                    }
                    else
                    {
                        // Set maxlength to the bigger one
                        textbox.MaxLength = Math.Max(FieldInfo.MaxValue.Length, FieldInfo.MinValue.Length);
                    }
                    break;
            }
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Button localize click. In AutomaticMode available only.
    /// </summary>
    void btnLocalize_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(textbox.Text.Trim()))
        {
            // Get maximum length of resource key
            int maxKeyLength = MAX_KEY_LENGTH;
            string prefix = ResourceKeyPrefix;
            if (SettingsKeyProvider.DevelopmentMode && String.IsNullOrEmpty(ResourceKeyPrefix))
            {
                prefix = PREFIX;
            }

            if (!String.IsNullOrEmpty(prefix))
            {
                maxKeyLength -= prefix.Length;
            }

            // Initialize resource string
            string newResource = TextHelper.LimitLength(ValidationHelper.GetCodeName(textbox.Text.Trim()), maxKeyLength, String.Empty, true);

            int i = 0;
            if (!newResource.StartsWithCSafe(prefix))
            {
                hdnValue.Value = prefix + newResource;
            }
            else
            {
                hdnValue.Value = newResource;
            }
            // If key exists then create new one with number as a suffix
            while (SqlResourceManager.GetResourceStringInfo(hdnValue.Value) != null)
            {
                // If newly created resource key is longer then allowed length then trim end by one character
                if ((prefix.Length + newResource.Length + ++i) > MAX_KEY_LENGTH)
                {
                    newResource = newResource.Substring(0, newResource.Length - 1);
                }

                if (!newResource.StartsWithCSafe(prefix))
                {
                    hdnValue.Value = prefix + newResource + i;
                }
                else
                {
                    hdnValue.Value = newResource + i;
                }
            }

            // Check if current user's culture exists
            UICultureInfo uiCulture = null;
            string cultureCode = CultureHelper.PreferredUICulture;
            try
            {
                uiCulture = UICultureInfoProvider.GetUICultureInfo(CultureHelper.PreferredUICulture);
            }
            // Use default UI culture
            catch
            {
                cultureCode = CultureHelper.DefaultUICulture;
            }
            // Use default UI culture
            if (uiCulture == null)
            {
                cultureCode = CultureHelper.DefaultUICulture;
            }

            // Save ResourceString
            ResourceStringInfo ri = new ResourceStringInfo();
            ri.StringKey = hdnValue.Value;
            ri.UICultureCode = cultureCode;
            ri.TranslationText = textbox.Text;
            ri.StringIsCustom = !SettingsKeyProvider.DevelopmentMode;
            SqlResourceManager.SetResourceStringInfo(ri);

            // Open 'localization to other languages' window
            ScriptHelper.RegisterStartupScript(this, typeof(string), "OpenLocalization", ScriptHelper.GetScript("modalDialog('" + ResolveUrl(LOCALIZE_STRING) + "?hiddenValueControl=" + hdnValue.ClientID + "&stringKey=" + ri.StringKey + "&parentTextbox=" + textbox.ClientID + "', 'localizableString', 600, 635, null, null, true);"));

            // Set macro settings
            Value = MACRO_START + hdnValue.Value + MACRO_END;
            Reload();
        }
        else
        {
            lblError.Visible = true;
            lblError.ResourceString = "localize.entertext";
        }
    }


    /// <summary>
    /// Remove localization button click.
    /// </summary>
    private void btnRemoveLocalization_Click(object sender, EventArgs e)
    {
        RemoveLocalization();
    }


    /// <summary>
    /// BasicForm saved event handler.
    /// </summary>
    private void Form_OnAfterSave(object sender, EventArgs e)
    {
        Save();
    }


    /// <summary>
    /// Gets callback result.
    /// </summary>
    string ICallbackEventHandler.GetCallbackResult()
    {
        // Prepare the parameters for dialog
        SetFieldDialog(callbackReturnVal);
        return "";
    }


    /// <summary>
    /// Raise callback event.
    /// </summary>
    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        // Get value from callback
        if ((eventArgument != null) && eventArgument.StartsWithCSafe("$|"))
        {
            callbackReturnVal = eventArgument.Substring(2);
        }
    }

    #endregion
}