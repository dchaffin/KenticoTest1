using System;
using System.Web;
using System.Web.UI;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Controls.Configuration;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSSitemanager_Sites_Site_SiteObjects_Tabs : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Excluded binding types
        string[] excludedBindingObjects = { SiteObjectType.CULTURESITE.ToLowerCSafe() };

        // Get site bindings
        string[] otherBindingTypes = SiteInfo.TYPEINFO.OtherBindingObjectTypes.Split(';');

        for (int i = 0; i < otherBindingTypes.Length; i++)
        {
            string type = otherBindingTypes[i];
            if (!excludedBindingObjects.Contains(type.ToLowerCSafe()))
            {
                // Create binding object out of type
                BaseInfo bindingType = CMSObjectHelper.GetReadOnlyObject(type);
                if ((bindingType != null) && bindingType.TypeInfo.IsSiteBinding)
                {
                    // Get other side of the site binding
                    string otherSide = bindingType.TypeInfo.ParentObjectType;
                    string safeOtherSide = TranslationHelper.GetSafeClassName(otherSide);

                    // Build URL
                    string url = "Site_SiteObjects_Content.aspx?siteid=" + QueryHelper.GetInteger("siteid", 0) + "&objecttype=" + otherSide + "&bindingtype=" + type;
                    url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url));

                    // Initialize tab
                    TabItem tab = new TabItem
                                      {
                                          Text = GetString("objecttasks." + safeOtherSide),
                                          RedirectUrl = url
                                      };

                    CurrentMaster.Tabs.AddTab(tab);
                }
            }
        }

        CurrentMaster.Tabs.UrlTarget = "content";
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        CurrentMaster.Tabs.DoTabSelection();
    }
}
