using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;

public partial class CMSFormControls_Sites_SelectSiteWithAllForAdmin : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets selected items.
    /// </summary>
    public override object Value
    {
        get
        {
            return ucSiteSelector.Value;
        }
        set
        {
            ucSiteSelector.Value = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (CMSContext.CurrentUser.IsGlobalAdministrator)
        {
            ucSiteSelector.AllowAll = true;
        }
        else
        {
            //Show only sites assignet to user
            ucSiteSelector.UserId = CMSContext.CurrentUser.UserID;
            ucSiteSelector.AllowAll = false;
        }

        ucSiteSelector.AllowSetSpecialFields = true;
        ucSiteSelector.UniSelector.SpecialFields = new string[1,2] { { GetString("dialogs.config.currentsite"), "##currentsite##" } };

        if (HasDependingFields)
        {
            ucSiteSelector.DropDownSingleSelect.AutoPostBack = true;
        }
    }
}