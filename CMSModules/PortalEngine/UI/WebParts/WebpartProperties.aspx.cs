using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_PortalEngine_UI_WebParts_WebpartProperties : CMSWebPartPropertiesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterModalPageScripts();

        rowsFrameset.Attributes["rows"] = "72, *";

        if (refreshOnCancel)
        {
            // Set the flag indicating that the WebPartId was changed and the parent window should be refreshed even on Cancel event
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshOnCancel", "refreshPageOnClose = true;", true);
        }
    }
}