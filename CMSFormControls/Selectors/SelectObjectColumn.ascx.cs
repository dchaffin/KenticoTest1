using System;
using System.Web.UI.WebControls;
using System.Linq;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.CMSHelper;
using CMS.FormEngine;


public partial class CMSFormControls_Selectors_SelectObjectColumn : FormEngineUserControl
{
    #region "Private variables"

    private string selectedColumn = String.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets selected object type.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ObjectType"), null);
        }
        set
        {
            SetValue("ObjectType", value);
        }
    }


    /// <summary>
    /// Gets or sets selected value.
    /// </summary>
    public override object Value
    {
        get
        {
            return (drpColumn.SelectedItem != null) ? drpColumn.SelectedItem.Value : String.Empty;
        }
        set
        {
            selectedColumn = ValidationHelper.GetString(value, String.Empty);
        }
    }


    /// <summary>
    /// Indicates if all columns should be displayed (except primary key column).
    /// </summary>
    public bool ShowAllColumns
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAllColumns"), false);
        }
        set
        {
            SetValue("ShowAllColumns", value);
        }
    }


    /// <summary>
    /// Indicates if column caption should be used instead of column name.
    /// </summary>
    public bool UseColumnCaption
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseColumnCaption"), false);
        }
        set
        {
            SetValue("UseColumnCaption", value);
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!StopProcessing)
        {
            SetupControl();
        }
    }


    /// <summary>
    /// Fills dropdown list with column names
    /// </summary>
    protected void SetupControl()
    {
        drpColumn.Items.Clear();

        // Get object by object type
        GeneralizedInfo generalObject = CMSObjectHelper.GetObject(ObjectType);
        if (generalObject != null)
        {
            FormInfo fi = FormHelper.GetFormInfo(generalObject.ObjectClassName, false);
            if (fi != null)
            {
                // Get fields except the primary key
                var fields = fi.GetFields(true, ShowAllColumns).Where(f => !f.PrimaryKey);

                // Sort fields by visibility and caption or name
                fields = fields.OrderByDescending(f => f.Visible).ThenBy(f => UseColumnCaption ? (string.IsNullOrEmpty(f.Caption) ? f.Name : f.Caption) : f.Name);

                foreach (FormFieldInfo field in fields)
                {
                    if (UseColumnCaption && !String.IsNullOrEmpty(field.Caption))
                    {
                        drpColumn.Items.Add(new ListItem(field.Caption, field.Name));
                    }
                    else
                    {
                        drpColumn.Items.Add(field.Name);
                    }
                }
            }
        }

        if (selectedColumn != String.Empty)
        {
            // Select selected column
            ListItem item = drpColumn.Items.FindByValue(ValidationHelper.GetString(selectedColumn, String.Empty));
            if (item != null)
            {
                item.Selected = true;
            }
        }
    }

    #endregion
}
