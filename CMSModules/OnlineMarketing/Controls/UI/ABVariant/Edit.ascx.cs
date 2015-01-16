using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_OnlineMarketing_Controls_UI_ABVariant_Edit : CMSAdminEditControl
{
    #region "Variables"

    private ABVariantInfo mVariantObj = null;
    private int mVariantId = 0;
    private int mTestID;

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
    /// Parent test ID.
    /// </summary>
    public int TestID
    {
        get
        {
            return mTestID;
        }
        set
        {
            mTestID = value;
        }
    }

    /// <summary>
    /// Variant data.
    /// </summary>
    public ABVariantInfo VariantObj
    {
        get
        {
            if (mVariantObj == null)
            {
                mVariantObj = ABVariantInfoProvider.GetABVariantInfo(ABVariantID);
            }

            return mVariantObj;
        }
        set
        {
            mVariantObj = value;
            if (value != null)
            {
                mVariantId = value.ABVariantID;
            }
            else
            {
                mVariantId = 0;
            }
        }
    }


    /// <summary>
    /// Variant ID.
    /// </summary>
    public int ABVariantID
    {
        get
        {
            return mVariantId;
        }
        set
        {
            mVariantId = value;
            mVariantObj = null;
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

        SetupControls();

        // Set edited object
        if (ABVariantID > 0)
        {
            EditedObject = VariantObj;
        }

        // Load the form data
        if (!URLHelper.IsPostback())
        {
            LoadData();
        }
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
        // Validators
        rfvABVariantName.ErrorMessage = GetString("general.requirescodename");
        rfvABVariantDisplayName.ErrorMessage = GetString("general.requiresdisplayname");

        // Display 'Changes were saved' message if required
        if (QueryHelper.GetBoolean("saved", false))
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
        if (VariantObj != null)
        {
            txtABVariantDisplayName.Text = VariantObj.ABVariantDisplayName;
            txtABVariantName.Text = VariantObj.ABVariantName;
            ucPath.Value = VariantObj.ABVariantPath;
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
            // Ensure the info object
            if (VariantObj == null)
            {
                VariantObj = new ABVariantInfo();
                VariantObj.ABVariantTestID = TestID;
                VariantObj.ABVariantSiteID = CMSContext.CurrentSiteID;
            }

            // Initialize object
            VariantObj.ABVariantDisplayName = txtABVariantDisplayName.Text.Trim();
            VariantObj.ABVariantName = txtABVariantName.Text.Trim();
            VariantObj.ABVariantPath = ucPath.Value.ToString().Trim();

            // Save object data to database
            ABVariantInfoProvider.SetABVariantInfo(VariantObj);

            ItemID = VariantObj.ABVariantID;
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
        // Validate required fields
        string errorMessage = new Validator()
            .NotEmpty(txtABVariantDisplayName.Text.Trim(), rfvABVariantDisplayName.ErrorMessage)
            .NotEmpty(txtABVariantName.Text.Trim(), rfvABVariantName.ErrorMessage)
            .IsCodeName(txtABVariantName.Text.Trim(), GetString("general.invalidcodename")).Result;

        string abTestName = string.Empty;
        string siteName = CMSContext.CurrentSiteName;

        // Get AB test info object
        ABTestInfo abti = ABTestInfoProvider.GetABTestInfo(TestID);
        if (abti != null)
        {
            abTestName = abti.ABTestName;
        }

        // Test unique codename
        ABVariantInfo info = ABVariantInfoProvider.GetABVariantInfo(txtABVariantName.Text.Trim(), abTestName, siteName);
        if ((info != null) && ((VariantObj == null) || (info.ABVariantID != VariantObj.ABVariantID)))
        {
            errorMessage = GetString("general.codenameexists");
        }

        if (String.IsNullOrEmpty(ucPath.Value.ToString()))
        {
            errorMessage = GetString("abtesting.entertestpage");
        }

        // Set the error message
        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return false;
        }

        return true;
    }

    #endregion
}