using System;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.Polls;
using CMS.SiteProvider;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Polls_Controls_AnswerEdit : CMSAdminEditControl
{
    #region "Variables"

    private PollAnswerInfo pollAnswerObj = null;
    private FormEngineUserControl bizFormElem = null;

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
    /// Gets or sets the answer ID.
    /// </summary>
    public int PollId
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["pollid"], 0);
        }
        set
        {
            ViewState["pollid"] = (object)value;
        }
    }


    /// <summary>
    /// Gets or sets saved property.
    /// </summary>
    public bool Saved
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["saved"], false);
        }
        set
        {
            ViewState["saved"] = (object)value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Required field validator error messages initialization
        rfvAnswerText.ErrorMessage = GetString("Polls_Answer_Edit.AnswerTextError");

        // Controls initializations				
        lblAnswerText.Text = GetString("Polls_Answer_Edit.AnswerTextLabel");
        lblVotes.Text = GetString("Polls_Answer_Edit.Votes");

        // Set if it is live site
        txtAnswerText.IsLiveSite = IsLiveSite;

        // Load BizForm selector if BizForms module is available
        if (ModuleEntry.IsModuleLoaded(ModuleEntry.BIZFORM) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleEntry.BIZFORM, CMSContext.CurrentSiteName))
        {
            bizFormElem = (FormEngineUserControl)Page.LoadUserControl("~/CMSModules/BizForms/FormControls/SelectBizForm.ascx");
            bizFormElem.ShortID = "bizFormElem";
            bizFormElem.SetValue("ShowSiteFilter", false);
            plcBizFormSelector.Controls.Add(bizFormElem);
            bizFormElem.Visible = true;

            UniSelector uniSelector = bizFormElem.GetValue("UniSelector") as UniSelector;
            if (uniSelector != null)
            {
                uniSelector.OnSelectionChanged += new EventHandler(BizFormSelector_OnSelectionChanged);
            }
        }

        if (!RequestHelper.IsPostBack() && !IsLiveSite)
        {
            LoadData();
        }
    }


    /// <summary>
    /// Loads new data for this control.
    /// </summary>
    public void LoadData()
    {
        // If working with existing record
        if (ItemID > 0)
        {
            pollAnswerObj = PollAnswerInfoProvider.GetPollAnswerInfo(ItemID);
            EditedObject = pollAnswerObj;

            if (pollAnswerObj != null)
            {
                // Fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    ReloadData();
                }

                // When saved, display info message
                if (Saved)
                {
                    ShowChangesSaved();
                    Saved = false;
                }

                PollId = pollAnswerObj.AnswerPollID;
            }
        }
        // If creating new record
        else
        {
            plcVotes.Visible = false;
            txtVotes.Text = "0";
        }
    }


    /// <summary>
    /// Clears data.
    /// </summary>
    public override void ClearForm()
    {
        base.ClearForm();
        txtAnswerText.Text = null;
        txtVotes.Text = null;
    }


    /// <summary>
    /// Reloads answer data.
    /// </summary>
    public override void ReloadData()
    {
        ClearForm();
        if (pollAnswerObj == null)
        {
            pollAnswerObj = PollAnswerInfoProvider.GetPollAnswerInfo(ItemID);
        }

        if (pollAnswerObj != null)
        {
            // Load the fields
            txtAnswerText.Text = pollAnswerObj.AnswerText;
            chkAnswerEnabled.Checked = pollAnswerObj.AnswerEnabled;
            txtVotes.Text = pollAnswerObj.AnswerCount.ToString();
            pnlGeneral.GroupingText = GetString("general.general");
            plcVotes.Visible = true;

            // Check if bizform module is available (for open-ended answers).
            if (ModuleEntry.IsModuleLoaded(ModuleEntry.BIZFORM) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleEntry.BIZFORM, CMSContext.CurrentSiteName))
            {
                // Show open-ended answer settings only for site poll
                PollInfo pi = PollInfoProvider.GetPollInfo(pollAnswerObj.AnswerPollID);
                plcOpenAnswer.Visible = (pi != null) && (pi.PollSiteID > 0) && (pi.PollGroupID == 0);

                chkAnswerIsOpenEnded.Checked = plcOpenAnswerSettings.Visible = pollAnswerObj.AnswerIsOpenEnded;
                bizFormElem.Value = pollAnswerObj.AnswerForm;
                alternativeFormElem.ClassName = "BizForm." + bizFormElem.Text;
                alternativeFormElem.Value = pollAnswerObj.AnswerAlternativeForm;
                chkAnswerHideForm.Checked = pollAnswerObj.AnswerHideForm;
                pnlAnswerForm.GroupingText = GetString("polls.answerformsettings");
            }
        }
        else
        {
            txtAnswerText.Text = String.Empty;
            plcVotes.Visible = false;
            pnlGeneral.GroupingText = string.Empty;
        }
    }

    
    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Load current answer object
        if (pollAnswerObj == null)
        {
            pollAnswerObj = PollAnswerInfoProvider.GetPollAnswerInfo(ItemID);
        }
        // Check permission for answer object (global/site poll)
        if (!CheckModifyPermission(PollId))
        {
            return;
        }

        string errorMessage = null;
        // Validate the input
        if (txtVotes.Visible)
        {
            errorMessage = new Validator().NotEmpty(txtAnswerText.Text, rfvAnswerText.ErrorMessage)
                .IsPositiveNumber(txtVotes.Text, GetString("Polls_Answer_Edit.VotesNotNumber"), true)
                .IsInteger(txtVotes.Text, GetString("Polls_Answer_Edit.VotesNotNumber")).Result;
        }
        else
        {
            errorMessage = new Validator().NotEmpty(txtAnswerText.Text, rfvAnswerText.ErrorMessage).Result;
        }

        if (String.IsNullOrEmpty(errorMessage))
        {
            // If pollAnswer doesn't already exist, create new one
            if (pollAnswerObj == null)
            {
                pollAnswerObj = new PollAnswerInfo();
                pollAnswerObj.AnswerOrder = PollAnswerInfoProvider.GetLastAnswerOrder(PollId) + 1;
                pollAnswerObj.AnswerCount = 0;
                pollAnswerObj.AnswerPollID = PollId;
            }

            // Set the fields
            pollAnswerObj.AnswerEnabled = chkAnswerEnabled.Checked;
            pollAnswerObj.AnswerText = txtAnswerText.Text.Trim();
            pollAnswerObj.AnswerCount = ValidationHelper.GetInteger(txtVotes.Text, 0);

            if (plcOpenAnswer.Visible)
            {
                string answerForm = ValidationHelper.GetString(bizFormElem.Value, string.Empty);
                if (chkAnswerIsOpenEnded.Checked && string.IsNullOrEmpty(answerForm))
                {
                    ShowError(GetString("Polls_Answer_Edit.SelectForm"));
                    return;
                }
                pollAnswerObj.AnswerForm = answerForm;
                pollAnswerObj.AnswerAlternativeForm = ValidationHelper.GetString(alternativeFormElem.Value, string.Empty);
                pollAnswerObj.AnswerHideForm = chkAnswerHideForm.Checked;
            }

            // Save the data
            PollAnswerInfoProvider.SetPollAnswerInfo(pollAnswerObj);
            Saved = true;
            ItemID = pollAnswerObj.AnswerID;

            // Raise event;
            RaiseOnSaved();
        }
        else
        {
            // Error message - Validation
            ShowError(errorMessage);
        }
    }


    /// <summary>
    /// Checks modify permission. Returns false if checking failed.
    /// </summary>
    /// <param name="pollId">Poll ID</param>
    private bool CheckModifyPermission(int pollId)
    {
        // Get parent of answer object and see if it is global poll or site poll
        PollInfo pi = null;
        if (pollId > 0) // non-zero value when creating new poll
        {
            pi = PollInfoProvider.GetPollInfo(pollId);
        }
        else if (pollAnswerObj != null) // not null when modifying existing answer
        {
            pi = PollInfoProvider.GetPollInfo(pollAnswerObj.AnswerPollID);
        }
        if (pi != null)
        {
            return (pi.PollSiteID > 0) && CheckPermissions("cms.polls", PERMISSION_MODIFY) ||
                   (pi.PollSiteID <= 0) && CheckPermissions("cms.polls", PERMISSION_GLOBALMODIFY);
        }
        return false;
    }


    /// <summary>
    /// Show/hide open-ended answer settings
    /// <summary>
    protected void chkAnswerIsOpenEnded_CheckedChanged(object sender, EventArgs e)
    {
        plcOpenAnswerSettings.Visible = chkAnswerIsOpenEnded.Checked;
        if (!chkAnswerIsOpenEnded.Checked)
        {
            bizFormElem.Value = string.Empty;
            alternativeFormElem.Value = string.Empty;
        }
    }

    /// <summary>
    /// Fired when BizForm is selected
    /// <summary>
    protected void BizFormSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        alternativeFormElem.ClassName = "BizForm." + bizFormElem.Text;
    }
}