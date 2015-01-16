using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Categories_Controls_CategoryEdit : CMSAdminEditControl
{
    #region "Variables"

    private int mCategoryId = 0;
    private int mParentCategoryId = 0;
    private CategoryInfo mParentCategory = null;
    private CategoryInfo mCategory = null;
    private int mUserID = 0;
    private UserInfo mUser = null;
    private int mSiteId = -1;
    private bool mShowEnabled = true;
    private bool mShowCodeName = true;
    private string mRefreshPageURL = null;
    private bool mAllowCreateOnlyGlobal = false;
    private bool? mAllowGlobalCategories = null;
    private bool canModifySite = false;
    private bool canModifyGlobal = false;
    private bool mDisplayOkButton = true;
    private bool mWasSaved = false;
    private bool mAllowDisabledParents = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets CssClass of the panel wrapping edit form.
    /// </summary>
    public string PanelCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Component name
    /// </summary>
    public override string ComponentName
    {
        get
        {
            return base.ComponentName;
        }
        set
        {
            headerActions.ComponentName = value;
            base.ComponentName = value;
        }
    }


    /// <summary>
    /// Header actions control
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return headerActions;
        }
    }


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
    /// Current category ID.
    /// </summary>
    public int CategoryID
    {
        get
        {
            return mCategoryId;
        }
        set
        {
            mCategoryId = value;
            mCategory = null;
            mParentCategory = null;
            mParentCategoryId = 0;
        }
    }


    /// <summary>
    /// Edited category object.
    /// </summary>
    public CategoryInfo Category
    {
        get
        {
            if (mCategory == null)
            {
                mCategory = CategoryInfoProvider.GetCategoryInfo(mCategoryId);
            }

            return mCategory;
        }
        set
        {
            mCategory = value;
            mCategoryId = 0;
            mParentCategoryId = 0;
            mParentCategory = null;

            if (value != null)
            {
                mCategoryId = value.CategoryID;
                mParentCategoryId = value.CategoryParentID;
            }
        }
    }


    /// <summary>
    /// Parent category ID for creating new category. Applies only when CategoryID is 0.
    /// </summary>
    public int ParentCategoryID
    {
        get
        {
            return mParentCategoryId;
        }
        set
        {
            mParentCategoryId = value;
            mParentCategory = null;
        }
    }


    /// <summary>
    /// Parent category ID for creating new category. Applies only when CategoryID is 0.
    /// </summary>
    public CategoryInfo ParentCategory
    {
        get
        {
            if (mParentCategory == null)
            {
                mParentCategory = CategoryInfoProvider.GetCategoryInfo(mParentCategoryId);
            }

            return mParentCategory;
        }
        set
        {
            mParentCategory = value;

            mParentCategoryId = 0;
            if (value != null)
            {
                mParentCategoryId = value.CategoryID;
            }
        }
    }


    /// <summary>
    /// ID of the site to create categories for.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteId < 0)
            {
                mSiteId = CMSContext.CurrentSiteID;
            }

            return mSiteId;
        }
        set
        {
            mSiteId = value;

            mAllowGlobalCategories = null;
        }
    }


    /// <summary>
    /// ID of the user to create/edit category for.
    /// </summary>
    public int UserID
    {
        get
        {
            return mUserID;
        }
        set
        {
            mUserID = value;
            mUser = null;
        }
    }


    /// <summary>
    /// User object to create/edit category for.
    /// </summary>
    public UserInfo User
    {
        get
        {
            if (mUser == null)
            {
                mUser = UserInfoProvider.GetUserInfo(UserID);
            }

            return mUser;
        }
        set
        {
            mUser = value;
            mUserID = 0;

            if (value != null)
            {
                mUserID = value.UserID;
            }
        }
    }


    /// <summary>
    /// Indicates if global categories are created by default under global parent category.
    /// </summary>
    public bool AllowCreateOnlyGlobal
    {
        get
        {
            return mAllowCreateOnlyGlobal;
        }
        set
        {
            mAllowCreateOnlyGlobal = value;
        }
    }


    /// <summary>
    /// Show enabled checkbox.
    /// </summary>
    public bool ShowEnabled
    {
        get
        {
            return mShowEnabled;
        }
        set
        {
            mShowEnabled = value;
        }
    }


    /// <summary>
    /// Show code name textbox.
    /// </summary>
    public bool ShowCodeName
    {
        get
        {
            return mShowCodeName;
        }
        set
        {
            mShowCodeName = value;
        }
    }


    /// <summary>
    /// The URL of the page where the result is redirected.
    /// </summary>
    public string RefreshPageURL
    {
        get
        {
            return mRefreshPageURL;
        }
        set
        {
            mRefreshPageURL = value;
        }
    }


    /// <summary>
    /// Indicates whether the category changes were saved.
    /// </summary>
    public bool WasSaved
    {
        get
        {
            return mWasSaved || !string.IsNullOrEmpty(QueryHelper.GetString("saved", string.Empty));
        }
        set
        {
            mWasSaved = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            EnsureChildControls();
            base.StopProcessing = value;

            selectParentCategory.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if OK button will be shown.
    /// </summary>
    public bool DisplayOkButton
    {
        get
        {
            return mDisplayOkButton;
        }
        set
        {
            mDisplayOkButton = value;
        }
    }


    /// <summary>
    /// Indicates if global categories are allowed even when disabled by settings.
    /// </summary>
    public bool AllowGlobalCategories
    {
        get
        {
            if (!mAllowGlobalCategories.HasValue)
            {
                // Get site name
                string siteName = SiteInfoProvider.GetSiteName(SiteID);

                // Figure out from settings
                mAllowGlobalCategories = SettingsKeyProvider.GetBoolValue(siteName + ".CMSAllowGlobalCategories");
            }

            return mAllowGlobalCategories ?? false;
        }
        set
        {
            mAllowGlobalCategories = value;
        }
    }


    /// <summary>
    /// Indicates if disabled categories are allowed in parent category selector. Default value is false.
    /// </summary>
    public bool AllowDisabledParents
    {
        get
        {
            return mAllowDisabledParents;
        }
        set
        {
            mAllowDisabledParents = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the controls
        SetupControl();

        if (!StopProcessing && !RequestHelper.IsPostBack())
        {
            LoadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!String.IsNullOrEmpty(PanelCssClass))
        {
            pnlEdit.CssClass = PanelCssClass;
        }

        if (WasSaved)
        {
            // Display information on success
            ShowChangesSaved();
        }
    }

    #endregion


    #region "Event handling"

    public void btnSaveCategory_Click(object sender, EventArgs e)
    {
        // Validate form entries
        string errorMessage = "";

        // Check "modify" permission
        if (!RaiseOnCheckPermissions("Modify", this))
        {
            if ((UserID > 0) && (UserID != CMSContext.CurrentUser.UserID) && (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Users", "Modify")))
            {
                string.Format(GetString("general.permissionresource"), "Modify", "CMS.Users");
            }
        }

        // Prepare ID of the site for new category
        int newSiteId = SiteID;
        if (canModifyGlobal && (AllowCreateOnlyGlobal || radGlobal.Checked))
        {
            // Create global categories only under global categories or root
            if ((ParentCategory == null) || (ParentCategory.CategoryIsGlobal && !ParentCategory.CategoryIsPersonal))
            {
                newSiteId = 0;
            }
        }

        bool personal = false;
        bool global = false;

        if (Category != null)
        {
            // Prepare personal and global flag for existing category
            personal = Category.CategoryIsPersonal;
            global = Category.CategoryIsGlobal;
        }
        else
        {
            // Prepare personal and global flag for new category
            personal = UserID > 0;
            global = newSiteId == 0;
        }

        errorMessage = ValidateForm(personal, global);

        if (errorMessage == "")
        {
            // Need Modify or GlobalModify permission to edit non-personal categories
            string permission = global ? "GlobalModify" : "Modify";
            if (!personal && !CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Categories", permission))
            {
                errorMessage = string.Format(GetString("general.permissionresource"), permission, "CMS.Categories");
            }
        }


        if (errorMessage == "")
        {
            try
            {
                CategoryInfo category = null;
                // Update existing item
                if (Category != null)
                {
                    category = Category;

                    if (!category.CategoryIsPersonal)
                    {
                        CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Categories", category.CategoryIsGlobal ? "GlobalModify" : "Modify");

                        // Update code name only if not personal category
                        category.CategoryName = GetCodeName();
                    }

                    // Update parent category ID
                    category.CategoryParentID = selectParentCategory.CategoryID;

                    EditedObject = category;
                }
                else
                {
                    // Create a new category info
                    category = new CategoryInfo();

                    // Initialize the default count when creating new category
                    category.CategoryCount = 0;
                    category.CategoryParentID = ParentCategoryID;
                    category.CategoryName = GetCodeName();

                    if (UserID > 0)
                    {
                        category.CategoryUserID = UserID;
                    }
                    else
                    {
                        category.CategorySiteID = newSiteId;
                    }
                }

                if (category != null)
                {
                    // Update category fields
                    txtDisplayName.Save();
                    category.CategoryDisplayName = txtDisplayName.Text.Trim();
                    category.CategoryDescription = txtDescription.Text;
                    category.CategoryEnabled = chkEnabled.Checked;

                    // Save category in the database
                    CategoryInfoProvider.SetCategoryInfo(category);
                    Category = category;

                    if (!string.IsNullOrEmpty(RefreshPageURL))
                    {
                        URLHelper.Redirect(RefreshPageURL + "?categoryid=" + Category.CategoryID.ToString() + "&saved=1");
                    }

                    RaiseOnSaved();

                    // Display information on success
                    ShowChangesSaved();
                }
            }
            catch (Exception ex)
            {
                // Display error message
                ShowError(GetString("general.saveerror"), ex.Message, null);
            }
        }
        else
        {
            // Display error message
            ShowError(errorMessage);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads the category data in the control.
    /// </summary>
    public override void ReloadData()
    {
        if (!StopProcessing)
        {
            base.ReloadData();
            SetupControl();
            LoadData();
            headerActions.ReloadData();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the control.
    /// </summary>
    private void SetupControl()
    {
        // Get and store permissions
        canModifySite = CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Categories", "Modify");
        canModifyGlobal = CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Categories", "GlobalModify");
        canModifyGlobal &= AllowGlobalCategories;

        // Initialize the control labels and error messages
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvCodeName.ErrorMessage = GetString("general.requirescodename");
        radGlobal.Text = GetString("category_edit.globalcategory");
        radSite.Text = GetString("category_edit.sitecategory");
        selectParentCategory.AllowDisabledCategories = AllowDisabledParents;

        btnSaveCategory.Visible = DisplayOkButton;

        txtDisplayName.IsLiveSite = txtDescription.IsLiveSite = IsLiveSite;

        // Handle show enabled checkbox
        if (!ShowEnabled)
        {
            lblEnabled.Visible = false;
            chkEnabled.Visible = false;
            chkEnabled.Checked = true;
        }

        // Handle show code name
        if (!ShowCodeName)
        {
            lblCodeName.Visible = false;
            txtCodeName.Visible = false;
            rfvCodeName.Visible = false;
            if (CategoryID <= 0)
            {
                txtCodeName.Text = Guid.NewGuid().ToString();
            }
        }
    }


    /// <summary>
    /// Loads the data.
    /// </summary>
    private void LoadData()
    {
        // Handle existing category editing - prepare the data
        if (CategoryID > 0)
        {
            // Perform actions when editing existing category
            HandleExistingCategory();
        }
        else
        {
            // Clear textboxes
            txtCodeName.Text = "";
            txtDescription.Text = "";
            txtDisplayName.Text = "";
            selectParentCategory.SiteID = SiteID;
            selectParentCategory.UserID = UserID;
            selectParentCategory.CategoryID = ParentCategoryID;
            plcParentCategory.Visible = false;
            //this.selectParentCategory.Enabled = false;

            bool personal = UserID > 0;

            // Display type selector only if there is a choice (according to permissions and settings) 
            bool canChooseType = canModifyGlobal && canModifySite;

            if (ParentCategory != null)
            {
                plcCategorySite.Visible = canChooseType && ParentCategory.CategoryIsGlobal && !ParentCategory.CategoryIsPersonal && !AllowCreateOnlyGlobal;
            }
            else
            {
                plcCategorySite.Visible = canChooseType && !personal && !AllowCreateOnlyGlobal;
            }

            // Select site type when can not create global category
            if (!canModifySite)
            {
                radGlobal.Checked = true;
                radSite.Checked = false;
            }

            // Select global type when can not create site category
            if (!canModifyGlobal)
            {
                radGlobal.Checked = false;
                radSite.Checked = true;
            }

            plcEnabled.Visible = !personal;
            plcCodeName.Visible = !personal;
        }

        selectParentCategory.ReloadData();
    }


    /// <summary>
    /// Handles actions related to the existing category editing.
    /// </summary>
    private void HandleExistingCategory()
    {
        // Get information on current category
        EditedObject = Category;
        if (Category != null)
        {
            // Pre-fill the data of current category
            txtDisplayName.Text = Category.CategoryDisplayName;
            txtCodeName.Text = Category.CategoryName;
            txtDescription.Text = Category.CategoryDescription;
            chkEnabled.Checked = Category.CategoryEnabled;

            selectParentCategory.UserID = Category.CategoryUserID;

            selectParentCategory.SiteID = Category.CategorySiteID;

            selectParentCategory.CategoryID = Category.CategoryParentID;
            selectParentCategory.ExcludeCategoryID = Category.CategoryID;
            selectParentCategory.DisableSiteCategories = Category.IsGlobal;
            plcParentCategory.Visible = true;
            selectParentCategory.Enabled = true;
            plcCategorySite.Visible = false;

            bool personal = Category.CategoryIsPersonal;

            plcEnabled.Visible = !personal;
            plcCodeName.Visible = !personal;
        }
    }


    /// <summary>
    /// Returns true if given codename is unique, otherwise false.
    /// </summary>
    /// <param name="newCodeName">Code name of the category.</param>
    /// <param name="isPersonal">Indicates if category is personal.</param>
    /// <param name="isGlobal">Indicates if category is global.</param>
    private bool CodeNameIsUnique(string newCodeName, bool isPersonal, bool isGlobal)
    {
        string where = "CategoryName = N'" + SqlHelperClass.GetSafeQueryString(newCodeName, false) + "'";

        // Check if site category
        if (!isPersonal && !isGlobal)
        {
            // Look for category in global, personal and selected site categories
            where = SqlHelperClass.AddWhereCondition(where, "CategorySiteID = " + SiteID + " OR CategorySiteID IS NULL");
        }

        // Get existing category
        DataSet ds = CategoryInfoProvider.GetCategories(where, "CategoryID", 1, "CategoryID");
        if (DataHelper.DataSourceIsEmpty(ds))
        {
            return true;
        }
        else
        {
            int id = ValidationHelper.GetInteger(ds.Tables[0].Rows[0]["CategoryID"], 0);

            // If the existing category is updated the code name already exists
            return (id > 0) && (CategoryID == id);
        }
    }


    /// <summary>
    /// Validates the form entries. Returns empty string if validation passed otherwise error message is returned.
    /// </summary>
    /// <param name="isPersonal">Indicates if category is personal.</param>
    /// <param name="isGlobal">Indicates if category is global.</param>
    private string ValidateForm(bool isPersonal, bool isGlobal)
    {
        string codeName = GetCodeName();

        string errorMessage = new Validator()
            .NotEmpty(codeName, GetString("general.requirescodename"))
            .NotEmpty(txtDisplayName.Text.Trim(), GetString("general.requiresdisplayname")).Result;

        if (string.IsNullOrEmpty(errorMessage) && !ValidationHelper.IsCodeName(codeName))
        {
            errorMessage = GetString("General.ErrorCodeNameInIdentifierFormat");
        }

        if (string.IsNullOrEmpty(errorMessage) && (txtDisplayName.Text.IndexOfCSafe('/') >= 0))
        {
            errorMessage = GetString("category_edit.SlashNotAllowedInDisplayName");
        }

        if (string.IsNullOrEmpty(errorMessage))
        {
            // Validate display name & code name entry
            rfvDisplayName.Validate();
            rfvCodeName.Validate();

            if ((rfvDisplayName.IsValid) && (rfvCodeName.IsValid))
            {
                // Validate code name entry
                if (!CodeNameIsUnique(codeName, isPersonal, isGlobal))
                {
                    errorMessage = GetString(isPersonal ? "category_edit.personalcategoryexists" : "general.codenameexists");

                    // Display validation error message
                    ShowError(errorMessage);
                }
            }
            else
            {
                errorMessage = rfvCodeName.ErrorMessage;
            }
        }

        return errorMessage;
    }


    /// <summary>
    /// Generates code name for currently created/edited category.
    /// </summary>
    private string GetCodeName()
    {
        if (Category != null)
        {
            // Use text box value when editing non-personal category
            if (!Category.CategoryIsPersonal)
            {
                return txtCodeName.Text.Trim();
            }

            // Personal categories can not change codename
            return Category.CategoryName;
        }
        else
        {
            if (UserID > 0)
            {
                if (User != null)
                {
                    // Generate codename for new personal category
                    string codeName = ValidationHelper.GetCodeName(User.UserName + "_" + txtDisplayName.Text.Trim());
                    return TextHelper.LimitLength(codeName, 250, "", false);
                }
            }

            // Use text box value when creating non-personal category
            return txtCodeName.Text.Trim();
        }
    }

    #endregion
}