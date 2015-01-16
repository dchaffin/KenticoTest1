using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Pages_Tools_Reports_Dashboard : DashboardPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        ucDashboard.ResourceName = "CMS.Ecommerce";
        ucDashboard.ElementName = "ReportsDashboard";
        ucDashboard.PortalPageInstance = this as PortalPage;
        ucDashboard.TagsLiteral = ltlTags;
        ucDashboard.DashboardSiteName = CMSContext.CurrentSiteName;

        ucDashboard.SetupDashboard();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentUserInfo cu = CMSContext.CurrentUser;

        // Check permissions
        if ((cu == null) || (!cu.IsAuthorizedPerResource("CMS.Ecommerce", "EcommerceRead") &&
            !cu.IsAuthorizedPerResource("CMS.Ecommerce", "ReadReports")))
        {
            CMSMyDeskPage.RedirectToCMSDeskAccessDenied("CMS.Ecommerce", "EcommerceRead OR ReadReports");
        }

        // Check UIProfile
        if (!cu.IsAuthorizedPerUIElement("CMS.Desk", "Ecommerce"))
        {
            CMSMyDeskPage.RedirectToCMSDeskUIElementAccessDenied("CMS.Desk", "Ecommerce");
        }

        if (!cu.IsAuthorizedPerUIElement("CMS.Ecommerce", "ReportsGroup"))
        {
            CMSMyDeskPage.RedirectToCMSDeskUIElementAccessDenied("CMS.Ecommerce", "ReportsGroup");
        }

        if (!cu.IsAuthorizedPerUIElement("CMS.Ecommerce", "ECReports"))
        {
            CMSMyDeskPage.RedirectToCMSDeskUIElementAccessDenied("CMS.Ecommerce", "ECReports");
        }

        if (!cu.IsAuthorizedPerUIElement("CMS.Ecommerce", "ReportsDashboard"))
        {
            CMSMyDeskPage.RedirectToCMSDeskUIElementAccessDenied("CMS.Ecommerce", "ReportsDashboard");
        }
    }
}