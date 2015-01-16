using System;
using System.Web.UI.WebControls;

using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_DocumentSource : CMSUserControl
{
    #region "Variables"

    private FormInfo fi = null;
    private string mClassName = null;
    private FieldEditorModeEnum mMode;

    #endregion


    #region "Events"

    /// <summary>
    /// Fired when source field DDL changes.
    /// </summary>
    public event EventHandler OnSourceFieldChanged;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets FormInfo object.
    /// </summary>
    public FormInfo FormInfo
    {
        get
        {
            return fi;
        }
        set
        {
            fi = value;
        }
    }


    /// <summary>
    /// Gets or sets class name.
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
    /// Gets or sets field editor mode.
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
    /// Gets or sets value indicating if field editor is used as alternative form editor.
    /// </summary>
    public bool IsAlternativeForm
    {
        get;
        set;
    }


    /// <summary>
    /// Gets value which is selected in DDL Source field.
    /// </summary>
    public string SourceFieldValue
    {
        get
        {
            return drpSourceField.SelectedValue;
        }
    }


    /// <summary>
    /// Gets value which is selected in DDL Alias Source field.
    /// </summary>
    public string SourceAliasFieldValue
    {
        get
        {
            return drpSourceAliasField.SelectedValue;
        }
    }


    /// <summary>
    /// Gets value indicating if any content is displayed.
    /// </summary>
    public bool VisibleContent
    {
        get
        {
            return pnlSourceField.Visible;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            Reload();
        }
    }


    /// <summary>
    /// Reloads control with data.
    /// </summary>
    public void Reload()
    {
        // Display or hide source field selection
        if ((Mode == FieldEditorModeEnum.ClassFormDefinition) && (fi != null) && !IsAlternativeForm)
        {
            // Fill source field drop down list
            pnlSourceField.Visible = true;
            
            // Add document name source field
            drpSourceField.Items.Clear();

            DataClassInfo dci = DataClassInfoProvider.GetDataClass(ClassName);
            if ((dci != null) && dci.ClassIsProduct)
            {
                drpSourceField.Items.Add(new ListItem(GetString("TemplateDesigner.ImplicitProductSourceField"), ""));
            }
            else
            {
                drpSourceField.Items.Add(new ListItem(GetString("TemplateDesigner.ImplicitSourceField"), ""));
            }

            // Add alias name source field
            drpSourceAliasField.Items.Clear();
            drpSourceAliasField.Items.Add(new ListItem(GetString("TemplateDesigner.DefaultSourceField"), ""));

            AddField(drpSourceAliasField, "NodeID");
            AddField(drpSourceAliasField, "DocumentID");            

            var columnNames = fi.GetColumnNames();
            if (columnNames != null)
            {
                // Add attribute list item to the list of attributes
                foreach (string name in columnNames)
                {
                    FormFieldInfo ffiColumn = fi.GetFormField(name);
                    // Don't add primary key field
                    if (!ffiColumn.PrimaryKey)
                    {
                        AddField(drpSourceField, name);
                    }
                    AddField(drpSourceAliasField, name);
                }
            }

            // Set selected value
            if (dci != null)
            {
                if (drpSourceField.Items.FindByValue(dci.ClassNodeNameSource) != null)
                {
                    drpSourceField.SelectedValue = dci.ClassNodeNameSource;
                }

                if (drpSourceAliasField.Items.FindByValue(dci.ClassNodeAliasSource) != null)
                {
                    drpSourceAliasField.SelectedValue = dci.ClassNodeAliasSource;
                }
            }
        }
    }


    /// <summary>
    /// Adds the given field to dropdown
    /// </summary>
    /// <param name="drp">Dropdown list</param>
    /// <param name="column">Field to add</param>
    private void AddField(DropDownList drp, string column)
    {
        drp.Items.Add(new ListItem(column, column));
    }

    #endregion


    #region "Event handling"

    /// <summary>
    /// Called when source field selected index changed.
    /// </summary>
    protected void drpSourceField_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (OnSourceFieldChanged != null)
        {
            OnSourceFieldChanged(sender, EventArgs.Empty);
        }
    }

    #endregion
}