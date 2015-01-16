using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;

using CMS.Ecommerce;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;

namespace CMS.Controls
{
    /// <summary>
    /// SKU transformation methods
    /// </summary>
    public partial class CMSTransformation
    {
        #region "Variables"

        SKUInfo mSKU = null;

        #endregion


        #region "Properties"

        /// <summary>
        /// SKU data loaded from the current data row
        /// </summary>
        public SKUInfo SKU
        {
            get
            {
                if (mSKU == null)
                {
                    // Get SKU object
                    DataRow row = (DataRowView != null) ? DataRowView.Row : null;
                    if (row != null)
                    {
                        #region "Smart search data row"
                        
                        // Try get data row value from search result
                        DataColumnCollection col =  row.Table.Columns;
                        if (col.Contains("id") && (col.Contains("score")))
                        {
                            // try get value
                            string id = ValidationHelper.GetString(Eval("id"), String.Empty);
                            if (!String.IsNullOrEmpty(id))
                            {
                                // Try get data row
                                Hashtable resultRows = RequestStockHelper.GetItem(SearchHelper.RESULTS_KEY) as Hashtable;
                                if (resultRows != null)
                                {
                                    row = resultRows[id] as DataRow;
                                }
                            }
                        }

                        #endregion

                        mSKU = new SKUInfo(row);
                    }
                }

                return mSKU;
            }
        }


        /// <summary>
        /// Indicates if product prices are displayed including discounts
        /// </summary>
        public bool PriceIncludingDiscounts
        {
            get
            {
                return ECommerceSettings.DisplayPriceIncludingDiscounts(CMSContext.CurrentSiteName);
            }
        }


        /// <summary>
        /// Indicates if product prices are displayed including taxes
        /// </summary>
        public bool PriceIncludingTaxes
        {
            get
            {
                return ECommerceSettings.DisplayPriceIncludingTaxes(CMSContext.CurrentSiteName);
            }
        }

        #endregion


        #region "SKU price"

        /// <summary>
        /// Returns SKU catalogue price based on the SKU data and the data of the current shopping cart. 
        /// Catalogue discounts and/or taxes are included optionally.
        /// </summary>
        /// <param name="discounts">True - catalogue discounts are applied to the price</param>
        /// <param name="taxes">True - catalogue taxes are applied to the discounted price</param>
        /// <param name="column">SKU column from which the price should be returned. If empty, SKUPrice column is used.</param>
        public double GetSKUPrice(bool discounts, bool taxes, string column)
        {
            return ApplyExchangeRate(SKUInfoProvider.GetSKUPrice(SKU, null, discounts, taxes, false, column));
        }


        /// <summary>
        /// Returns SKU catalogue price based on the SKU data and the data of the current shopping cart. 
        /// Catalogue discounts and/or taxes are included optionally.
        /// </summary>
        /// <param name="discounts">True - catalogue discounts are applied to the price</param>
        /// <param name="taxes">True - catalogue taxes are applied to the discounted price</param>
        public double GetSKUPrice(bool discounts, bool taxes)
        {
            return GetSKUPrice(discounts, taxes, null);
        }


        /// <summary>
        /// Returns SKU catalogue price based on the SKU data and the data of the current shopping cart. 
        /// Catalogue discounts and/or taxes are included based on the site settings.
        /// </summary>
        /// <param name="column">SKU column from which the price should be returned. If empty, SKUPrice column is used.</param>
        public double GetSKUPrice(string column)
        {
            return GetSKUPrice(PriceIncludingDiscounts, PriceIncludingTaxes, column);
        }


        /// <summary>
        /// Returns SKU catalogue price based on the SKU data and the data of the current shopping cart. 
        /// Catalogue discounts and/or taxes are included based on the site settings.
        /// </summary>
        public double GetSKUPrice()
        {
            return GetSKUPrice(PriceIncludingDiscounts, PriceIncludingTaxes, null);
        }


        /// <summary>
        /// Returns formatted SKU catalogue price based on the SKU data and the data of the current shopping cart. 
        /// Catalogue discounts and/or taxes are included optionally.
        /// </summary>
        /// <param name="discounts">True - catalogue discounts are applied to the price</param>
        /// <param name="taxes">True - catalogue taxes are applied to the discounted price</param>
        /// <param name="column">SKU column from which the price should be returned. If empty, SKUPrice column is used.</param>
        public string GetSKUFormattedPrice(bool discounts, bool taxes, string column)
        {
            return SKUInfoProvider.GetSKUFormattedPrice(SKU, null, discounts, taxes, column);
        }


        /// <summary>
        /// Returns formatted SKU catalogue price based on the SKU data and the data of the current shopping cart. 
        /// Catalogue discounts and/or taxes are included optionally.
        /// </summary>
        /// <param name="discounts">True - catalogue discounts are applied to the price</param>
        /// <param name="taxes">True - catalogue taxes are applied to the discounted price</param>
        public string GetSKUFormattedPrice(bool discounts, bool taxes)
        {
            return GetSKUFormattedPrice(discounts, taxes, null);
        }


        /// <summary>
        /// Returns formatted product catalogue price based on the SKU data and the data of the current shopping cart. 
        /// Catalogue discounts and/or taxes are included based on the site settings.
        /// </summary>
        /// <param name="column">SKU column from which the price should be returned. If empty, SKUPrice column is used.</param>
        public string GetSKUFormattedPrice(string column)
        {
            return GetSKUFormattedPrice(PriceIncludingDiscounts, PriceIncludingTaxes, column);
        }


        /// <summary>
        /// Returns formatted product catalogue price based on the SKU data and the data of the current shopping cart. 
        /// Catalogue discounts and/or taxes are included based on the site settings.
        /// </summary>
        public string GetSKUFormattedPrice()
        {
            return GetSKUFormattedPrice(null);
        }

        #endregion


        #region "SKU list price"

        /// <summary>
        /// Returns SKU list price based on the SKU data and the data of the current shopping cart. 
        /// Catalogue discounts are not included, taxes are included based on the site settings.
        /// </summary>
        public double GetSKUListPrice()
        {
            return GetSKUListPrice(PriceIncludingTaxes);
        }


        /// <summary>
        /// Returns SKU list price based on the SKU data and the data of the current shopping cart. 
        /// Catalogue discounts are not included, taxes are included optionally.
        /// </summary>
        public double GetSKUListPrice(bool taxes)
        {
            return GetSKUPrice(false, taxes, "SKURetailPrice");
        }


        /// <summary>
        /// Returns formatted SKU list price based on the SKU data and the data of the current shopping cart. 
        /// Catalogue discounts are not included, taxes are included based on the site settings.
        /// </summary>
        public string GetSKUFormattedListPrice()
        {
            return GetSKUFormattedListPrice(PriceIncludingTaxes);
        }


        /// <summary>
        /// Returns formatted SKU list price based on the SKU data and the data of the current shopping cart. 
        /// Catalogue discounts are not included, taxes are included optionally.
        /// </summary>
        /// <param name="taxes">True - catalogue taxes are applied to the list price</param>
        public string GetSKUFormattedListPrice(bool taxes)
        {
            return GetSKUFormattedPrice(false, taxes, "SKURetailPrice");
        }

        #endregion


        #region "SKU price saving"

        /// <summary>
        /// Returns amount of saved money based on the difference between product seller price and product list price.
        /// </summary> 
        /// <param name="discounts">Indicates if discounts should be applied to the seller price before the saved amount is calculated</param>
        /// <param name="taxes">Indicates if taxes should be applied to both list price and seller price before the saved amount is calculated</param>
        /// <param name="column1">Name of the column from which the seller price is retrieved, if empty SKUPrice column is used</param>
        /// <param name="column2">Name of the column from which the list price is retrieved, if empty SKURetailPrice column is used</param>
        /// <param name="percentage">True - result is percentage, False - result is in the current currency</param>
        public double GetSKUPriceSaving(bool discounts, bool taxes, string column1, string column2, bool percentage)
        {
            return EcommerceFunctions.GetSKUPriceSaving(SKU, discounts, taxes, column1, column2, percentage);
        }


        /// <summary>
        /// Returns amount of saved money based on the difference between product seller price and product list price.
        /// </summary> 
        /// <param name="discounts">Indicates if discounts should be applied to the seller price before the saved amount is calculated</param>
        /// <param name="taxes">Indicates if taxes should be applied to both list price and seller price before the saved amount is calculated</param>
        /// <param name="percentage">True - result is percentage, False - result is in the current currency</param>
        public double GetSKUPriceSaving(bool discounts, bool taxes, bool percentage)
        {
            return GetSKUPriceSaving(discounts, taxes, null, null, percentage);
        }


        /// <summary>
        /// Returns amount of saved money based on the difference between product seller price and product list price.
        /// Catalogue discounts and/or taxes are included based on the site settings.
        /// </summary> 
        /// <param name="column1">Name of the column from which the seller price is retrieved, if empty SKUPrice column is used</param>
        /// <param name="column2">Name of the column from which the list price is retrieved, if empty SKURetailPrice column is used</param>
        /// <param name="percentage">True - result is percentage, False - result is in the current currency</param>
        public double GetSKUPriceSaving(string column1, string column2, bool percentage)
        {
            return GetSKUPriceSaving(PriceIncludingDiscounts, PriceIncludingTaxes, column1, column2, percentage);
        }


        /// <summary>
        /// Returns amount of saved money based on the difference between product seller price and product list price.
        /// Catalogue discounts and/or taxes are included based on the site settings.
        /// </summary> 
        /// <param name="percentage">True - result is percentage, False - result is in the current currency</param>
        public double GetSKUPriceSaving(bool percentage)
        {
            return GetSKUPriceSaving(null, null, percentage);
        }


        /// <summary>
        /// Returns formatted string representing amount of saved money based on the difference between product seller price and product list price.
        /// </summary> 
        /// <param name="discounts">Indicates if discounts should be applied to the seller price before the saved amount is calculated</param>
        /// <param name="taxes">Indicates if taxes should be applied to both list price and seller price before the saved amount is calculated</param>
        /// <param name="column1">Name of the column from which the seller price is retrieved, if empty SKUPrice column is used</param>
        /// <param name="column2">Name of the column from which the list price is retrieved, if empty SKURetailPrice column is used</param>
        public string GetSKUFormattedPriceSaving(bool discounts, bool taxes, string column1, string column2)
        {
            return FormatPrice(GetSKUPriceSaving(discounts, taxes, column1, column2, false));
        }


        /// <summary>
        /// Returns formatted string representing amount of saved money based on the difference between product seller price and product list price.
        /// </summary> 
        /// <param name="discounts">Indicates if discounts should be applied to the seller price before the saved amount is calculated</param>
        /// <param name="taxes">Indicates if taxes should be applied to both list price and seller price before the saved amount is calculated</param>
        public string GetSKUFormattedPriceSaving(bool discounts, bool taxes)
        {
            return GetSKUFormattedPriceSaving(discounts, taxes, null, null);
        }


        /// <summary>
        /// Returns formatted string representing amount of saved money based on the difference between product seller price and product list price.
        /// Catalogue discounts and/or taxes are included based on the site settings.
        /// </summary> 
        /// <param name="column1">Name of the column from which the seller price is retrieved, if empty SKUPrice column is used</param>
        /// <param name="column2">Name of the column from which the list price is retrieved, if empty SKURetailPrice column is used</param>
        public string GetSKUFormattedPriceSaving(string column1, string column2)
        {
            return GetSKUFormattedPriceSaving(PriceIncludingDiscounts, PriceIncludingTaxes, column1, column2);
        }


        /// <summary>
        /// Returns formatted string representing amount of saved money based on the difference between product seller price and product list price.
        /// Catalogue discounts and/or taxes are included based on the site settings.
        /// </summary> 
        public string GetSKUFormattedPriceSaving()
        {
            return GetSKUFormattedPriceSaving(null, null);
        } 

        #endregion


        #region "SKU price formatting"

        /// <summary>
        /// Applies current exchange rate to the given price and returns the result.
        /// </summary>
        /// <param name="price">Price in site main currency the current exchange rate should be applied to</param>        
        public double ApplyExchangeRate(double price)
        {
            return ECommerceContext.CurrentShoppingCart.ApplyExchangeRate(price);
        }


        /// <summary>
        /// Returns price rounded and formatted according to the current currency properties.
        /// </summary>
        /// <param name="price">Price to be formatted</param> 
        /// <param name="round">True - price is rounded according to the current currency settings before formatting</param>
        public string FormatPrice(double price, bool round)
        {
            return ECommerceContext.CurrentShoppingCart.GetFormattedPrice(price, round);
        }


        /// <summary>
        /// Returns price rounded and formatted according to the current currency properties.
        /// </summary>
        /// <param name="price">Price to be formatted</param>        
        public string FormatPrice(double price)
        {
            return FormatPrice(price, true);
        }

        #endregion


        #region "SKU properties"

        /// <summary>
        /// Returns value of the specified product public status column.
        /// If the product is evaluated as a new product in the store, public status set by 'CMSStoreNewProductStatus' setting is used, otherwise product public status is used.
        /// </summary>
        /// <param name="column">Name of the product public status column the value should be retrieved from</param>
        public object GetSKUIndicatorProperty(string column)
        {
            return EcommerceFunctions.GetSKUIndicatorProperty(SKU, column);
        }


        /// <summary>
        /// Indicates if the given SKU can be bought by the customer based on the SKU inventory properties.
        /// </summary>       
        public bool IsSKUAvailableForSale()
        {
            return SKUInfoProvider.IsSKUAvailableForSale(SKU);                
        }

        #endregion


        #region "SKU URLs"

        /// <summary>
        /// Returns SKU permanent URL.
        /// </summary>        
        public string GetSKUUrl()
        {
            return EcommerceFunctions.GetProductUrl(Eval("SKUGuid"), EvalText("SKUName", true));
        }


        /// <summary>
        /// Returns SKU image URL including dimension's modifiers (width, height) and site name parameter if product is from different site than current. 
        /// If image URL is not specified, SKU default image URL is used.
        /// </summary>
        /// <param name="width">Image requested width</param>
        /// <param name="height">Image requested height</param>      
        public string GetSKUImageUrl(int width, int height)
        {
            return EcommerceFunctions.GetSKUImageUrl(Eval("SKUImagePath"), width, height, 0, Eval("SKUSiteID"));
        }


        /// <summary>
        /// Returns SKU image URL including dimension's modifiers (width, height) and site name parameter if product is from different site than current. 
        /// If image URL is not specified, SKU default image URL is used.
        /// </summary>
        /// <param name="maxsidesize">Image requested maximum side size</param>        
        public string GetSKUImageUrl(int maxSideSize)
        {
            return EcommerceFunctions.GetSKUImageUrl(Eval("SKUImagePath"), 0, 0, maxSideSize, Eval("SKUSiteID"));
        }

        #endregion
    }
}
