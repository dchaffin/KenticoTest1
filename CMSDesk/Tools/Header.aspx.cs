using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSDesk_Tools_Header : CMSToolsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        uiToolbarElem.OnButtonFiltered += new CMSAdminControls_UI_UIProfiles_UIToolbar.ButtonFilterEventHandler(uiToolbarElem_OnButtonFiltered);
    }


    private bool uiToolbarElem_OnButtonFiltered(object sender, UniMenuArgs e)
    {
        if (e.UIElement == null)
        {
            return false;
        }

        // Check site availability
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("cms.tools", e.UIElement.ElementName, true))
        {
            return false;
        }

        // Check whether separable modules are loaded
        return IsToolsUIElementAvailable(e.UIElement);
    }
}