using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_WebPartProperties_properties_frameset : CMSWebPartPropertiesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        frameContent.Attributes.Add("src", "webpartproperties_properties.aspx" + URLHelper.Url.Query);
        frameButtons.Attributes.Add("src", "webpartproperties_buttons.aspx" + URLHelper.Url.Query);
    }
}