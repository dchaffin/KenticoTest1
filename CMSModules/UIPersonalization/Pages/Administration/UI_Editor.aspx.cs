using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_UIPersonalization_Pages_Administration_UI_Editor : CMSUIPersonalizationPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.SiteID = SiteID;
        editElem.HideSiteSelector = (SiteID != 0);
        ResourceInfo ri = ResourceInfoProvider.GetResourceInfo("CMS.WYSIWYGEditor");
        if (ri != null)
        {
            editElem.ResourceID = ri.ResourceId;
            editElem.IsLiveSite = false;
        }
    }
}