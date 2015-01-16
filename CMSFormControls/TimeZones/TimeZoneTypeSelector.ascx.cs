using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SiteProvider;

public partial class CMSFormControls_TimeZones_TimeZoneTypeSelector : FormEngineUserControl
{
    #region "Variables"

    private TimeZoneTypeEnum mTimeZoneType;

    #endregion


    #region "Properties"

    /// <summary>
    /// Value override.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpTimeZoneTypeSelector.SelectedValue;
        }
        set
        {
            drpTimeZoneTypeSelector.SelectedValue = (string)value;
            TimeZoneType = TimeZoneInfoProvider.GetTimeZoneTypeEnum((string)value);
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
            base.Enabled = value;
            drpTimeZoneTypeSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Type of timezone.
    /// </summary>
    public TimeZoneTypeEnum TimeZoneType
    {
        get
        {
            return mTimeZoneType;
        }
        set
        {
            mTimeZoneType = value;
        }
    }


    /// <summary>
    /// Inner dropdownlist with time zone types.
    /// </summary>
    public DropDownList TimeZoneTypeDropDownList
    {
        get
        {
            return drpTimeZoneTypeSelector;
        }
    }

    #endregion


    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
    }


    /// <summary>
    /// CreateChildControls() override.
    /// </summary>
    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        if (!StopProcessing)
        {
            ReloadData();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Fills dropdownlist with values and realoades it.
    /// </summary>
    public void ReloadData()
    {
        drpTimeZoneTypeSelector.Items.Clear();
        DataHelper.FillListControlWithEnum(typeof(TimeZoneTypeEnum), drpTimeZoneTypeSelector, "timez.type", TimeZoneInfoProvider.GetTimeZoneTypeString);

        string tmp = TimeZoneInfoProvider.GetTimeZoneTypeString(TimeZoneType);
        if (drpTimeZoneTypeSelector.Items.FindByValue(tmp) != null)
        {
            drpTimeZoneTypeSelector.SelectedValue = tmp;
        }
        else
        {
            drpTimeZoneTypeSelector.SelectedValue = null;
            TimeZoneType = TimeZoneTypeEnum.Inherit;
        }
    }

    #endregion
}