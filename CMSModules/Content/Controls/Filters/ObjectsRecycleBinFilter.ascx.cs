using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSModules_Content_Controls_Filters_ObjectsRecycleBinFilter : CMSAbstractBaseFilterControl
{
    #region "Private variables"

    private bool mIsSingleSite = false;
    private bool mLoadObjTypes = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Returns selected user identifier.
    /// </summary>
    public int SelectedUser
    {
        get
        {
            Control postbackControl = ControlsHelper.GetPostBackControl(Page);
            return ValidationHelper.GetInteger((postbackControl == btnShow) ? userSelector.Value : ViewState["SelectedUser"], CMSContext.CurrentUser.UserID);
        }
        private set
        {
            userSelector.Value = value;
            ViewState["SelectedUser"] = value;
            WhereCondition = CreateWhereCondition(base.WhereCondition);
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            Control postbackControl = ControlsHelper.GetPostBackControl(Page);
            return DataHelper.GetNotEmpty((postbackControl == btnShow) ? CreateWhereCondition(base.WhereCondition) : ViewState["WhereCondition"], string.Empty);
        }
        set
        {
            base.WhereCondition = value;
            ViewState["WhereCondition"] = value;
        }
    }


    /// <summary>
    /// Determines whether filter is set.
    /// </summary>
    public bool FilterIsSet
    {
        get
        {
            int userId = ValidationHelper.GetInteger(userSelector.Value, 0);
            string objType = ValidationHelper.GetString(objTypeSelector.Value, "");
            return nameFilter.FilterIsSet || !String.IsNullOrEmpty(objType) || ((userId > 0) && UsersPlaceHolder.Visible);
        }
    }


    /// <summary>
    /// Gets place holder with user selector.
    /// </summary>
    public PlaceHolder UsersPlaceHolder
    {
        get
        {
            return plcUsers;
        }
    }


    /// <summary>
    /// Site ID to filter usable values.
    /// </summary>
    public int SiteID
    {
        get
        {
            return userSelector.SiteID;
        }
        set
        {
            userSelector.SiteID = value;
        }
    }


    /// <summary>
    /// Indicates if all item should be displayed in user selector.
    /// </summary>
    public bool IsSingleSite
    {
        get
        {
            return mIsSingleSite;
        }
        set
        {
            mIsSingleSite = value;
        }
    }


    /// <summary>
    /// Indicates if all available users should be displayed.
    /// </summary>
    public bool DisplayUsersFromAllSites
    {
        get
        {
            return userSelector.DisplayUsersFromAllSites;
        }
        set
        {
            userSelector.DisplayUsersFromAllSites = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page Init event.
    /// </summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            // Preselect default value
            SelectedUser = CMSContext.CurrentUser.UserID;
        }
    }


    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        userSelector.DropDownSingleSelect.AutoPostBack = true;

        if (!RequestHelper.IsCallback())
        {
            userSelector.DropDownSingleSelect.Width = new Unit(305);
            objTypeSelector.DropDownListControl.Width = new Unit(305);
        }

        if (!RequestHelper.IsPostBack())
        {
            // Preselect default value
            SelectedUser = CMSContext.CurrentUser.UserID;
        }
    }


    /// <summary>
    /// Page pre render event.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (mLoadObjTypes)
        {
            SetObjTypeSelector();
            objTypeSelector.Reload(true);
        }
        pnlObjType.Update();
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Reload control data.
    /// </summary>
    public void ReloadData()
    {
        SetObjTypeSelector();
        objTypeSelector.Reload(true);
        mLoadObjTypes = false;
        WhereCondition = null;
    }


    private string CreateWhereCondition(string originalWhere)
    {
        string where = originalWhere;
        // Add where conditions from filters
        where = SqlHelperClass.AddWhereCondition(where, nameFilter.WhereCondition);

        string objType = SqlHelperClass.GetSafeQueryString(ValidationHelper.GetString(objTypeSelector.Value, ""), false);
        if (!string.IsNullOrEmpty(objType))
        {
            where = SqlHelperClass.AddWhereCondition(where, "VersionObjectType LIKE N'" + objType + "'");
        }

        int userId = ValidationHelper.GetInteger(userSelector.Value, 0);
        if (userId > 0)
        {
            where = SqlHelperClass.AddWhereCondition(where, "VersionDeletedByUserID = " + userId);
        }

        return where;
    }



    /// <summary>
    /// Set object type selector properties
    /// </summary>
    protected void SetObjTypeSelector()
    {
        objTypeSelector.UserID = ValidationHelper.GetInteger(userSelector.Value, 0);
        if (SiteID > 0)
        {
            objTypeSelector.WhereCondition = "VersionObjectSiteID = " + SiteID;
            if (IsSingleSite)
            {
                objTypeSelector.WhereCondition = SqlHelperClass.AddWhereCondition(objTypeSelector.WhereCondition, "VersionObjectSiteID IS NULL", "OR");
            }
        }
        else if ((SiteID == 0) && !IsSingleSite)
        {
            objTypeSelector.WhereCondition = SqlHelperClass.AddWhereCondition(objTypeSelector.WhereCondition, "VersionObjectSiteID IS NULL");
        }
    }

    #endregion


    #region "Control events"

    protected void btnShow_Click(object sender, EventArgs e)
    {
        SelectedUser = ValidationHelper.GetInteger(userSelector.Value, CMSContext.CurrentUser.UserID);
    }

    #endregion
}