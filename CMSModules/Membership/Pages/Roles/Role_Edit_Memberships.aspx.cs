using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Roles_Role_Edit_Memberships : CMSRolesPage
{
    #region "Variables"

    private string currentValues = String.Empty;
    private int mRoleID = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current role ID.
    /// </summary>
    private int RoleID
    {
        get
        {
            if (mRoleID == 0)
            {
                mRoleID = QueryHelper.GetInteger("roleid", 0);
            }

            return mRoleID;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentUserInfo user = CMSContext.CurrentUser;
        // Check UI profile for membership
        if (!user.IsAuthorizedPerUIElement("CMS.Administration", "Membership"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Administration", "Membership");
        }

        // Check "read" permission
        if (!user.IsAuthorizedPerResource("CMS.Membership", "Read"))
        {
            RedirectToAccessDenied("CMS.Membership", "Read");
        }

        DataSet ds = MembershipRoleInfoProvider.GetMembershipRoles("RoleID = " + RoleID, String.Empty);

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            currentValues = TextHelper.Join(";", SystemDataHelper.GetStringValues(ds.Tables[0], "MembershipID"));
        }

        if (!RequestHelper.IsPostBack())
        {
            // Set values
            usMemberships.Value = currentValues;
        }

        // Init uni selector
        usMemberships.ReturnColumnName = "MembershipID";
        usMemberships.DynamicColumnName = true;
        usMemberships.OnSelectionChanged += new EventHandler(usMemberships_OnSelectionChanged);

        string siteWhere = (SiteID <= 0) ? "MembershipSiteID IS NULL" : "MembershipSiteID =" + SiteID;
        usMemberships.ListingWhereCondition = SqlHelperClass.AddWhereCondition(usMemberships.ListingWhereCondition, siteWhere);
        usMemberships.WhereCondition = SqlHelperClass.AddWhereCondition(usMemberships.WhereCondition, siteWhere);
    }


    protected void usMemberships_OnSelectionChanged(object sender, EventArgs ea)
    {
        SaveData();
    }


    /// <summary>
    /// Store selected (unselected) memberships.
    /// </summary>
    private void SaveData()
    {
        bool saved = false;

        // Check permission for manage membership 
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Membership", "Modify"))
        {
            RedirectToAccessDenied("CMS.Membership", "Modify");
        }

        // Remove old items
        string newValues = ValidationHelper.GetString(usMemberships.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                foreach (string item in newItems)
                {
                    int membershipID = ValidationHelper.GetInteger(item, 0);
                    MembershipRoleInfoProvider.RemoveMembershipFromRole(membershipID, RoleID);
                    MembershipInfoProvider.InvalidateMembershipUsers(membershipID);

                    saved = true;
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to membership
                foreach (string item in newItems)
                {
                    int membershipID = ValidationHelper.GetInteger(item, 0);
                    MembershipRoleInfoProvider.AddMembershipToRole(membershipID, RoleID);
                    MembershipInfoProvider.InvalidateMembershipUsers(membershipID);

                    saved = true;
                }
            }
        }

        if (saved)
        {
            usMemberships.Reload(true);
            ShowChangesSaved();
        }
    }

    #endregion
}