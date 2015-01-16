using System;
using System.Web.UI;

using CMS.Controls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.CMSHelper;

public partial class CMSFormControls_Filters_SiteFilter : CMSAbstractBaseFilterControl
{
    #region "Variables"

    private CMSUserControl filteredControl = null;
    private string mFilterMode = null;
    public bool mIsLiveSite = true;


    #endregion


    #region "Properties"

    /// <summary>
    /// Current filter mode.
    /// </summary>
    public override string FilterMode
    {
        get
        {
            return mFilterMode ?? (mFilterMode = ValidationHelper.GetString(filteredControl.GetValue("FilterMode"), "").ToLowerCSafe());
        }
        set
        {
            mFilterMode = value;
        }
    }


    /// <summary>
    /// Enables or disables live site mode of site selector.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return mIsLiveSite;
        }
        set
        {
            mIsLiveSite = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            int selectedSite = ValidationHelper.GetInteger(siteSelector.Value, 0);
            base.WhereCondition = GenerateWhereCondition(selectedSite);
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Filter value.
    /// </summary>
    public override object Value
    {
        get
        {
            return siteSelector.Value;
        }
        set
        {
            SetValue(value);
        }
    }


    /// <summary>
    /// Indicates if label should be displayed.
    /// </summary>
    public bool ShowLabel
    {
        get
        {
            return plcLabel.Visible;
        }
        set
        {
            plcLabel.Visible = value;
        }
    }


    /// <summary>
    /// Gets inner uniselector.
    /// </summary>
    public UniSelector Selector
    {
        get
        {
            return siteSelector.UniSelector;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        filteredControl = FilteredControl as CMSUserControl;

        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.UniSelector.OnSelectionChanged += Site_Changed;
        siteSelector.UniSelector.DialogWindowName = "SiteSelectionDialog";
        siteSelector.IsLiveSite = IsLiveSite;

        // All cultures field in cultures mode
        switch (FilterMode)
        {
            case "cultures":
                {
                    siteSelector.AllowEmpty = false;
                    siteSelector.AllowAll = false;
                    siteSelector.UniSelector.SpecialFields = new string[,] { { ResHelper.GetString("general.allcultures"), string.Empty } };
                }
                break;

            case "role":
                {
                    siteSelector.AllowEmpty = false;
                    siteSelector.AllowAll = false;
                    siteSelector.AllowGlobal = true;
                }
                break;

            case "user":
                {
                    siteSelector.AllowAll = true;
                    siteSelector.AllowEmpty = false;
                }
                break;

            case "notificationtemplate":
                {
                    siteSelector.AllowEmpty = false;
                    siteSelector.AllowAll = false;
                }
                break;

            case "notificationtemplateglobal":
                {
                    siteSelector.AllowEmpty = false;
                    siteSelector.AllowAll = false;
                    siteSelector.UniSelector.SpecialFields = new string[,] { { ResHelper.GetString("general.global"), "" } };
                }
                break;

            case "department":
                {
                    siteSelector.AllowEmpty = false;
                    siteSelector.AllowAll = false;
                    siteSelector.AllowGlobal = true;
                }
                break;

            default:
                {
                    if ((Parameters != null) && (Parameters["ObjectType"] != null))
                    {
                        // Get object type
                        GeneralizedInfo currentObject = CMSObjectHelper.GetObject(ValidationHelper.GetString(Parameters["ObjectType"], String.Empty));
                        if (currentObject != null)
                        {
                            // Show global value if supports global objects
                            if (currentObject.TypeInfo.SupportsGlobalObjects)
                            {
                                siteSelector.AllowGlobal = true;
                            }
                            siteSelector.AllowAll = false;
                            siteSelector.AllowEmpty = false;
                            siteSelector.Value = CMSContext.CurrentSiteID;
                        }
                    }
                    else
                    {
                        // Use default settings
                        siteSelector.AllowAll = true;
                        siteSelector.AllowEmpty = false;
                        plcLabel.Visible = false;
                    }
                }
                break;
        }

        // Set initial filter value
        if (!RequestHelper.IsPostBack())
        {
            if (filteredControl != null)
            {
                int defaultValue = ValidationHelper.GetInteger(filteredControl.GetValue("DefaultFilterValue"), 0);

                if (defaultValue > 0)
                {
                    siteSelector.UniSelector.Value = defaultValue;
                    WhereCondition = GenerateWhereCondition(defaultValue);
                }
            }
        }
    }


    /// <summary>
    /// OnLoad override - check whether filter is set.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        lblSite.ResourceString = "general.site";
        lblSite.DisplayColon = true;

        if (siteSelector.UniSelector.HasData)
        {
            int selectedSite = ValidationHelper.GetInteger(siteSelector.Value, 0);

            // Generate current where condition 
            WhereCondition = GenerateWhereCondition(selectedSite);
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Generates where condition.
    /// </summary>
    protected string GenerateWhereCondition(int siteId)
    {
        switch (FilterMode)
        {
            case "user":
                {
                    // If some site selected filter users
                    if (siteId > 0)
                    {
                        return "UserID IN (SELECT UserID FROM CMS_UserSite WHERE SiteID = " + siteId + ")";
                    }
                }
                break;

            case "role":
                {
                    // If some site selected filter users
                    if (siteId > 0)
                    {
                        return "SiteID = " + siteId;
                    }
                    if (siteId.ToString() == siteSelector.GlobalRecordValue)
                    {
                        return "SiteID IS NULL";
                    }
                }
                break;

            case "subscriber":
                {
                    // If some site filters subscibers
                    if (siteId > 0)
                    {
                        return "SubscriberSiteID = " + siteId;
                    }
                }
                break;

            case "cultures":
                {
                    if (siteId > 0)
                    {
                        return "CultureID IN (SELECT CultureID FROM CMS_SiteCulture WHERE SiteID = " + siteId + ")";
                    }
                }
                break;

            case "bizform":
                {
                    if (siteId > 0)
                    {
                        return "FormSiteID = " + siteId;
                    }
                }
                break;

            case "notificationtemplate":
            case "notificationtemplateglobal":
                {
                    string where = "";

                    // Set the prefix for the item
                    SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
                    if (si != null)
                    {
                        filteredControl.SetValue("ItemPrefix", si.SiteName + ".");
                        where = "TemplateSiteID = " + siteId;
                    }
                    // Add global templates
                    else
                    {
                        filteredControl.SetValue("ItemPrefix", null);
                        where = "TemplateSiteID IS NULL";
                    }

                    return where;
                }

            case "department":
                {
                    // If some site selected filter departments
                    if (siteId > 0)
                    {
                        return "DepartmentSiteID = " + siteId;
                    }
                    if (siteId.ToString() == siteSelector.GlobalRecordValue)
                    {
                        return "DepartmentSiteID IS NULL";
                    }
                }
                break;

            default:
                {
                    // Automatic filtering mode
                    if ((siteId > 0) || (siteId == UniSelector.US_GLOBAL_RECORD))
                    {
                        IObjectTypeDriven filtered = this.FilteredControl as IObjectTypeDriven;
                        if (filtered != null)
                        {
                            BaseInfo infoObj = CMSObjectHelper.GetReadOnlyObject(filtered.ObjectType);
                            return infoObj.TypeInfo.GetSiteWhereCondition(siteId, false);
                        }
                    }
                }
                break;
        }

        return String.Empty;
    }


    private void SetValue(object value)
    {
        try
        {
            siteSelector.Value = value;
        }
        catch
        {
        }
    }

    #endregion


    #region "Other events"

    /// <summary>
    /// Handles Site selector OnSelectionChanged event.
    /// </summary>
    protected void Site_Changed(object sender, EventArgs e)
    {
        // Raise OnFilterChange event
        RaiseOnFilterChanged();
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to default state.
    /// </summary>
    public override void ResetFilter()
    {
        if (filteredControl != null)
        {
            int defaultValue = ValidationHelper.GetInteger(filteredControl.GetValue("DefaultFilterValue"), 0);
            siteSelector.SiteID = defaultValue;
            siteSelector.Reload(true);
            WhereCondition = GenerateWhereCondition(defaultValue);
        }
    }

    #endregion
}