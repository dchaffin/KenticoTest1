using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABVariant_NewPage : CMSContentPage
{
    #region "Page methods and events"

    /// <summary>
    /// Raises the <see cref="E:Init"/> event.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Check module permissions
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.ABTest", "Read"))
        {
            RedirectToAccessDenied(String.Format(GetString("general.permissionresource"), "read", "A/B testing"));
        }

        // Check UI Permissions
        if ((CMSContext.CurrentUser == null) || (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "New.ABTestVariant")))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "New.ABTestVariant");
        }

        // Header actions
        CurrentMaster.HeaderActions.AddAction(new SaveAction(Page));
        CurrentMaster.HeaderActions.AddAction(new SaveAction(Page)
                                                        {
                                                            RegisterShortcutScript = false,
                                                            Text = GetString("editmenu.iconsaveandanother"),
                                                            Tooltip = GetString("EditMenu.SaveAndAnother"),
                                                            ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/saveandanother.png"),
                                                            SmallImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/16/saveandanother.png"),
                                                            CommandArgument = "true"
                                                        });
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        CurrentMaster.HeaderActions.HelpTopicName = "CMS_Content_New_ABvariant";

        // Register title script
        ScriptHelper.RegisterTitleScript(this, GetString("content.ui.newvariant"));
        ScriptHelper.RegisterScriptFile(Page, "cmsedit.js");
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        String siteName = CMSContext.CurrentSiteName;

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSABTestingEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("abtesting.disabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;

        ScriptHelper.RegisterProgress(Page);
    }

    #endregion



    void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Check the permissions
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.abtest", "Manage"))
        {
            ShowError(GetString("om.abtest.nomanagepermission"));
            return;
        }

        bool createAnother = ValidationHelper.GetBoolean(e.CommandArgument, false);

        // Create document
        int newNodeID = ucNewPage.Save(createAnother);
        if (newNodeID != 0)
        {
            // Refresh tree
            string script = null;
            if (createAnother)
            {
                int parentID = QueryHelper.GetInteger("nodeID", 0);
                if (parentID != 0)
                {
                    script = ScriptHelper.GetScript("RefreshTree(" + parentID + ", " + parentID + ");CreateAnother();");
                }
            }
            else
            {
                script = ScriptHelper.GetScript("RefreshTree(" + newNodeID + ", " + newNodeID + ");SelectNode(" + newNodeID + ");");
            }
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "Refresh", script);
        }
    }
}