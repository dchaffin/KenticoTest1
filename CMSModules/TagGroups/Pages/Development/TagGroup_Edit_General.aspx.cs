using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_TagGroups_Pages_Development_TagGroup_Edit_General : SiteManagerPage
{
    #region "Private fields"

    private int mGroupID = 0;
    private string mSiteId = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// ID of the currently processed tag group.
    /// </summary>
    public int GroupID
    {
        get
        {
            if (mGroupID > 0)
            {
                return mGroupID;
            }

            return QueryHelper.GetInteger("groupId", 0);
        }
        set
        {
            mGroupID = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the control
        SetupControl();
    }


    #region "Private methods"

    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControl()
    {
        // Process site ID parameter if supplied
        mSiteId = QueryHelper.GetString("siteid", "");

        // Set the edit control
        groupEdit.GroupID = GroupID;
        groupEdit.SiteID = int.Parse(mSiteId);
    }

    #endregion
}