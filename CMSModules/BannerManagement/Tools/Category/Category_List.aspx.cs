using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.SettingsProvider;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.ExtendedControls;

[Title("Objects/CMS_BannerCategory/object.png", "banner.bannercategory_list.title", "banner_category_list")]

[Actions(1)]
[Action(0, ResourceString = "banner.bannercategory_list.newcategory", TargetUrl = "~/CMSModules/BannerManagement/Tools/Category/Category_New.aspx?siteid={%SelectedSiteID%}", ImageUrl = "Objects/CMS_BannerCategory/add.png")]

public partial class CMSModules_BannerManagement_Tools_Category_Category_List : CMSBannerManagementPage
{
    #region "Private fields"

    private int selectedSiteID;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the grid view      
        gridBannerManagement.OnAction += new OnActionEventHandler(gridBannerManagement_OnAction);
        gridBannerManagement.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridBannerManagement_OnExternalDataBound);
        
        // If user can view global and local rooms, display site selector
        if (ReadAllowed && ReadGlobalAllowed)
        {
            CurrentMaster.DisplaySiteSelectorPanel = true;

            // Set selected site in SiteSelector only if is not postback (that means if user has returned from another page)
            if (!RequestHelper.IsPostBack())
            {
                siteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteid", CMSContext.CurrentSiteID);
            }

            // Get site id from site selector
            selectedSiteID = siteOrGlobalSelector.SiteID;

            // Security check: user can select global (-4) this site and global (-5) or current site, if something else was selected, set it back to current site
            if ((selectedSiteID != -4) && (selectedSiteID != -5) && (selectedSiteID != CMSContext.CurrentSiteID))
            {
                selectedSiteID = siteOrGlobalSelector.SiteID = CMSContext.CurrentSiteID;
            }
        }
        else
        {
            if (ReadAllowed)
            {
                selectedSiteID = CMSContext.CurrentSiteID;
            }
            else
            {
                selectedSiteID = -4;
            }
        }

        // -4 is global
        if (selectedSiteID == -4)
        {
            gridBannerManagement.WhereCondition = "(BannerCategorySiteID IS NULL)";
        }
        // Global and this site
        else if (selectedSiteID == -5)
        {
            gridBannerManagement.WhereCondition = string.Format("(BannerCategorySiteID IS NULL OR BannerCategorySiteID = {0})", CMSContext.CurrentSiteID);
        }
        // Single site
        else
        {
            gridBannerManagement.WhereCondition = "(BannerCategorySiteID = " + selectedSiteID + ")";
        }

        // Store selected site ID to MacroResolver, so it can be retrieved in ActionAttribute's Apply method (on second pass, which is called on PreRender)
        CMSContext.CurrentResolver.SetNamedSourceData("SelectedSiteID", selectedSiteID);

        RegisterRefreshUsingPostBackScript();
    }
    
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (selectedSiteID == -5)
        {
            HeaderActions.ActionsList[0].Enabled = false;

            FormEngineUserControl label = this.LoadUserControl("~/CMSFormControls/Basic/LabelControl.ascx") as FormEngineUserControl;
            label.Value = GetString("banner.choosegloborsite");

            HeaderActions.AdditionalControls.Add(label);
            HeaderActions.AdditionalControlsCssClass = "HeaderActionsLabel";
            HeaderActions.ReloadAdditionalControls();
        }
    }

    #endregion


    #region "UniGrid Events"

    private void gridBannerManagement_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                int categoryID = ValidationHelper.GetInteger(actionArgument, 0);

                BannerCategoryInfo bannerCategory = BannerCategoryInfoProvider.GetBannerCategoryInfo(categoryID);
                
                // If category wasn't found 
                if (bannerCategory == null)
                {
                    ShowError(ResHelper.GetString("banner.bannercategory_list.nocategoryid"));

                    return;
                }

                CheckModifyPermission(bannerCategory.BannerCategorySiteID);

                // Delete the class
                bannerCategory.Delete();

                break;
        }
    }


    private object gridBannerManagement_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        sourceName = sourceName.ToLowerCSafe();

        switch (sourceName)
        {
            case "delete":
                DataRow row = ((DataRowView)((GridViewRow)parameter).DataItem).Row;

                int? siteID = row.IsNull("BannerCategorySiteID") ? (int?)null : ValidationHelper.GetInteger(row["BannerCategorySiteID"], 0);

                string iconName;
                
                ImageButton button = ((ImageButton)sender);

                if (HasUserModifyPermission(siteID))
                {
                    iconName = "Delete";
                }
                else
                {
                    iconName = "Deletedisabled";
                    button.Enabled = false;
                }

                button.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/" + iconName + ".png");

                break;
        }

        return parameter;
    }

    #endregion


    #region "Methods"

    private void RegisterRefreshUsingPostBackScript()
    {
        string script = @"
function RefreshUsingPostBack()
{{
    {0};
}}";
        script = string.Format(script, ControlsHelper.GetPostBackEventReference(btnHiddenPostBackButton, null));

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshUsingPostBack", script, true);
    }

    #endregion
}
