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

[Security(Resource = "CMS.Ecommerce", UIElements = "Configuration.Settings.General")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_General : CMSEcommerceStoreSettingsPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        GlobalObjectsKeyName = ECommerceSettings.USE_GLOBAL_CURRENCIES;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeControls();

        // Register scripts for saving document by shortcut
        ScriptHelper.RegisterSaveShortcut(this, "save", null);

        // Register scripts for currency change
        ScriptHelper.RegisterDialogScript(this);

        // Init label for accessibility
        lblHdnChangeCurrency.Text = GetString("general.change");
        lblHdnChangeCurrency.Style["display"] = "none";

        // Append tooltip
        ScriptHelper.AppendTooltip(lblCurrentMainCurrency, GetString("com.maincurrencytooltip"), "help");

        // Display info message when main currency saved
        if (QueryHelper.GetBoolean("currencysaved", false))
        {
            ShowInformation(GetString("com.storesettings.maincurrencychanged"));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Gets current main currency.
    /// </summary>
    private void GetCurrentMainCurrency()
    {
        int mainCurrencyId = -1;

        CurrencyInfo mainCurrency = CurrencyInfoProvider.GetMainCurrency(ConfiguredSiteID);
        if (mainCurrency != null)
        {
            mainCurrencyId = mainCurrency.CurrencyID;
            lblMainCurrency.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(mainCurrency.CurrencyDisplayName));
        }
        else
        {
            lblMainCurrency.Text = GetString("general.na");
        }

        DataSet ds = CurrencyInfoProvider.GetCurrencies("CurrencyEnabled = 1 AND ISNULL(CurrencySiteID, 0) = " + ConfiguredSiteID + " AND NOT CurrencyID = " + mainCurrencyId, null);
        // When there is no choice
        if (DataHelper.DataSourceIsEmpty(ds))
        {
            // Disable "change main currency" button
            btnChangeCurrency.Enabled = false;
        }
    }


    /// <summary>
    /// Initialization of form controls.
    /// </summary>
    private void InitializeControls()
    {
        CurrentMaster.HeaderActions.Actions = GetStoreSettingsActions();
        CurrentMaster.HeaderActions.ActionPerformed += StoreSettingsActions_ActionPerformed;

        // Assign category, group and site ID
        SettingsGroupViewer.CategoryName = "CMS.ECommerce";
        SettingsGroupViewer.Where = "CategoryName IN (N'CMS.Ecommerce.Accounts', N'CMS.Ecommerce.Shipping', N'CMS.Ecommerce.Taxes', N'CMS.Ecommerce.ProductsUI', N'CMS.Ecommerce.ProductProperties', N'CMS.ECommerce.Pages', N'CMS.Ecommerce.Pricing')";
        SettingsGroupViewer.SiteID = SiteID;

        GetCurrentMainCurrency();

        string dialogUrl = URLHelper.ResolveUrl("StoreSettings_ChangeCurrency.aspx");
        btnChangeCurrency.OnClientClick = "modalDialog('" + dialogUrl + "?siteId=" + ConfiguredSiteID + "', 'ChangeMainCurrency', 600, 480); return false;";
    }


    /// <summary>
    /// Handles saving of configuration.
    /// </summary>
    protected override void SaveChanges()
    {
        SettingsGroupViewer.SaveChanges();
    }

    #endregion
}