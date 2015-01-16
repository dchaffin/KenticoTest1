using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Modules_Pages_Development_Module_UI_Roles : SiteManagerPage
{
    #region "Constants"

    /// <summary>
    /// CSS class for highlighted UI permission matrix rows.
    /// </summary>
    private const string HIGHLIGHTED_ROW_CSS = "Highlighted";

    #endregion


    #region "Variables"

    private int mElementID = 0;
    private UserInfo mSelectedUser = null;
    private UIElementInfo mElement = null;
    private ResourceInfo mElementResource = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets selected user ID.
    /// </summary>
    private int SelectedUserID
    {
        get
        {
            return ValidationHelper.GetInteger(userSelector.Value, 0);
        }
    }


    /// <summary>
    /// Gets UserInfo object for selected user.
    /// </summary>
    private UserInfo SelectedUser
    {
        get
        {
            if ((mSelectedUser == null) && (SelectedUserID > 0))
            {
                mSelectedUser = UserInfoProvider.GetUserInfo(SelectedUserID);
            }

            return mSelectedUser;
        }
    }


    /// <summary>
    /// Gets selected UI element ID.
    /// </summary>
    private int ElementID
    {
        get
        {
            if (mElementID == 0)
            {
                mElementID = QueryHelper.GetInteger("elementid", 0);
            }
            return mElementID;
        }
    }


    /// <summary>
    /// Gets UIElementInfo object for selected UI element.
    /// </summary>
    private UIElementInfo Element
    {
        get
        {
            if (mElement == null)
            {
                mElement = UIElementInfoProvider.GetUIElementInfo(ElementID);
            }
            return mElement;
        }
    }


    /// <summary>
    /// Gets element resource for selected UI element.
    /// </summary>
    private ResourceInfo ElementResource
    {
        get
        {
            if (mElementResource == null)
            {
                if (Element != null)
                {
                    mElementResource = ResourceInfoProvider.GetResourceInfo(Element.ElementResourceID);
                }
            }
            return mElementResource;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize UI permission matrix
        gridMatrix.ContentBefore = "<table class=\"UIPermissionMatrix\" cellspacing=\"0\" cellpadding=\"3\" rules=\"rows\" border=\"1\" style=\"width:100%;border-collapse:collapse;\">";
        gridMatrix.ContentAfter = "</table>";
        gridMatrix.ColumnsCount = 1;
        gridMatrix.OnItemChanged += gridMatrix_OnItemChanged;
        gridMatrix.ContentBeforeRowsCssClass = "ContentBefore";
        gridMatrix.OnGetRowItemCssClass += gridMatrix_OnGetRowItemCssClass;
        gridMatrix.NoRecordsMessage = GetString("general.emptymatrix");

        //Initialize site selector
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.UniSelector.OnSelectionChanged += UniSelector_SelectedIndexChanged;
        siteSelector.IsLiveSite = false;
        siteSelector.AllowGlobal = true;

        // Initialize user selector
        userSelector.SiteID = (siteSelector.SiteID > 0) ? siteSelector.SiteID : 0;
        userSelector.ShowSiteFilter = false;
        userSelector.DropDownSingleSelect.AutoPostBack = true;

        // Display all users if global
        if (userSelector.SiteID <= 0)
        {
            userSelector.DisplayUsersFromAllSites = true;
        }
        else
        {
            userSelector.DisplayUsersFromAllSites = false;
        }

        chkUserOnly.Text = GetString("Administration-Permissions_Header.UserRoles");

        // Register js script to display checkbox not adjustable message
        string script = "function NA() {alert(" + ScriptHelper.GetString(GetString("Administration-Permissions_Matrix.NotAdjustable")) + ");}";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "NotAdjustableScript", ScriptHelper.GetScript(script));

        if ((Element != null) && Element.ElementLevel > 1)
        {
            // Prepare header action
            HeaderActions actions = CurrentMaster.HeaderActions;

            // Create 'Copy from parent' action
            actions.ActionsList.Add(new HeaderAction
            {
                ControlType = HeaderActionTypeEnum.Hyperlink,
                Text = GetString("uiprofile.permissionsfromparent"),
                OnClientClick = "return ConfirmCopyFromParent();",
                ImageUrl = GetImageUrl("Objects/CMS_UIElement/list.png"),
                CommandName = "copyFromParent",
                CommandArgument = string.Empty
            });

            actions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);

            // Register javascript to confirm copy 
            script = "function ConfirmCopyFromParent() {return confirm(" + ScriptHelper.GetString(GetString("uiprofile.ConfirmCopyFromParent")) + ");}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmCopyFromParent", ScriptHelper.GetScript(script));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Load the matrix
        if (Element != null)
        {
            // Disable check box if no user selected
            if (SelectedUserID > 0)
            {
                chkUserOnly.Enabled = true;
            }
            else
            {
                chkUserOnly.Checked = false;
                chkUserOnly.Enabled = false;
            }

            // Set info label
            lblInfo.Text = String.Format(GetString("resource.ui.rolesinfo"), HTMLHelper.HTMLEncode(Element.ElementDisplayName));

            // Set matrix parameters
            gridMatrix.QueryParameters = GetQueryParameters(siteSelector.SiteID, mElementID, Element.ElementDisplayName);
            gridMatrix.WhereCondition = GetWhereCondition();
            gridMatrix.ShowContentBeforeRows = (SelectedUser != null);
            gridMatrix.ContentBeforeRows = GetBeforeRowsContent();

            ucDisabledModule.SiteName = siteSelector.SiteName;
            ucDisabledModule.InfoText = GetString("resource.ui.disabled");

            if (!gridMatrix.HasData)
            {
                plcUpdate.Visible = false;
                lblInfo.Text = (chkUserOnly.Checked) ? GetString("general.norolemember") : GetString("uiprofile.norole");
            }
            else
            {
                // Inform user that global admin was selected
                if ((SelectedUserID > 0) && SelectedUser.IsGlobalAdministrator)
                {
                    ShowInformation(GetString("uiprofile.GlobalAdministrator"));
                }

                plcUpdate.Visible = true;
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Control events"

    protected void UniSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Element != null)
        {
            // Reset selected user
            userSelector.ReloadData();
            mSelectedUser = null;

            // Set matrix current page to first
            gridMatrix.Pager.CurrentPage = 1;
        }
    }


    protected void gridMatrix_OnItemChanged(object sender, int rowItemId, int colItemId, bool newState)
    {
        if (newState)
        {
            RoleUIElementInfoProvider.AddRoleUIElementInfo(rowItemId, colItemId);
        }
        else
        {
            RoleUIElementInfoProvider.DeleteRoleUIElementInfo(rowItemId, colItemId);
        }

        // Invalidate all users
        UserInfo.TYPEINFO.InvalidateAllObjects();

        // Forget old user
        mCurrentUser = null;

        // Clear hashtables with users
        UserInfoProvider.Clear(true);

        // Update content before rows
        gridMatrix.ContentBeforeRows = GetBeforeRowsContent();
    }


    protected string gridMatrix_OnGetRowItemCssClass(object sender, DataRow dr)
    {
        string roleName = ValidationHelper.GetString(dr["RoleName"], String.Empty);

        // Check if all necessary data are available
        if (!String.IsNullOrEmpty(roleName) && (SelectedUser != null))
        {
            if (SelectedUser.IsInRole(roleName, siteSelector.SiteName, (siteSelector.SiteID <= 0), true))
            {
                return HIGHLIGHTED_ROW_CSS;
            }
        }

        return String.Empty;
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "copyfromparent":
                if (Element != null)
                {
                    CopyFromParent(Element);

                    // Set matrix current page to first
                    gridMatrix.Pager.CurrentPage = 1;
                }
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns query parameters for permission matrix.
    /// </summary>
    /// <param name="siteId">Site ID</param>
    /// <param name="elementId">Element ID</param>
    /// <param name="elementName">Element display name</param>
    private QueryDataParameters GetQueryParameters(int siteId, int elementId, string elementName)
    {
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@SiteID", (ValidationHelper.GetString(siteSelector.Value, String.Empty) == siteSelector.GlobalRecordValue) ? 0 : siteId);
        parameters.Add("@ElementID", mElementID);
        parameters.Add("@ElementDisplayName", ResHelper.LocalizeString(elementName));

        return parameters;
    }


    /// <summary>
    /// Copies role binding from parent UI element.
    /// </summary>
    /// <param name="element">Element which are permissions copied to</param>
    private void CopyFromParent(UIElementInfo element)
    {
        using (CMSTransactionScope tr = new CMSTransactionScope())
        {
            if (element != null)
            {
                // Delete existing bindings
                DataSet elemRoles = RoleUIElementInfoProvider.GetRoleUIElements("ElementID = " + element.ElementID, null);
                if (!DataHelper.DataSourceIsEmpty(elemRoles))
                {
                    foreach (DataRow dr in elemRoles.Tables[0].Rows)
                    {
                        // Get role id
                        int roleId = ValidationHelper.GetInteger(dr["RoleID"], 0);
                        // Remove binding
                        RoleUIElementInfoProvider.DeleteRoleUIElementInfo(roleId, element.ElementID);
                    }
                }

                // Add same bindings as parent has
                int parentElemId = element.ElementParentID;

                DataSet parentRoles = RoleUIElementInfoProvider.GetRoleUIElements("ElementID = " + parentElemId, null);
                if (!DataHelper.DataSourceIsEmpty(parentRoles))
                {
                    foreach (DataRow dr in parentRoles.Tables[0].Rows)
                    {
                        // Get role id
                        int roleId = ValidationHelper.GetInteger(dr["RoleID"], 0);
                        // Create binding
                        RoleUIElementInfoProvider.AddRoleUIElementInfo(roleId, element.ElementID);
                    }
                }
            }

            // Commit transaction
            tr.Commit();
        }
        
        // Invalidate all users
        UserInfo.TYPEINFO.InvalidateAllObjects();

        // Clear hashtables with users
        UserInfoProvider.Clear(true);
    }


    /// <summary>
    /// Gets user effective UI permission HTML content.
    /// </summary>
    private string GetBeforeRowsContent()
    {
        // Initialize string builder
        StringBuilder sb = new StringBuilder("");

        // Check if every necessary property is set
        if ((SelectedUser != null) && (Element != null) && (ElementResource != null))
        {
            // Initialize variables used during rendering
            string firstColumnsWidth = (gridMatrix.FirstColumnsWidth > 0) ? "width:" + gridMatrix.FirstColumnsWidth + (gridMatrix.UsePercentage ? "%;" : "px;") : "";
            string imagesUrl = GetImageUrl("Design/Controls/UniMatrix/", false, true);
            string userName = HTMLHelper.HTMLEncode(TextHelper.LimitLength(Functions.GetFormattedUserName(SelectedUser.UserName, SelectedUser.FullName), 50));

            // Get user name column
            sb.Append("<td class=\"MatrixHeader\" style=\"");
            sb.Append(firstColumnsWidth);
            sb.Append("white-space:nowrap;\" title=\"");
            sb.Append(userName);
            sb.Append("\">");
            sb.Append(userName);
            sb.Append("</td>\n");

            // Render UI permission cell
            sb.Append("<td style=\"white-space:nowrap; text-align: center;\"><img src=\"");
            sb.Append(imagesUrl);

            // Render checkboxes
            if (SelectedUser.IsGlobalAdministrator || UserInfoProvider.IsAuthorizedPerUIElement(ElementResource.ResourceName, new string[] { Element.ElementName }, siteSelector.SiteName, SelectedUser, true, siteSelector.SiteID <= 0))
            {
                sb.Append("allowed.png");
            }
            else
            {
                sb.Append("denied.png");
            }

            // Append tootlip and alternative text
            sb.Append("\" onclick=\"NA()\" title=\"\" alt=\"\" /></td>\n");

            // Add finish row
            sb.Append("<td></td>");
        }
        return sb.ToString();
    }


    /// <summary>
    /// Gets where condition for the matrix.
    /// </summary>
    /// <returns>String representing where condition for the matrix</returns>
    private string GetWhereCondition()
    {
        string where = "RoleGroupId IS NULL";

        if (chkUserOnly.Checked && (SelectedUserID > 0))
        {
            // Get selected site name
            string siteName = UserInfo.GLOBAL_ROLES_KEY;
            if (siteSelector.SiteID > 0)
            {
                siteName = siteSelector.SiteName.ToLowerCSafe();
            }

            string rolesWhere = SelectedUser.GetRoleIdList((siteSelector.SiteID <= 0), true, siteName);

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

    #endregion
}