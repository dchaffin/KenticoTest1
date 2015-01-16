using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;

public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Account_Detail : CMSContactManagementAccountsPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        frameContent.Attributes.Add("src", "../Account/Frameset.aspx" + URLHelper.Url.Query + "&dialogmode=1");
        frameFooter.Attributes.Add("src", "../DetailFooter.aspx" + URLHelper.Url.Query);
    }

    #endregion
}