using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Polls;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Polls_Controls_PollNew : CMSAdminEditControl
{
    #region "Variables"

    private int mGroupID = 0;
    private int mSiteID = 0;
    private Guid mGroupGUID = Guid.Empty;
    private string mLicenseError = null;
    private bool error = false;

    #endregion


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
    /// Gets or sets the site ID for which the poll should be created.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteID <= 0)
            {
                mSiteID = CMSContext.CurrentSiteID;
            }

            return mSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Gets or sets the group ID for which the poll should be created.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupID;
        }
        set
        {
            mGroupID = value;
        }
    }


    /// <summary>
    /// Gets or sets the group GUID for which the poll should be created.
    /// </summary>
    public Guid GroupGUID
    {
        get
        {
            return mGroupGUID;
        }
        set
        {
            mGroupGUID = value;
        }
    }


    /// <summary>
    /// Gets or sets license error message.
    /// </summary>
    public string LicenseError
    {
        get
        {
            return mLicenseError;
        }
        set
        {
            mLicenseError = value;
        }
    }


    /// <summary>
    /// Indicates if a poll should be created as "global poll".
    /// </summary>
    public bool CreateGlobal
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Hide code name editing in simple mode
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            plcCodeName.Visible = false;
        }

        // Init the labels
        rfvCodeName.ErrorMessage = GetString("general.requirescodename");
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvQuestion.ErrorMessage = GetString("Polls_New.QuestionError");
        rfvMaxLength.ErrorMessage = GetString("general.errortexttoolong");

        lblTitle.Text = GetString("Polls_New.TitleLabel");
        lblQuestion.Text = GetString("Polls_New.QuestionLabel");

        // Set if it is live site
        txtDisplayName.IsLiveSite = txtTitle.IsLiveSite = txtQuestion.IsLiveSite = IsLiveSite;

        if (!RequestHelper.IsPostBack())
        {
            ClearForm();
        }
    }


    /// <summary>
    /// Resets all boxes.
    /// </summary>
    public override void ClearForm()
    {
        txtCodeName.Text = null;
        txtDisplayName.Text = null;
        txtTitle.Text = null;
        txtQuestion.Text = null;
    }


    /// <summary>
    /// Shows the specified error message.
    /// </summary>
    /// <param name="text">Error message text</param>
    public override void ShowError(string text)
    {
        error = true;
        base.ShowError(text);
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (CreateGlobal)
        {
            if (!CheckPermissions("cms.polls", PERMISSION_GLOBALMODIFY))
            {
                return;
            }
        }
        else
        {
            if (!CheckPermissions("cms.polls", PERMISSION_MODIFY))
            {
                return;
            }
        }

        // Generate code name in simple mode
        string codeName = txtCodeName.Text.Trim();
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            codeName = ValidationHelper.GetCodeName(txtDisplayName.Text.Trim(), null, null);
        }

        // Perform validation
        string errorMessage = new Validator().NotEmpty(codeName, rfvCodeName.ErrorMessage)
            .NotEmpty(txtDisplayName.Text, rfvDisplayName.ErrorMessage).NotEmpty(txtQuestion.Text.Trim(), rfvQuestion.ErrorMessage).Result;

        // Check CodeName for identifier format
        if (!ValidationHelper.IsCodeName(codeName))
        {
            errorMessage = GetString("General.ErrorCodeNameInIdentifierFormat");
        }

        if (string.IsNullOrEmpty(errorMessage))
        {
            bool isnew = false;

            // Create new 
            PollInfo pollObj = new PollInfo();
            pollObj.PollAllowMultipleAnswers = false;
            pollObj.PollAccess = SecurityAccessEnum.AllUsers;

            // Check if codename already exists on a group or is a global
            PollInfo pi = null;
            if (CreateGlobal)
            {
                pi = PollInfoProvider.GetPollInfo("." + codeName, 0);
                if ((pi != null) && (pi.PollSiteID <= 0))
                {
                    errorMessage = GetString("polls.codenameexists");
                }
            }
            else
            {
                pi = PollInfoProvider.GetPollInfo(codeName, SiteID, GroupID);
                if ((pi != null) && (pi.PollSiteID > 0))
                {
                    errorMessage = GetString("polls.codenameexists");
                }
            }

            if (string.IsNullOrEmpty(errorMessage))
            {
                // Set the fields
                pollObj.PollCodeName = codeName;
                pollObj.PollDisplayName = txtDisplayName.Text.Trim();
                pollObj.PollTitle = txtTitle.Text.Trim();
                pollObj.PollQuestion = txtQuestion.Text.Trim();
                pollObj.PollLogActivity = true;
                if (GroupID > 0)
                {
                    if (SiteID <= 0)
                    {
                        ShowError(GetString("polls.nositeid"));
                        return;
                    }

                    pollObj.PollGroupID = GroupID;
                    pollObj.PollSiteID = SiteID;
                }
                else
                {
                    // Assigned poll to particular site if it is not global poll
                    if (!CreateGlobal)
                    {
                        if (!PollInfoProvider.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Polls, VersionActionEnum.Insert))
                        {
                            LicenseError = GetString("LicenseVersion.Polls");
                        }
                        else
                        {
                            pollObj.PollSiteID = SiteID;
                        }
                    }
                }

                // Save the object
                PollInfoProvider.SetPollInfo(pollObj);
                ItemID = pollObj.PollID;
                isnew = true;

                // Add global poll to current site
                if ((CMSContext.CurrentSite != null) && CreateGlobal)
                {
                    if (PollInfoProvider.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Polls, VersionActionEnum.Insert))
                    {
                        if ((pollObj.PollGroupID == 0) && (pollObj.PollSiteID == 0))
                        {
                            // Bind only global polls to current site
                            PollInfoProvider.AddPollToSite(pollObj.PollID, CMSContext.CurrentSiteID);
                        }
                    }
                    else
                    {
                        LicenseError = GetString("LicenseVersion.Polls");
                    }
                }

                // Redirect to edit mode
                if ((isnew) && (!error))
                {
                    RaiseOnSaved();
                }
            }
            else
            {
                // Error message - code name already exists
                ShowError(GetString("polls.codenameexists"));
            }
        }
        
        if (!string.IsNullOrEmpty(errorMessage))
        {
            // Error message - validation
            ShowError(errorMessage);
        }
    }
}