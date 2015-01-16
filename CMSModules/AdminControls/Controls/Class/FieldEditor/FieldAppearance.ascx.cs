using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.CMSHelper;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_FieldAppearance : CMSUserControl
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

    #endregion


    #region "Variables"

    private FieldEditorControlsEnum mDisplayedControls = FieldEditorControlsEnum.ModeSelected;
    private FieldEditorModeEnum mMode;
    private string mClassName = null;
    private FormFieldInfo ffi = null;
    private bool mShowFieldVisibility = false;
    public Func<FieldEditorControlsEnum, FieldEditorModeEnum, bool, FieldEditorControlsEnum> GetControls;

    #endregion


    #region "Properties"

    /// <summary>
    /// Type of field.
    /// </summary>
    public string AttributeType
    {
        get
        {
            return ValidationHelper.GetString(ViewState["AttributeType"], null);
        }
        set
        {
            drpControl.DataType = value;
            ViewState["AttributeType"] = value;
        }
    }


    /// <summary>
    /// Gets or sets selected control type.
    /// </summary>
    public string FieldType
    {
        get
        {
            return drpControl.Text.ToLowerCSafe();
        }
        set
        {
            if (!String.IsNullOrEmpty(value))
            {
                drpControl.Value = value.ToLowerCSafe();
                drpControl.Reload(true);
            }
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
    /// FormFieldInfo of given field.
    /// </summary>
    public FormFieldInfo FieldInfo
    {
        get
        {
            return ffi;
        }
        set
        {
            ffi = value;
        }
    }


    /// <summary>
    /// Gets value from Field caption attribute.
    /// </summary>
    public string FieldCaption
    {
        get
        {
            return txtFieldCaption.Text.Trim();
        }
    }


    /// <summary>
    /// Gets or sets value indicating if user can change visibility of given field.
    /// </summary>
    public bool ChangeVisibility
    {
        get
        {
            return chkChangeVisibility.Checked;
        }
        set
        {
            chkChangeVisibility.Checked = value;
        }
    }


    /// <summary>
    /// Gets or sets value of visibility control.
    /// </summary>
    public string VisibilityValue
    {
        get
        {
            return ctrlVisibility.Value;
        }
        set
        {
            ctrlVisibility.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets value of DDL visibility section.
    /// </summary>
    public string VisibilityDDL
    {
        get
        {
            return drpVisibilityControl.SelectedValue;
        }
        set
        {
            drpVisibilityControl.SelectedValue = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if Public field checkbox is checked.
    /// </summary>
    public bool PublicField
    {
        get
        {
            return chkPublicField.Checked;
        }
        set
        {
            chkPublicField.Checked = value;
        }
    }


    /// <summary>
    /// Gets value which represents text field Description value.
    /// </summary>
    public string Description
    {
        get
        {
            return txtDescription.Text.Trim();
        }
    }


    /// <summary>
    /// Gets or sets field visibility.
    /// </summary>
    public bool ShowFieldVisibility
    {
        get
        {
            return mShowFieldVisibility;
        }
        set
        {
            mShowFieldVisibility = value;
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
    /// Gets or sets alternative form full name.
    /// </summary>
    public string AlternativeFormFullName
    {
        get;
        set;
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
    /// Indicates if the form is alternative filter form.
    /// </summary>
    public bool IsAlternativeFilterForm
    {
        get
        {
            if (IsAlternativeForm && !string.IsNullOrEmpty(AlternativeFormFullName))
            {
                return AlternativeFormFullName.ToLowerCSafe().EndsWithCSafe(".filter");
            }

            return false;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if control is inheritable or not.
    /// </summary>
    public bool ControlInheritable
    {
        get
        {
            return chkControlInheritable.Checked;
        }
        set
        {
            chkControlInheritable.Checked = value;
        }
    }


    /// <summary>
    /// Enables or disables to edit <see cref='CMS.FormEngine.FormFieldInfo.Inheritable'> settings.
    /// </summary>
    public bool ShowInheritanceSettings
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

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        drpControl.ReturnColumnName = FormUserControlInfo.TYPEINFO.CodeNameColumn;
    }


    public void Page_Load(object sender, EventArgs e)
    {
        pnlAppearance.GroupingText = GetString("templatedesigner.section.fieldappearance");

        drpControl.DataType = AttributeType;
        drpControl.Value = FieldType;

        // Public field option initialization    
        plcPublicField.Visible = (Mode == FieldEditorModeEnum.BizFormDefinition);
        plcInheritance.Visible = ShowInheritanceSettings;
    }


    /// <summary>
    /// Loads field with values from FormFieldInfo.
    /// </summary>
    public void Reload()
    {
        if (ffi != null)
        {
            txtDescription.Text = ffi.Description;
            txtFieldCaption.Text = ffi.Caption;

            if (ShowInheritanceSettings)
            {
                chkControlInheritable.Checked = ffi.Inheritable;
            }

            // Field visibility
            if (ShowFieldVisibility)
            {
                chkChangeVisibility.Checked = ffi.AllowUserToChangeVisibility;
                ctrlVisibility.Value = ffi.Visibility;

                // Load controls for user visibility
                drpVisibilityControl.DataSource = FormUserControlInfoProvider.GetFormUserControls("UserControlCodeName, UserControlDisplayName", "UserControlForVisibility = 1", "UserControlDisplayName");
                drpVisibilityControl.DataBind();

                try
                {
                    drpVisibilityControl.Items.FindByValue(ffi.VisibilityControl);
                    drpVisibilityControl.SelectedValue = ffi.VisibilityControl;
                }
                catch
                {
                }
            }
            plcVisibility.Visible = ShowFieldVisibility;

            if (Mode == FieldEditorModeEnum.BizFormDefinition)
            {
                chkPublicField.Checked = ffi.PublicField;
            }

            string selectedItem;
            // Get control name from settings
            if (!String.IsNullOrEmpty(Convert.ToString(ffi.Settings["controlname"])))
            {
                selectedItem = Convert.ToString(ffi.Settings["controlname"]).ToLowerCSafe();
            }
            // Or get control name from field type
            else
            {
                selectedItem = FormHelper.GetFormFieldControlTypeString(ffi.FieldType).ToLowerCSafe();
            }

            FormUserControlInfo fi = FormUserControlInfoProvider.GetFormUserControlInfo(selectedItem);

            if (fi != null)
            {
                // Preselect options for specific control
                FieldType = selectedItem;
            }
            else
            {
                // Preselect default control for attribute type
                FieldType = FormHelper.GetFormFieldDefaultControlType(AttributeType);
            }

            LoadFieldTypes(ffi.PrimaryKey);
        }
        // If FormFieldInfo is not specified then clear form
        else
        {
            chkPublicField.Checked = true;
            ctrlVisibility.Value = null;
            drpVisibilityControl.ClearSelection();
            chkChangeVisibility.Checked = false;
            txtFieldCaption.Text = null;
            txtDescription.Text = null;
            drpControl.Reload(false);
            drpControl.Value = FormHelper.GetFormFieldDefaultControlType(FormFieldDataTypeCode.TEXT);
        }
    }


    /// <summary>
    /// Fill field types list. Form control types will be restricted to actual selection in Form control types drop-down list.
    /// </summary>
    /// <param name="isPrimary">Determines whether the attribute is primary key</param>
    /// <param name="clearValue">Determines if selector should clear selected value</param>
    public void LoadFieldTypes(bool isPrimary)
    {
        FieldEditorControlsEnum controls = GetControls(DisplayedControls, mMode, DevelopmentMode);

        string filteredControlsWhere = FormHelper.GetWhereConditionForDataType(AttributeType, controls, isPrimary, FormUserControlTypeEnum.Unspecified);
        drpControl.WhereCondition = filteredControlsWhere;

        // Exception for blogposts in document types
        if ((controls != FieldEditorControlsEnum.DocumentTypes) || (ClassName.ToLowerCSafe() != "cms.blogpost"))
        {
            // Show trackbackpingedurls only when editing blogposts
            drpControl.WhereCondition = SqlHelperClass.AddWhereCondition(drpControl.WhereCondition, FormUserControlInfo.TYPEINFO.CodeNameColumn + " <> 'trackbackspingedurls'");
            drpControl.WhereCondition = SqlHelperClass.AddWhereCondition(drpControl.WhereCondition, FormUserControlInfo.TYPEINFO.CodeNameColumn + " <> 'trackbacksnotpingedurls'");
        }

        drpControl.Reload(true);
    }

    #endregion
}