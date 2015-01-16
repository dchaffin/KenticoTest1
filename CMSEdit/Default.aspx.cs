using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.PortalEngine;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.DocumentEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSEdit_default : AbstractCMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Redirect to the web site root by default
        string returnUrl = URLHelper.ResolveUrl("~/");

        // Check whether on-site editing is enabled
        if (PortalHelper.IsOnSiteEditingEnabled(CMSContext.CurrentSiteName))
        {
            CurrentUserInfo cui = CMSContext.CurrentUser;
            // Check the permissions
            if ((cui != null) && cui.IsEditor && cui.IsAuthorizedPerResource("cms.content", "ExploreTree") && cui.IsAuthorizedPerResource("cms.content", "Read"))
            {
                // Set edit-live view mode
                PortalContext.SetViewMode(ViewModeEnum.EditLive);
            }
            else
            {
                // Redirect to access denied page when the current user does not have permissions for the OnSite editing
                CMSPage.RedirectToUINotAvailable();
            }

            // Try get return URL
            string queryUrl = QueryHelper.GetString("returnurl", String.Empty);
            if (!String.IsNullOrEmpty(queryUrl) && (queryUrl.StartsWith("~/") || queryUrl.StartsWith("/")))
            {
                // Remove return url duplication if exist
                int commaIndex = queryUrl.IndexOfCSafe(",", 0, false);
                if (commaIndex > 0)
                {
                    queryUrl = queryUrl.Substring(0, commaIndex);
                }
                returnUrl = URLHelper.ResolveUrl(queryUrl);
            }
            // Use default alias path if return url isn't defined
            else
            {
                string aliasPath = PageInfoProvider.GetDefaultAliasPath(URLHelper.GetCurrentDomain(), CMSContext.CurrentSiteName);
                if (!String.IsNullOrEmpty(aliasPath))
                {
                    // Get the document which will be displayed for the default alias path
                    TreeProvider tr = new TreeProvider();
                    TreeNode node = tr.SelectSingleNode(CMSContext.CurrentSiteName, aliasPath, CMSContext.PreferredCultureCode, true);
                    if (node != null)
                    {
                        aliasPath = node.NodeAliasPath;
                    }

                    returnUrl = DocumentURLProvider.GetUrl(aliasPath);
                    returnUrl = URLHelper.ResolveUrl(returnUrl);
                }
            }

            // Remove view mode value from query string
            returnUrl = URLHelper.RemoveParameterFromUrl(returnUrl, "viewmode");
        }

        // Redirect to the requested page
        URLHelper.Redirect(returnUrl);
    }
}
