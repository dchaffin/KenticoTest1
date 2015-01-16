using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_InlineControls_Pages_Development_List : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("InlineControl_List.HeaderCaption");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_InlineControl/object.png");
        CurrentMaster.Title.HelpTopicName = "inline_controls_list";
        CurrentMaster.Title.HelpName = "helpTopic";

        // New item link
        string[,] actions = new string[1,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("InlineControl_List.NewItemCaption");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl("New.aspx");
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("Objects/CMS_InlineControl/add.png");
        CurrentMaster.HeaderActions.Actions = actions;

        UniGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);
        UniGrid.ZeroRowsText = GetString("general.nodatafound");
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("Frameset.aspx?inlinecontrolid=" + Convert.ToString(actionArgument));
        }
        else if (actionName == "delete")
        {
            // Delete InlineControlInfo object from database
            InlineControlInfoProvider.DeleteInlineControlInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
    }
}