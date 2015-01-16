using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_SystemTables_Pages_Development_SystemTable_List : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        UniGridSysTables.OnAction += UniGridSysTables_OnAction;
        UniGridSysTables.ZeroRowsText = GetString("general.nodatafound");
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGridSysTables_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect("Frameset.aspx?classid=" + Convert.ToString(actionArgument));
                break;
        }
    }
}