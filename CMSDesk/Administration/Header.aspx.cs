using System;

using CMS.CMSHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSDesk_Administration_Header : CMSAdministrationPage
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
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("cms.administration", e.UIElement.ElementName, true))
        {
            return false;
        }

        // Check whether separable modules are loaded
        return IsAdministrationUIElementAvailable(e.UIElement);
    }
}