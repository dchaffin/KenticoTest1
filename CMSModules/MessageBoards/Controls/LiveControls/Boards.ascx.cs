using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.MessageBoard;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_MessageBoards_Controls_LiveControls_Boards : CMSAdminItemsControl
{
    #region "Selected tab enumeration"

    private enum SelectedControlEnum
    {
        Listing,
        Messages,
        General,
        Subscriptions,
        Moderators,
        Security
    };

    #endregion


    #region "Private fields"

    private BoardInfo board = null;

    private int mGroupID = 0;
    private bool mHideWhenGroupIsNotSupplied = false;
    private const string breadCrumbsSeparator = " <span class=\"TitleBreadCrumbSeparator\">&nbsp;</span> ";

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns currently selected tab.
    /// </summary>
    private SelectedControlEnum SelectedControl
    {
        get
        {
            int selectedTab = ValidationHelper.GetInteger(ViewState["SelectedControl"], 0);
            switch (selectedTab)
            {
                case 1:
                    return SelectedControlEnum.General;
                case 2:
                    return SelectedControlEnum.Moderators;
                case 3:
                    return SelectedControlEnum.Security;
                case 4:
                    return SelectedControlEnum.Subscriptions;
                default:
                    return SelectedControlEnum.Messages;
            }
        }
        set
        {
            // Convert enum to int
            switch (value)
            {
                case SelectedControlEnum.General:
                    tabElem.SelectedTab = 1;
                    break;

                case SelectedControlEnum.Moderators:
                    tabElem.SelectedTab = 2;
                    break;

                case SelectedControlEnum.Security:
                    tabElem.SelectedTab = 3;
                    break;

                case SelectedControlEnum.Subscriptions:
                    tabElem.SelectedTab = 4;
                    break;

                case SelectedControlEnum.Messages:
                    tabElem.SelectedTab = 0;
                    break;
            }
        }
    }


    /// <summary>
    /// Current board ID for internal use.
    /// </summary>
    private int BoardID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["BoardID"], 0);
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Current group ID.
    /// </summary>
    public int GroupID
    {
        get
        {
            if (mGroupID == 0)
            {
                mGroupID = ValidationHelper.GetInteger(GetValue("GroupID"), 0);
            }

            if (mGroupID == 0)
            {
                mGroupID = QueryHelper.GetInteger("groupid", 0);
            }
            return mGroupID;
        }
        set
        {
            mGroupID = value;
        }
    }


    /// <summary>
    /// Determines whether to hide the content of the control when GroupID is not supplied.
    /// </summary>
    public bool HideWhenGroupIsNotSupplied
    {
        get
        {
            return mHideWhenGroupIsNotSupplied;
        }
        set
        {
            mHideWhenGroupIsNotSupplied = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if the group was supplied and hide control if necessary
        if ((GroupID == 0) && (HideWhenGroupIsNotSupplied))
        {
            Visible = false;
        }

        if (StopProcessing || !Visible)
        {
            EnableViewState = false;
            boardMessages.StopProcessing = true;
            boardList.StopProcessing = true;
            boardEdit.StopProcessing = true;
            boardModerators.StopProcessing = true;
            boardSecurity.StopProcessing = true;
            boardSubscriptions.StopProcessing = true;
            return;
        }

        // Initializes the controls
        SetupControls();

        // Display current control
        if (ViewState["SelectedControl"] != null)
        {
            DisplayControl(SelectedControl, false);
        }

        // Reload data if necessary
        if (!URLHelper.IsPostback() && !IsLiveSite)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        tabElem.TabControlIdPrefix = "boards";
        tabElem.OnTabClicked += new EventHandler(tabElem_OnTabChanged);

        lnkEditBack.Text = GetString("Group_General.Boards.Boards.BackToList");
        lnkEditBack.Click += new EventHandler(lnkEditBack_Click);

        // Register for the security events
        boardList.OnCheckPermissions += new CheckPermissionsEventHandler(boardList_OnCheckPermissions);
        boardEdit.OnCheckPermissions += new CheckPermissionsEventHandler(boardEdit_OnCheckPermissions);
        boardModerators.OnCheckPermissions += new CheckPermissionsEventHandler(boardModerators_OnCheckPermissions);
        boardSecurity.OnCheckPermissions += new CheckPermissionsEventHandler(boardSecurity_OnCheckPermissions);

        // Setup controls        
        boardList.IsLiveSite = IsLiveSite;
        boardList.GroupID = GroupID;
        boardList.OnAction += new CommandEventHandler(boardList_OnAction);

        boardMessages.IsLiveSite = IsLiveSite;
        boardMessages.OnCheckPermissions += new CheckPermissionsEventHandler(boardMessages_OnCheckPermissions);
        boardMessages.BoardID = BoardID;
        boardMessages.GroupID = GroupID;
        boardMessages.EditPageUrl = (GroupID > 0) ? "~/CMSModules/Groups/CMSPages/Message_Edit.aspx" : "~/CMSModules/MessageBoards/CMSPages/Message_Edit.aspx";

        boardEdit.IsLiveSite = IsLiveSite;
        boardEdit.BoardID = BoardID;
        boardEdit.DisplayMode = DisplayMode;

        boardModerators.IsLiveSite = IsLiveSite;
        boardModerators.BoardID = BoardID;

        boardSecurity.IsLiveSite = IsLiveSite;
        boardSecurity.BoardID = BoardID;
        boardSecurity.GroupID = GroupID;

        boardSubscriptions.IsLiveSite = IsLiveSite;
        boardSubscriptions.BoardID = BoardID;
        boardSubscriptions.GroupID = GroupID;

        // Initialize tab control
        string[,] tabs = new string[5,4];
        tabs[0, 0] = GetString("Group_General.Boards.Boards.Messages");
        tabs[1, 0] = GetString("Group_General.Boards.Boards.Edit");
        tabs[2, 0] = GetString("Group_General.Boards.Boards.Moderators");
        tabs[3, 0] = GetString("Group_General.Boards.Boards.Security");
        tabs[4, 0] = GetString("Group_General.Boards.Boards.SubsList");
        tabElem.Tabs = tabs;

        // Initialize breadcrubms
        if (BoardID > 0)
        {
            board = BoardInfoProvider.GetBoardInfo(BoardID);
            if (board != null)
            {
                lblEditBack.Text = breadCrumbsSeparator + HTMLHelper.HTMLEncode(board.BoardDisplayName);
            }
        }
    }


    /// <summary>
    /// Displays specified control.
    /// </summary>
    /// <param name="selectedControl">Control to be displayed</param>
    /// <param name="reload">If True, ReloadData on child control is called</param>
    private void DisplayControl(SelectedControlEnum selectedControl, bool reload)
    {
        // First hide and stop all elements
        plcList.Visible = false;
        boardList.StopProcessing = true;

        plcTabs.Visible = true;
        plcTabsHeader.Visible = true;

        tabEdit.Visible = false;
        boardEdit.StopProcessing = true;

        tabMessages.Visible = false;
        boardMessages.StopProcessing = true;

        tabModerators.Visible = false;
        boardModerators.StopProcessing = true;

        tabSecurity.Visible = false;
        boardSecurity.StopProcessing = true;

        tabSubscriptions.Visible = false;
        boardSubscriptions.StopProcessing = true;

        // Set correct tab
        SelectedControl = selectedControl;
        pnlContent.CssClass = "TabBody";

        // Enable currently selected element
        switch (selectedControl)
        {
            case SelectedControlEnum.Listing:
                pnlContent.CssClass = "";
                plcTabs.Visible = false;
                plcTabsHeader.Visible = false;
                plcList.Visible = true;
                boardList.StopProcessing = false;
                if (reload)
                {
                    // Relaod data
                    boardList.ReloadData();
                }
                break;

            case SelectedControlEnum.General:
                tabEdit.Visible = true;
                boardEdit.StopProcessing = false;
                if (reload)
                {
                    // Relaod data
                    boardEdit.ReloadData();
                }
                break;

            case SelectedControlEnum.Messages:
                tabMessages.Visible = true;
                boardMessages.StopProcessing = false;
                if (reload)
                {
                    boardMessages.IsLiveSite = IsLiveSite;
                    boardMessages.BoardID = ValidationHelper.GetInteger(ViewState["BoardID"], 0);
                    boardMessages.ReloadData();
                }

                // Breadcrumbs
                if (board == null)
                {
                    board = BoardInfoProvider.GetBoardInfo(BoardID);
                    if (board != null)
                    {
                        lblEditBack.Text = breadCrumbsSeparator + HTMLHelper.HTMLEncode(board.BoardDisplayName);
                    }
                }

                break;

            case SelectedControlEnum.Moderators:
                tabModerators.Visible = true;
                boardModerators.StopProcessing = false;
                if (reload)
                {
                    // Relaod data
                    boardModerators.ReloadData(true);
                }
                break;

            case SelectedControlEnum.Security:
                tabSecurity.Visible = true;
                boardSecurity.StopProcessing = false;
                if (reload)
                {
                    // Relaod data
                    boardSecurity.ReloadData();
                }
                break;

            case SelectedControlEnum.Subscriptions:
                tabSubscriptions.Visible = true;
                boardSubscriptions.StopProcessing = false;
                if (reload)
                {
                    boardSubscriptions.BoardID = BoardID;
                    // Relaod data
                    boardSubscriptions.ReloadData();
                }

                break;
        }
    }


    /// <summary>
    /// Reloads the contol data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControls();

        DisplayControl(SelectedControlEnum.Listing, true);
    }


    /// <summary>
    /// Displays the default contol.
    /// </summary>
    public void SetDefault()
    {
        DisplayControl(SelectedControlEnum.Listing, true);
    }


    /// <summary>
    /// Clears the boards filter up.
    /// </summary>
    public void ClearFilter()
    {
        boardList.ClearFilter();
    }


    #region "Event handlers"

    protected void tabElem_OnTabChanged(object sender, EventArgs e)
    {
        ViewState.Add("SelectedControl", tabElem.SelectedTab);
        DisplayControl(SelectedControl, true);
    }


    protected void boardList_OnAction(object sender, CommandEventArgs e)
    {
        if ((e.CommandName.ToLowerCSafe() == "edit") && plcList.Visible)
        {
            ViewState["BoardID"] = e.CommandArgument;
            DisplayControl(SelectedControlEnum.Messages, true);
        }
    }


    protected void lnkEditBack_Click(object sender, EventArgs e)
    {
        ViewState.Add("SelectedControl", null);
        DisplayControl(SelectedControlEnum.Listing, true);
    }

    #endregion


    #region "Security event handlers"

    protected void boardList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Raise event
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected void boardMessages_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Raise event
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected void boardSecurity_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Raise event
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected void boardModerators_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Raise event
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected void boardEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Raise event
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion
}