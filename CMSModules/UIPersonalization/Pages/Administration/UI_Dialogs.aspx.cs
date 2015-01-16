using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.UIControls;

public partial class CMSModules_UIPersonalization_Pages_Administration_UI_Dialogs : CMSUIPersonalizationPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.SiteID = SiteID;
        editElem.IsLiveSite = false;
        editElem.HideSiteSelector = (SiteID != 0);
    }
}