using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Configuration.Settings.Emails")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_Emails : CMSEcommerceStoreSettingsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Register scripts for saving document by shortcut
        ScriptHelper.RegisterSaveShortcut(this, "save", null);

        // Set up header
        CurrentMaster.HeaderActions.Actions = GetStoreSettingsActions();
        CurrentMaster.HeaderActions.ActionPerformed += StoreSettingsActions_ActionPerformed;

        // Assign category, group and site ID
        SettingsGroupViewer.CategoryName = "CMS.ECommerce";
        SettingsGroupViewer.Where = "CategoryName = N'CMS.Ecommerce.Emails'";
        SettingsGroupViewer.SiteID = SiteID;
    }


    /// <summary>
    /// Handles saving of configuration.
    /// </summary>
    protected override void SaveChanges()
    {
        SettingsGroupViewer.SaveChanges();
    }
}