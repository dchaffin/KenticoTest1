using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.PortalEngine;
using CMS.DocumentEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Controls;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;
using SubMenuItem = CMS.UIControls.UniMenuConfig.SubItem;
using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSAdminControls_UI_UniMenu_OnSiteEdit_CultureMenu : CMSUserControl
{
    #region "Private variables"

    private Dictionary<string, string> mNodeCultures = null;
    private InfoDataSet<CultureInfo> mSiteCultures = null;
    private PageInfo pi = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets all the cultures of the current node.
    /// </summary>
    private Dictionary<string, string> NodeCultures
    {
        get
        {
            if (mNodeCultures == null)
            {
                // Get all language versions
                TreeNodeDataSet culturesDs = GetNodeCultures();
                mNodeCultures = new Dictionary<string, string>();

                // Create culture/UrlPath collection
                if (!DataHelper.DataSourceIsEmpty(culturesDs))
                {
                    foreach (DataRow dr in culturesDs.Tables[0].Rows)
                    {
                        string docCulture = ValidationHelper.GetString(dr["DocumentCulture"], String.Empty);
                        string urlPath = ValidationHelper.GetString(dr["DocumentUrlPath"], String.Empty);
                        mNodeCultures.Add(docCulture, urlPath);
                    }
                }
            }

            return mNodeCultures;
        }
    }


    /// <summary>
    /// Gets all the site cultures.
    /// </summary>
    private InfoDataSet<CultureInfo> SiteCultures
    {
        get
        {
            if (mSiteCultures == null)
            {
                mSiteCultures = CultureInfoProvider.GetSiteCultures(CMSContext.CurrentSiteName);
            }

            return mSiteCultures;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        string prefferedCultureCode = CMSContext.PreferredCultureCode;
        string defaultSmallIconUrl = GetImageUrl("CMSModules/CMS_DeviceProfiles/default_list.png");
        string defaultBigIconUrl = GetImageUrl("CMSModules/CMS_DeviceProfiles/default.png");
        string defaultString = GetString("general.default");

        InfoDataSet<CultureInfo> siteCultures = CultureInfoProvider.GetSiteCultures(CMSContext.CurrentSiteName);
        pi = CMSContext.CurrentPageInfo ?? OnSiteEditHelper.PageInfoForPageNotFound;

        // Cultures button
        MenuItem cultureItem = new MenuItem();
        cultureItem.CssClass = "BigButton";
        cultureItem.ImageAlign = ImageAlign.Top;
        cultureItem.ImagePath = URLHelper.UnResolveUrl(UIHelper.GetFlagIconUrl(Page, prefferedCultureCode, "16x16"), URLHelper.ApplicationPath);
        cultureItem.Text = GetString("general.cultures");
        cultureItem.Tooltip = GetString("onsiteedit.languageselector");
        cultureItem.ImageAltText = GetString("general.cultures");

        // Add all cultures to the sub menu
        foreach (CultureInfo culture in siteCultures)
        {
            string iconUrl = UIHelper.GetFlagIconUrl(Page, culture.CultureCode, "16x16");
            string cultureName = culture.CultureName;
            string cultureCode = culture.CultureCode;
            string cultureShortName = culture.CultureShortName;

            if (cultureCode != prefferedCultureCode)
            {
                SubMenuItem menuItem = new SubMenuItem()
                {
                    Text = cultureName,
                    Tooltip = cultureName,
                    ImagePath = iconUrl,
                    ImageAltText = cultureName
                };

                // Build the web part image html
                bool translationExists = NodeCultures.ContainsKey(cultureCode);

                if (translationExists)
                {
                    // Assign click action which changes the document culture
                    menuItem.OnClientClick = "document.location.replace(" + ScriptHelper.GetString(URLHelper.UpdateParameterInUrl(URLHelper.CurrentURL, URLHelper.LanguageParameterName, cultureCode)) + ");";
                }
                else
                {
                    // Display the "Not translated" image
                    menuItem.RightImagePath = GetImageUrl("/CMSModules/CMS_PortalEngine/OnSiteEdit/no_culture.png");
                    menuItem.RightImageAltText = GetString("onsitedit.culturenotavailable");

                    // Assign click action -> Create new document culture
                    menuItem.OnClientClick = "NewDocumentCulture(" + pi.NodeID + ",'" + cultureCode + "');";
                }

                cultureItem.SubItems.Add(menuItem);
            }
            else
            {
                // Current culture
                cultureItem.Text = culture.CultureShortName;
                cultureItem.Tooltip = cultureName;
                cultureItem.ImagePath = iconUrl;
                cultureItem.ImageAltText = cultureName;
            }
        }

        btnCulture.Buttons.Add(cultureItem);
    }


    /// <summary>
    /// Returns all culture nodes
    /// </summary>
    protected TreeNodeDataSet GetNodeCultures()
    {
        // Get all language versions
        TreeProvider tp = new TreeProvider(CMSContext.CurrentUser);
        return tp.SelectNodes(CMSContext.CurrentSiteName, "/%", TreeProvider.ALL_CULTURES, false, null, "NodeID = " + pi.NodeID, "DocumentCulture", -1, false, 0, "DocumentCulture, DocumentUrlPath");
    }

    #endregion
}
