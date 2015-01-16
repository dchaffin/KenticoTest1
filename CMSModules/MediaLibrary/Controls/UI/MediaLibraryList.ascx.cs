using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.MediaLibrary;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_MediaLibrary_Controls_UI_MediaLibraryList : CMSAdminListControl
{
    #region "Private variables"

    private int mGroupId = 0;

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
    /// Indicates if control is used on a live site
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
    /// Gets or sets group ID.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupId;
        }
        set
        {
            mGroupId = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);
        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.WhereCondition = GetWhereCondition();
        gridElem.GroupObject = (GroupID > 0);
        gridElem.ZeroRowsText = GetString("general.nodatafound");
    }


    private void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                SelectedItemID = ValidationHelper.GetInteger(actionArgument, 0);
                RaiseOnEdit();
                break;

            case "delete":
                MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(ValidationHelper.GetInteger(actionArgument, 0));
                // Check 'Manage' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, PERMISSION_MANAGE))
                {
                    ShowError(MediaLibraryHelper.GetAccessDeniedMessage(PERMISSION_MANAGE));
                    return;
                }
                try
                {
                    MediaLibraryInfoProvider.DeleteMediaLibraryInfo(ValidationHelper.GetInteger(actionArgument, 0));
                }
                catch (Exception ex)
                {
                    EventLogProvider eventLog = new EventLogProvider();
                    eventLog.LogEvent("Media library", "DELETEOBJ", ex, CMSContext.CurrentSiteID);

                    ShowError(ex.Message, EventLogProvider.GetExceptionLogMessage(ex), null);
                }
                break;
        }

        RaiseOnAction(actionName, actionArgument);
    }


    /// <summary>
    /// Returns proper where condition.
    /// </summary>
    private string GetWhereCondition()
    {
        // Filter by current site
        string whereCond = null;
        if (CMSContext.CurrentSite != null)
        {
            whereCond = "LibrarySiteID=" + CMSContext.CurrentSite.SiteID;
        }

        // Filter by group id if specified
        if (GroupID != 0)
        {
            if (whereCond != null)
            {
                whereCond += " AND ";
            }
            else
            {
                whereCond = String.Empty;
            }

            whereCond += "LibraryGroupID=" + GroupID;
        }
        else
        {
            whereCond += " AND (LibraryGroupID IS NULL OR LibraryGroupID = 0)";
        }
        return whereCond;
    }

    #endregion
}