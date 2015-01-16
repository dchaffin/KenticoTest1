using System;

using CMS.FormControls;
using CMS.Controls;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_FormControls_FormControls_DataTypeFilter : CMSAbstractBaseFilterControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpTypeSelector.Value;
        }
        set
        {
            drpTypeSelector.Value = value;
        }
    }


    public override string WhereCondition
    {
        get
        {
            return (drpTypeSelector.Value.ToString() != UniSelector.US_ALL_RECORDS.ToString() ? "UserControlType = " + drpTypeSelector.Value : String.Empty);
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        drpTypeSelector.OnFilterChanged += new ActionEventHandler(drpTypeSelector_OnFilterChanged);
    }


    protected void drpTypeSelector_OnFilterChanged()
    {
        FilterChanged = true; 
        RaiseOnFilterChanged();
    }

    #endregion
}
