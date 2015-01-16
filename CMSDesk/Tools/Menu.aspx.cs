using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSDesk_Tools_Menu : CMSToolsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        toolsUiPanelMenu.CategoryCreated += toolsUiPanelMenu_CategoryCreated;
    }


    private void toolsUiPanelMenu_CategoryCreated(object sender, CMSAdminControls_UI_UIProfiles_UIPanelMenu.CategoryCreatedEventArgs e)
    {
        // Cancel category if it is not available
        if (e.UIElement != null)
        {
            if (!IsToolsUIElementAvailable(e.UIElement))
            {
                e.Cancel = true;
            }

            // Check site availabitility
            if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("cms.tools", e.UIElement.ElementName, true))
            {
                e.Cancel = true;
            }
        }
    }
}