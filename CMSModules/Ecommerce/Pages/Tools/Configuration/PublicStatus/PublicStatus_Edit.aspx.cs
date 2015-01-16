using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;

// Edited object
[EditedObject(PredefinedObjectType.PUBLICSTATUS, "publicStatusId")]
// Title
[Title("Objects/Ecommerce_PublicStatus/object.png", "PublicStatus_Edit.HeaderCaption", "newedit_public_status", ExistingObject = true)]
[Title("Objects/Ecommerce_PublicStatus/new.png", "PublicStatus_New.HeaderCaption", "newedit_public_status", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_PublicStatus_PublicStatus_Edit : CMSPublicStatusesPage
{
    #region "Variables"

    protected int mPublicStatusId = 0;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Field validator error messages initialization
        rfvDisplayName.ErrorMessage = GetString("publicstatus_edit.errorEmptyDisplayName");
        rfvCodeName.ErrorMessage = GetString("publicstatus_edit.errorEmptyCodeName");

        // Control initializations				
        lblPublicStatusName.Text = GetString("PublicStatus_Edit.PublicStatusNameLabel");
        lblPublicStatusDisplayName.Text = GetString("PublicStatus_Edit.PublicStatusDisplayNameLabel");

        string currentPublicStatus = GetString("PublicStatus_Edit.NewItemCaption");

        // Get publicStatus id from querystring		
        mPublicStatusId = QueryHelper.GetInteger("publicStatusId", 0);
        if (mPublicStatusId > 0)
        {
            PublicStatusInfo publicStatusObj = EditedObject as PublicStatusInfo;

            if (publicStatusObj != null)
            {
                CheckEditedObjectSiteID(publicStatusObj.PublicStatusSiteID);
                currentPublicStatus = ResHelper.LocalizeString(publicStatusObj.PublicStatusDisplayName);

                // Fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    LoadData(publicStatusObj);

                    // Show that the publicStatus was created or updated successfully
                    if (QueryHelper.GetString("saved", "") == "1")
                    {
                        // Show message
                        ShowChangesSaved();
                    }
                }
            }
        }

        // Initializes page title breadcrumbs control		
        string[,] pageTitleTabs = new string[2, 3];
        pageTitleTabs[0, 0] = GetString("PublicStatus_Edit.ItemListLink");
        pageTitleTabs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/PublicStatus/PublicStatus_List.aspx?siteId=" + SiteID;
        pageTitleTabs[0, 2] = "";
        pageTitleTabs[1, 0] = currentPublicStatus;
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
    }


    /// <summary>
    /// Load data of editing publicStatus.
    /// </summary>
    /// <param name="publicStatusObj">PublicStatus object</param>
    protected void LoadData(PublicStatusInfo publicStatusObj)
    {
        txtPublicStatusName.Text = publicStatusObj.PublicStatusName;
        chkPublicStatusEnabled.Checked = publicStatusObj.PublicStatusEnabled;
        txtPublicStatusDisplayName.Text = publicStatusObj.PublicStatusDisplayName;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check permissions
        CheckConfigurationModification();

        // Check input values from textboxes and other controls
        string errorMessage = new Validator()
            .NotEmpty(txtPublicStatusDisplayName.Text.Trim(), GetString("publicstatus_edit.errorEmptyDisplayName"))
            .NotEmpty(txtPublicStatusName.Text.Trim(), GetString("publicstatus_edit.errorEmptyCodeName")).Result;

        if (!ValidationHelper.IsCodeName(txtPublicStatusName.Text.Trim()))
        {
            errorMessage = GetString("General.ErrorCodeNameInIdentifierFormat");
        }

        if (errorMessage == "")
        {
            // Check unique name for configured site
            PublicStatusInfo publicStatusObj = PublicStatusInfoProvider.GetPublicStatusInfo(txtPublicStatusName.Text.Trim(), SiteInfoProvider.GetSiteName(ConfiguredSiteID));

            if ((publicStatusObj == null) || (publicStatusObj.PublicStatusID == mPublicStatusId))
            {
                // Get object
                if (publicStatusObj == null)
                {
                    publicStatusObj = PublicStatusInfoProvider.GetPublicStatusInfo(mPublicStatusId);
                    if (publicStatusObj == null)
                    {
                        publicStatusObj = new PublicStatusInfo();
                        publicStatusObj.PublicStatusSiteID = ConfiguredSiteID;
                    }
                }

                publicStatusObj.PublicStatusID = mPublicStatusId;
                publicStatusObj.PublicStatusName = txtPublicStatusName.Text.Trim();
                publicStatusObj.PublicStatusEnabled = chkPublicStatusEnabled.Checked;
                publicStatusObj.PublicStatusDisplayName = txtPublicStatusDisplayName.Text.Trim();

                PublicStatusInfoProvider.SetPublicStatusInfo(publicStatusObj);

                URLHelper.Redirect("PublicStatus_Edit.aspx?publicStatusId=" + publicStatusObj.PublicStatusID + "&saved=1&siteId=" + SiteID);
            }
            else
            {
                // Show error message
                ShowError(GetString("PublicStatus_Edit.PublicStatusNameExists"));
            }
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }
}