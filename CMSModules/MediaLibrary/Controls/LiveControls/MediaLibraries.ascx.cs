using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.MediaLibrary;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_MediaLibrary_Controls_LiveControls_MediaLibraries : CMSAdminItemsControl, IPostBackEventHandler
{
    #region "Private variables"

    protected int mGroupID = 0;
    protected Guid mGroupGUID = Guid.Empty;
    protected bool mHideWhenGroupIsNotSupplied = false;
    protected bool isNewLibrary = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// ID of the group library belongs to.
    /// </summary>
    public int GroupID
    {
        get
        {
            if (mGroupID <= 0)
            {
                mGroupID = ValidationHelper.GetInteger(GetValue("GroupID"), 0);
            }
            return mGroupID;
        }
        set
        {
            mGroupID = value;
        }
    }


    /// <summary>
    /// GUID of the group library belongs to.
    /// </summary>
    public Guid GroupGUID
    {
        get
        {
            if (mGroupGUID == Guid.Empty)
            {
                mGroupGUID = ValidationHelper.GetGuid(GetValue("GroupGUID"), Guid.Empty);
            }
            return mGroupGUID;
        }
        set
        {
            mGroupGUID = value;
        }
    }


    /// <summary>
    /// ID of the media library.
    /// </summary>
    protected int LibraryID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["LibraryID"], 0);
        }
        set
        {
            ViewState.Add("LibraryID", value);
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
        #region "Security"

        RaiseOnCheckPermissions(PERMISSION_READ, this);

        #endregion


        if (!Visible)
        {
            EnableViewState = false;
        }

        if (StopProcessing)
        {
            libraryList.StopProcessing = true;
            libraryFiles.StopProcessing = true;
            libraryEdit.StopProcessing = true;
            librarySecurity.StopProcessing = true;
        }
        else
        {
            // Check if the group was supplied and hide control if necessary
            if ((GroupID == 0) && (HideWhenGroupIsNotSupplied))
            {
                Visible = false;
            }

            // Initialize controls
            SetupControls();
        }
    }


    #region "Private methods"

    /// <summary>
    /// Initializes all the controls used on live control.
    /// </summary>
    private void SetupControls()
    {
        // Set display mode
        libraryList.DisplayMode = DisplayMode;
        libraryFiles.DisplayMode = DisplayMode;
        libraryEdit.DisplayMode = DisplayMode;
        librarySecurity.DisplayMode = DisplayMode;

        // Initialize tabs & header actions
        InitializeTabs();
        InitializeHeaderActions();

        tabElem.TabControlIdPrefix = "libraries";
        tabElem.OnTabClicked += new EventHandler(tabElem_OnTabChanged);

        lnkEditBack.Text = GetString("Group_General.MediaLibrary.BackToList");
        lnkEditBack.Click += new EventHandler(lnkEditBack_Click);

        libraryList.GroupID = GroupID;
        libraryList.OnAction += new CommandEventHandler(libraryList_OnAction);

        // Initialize library edit tab controls
        libraryEdit.MediaLibraryID = LibraryID;
        libraryEdit.MediaLibraryGroupID = GroupID;
        libraryEdit.MediaLibraryGroupGUID = GroupGUID;
        libraryEdit.OnSaved += new EventHandler(libraryEdit_OnSaved);

        // Initialize library security tab controls
        librarySecurity.MediaLibraryID = LibraryID;

        // Initialize library files list tab controls
        libraryFiles.LibraryID = LibraryID;
    }


    /// <summary>
    /// Initializes used tab menu control.
    /// </summary>
    private void InitializeTabs()
    {
        string[,] tabs = new string[3,4];
        tabs[0, 0] = GetString("general.files");
        tabs[1, 0] = GetString("General.general");
        tabs[2, 0] = GetString("general.security");

        tabElem.Tabs = tabs;
    }


    /// <summary>
    /// Initializes the header action element.
    /// </summary>
    private void InitializeHeaderActions()
    {
        // New subscription link
        string[,] actions = new string[1,7];
        actions[0, 0] = HeaderActions.TYPE_LINKBUTTON;
        actions[0, 1] = GetString("Group_General.MediaLibrary.NewLibrary");
        actions[0, 5] = GetImageUrl("CMSModules/CMS_MediaLibrary/add.png");
        actions[0, 6] = "newmedialibrary";

        newLibrary.Actions = actions;
        newLibrary.ActionPerformed += new CommandEventHandler(newLibrary_ActionPerformed);
    }


    /// <summary>
    /// Initializes the breadcrumbs controls.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        if (LibraryID > 0)
        {
            MediaLibraryInfo library = MediaLibraryInfoProvider.GetMediaLibraryInfo(LibraryID);
            if (library != null)
            {
                lblEditBack.Text = " <span class=\"TitleBreadCrumbSeparator\">&nbsp;</span> " + HTMLHelper.HTMLEncode(library.LibraryDisplayName);
            }
        }
        else
        {
            lblEditBack.Text = " <span class=\"TitleBreadCrumbSeparator\">&nbsp;</span> " + GetString("Group_General.MediaLibrary.NewLibrary");
        }
    }


    /// <summary>
    /// Handles displaying of library edit form.
    /// </summary>
    private void DisplayLibraryNew()
    {
        isNewLibrary = true;

        plcTabsHeader.Visible = true;
        pnlTabsMain.Visible = false;
        plcHeaderActions.Visible = false;

        plcList.Visible = false;

        plcTabs.Visible = true;
        tabEdit.Visible = true;
        tabFiles.Visible = false;
        tabSecurity.Visible = false;

        libraryEdit.MediaLibraryID = LibraryID;
        libraryEdit.MediaLibraryGroupID = GroupID;
        libraryEdit.MediaLibraryGroupGUID = GroupGUID;
        libraryEdit.ReloadData();
    }


    /// <summary>
    /// Handles displaying of library edit form.
    /// </summary>
    private void DisplayLibraryEdit()
    {
        plcTabsHeader.Visible = true;
        pnlTabsMain.Visible = true;

        plcHeaderActions.Visible = false;

        plcList.Visible = false;

        plcTabs.Visible = true;
        tabEdit.Visible = false;
        tabFiles.Visible = true;
        tabSecurity.Visible = false;

        libraryFiles.LibraryID = LibraryID;
        libraryFiles.ReloadData();
    }


    /// <summary>
    /// Displays default media library tab content.
    /// </summary>
    private void SetDefault()
    {
        plcTabsHeader.Visible = false;
        plcHeaderActions.Visible = true;
        plcList.Visible = true;
        plcTabs.Visible = false;
    }

    #endregion


    #region "Event handlers"

    protected void libraryList_OnAction(object sender, CommandEventArgs e)
    {
        string commandName = e.CommandName.ToLowerCSafe();
        switch (commandName)
        {
            case "edit":
                LibraryID = ValidationHelper.GetInteger(e.CommandArgument, 0);

                plcTabs.Visible = true;

                InitializeBreadcrumbs();

                tabElem.SelectedTab = 0;

                // Load library data
                DisplayLibraryEdit();
                break;

            case "delete":
                LibraryID = 0;

                SetupControls();
                break;

            default:
                break;
        }
    }


    private void lnkEditBack_Click(object sender, EventArgs e)
    {
        libraryFiles.ShouldProcess = false;

        SetDefault();
    }


    protected void libraryEdit_OnSaved(object sender, EventArgs e)
    {
        isNewLibrary = (LibraryID == 0);

        LibraryID = libraryEdit.MediaLibraryID;

        InitializeBreadcrumbs();

        // If brand new library created
        if (isNewLibrary)
        {
            // Reload library data
            DisplayLibraryEdit();
        }
    }


    protected void newLibrary_ActionPerformed(object sender, CommandEventArgs e)
    {
        libraryFiles.ShouldProcess = false;

        // Clear the editing form
        libraryEdit.ClearForm();

        // Reset library info in view state and refresh breadcrumbs info
        LibraryID = 0;

        InitializeBreadcrumbs();
        DisplayLibraryNew();
    }


    private void tabElem_OnTabChanged(object sender, EventArgs e)
    {
        plcList.Visible = false;
        plcHeaderActions.Visible = false;

        int tabIndex = tabElem.SelectedTab;

        tabFiles.Visible = (tabIndex == 0);
        if (tabFiles.Visible)
        {
            plcTabs.Visible = true;
            libraryFiles.LibraryID = LibraryID;
            libraryFiles.ReloadData();
        }
        libraryFiles.ShouldProcess = tabFiles.Visible;

        tabEdit.Visible = (tabIndex == 1);
        if (tabEdit.Visible)
        {
            libraryEdit.ReloadData();
        }

        tabSecurity.Visible = (tabIndex == 2);
    }

    #endregion


    #region "IPostBackEventHandler Members"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument.ToLowerCSafe() == "reloadtree")
        {
            libraryFiles.ReLoadUserControl();
        }
    }

    #endregion
}