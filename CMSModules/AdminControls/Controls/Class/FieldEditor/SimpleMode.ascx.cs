using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_SimpleMode : CMSUserControl
{
    #region "Events"

    /// <summary>
    /// Fired when DDL with form controls has been changed.
    /// </summary>
    public event EventHandler OnFieldSelected
    {
        add
        {
            drpControl.OnSelectionChanged += value;
        }
        remove
        {
            drpControl.OnSelectionChanged -= value;
        }
    }


    /// <summary>
    /// Occurs when FormFieldInfo is requested from control.
    /// </summary>
    public event EventHandler OnGetFieldInfo;

    #endregion


    #region "Variables"

    private FieldEditorSelectedItemEnum mSelectedItemType = FieldEditorSelectedItemEnum.Field;


    private FieldEditorControlsEnum mDisplayedControls = FieldEditorControlsEnum.ModeSelected;


    private string mClassName = string.Empty;


    private FieldEditorModeEnum mMode;


    private FormInfo fi = null;


    private FormFieldInfo mFieldInfo = null;


    private const string controlPrefix = "#uc#";


    private static Hashtable mSettings = null;


    private bool mDevelopmentMode = false;


    public Func<FieldEditorControlsEnum, FieldEditorModeEnum, bool, FieldEditorControlsEnum> GetControls;

    #endregion


    #region "Properties"

    /// <summary>
    /// Selected item type.
    /// </summary>
    public FieldEditorSelectedItemEnum SelectedItemType
    {
        get
        {
            return mSelectedItemType;
        }
        set
        {
            mSelectedItemType = value;
        }
    }


    /// <summary>
    /// Shows in what control is this form used.
    /// </summary>
    public FormTypeEnum FormType
    {
        get
        {
            return controlSettings.FormType;
        }
        set
        {
            controlSettings.FormType = value;
        }
    }


    /// <summary>
    /// Indicates whether new item is edited.
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
    /// Type of custom controls that can be selected from the control list in FieldEditor.
    /// </summary>
    public FieldEditorControlsEnum DisplayedControls
    {
        get
        {
            return mDisplayedControls;
        }
        set
        {
            mDisplayedControls = value;
        }
    }


    /// <summary>
    /// Class name.
    /// </summary>
    public string ClassName
    {
        get
        {
            return mClassName;
        }
        set
        {
            mClassName = value;
        }
    }


    /// <summary>
    /// Field editor mode.
    /// </summary>
    public FieldEditorModeEnum Mode
    {
        get
        {
            return mMode;
        }
        set
        {
            mMode = value;
        }
    }


    /// <summary>
    /// Form field info of current field.
    /// </summary>
    public FormFieldInfo FieldInfo
    {
        get
        {
            return mFieldInfo;
        }
        set
        {
            mFieldInfo = value;
        }
    }


    /// <summary>
    /// Field settings hashtable.
    /// </summary>
    public Hashtable Settings
    {
        get
        {
            return mSettings;
        }
        set
        {
            mSettings = new Hashtable(value, StringComparer.InvariantCultureIgnoreCase);
        }
    }


    /// <summary>
    /// Gets or sets selected control type.
    /// </summary>
    public string FieldType
    {
        get
        {
            return drpControl.Text;
        }
        set
        {
            if (!String.IsNullOrEmpty(value))
            {
                drpControl.Value = value.ToLowerCSafe();
            }
        }
    }


    /// <summary>
    /// Gets attribute name.
    /// </summary>
    public string AttributeName
    {
        get
        {
            return txtColumnName.Text.Trim();
        }
    }


    /// <summary>
    /// Gets field caption.
    /// </summary>
    public string FieldCaption
    {
        get
        {
            return txtFieldCaption.Text.Trim();
        }
    }


    /// <summary>
    /// Gets value indicating if field allows empty values.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return chkAllowEmpty.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if field is system field.
    /// </summary>
    public bool IsSystem
    {
        get
        {
            return chkIsSystem.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if field is marked as public.
    /// </summary>
    public bool PublicField
    {
        get
        {
            return chkPublicField.Checked;
        }
    }


    /// <summary>
    /// BasicFrom data.
    /// </summary>
    public DataRow FormData
    {
        get
        {
            return controlSettings.FormData;
        }
    }


    /// <summary>
    /// Attribute size.
    /// </summary>
    public int AttributeSize
    {
        get
        {
            return ValidationHelper.GetInteger(txtSimpleTextBoxMaxLength.Text.Trim(), 0);
        }
    }


    /// <summary>
    /// Indicates if field editor is in development mode.
    /// </summary>
    public bool DevelopmentMode
    {
        get
        {
            return mDevelopmentMode;
        }
        set
        {
            mDevelopmentMode = value;
        }
    }


    /// <summary>
    /// Gets field data type.
    /// </summary>
    public string AttributeType
    {
        get
        {
            string selectedType = null;

            // Try to load FormFieldInfo from parent
            if (OnGetFieldInfo != null)
            {
                OnGetFieldInfo(this, EventArgs.Empty);
            }

            // Get data type from FormFieldInfo if not NULL
            if (mFieldInfo != null)
            {
                selectedType = FormHelper.GetFormFieldDataTypeString(mFieldInfo.DataType);
            }
            // Get default data type for selected control
            else
            {
                selectedType = FormUserControlInfoProvider.GetUserControlDefaultDataType(FieldType)[0];
            }

            // For special cases change data type
            if (((Mode == FieldEditorModeEnum.BizFormDefinition) || (Mode == FieldEditorModeEnum.SystemTable))
                && (IsFileControl() || (FieldType.ToLowerCSafe() == Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.UploadControl).ToLowerCSafe())))
            {
                selectedType = FormFieldDataTypeCode.FILE;
            }

            return selectedType;
        }
    }

    #endregion


    #region "Methods"

    public void Page_Load(object sender, EventArgs e)
    {
        drpControl.MinimumPriority = (int)ObjectPriorityEnum.High;
        drpControl.ReturnColumnName = FormUserControlInfo.TYPEINFO.CodeNameColumn;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        pnlSimple.GroupingText = GetString("templatedesigner.simplemode");

        // Display maximum length textbox according to current field type
        plcSimpleTextBox.Visible = (AttributeType == FormFieldDataTypeCode.TEXT);

        // Public field option initialization    
        pnlPublicField.Visible = (Mode == FieldEditorModeEnum.BizFormDefinition);

        // Display or hide 'is system' option
        plcIsSystem.Visible = DevelopmentMode;

        // Enable or disable some controls in simplified mode while editing existing field
        if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
        {
            // Editing new or existing item
            txtColumnName.Enabled = IsNewItemEdited;
            txtColumnName.ReadOnly = !IsNewItemEdited;
            drpControl.Enabled = IsNewItemEdited;
        }
    }


    /// <summary>
    /// Sets right where conditions for data types dropdown - depending on where field editor is.
    /// </summary>
    private void LoadDataTypes()
    {
        FieldEditorControlsEnum controls = GetDisplayedControls();
        drpControl.WhereCondition = FormHelper.GetWhereConditionForDataType(null, controls, true, FormUserControlTypeEnum.Unspecified);
    }


    /// <summary>
    /// Loads control with new FormInfo data.
    /// </summary>
    /// <param name="type">Selected control type</param>
    /// <param name="defaultValues">Indicates if default values should be loaded</param>
    public void LoadControlSettings(string type, bool defaultValues)
    {
        string selectedFieldType = type;
        if (String.IsNullOrEmpty(type))
        {
            selectedFieldType = drpControl.Text.ToLowerCSafe();
        }
        if (selectedFieldType.StartsWithCSafe(controlPrefix))
        {
            selectedFieldType = selectedFieldType.Substring(controlPrefix.Length);
        }

        FormUserControlInfo control = FormUserControlInfoProvider.GetFormUserControlInfo(selectedFieldType);
        if (control != null)
        {
            string controlParameters = "";

            if (control.UserControlParentID == 0)
            {
                // Get normal form control parameters
                controlParameters = control.UserControlParameters;
            }
            else
            {
                // Get merged form info from inherited and parent control
                controlParameters = control.MergedFormInfo;
            }

            // Get form controls' settings
            fi = FormHelper.GetFormControlParameters(selectedFieldType, controlParameters, true);

            controlSettings.FormInfo = fi;

            Reload(defaultValues);
        }

        controlSettings.Visible = fi.ItemsList.OfType<FormFieldInfo>().Any(x => x.DisplayInSimpleMode);
    }


    /// <summary>
    /// Reload control.
    /// </summary>
    /// <param name="defaultValues">Indicates if default values should be loaded</param>
    public void Reload(bool defaultValues)
    {
        // Display or hide controls for default values
        DisplayDefaultControls();
        LoadDataTypes();

        // Load controls with values
        if (FieldInfo != null)
        {
            txtColumnName.Text = FieldInfo.Name;
            chkPublicField.Checked = FieldInfo.PublicField;
            txtFieldCaption.Text = FieldInfo.Caption;
            chkAllowEmpty.Checked = FieldInfo.AllowEmpty;
            chkIsSystem.Checked = FieldInfo.System;
            txtSimpleTextBoxMaxLength.Text = ValidationHelper.GetString(FieldInfo.Size, null);
            controlSettings.Settings = FieldInfo.Settings;
        }
        // Get default maximum length for text fields
        else if (defaultValues)
        {
            txtSimpleTextBoxMaxLength.Text = FormUserControlInfoProvider.GetUserControlDefaultDataType(FieldType)[1];
        }

        // Primary key not allowed to change
        // System field not allowed to change unless development mode
        if ((FieldInfo != null) && ((FieldInfo.PrimaryKey) || (FieldInfo.System && !DevelopmentMode)))
        {
            DisableFieldEditing();
        }
        // Enable to change settings for new items and non-primary items
        else
        {
            EnableFieldEditing();
        }
        controlSettings.Reload(true);
    }


    /// <summary>
    /// Clears simple form.
    /// </summary>
    public void ClearForm()
    {
        txtColumnName.Text = null;
        chkPublicField.Checked = true;
        txtFieldCaption.Text = null;
        chkAllowEmpty.Checked = false;
        chkIsSystem.Checked = false;
        txtSimpleTextBoxMaxLength.Text = null;
        txtDefaultValue.Text = null;
        chkDefaultValue.Checked = false;
        txtLargeDefaultValue.Value = null;
        datetimeDefaultValue.SelectedDateTime = DateTime.MinValue;

        string defaultControl = FormHelper.GetFormFieldDefaultControlType(FormFieldDataTypeCode.TEXT);
        if (FormHelper.IsValidControl(defaultControl, GetControls(DisplayedControls, mMode, DevelopmentMode)))
        {
            drpControl.Value = defaultControl;
        }
        else
        {
            drpControl.Value = String.Empty;
        }
        drpControl.Reload(true);
    }


    /// <summary>
    /// Save data from Basic form.
    /// </summary>
    public bool SaveData()
    {
        if (controlSettings.Visible)
        {
            return controlSettings.SaveData();
        }
        return true;
    }


    /// <summary>
    /// Loads both field types and control types from inner settings.
    /// </summary>
    public void LoadTypes()
    {
        if (FieldInfo != null)
        {
            // Get control name..
            string item = null;
            // ..from settings..
            if (!string.IsNullOrEmpty(ValidationHelper.GetString(FieldInfo.Settings["controlname"], null)))
            {
                item = Convert.ToString(FieldInfo.Settings["controlname"]).ToLowerCSafe();
            }
            // ..or from field type.
            else
            {
                item = FormHelper.GetFormFieldControlTypeString(FieldInfo.FieldType).ToLowerCSafe();
            }

            // Load and select options
            FormUserControlInfo fi = FormUserControlInfoProvider.GetFormUserControlInfo(item);
            if (fi != null)
            {
                drpControl.ReturnColumnName = FormUserControlInfo.TYPEINFO.CodeNameColumn;
                drpControl.Value = item;
                drpControl.Reload(true);
            }
        }
    }


    /// <summary>
    /// Returns control default value based on control data type.
    /// </summary>
    /// <param name="ffi">FormFieldInfo for existing field. Can be NULL for newly created fields</param>
    /// <returns>Returns default value.</returns>
    public string GetDefaultValue(FormFieldInfo ffi)
    {
        // Return default value based on FFI settings
        if (ffi != null)
        {
            string fieldDataType = FormHelper.GetFormFieldDataTypeString(ffi.DataType);

            switch (fieldDataType)
            {
                case FormFieldDataTypeCode.BOOLEAN:
                    return Convert.ToString(chkDefaultValue.Checked).ToLowerCSafe();

                case FormFieldDataTypeCode.DATETIME:
                    return GetDateTimeDefaultValue();

                case FormFieldDataTypeCode.LONGTEXT:
                    return txtLargeDefaultValue.Value.ToString().Trim();

                default:
                    return txtDefaultValue.Text.Trim();
            }
        }
        // Return default value based on currently selected field type
        else
        {
            string defaultValue = null;

            // DateTime based control
            if (datetimeDefaultValue.Visible)
            {
                string datetimevalue = datetimeDefaultValue.DateTimeTextBox.Text.Trim();
                if (datetimevalue.ToLowerCSafe() == DateTimePicker.DATE_TODAY.ToLowerCSafe())
                {
                    defaultValue = DateTimePicker.DATE_TODAY;
                }
                else if (datetimevalue.ToLowerCSafe() == DateTimePicker.TIME_NOW.ToLowerCSafe())
                {
                    defaultValue = DateTimePicker.TIME_NOW;
                }
                else if (ValidationHelper.IsMacro(datetimevalue))
                {
                    defaultValue = datetimevalue;
                }
                else if (datetimeDefaultValue.SelectedDateTime == DateTimePicker.NOT_SELECTED)
                {
                    defaultValue = "";
                }
                else
                {
                    defaultValue = datetimeDefaultValue.SelectedDateTime.ToString();
                }
            }
            // Bool based control
            else if (chkDefaultValue.Visible)
            {
                defaultValue = Convert.ToString(chkDefaultValue.Checked).ToLowerCSafe();
            }
            // Long text based control
            else if (txtLargeDefaultValue.Visible)
            {
                defaultValue = txtLargeDefaultValue.Value.ToString().Trim();
            }
            // Other types of controls
            else
            {
                defaultValue = txtDefaultValue.Text.Trim();
            }

            return defaultValue;
        }
    }


    /// <summary>
    /// Disables field editing controls.
    /// </summary>
    public void DisableFieldEditing()
    {
        txtDefaultValue.Enabled = false;
        txtLargeDefaultValue.Enabled = false;
        chkDefaultValue.Enabled = false;
        datetimeDefaultValue.Enabled = false;
        chkAllowEmpty.Enabled = false;
        chkIsSystem.Enabled = false;
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
        chkAllowEmpty.Enabled = (Mode != FieldEditorModeEnum.SystemTable);
        chkIsSystem.Enabled = true;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Hides or displays controls which are needed for default values.
    /// </summary>
    private void DisplayDefaultControls()
    {
        string fieldDataType = null;

        // Load control with values from FormFieldInfo
        if (FieldInfo != null)
        {
            fieldDataType = FormHelper.GetFormFieldDataTypeString(FieldInfo.DataType);
        }
        else
        {
            fieldDataType = FormUserControlInfoProvider.GetUserControlDefaultDataType(drpControl.Text)[0];
        }

        // UserControl with boolean datatype
        if (fieldDataType == FormFieldDataTypeCode.BOOLEAN)
        {
            if (FieldInfo != null)
            {
                chkDefaultValue.Checked = ValidationHelper.GetBoolean(FieldInfo.DefaultValue, false);
            }
            chkDefaultValue.Visible = true;
            txtDefaultValue.Visible = false;
            txtLargeDefaultValue.Visible = false;
            datetimeDefaultValue.Visible = false;
        }
        // UserControl with datetime datatype
        else if (fieldDataType == FormFieldDataTypeCode.DATETIME)
        {
            if (FieldInfo != null)
            {
                if (string.IsNullOrEmpty(FieldInfo.DefaultValue))
                {
                    datetimeDefaultValue.DateTimeTextBox.Text = String.Empty;
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
            }
            chkDefaultValue.Visible = false;
            txtDefaultValue.Visible = false;
            txtLargeDefaultValue.Visible = false;
            datetimeDefaultValue.Visible = true;
        }
        // UserControl with longtext datatype
        else if (fieldDataType == FormFieldDataTypeCode.LONGTEXT)
        {
            if (FieldInfo != null)
            {
                txtLargeDefaultValue.Value = FieldInfo.DefaultValue;
            }
            chkDefaultValue.Visible = false;
            txtDefaultValue.Visible = false;
            txtLargeDefaultValue.Visible = true;
            datetimeDefaultValue.Visible = false;
        }
        // UserControl with double datatype
        else if (fieldDataType == FormFieldDataTypeCode.DOUBLE)
        {
            if (FieldInfo != null)
            {
                txtDefaultValue.Text = FormHelper.GetDoubleValueInCurrentCulture(FieldInfo.DefaultValue);
            }
            chkDefaultValue.Visible = false;
            txtDefaultValue.Visible = true;
            txtLargeDefaultValue.Visible = false;
            datetimeDefaultValue.Visible = false;
        }
        // UserControl with text datatype
        else
        {
            if (FieldInfo != null)
            {
                txtDefaultValue.Text = FieldInfo.DefaultValue;
            }
            chkDefaultValue.Visible = false;
            txtDefaultValue.Visible = true;
            txtLargeDefaultValue.Visible = false;
            datetimeDefaultValue.Visible = false;
        }
    }


    /// <summary>
    /// Loads DataRow for BasicForm with data from FormFieldInfo settings.
    /// </summary>
    private DataRowContainer GetData()
    {
        DataRowContainer result = new DataRowContainer(fi.GetDataRow());

        if (Settings != null)
        {
            foreach (string columnName in result.ColumnNames)
            {
                if (Settings.ContainsKey(columnName) && !String.IsNullOrEmpty(Convert.ToString(Settings[columnName])))
                {
                    result[columnName] = Settings[columnName];
                }
            }
        }
        return result;
    }


    /// <summary>
    /// Returns default date time value.
    /// </summary>
    private string GetDateTimeDefaultValue()
    {
        string datetimevalue = datetimeDefaultValue.DateTimeTextBox.Text.Trim();

        // Get today's date
        if (datetimevalue.ToLowerCSafe() == DateTimePicker.DATE_TODAY.ToLowerCSafe())
        {
            datetimevalue = DateTimePicker.DATE_TODAY;
        }
        // Get time
        else if (datetimevalue.ToLowerCSafe() == DateTimePicker.TIME_NOW.ToLowerCSafe())
        {
            datetimevalue = DateTimePicker.TIME_NOW;
        }
        // Macro
        else if (ValidationHelper.IsMacro(datetimevalue))
        {
            // Use current value
        }
        // No date time
        else if (datetimeDefaultValue.SelectedDateTime == DateTimePicker.NOT_SELECTED)
        {
            datetimevalue = "";
        }
        // Selected value
        else
        {
            datetimevalue = datetimeDefaultValue.SelectedDateTime.ToString();
        }
        return datetimevalue;
    }


    /// <summary>
    /// Gets displayed controls.
    /// </summary>
    private FieldEditorControlsEnum GetDisplayedControls()
    {
        FieldEditorControlsEnum controls;
        if (DisplayedControls == FieldEditorControlsEnum.ModeSelected)
        {
            switch (Mode)
            {
                case FieldEditorModeEnum.BizFormDefinition:
                    controls = FieldEditorControlsEnum.Bizforms;
                    break;

                case FieldEditorModeEnum.ClassFormDefinition:
                    controls = FieldEditorControlsEnum.DocumentTypes;
                    break;

                case FieldEditorModeEnum.SystemTable:
                    controls = FieldEditorControlsEnum.SystemTables;
                    break;

                case FieldEditorModeEnum.CustomTable:
                    controls = FieldEditorControlsEnum.CustomTables;
                    break;

                case FieldEditorModeEnum.WebPartProperties:
                    controls = FieldEditorControlsEnum.Controls;
                    break;

                case FieldEditorModeEnum.General:
                    controls = FieldEditorControlsEnum.All;
                    break;

                default:
                    controls = FieldEditorControlsEnum.None;
                    break;
            }
        }
        else
        {
            controls = DisplayedControls;
        }

        return controls;
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
}