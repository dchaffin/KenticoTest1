using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Configuration.Departments.Users")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Departments_Department_Users : CMSDepartmentsPage
{
    protected int mDepartmentId = 0;
    protected string mCurrentValues = string.Empty;
    protected DepartmentInfo mDepartmentInfoObj = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        lblAvialable.Text = GetString("com.department.usersavailable");
        mDepartmentId = QueryHelper.GetInteger("departmentid", 0);
        if (mDepartmentId > 0)
        {
            mDepartmentInfoObj = DepartmentInfoProvider.GetDepartmentInfo(mDepartmentId);
            EditedObject = mDepartmentInfoObj;

            if (mDepartmentInfoObj != null)
            {
                CheckEditedObjectSiteID(mDepartmentInfoObj.DepartmentSiteID);

                // Get the active users
                string where = "UserID IN (SELECT UserID FROM COM_UserDepartment WHERE DepartmentID = " + mDepartmentId + ")";
                DataSet ds = UserInfoProvider.GetUsers(where, null, 0, "UserID");
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    mCurrentValues = TextHelper.Join(";", SystemDataHelper.GetStringValues(ds.Tables[0], "UserID"));
                }

                if (!RequestHelper.IsPostBack())
                {
                    uniSelector.Value = mCurrentValues;
                }
            }
        }

        uniSelector.IconPath = GetObjectIconUrl("cms.user", "object.png");
        uniSelector.OnSelectionChanged += uniSelector_OnSelectionChanged;
        uniSelector.WhereCondition = GetWhereCondition();
    }


    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveItems();
    }


    protected void SaveItems()
    {
        if (mDepartmentInfoObj == null)
        {
            return;
        }

        // Check permissions
        CheckConfigurationModification(mDepartmentInfoObj.DepartmentSiteID);

        // Remove old items
        string newValues = ValidationHelper.GetString(uniSelector.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, mCurrentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to user
                foreach (string item in newItems)
                {
                    int userId = ValidationHelper.GetInteger(item, 0);
                    UserDepartmentInfoProvider.RemoveUserFromDepartment(mDepartmentId, userId);
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(mCurrentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to user
                foreach (string item in newItems)
                {
                    int userId = ValidationHelper.GetInteger(item, 0);
                    UserDepartmentInfoProvider.AddUserToDepartment(mDepartmentId, userId);
                }
            }
        }

        // Show message
        ShowChangesSaved();
    }


    /// <summary>
    /// Creates where conditin for user selector.
    /// </summary>
    protected string GetWhereCondition()
    {
        string where = "UserID IN (SELECT UserID FROM CMS_UserSite WHERE SiteID = " + CMSContext.CurrentSiteID + ")";

        where = SqlHelperClass.AddWhereCondition(where, UserInfoProvider.USER_ENABLED_WHERE_CONDITION);

        // Include selected values
        if (!string.IsNullOrEmpty(mCurrentValues))
        {
            string[] usersIds = mCurrentValues.Split(';');
            int[] intUsersIds = ValidationHelper.GetIntegers(usersIds, 0);

            where = SqlHelperClass.AddWhereCondition(where, SqlHelperClass.GetWhereCondition("UserID", intUsersIds), "OR");
        }

        return where;
    }
}