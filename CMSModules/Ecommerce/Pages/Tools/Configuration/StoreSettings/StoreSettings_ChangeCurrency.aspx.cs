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
using CMS.UIControls;

[Title(ImageUrl = "Objects/Ecommerce_Currency/object.png", ResourceString = "StoreSettings_ChangeCurrency.ChangeCurrencyTitle")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_ChangeCurrency : CMSEcommerceModalPage
{
    private CurrencyInfo mainCurrency = null;
    private int editedSiteId = -1;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register JQuery
        ScriptHelper.RegisterJQuery(this);

        // Check permissions (only global admin can see this dialog)
        CurrentUserInfo ui = CMSContext.CurrentUser;

        if ((ui == null) || !ui.IsGlobalAdministrator)
        {
            // Redirect to access denied
            RedirectToAccessDenied(GetString("StoreSettings_ChangeCurrency.AccessDenied"));
        }

        int siteId = QueryHelper.GetInteger("siteId", CMSContext.CurrentSiteID);

        if (ui.IsGlobalAdministrator)
        {
            editedSiteId = (siteId <= 0) ? 0 : siteId;
        }
        else
        {
            editedSiteId = CMSContext.CurrentSiteID;
        }

        mainCurrency = CurrencyInfoProvider.GetMainCurrency(editedSiteId);

        // Load the UI
        CurrentMaster.Page.Title = "Ecommerce - Change main currency";
        chkRecalculateFromGlobal.InputAttributes["onclick"] = "checkRecalculation()";
        chkCredit.InputAttributes["onclick"] = "checkRecalculation()";
        chkDocuments.InputAttributes["onclick"] = "checkRecalculation()";
        chkExchangeRates.InputAttributes["onclick"] = "checkRecalculation()";
        chkFlatDiscountsCoupons.InputAttributes["onclick"] = "checkRecalculation()";
        chkFlatTaxes.InputAttributes["onclick"] = "checkRecalculation()";
        chkFlatVolumeDiscounts.InputAttributes["onclick"] = "checkRecalculation()";
        chkProductPrices.InputAttributes["onclick"] = "checkRecalculation()";
        chkShipping.InputAttributes["onclick"] = "checkRecalculation()";
        chkFreeShipping.InputAttributes["onclick"] = "checkRecalculation()";
        chkOrders.InputAttributes["onclick"] = "checkRecalculation()";
        btnCancel.Text = GetString("General.Cancel");
        btnOk.Text = GetString("General.OK");
        imgHelp.ImageUrl = GetImageUrl("General/HelpSmall.png");
        imgHelp.ToolTip = GetString("StoreSettings_ChangeCurrency.ExchangeRateHelp");
        imgRoundHelp.ImageUrl = GetImageUrl("General/HelpSmall.png");
        imgRoundHelp.ToolTip = GetString("StoreSettings_ChangeCurrency.ExchangeRateRoundHelp");

        if (mainCurrency != null)
        {
            // Set confirmation message for OK button
            btnOk.Attributes["onclick"] = "return confirm(" + ScriptHelper.GetString(GetString("StoreSettings_ChangeCurrency.Confirmation")) + ")";

            lblOldMainCurrency.Text = HTMLHelper.HTMLEncode(mainCurrency.CurrencyDisplayName);
        }
        else
        {
            plcObjectsSelection.Visible = false;
            plcRecalculationDetails.Visible = false;
            plcOldCurrency.Visible = false;
        }

        currencyElem.AddSelectRecord = true;
        currencyElem.SiteID = editedSiteId;

        plcRecountDocuments.Visible = (editedSiteId != 0);
        plcRecalculateFromGlobal.Visible = (editedSiteId == 0);

        if (!URLHelper.IsPostback())
        {
            if (QueryHelper.GetBoolean("saved", false))
            {
                // Refresh the page
                ltlScript.Text = ScriptHelper.GetScript(@"var loc = wopener.location;
                if(!(/currencysaved=1/.test(loc)))
                {
                    loc += '&currencysaved=1';
                }
                wopener.location.replace(loc); CloseDialog();");

                ShowChangesSaved();
            }
        }

        // Prepare checkRecalculation script parts for checkboxes which can be hidden
        string conditionalCheckboxesScript = "";

        if (plcRecountDocuments.Visible)
        {
            conditionalCheckboxesScript = string.Format(@"
  tmp = jQuery('#{0}');
  if(tmp.length == 1 && tmp[0].checked) {{recalcNeeded = true;}}", chkDocuments.ClientID);
        }

        if (plcRecalculateFromGlobal.Visible)
        {
            conditionalCheckboxesScript += string.Format(@"
  tmp = jQuery('#{0}');
  if(tmp.length == 1 && tmp[0].checked) {{recalcNeeded = true;}}", chkRecalculateFromGlobal.ClientID);
        }

        // Init scripts
        string checkRecalculationScript = string.Format(@"
function checkRecalculation(){{
  var recalcNeeded = false;   
  var tmp = jQuery('#{0}');
  if(tmp.length == 1 && tmp[0].checked) {{recalcNeeded = true;}}
  tmp = jQuery('#{1}');
  if(tmp.length == 1 && tmp[0].checked) {{recalcNeeded = true;}}
  tmp = jQuery('#{2}');
  if(tmp.length == 1 && tmp[0].checked) {{recalcNeeded = true;}}
  tmp = jQuery('#{3}');
  if(tmp.length == 1 && tmp[0].checked) {{recalcNeeded = true;}}
  tmp = jQuery('#{4}');
  if(tmp.length == 1 && tmp[0].checked) {{recalcNeeded = true;}}
  tmp = jQuery('#{5}');
  if(tmp.length == 1 && tmp[0].checked) {{recalcNeeded = true;}}
  tmp = jQuery('#{6}');
  if(tmp.length == 1 && tmp[0].checked) {{recalcNeeded = true;}}
  tmp = jQuery('#{7}');
  if(tmp.length == 1 && tmp[0].checked) {{recalcNeeded = true;}}
  tmp = jQuery('#{8}');
  if(tmp.length == 1 && tmp[0].checked) {{recalcNeeded = true;}}
{11}

  if(recalcNeeded)
  {{
    jQuery('#{9}').parents('tr').show();
    jQuery('#{10}').parents('tr').show();
  }}else
  {{
    jQuery('#{9}').parents('tr').hide();
    jQuery('#{10}').parents('tr').hide();
  }}
}}", chkExchangeRates.ClientID, chkProductPrices.ClientID, chkFlatTaxes.ClientID, chkFlatDiscountsCoupons.ClientID, chkFlatVolumeDiscounts.ClientID, chkCredit.ClientID, chkShipping.ClientID, chkFreeShipping.ClientID, chkOrders.ClientID, txtEchangeRate.ClientID, txtRound.ClientID, conditionalCheckboxesScript);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CheckRecalculationScript", checkRecalculationScript, true);
        ScriptHelper.RegisterStartupScript(this, typeof(string), "CheckRecalculationStartup", "checkRecalculation();", true);
    }


    /// <summary>
    /// Changes the selected prices and other object fields.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        string err = "";

        if ((mainCurrency != null) && RecalculationRequested())
        {
            err = new Validator().NotEmpty(txtEchangeRate.Text.Trim(), GetString("StoreSettings_ChangeCurrency.EmptyExchangeRate"))
                .NotEmpty(txtRound.Text.Trim(), GetString("StoreSettings_ChangeCurrency.InvalidRound"))
                .IsInteger(txtRound.Text.Trim(), GetString("StoreSettings_ChangeCurrency.InvalidRound"))
                .IsDouble(txtEchangeRate.Text.Trim(), GetString("StoreSettings_ChangeCurrency.InvalidExchangeRate")).Result;
        }

        // Check exchange rate value
        double newRate = ValidationHelper.GetDouble(txtEchangeRate.Text.Trim(), 1);
        if (string.IsNullOrEmpty(err) && (newRate <= 0))
        {
            err = GetString("StoreSettings_ChangeCurrency.NegativeExchangeRate");
        }

        // Check new currency ID
        int newCurrencyId = ValidationHelper.GetInteger(currencyElem.Value, 0);
        if (string.IsNullOrEmpty(err) && (newCurrencyId <= 0))
        {
            err = GetString("StoreSettings_ChangeCurrency.NoNewMainCurrency");
        }

        // Show error message if any
        if (err != "")
        {
            ShowError(err);
            return;
        }

        // Get the new main currency
        CurrencyInfo newCurrency = CurrencyInfoProvider.GetCurrencyInfo(newCurrencyId);
        if (newCurrency != null)
        {
            // Only select new main currency when no old main currency specified
            if (mainCurrency == null)
            {
                newCurrency.CurrencyIsMain = true;
                CurrencyInfoProvider.SetCurrencyInfo(newCurrency);

                // Refresh the page
                URLHelper.Redirect(URLHelper.AddParameterToUrl(URLHelper.CurrentURL, "saved", "1"));

                return;
            }

            // Set new main currency and recalculate requested objects
            double rate = 1 / newRate;
            int round = ValidationHelper.GetInteger(txtRound.Text.Trim(), 2);

            try
            {
                RecalculationSettings settings = new RecalculationSettings();
                settings.ExchangeRates = chkExchangeRates.Checked;
                settings.FromGlobalCurrencyRates = (plcRecalculateFromGlobal.Visible && chkRecalculateFromGlobal.Checked);
                settings.Products = chkProductPrices.Checked;
                settings.Taxes = chkFlatTaxes.Checked;
                settings.DiscountCoupons = chkFlatDiscountsCoupons.Checked;
                settings.VolumeDiscounts = chkFlatVolumeDiscounts.Checked;
                settings.CreditEvents = chkCredit.Checked;
                settings.ShippingOptions = chkShipping.Checked;
                settings.Documents = (plcRecountDocuments.Visible && chkDocuments.Checked);
                settings.ShippingFreeLimit = chkFreeShipping.Checked;
                settings.Orders = chkOrders.Checked;

                // Recalculates the values
                CurrencyInfoProvider.ChangeMainCurrency(editedSiteId, newCurrencyId, rate, round, settings);

                // Refresh the page
                URLHelper.Redirect(URLHelper.AddParameterToUrl(URLHelper.CurrentURL, "saved", "1"));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return;
            }
        }
        else
        {
            // Show erroe - No currency selected
            ShowError(GetString("StoreSettings_ChangeCurrency.NoNewMainCurrency"));
            return;
        }
    }


    /// <summary>
    /// Returns true when at least one checkox is checked.
    /// </summary>
    private bool RecalculationRequested()
    {
        return chkExchangeRates.Checked || (plcRecalculateFromGlobal.Visible && chkRecalculateFromGlobal.Checked) || chkProductPrices.Checked ||
               chkFlatTaxes.Checked || chkFlatDiscountsCoupons.Checked || chkFlatVolumeDiscounts.Checked || chkCredit.Checked || chkShipping.Checked ||
               (plcRecountDocuments.Visible && chkDocuments.Checked) || chkFreeShipping.Checked || chkOrders.Checked;
    }
}