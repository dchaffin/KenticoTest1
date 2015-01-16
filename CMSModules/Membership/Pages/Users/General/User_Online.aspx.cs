using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Membership_Pages_Users_General_User_Online : CMSUsersPage
{
    #region "Variables"

    protected bool isInitiateChatEnabled;
    private string mSiteName;
    private const string CONTACT_DETAIL_DIALOG = "~/CMSModules/ContactManagement/Pages/Tools/Account/Contact_Detail.aspx";
    private bool? mOnlineMarketingEnabled;
    private IUserFilter filter = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets current site name.
    /// </summary>
    private new string SiteName
    {
        get
        {
            if (String.IsNullOrEmpty(mSiteName))
            {
                if (SiteID > 0)
                {
                    SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteID);
                    if (si != null)
                    {
                        mSiteName = si.SiteName;
                    }
                }
            }
            return mSiteName;
        }
    }


    /// <summary>
    /// Indicates if online marketing is enabled.
    /// </summary>
    private bool OnlineMarketingEnabled
    {
        get
        {
            if (mOnlineMarketingEnabled == null)
            {
                mOnlineMarketingEnabled = LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.ContactManagement) &&
                    ResourceSiteInfoProvider.IsResourceOnSite("CMS.ContactManagement", CMSContext.CurrentSiteName) &&
                    SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSEnableOnlineMarketing");

            }
            return (bool)mOnlineMarketingEnabled;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        FilterMode = "online";

        if (DisplayContacts)
        {
            gridElem.GridName = "User_OnlineDB.xml";
            gridElem.OrderBy = "SessionFullName";
            gridElem.Columns = "SessionUserID, SessionFullName, SessionEmail, SessionUserName, SessionNickName, SessionUserCreated, SessionLocation, SessionContactID, SessionSiteID, SessionLastActive";

            EnableDisplayingGuests = true;
        }

        // Hide unigrid default filter button
        gridElem.HideFilterButton = true;
        gridElem.LoadGridDefinition();

        // Get custom filter control
        if (SessionManager.StoreOnlineUsersInDatabase)
        {
            filter = (IUserFilter)gridElem.FilterFields["SessionSiteID"].ValueControl;
        }
        else
        {
            filter = (IUserFilter)gridElem.FilterFields["UserID"].ValueControl;

            // Where condition needs to be generated not only when user click "advanced filter show" button
            gridElem.ApplyFilter(this, new EventArgs());
        }

        if (DisplayContacts)
        {
            filter.SessionInsteadOfUser = true;
        }
        filter.DisplayUserEnabled = false;
        filter.EnableDisplayingHiddenUsers = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions
        LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.OnlineUsers);

        isInitiateChatEnabled = CheckInitiateChat();

        // If session management is disabled inform about it
        if (!SessionManager.OnlineUsersEnabled)
        {
            // Set disabled module info
            ucDisabledModule.SettingsKeys = "CMSUseSessionManagement";
            ucDisabledModule.InfoText = GetString("administration.users.online.disabled");
            
            ucDisabledModule.Visible = true;       
            gridElem.Visible = false;
            lblGeneralInfo.Visible = false;
        }
        else
        {
            SetHeaderActions();
            PrepareUnigrid();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        SetCountOfUsers();
    }


    protected override void CheckUIPermissions()
    {
        // Check UI Permissions for online marketing if needed
        if (QueryHelper.GetBoolean("isonlinemarketing", false))
        {
            var user = CMSContext.CurrentUser;
            if (!user.IsAuthorizedPerUIElement("CMS.Desk", "OnlineMarketing"))
            {
                RedirectToCMSDeskUIElementAccessDenied("CMS.Desk", "OnlineMarketing");
            }
            if (!user.IsAuthorizedPerUIElement("CMS.OnlineMarketing", new string[] { "OMCMGroup", "ContactsFrameset", "On-line_users" }, CMSContext.CurrentSiteName))
            {
                RedirectToCMSDeskUIElementAccessDenied("CMS.OnlineMarketing", "OMCMGroup;ContactsFrameset;On-line_users");
            }
        }
        else
        {
            base.CheckUIPermissions();
        }
    }


    /// <summary>
    /// Checks the permission for administration if not in on-line marketing.
    /// </summary>
    public override void CheckAdministration()
    {
        if (!QueryHelper.GetBoolean("isonlinemarketing", false))
        {
            base.CheckAdministration();
        }
    }

    #endregion


    #region "Unigrid"

    /// <summary>
    /// Sets where condition before data binding.
    /// </summary>
    protected void gridElem_OnBeforeDataReload()
    {

        if (gridElem.QueryParameters == null)
        {
            gridElem.QueryParameters = new QueryDataParameters();
        }
        gridElem.QueryParameters.Add("@Now", DateTime.Now);

        bool guestByDefault = QueryHelper.GetBoolean("guest", false);

        if (DisplayContacts)
        {
            gridElem.GridView.Columns[3].Visible = !guestByDefault;
            gridElem.GridView.Columns[4].Visible = !guestByDefault;
            gridElem.GridView.Columns[8].Visible = filter.DisplayGuests;
            gridElem.GridView.Columns[9].Visible = DisplayScore && (filter.SelectedScore > 0);
            gridElem.GridView.Columns[10].Visible = String.IsNullOrEmpty(SiteName) && (filter.SelectedSite <= 0);
            RegisterScripts();
        }
    }


    /// <summary>
    ///  On action event.
    /// </summary>
    private void gridElem_OnAction(string actionName, object actionArgument)
    {
        // Check "modify" permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        int userId = ValidationHelper.GetInteger(actionArgument, 0);
        switch (actionName.ToLowerCSafe())
        {
            // Kick action
            case "kick":
                SessionManager.KickUser(userId);
                ShowConfirmation(GetString("kicked.user"));
                break;
            // Undo kick action
            case "undokick":
                SessionManager.RemoveUserFromKicked(userId);
                ShowConfirmation(GetString("kicked.cancel"));
                break;
        }
    }


    /// <summary>
    ///  On external databound event.
    /// </summary>
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "formattedusername":
                return GetFormattedUsername(parameter);

            case "fullname":
                return GetFullName(parameter);

            case "kick":
                return ModifyKickedButton(sender, parameter);

            case "undokick":
                return ModifyUndoKickButton(sender, parameter);

            case "initiatechat":
                return ModifyChatButton(sender, parameter);

            case "isguest":
                return GetIsGuest(parameter);

            case "editcontact":
                return ModifyContactButton(sender, parameter);

            default:
                return "";
        }
    }


    DataSet gridElem_OnAfterRetrieveData(DataSet ds)
    {
        if (!DataHelper.DataSourceIsEmpty(ds) && (filter != null) && (filter.SelectedScore > 0))
        {
            return GetScore(ds);
        }
        return ds;
    }

    #endregion


    #region "Unigrid data bound methods"

    /// <summary>
    /// Get formatted user name.
    /// </summary>
    private string GetFormattedUsername(object parameter)
    {
        var drv = (DataRowView)parameter;
        if (drv != null)
        {
            var ui = new UserInfo(drv.Row);
            string userName = Functions.GetFormattedUserName(ui.UserName);
            if (AuthenticationHelper.UserKicked(ui.UserID))
            {
                return HTMLHelper.HTMLEncode(userName) + " <span style=\"color:#ee0000;\">" + GetString("administration.users.onlineusers.kicked") + "</span>";
            }

            return HTMLHelper.HTMLEncode(userName);
        }
        return "";
    }


    /// <summary>
    /// Returns full name of the visitor.
    /// </summary>
    private object GetFullName(object parameter)
    {
        if (parameter == DBNull.Value)
        {
            return GetString("usersonline.anonymous");
        }
        return parameter;
    }


    /// <summary>
    /// Displays button for kicking users.
    /// </summary>
    private object ModifyKickedButton(object sender, object parameter)
    {
        bool userIsAdmin = false;
        int userID;
        if (SessionManager.StoreOnlineUsersInDatabase)
        {
            userID = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["SessionUserID"], 0);
            if (userID > 0)
            {
                UserInfo user = UserInfoProvider.GetUserInfo(userID);
                if (user != null)
                {
                    userIsAdmin = user.UserIsGlobalAdministrator;
                }
            }
        }
        else
        {
            userID = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserID"], 0);
            userIsAdmin = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserIsGlobalAdministrator"], false);
        }

        if (AuthenticationHelper.UserKicked(userID) || userIsAdmin || (userID == 0))
        {
            var button = ((ImageButton)sender);
            button.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Kickdisabled.png");
            button.Enabled = false;
        }
        else
        {
            var button = ((ImageButton)sender);
            button.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Kick.png");
            button.Enabled = true;
        }
        return "";
    }


    /// <summary>
    /// Displays button for undo kicked users.
    /// </summary>
    private object ModifyUndoKickButton(object sender, object parameter)
    {
        int userID = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["UserID"], 0);
        if (AuthenticationHelper.UserKicked(userID))
        {
            var button = ((ImageButton)sender);
            button.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Undo.png");
            button.Enabled = true;
        }
        else
        {
            var button = ((ImageButton)sender);
            button.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Undodisabled.png");
            button.Enabled = false;
        }
        return "";
    }


    /// <summary>
    /// Display chat button.
    /// </summary>
    private object ModifyChatButton(object sender, object parameter)
    {
        ImageButton button = ((ImageButton)sender);
        button.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Message.png");

        if (!isInitiateChatEnabled)
        {
            button.Visible = false;
        }
        else
        {
            DataRow row = ((DataRowView)((GridViewRow)parameter).DataItem).Row;

            int contactID;
            int userID;

            if (DisplayContacts)
            {
                contactID = ValidationHelper.GetInteger(row["SessionContactID"], 0);
                userID = ValidationHelper.GetInteger(row["SessionUserID"], 0);
            }
            else
            {
                contactID = 0;
                userID = ValidationHelper.GetInteger(row["UserID"], 0);
            }

            // If userID and contactID are not known (can happen if online marketing is turned off), hide initiate chat button
            if ((userID <= 0) && (contactID <= 0))
            {
                button.Visible = false;
            }
            else
            {
                button.OnClientClick = string.Format("if (window.top.ChatSupportManager) {{window.top.ChatSupportManager.InitiateChat({0}, {1});}} return false;", userID, contactID);
            }
        }
        return "";
    }


    /// <summary>
    /// Modify contact button.
    /// </summary>
    private object ModifyContactButton(object sender, object parameter)
    {
        ImageButton button = ((ImageButton)sender);
        int contactID = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["SessionContactID"], 0);
        if ((contactID > 0) && OnlineMarketingEnabled)
        {
            button.OnClientClick = String.Format("EditContact({0}); return false;", contactID);
        }
        else
        {
            button.Visible = false;
        }
        return "";
    }




    /// <summary>
    /// Returns if online user is guest.
    /// </summary>
    private object GetIsGuest(object parameter)
    {
        int userID = ValidationHelper.GetInteger(parameter, 0);
        if (userID == 0)
        {
            return GetString("general.yes");
        }
        return "";
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Set header actions.
    /// </summary>
    private void SetHeaderActions()
    {
        string[,] actions = new string[1, 6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("General.Refresh");
        actions[0, 2] = null;
        actions[0, 3] = URLHelper.CurrentURL;
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Refresh.png");
        CurrentMaster.HeaderActions.Actions = actions;
    }


    /// <summary>
    /// Sets number of online users.
    /// </summary>
    private void SetCountOfUsers()
    {
        int publicUsers;
        int authenticatedUsers;
        SessionManager.GetUsersNumber(SiteName, null, true, false, out publicUsers, out authenticatedUsers);
        lblGeneralInfo.Text =
            String.Format(GetString("OnlineUsers.GeneralInfo"), publicUsers + authenticatedUsers, publicUsers,
                          authenticatedUsers) + "<br /><br />";
    }


    /// <summary>
    /// Prepares unigrid.
    /// </summary>
    private void PrepareUnigrid()
    {
        // Online users in DB
        if (SessionManager.StoreOnlineUsersInDatabase)
        {
            gridElem.ObjectType = "cms.onlineuser";
        }
        // On-line users in hashtable
        else
        {
            string usersWhere = ValidationHelper.GetString(SessionManager.GetUsersWhereCondition(null, SiteName, true, true), String.Empty);

            if (!String.IsNullOrEmpty(usersWhere))
            {
                gridElem.ObjectType = "cms.userlist";
                gridElem.WhereCondition = usersWhere;
            }
            else
            {
                // Clear the object type
                gridElem.ObjectType = string.Empty;
            }
        }

        // Setup unigrid events
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.ZeroRowsText = GetString("general.nodatafound");
        gridElem.OnAfterRetrieveData += gridElem_OnAfterRetrieveData;

    }


    /// <summary>
    /// Registers javascript.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);
        string script = @"
function EditContact(contactID)
{
    modalDialog('" + ResolveUrl(CONTACT_DETAIL_DIALOG) + @"?contactid=' + contactID, 'ContactDetail', '1024', '700');
}
";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Actions", ScriptHelper.GetScript(script));
    }



    /// <summary>
    /// Gets score for specified contacts.
    /// </summary>
    private DataSet GetScore(DataSet ds)
    {
        ds.Tables[0].Columns.Add("Score");

        StringBuilder contacts = new StringBuilder();
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            int contactID = ValidationHelper.GetInteger(dr["SessionContactID"], 0);
            if (contactID > 0)
            {
                contacts.Append(contactID + ",");
            }
        }
        string contactList = contacts.ToString();
        if (!String.IsNullOrEmpty(contactList))
        {
            contactList = contactList.Substring(0, contactList.Length - 1);
            GeneralizedInfo contactScore = CMSObjectHelper.GetReadOnlyObject("om.contactscorelist");
            DataSet result = contactScore.GetData(null, String.Format("ContactID IN ({0}) AND ScoreID = {1}", contactList, filter.SelectedScore), null, -1, "ContactID,ScoreValue", false);

            if (!DataHelper.DataSourceIsEmpty(result))
            {
                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    DataRow[] row = ds.Tables[0].Select("SessionContactID =" + dr["ContactID"]);
                    row[0]["Score"] = dr["ScoreValue"];
                }
            }
        }
        return ds;
    }

    #endregion
}