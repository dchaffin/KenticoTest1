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
using CMS.WebAnalytics;
using CMS.ExtendedControls;

public partial class CMSModules_OnlineMarketing_Controls_UI_AbTest_Edit : CMSAdminEditControl
{
    #region "Variables"

    private ABTestInfo mAbTestObj = null;
    private int mAbTestId = 0;
    private string mAliasPath = String.Empty;
    private bool mShowAliasPath = true;

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
    /// Ab test data.
    /// </summary>
    public ABTestInfo AbTestObj
    {
        get
        {
            if (mAbTestObj == null)
            {
                mAbTestObj = ABTestInfoProvider.GetABTestInfo(ABTestID);
            }

            return mAbTestObj;
        }
        set
        {
            mAbTestObj = value;
            if (value != null)
            {
                mAbTestId = value.ABTestID;
            }
            else
            {
                mAbTestId = 0;
            }
        }
    }


    /// <summary>
    /// If true show alias path selector.
    /// </summary>
    public bool ShowAliasPath
    {
        get
        {
            // If alias path is set don't show this selector
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
    /// Alias path of document to which this abtest belongs.
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
    /// Ab test ID.
    /// </summary>
    public int ABTestID
    {
        get
        {
            return mAbTestId;
        }
        set
        {
            mAbTestId = value;
            mAbTestObj = null;
        }
    }


    /// <summary>
    /// Indicates if control is placed on live site
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
            plcOriginalPage.Visible = false;
        }

        SetupControls();

        // Set edited object
        if (ABTestID > 0)
        {
            EditedObject = AbTestObj;
        }
        else
        {
            // Hide conversions textbox for new ABTests
            pnlConversions.Visible = false;
        }

        // Disabled field - load not only on postback
        if (AbTestObj != null)
        {
            txtABTestConversions.Text = AbTestObj.ABTestConversions.ToString();
        }

        // Empty textbox for zero values
        if (txtABTestMaxConversions.Text.Trim() == "0")
        {
            txtABTestMaxConversions.Text = String.Empty;
        }

        // Load the form data
        if (!URLHelper.IsPostback())
        {
            LoadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string status = string.Empty;

        // Set status
        if (AbTestObj != null)
        {
            if (!AbTestObj.ABTestEnabled)
            {
                status = "<span class=\"StatusDisabled\">" + GetString("general.disabled") + "</span>";
            }
            else
            {
                if (ABTestInfoProvider.ABTestIsRunning(AbTestObj))
                {
                    // Display disabled information
                    if (!AnalyticsHelper.AnalyticsEnabled(CMSContext.CurrentSiteName)
                        || !AnalyticsHelper.TrackConversionsEnabled(CMSContext.CurrentSiteName))
                    {
                        status = GetString("abtesting.statusNone");
                    }
                    else
                    {
                        if (!ABTestInfoProvider.ContainsVariants(AbTestObj))
                        {
                            // Display warning when the test does not contain any variant
                            status = "<img src=\"" + GetImageUrl("Design/Controls/UniGrid/Actions/Warning.png") + "\" alt=\"" + GetString("abtest.novariants") + "\" title=\""
                                     + GetString("abtest.novariants") + "\" />&nbsp;&nbsp;"
                                     + GetString("abtesting.statusNone");
                        }
                        else
                        {
                            status = "<span class=\"StatusEnabled\">" + GetString("abtesting.status" + ABTestInfoProvider.GetABTestStatus(AbTestObj)) + "</span>";
                        }
                    }
                }
                else
                {
                    status = GetString("abtesting.status" + ABTestInfoProvider.GetABTestStatus(AbTestObj));
                }
            }
        }
        else
        {
            status = GetString("general.none");
        }

        ltrStatusValue.Text = status;
    }


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
        // Path selector
        ucPath.SiteID = CMSContext.CurrentSiteID;

        // Validators
        rfvABTestName.ErrorMessage = GetString("general.requirescodename");
        rfvABTestDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvABTestMaxConversions.MaximumValue = Int32.MaxValue.ToString();
        rfvABTestMaxConversions.MinimumValue = "0";
        rfvABTestMaxConversions.ErrorMessage = String.Format(GetString("general.outofrange"), rfvABTestMaxConversions.MinimumValue, rfvABTestMaxConversions.MaximumValue);

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
        if (AbTestObj != null)
        {
            txtABTestName.Text = AbTestObj.ABTestName;
            txtABTestDisplayName.Text = AbTestObj.ABTestDisplayName;
            txtABTestDescription.Text = AbTestObj.ABTestDescription;

            // Show empty textbox for zero value
            if (AbTestObj.ABTestMaxConversions != 0)
            {
                txtABTestMaxConversions.Text = AbTestObj.ABTestMaxConversions.ToString();
            }
            dtpABTestOpenFrom.SelectedDateTime = AbTestObj.ABTestOpenFrom;
            dtpABTestOpenTo.SelectedDateTime = AbTestObj.ABTestOpenTo;
            ucPath.Value = AbTestObj.ABTestOriginalPage;
            chkABTestEnabled.Checked = AbTestObj.ABTestEnabled;
            ucCultureSelector.Value = AbTestObj.ABTestCulture;

            switch (AbTestObj.ABTestTargetConversionType)
            {
                case TargetConversionType.Total:
                    radTotal.Checked = true;
                    break;

                case TargetConversionType.AnyVariant:
                    radAnyVariant.Checked = true;
                    break;

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
        // Check manage permissions
        if (!CheckPermissions("CMS.ABTest", PERMISSION_MANAGE))
        {
            return;
        }

        // Validate the form
        if (Validate())
        {
            bool isNew = false;

            // Ensure the info object
            if (AbTestObj == null)
            {
                AbTestObj = new ABTestInfo();
                if (AliasPath != String.Empty)
                {
                    AbTestObj.ABTestOriginalPage = AliasPath;
                }

                isNew = true;
            }

            String dName = txtABTestDisplayName.Text.Trim();
            if (AbTestObj.ABTestDisplayName != dName)
            {
                ScriptHelper.RefreshTabHeader(Page, null);
            }

            // Initialize object
            String newName = txtABTestName.Text.Trim();
            AbTestObj.ABTestDisplayName = dName;
            AbTestObj.ABTestDescription = txtABTestDescription.Text.Trim();
            AbTestObj.ABTestMaxConversions = ValidationHelper.GetInteger(txtABTestMaxConversions.Text.Trim(), 0);
            AbTestObj.ABTestOpenFrom = dtpABTestOpenFrom.SelectedDateTime;
            AbTestObj.ABTestOpenTo = dtpABTestOpenTo.SelectedDateTime;
            AbTestObj.ABTestCulture = ucCultureSelector.Value.ToString();
            AbTestObj.ABTestEnabled = chkABTestEnabled.Checked;
            AbTestObj.ABTestSiteID = CMSContext.CurrentSiteID;

            // Name has changed. Change analytics statistics data for existing object
            if ((AbTestObj.ABTestID != 0) && (AbTestObj.ABTestName != newName))
            {
                ABTestInfoProvider.RenameABTestStatistics(AbTestObj.ABTestName, newName, CMSContext.CurrentSiteID);
            }
            AbTestObj.ABTestName = newName;

            // Store conversion type
            TargetConversionType conversionType = TargetConversionType.Total;
            if (radAnyVariant.Checked)
            {
                conversionType = TargetConversionType.AnyVariant;
            }
            AbTestObj.ABTestTargetConversionType = conversionType;

            if (ShowAliasPath)
            {
                AbTestObj.ABTestOriginalPage = ucPath.Value.ToString().Trim();
            }

            // Save object data to database
            ABTestInfoProvider.SetABTestInfo(AbTestObj);

            ItemID = AbTestObj.ABTestID;

            // For new A/B test create default variant
            if (isNew)
            {
                // Create instance of ab variant
                ABVariantInfo variant = new ABVariantInfo();

                // Set properties
                variant.ABVariantPath = AbTestObj.ABTestOriginalPage;
                variant.ABVariantTestID = AbTestObj.ABTestID;
                variant.ABVariantDisplayName = AbTestObj.ABTestDisplayName + " " + GetString("om.variant");
                variant.ABVariantName = AbTestObj.ABTestName;
                variant.ABVariantSiteID = AbTestObj.ABTestSiteID;

                // Save to the storage
                ABVariantInfoProvider.SetABVariantInfo(variant);
            }

            RaiseOnSaved();

            // Set the info message
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Validates the form. If validation succeeds returns true, otherwise returns false.
    /// </summary>
    private bool Validate()
    {
        string codename = txtABTestName.Text.Trim();

        // Validate required fields
        string errorMessage = new Validator()
            .NotEmpty(txtABTestDisplayName.Text.Trim(), rfvABTestDisplayName.ErrorMessage)
            .NotEmpty(codename, rfvABTestName.ErrorMessage)
            .IsCodeName(codename, GetString("general.invalidcodename")).Result;

        if (!dtpABTestOpenFrom.IsValidRange() || !dtpABTestOpenTo.IsValidRange())
        {
            errorMessage = GetString("general.errorinvaliddatetimerange");
        }

        if ((dtpABTestOpenFrom.SelectedDateTime != DateTimeHelper.ZERO_TIME)
            && (dtpABTestOpenTo.SelectedDateTime != DateTimeHelper.ZERO_TIME)
            && (dtpABTestOpenFrom.SelectedDateTime > dtpABTestOpenTo.SelectedDateTime))
        {
            errorMessage = GetString("om.wrongtimeinterval");
        }

        string maxConversions = txtABTestMaxConversions.Text.Trim();
        if (!String.IsNullOrEmpty(maxConversions) && (String.IsNullOrEmpty(errorMessage)))
        {
            errorMessage = new Validator().IsInteger(maxConversions, GetString("om.targetconversionrequiresinteger")).IsPositiveNumber(maxConversions, GetString("om.targetconversionrequiresinteger")).Result;
        }

        // Check the uniqueness of the codename
        ABTestInfo info = ABTestInfoProvider.GetABTestInfo(txtABTestName.Text.Trim(), CMSContext.CurrentSiteName);
        if ((info != null) && ((AbTestObj == null) || (info.ABTestID != AbTestObj.ABTestID)))
        {
            errorMessage = GetString("general.codenameexists");
        }

        if (String.IsNullOrEmpty(ucPath.Value.ToString()) && (AliasPath == String.Empty))
        {
            errorMessage = GetString("abtesting.enteroriginalpage");
        }

        // Test if there is no enabled test for same page
        if (chkABTestEnabled.Checked && TestToValidate())
        {
            QueryDataParameters parameters = null;
            string testPage = ((AliasPath != String.Empty) && !ShowAliasPath) ? AliasPath : ucPath.Value.ToString();
            string where = ABTestInfoProvider.GetRunningCondition(ABTestID, testPage, CMSContext.CurrentSiteID, ucCultureSelector.Value.ToString(), dtpABTestOpenFrom.SelectedDateTime, dtpABTestOpenTo.SelectedDateTime, out parameters);
            DataSet ds = ABTestInfoProvider.GetABTests(where, null, -1, null, parameters);
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
        if (chkABTestEnabled.Checked)
        {
            int max = ValidationHelper.GetInteger(txtABTestMaxConversions.Text.Trim(), 0);
            int conversions = (AbTestObj != null) ? AbTestObj.ABTestConversions : 0;
            if ((max == 0) || (max > conversions))
            {
                DateTime dtTo = ValidationHelper.GetDateTime(dtpABTestOpenTo.SelectedDateTime, DateTimeHelper.ZERO_TIME);
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