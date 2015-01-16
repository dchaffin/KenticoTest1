using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_TagGroups_Pages_Development_TagGroup_List : SiteManagerPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get site id
        int siteId = QueryHelper.GetInteger("siteid", CMSContext.CurrentSiteID);

        if (!URLHelper.IsPostback())
        {
            siteSelector.Value = siteId.ToString();
        }

        // Setup page title text and image
        CurrentMaster.Title.TitleText = GetString("tags.taggroup_list.title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_TagGroup/object.png");

        CurrentMaster.DisplaySiteSelectorPanel = true;

        // Initialize the grid view      
        gridTagGroups.OnAction += new OnActionEventHandler(gridTagGroups_OnAction);
        gridTagGroups.GridName = "~/CMSModules/TagGroups/Pages/Development/TagGroup_List.xml";
        gridTagGroups.OrderBy = "TagGroupDisplayName";
        gridTagGroups.ZeroRowsText = GetString("general.nodatafound");
        gridTagGroups.WhereCondition = "(TagGroupIsAdHoc = 0)";

        if (siteSelector.UniSelector.HasData)
        {
            int selectSiteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
            if (selectSiteId > 0)
            {
                gridTagGroups.WhereCondition += " AND (TagGroupSiteID = " + selectSiteId + ")";
            }
        }

        CurrentMaster.Title.HelpTopicName = "taggroups_list";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Prepare the new class header element
        string[,] actions = new string[1, 11];
        actions[0, 0] = "HyperLink";
        actions[0, 1] = GetString("tags.taggroup_list.newgroup");
        actions[0, 3] = "javascript: AddNewItem();";
        actions[0, 5] = GetImageUrl("Objects/CMS_TagGroup/add.png");

        CurrentMaster.HeaderActions.Actions = actions;

        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.UniSelector.OnSelectionChanged += new EventHandler(UniSelector_OnSelectionChanged);
        siteSelector.IsLiveSite = false;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (siteSelector.UniSelector.HasData)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "AddNewItem", ScriptHelper.GetScript(
                "function AddNewItem() { this.window.location = '" + ResolveUrl("~/CMSModules/TagGroups/Pages/Development/TagGroup_New.aspx?siteid=" + siteSelector.Value) + "'; }"));
        }
        else
        {
            lblInfo.Text = GetString("tags.taggroup_list.nosites");
            lblInfo.Visible = true;
            gridTagGroups.Visible = false;
            CurrentMaster.PanelBody.FindControl("pnlAdditionalControls").Visible = false;
            CurrentMaster.PanelBody.FindControl("pnlActions").Visible = false;
        }
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        gridTagGroups.ReloadData();
        pnlUpdate.Update();
    }

    #endregion


    #region "UniGrid Events"

    private void gridTagGroups_OnAction(string actionName, object actionArgument)
    {
        int groupId = -1;
        string siteId = null;

        switch (actionName)
        {
            // Editing of the category fired
            case "edit":
                // Get category ID
                groupId = ValidationHelper.GetInteger(actionArgument, -1);
                siteId = Convert.ToString(siteSelector.Value);

                // Create a target site URL and pass the category ID as a parameter
                string editUrl = "TagGroup_Edit.aspx?groupid=" + groupId.ToString() + "&siteid=" + siteId;
                URLHelper.Redirect(editUrl);
                break;

            // Deleteing of the category was fired
            case "delete":
                groupId = ValidationHelper.GetInteger(actionArgument, -1);

                if (groupId > -1)
                {
                    // If no item depends on the current group                
                    DataSet ds = TagInfoProvider.GetTags("TagGroupID = " + groupId, null);
                    if (DataHelper.DataSourceIsEmpty(ds))
                    {
                        // Delete the class
                        TagGroupInfoProvider.DeleteTagGroupInfo(groupId);
                    }
                    else
                    {
                        // Show error message
                        ShowError(GetString("tags.taggroup_list.hasdependencies"));
                    }
                }
                break;
        }
    }

    #endregion
}