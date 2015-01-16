using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_TagGroups_Controls_TagEdit : CMSUserControl
{
    #region "Private fields"

    private int mGroupID = 0;
    private int mSiteID = 0;
    private bool mIsEdit = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether the control is used to edit existing group.
    /// </summary>
    public bool IsEdit
    {
        get
        {
            return mIsEdit;
        }
        set
        {
            mIsEdit = value;
        }
    }


    /// <summary>
    /// Indicates whether the previous tag group was saved.
    /// </summary>
    public bool WasSaved
    {
        get
        {
            if (QueryHelper.GetInteger("saved", 0) == 1)
            {
                return true;
            }
            return false;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// ID of the currently processed tag group.
    /// </summary>
    public int GroupID
    {
        get
        {
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
    /// ID of site of the currently processed tag group.
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

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize control
        SetupControl();
    }


    #region "Event handlers"

    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Validate form entries
        string errorMessage = ValidateForm();

        if (errorMessage == "")
        {
            TagGroupInfo tgi = null;

            try
            {
                // Edit existing tag group
                if (GroupID > 0)
                {
                    tgi = TagGroupInfoProvider.GetTagGroupInfo(GroupID);
                }
                else
                {
                    tgi = new TagGroupInfo();
                }

                // Update tag group info fields                
                tgi.TagGroupDisplayName = txtDisplayName.Text;
                tgi.TagGroupName = txtCodeName.Text;
                tgi.TagGroupDescription = txtDescription.Text;
                tgi.TagGroupSiteID = SiteID;

                // If the new tag group is created set the default value for TagGroupIsAdHoc property 
                if (!IsEdit)
                {
                    tgi.TagGroupIsAdHoc = false;
                }

                // Update tag group info
                TagGroupInfoProvider.SetTagGroupInfo(tgi);

                // Redirect to edit page once the new tag group is created
                if (!IsEdit)
                {
                    string editUrl = "~/CMSModules/TagGroups/Pages/Development/TagGroup_Edit.aspx?groupid=" + tgi.TagGroupID.ToString() + "&siteid=" + SiteID.ToString() + "&saved=1";
                    URLHelper.Redirect(editUrl);
                }

                // Show message
                ShowChangesSaved();

                // Refresh header
                ScriptHelper.RefreshTabHeader(Page, "general");
            }
            catch (Exception ex)
            {
                // Show error message
                ShowError(GetString("general.erroroccurred") + " " + ex.Message);
            }
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControl()
    {
        // Set the error messages
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvCodeName.ErrorMessage = GetString("general.requirescodename");

        // Handle existing tag group editing
        if (GroupID > 0)
        {
            if (!RequestHelper.IsPostBack())
            {
                HandleExisting();
            }
        }

        if (!RequestHelper.IsPostBack())
        {
            if (WasSaved)
            {
                // Show message
                ShowChangesSaved();
            }
        }
    }


    /// <summary>
    /// Handles pre-actions required when editing existing tag group.
    /// </summary>
    private void HandleExisting()
    {
        // Get info on existing tag group
        TagGroupInfo tgi = TagGroupInfoProvider.GetTagGroupInfo(GroupID);

        // Prefill the fields with obtained data
        txtDisplayName.Text = tgi.TagGroupDisplayName;
        txtCodeName.Text = tgi.TagGroupName;
        txtDescription.Text = tgi.TagGroupDescription;
    }


    /// <summary>
    /// Validates required entries.
    /// </summary>    
    /// <param name="newGroupName">Group name to check for unique</param>
    /// <param name="siteId">ID of the site tag group should be inserted</param>
    /// <returns>True if all the necessary data were entered, otherwise false is returned</returns>
    private bool CodeNameIsUnique(string newGroupName, int siteId)
    {
        // Gheck if the tag group already exist
        TagGroupInfo tagGroup = TagGroupInfoProvider.GetTagGroupInfo(newGroupName, siteId);
        if (tagGroup == null)
        {
            return true;
        }
        else
        {
            // If the existing tag group is updated the code name already exist
            if ((GroupID > 0) && (GroupID == tagGroup.TagGroupID))
            {
                return true;
            }

            return false;
        }
    }


    /// <summary>
    /// Validates the form entries.
    /// </summary>
    /// <returns>Empty string if validation passed otherwise error message is returned</returns>
    private string ValidateForm()
    {
        string errorMessage = string.Empty;
        // Trim all textboxes
        txtDisplayName.Text = txtDisplayName.Text.Trim();
        txtCodeName.Text = txtCodeName.Text.Trim();
        txtDescription.Text = txtDescription.Text.Trim();

        errorMessage = new Validator().IsCodeName(txtCodeName.Text, GetString("general.invalidcodename")).Result;

        if (string.IsNullOrEmpty(errorMessage))
        {
            rfvCodeName.Validate();
            rfvDisplayName.Validate();

            // If all the required fields entries were supplied
            if (rfvCodeName.IsValid & rfvDisplayName.IsValid)
            {
                // Check if the given code name is unique
                if (!CodeNameIsUnique(txtCodeName.Text, SiteID))
                {
                    errorMessage = GetString("general.codenameexists");

                    // Show error message
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

    #endregion
}