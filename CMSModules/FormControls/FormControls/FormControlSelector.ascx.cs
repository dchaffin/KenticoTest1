using System;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.FormEngine;
using CMS.GlobalHelper;

public partial class CMSModules_FormControls_FormControls_FormControlSelector : FormEngineUserControl
{

    #region "Private variables"

    private bool showInheritedControls = true;

    #endregion


    #region "Public events"

    /// <summary>
    /// OnSelectionChanged event.
    /// </summary>
    public event EventHandler OnSelectionChanged
    {
        add
        {
            uniSelector.OnSelectionChanged += value;
        }
        remove
        {
            uniSelector.OnSelectionChanged -= value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Form control data type
    /// </summary>
    public string DataType
    {
        get;
        set;
    }


    /// <summary>
    /// Column name of the object which value should be returned by the selector. 
    /// If NULL, ID column is used.
    /// </summary>
    public string ReturnColumnName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets whether to prioritize items (high priority items on top) or not.
    /// </summary>
    public bool PrioritizeItems
    {
        get
        {
            return uniSelector.PrioritizeItems;
        }
        set
        {
            uniSelector.PrioritizeItems = value;
        }
    }


    /// <summary>
    /// Gets or sets minimum priority of controls to be shown.
    /// </summary>
    public int? MinimumPriority
    {
        get
        {
            return uniSelector.MinimumPriority;
        }
        set
        {
            uniSelector.MinimumPriority = value;
        }
    }


    /// <summary>
    /// Gets or sets if selector shows also inherited controls.
    /// </summary>
    public bool ShowInheritedControls
    {
        get
        {
            return showInheritedControls;
        }
        set
        {
            showInheritedControls = value;
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
            EnsureChildControls();
            base.Enabled = value;
            uniSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return uniSelector.Value;
        }
        set
        {
            EnsureChildControls();
            uniSelector.ReturnColumnName = ReturnColumnName;
            uniSelector.Value = value;

            // If value is not present, set it anyways. Used in field editor - simple mode. 
            // Not all controls are shown there, though one must get preselected.
            if (value != null && ForcedValueSet)
            {
                FormUserControlInfo control = FormUserControlInfoProvider.GetFormUserControlInfo(value.ToString());
                if (control != null)
                {
                    SpecialFields = new string[,] { { control.UserControlDisplayName, value.ToString() } };
                }
            }
        }
    }


    /// <summary>
    /// Forces value of dropdownlist to be created as a special field.
    /// </summary>
    public bool ForcedValueSet
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets if empty value item is visible in selection.
    /// </summary>
    public bool AllowEmptyValue
    {
        get
        {
            return uniSelector.AllowEmpty;
        }
        set
        {
            uniSelector.AllowEmpty = value;
        }
    }


    /// <summary>
    /// Array with the special field values that should be added to the DropDownList selection, 
    /// between the (none) item and the other items, e.g.: 
    /// string[1,2]: { { "(current group)", "{%CommunityContext.CurrentGroup.GroupID%}" } }.
    /// </summary>
    public string[,] SpecialFields
    {
        get
        {
            return uniSelector.SpecialFields;
        }
        set
        {
            uniSelector.SpecialFields = value;
        }
    }


    /// <summary>
    /// Sets AutoPostBack of control.
    /// </summary>
    public bool AutoPostBack
    {
        set
        {
            uniSelector.DropDownSingleSelect.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        uniSelector.PriorityStyles.Add((int)ObjectPriorityEnum.High, "HighPriority");
        uniSelector.FilterControl = "~/CMSModules/FormControls/FormControls/DataTypeFilter.ascx";
        AddWhereConditions();
        uniSelector.ReturnColumnName = ReturnColumnName;
    }


    /// <summary>
    /// Adds where conditions based on inner properties.
    /// </summary>
    private void AddWhereConditions()
    {
        uniSelector.WhereCondition = WhereCondition;

        // Set conditions
        if (!ShowInheritedControls)
        {
            // Show only not inherited controls
            uniSelector.WhereCondition = SqlHelperClass.AddWhereCondition(uniSelector.WhereCondition, "UserControlParentID IS NULL");
        }
        if (!String.IsNullOrEmpty(DataType))
        {
            uniSelector.WhereCondition = SqlHelperClass.AddWhereCondition(uniSelector.WhereCondition, FormHelper.GetDataTypeColumn(DataType) + " = 1");
        }
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void Reload(bool forceReload)
    {
        AddWhereConditions();
        uniSelector.ReturnColumnName = ReturnColumnName;
        uniSelector.Reload(forceReload);
    }

    #endregion


    #region "View State Methods"

    protected override void LoadViewState(object savedState)
    {
        object[] updatedState = (object[])savedState;

        // Load orig ViewState
        if (updatedState[0] != null)
        {
            base.LoadViewState(updatedState[0]);
        }
        if (updatedState[1] != null)
        {
            WhereCondition = (string)updatedState[1];
        }
    }


    protected override object SaveViewState()
    {
        object[] updatedState = new object[2];
        updatedState[0] = base.SaveViewState();
        updatedState[1] = WhereCondition;
        return updatedState;
    }

    #endregion

}
