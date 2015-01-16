using System;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.TranslationServices;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_DatabaseConfiguration : CMSUserControl
{
    #region "Events"

    /// <summary>
    /// Event raised when drop-down list is changed.
    /// </summary>
    public event EventHandler DropChanged;


    /// <summary>
    /// Event raised when attribute is changed.
    /// </summary>
    public event EventHandler AttributeChanged;

    #endregion


    #region "Properties"

    /// <summary>
    /// Field editor mode.
    /// </summary>
    public FieldEditorModeEnum Mode
    {
        get;
        set;
    }


    /// <summary>
    /// FormFieldInfo of given field.
    /// </summary>
    public FormFieldInfo FieldInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value indicating if current field is primary.
    /// </summary>
    public bool IsFieldPrimary
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value indicating if System fields should be enabled.
    /// </summary>
    public bool EnableSystemFields
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value indicating if System fields should be displayed.
    /// </summary>
    public bool ShowSystemFields
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets attribute name.
    /// </summary>
    public string AttributeName
    {
        get
        {
            return txtAttributeName.Text.Trim();
        }
        set
        {
            txtAttributeName.Text = value;
        }
    }


    /// <summary>
    /// Gets value indicating new item is being edited.
    /// </summary>
    public bool IsNewItemEdited
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsNewItemEdited"], false);
        }
        set
        {
            ViewState["IsNewItemEdited"] = value;
        }
    }


    /// <summary>
    /// Indicates if document type is edited.
    /// </summary>
    public bool IsDocumentType
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if field editor works in development mode.
    /// </summary>
    public bool DevelopmentMode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value which should be selected in group selector.
    /// </summary>
    public string GroupValue
    {
        get
        {
            return drpGroup.SelectedValue;
        }
    }


    /// <summary>
    /// Gets or sets value which should be selected in system fields selector.
    /// </summary>
    public string SystemValue
    {
        get
        {
            return drpSystemFields.SelectedValue;
        }
    }


    /// <summary>
    /// Returns True if system fields are enabled and one of them is selected.
    /// </summary>
    public bool IsSystemFieldSelected
    {
        get
        {
            return (EnableSystemFields && plcGroup.Visible);
        }
    }


    /// <summary>
    /// Returns text value from Attribute size textbox.
    /// </summary>
    public string AttributeSize
    {
        get
        {
            return txtAttributeSize.Text.Trim();
        }
    }


    /// <summary>
    /// Returns text value from Default value textbox.
    /// </summary>
    public string DefaultValueText
    {
        get
        {
            return txtDefaultValue.Text.Trim();
        }
    }


    /// <summary>
    /// Returns text value from Large Default value text area.
    /// </summary>
    public string LargeDefaultValueText
    {
        get
        {
            return txtLargeDefaultValue.Value.ToString().Trim();
        }
    }


    /// <summary>
    /// Gets value which is contained in default Date time picker.
    /// </summary>
    public string DefaultDateTime
    {
        get
        {
            return datetimeDefaultValue.DateTimeTextBox.Text.Trim();
        }
    }


    /// <summary>
    /// Gets value which indicates if checkbox Default value is checked.
    /// </summary>
    public bool DefaultBoolValue
    {
        get
        {
            return chkDefaultValue.Checked;
        }
    }

    /// <summary>
    /// Gets or sets selected attribute type.
    /// </summary>
    public string AttributeType
    {
        get
        {
            return drpAttributeType.SelectedValue.ToLowerCSafe();
        }
        set
        {
            if (!String.IsNullOrEmpty(value))
            {
                drpAttributeType.SelectedValue = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets value indicating if field allows empty values.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return chkAllowEmpty.Checked;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if field is system field.
    /// </summary>
    public bool IsSystem
    {
        get
        {
            return chkIsSystem.Checked;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if a field is included to translation services export.
    /// </summary>
    public bool TranslateField
    {
        get
        {
            return chkTranslateField.Checked && plcTranslation.Visible;
        }
    }


    /// <summary>
    /// Indicates if Field Editor is used as alternative form.
    /// </summary>
    public bool IsAlternativeForm
    {
        get;
        set;
    }


    /// <summary>
    /// Class name.
    /// </summary>
    public string ClassName
    {
        get;
        set;
    }


    /// <summary>
    /// Coupled class name.
    /// </summary>
    public string CoupledClassName
    {
        get;
        set;
    }

    #endregion


    #region "Public methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        pnlDatabase.GroupingText = GetString("templatedesigner.section.database");
    }


    /// <summary>
    /// Loads group drop-down list with data.
    /// </summary>
    public void LoadGroupField()
    {
        // Set field types
        if (EnableSystemFields && (drpGroup.Items.Count == 0))
        {
            drpGroup.Items.Add(new ListItem(GetString("TemplateDesigner.DocumentAttributes"), "cms_document"));
            drpGroup.Items.Add(new ListItem(GetString("TemplateDesigner.NodeAttributes"), "cms_tree"));
        }
    }


    /// <summary>
    /// Loads field with values from FormFieldInfo object.
    /// </summary>
    public void Reload(string attributeName, bool enableSystemDDL)
    {
        if (EnableSystemFields && ShowSystemFields)
        {
            plcGroup.Visible = true;
            drpSystemFields.Visible = true;
            txtAttributeName.Visible = false;
            drpGroup.Enabled = enableSystemDDL;
            drpSystemFields.Enabled = enableSystemDDL;

            // Get system table name
            string tableName;
            if ((string.IsNullOrEmpty(attributeName)) || (attributeName.ToLowerCSafe().StartsWithCSafe("document")))
            {
                // Fields from table CMS_Document
                tableName = "cms_document";
            }
            else
            {
                // Fields from table CMS_Node
                tableName = "cms_tree";
            }
            drpGroup.SelectedValue = tableName;

            // Select system attribute name in dropdownlist
            SetSystemAttributeName(tableName, attributeName);
        }
        else
        {
            // Show textbox only     
            plcGroup.Visible = false;
            drpSystemFields.Visible = false;
            txtAttributeName.Visible = true;
            txtAttributeName.Text = AttributeName;
        }

        LoadAttributeTypes();

        if (FieldInfo != null)
        {
            if (DevelopmentMode)
            {
                lblGuidValue.Text = FieldInfo.Guid.ToString();
            }

            txtAttributeName.Text = FieldInfo.Name;
            txtAttributeSize.Text = (FieldInfo.Size == 0) ? "" : Convert.ToString(FieldInfo.Size);

            // Select attribute type
            string selectedItem = FormHelper.GetFormFieldDataTypeString(FieldInfo.DataType);
            if ((FieldInfo.DataType == FormFieldDataTypeEnum.Text) && (FieldInfo.Size == 500) && ((Mode == FieldEditorModeEnum.BizFormDefinition) || (Mode == FieldEditorModeEnum.SystemTable))
                && IsFileControl())
            {
                selectedItem = FormFieldDataTypeCode.FILE;
            }
            if (drpAttributeType.Items.FindByValue(selectedItem) != null)
            {
                drpAttributeType.SelectedValue = selectedItem;
            }
            chkAllowEmpty.Checked = FieldInfo.AllowEmpty;
            chkIsSystem.Checked = (FieldInfo.System || (FieldInfo.PrimaryKey && !IsDocumentType));
            plcIsSystem.Visible = DevelopmentMode;
            chkTranslateField.Checked = FieldInfo.TranslateField;

            SetDefaultValue();
            ShowDefaultControl();
        }
        // Clear form in FormFieldInfo not specified
        else
        {
            lblGuidValue.Text = null;
            drpAttributeType.SelectedValue = null;
            txtAttributeName.Text = null;
            txtDefaultValue.Text = "";
            txtLargeDefaultValue.Value = "";
            txtAttributeSize.Text = null;
            chkDefaultValue.Checked = false;
            chkAllowEmpty.Checked = (Mode == FieldEditorModeEnum.SystemTable);
            chkIsSystem.Checked = DevelopmentMode && !IsDocumentType;
            datetimeDefaultValue.SelectedDateTime = DateTimePicker.NOT_SELECTED;
            plcIsSystem.Visible = DevelopmentMode;
            chkTranslateField.Checked = false;
        }

        // If key is primary, disable "Allow null" checkbox
        if (IsFieldPrimary)
        {
            chkAllowEmpty.Enabled = false;
            chkAllowEmpty.Checked = false;
        }

        // Primary key not allowed to change
        // System field not allowed to change unless development mode
        if ((FieldInfo != null) && (FieldInfo.PrimaryKey || (FieldInfo.System && !EnableSystemFields)))
        {
            bool enableDefault = ((Mode != FieldEditorModeEnum.SystemTable) || (IsAlternativeForm)) && !FieldInfo.PrimaryKey;
            DisableFieldEditing(FieldInfo.External, enableDefault);
        }
        else
        {
            EnableFieldEditing();
            EnableDisableSections();
        }

        chkIsSystem.Enabled &= !IsDocumentType;
        EnableOrDisableAttributeSize();
    }


    /// <summary>
    /// Sets default value according to attribute type.
    /// </summary>
    public void SetDefaultValue()
    {
        txtDefaultValue.Text = "";
        txtLargeDefaultValue.Value = "";
        chkDefaultValue.Checked = false;
        datetimeDefaultValue.SelectedDateTime = DateTimePicker.NOT_SELECTED;

        if (FieldInfo != null)
        {
            switch (AttributeType)
            {
                case FormFieldDataTypeCode.DATETIME:
                    if (string.IsNullOrEmpty(FieldInfo.DefaultValue))
                    {
                        datetimeDefaultValue.DateTimeTextBox.Text = "";
                    }
                    else if (FieldInfo.DefaultValue.ToLowerCSafe() == DateTimePicker.DATE_TODAY.ToLowerCSafe())
                    {
                        datetimeDefaultValue.DateTimeTextBox.Text = DateTimePicker.DATE_TODAY;
                    }
                    else if (FieldInfo.DefaultValue.ToLowerCSafe() == DateTimePicker.TIME_NOW.ToLowerCSafe())
                    {
                        datetimeDefaultValue.DateTimeTextBox.Text = DateTimePicker.TIME_NOW;
                    }
                    else if (ValidationHelper.IsMacro(FieldInfo.DefaultValue))
                    {
                        datetimeDefaultValue.DateTimeTextBox.Text = FieldInfo.DefaultValue;
                    }
                    else
                    {
                        datetimeDefaultValue.SelectedDateTime = FormHelper.GetDateTimeValueInCurrentCulture(FieldInfo.DefaultValue);
                    }
                    break;

                case FormFieldDataTypeCode.BOOLEAN:
                    chkDefaultValue.Checked = ValidationHelper.GetBoolean(FieldInfo.DefaultValue, false);
                    break;

                case FormFieldDataTypeCode.LONGTEXT:
                    txtLargeDefaultValue.Value = FieldInfo.DefaultValue;
                    break;

                case FormFieldDataTypeCode.DOUBLE:
                    if (ValidationHelper.IsMacro(FieldInfo.DefaultValue))
                    {
                        txtDefaultValue.Text = FieldInfo.DefaultValue;
                    }
                    else
                    {
                        txtDefaultValue.Text = FormHelper.GetDoubleValueInCurrentCulture(FieldInfo.DefaultValue);
                    }
                    break;

                default:
                    txtDefaultValue.Text = FieldInfo.DefaultValue;
                    break;
            }
        }
    }


    /// <summary>
    /// Returns attribute name.
    /// </summary>    
    public string GetAttributeName()
    {
        if (drpSystemFields.Visible)
        {
            return drpSystemFields.SelectedValue;
        }
        return txtAttributeName.Text.Trim();
    }


    /// <summary>
    /// Enable or disable size attribute.
    /// </summary>
    public void EnableOrDisableAttributeSize()
    {
        if (AttributeType == "text")
        {
            if (IsAlternativeForm || IsSystemFieldSelected)
            {
                txtAttributeSize.Enabled = false;
            }
            else
            {
                txtAttributeSize.Enabled = true;
            }
            txtAttributeSize.MaxLength = 9;
        }
        else
        {
            txtAttributeSize.Enabled = false;
            txtAttributeSize.Text = "";
        }

        if (AttributeType == "docattachments")
        {
            chkAllowEmpty.Checked = true;
        }
        chkAllowEmpty.Enabled = ((AttributeType != "docattachments") && (((Mode == FieldEditorModeEnum.SystemTable) && IsAlternativeForm) || (Mode != FieldEditorModeEnum.SystemTable)));

        if (AttributeType == "binary")
        {
            chkAllowEmpty.Checked = true;
            chkAllowEmpty.Enabled = false;
            txtDefaultValue.Enabled = false;
        }

        if (IsAlternativeForm && !GetFormAllowEmpty())
        {
            chkAllowEmpty.Enabled = false;
        }
    }


    /// <summary>
    /// Disables field editing controls.
    /// </summary>
    /// <param name="enableName">Indicates if field name should remain enabled</param>
    /// <param name="enableDefault">Indicates if control for default value should remain enabled</param>
    public void DisableFieldEditing(bool enableName, bool enableDefault)
    {
        txtDefaultValue.Enabled = enableDefault;
        txtLargeDefaultValue.Enabled = enableDefault;
        chkDefaultValue.Enabled = enableDefault;
        datetimeDefaultValue.Enabled = enableDefault;
        drpAttributeType.Enabled = false;
        chkAllowEmpty.Enabled = false;
        EnableDisableAttributeName(enableName);
        chkIsSystem.Enabled = false;
        txtAttributeSize.Enabled = false;
    }


    /// <summary>
    /// Enables field editing controls, except field name.
    /// </summary>
    public void EnableFieldEditing()
    {
        txtDefaultValue.Enabled = true;
        txtLargeDefaultValue.Enabled = true;
        chkDefaultValue.Enabled = true;
        datetimeDefaultValue.Enabled = true;
        drpAttributeType.Enabled = true;
        chkAllowEmpty.Enabled = (Mode != FieldEditorModeEnum.SystemTable) && !IsAlternativeForm;
        chkIsSystem.Enabled = true;
        EnableDisableAttributeName(true);
    }


    /// <summary>
    /// Show default value control according to attribute type.
    /// </summary>
    public void ShowDefaultControl()
    {
        plcDefaultValue.Visible = true;
        SetFieldForTranslations();

        switch (AttributeType)
        {
            case FormFieldDataTypeCode.DATETIME:
                datetimeDefaultValue.Visible = true;
                chkDefaultValue.Visible = false;
                txtLargeDefaultValue.Visible = false;
                txtDefaultValue.Visible = false;
                break;

            case FormFieldDataTypeCode.BOOLEAN:
                chkDefaultValue.Visible = true;
                txtLargeDefaultValue.Visible = false;
                txtDefaultValue.Visible = false;
                datetimeDefaultValue.Visible = false;
                break;

            case FormFieldDataTypeCode.LONGTEXT:
                txtLargeDefaultValue.Visible = true;
                chkDefaultValue.Visible = false;
                txtDefaultValue.Visible = false;
                datetimeDefaultValue.Visible = false;
                break;

            case FormFieldDataTypeCode.FILE:
                // Hide default value for File-type fields within Document types
                if (Mode == FieldEditorModeEnum.ClassFormDefinition)
                {
                    plcDefaultValue.Visible = false;
                }
                // Display textbox otherwise
                else
                {
                    txtDefaultValue.Visible = true;
                    chkDefaultValue.Visible = false;
                    txtLargeDefaultValue.Visible = false;
                    datetimeDefaultValue.Visible = false;
                }
                break;

            default:
                txtDefaultValue.Visible = true;
                chkDefaultValue.Visible = false;
                txtLargeDefaultValue.Visible = false;
                datetimeDefaultValue.Visible = false;
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Fill attribute types list.
    /// </summary>
    private void LoadAttributeTypes()
    {
        if (drpAttributeType.Items.Count > 0)
        {
            return;
        }

        drpAttributeType.DataSource = FormHelper.GetFieldTypes(IsDocumentType, DevelopmentMode);
        drpAttributeType.DataBind();

        // Remove file type when editing webpart properties 
        if ((Mode == FieldEditorModeEnum.WebPartProperties) || (Mode == FieldEditorModeEnum.CustomTable) || (Mode == FieldEditorModeEnum.SystemTable) || (Mode == FieldEditorModeEnum.FormControls))
        {
            ListItem item = drpAttributeType.Items.FindByValue(FormFieldDataTypeCode.FILE);
            if (item != null)
            {
                drpAttributeType.Items.Remove(item);
            }
        }

        if (drpAttributeType.Items.Count > 0)
        {
            drpAttributeType.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// Sets system attribute name.
    /// </summary>
    private void SetSystemAttributeName(string tableName, string attributeName)
    {
        TableManager tm = new TableManager(null);

        // Load system fields from specified table
        drpSystemFields.DataSource = tm.GetColumnInformation(tableName);
        drpSystemFields.DataBind();

        try
        {
            drpSystemFields.SelectedValue = attributeName;
        }
        catch
        {
            if (drpSystemFields.Items.Count > 0)
            {
                drpSystemFields.SelectedIndex = 0;
            }
        }
    }


    /// <summary>
    /// Enable or disable field for translation according to actual Attribute Type (from DropDown list)
    /// </summary>
    private void SetFieldForTranslations()
    {
        FormFieldDataTypeEnum dataType = FormHelper.GetFormFieldDataType(AttributeType);

        if (TranslationServiceHelper.IsFieldForTranslation(dataType))
        {
            plcTranslation.Visible = true;
        }
        else
        {
            plcTranslation.Visible = false;
            chkTranslateField.Checked = false;
        }
    }


    /// <summary>
    /// Enables or disables control related to attribute name.
    /// </summary>
    private void EnableDisableAttributeName(bool enable)
    {
        drpGroup.Enabled = enable && IsNewItemEdited;
        drpSystemFields.Enabled = enable && IsNewItemEdited;
        txtAttributeName.Enabled = enable;
    }


    /// <summary>
    /// Enables or disables sections of the unigrid according to the selected mode.
    /// </summary>
    private void EnableDisableSections()
    {
        EnableDisableAttributeName(!IsAlternativeForm);
        drpAttributeType.Enabled = !IsAlternativeForm && (!IsSystem || !IsSystemFieldSelected && DevelopmentMode);
        txtAttributeSize.Enabled = !IsAlternativeForm;
    }


    /// <summary>
    /// Used only for alternative forms. If current field in class allows empty then it returns TRUE.
    /// </summary>
    private bool GetFormAllowEmpty()
    {
        // Check if field exists in class
        FormInfo fi = FormHelper.GetFormInfo(ClassName, false);
        FormFieldInfo ffi;
        if (fi != null)
        {
            ffi = fi.GetFormField(FieldInfo.Name);
            if (ffi != null)
            {
                return ffi.AllowEmpty;
            }
        }

        // Check if field exists in coupled class
        if (!String.IsNullOrEmpty(CoupledClassName))
        {
            fi = FormHelper.GetFormInfo(CoupledClassName, false);
            if (fi != null)
            {
                ffi = fi.GetFormField(FieldInfo.Name);
                if (ffi != null)
                {
                    return ffi.AllowEmpty;
                }
            }
        }

        return false;
    }


    /// <summary>
    /// Returns if field's control is of file type.
    /// </summary>
    private bool IsFileControl()
    {
        if (FieldInfo == null)
        {
            return false;
        }

        // Compare the control against default upload control
        if (FormHelper.IsFieldOfType(FieldInfo, FormFieldControlTypeEnum.UploadControl))
        {
            return true;
        }

        // Get form user control
        string controlName = ValidationHelper.GetString(FieldInfo.Settings["controlname"], string.Empty);
        if (!string.IsNullOrEmpty(controlName))
        {
            FormUserControlInfo controlInfo = FormUserControlInfoProvider.GetFormUserControlInfo(controlName);
            if (controlInfo != null)
            {
                return ((Mode == FieldEditorModeEnum.SystemTable) && controlInfo.UserControlForFile)
                    || ((Mode == FieldEditorModeEnum.BizFormDefinition) && (controlInfo.UserControlDefaultDataType.EqualsCSafe("file", StringComparison.InvariantCultureIgnoreCase)
                    || controlInfo.UserControlForFile));
            }
        }

        return false;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Drop-down list event handler.
    /// </summary>
    protected void drpGroup_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetSystemAttributeName(drpGroup.SelectedValue, "");
        if (DropChanged != null)
        {
            DropChanged(this, EventArgs.Empty);
        }
    }


    /// <summary>
    ///  Drop-down system fields event handler.
    /// </summary>
    protected void drpSystemFields_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DropChanged != null)
        {
            DropChanged(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// Listbox attribute type event handler.
    /// </summary>
    protected void drpAttributeType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AttributeChanged != null)
        {
            AttributeChanged(this, EventArgs.Empty);
        }
    }

    #endregion
}