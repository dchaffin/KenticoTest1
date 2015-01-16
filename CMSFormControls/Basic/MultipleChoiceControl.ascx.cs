using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;


public partial class CMSFormControls_Basic_MultipleChoiceControl : FormEngineUserControl
{
    #region "Variables"

    private string[] selectedValues = null;

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
            StringBuilder text = new StringBuilder();
            foreach (ListItem item in list.Items)
            {
                if (item.Selected)
                {
                    text.Append(item.Value + "|");
                }
            }
            return text.ToString().TrimEnd('|');
        }
        set
        {
            selectedValues = ValidationHelper.GetString(value, "").Split('|');
            list.ClearSelection();
            if (selectedValues != null)
            {
                foreach (string val in selectedValues)
                {
                    if (list.Items.FindByValue(val) != null)
                    {
                        list.Items.FindByValue(val).Selected = true;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Returns selected value display names separated with comma.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            StringBuilder text = new StringBuilder();
            bool first = true;
            foreach (ListItem item in list.Items)
            {
                if (item.Selected)
                {
                    if (!first)
                    {
                        text.Append(", ");
                    }
                    text.Append(item.Text);
                    first = false;
                }
            }
            return text.ToString();
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadAndSelectList();

        // Set control styles
        if (!String.IsNullOrEmpty(CssClass))
        {
            list.CssClass = CssClass;
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(list.CssClass))
        {
            list.CssClass = "CheckBoxListField";
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
            if (direction.EqualsCSafe("horizontal", true))
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

            FormHelper.SelectMultipleValues(selectedValues, list.Items, ListSelectionMode.Multiple);
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