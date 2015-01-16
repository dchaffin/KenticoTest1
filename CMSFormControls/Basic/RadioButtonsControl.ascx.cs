using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSFormControls_Basic_RadioButtonsControl : FormEngineUserControl
{
    #region "Variables"

    private string selectedValue = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return list.Enabled;
        }
        set
        {
            list.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return list.SelectedValue;
        }
        set
        {
            object convertedValue = value;
            if ((value != null) || ((FieldInfo != null) && FieldInfo.AllowEmpty))
            {
                if (FieldInfo != null)
                {
                    // Convert default boolean value to proper format
                    if (FieldInfo.DataType == FormFieldDataTypeEnum.Boolean)
                    {
                        convertedValue = ValidationHelper.GetBoolean(value, false);
                    }
                    // Ensure rendering of decimal in current culture format
                    else if (FieldInfo.DataType == FormFieldDataTypeEnum.Decimal)
                    {
                        convertedValue = FormHelper.GetDoubleValueInCurrentCulture(value);
                    }
                    // Ensure rendering of datetime in current culture format
                    else if (FieldInfo.DataType == FormFieldDataTypeEnum.DateTime)
                    {
                        convertedValue = FormHelper.GetDateTimeValueInCurrentCulture(value);
                    }
                }

                selectedValue = ValidationHelper.GetString(convertedValue, null);
                list.ClearSelection();
                if (list.Items.FindByValue(selectedValue) != null)
                {
                    list.SelectedValue = selectedValue;
                }
            }
        }
    }


    /// <summary>
    /// Returns display name of the value.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return (list.SelectedItem == null ? list.Text : list.SelectedItem.Text);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadAndSelectList();

        // Apply styles
        if (!String.IsNullOrEmpty(CssClass))
        {
            list.CssClass = CssClass;
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(list.CssClass))
        {
            list.CssClass = "RadioButtonList";
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            list.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckRegularExpression = true;
        CheckFieldEmptiness = true;
    }


    /// <summary>
    /// Loads and selects control.
    /// </summary>
    private void LoadAndSelectList()
    {
        if (list.Items.Count == 0)
        {
            // Set control direction
            string direction = ValidationHelper.GetString(GetValue("repeatdirection"), "");
            if (direction.ToLowerCSafe() == "horizontal")
            {
                list.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                list.RepeatDirection = RepeatDirection.Vertical;
            }

            string options = ValidationHelper.GetString(GetValue("options"), null);
            string query = ValidationHelper.GetString(GetValue("query"), null);

            try
            {
                FormHelper.LoadItemsIntoList(options, query, list.Items, FieldInfo);
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }
            FormHelper.SelectSingleValue(selectedValue, list);
        }
    }


    /// <summary>
    /// Displays exception control with current error.
    /// </summary>
    /// <param name="ex">Thrown exception</param>
    private void DisplayException(Exception ex)
    {
        FormControlError ctrlError = new FormControlError();
        ctrlError.InnerException = ex;
        Controls.Add(ctrlError);
        list.Visible = false;
    }

    #endregion
}