using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Membership_Controls_Membership_Edit : CMSAdminEditControl
{
    #region "Variables"

    private MembershipInfo mMembershipObj = null;
    private int mMembershipId = 0;
    private int mSiteID = 0;

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
    /// Membership data.
    /// </summary>
    public MembershipInfo MembershipObj
    {
        get
        {
            if (mMembershipObj == null)
            {
                mMembershipObj = MembershipInfoProvider.GetMembershipInfo(MembershipID);
            }

            return mMembershipObj;
        }
        set
        {
            mMembershipObj = value;
            if (value != null)
            {
                mMembershipId = value.MembershipID;
            }
            else
            {
                mMembershipId = 0;
            }
        }
    }


    /// <summary>
    /// SiteID filter - if 0 global membership is used.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Membership ID.
    /// </summary>
    public int MembershipID
    {
        get
        {
            return mMembershipId;
        }
        set
        {
            mMembershipId = value;
            mMembershipObj = null;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
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
        if (MembershipID > 0)
        {
            EditedObject = (MembershipInfo)MembershipObj;
        }

        // Load the form data
        if (!URLHelper.IsPostback())
        {
            ReloadData();
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
        rfvMembershipCodeName.ErrorMessage = GetString("general.requirescodename");
        rfvMembershipName.ErrorMessage = GetString("general.requiresdisplayname");

        // Display 'Changes were saved' message if required
        if (QueryHelper.GetBoolean("saved", false) && !URLHelper.IsPostback())
        {
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Loads the data into the form.
    /// </summary>
    public override void ReloadData()
    {
        // Load the form from the info object
        if (MembershipObj != null)
        {
            txtMembershipName.Text = MembershipObj.MembershipDisplayName;
            txtMembershipCodeName.Text = MembershipObj.MembershipName;
            txtMembershipDescription.Text = MembershipObj.MembershipDescription;
        }
    }


    /// <summary>
    // Processes the form - saves the data.
    /// </summary>
    private void Process()
    {
        // Check "modify" permission
        if (!CheckPermissions("CMS.Membership", PERMISSION_MODIFY))
        {
            return;
        }

        // Ensure the info object
        if (MembershipObj == null)
        {
            MembershipObj = new MembershipInfo();
            MembershipObj.MembershipSiteID = SiteID;
        }

        if (MembershipObj.MembershipDisplayName != txtMembershipName.Text.Trim())
        {
            // Refresh a breadcrumb if used in the tabs layout
            ScriptHelper.RefreshTabHeader(Page, string.Empty);
        }

        // Initialize object
        MembershipObj.MembershipDisplayName = txtMembershipName.Text.Trim();
        MembershipObj.MembershipName = txtMembershipCodeName.Text.Trim();
        MembershipObj.MembershipDescription = txtMembershipDescription.Text.Trim();

        // Validate the form
        if (Validate())
        {
            // Save object data to database
            MembershipInfoProvider.SetMembershipInfo(MembershipObj);

            ItemID = MembershipObj.MembershipID;
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
            .NotEmpty(txtMembershipCodeName.Text.Trim(), rfvMembershipCodeName.ErrorMessage)
            .NotEmpty(txtMembershipName.Text.Trim(), rfvMembershipName.ErrorMessage)
            .IsCodeName(txtMembershipCodeName.Text.Trim(), GetString("general.invalidcodename")).Result;

        MembershipObj.Generalized.CheckUnique = false;
        MembershipObj.Generalized.ValidateCodeName = false;

        // If not unique codename don't save
        if (!MembershipObj.CheckUniqueCodeName())
        {
            ShowError(String.Format(GetString("membership.notuniquecodename"), MembershipObj.MembershipName));

            return false;
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