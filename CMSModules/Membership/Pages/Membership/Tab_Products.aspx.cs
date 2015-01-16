using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Membership_Tab_Products : CMSMembershipPage
{
    private int membershipID;


    protected void Page_Load(object sender, EventArgs e)
    {
        membershipID = QueryHelper.GetInteger("MembershipID", 0);
        MembershipInfo mi = MembershipInfoProvider.GetMembershipInfo(membershipID);

        // Test security
        CheckMembershipPermissions(mi);
    }
}