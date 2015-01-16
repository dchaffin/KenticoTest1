using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.Ecommerce;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.PortalEngine;
using System.Text;

public partial class CMSWebParts_Ecommerce_EcommerceSettingsChecker : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Check main currency.
    /// </summary>
    public bool MainCurrencyCheck
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("MainCurrencyCheck"), true);
        }
        set
        {
            this.SetValue("MainCurrencyCheck", value);
        }
    }


    /// <summary>
    /// Check exchange rates.
    /// </summary>
    public bool ExchangeRatesCheck
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ExchangeRatesCheck"), true);
        }
        set
        {
            this.SetValue("ExchangeRatesCheck", value);
        }
    }


    /// <summary>
    /// Check default country for taxes calculation.
    /// </summary>
    public bool DefaultCountryCheck
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("DefaultCountryCheck"), true);
        }
        set
        {
            this.SetValue("DefaultCountryCheck", value);
        }
    }


    /// <summary>
    /// Check order statuses.
    /// </summary>
    public bool OrderStatusesCheck
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("OrderStatusesCheck"), true);
        }
        set
        {
            this.SetValue("OrderStatusesCheck", value);
        }
    }


    /// <summary>
    /// Check shipping options.
    /// </summary>
    public bool ShippingMethodsCheck
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ShippingMethodsCheck"), true);
        }
        set
        {
            this.SetValue("ShippingMethodsCheck", value);
        }
    }


    /// <summary>
    /// Check payment methods.
    /// </summary>
    public bool PaymentMethodsCheck
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("PaymentMethodsCheck"), true);
        }
        set
        {
            this.SetValue("PaymentMethodsCheck", value);
        }
    }


    /// <summary>
    /// Minimize if check succeeded.
    /// </summary>
    public bool Minimize
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("Minimize"), true);
        }
        set
        {
            this.SetValue("Minimize", value);
        }
    }


    /// <summary>
    /// Check invoice template.
    /// </summary>
    public bool InvoiceTemplateCheck
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("InvoiceTemplateCheck"), true);
        }
        set
        {
            this.SetValue("InvoiceTemplateCheck", value);
        }
    }

    #endregion


    #region "Private members"

    private bool mErrorOccurred = false;
    private bool mGlobalUsage = (ECommerceSettings.AllowGlobalProducts(CMSContext.CurrentSiteName) || ECommerceSettings.AllowGlobalProductOptions(CMSContext.CurrentSiteName) || ECommerceSettings.AllowGlobalShippingOptions(CMSContext.CurrentSiteName) || ECommerceSettings.AllowGlobalDiscountLevels(CMSContext.CurrentSiteName) || ECommerceSettings.AllowGlobalDiscountCoupons(CMSContext.CurrentSiteName) || ECommerceSettings.UseGlobalCredit(CMSContext.CurrentSiteName) || ECommerceSettings.UseGlobalTaxClasses(CMSContext.CurrentSiteName));

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            CheckMainCurrency();
            CheckExchangeRates();
            CheckCurrencyRateCombination();
            CheckOrderStatuses();
            CheckPaymentMethods();
            CheckShippingMethods();
            CheckDefaultCountry();
            CheckInvoiceTemplates();

            if (PartInstance != null)
            {
                // Minimize when minize option is turned on and no errors occured
                if (Minimize && !mErrorOccurred)
                {
                    PartInstance.Minimized = true;
                }
                // If error occured maximize widget
                else if (mErrorOccurred)
                {
                    PartInstance.Minimized = false;
                }
            }

            // Show/hide error labels
            lblErrorMsg.Visible = lblSpacer.Visible = mErrorOccurred;
            lblOk.Visible = !mErrorOccurred;
            lblNa.Visible = false;

            // If all checks are turned off, show info message
            if (!MainCurrencyCheck && !ExchangeRatesCheck && !DefaultCountryCheck && !OrderStatusesCheck && !ShippingMethodsCheck && !PaymentMethodsCheck && !InvoiceTemplateCheck)
            {
                pnlError.Visible = false;
                lblNa.Visible = true;
            }
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Checks Main currency. 
    /// </summary>
    private void CheckMainCurrency()
    {
        if (MainCurrencyCheck)
        {
            CurrencyInfo ci = null;
            if (ECommerceSettings.UseGlobalCurrencies(CMSContext.CurrentSiteName))
            {
                ci = CurrencyInfoProvider.GetMainCurrency(0);
            }
            else
            {
                ci = CurrencyInfoProvider.GetMainCurrency(CMSContext.CurrentSiteID);
            }

            if (ci == null)
            {
                DisplayMessage("com.settingschecker.nomaincurrency");
                return;
            }

            // If we are using some global products, check global currency too independend on usage of global/site currencies
            if (mGlobalUsage)
            {
		        CurrencyInfo ciGlobal = CurrencyInfoProvider.GetMainCurrency(0);
                if (ciGlobal == null)
                {
                    DisplayMessage("com.settingschecker.nomaincurrency");
                    return;
                }
            }
        }
    }


    /// <summary>
    /// Checks exchange rates. 
    /// </summary>
    private void CheckExchangeRates()
    {
        if (ExchangeRatesCheck)
        {
            int currentSiteID = 0;

            // Check if the site is using global exchage rates
            if (!ECommerceSettings.UseGlobalExchangeRates(CMSContext.CurrentSiteName))
            {
                currentSiteID = CMSContext.CurrentSiteID;
            }

            // Retrieve last valid exchange table
            ExchangeTableInfo et = ExchangeTableInfoProvider.GetLastValidExchangeTableInfo(CMSContext.CurrentSiteID);
            if (et == null)
            {
                DisplayMessage("com.settingschecker.emptyexchangerate");
            }
            else
            {
                DataSet ds = CurrencyInfoProvider.GetCurrencies(currentSiteID, true);

                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    // Prepare where condition
                    StringBuilder sb = new StringBuilder();
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        sb.Append(item["CurrencyID"] + ",");
                    }
                    sb.Remove(sb.Length - 1, 1);

                    // Get exchange rate from global currency, if some global checkboxes are checked 
                    if (mGlobalUsage)
                    {
                        double exchangerateFromGlobalCurrency = ExchangeTableInfoProvider.GetLastExchangeRateFromGlobalMainCurrency(CMSContext.CurrentSiteID);
                        if (exchangerateFromGlobalCurrency <= 0)
                        {
                            DisplayMessage("com.settingschecker.emptyexchangerate");
                            return;
                        }
                    }

                    // Get all exchange rates for selected table
                    DataSet exchangeDs = ExchangeRateInfoProvider.GetExchangeRates("(ExchangeTableID = " + et.ExchangeTableID + ") AND (ExchangeRateToCurrencyID IN  (" + sb.ToString() + "))", null);
                    if (DataHelper.DataSourceIsEmpty(exchangeDs))
                    {
                        // If there is only one currency in dataset, do not show error message
                        if (ds.Tables[0].Rows.Count > 1)
                        {
                            DisplayMessage("com.settingschecker.emptyexchangerate");
                            return;
                        }

                    }
                    // Check if count of currencies is same in exchangetable 
                    else if ((ds.Tables[0].Rows.Count != exchangeDs.Tables[0].Rows.Count))
                    {
                        // If we are using global objects, there will be one more currency
                        if (mGlobalUsage)
                        {
                            if (ds.Tables[0].Rows.Count != exchangeDs.Tables[0].Rows.Count + 1)
                            {
                                DisplayMessage("com.settingschecker.emptyexchangerate");
                                return;
                            }

                        }
                        else
                        {
                            DisplayMessage("com.settingschecker.emptyexchangerate");
                            return;
                        }
                    }
                    else
                    {
                        foreach (DataRow item in exchangeDs.Tables[0].Rows)
                        {
                            if (item["ExchangeRateValue"] == null)
                            {
                                DisplayMessage("com.settingschecker.emptyexchangerate");
                                break;
                            }
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Checks currency rate combination. 
    /// </summary>
    private void CheckCurrencyRateCombination()
    {
        if (ExchangeRatesCheck)
        {
            if (ECommerceSettings.UseGlobalExchangeRates(CMSContext.CurrentSiteName) && !ECommerceSettings.UseGlobalCurrencies(CMSContext.CurrentSiteName))
            {
                DisplayMessage("com.settingschecker.wrongcurrencyratecombination");
            }
        }
    }


    /// <summary>
    /// Checks order statuses. 
    /// </summary>
    private void CheckOrderStatuses()
    {
        if (OrderStatusesCheck)
        {
            string statusSiteID = "IS NULL";

            // Check if the site is using global order statuses
            if (!ECommerceSettings.UseGlobalOrderStatus(CMSContext.CurrentSiteName))
            {
                statusSiteID = "= " + CMSContext.CurrentSiteID;
            }

            // Check if at least one order status exists
            DataSet ds = OrderStatusInfoProvider.GetOrderStatuses("(StatusEnabled = 1) AND (StatusSiteID " + statusSiteID + ")", "StatusID", 1, "StatusID");
            if (DataHelper.DataSourceIsEmpty(ds))
            {
                DisplayMessage("com.settingschecker.orderstatusnodata");
            }
        }
    }


    /// <summary>
    /// Checks payment methods. 
    /// </summary>
    private void CheckPaymentMethods()
    {
        if (PaymentMethodsCheck)
        {
            // Get all payment options
            DataSet ds = PaymentOptionInfoProvider.GetPaymentOptions("PaymentOptionEnabled = 1", "PaymentOptionID", 0, "PaymentOptionID", CMSContext.CurrentSiteID);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                // Build where condition
                StringBuilder sb = new StringBuilder();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    sb.Append(row["PaymentOptionID"] + ",");
                }
                sb.Remove(sb.Length - 1, 1);

                // Check if at least one is assigned to shipping method
                DataSet ds2 = PaymentShippingInfoProvider.GetPaymentShippings("PaymentOptionID IN (" + sb.ToString() + ")", null, 1, "PaymentOptionID");
                if (DataHelper.DataSourceIsEmpty(ds2))
                {
                    DisplayMessage("com.settingschecker.nopaymentoptionsshippingmethod");
                }
            }
            // If there is no Payment option, it shows error
            else
            {
                DisplayMessage("com.settingschecker.nopaymentoptions");
            }
        }
    }


    /// <summary>
    /// Checks shipping methods. 
    /// </summary>
    private void CheckShippingMethods()
    {
        if (ShippingMethodsCheck)
        {
            // Check if at least one shipping option exists
            DataSet ds = ShippingOptionInfoProvider.GetShippingOptions("ShippingOptionEnabled = 1", "ShippingOptionID", 1, "ShippingOptionID", CMSContext.CurrentSiteID);
            if (DataHelper.DataSourceIsEmpty(ds))
            {
                DisplayMessage("com.settingschecker.noshippingmethods");
            }
        }
    }


    /// <summary>
    /// Checks default country for taxes calculation.
    /// </summary>
    private void CheckDefaultCountry()
    {
        if (DefaultCountryCheck)
        {
            string defaultCountry = ECommerceSettings.DefaultCountryName(CMSContext.CurrentSiteName);
            if (string.IsNullOrEmpty(defaultCountry))
            {
                DisplayMessage("com.settingschecker.nodefaultcountry");
            }
        }
    }


    /// <summary>
    /// Checks invoice templates.
    /// </summary>
    private void CheckInvoiceTemplates()
    {
        if (InvoiceTemplateCheck)
        {
            string currentSite = string.Empty;

            // Check if the site is using global invoice template
            if (!ECommerceSettings.UseGlobalInvoice(CMSContext.CurrentSiteName))
            {
                currentSite = CMSContext.CurrentSiteName;
            }

            // Check invoice template
            if (string.IsNullOrEmpty(ECommerceSettings.InvoiceTemplate(currentSite)))
            {
                DisplayMessage("com.settingschecker.noinvoicetemplate");
            }
        }
    }


    /// <summary>
    /// Shows message on widget
    /// </summary>
    /// <param name="resourceString">Resource string to be shown</param>
    private void DisplayMessage(string resourceString)
    {
        pnlError.Controls.Add(new LocalizedLabel() { ResourceString = resourceString, CssClass = "ErrorLabel EditingFormLabel" });
        mErrorOccurred = true;
    }

    #endregion
}