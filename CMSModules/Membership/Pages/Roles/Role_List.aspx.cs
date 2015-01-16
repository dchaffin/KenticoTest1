using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_Membership_Pages_Roles_Role_List : CMSRolesPage
{
    #region "Properties"

    /// <summary>
    /// Local header actions
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return headerActions;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        ((Panel)CurrentMaster.PanelBody.FindControl("pnlContent")).CssClass = "";
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup page title text and image
        CurrentMaster.Title.TitleText = GetString("general.roles");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Role/object.png");

        CurrentMaster.Title.HelpTopicName = "roles_list";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Control initialization
        HeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("Administration-Role_List.NewRole"),
            ImageUrl = GetImageUrl("Objects/CMS_Role/add.png"),
            OnClientClick = "AddNewItem();"
        });

        if (SiteID != 0)
        {
            pnlSites.Visible = false;
        }
        else
        {
            // Set site selector
            siteSelector.DropDownSingleSelect.AutoPostBack = true;
            siteSelector.AllowAll = false;
            siteSelector.AllowEmpty = false;
            siteSelector.OnlyRunningSites = false;
            siteSelector.UniSelector.OnSelectionChanged += new EventHandler(UniSelector_OnSelectionChanged);
            siteSelector.AllowSetSpecialFields = true;

            if (!RequestHelper.IsPostBack())
            {
                if (SelectedSiteID > 0)
                {
                    siteSelector.Value = SelectedSiteID;
                }
                else if (CMSContext.CurrentUser.UserSiteManagerAdmin)
                {
                    siteSelector.Value = siteSelector.GlobalRecordValue;
                    SelectedSiteID = ValidationHelper.GetInteger(siteSelector.GlobalRecordValue, UniSelector.US_GLOBAL_RECORD);
                }
            }
            else
            {
                SelectedSiteID = ValidationHelper.GetInteger(siteSelector.Value, 0);
            }
        }

        roleListElem.SiteID = (SiteID > 0) ? SiteID : SelectedSiteID;
        roleListElem.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(roleListElem_OnCheckPermissions);

        roleListElem.OnEdit += new EventHandler(roleListElem_OnEdit);
    }


    protected void roleListElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.roles", permissionType))
        {
            RedirectToCMSDeskAccessDenied("CMS.Roles", permissionType);
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Register correct script for new item
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "AddNewItem", ScriptHelper.GetScript(
            "function AddNewItem() { this.window.location = '" + ResolveUrl("Role_New.aspx?" + GetSiteOrSelectedSite() + "'} ")));
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns 'siteid' or 'selectedsiteid' parametr depending on QueryString.
    /// </summary>
    /// <returns>Query parameter</returns>
    private string GetSiteOrSelectedSite()
    {
        // Site ID is used in CMS desk
        if (SiteID > 0)
        {
            return "siteId=" + SiteID;
        }
        // SelectedSiteID is used in CMS Site Manager
        else if (SelectedSiteID > 0)
        {
            return "selectedsiteid=" + SelectedSiteID;
        }

        return String.Empty;
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Update unigrid
        pnlUpdate.Update();
    }


    /// <summary>
    /// Edit event handler.
    /// </summary>
    protected void roleListElem_OnEdit(object sender, EventArgs e)
    {
        URLHelper.Redirect("Role_Edit_Frameset.aspx?roleId=" + roleListElem.SelectedItemID + "&" + GetSiteOrSelectedSite());
    }

    #endregion
}