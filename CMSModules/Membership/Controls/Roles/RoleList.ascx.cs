using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Membership_Controls_Roles_RoleList : CMSAdminListControl
{
    #region "Variables"

    private int mSiteId = 0;
    private int mGroupId = 0;
    private int mGlobalRecordValue = UniSelector.US_GLOBAL_RECORD;
    private bool mIsGroupList = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the group ID for which the roles should be displayed (0 means all groups).
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
            gridElem.WhereCondition = CreateWhereCondition();
        }
    }


    /// <summary>
    /// Gets or sets global record value (value for global item selected in drop down).
    /// </summary>
    public int GlobalRecordValue
    {
        get
        {
            return mGlobalRecordValue;
        }
        set
        {
            mGlobalRecordValue = value;
            gridElem.WhereCondition = CreateWhereCondition();
        }
    }


    /// <summary>
    /// Gets or sets the site ID for which the roles should be displayed.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
            gridElem.WhereCondition = CreateWhereCondition();
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


    /// <summary>
    /// Creates where condition for unigrid according to the parameters.
    /// </summary>
    private string CreateWhereCondition()
    {
        string where = "";

        if (mSiteId > 0)
        {
            where += "(SiteID = " + mSiteId + ")";
        }
        else
            // Global selected
            if ((mSiteId == GlobalRecordValue) && CMSContext.CurrentUser.UserSiteManagerAdmin)
            {
                where += "(SiteID IS NULL)";
            }
            else
            {
                where += "(SiteID =" + CMSContext.CurrentSiteID + ")";
            }

        if (!string.IsNullOrEmpty(where))
        {
            where += " AND ";
        }
        if (mGroupId > 0 || IsGroupList)
        {
            where += "(RoleGroupID = " + mGroupId + ")";
        }
        else
        {
            where += "(RoleGroupID IS NULL)";
        }

        return where;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        if (StopProcessing)
        {
            return;
        }

        // Unigrid
        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.WhereCondition = CreateWhereCondition();
        gridElem.ZeroRowsText = GetString("general.nodatafound");
        gridElem.GroupObject = (mGroupId > 0);
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            SelectedItemID = Convert.ToInt32(actionArgument);
            RaiseOnEdit();
        }
        else if (actionName == "delete")
        {
            if (!CheckPermissions("CMS.Roles", PERMISSION_MODIFY))
            {
                return;
            }

            RoleInfoProvider.DeleteRoleInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
        RaiseOnAction(actionName, actionArgument);
    }
}