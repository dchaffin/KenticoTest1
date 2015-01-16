using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.GlobalHelper;
using CMS.Messaging;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.ExtendedControls;

public partial class CMSModules_Messaging_Controls_SearchUser : CMSUserControl
{
    #region "Public properties"

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
    /// Zero rows text.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return gridUsers.ZeroRowsText;
        }
        set
        {
            gridUsers.ZeroRowsText = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set default zero rows text
        if (string.IsNullOrEmpty(ZeroRowsText))
        {
            ZeroRowsText = GetString("messaging.search.nousersfound");
        }

        // Initialize UniGrid
        gridUsers.IsLiveSite = IsLiveSite;
        gridUsers.OnDataReload += gridUsers_OnDataReload;
        gridUsers.OnExternalDataBound += gridUsers_OnExternalDataBound;
        gridUsers.OnAction += gridUsers_OnAction;
    }

    #endregion


    #region "Grid methods"

    protected object gridUsers_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "formattedusername":
                DataRowView drv = parameter as DataRowView;
                int userId = ValidationHelper.GetInteger(drv["UserID"], 0);
                return GetItemText(userId, drv["UserName"], drv["FullName"], drv["UserNickName"]);
        }
        return parameter;
    }


    protected void gridUsers_OnAction(string actionName, object actionArgument)
    {
        PerformAction(actionName, actionArgument);
    }


    protected DataSet gridUsers_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        object[,] searchParams = new object[2, 3];
        searchParams[0, 0] = "@search";
        searchParams[0, 1] = "%" + txtSearch.Text + "%";
        searchParams[1, 0] = "@siteID";
        searchParams[1, 1] = CMSContext.CurrentSite.SiteID;

        string where = "UserName NOT LIKE N'public'";

        // If user is not global administrator and control is in LiveSite mode
        if (IsLiveSite && !CMSContext.CurrentUser.IsGlobalAdministrator)
        {
            // Do not select hidden users
            where = SqlHelperClass.AddWhereCondition(where, "((UserIsHidden IS NULL) OR (UserIsHidden=0))");

            // Select only approved users
            where = SqlHelperClass.AddWhereCondition(where, "((UserWaitingForApproval IS NULL) OR (UserWaitingForApproval = 0))");

            // Select only enabled users
            where = SqlHelperClass.AddWhereCondition(where, UserInfoProvider.USER_ENABLED_WHERE_CONDITION);
        }

        // Load all users for current site
        if (CMSContext.CurrentSite != null)
        {
            // Public user has no actions
            if (CMSContext.CurrentUser.IsPublic())
            {
                gridUsers.GridView.Columns[0].Visible = false;
            }
        }

        return ConnectionHelper.ExecuteQuery("cms.user.finduserinsite", QueryDataParameters.FromArray(searchParams), where, "UserName ASC", currentTopN, "View_CMS_User.UserID,UserName,UserNickName,FullName", currentOffset, currentPageSize, ref totalRecords);
    }


    /// <summary>
    /// Returns correct format of username info.
    /// </summary>
    /// <param name="userId">Selected user id</param>
    /// <param name="username">User name</param>
    /// <param name="usernickname">User nickname</param>
    /// <param name="fullname">User full name</param>
    protected string GetItemText(int userId, object username, object fullname, object usernickname)
    {
        string usrName = ValidationHelper.GetString(username, string.Empty);
        string nick = HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(usrName, fullname.ToString(), usernickname.ToString(), IsLiveSite));
        return "<a href=\"javascript: window.parent.CloseAndRefresh(" + userId + ", " + ScriptHelper.GetString(usrName) + ", " + ScriptHelper.GetString(QueryHelper.GetText("mid", String.Empty)) + ", " + ScriptHelper.GetString(QueryHelper.GetText("hidid", String.Empty)) + ")\">" + nick + "</a>";
    }

    #endregion


    #region "Button handling"

    /// <summary>
    /// Searches for specified phrase.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        // Initiates reload
    }

    #endregion


    #region "Other methods"

    protected void PerformAction(string actionName, object actionArgument)
    {
        int currentid = ValidationHelper.GetInteger(actionArgument, 0);
        switch (actionName)
        {
            case "contact":
                // Add user to contact list
                ContactListInfoProvider.AddToContactList(CMSContext.CurrentUser.UserID, currentid);
                ShowConfirmation(GetString("messaging.search.addedsuccessfulytocontactlist"));
                break;

            case "ignore":
                // Add user to ignore list
                IgnoreListInfoProvider.AddToIgnoreList(CMSContext.CurrentUser.UserID, currentid);
                ShowConfirmation(GetString("messaging.search.addedsuccessfulytoignorelist"));
                break;
        }
    }

    #endregion
}