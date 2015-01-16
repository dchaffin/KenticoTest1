using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSFormControls_Sites_SelectSite : FormEngineUserControl
{
    #region "Variables"

    private bool? mAllowAll;
    private bool? mAllowEmpty;
    private bool? mAllowGlobal;
    private bool mAllowMultipleSelection;

    #endregion


    #region "Public properties"

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
            usSites.Enabled = value;
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
            return usSites.Value;
        }
        set
        {
            EnsureChildControls();

            if ((FieldInfo != null) && (FieldInfo.DataType == FormFieldDataTypeEnum.Integer))
            {
                usSites.SelectionMode = SelectionModeEnum.SingleDropDownList;
            }

            usSites.Value = value;
        }
    }


    /// <summary>
    /// Enables or disables multiple site selection.
    /// </summary>
    public bool AllowMultipleSelection
    {
        get
        {
            return mAllowMultipleSelection;
        }
        set
        {
            EnsureChildControls();
            mAllowMultipleSelection = value;
            if (mAllowMultipleSelection)
            {
                usSites.SelectionMode = SelectionModeEnum.MultipleTextBox;
            }
            else
            {
                usSites.SelectionMode = SelectionModeEnum.SingleTextBox;
            }
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            EnsureChildControls();
            base.IsLiveSite = value;
            usSites.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Enables or disables (all) item in selector.
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return mAllowAll ?? ValidationHelper.GetBoolean(GetValue("AllowAll"), false);
        }
        set
        {
            EnsureChildControls();
            mAllowAll = value;
            usSites.AllowAll = value;
        }
    }


    /// <summary>
    /// Enables or disables (empty) item in selector.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return mAllowEmpty ?? ValidationHelper.GetBoolean(GetValue("AllowEmpty"), false);
        }
        set
        {
            EnsureChildControls();
            mAllowEmpty = value;
            usSites.AllowEmpty = value;
        }
    }


    /// <summary>
    /// Enables or disables (global) item in selector. Uses uniSelector's SpecialFields property.
    /// </summary>
    public bool AllowGlobal
    {
        get
        {
            return mAllowGlobal ?? ValidationHelper.GetBoolean(GetValue("AllowGlobal"), false);
        }
        set
        {
            mAllowGlobal = value;
            EnsureChildControls();
            if (AllowGlobal)
            {
                usSites.SpecialFields = new string[1, 2] { { GetString("general.global"), UniSelector.US_GLOBAL_RECORD.ToString() } };
            }
            else
            {
                usSites.SpecialFields = null;
            }
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        SetupControls();

        if (HasDependingFields)
        {
            usSites.OnSelectionChanged += usSites_OnSelectionChanged;

            if (usSites.SelectionMode == SelectionModeEnum.SingleDropDownList)
            {
                // If this control has depending fields (some other control changes when this one is changed) and mode is SingleDropDownList, set AutoPostBack to true,
                // because changes have to be propagated immediatelly
                usSites.DropDownSingleSelect.AutoPostBack = true;
            }
        }
    }


    private void SetupControls()
    {
        usSites.AllowAll = AllowAll;
        usSites.AllowEmpty = AllowEmpty;
        AllowGlobal = AllowGlobal;

        // Set resource strings for current mode
        usSites.ResourcePrefix = (AllowMultipleSelection) ? "sitesselect" : "siteselect";

        // Use sitenames as values
        if ((FieldInfo != null) && (FieldInfo.DataType == FormFieldDataTypeEnum.Integer))
        {
            usSites.ReturnColumnName = "SiteID";
            usSites.SelectionMode = SelectionModeEnum.SingleDropDownList;
        }
        else
        {
            usSites.ReturnColumnName = "SiteName";
        }
    }


    /// <summary>
    /// Handles the OnSelectionChanged event of the usSites control.
    /// </summary>
    private void usSites_OnSelectionChanged(object sender, EventArgs e)
    {
        RaiseOnChanged();
    }


    /// <summary>
    /// Creates child controls and loads update panle container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load updat panel container
        if (usSites == null)
        {
            pnlUpdate.LoadContainer();
        }

        // Call base method
        base.CreateChildControls();
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        // Return where condition for integer
        if ((FieldInfo != null) && (FieldInfo.DataType == FormFieldDataTypeEnum.Integer))
        {
            if (ValidationHelper.GetInteger(usSites.Value, UniSelector.US_NONE_RECORD) == UniSelector.US_NONE_RECORD)
            {
                return FieldInfo.Name + " IS NULL";
            }
            else
            {
                return String.Format("{0} = {1}", FieldInfo.Name, usSites.Value);
            }
        }
        // Return where condition for string
        else
        {
            if (String.IsNullOrEmpty(ValidationHelper.GetString(usSites.Value, null)))
            {
                return String.Format("{0} IS NULL OR {0} = N''", FieldInfo.Name);
            }
            else
            {
                return String.Format("{0} = N'{1}'", FieldInfo.Name, usSites.Value);
            }
        }
    }

    #endregion
}