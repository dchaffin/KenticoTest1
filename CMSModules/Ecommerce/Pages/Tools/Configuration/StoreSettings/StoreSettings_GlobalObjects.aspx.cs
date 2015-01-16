using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_GlobalObjects : CMSEcommerceStoreSettingsPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions for CMS Desk -> Ecommerce
        if (!CMSContext.CurrentUser.IsGlobalAdministrator)
        {
            RedirectToAccessDenied(GetString("security.accesspage.onlyglobaladmin"));
            return;
        }

        // Register scripts for saving document by shortcut
        ScriptHelper.RegisterSaveShortcut(this, "save", null);

        // Set up header
        CurrentMaster.HeaderActions.Actions = GetStoreSettingsActions();
        CurrentMaster.HeaderActions.ActionPerformed += StoreSettingsActions_ActionPerformed;

        // Assign category, group and site ID
        SettingsGroupViewer.CategoryName = "CMS.Ecommerce.GlobalObjects";
        SettingsGroupViewer.SiteID = SiteID;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string siteName = SiteInfoProvider.GetSiteName(SiteID);

        // Show warning when using global exchange rates and site specific currencies
        if (ECommerceSettings.UseGlobalExchangeRates(siteName) && !ECommerceSettings.UseGlobalCurrencies(siteName))
        {
            ShowWarning(GetString("com.WrongCurrencyRateCombination"), null, null);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles saving of configuration.
    /// </summary>
    protected override void SaveChanges()
    {
        SettingsGroupViewer.SaveChanges();
    }

    #endregion
}