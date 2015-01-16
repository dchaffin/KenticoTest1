using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;

public partial class CMSFormControls_Basic_ListBoxControl : FormEngineUserControl
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
            return listbox.Enabled;
        }
        set
        {
            listbox.Enabled = value;
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
            foreach (ListItem item in listbox.Items)
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
            listbox.ClearSelection();
            foreach (string val in selectedValues)
            {
                if (listbox.Items.FindByValue(val) != null)
                {
                    listbox.Items.FindByValue(val).Selected = true;
                    if (listbox.SelectionMode == ListSelectionMode.Single)
                    {
                        break;
                    }
                }
            }
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadAndSelectList();

        // Set control style
        if (!String.IsNullOrEmpty(CssClass))
        {
            listbox.CssClass = CssClass;
            CssClass = null;
        }
        else if (String.IsNullOrEmpty(listbox.CssClass))
        {
            listbox.CssClass = "ListBoxField";
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            listbox.Attributes.Add("style", ControlStyle);
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
        if (listbox.Items.Count == 0)
        {
            // Set listbox mode
            bool allowmultiple = ValidationHelper.GetBoolean(GetValue("allowmultiplechoices"), true);
            if (allowmultiple)
            {
                listbox.SelectionMode = ListSelectionMode.Multiple;
            }
            else
            {
                listbox.SelectionMode = ListSelectionMode.Single;
            }

            string options = ValidationHelper.GetString(GetValue("options"), null);
            string query = ValidationHelper.GetString(GetValue("query"), null);

            try
            {
                FormHelper.LoadItemsIntoList(options, query, listbox.Items, FieldInfo);
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }

            FormHelper.SelectMultipleValues(selectedValues, listbox.Items, listbox.SelectionMode);
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
        listbox.Visible = false;
    }

    #endregion
}