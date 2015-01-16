using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.Notifications;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Notifications_Development_Templates_Template_List : CMSNotificationsPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Process site ID parameter if supplied
        string siteId = QueryHelper.GetString("siteid", "");
        if (!URLHelper.IsPostback())
        {
            siteSelector.Value = siteId;
        }

        // Initialize the grid view        
        gridTemplates.OnAction += new OnActionEventHandler(gridTemplates_OnAction);
        gridTemplates.OrderBy = "TemplateDisplayName";
        gridTemplates.ZeroRowsText = GetString("general.nodatafound");

        string where = "";
        int siteid = ValidationHelper.GetInteger(siteSelector.Value, 0);
        if (siteid > 0)
        {
            where = "TemplateSiteID = " + siteid.ToString();
        }
        else
        {
            where = "TemplateSiteID IS NULL";
        }
        gridTemplates.WhereCondition = where;

        // Prepare the new class header element
        string[,] actions = new string[1,11];
        actions[0, 0] = "HyperLink";
        actions[0, 1] = GetString("notifications.template_edit.new");
        actions[0, 3] = "javascript: AddNewItem();";
        actions[0, 5] = GetImageUrl("Objects/Notification_Template/add.png");

        CurrentMaster.HeaderActions.Actions = actions;
        CurrentMaster.DisplaySiteSelectorPanel = true;

        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.UniSelector.SpecialFields = new string[,] { { GetString("general.global"), "" } };
        siteSelector.UniSelector.OnSelectionChanged += new EventHandler(UniSelector_OnSelectionChanged);
        siteSelector.IsLiveSite = false;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "AddNewItem", ScriptHelper.GetScript(
            "function AddNewItem() { this.window.location = '" + ResolveUrl("~/CMSModules/Notifications/Development/Templates/Template_New.aspx?siteid=" + siteSelector.Value) + "'; }"));
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        gridTemplates.ReloadData();
        pnlUpdate.Update();
    }

    #endregion


    #region "UniGrid Events"

    private void gridTemplates_OnAction(string actionName, object actionArgument)
    {
        // Get template id
        int templateId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect("Template_Edit.aspx?templateid=" + templateId.ToString() + "&siteid=" + siteSelector.Value.ToString());
                break;

            case "delete":
                NotificationTemplateInfoProvider.DeleteNotificationTemplateInfo(templateId);
                break;
        }
    }

    #endregion
}