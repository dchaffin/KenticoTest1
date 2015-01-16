using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Forums;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Forums_Controls_Forums_ForumGroupList : CMSAdminListControl
{
    #region "Variables"

    private string mWhereCondition = String.Empty;
    private int mCommunityGroupId = 0;
    private string mTitleElemUrl = String.Empty;
    private bool process = true;
    private bool mIsGroupList = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets ID of current group.
    /// </summary>
    public int CommunityGroupId
    {
        get
        {
            return mCommunityGroupId;
        }
        set
        {
            ucPostApprove.CommunityGroupId = value;
            mCommunityGroupId = value;
        }
    }


    /// <summary>
    /// Additional WHERE condition to filter data.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mWhereCondition;
        }
        set
        {
            ucPostApprove.WhereCondition = value;
            mWhereCondition = value;
        }
    }


    /// <summary>
    /// URL of the approve title image.
    /// </summary>
    public string TitleElemUrl
    {
        get
        {
            return mTitleElemUrl;
        }
        set
        {
            mTitleElemUrl = value;
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
            ucPostApprove.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets whether list is showed in group UI.
    /// </summary>
    public bool IsGroupList
    {
        get
        {
            return mIsGroupList;
        }
        set
        {
            mIsGroupList = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ucPostApprove.HideControlForNoData = true;
        ucPostApprove.IsLiveSite = IsLiveSite;
        process = true;
        // If control is not visible don't process anything       
        if (!Visible || StopProcessing)
        {
            process = false;
            EnableViewState = false;
            return;
        }

        // Group where condition part
        string groupWhere = "GroupSiteID = " + CMSContext.CurrentSite.SiteID;

        if (CommunityGroupId > 0 || IsGroupList)
        {
            gridGroups.GroupObject = true;
            groupWhere = SqlHelperClass.AddWhereCondition(groupWhere, "GroupGroupID= " + CommunityGroupId);
        }

        // Add where condition from property
        if (WhereCondition != String.Empty)
        {
            groupWhere += " AND " + WhereCondition;
        }

        // Check 'Read' permission
        if (RaiseOnCheckPermissions(PERMISSION_READ, this))
        {
            if (StopProcessing)
            {
                pnlGroups.Visible = false;
            }
        }
        else
        {
            // Check the 'Read' permission for default resource
            if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.forums", PERMISSION_READ))
            {
                AccessDenied("cms.forums", PERMISSION_READ);
            }
        }

        titleElem.TitleText = GetString("Forums.WaitingApprove");
        titleElem.TitleImage = (!String.IsNullOrEmpty(TitleElemUrl)) ? TitleElemUrl : GetImageUrl("CMSModules/CMS_Forums/waitingforapproval.png");
        titleElem.SetWindowTitle = false;

        gridGroups.WhereCondition = groupWhere;
        gridGroups.IsLiveSite = IsLiveSite;
        gridGroups.OnAction += new OnActionEventHandler(gridGroups_OnAction);
        gridGroups.GridView.AllowSorting = false;
        gridGroups.ZeroRowsText = GetString("general.nodatafound");
    }


    /// <summary>
    /// Reloads the grid data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        gridGroups.ReloadData();
        ucPostApprove.ReloadData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!IsLiveSite && process)
        {
            ReloadData();
        }

        if ((DataHelper.DataSourceIsEmpty(ucPostApprove.DataSource)) || (!ucPostApprove.Visible))
        {
            pnlApprove.Visible = false;
        }

        base.OnPreRender(e);
    }


    #region "UniGrid events"

    /// <summary>
    /// Forums unigird on action event.
    /// </summary>
    protected void gridGroups_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
            case "up":
            case "down":
                if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
                {
                    return;
                }
                break;
        }

        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                ForumGroupInfoProvider.DeleteForumGroupInfo(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "up":
                ForumGroupInfoProvider.MoveGroupUp(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "down":
                ForumGroupInfoProvider.MoveGroupDown(ValidationHelper.GetInteger(actionArgument, 0));
                break;
        }

        RaiseOnAction(actionName, actionArgument);
    }

    #endregion
}