using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_OnlineMarketing_Controls_UI_MVTest_Edit : CMSAdminEditControl
{
    #region "Variables"

    private MVTestInfo mMvtestObj = null;
    private int mMvtestId = 0;
    private string mAliasPath = string.Empty;
    private bool mShowAliasPath = true;
    private int siteId = CMSContext.CurrentSiteID;

    #endregion


    #region "Properties"

    /// <summary>
    /// Mvtest data.
    /// </summary>
    public MVTestInfo MvtestObj
    {
        get
        {
            if (mMvtestObj == null)
            {
                mMvtestObj = MVTestInfoProvider.GetMVTestInfo(MVTestID);
            }

            return mMvtestObj;
        }
        set
        {
            mMvtestObj = value;
            if (value != null)
            {
                mMvtestId = value.MVTestID;
            }
            else
            {
                mMvtestId = 0;
            }
        }
    }


    /// <summary>
    /// Mvtest ID.
    /// </summary>
    public int MVTestID
    {
        get
        {
            return mMvtestId;
        }
        set
        {
            mMvtestId = value;
            mMvtestObj = null;
        }
    }


    /// <summary>
    /// If true show alias path selector.
    /// </summary>
    public bool ShowAliasPath
    {
        get
        {
            // If alias path is set dont show this selector
            if (AliasPath != String.Empty)
            {
                return false;
            }
            return mShowAliasPath;
        }
        set
        {
            mShowAliasPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the alias path.
    /// </summary>
    public string AliasPath
    {
        get
        {
            return mAliasPath;
        }
        set
        {
            mAliasPath = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            plcMess.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        if (!ShowAliasPath)
        {
            plcMVTestPage.Visible = false;
        }

        SetupControls();

        // Set edited object
        if (MVTestID > 0)
        {
            EditedObject = MvtestObj;
        }
        else
        {
            pnlConversions.Visible = false;
        }

        // Disabled field - load not only on postback
        if (MvtestObj != null)
        {
            txtMVTestConversions.Text = MvtestObj.MVTestConversions.ToString();
        }

        // Empty textbox for zero values
        if (txtMVTestMaxConversions.Text.Trim() == "0")
        {
            txtMVTestMaxConversions.Text = String.Empty;
        }

        // Load the form data
        if (!URLHelper.IsPostback())
        {
            LoadData();
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Set status
        lblStatusValue.ResourceString = "mvtest.status.";

        // Disabled by default
        MVTestStatusEnum status = MVTestStatusEnum.Disabled;
        if (MvtestObj != null)
        {
            // Set correct status has been
            status = MVTestInfoProvider.GetMVTestStatus(MvtestObj);
        }

        // Set the current status resource string
        lblStatusValue.ResourceString += status;

        switch (status)
        {
            case MVTestStatusEnum.Running:
                lblStatusValue.CssClass = "StatusEnabled";
                break;

            case MVTestStatusEnum.Disabled:
                lblStatusValue.CssClass = "StatusDisabled";
                break;
        }
    }


    /// <summary>
    /// Handles the Click event of the btnOk control.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Validate and save the data
        Process();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes form controls.
    /// </summary>
    private void SetupControls()
    {
        // Button
        btnOk.Text = GetString("general.ok");

        // Path selector
        ucPath.SiteID = siteId;

        // Validators
        rfvMVTestCodeName.ErrorMessage = GetString("general.requirescodename");
        rfvMVTestDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvMVTestMaxConversions.MaximumValue = Int32.MaxValue.ToString();
        rfvMVTestMaxConversions.MinimumValue = "0";
        rfvMVTestMaxConversions.ErrorMessage = String.Format(GetString("general.outofrange"), rfvMVTestMaxConversions.MinimumValue, rfvMVTestMaxConversions.MaximumValue);

        // Display 'Changes were saved' message if required
        if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("saved", false))
        {
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Loads the data into the form.
    /// </summary>
    private void LoadData()
    {
        // Load the form from the info object
        if (MvtestObj != null)
        {
            txtMVTestCodeName.Text = MvtestObj.MVTestName;
            txtMVTestDisplayName.Text = MvtestObj.MVTestDisplayName;
            txtMVTestDescription.Text = MvtestObj.MVTestDescription;
            ucPath.Value = MvtestObj.MVTestPage;
            ucCultureSelector.Value = MvtestObj.MVTestCulture;

            // For 0 - empty textbox
            if (MvtestObj.MVTestMaxConversions != 0)
            {
                txtMVTestMaxConversions.Text = MvtestObj.MVTestMaxConversions.ToString();
            }

            dtpMVTestOpenFrom.SelectedDateTime = MvtestObj.MVTestOpenFrom;
            dtpMVTestOpenTo.SelectedDateTime = MvtestObj.MVTestOpenTo;
            chkMVTestEnabled.Checked = MvtestObj.MVTestEnabled;

            switch (MvtestObj.MVTestTargetConversionType)
            {
                case MVTTargetConversionTypeEnum.AnyCombination:
                    radAnyVariant.Checked = true;
                    break;

                case MVTTargetConversionTypeEnum.Total:
                default:
                    radTotal.Checked = true;
                    break;
            }
        }
    }


    /// <summary>
    // Processes the form - saves the data.
    /// </summary>
    private void Process()
    {
        if (CheckPermissions("CMS.MVTest", PERMISSION_MANAGE))
        {
            // Validate the form
            if (Validate())
            {
                // Ensure the info object
                if (MvtestObj == null)
                {
                    MvtestObj = new MVTestInfo();
                    MvtestObj.MVTestSiteID = siteId;
                    MvtestObj.MVTestPage = AliasPath;
                }

                // Initialize object
                String newCodeName = txtMVTestCodeName.Text.Trim();
                MvtestObj.MVTestDisplayName = txtMVTestDisplayName.Text.Trim();
                MvtestObj.MVTestDescription = txtMVTestDescription.Text;
                MvtestObj.OriginalCulture = MvtestObj.MVTestCulture;
                MvtestObj.MVTestCulture = ucCultureSelector.Value.ToString();
                MvtestObj.MVTestMaxConversions = ValidationHelper.GetInteger(txtMVTestMaxConversions.Text, 0);
                MvtestObj.MVTestOpenFrom = dtpMVTestOpenFrom.SelectedDateTime;
                MvtestObj.MVTestOpenTo = dtpMVTestOpenTo.SelectedDateTime;
                MvtestObj.MVTestEnabled = chkMVTestEnabled.Checked;

                // Name has changed. Change analytics statistics data for existing object
                if ((MvtestObj.MVTestID != 0) && (MvtestObj.MVTestName != newCodeName))
                {
                    MVTestInfoProvider.RenameMVTestStatistics(MvtestObj.MVTestName, newCodeName, CMSContext.CurrentSiteID);
                }
                MvtestObj.MVTestName = newCodeName;

                if (radTotal.Checked)
                {
                    MvtestObj.MVTestTargetConversionType = MVTTargetConversionTypeEnum.Total;
                }
                else if (radAnyVariant.Checked)
                {
                    MvtestObj.MVTestTargetConversionType = MVTTargetConversionTypeEnum.AnyCombination;
                }

                if (ShowAliasPath)
                {
                    MvtestObj.MVTestPage = ucPath.Value.ToString().Trim();
                }

                // Save object data to database
                MVTestInfoProvider.SetMVTestInfo(MvtestObj);

                ItemID = MvtestObj.MVTestID;
                RaiseOnSaved();

                // Set the info message
                ShowChangesSaved();
            }
        }
    }


    /// <summary>
    /// Validates the form. If validation succeeds returns true, otherwise returns false.
    /// </summary>
    private bool Validate()
    {
        string codename = txtMVTestCodeName.Text.Trim();

        // Validate required fields
        string errorMessage = new Validator()
            .NotEmpty(txtMVTestDisplayName.Text.Trim(), rfvMVTestDisplayName.ErrorMessage)
            .NotEmpty(codename, rfvMVTestCodeName.ErrorMessage)
            .IsCodeName(codename, GetString("general.invalidcodename")).Result;

        // Prepare the properties required for validation (codename + siteID)
        MVTestInfo tempMvtTest = new MVTestInfo();
        tempMvtTest.MVTestName = codename;
        tempMvtTest.MVTestSiteID = siteId;
        if (MvtestObj != null)
        {
            tempMvtTest.MVTestID = MvtestObj.MVTestID;
        }

        // Check the uniqueness of the codename
        if (!tempMvtTest.CheckUniqueCodeName())
        {
            errorMessage = GetString("general.codenameexists");
        }

        if (!dtpMVTestOpenFrom.IsValidRange() || !dtpMVTestOpenTo.IsValidRange())
        {
            errorMessage = GetString("general.errorinvaliddatetimerange");
        }

        if ((dtpMVTestOpenFrom.SelectedDateTime != DateTimeHelper.ZERO_TIME)
            && (dtpMVTestOpenTo.SelectedDateTime != DateTimeHelper.ZERO_TIME)
            && (dtpMVTestOpenFrom.SelectedDateTime > dtpMVTestOpenTo.SelectedDateTime))
        {
            errorMessage = GetString("om.wrongtimeinterval");
        }

        string currentConversions = txtMVTestConversions.Text.Trim();
        string maxConversions = txtMVTestMaxConversions.Text.Trim();
        if (!String.IsNullOrEmpty(currentConversions) && (String.IsNullOrEmpty(errorMessage)))
        {
            errorMessage = new Validator().IsInteger(currentConversions, GetString("om.currentconversionrequiresinteger")).IsPositiveNumber(currentConversions, GetString("om.currentconversionrequiresinteger")).Result;
        }

        if (!String.IsNullOrEmpty(maxConversions) && (String.IsNullOrEmpty(errorMessage)))
        {
            errorMessage = new Validator().IsInteger(maxConversions, GetString("om.targetconversionrequiresinteger")).IsPositiveNumber(maxConversions, GetString("om.targetconversionrequiresinteger")).Result;
        }


        if (string.IsNullOrEmpty(ucPath.Value.ToString().Trim()) && (AliasPath == String.Empty))
        {
            errorMessage = GetString("mvtest.pagerequired");
        }

        // Test if there is no enabled test for same page
        if (chkMVTestEnabled.Checked && TestToValidate())
        {
            QueryDataParameters parameters = null;
            string testPage = ((AliasPath != String.Empty) && !ShowAliasPath) ? AliasPath : ucPath.Value.ToString();
            string where = MVTestInfoProvider.GetRunningCondition(MVTestID, testPage, siteId, ucCultureSelector.Value.ToString(), dtpMVTestOpenFrom.SelectedDateTime, dtpMVTestOpenTo.SelectedDateTime, out parameters);
            DataSet ds = MVTestInfoProvider.GetMVTests(where, null, parameters);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                errorMessage = GetString("om.twotestsonepageerror");
            }
        }

        // Set the error message
        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return false;
        }

        return true;
    }


    /// <summary>
    /// Returns true, if test should be validated for test page
    /// </summary>
    private bool TestToValidate()
    {
        if (chkMVTestEnabled.Checked)
        {
            int max = ValidationHelper.GetInteger(txtMVTestMaxConversions.Text.Trim(), 0);
            int conversions = (MvtestObj != null) ? MvtestObj.MVTestConversions : 0;
            if ((max == 0) || (max > conversions))
            {
                DateTime dtTo = ValidationHelper.GetDateTime(dtpMVTestOpenTo.SelectedDateTime, DateTimeHelper.ZERO_TIME);
                if ((dtTo == DateTimeHelper.ZERO_TIME) || (dtTo > DateTime.Now))
                {
                    return true;
                }
            }
        }

        return false;
    }

    #endregion
}