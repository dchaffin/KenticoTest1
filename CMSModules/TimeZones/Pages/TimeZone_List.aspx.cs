using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_TimeZones_Pages_TimeZone_List : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("TimeZ.List.Header");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_TimeZone/object.png");

        // Set context help
        CurrentMaster.Title.HelpTopicName = "time_zones_list";
        CurrentMaster.Title.HelpName = "helpTopic";

        uniGrid.OnAction += uniGrid_OnAction;
        uniGrid.OnExternalDataBound += uniGrid_OnExternalDataBound;
        uniGrid.ZeroRowsText = GetString("general.nodatafound");

        // New item link
        string[,] actions = new string[1, 6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("TimeZ.List.NewItem");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl("TimeZone_Edit.aspx");
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("Objects/CMS_TimeZone/add.png");
        CurrentMaster.HeaderActions.Actions = actions;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "edit":
                URLHelper.Redirect("TimeZone_Edit.aspx?zoneid=" + Convert.ToString(actionArgument));
                break;

            case "delete":
                TimeZoneInfoProvider.DeleteTimeZoneInfo(Convert.ToInt32(actionArgument));
                break;
        }
    }


    private object uniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "daylight":
                return UniGridFunctions.ColorLessSpanYesNo(parameter);
        }

        return parameter;
    }
}