using System;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Membership_FormControls_Users_SelectUser : FormEngineUserControl
{
    #region "Variables"

    private string mResourcePrefix = String.Empty;
    private bool mAllowAll = false;
    private bool mAllowDefault = false;
    private bool mAllowEmpty = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets ID of the site. User of this site are displayed
    /// Use 0 for current siteid, -1 for all sites(no filter)
    /// Note: SiteID is not used if site filter is enabled
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the id of the group to display.
    /// </summary>
    public int GroupID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets ID of the role. User of this role are displayed.
    /// </summary>
    public int RoleID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value which determines whether to allow more than one user to select.
    /// </summary>
    public SelectionModeEnum SelectionMode
    {
        get
        {
            EnsureChildControls();
            return usUsers.SelectionMode;
        }
        set
        {
            EnsureChildControls();
            usUsers.SelectionMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the text displayed if there are no data.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            EnsureChildControls();
            return usUsers.ZeroRowsText;
        }
        set
        {
            EnsureChildControls();
            usUsers.ZeroRowsText = value;
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
            usUsers.Enabled = value;
        }
    }


    ///<summary>
    /// Gets or sets field value.
    ///</summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return usUsers.Value;
        }
        set
        {
            EnsureChildControls();
            usUsers.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets if site filter should be shown or not.
    /// </summary>
    public bool ShowSiteFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSiteFilter"), true);
        }
        set
        {
            SetValue("ShowSiteFilter", value);
        }
    }


    /// <summary>
    /// Gets the current UniSelector instance.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return usUsers;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the UniSelector should apply WhereCondition for the selected value (default: false). This does not affect the modal dialog.
    /// </summary>
    public bool ApplyValueRestrictions
    {
        get
        {
            EnsureChildControls();
            return usUsers.ApplyValueRestrictions;
        }
        set
        {
            EnsureChildControls();
            usUsers.ApplyValueRestrictions = value;
        }
    }


    /// <summary>
    /// Gets the single select drop down field.
    /// </summary>
    public DropDownList DropDownSingleSelect
    {
        get
        {
            return usUsers.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets or sets the resource prefix of uni-selector. If not set default values are used.
    /// </summary>
    public string ResourcePrefix
    {
        get
        {
            return mResourcePrefix;
        }
        set
        {
            mResourcePrefix = value;
            usUsers.ResourcePrefix = value;
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
            usUsers.IsLiveSite = value;
        }
    }


    /// <summary>
    /// If enabled disabled users aren't shown in selector.
    /// </summary>
    public bool HideDisabledUsers
    {
        get;
        set;
    }


    /// <summary>
    /// If enabled hidden users aren't shown in selector (does not apply to global administrators, these are shown regardless of this property).
    /// </summary>
    public bool HideHiddenUsers
    {
        get;
        set;
    }


    /// <summary>
    /// If enabled non-approved users aren't shown is selector.
    /// </summary>
    public bool HideNonApprovedUsers
    {
        get;
        set;
    }


    /// <summary>
    /// If true selector will always display users from all sites.
    /// Suited for selecting users having access to the site.
    /// </summary>
    public bool DisplayUsersFromAllSites
    {
        get;
        set;
    }


    /// <summary>
    /// Includes global administrators into site users.
    /// </summary>
    public bool TreatGlobalAdminsAsSiteUsers
    {
        get;
        set;
    }


    /// <summary>
    /// Include global administrators no matter what.
    /// </summary>
    public bool AlwaysShowGlobalAdministrators
    {
        get;
        set;
    }


    /// <summary>
    /// Enables or disables (all) item in selector.
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return mAllowAll;
        }
        set
        {
            mAllowAll = value;
            EnsureChildControls();
            usUsers.AllowAll = value;
        }
    }


    /// <summary>
    /// Enables or disables (empty) item in selector.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return mAllowEmpty;
        }
        set
        {
            mAllowEmpty = value;
            EnsureChildControls();
            usUsers.AllowEmpty = value;
        }
    }


    /// <summary>
    /// Enables or disables (default) item in selector.
    /// </summary>
    public bool AllowDefault
    {
        get
        {
            return mAllowDefault;
        }
        set
        {
            mAllowDefault = value;
            EnsureChildControls();
            usUsers.AllowDefault = value;
        }
    }


    /// <summary>
    /// Additional users to show identified by user IDs.
    /// </summary>
    public int[] AdditionalUsers
    {
        get
        {
            return (int[])GetValue("AdditionalUsers");
        }
        set
        {
            SetValue("AdditionalUsers", value);
        }
    }


    /// <summary>
    /// Where condition used to filter control data.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        // Set properties names
        object[,] values = new object[1, 2];
        values[0, 0] = "DisplayName";
        values[0, 1] = usUsers.DisplayNameFormat;
        return values;
    }


    protected override void OnInit(EventArgs e)
    {
        usUsers.DisplayNameFormat = UniSelector.USER_DISPLAY_FORMAT;
        usUsers.UniGrid.Pager.UniPager.PageSize = 10;

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControls();

            // Set 'cms.user' icon. No icon for 'cms.userlist'
            usUsers.IconPath = GetObjectIconUrl("cms.user", null);

            ReloadData();

            // Reload data again, if page changed
            if (usUsers.UniGrid.Pager != null && usUsers.UniGrid.Pager.UniPager != null)
            {
                usUsers.UniGrid.Pager.UniPager.OnPageChanged += new UniPager.PageChangedHandler(UniPager_OnPageChanged);
            }
        }
        else
        {
            usUsers.StopProcessing = true;
        }
    }


    private void UniPager_OnPageChanged(object sender, int pageNumber)
    {
        ReloadData();
    }


    protected void SetupControls()
    {
        usUsers.WhereCondition = null;

        // If current control context is widget or livesite hide site selector
        if (ControlsHelper.CheckControlContext(this, ControlContext.WIDGET_PROPERTIES) || ControlsHelper.CheckControlContext(this, ControlContext.LIVE_SITE))
        {
            ShowSiteFilter = false;
        }

        // Set prefix if not set
        if (ResourcePrefix == String.Empty)
        {
            // Set resource prefix based on mode
            if ((SelectionMode == SelectionModeEnum.Multiple) || (SelectionMode == SelectionModeEnum.MultipleButton) || (SelectionMode == SelectionModeEnum.MultipleTextBox))
            {
                usUsers.ResourcePrefix = "selectusers";
            }
        }

        // Add sites filter
        if (ShowSiteFilter)
        {
            usUsers.FilterControl = "~/CMSFormControls/Filters/SiteFilter.ascx";
            usUsers.SetValue("DefaultFilterValue", (SiteID > 0) ? SiteID : CMSContext.CurrentSiteID);
            usUsers.SetValue("FilterMode", "user");
        }


        // Hide hidden users
        if (HideHiddenUsers)
        {
            const string where = "((UserIsHidden IS NULL) OR (UserIsHidden = 0))";
            usUsers.WhereCondition = SqlHelperClass.AddWhereCondition(usUsers.WhereCondition, where);
        }

        // Hide disabled users
        if (HideDisabledUsers)
        {
            const string where = UserInfoProvider.USER_ENABLED_WHERE_CONDITION;
            usUsers.WhereCondition = SqlHelperClass.AddWhereCondition(usUsers.WhereCondition, where);
        }

        // Hide non-approved users
        if (HideNonApprovedUsers)
        {
            const string where =
                "UserID IN (SELECT UserSettingsUserID FROM CMS_UserSettings WHERE ((UserWaitingForApproval IS NULL) OR (UserWaitingForApproval = 0)))";
            usUsers.WhereCondition = SqlHelperClass.AddWhereCondition(usUsers.WhereCondition, where);
        }

        // Select group users
        if (GroupID > 0)
        {
            string where = "UserID IN (SELECT MemberUserID FROM Community_GroupMember WHERE MemberGroupID = " + GroupID + ")";
            usUsers.WhereCondition = SqlHelperClass.AddWhereCondition(usUsers.WhereCondition, where);

            // Hide non-approved group users
            if (HideNonApprovedUsers)
            {
                string whereNonApproved =
                    "UserID IN (SELECT MemberUserID FROM Community_GroupMember WHERE (MemberGroupID = " + GroupID + ") AND (MemberRejectedWhen IS NULL))";
                usUsers.WhereCondition = SqlHelperClass.AddWhereCondition(usUsers.WhereCondition, whereNonApproved);
            }
        }

        // Select users in role
        if (RoleID > 0)
        {
            string where = "UserID IN (SELECT UserID FROM CMS_UserRole WHERE RoleID = " + RoleID + ")";
            usUsers.WhereCondition = SqlHelperClass.AddWhereCondition(usUsers.WhereCondition, where);
        }

        // Select users depending on site; if filter enabled, where condition is added from filter itself
        if (!ShowSiteFilter && (SiteID >= 0) && !DisplayUsersFromAllSites)
        {
            int siteId = (SiteID == 0) ? CMSContext.CurrentSiteID : SiteID;
            string where = "UserID IN (SELECT UserID FROM CMS_UserSite WHERE SiteID = " + siteId + ")";
            if (TreatGlobalAdminsAsSiteUsers)
            {
                where = SqlHelperClass.AddWhereCondition(where, "UserIsGlobalAdministrator=1", "OR");
            }
            usUsers.WhereCondition = SqlHelperClass.AddWhereCondition(usUsers.WhereCondition, where);
        }

        if (AlwaysShowGlobalAdministrators)
        {
            usUsers.WhereCondition = SqlHelperClass.AddWhereCondition(usUsers.WhereCondition, "UserIsGlobalAdministrator=1", "OR");
        }

        // Add additional users
        if ((AdditionalUsers != null) && (AdditionalUsers.Length > 0))
        {
            usUsers.WhereCondition = SqlHelperClass.AddWhereCondition(usUsers.WhereCondition, SqlHelperClass.GetWhereCondition("UserID", AdditionalUsers), "OR");
        }

        // Control where condition
        if (!String.IsNullOrEmpty(WhereCondition))
        {
            usUsers.WhereCondition = SqlHelperClass.AddWhereCondition(usUsers.WhereCondition, WhereCondition);
        }

        pnlUpdate.ShowProgress = (SelectionMode == SelectionModeEnum.Multiple);
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load update panel container
        if (usUsers == null)
        {
            pnlUpdate.LoadContainer();
        }
        // Call base method
        base.CreateChildControls();
    }


    /// <summary>
    /// Reloads the data of the UniSelector.
    /// </summary>
    public void ReloadData()
    {
        usUsers.Reload(true);
        pnlUpdate.Update();
    }


    /// <summary>
    /// Reloads whole control including control settings and data.
    /// </summary>
    public void Reload()
    {
        SetupControls();
        ReloadData();
    }

    #endregion
}