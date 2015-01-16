using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Permissions_Controls_PermissionsMatrix : CMSAdminControl
{
    #region "Constants"

    /// <summary>
    /// CSS class for highlighted permission matrix rows.
    /// </summary>
    private const string HIGHLIGHTED_ROW_CSS = "Highlighted";

    #endregion


    #region "Variables"

    private string mSelectedId = string.Empty;
    private string mSelectedType = string.Empty;
    private int mSiteId = 0;
    private int mRoleId = 0;
    private bool mGlobalRoles = false;

    private UserInfo mSelectedUser = null;
    private SiteInfo mSelectedSite = null;

    private CurrentUserInfo currentUser = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
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
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets Site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets Role ID.
    /// </summary>
    public int RoleID
    {
        get
        {
            return mRoleId;
        }
        set
        {
            mRoleId = value;
        }
    }


    /// <summary>
    /// Gets or sets ID selected in the moduleSelector or docTypeSelector or customTableSelector according to the selected PermissionType.
    /// </summary>
    public string SelectedID
    {
        get
        {
            return mSelectedId;
        }
        set
        {
            mSelectedId = value;
        }
    }


    /// <summary>
    /// Gets or sets type constant according to the selected value in the moduleSelector or docTypeSelector or customTableSelector and the selected PermissionType.
    /// </summary>
    public string SelectedType
    {
        get
        {
            return mSelectedType;
        }
        set
        {
            mSelectedType = value;
        }
    }


    /// <summary>
    /// Currently selected user ID for report
    /// </summary>
    public int SelectedUserID
    {
        get;
        set;
    }


    /// <summary>
    /// Query name to get the data.
    /// </summary>
    public string QueryName
    {
        get
        {
            return gridMatrix.QueryName;
        }
        set
        {
            gridMatrix.QueryName = value;
        }
    }


    /// <summary>
    /// ID column of the row.
    /// </summary>
    public string RowItemIDColumn
    {
        get
        {
            return gridMatrix.RowItemIDColumn;
        }
        set
        {
            gridMatrix.RowItemIDColumn = value;
        }
    }


    /// <summary>
    /// Display name column of the row.
    /// </summary>
    public string RowItemDisplayNameColumn
    {
        get
        {
            return gridMatrix.RowItemDisplayNameColumn;
        }
        set
        {
            gridMatrix.RowItemDisplayNameColumn = value;
        }
    }


    /// <summary>
    /// Tooltip column of the row.
    /// </summary>
    public string RowItemTooltipColumn
    {
        get
        {
            return gridMatrix.RowItemTooltipColumn;
        }
        set
        {
            gridMatrix.RowItemTooltipColumn = value;
        }
    }


    /// <summary>
    /// Display name column of the column.
    /// </summary>
    public string ColumnItemDisplayNameColumn
    {
        get
        {
            return gridMatrix.ColumnItemDisplayNameColumn;
        }
        set
        {
            gridMatrix.ColumnItemDisplayNameColumn = value;
        }
    }


    /// <summary>
    /// ID column of the column.
    /// </summary>
    public string ColumnItemIDColumn
    {
        get
        {
            return gridMatrix.ColumnItemIDColumn;
        }
        set
        {
            gridMatrix.ColumnItemIDColumn = value;
        }
    }


    /// <summary>
    /// Tooltip column of the column.
    /// </summary>
    public string ColumnItemTooltipColumn
    {
        get
        {
            return gridMatrix.ColumnItemTooltipColumn;
        }
        set
        {
            gridMatrix.ColumnItemTooltipColumn = value;
        }
    }


    /// <summary>
    /// Tooltip column of the item.
    /// </summary>
    public string ItemTooltipColumn
    {
        get
        {
            return gridMatrix.ItemTooltipColumn;
        }
        set
        {
            gridMatrix.ItemTooltipColumn = value;
        }
    }


    /// <summary>
    /// Gets or sets the text displayed in the upper left corner, if filter is not shown.
    /// </summary>
    public string CornerText
    {
        get
        {
            return gridMatrix.CornerText;
        }
        set
        {
            gridMatrix.CornerText = value;
        }
    }


    /// <summary>
    /// Indicates whether show global roles.
    /// </summary>
    public bool GlobalRoles
    {
        get
        {
            return mGlobalRoles;
        }
        set
        {
            mGlobalRoles = value;
        }
    }


    /// <summary>
    /// Gets UserInfo object for selected user ID.
    /// </summary>
    private UserInfo SelectedUser
    {
        get
        {
            if (mSelectedUser == null)
            {
                mSelectedUser = UserInfoProvider.GetUserInfo(SelectedUserID);
            }
            return mSelectedUser;
        }
    }


    /// <summary>
    /// Gets SiteInfo object for selected site ID.
    /// </summary>
    private SiteInfo SelectedSite
    {
        get
        {
            if (mSelectedSite == null)
            {
                mSelectedSite = SiteInfoProvider.GetSiteInfo(SiteID);
            }
            return mSelectedSite;
        }
    }


    /// <summary>
    /// Indicates if only selected user roles should be displayed.
    /// </summary>
    public bool UserRolesOnly
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if filter was changed.
    /// </summary>
    public bool FilterChanged
    {
        get;
        set;
    }


    /// <summary>
    /// If true, the permissions are used as rows
    /// </summary>
    public bool PermissionsAsRows
    {
        get;
        set;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs when data has been loaded. Allows manipulation with data.
    /// </summary>
    public delegate void OnMatrixDataLoaded(DataSet ds);

    public event OnMatrixDataLoaded OnDataLoaded;

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register js script
        string script = "function NA() {alert(" + ScriptHelper.GetString(GetString("Administration-Permissions_Matrix.NotAdjustable")) + ");}";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "PermissionMatrix_NA", ScriptHelper.GetScript(script));

        gridMatrix.OrderBy = "Matrix.RoleDisplayName, Matrix.RoleID, Matrix.PermissionOrder";
        gridMatrix.ContentBefore = @"<table class=""PermissionMatrix"" cellspacing=""0"" cellpadding=""0"" rules=""rows"" border=""1"" style=""border-collapse:collapse;"">";
        gridMatrix.OnItemChanged += gridMatrix_OnItemChanged;
        gridMatrix.DataLoaded += gridMatrix_DataLoaded;

        if (PermissionsAsRows)
        {
            gridMatrix.CheckRowPermissions += gridMatrix_CheckPermissions;
            gridMatrix.DisabledRowMark = " <img class=\"InlineIcon\" src=\"" + GetImageUrl("/General/globaladmin.png") + "\" alt=\"" + ResHelper.GetString("PermissionMatrix.GlobalAdminOnly") + "\" title=\"" + ResHelper.GetString("PermissionMatrix.GlobalAdminOnly") + "\" />";
        }
        else
        {
            gridMatrix.CheckColumnPermissions += gridMatrix_CheckPermissions;
            gridMatrix.DisabledColumnMark = " <img class=\"InlineIcon\" src=\"" + GetImageUrl("/General/globaladmin.png") + "\" alt=\"" + ResHelper.GetString("PermissionMatrix.GlobalAdminOnly") + "\" title=\"" + ResHelper.GetString("PermissionMatrix.GlobalAdminOnly") + "\" />";
        }

        gridMatrix.ContentBeforeRowsCssClass = "ContentBefore";
        gridMatrix.OnGetRowItemCssClass += gridMatrix_OnGetRowItemCssClass;
        gridMatrix.NoRecordsMessage = GetString("general.emptymatrix");

        currentUser = CMSContext.CurrentUser;

        CheckPermissions();
    }


    protected bool gridMatrix_CheckPermissions(object permId)
    {
        int permissionId = ValidationHelper.GetInteger(permId, 0);

        // Check how the permission can be edited
        PermissionNameInfo pni = PermissionNameInfoProvider.GetPermissionNameInfo(permissionId);
        if (pni != null)
        {
            return currentUser.UserSiteManagerAdmin || !pni.PermissionEditableByGlobalAdmin;
        }

        return true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Initialize value dependant properties of uni matrix
        gridMatrix.ShowContentBeforeRows = SelectedUserID > 0;
        gridMatrix.QueryName = GetQueryNameByType();
        gridMatrix.QueryParameters = GetQueryParameters();
        gridMatrix.WhereCondition = GetWhereCondition();

        if (FilterChanged || (gridMatrix.Pager.CurrentPage <= 0))
        {
            gridMatrix.Pager.CurrentPage = 1;
        }
        if (gridMatrix.Pager.CurrentPage > gridMatrix.Pager.PageCount)
        {
            gridMatrix.Pager.CurrentPage = gridMatrix.Pager.PageCount;
        }

        if (!gridMatrix.HasData)
        {
            if (UserRolesOnly)
            {
                lblInfo.Text = GetString("general.norolemember");
            }
        }
        else
        {
            // Inform user that global admin was selected
            if ((SelectedUserID > 0) && SelectedUser.IsGlobalAdministrator)
            {
                lblGlobalAdmin.Text = GetString("Administration-Permissions_Matrix.GlobalAdministrator");
            }
        }
        lblInfo.Visible = !string.IsNullOrEmpty(lblInfo.Text);
        lblGlobalAdmin.Visible = !string.IsNullOrEmpty(lblGlobalAdmin.Text);

        // Set content before rows and refresh matrix content
        gridMatrix.ContentBeforeRows = GetBeforeRowsContent(SiteID, ValidationHelper.GetInteger(SelectedID, 0), SelectedType, SelectedUserID);
        pnlUpdMat.Update();

        base.OnPreRender(e);
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Gets where condition for the matrix according to the RoleID.
    /// </summary>
    /// <returns>String representing where condition for the matrix.</returns>
    private string GetWhereCondition()
    {
        string where = null;
        if (RoleID > 0)
        {
            where = string.Format("RoleGroupID IS NULL AND RoleID = {0}", RoleID);
        }
        else
        {
            where = "RoleGroupID IS NULL";
        }

        if (UserRolesOnly && (SelectedUserID > 0))
        {
            // Get selected site name
            string siteName = null;
            if (SiteID > 0)
            {
                siteName = SelectedSite.SiteName.ToLowerCSafe();
            }
            else
            {
                siteName = UserInfo.GLOBAL_ROLES_KEY;
            }

            string rolesWhere = SelectedUser.GetRoleIdList((SiteID <= 0), true, siteName);

            // Add roles where condition
            if (!String.IsNullOrEmpty(rolesWhere))
            {
                where = SqlHelperClass.AddWhereCondition(where, "RoleID IN(" + rolesWhere + ")");
            }
            else
            {
                where = SqlHelperClass.NO_DATA_WHERE;
                gridMatrix.StopProcessing = true;
            }
        }

        return where;
    }


    /// <summary>
    /// Checks manage permission and enable or disable matrix according to that permission.
    /// </summary>
    private void CheckPermissions()
    {
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Permissions", "Manage"))
        {
            gridMatrix.Enabled = false;
            ShowWarning(string.Format(GetString("CMSSiteManager.AccessDeniedOnPermissionName"), "Manage"), null, null);
        }
    }


    /// <summary>
    /// Gets query parameters for the permission matrix.
    /// </summary>
    /// <returns>Two dimensional object array of query parameters.</returns>
    private QueryDataParameters GetQueryParameters()
    {
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@ID", SelectedID);
        parameters.Add("@SiteID", (GlobalRoles ? 0 : SiteID == 0 ? CMSContext.CurrentSiteID : SiteID));
        parameters.Add("@DisplayInMatrix", true);

        return parameters;
    }


    /// <summary>
    /// Gets name of the query which will be used for the matrix according to the selected type.
    /// </summary>
    /// <returns>String representing query name.</returns>
    private string GetQueryNameByType()
    {
        switch (mSelectedType)
        {
            case "r":
                return "cms.permission.getResourcePermissionMatrix";

            default:
                return "cms.permission.getClassPermissionMatrix";
        }
    }


    /// <summary>
    /// Gets user effective permissions HTML content.
    /// </summary>
    /// <param name="siteId">Site ID</param>
    /// <param name="resourceId">ID of particular resource</param>
    /// <param name="selectedType">Permission type</param>
    /// <param name="userId">User ID</param>
    private string GetBeforeRowsContent(int siteId, int resourceId, string selectedType, int userId)
    {
        // Check if selected users exists
        UserInfo user = SelectedUser;
        if (user == null)
        {
            gridMatrix.ShowContentBeforeRows = false;
            return null;
        }

        string columns = "PermissionID";

        // Ensure tooltip column
        if (!String.IsNullOrEmpty(gridMatrix.ItemTooltipColumn))
        {
            columns += ",Matrix." + gridMatrix.ItemTooltipColumn;
        }

        // Get permission data
        DataSet dsPermissions = null;
        switch (selectedType)
        {
            case "r":
                dsPermissions = UserInfoProvider.GetUserResourcePermissions(user, siteId, resourceId, true, columns);
                break;

            default:
                dsPermissions = UserInfoProvider.GetUserDataClassPermissions(user, siteId, resourceId, true, columns);
                break;
        }

        // Initialize string builder
        StringBuilder sb = new StringBuilder("");

        if (!DataHelper.DataSourceIsEmpty(dsPermissions))
        {
            // Initialize variables used during rendering
            string firstColumnsWidth = (gridMatrix.FirstColumnsWidth > 0) ? "width:" + gridMatrix.FirstColumnsWidth + (gridMatrix.UsePercentage ? "%;" : "px;") : "";
            string imagesUrl = GetImageUrl("Design/Controls/UniMatrix/", IsLiveSite, true);
            DataRowCollection rows = dsPermissions.Tables[0].Rows;
            string userName = Functions.GetFormattedUserName(user.UserName, user.FullName);

            // Get user name column
            sb.Append("<td class=\"MatrixHeader\" style=\"");
            sb.Append(firstColumnsWidth);
            sb.Append("white-space:nowrap;\" title=\"");
            sb.Append(HTMLHelper.HTMLEncode(userName));
            sb.Append("\">");
            sb.Append(HTMLHelper.HTMLEncode(TextHelper.LimitLength(userName, 50)));
            sb.Append("</td>\n");


            // Process permissions according to matrix order
            foreach (int index in gridMatrix.ColumnOrderIndex)
            {
                DataRow dr = (DataRow)rows[index];

                // Render cell
                sb.Append("<td style=\"white-space:nowrap; text-align: center;\"><img src=\"");
                sb.Append(imagesUrl);

                // Render checkboxes
                if (SelectedUser.IsGlobalAdministrator || Convert.ToInt32(dr["Allowed"]) == 1)
                {
                    sb.Append("allowed.png");
                }
                else
                {
                    sb.Append("denied.png");
                }

                // Append tootlip and alternative text
                sb.Append("\" title=\"");
                string tooltip = HTMLHelper.HTMLEncode(CMSContext.ResolveMacros(ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, gridMatrix.ItemTooltipColumn), "")));
                sb.Append(tooltip);
                sb.Append("\" alt=\"");
                sb.Append(tooltip);
                sb.Append("\" onclick=\"NA()\" /></td>\n");
            }

            bool manyColumns = gridMatrix.ColumnOrderIndex.Length >= 10;

            // Add finish row
            if (!manyColumns)
            {
                sb.Append("<td colspan=\"" + (gridMatrix.ColumnsCount - rows.Count + 1) + "\"></td>");
            }
        }
        return sb.ToString();
    }

    #endregion


    #region "Event Handlers"

    protected void gridMatrix_OnItemChanged(object sender, int rowItemId, int colItemId, bool allow)
    {
        // roleId and permissionId possitions differ according to the page where control is used
        int roleId = (mRoleId > 0) ? colItemId : rowItemId;
        int permissionId = (mRoleId > 0) ? rowItemId : colItemId;

        // Check "Manage" permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Permissions", "Manage"))
        {
            CMSPage.RedirectToCMSDeskAccessDenied("CMS.Permissions", "Manage");
        }

        // Check permission for the given column
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Permissions", "Manage"))
        {
            CMSPage.RedirectToSiteManagerAccessDenied("CMS.Permissions", "Manage");
        }

        if (allow)
        {
            RolePermissionInfoProvider.SetRolePermissionInfo(roleId, permissionId);
        }
        else
        {
            RolePermissionInfoProvider.DeleteRolePermissionInfo(roleId, permissionId);
        }

        // Reload content before rows
        gridMatrix.ContentBeforeRows = GetBeforeRowsContent(SiteID, ValidationHelper.GetInteger(SelectedID, 0), SelectedType, SelectedUserID);
    }


    protected void gridMatrix_DataLoaded(DataSet ds)
    {
        if (OnDataLoaded != null)
        {
            OnDataLoaded(ds);
        }
    }


    protected string gridMatrix_OnGetRowItemCssClass(object sender, DataRow dr)
    {
        string roleName = ValidationHelper.GetString(dr["RoleName"], String.Empty);

        // Check if all necessary data are available
        if (!String.IsNullOrEmpty(roleName) && (SelectedUser != null))
        {
            string siteName = "";

            // Get site name if not global
            if (SelectedSite != null)
            {
                siteName = SelectedSite.SiteName;
            }

            // Check if user is in specified role
            if (SelectedUser.IsInRole(roleName, siteName, (SiteID <= 0), true))
            {
                return HIGHLIGHTED_ROW_CSS;
            }
        }
        return String.Empty;
    }

    #endregion
}