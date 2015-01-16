using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Controls.Configuration;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

[Tabs("CMS.Ecommerce", "Configuration.Departments", "content")]
[Title("Objects/Ecommerce_Department/object.png", "Department_Edit.HeaderCaption", "new_department")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Departments_Department_Header : CMSDepartmentsPage
{
    protected int departmentId = 0;
    protected int editedSiteId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        departmentId = QueryHelper.GetInteger("departmentId", 0);

        CMSMasterPage master = (CMSMasterPage)CurrentMaster;

        string currentDepartmentName = string.Empty;
        DepartmentInfo di = DepartmentInfoProvider.GetDepartmentInfo(departmentId);

        if (di != null)
        {
            editedSiteId = di.DepartmentSiteID;

            // Check site id
            CheckEditedObjectSiteID(editedSiteId);
            currentDepartmentName = ResHelper.LocalizeString(di.DepartmentDisplayName);
        }

        // initializes page title
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = GetString("Department_Edit.ItemListLink");
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/Departments/Department_List.aspx?siteId=" + SiteID;
        breadcrumbs[0, 2] = "configEdit";
        breadcrumbs[1, 0] = FormatBreadcrumbObjectName(currentDepartmentName, editedSiteId);
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";

        master.Title.Breadcrumbs = breadcrumbs;
        master.Tabs.OnTabCreated += Tabs_OnTabCreated;
    }


    private TabItem Tabs_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        // Add SiteId parameter to each tab
        if (!string.IsNullOrEmpty(tab.RedirectUrl))
        {
            tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "siteId", SiteID.ToString());
        }

        return tab;
    }
}