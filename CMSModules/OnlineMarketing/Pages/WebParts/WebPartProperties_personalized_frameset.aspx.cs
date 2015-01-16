using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_OnlineMarketing_Pages_WebParts_WebPartProperties_personalized_frameset : CMSWebPartPropertiesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions for web part properties UI
        CurrentUserInfo currentUser = CMSContext.CurrentUser;

        if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", "WebPartProperties.Variant"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "WebPartProperties.Variant");
        }

        frameContent.Attributes.Add("src", "webpartproperties_personalized.aspx" + URLHelper.Url.Query);
        frameButtons.Attributes.Add("src", ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/webpartproperties_buttons.aspx" + URLHelper.Url.Query));
    }
}