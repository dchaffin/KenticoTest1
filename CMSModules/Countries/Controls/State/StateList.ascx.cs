using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

public partial class CMSModules_Countries_Controls_State_StateList : CMSAdminEditControl
{
    #region "Properties"

    /// <summary>
    /// UniGrid control used for listing.
    /// </summary>
    public UniGrid UniGridControl
    {
        get
        {
            return gridStates;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    #endregion
}