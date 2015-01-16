using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.UIControls;

public partial class CMSModules_Newsletters_CMSPages_GetNewsletterIssue : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check read permission for newsletters
        if (CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Newsletter", "Read"))
        {
            IssueInfo iss = null;
            int issueId = QueryHelper.GetInteger("IssueId", 0);
            if (issueId > 0)
            {
                // Get newsletter issue by ID
                iss = IssueInfoProvider.GetIssueInfo(issueId);
            }
            else
            {
                // Get newsletter issue by GUID and site ID
                Guid issueGuid = QueryHelper.GetGuid("IssueGUID", Guid.Empty);
                iss = IssueInfoProvider.GetIssueInfo(issueGuid, CMSContext.CurrentSiteID);
            }

            if ((iss != null) && (iss.IssueSiteID == CMSContext.CurrentSiteID))
            {
                // Get newsletter
                NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(iss.IssueNewsletterID);

                Response.Clear();
                Response.Write(IssueInfoProvider.GetEmailBody(iss, news, null, null, false, CMSContext.CurrentSiteName, null, null, null));
                Response.Flush();

                RequestHelper.EndResponse();
            }
        }
    }
}