using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSSiteManager_Sites_Site_SiteObjects_Content : CMSDeskPage
{
    protected override void OnInit(EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            string objectType = QueryHelper.GetString("objecttype", null);
            string safeObjectType = TranslationHelper.GetSafeClassName(objectType);
            selectorElem.ObjectType = objectType;
            selectorElem.BindingObjectType = QueryHelper.GetString("bindingtype", null);
            selectorElem.SiteID = QueryHelper.GetInteger("siteid", 0);
            
            // Initialize page title
            SetTitle("Objects/" + safeObjectType + "/object.png", GetString("objecttasks." + safeObjectType), "site_objects_tab", "helpTopic");
        }
        base.OnInit(e);
    }
}