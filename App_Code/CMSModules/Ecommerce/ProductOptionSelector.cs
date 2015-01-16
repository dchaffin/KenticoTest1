using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.FormControls;
using CMS.CMSHelper;

namespace CMS.Ecommerce
{
    /// <summary>
    /// Base class of the option category sku selector control.
    /// </summary>
    public class ProductOptionSelector : CMSUserControl
    {
        #region "Variables"

        private int mOptionCategoryId = 0;
        private bool mShowOptionCategoryName = true;
        private bool mShowOptionCategoryDescription = true;
        private bool mUseDefaultCurrency = false;
        private bool mShowPriceIncludingTax = false;
        private Control mSelectionControl = null;

        private OptionCategoryInfo mOptionCategory = null;
        private ShoppingCartInfo mLocalShoppingCartObj = null;

        private Hashtable mProductOptions = null;
        private InfoDataSet<SKUInfo> mProductOptionsData = null;

        private int mTextOptionSKUID = 0;

        #endregion


        #region "Public properties"

        /// <summary>
        /// SKU ID
        /// </summary>
        public int SKUID
        {
            get;
            set;
        }


        /// <summary>
        /// Option category ID
        /// </summary>
        public int OptionCategoryId
        {
            get
            {
                if ((mOptionCategoryId == 0) && (OptionCategory != null))
                {
                    mOptionCategoryId = OptionCategory.CategoryID;
                }

                return mOptionCategoryId;
            }
            set
            {
                // Force loading new selection control
                //mSelectionControl = null;

                // Force creating new option category object
                mOptionCategory = null;

                mOptionCategoryId = value;
            }
        }


        /// <summary>
        /// Indicates whether option category name should be displayed to the user.
        /// </summary>
        public bool ShowOptionCategoryName
        {
            get
            {
                return mShowOptionCategoryName;
            }
            set
            {
                mShowOptionCategoryName = value;
            }
        }


        /// <summary>
        /// Indicates whether option category description should be displayed to the user.
        /// </summary>
        public bool ShowOptionCategoryDescription
        {
            get
            {
                return mShowOptionCategoryDescription;
            }
            set
            {
                mShowOptionCategoryDescription = value;
            }
        }


        /// <summary>
        /// Selection control according to the current option category selection type.
        /// </summary>
        public Control SelectionControl
        {
            get
            {
                if (mSelectionControl == null)
                {
                    mSelectionControl = GetSelectionControl();
                }
                return mSelectionControl;
            }
        }


        /// <summary>
        /// Option category object. It is loaded from option category datarow by default, if it is not specified it is loaded by option category id.
        /// </summary>
        public OptionCategoryInfo OptionCategory
        {
            get
            {
                if (mOptionCategory == null)
                {
                    mOptionCategory = OptionCategoryInfoProvider.GetOptionCategoryInfo(mOptionCategoryId);
                }

                return mOptionCategory;
            }
            set
            {
                mOptionCategory = value;
            }
        }


        /// <summary>
        /// Shopping cart object required for price formatting. Use this property when in CMSDesk.
        /// </summary>
        public ShoppingCartInfo LocalShoppingCartObj
        {
            get
            {
                return mLocalShoppingCartObj;
            }
            set
            {
                mLocalShoppingCartObj = value;
            }
        }


        /// <summary>
        /// TRUE - default currency is used for price formatting and no discounts and taxes are applied to price, by default it is set to FALSE.
        /// </summary>
        public bool UseDefaultCurrency
        {
            get
            {
                return mUseDefaultCurrency;
            }
            set
            {
                mUseDefaultCurrency = value;
            }
        }


        /// <summary>
        /// TRUE - product option price is displayed including tax, FALSE - product option price is displayed without tax.
        /// </summary>
        public bool ShowPriceIncludingTax
        {
            get
            {
                return mShowPriceIncludingTax;
            }
            set
            {
                mShowPriceIncludingTax = value;
            }
        }

        /// <summary>
        /// SKU data of all product options of the current option category
        /// </summary>
        public Hashtable ProductOptions
        {
            get
            {
                if (mProductOptions == null)
                {
                    mProductOptions = GetProductOptions();
                }

                return mProductOptions;
            }
        }

        #endregion


        #region "Private properties"

        public int TextOptionSKUID
        {
            get
            {
                if (mTextOptionSKUID <= 0)
                {                    
                    if (!DataHelper.DataSourceIsEmpty(ProductOptionsData))
                    {                        
                        mTextOptionSKUID = ValidationHelper.GetInteger(ProductOptionsData.Items[0].SKUID, 0);
                    }
                }

                return mTextOptionSKUID;
            }
            set
            {
                mTextOptionSKUID = value;
            }
        }

        #endregion


        #region "Public methods"

        /// <summary>
        /// Reloads selection control according to the option category selection type.
        /// </summary>
        public void LoadCategorySelectionControl()
        {
            mSelectionControl = GetSelectionControl();
        }


        /// <summary>
        /// Reloads selection control data according to the option category data.
        /// </summary>
        public void ReloadData()
        {
            DebugHelper.SetContext("ProductOptionSelector");

            // Load actual product options
            LoadSKUOptions();

            // Mark default product options
            SetDefaultSKUOptions();

            DebugHelper.ReleaseContext();
        }


        /// <summary>
        /// Gets selected product options from the selection control.
        /// </summary>
        public string GetSelectedSKUOptions()
        {
            if (SelectionControl != null)
            {
                // Dropdown list, Radiobutton list - single selection
                if ((SelectionControl.GetType() == typeof(LocalizedDropDownList)) ||
                    (SelectionControl.GetType() == typeof(LocalizedRadioButtonList)))
                {
                    return ((ListControl)SelectionControl).SelectedValue;
                }
                // Checkbox list - multiple selection
                else if (SelectionControl.GetType() == typeof(LocalizedCheckBoxList))
                {
                    string result = "";
                    foreach (ListItem item in ((CheckBoxList)SelectionControl).Items)
                    {
                        if (item.Selected)
                        {
                            result += item.Value + ",";
                        }
                    }
                    return result.TrimEnd(',');
                }
                // TextBox
                else if (SelectionControl is TextBox)
                {
                    return ((TextBox)(SelectionControl)).Text;
                }
                // Form control
                else if (SelectionControl is FormEngineUserControl)
                {
                    FormEngineUserControl fc = SelectionControl as FormEngineUserControl;
                    return ValidationHelper.GetString(fc.Value, string.Empty);
                }
            }

            return null;
        }


        public List<ShoppingCartItemParameters> GetSelectedOptionsParameters()
        {
            List<ShoppingCartItemParameters> options = new List<ShoppingCartItemParameters>();
            ShoppingCartItemParameters param = null;

            if (SelectionControl != null)
            {
                // Dropdown list, Radiobutton list - single selection
                if ((SelectionControl.GetType() == typeof(LocalizedDropDownList)) ||
                    (SelectionControl.GetType() == typeof(LocalizedRadioButtonList)))
                {
                    param = new ShoppingCartItemParameters();
                    param.SKUID = ValidationHelper.GetInteger(((ListControl)SelectionControl).SelectedValue, 0);

                    if (param.SKUID > 0)
                    {
                        options.Add(param);
                    }
                }
                // Checkbox list - multiple selection
                else if (SelectionControl.GetType() == typeof(LocalizedCheckBoxList))
                {
                    foreach (ListItem item in ((CheckBoxList)SelectionControl).Items)
                    {
                        if (item.Selected)
                        {
                            param = new ShoppingCartItemParameters();
                            param.SKUID = ValidationHelper.GetInteger(item.Value, 0);

                            if (param.SKUID > 0)
                            {
                                options.Add(param);
                            }
                        }
                    }
                }
                else if (SelectionControl is TextBox)
                {
                    // Bind data
                    if (SelectionControl is TextBoxWithLabel)
                    {
                        if (TextOptionSKUID > 0)
                        {
                            param = new ShoppingCartItemParameters();
                            param.SKUID = TextOptionSKUID;
                            param.Text = ((TextBox)(SelectionControl)).Text;
                            if (param.SKUID > 0)
                            {
                                options.Add(param);
                            }
                        }
                    }
                }
                else if (SelectionControl is FormEngineUserControl)
                {
                    FormEngineUserControl fc = SelectionControl as FormEngineUserControl;

                    string val = ValidationHelper.GetString(fc.Value, string.Empty);
                    int[] optionsIds = ValidationHelper.GetIntegers(val.Split(','), 0);

                    // Add selected options to parameters
                    foreach (int id in optionsIds)
                    {
                        if (id > 0)
                        {
                            param = new ShoppingCartItemParameters();
                            param.SKUID = id;
                            options.Add(param);
                        }
                    }

                }
            }

            return options;
        }


        /// <summary>
        /// Returns TRUE when selection control is empty or only '(none)' record is included, otherwise returns FALSE.
        /// </summary>
        public bool IsSelectionControlEmpty()
        {
            if (SelectionControl != null)
            {
                // Text type
                TextBox tb = SelectionControl as TextBox;
                if (tb != null)
                {
                    return string.IsNullOrEmpty(tb.Text);
                }

                // List control types
                ListControl list = SelectionControl as ListControl;
                if (list != null)
                {
                    bool noItems = (list.Items.Count == 0);
                    bool onlyNoneRecord = ((list.Items.Count == 1) && (list.Items.FindByValue("0") != null));

                    return (noItems || onlyNoneRecord);
                }

                // Form controls
                FormEngineUserControl formControl = SelectionControl as FormEngineUserControl;
                if (formControl != null)
                {
                    if (ValidationHelper.GetInteger(formControl.Value, 1) <= 0)
                    {
                        return true;
                    }

                    return (formControl.Value == null) || string.IsNullOrEmpty(formControl.Value.ToString());
                }

                return false;
            }

            return true;
        }


        /// <summary>
        /// Returns true if there is a choice of values in selection control.
        /// </summary>
        public bool HasSelectableOptions()
        {
            if (SelectionControl != null)
            {
                // Text type or form control
                if ((SelectionControl is TextBox) || (SelectionControl is FormEngineUserControl))
                {
                    return true;
                }

                return !IsSelectionControlEmpty();
            }

            return true;
        }


        /// <summary>
        /// Validates selected/entered product option. If it is valid, returns true, otherwise returns false.
        /// </summary>        
        public virtual bool IsValid()
        {
            return true;
        }

        #endregion


        #region "Private methods"

        /// <summary>
        /// Returns initialized selection control according to the specified selection type.
        /// </summary>
        private Control GetSelectionControl()
        {
            if (OptionCategory != null)
            {
                switch (OptionCategory.CategorySelectionType)
                {
                    // Dropdownlist                     
                    case OptionCategorySelectionTypeEnum.Dropdownlist:

                        LocalizedDropDownList dropDown = new LocalizedDropDownList();
                        dropDown.ID = "dropdown";
                        dropDown.CssClass = "DropDownField";
                        dropDown.DataTextField = "SKUName";
                        dropDown.DataValueField = "SKUID";
                        dropDown.DataBound += new EventHandler(SelectionControl_DataBound);

                        return dropDown;

                    // Checkbox list with horizontal direction
                    case OptionCategorySelectionTypeEnum.CheckBoxesHorizontal:

                        LocalizedCheckBoxList boxListHorizontal = new LocalizedCheckBoxList();
                        boxListHorizontal.ID = "chkHorizontal";
                        boxListHorizontal.RepeatDirection = RepeatDirection.Horizontal;
                        boxListHorizontal.DataTextField = "SKUName";
                        boxListHorizontal.DataValueField = "SKUID";
                        boxListHorizontal.DataBound += new EventHandler(SelectionControl_DataBound);

                        return boxListHorizontal;

                    // Checkbox list with vertical direction
                    case OptionCategorySelectionTypeEnum.CheckBoxesVertical:

                        LocalizedCheckBoxList boxListVertical = new LocalizedCheckBoxList();
                        boxListVertical.ID = "chkVertical";
                        boxListVertical.RepeatDirection = RepeatDirection.Vertical;
                        boxListVertical.DataTextField = "SKUName";
                        boxListVertical.DataValueField = "SKUID";
                        boxListVertical.DataBound += new EventHandler(SelectionControl_DataBound);

                        return boxListVertical;

                    // Radiobuton list with horizontal direction
                    case OptionCategorySelectionTypeEnum.RadioButtonsHorizontal:

                        LocalizedRadioButtonList buttonListHorizontal = new LocalizedRadioButtonList();
                        buttonListHorizontal.ID = "radHorizontal";
                        buttonListHorizontal.RepeatDirection = RepeatDirection.Horizontal;
                        buttonListHorizontal.DataTextField = "SKUName";
                        buttonListHorizontal.DataValueField = "SKUID";
                        buttonListHorizontal.DataBound += new EventHandler(SelectionControl_DataBound);

                        return buttonListHorizontal;

                    // Radiobuton list with vertical direction
                    case OptionCategorySelectionTypeEnum.RadioButtonsVertical:

                        LocalizedRadioButtonList buttonListVertical = new LocalizedRadioButtonList();
                        buttonListVertical.ID = "radVertical";
                        buttonListVertical.RepeatDirection = RepeatDirection.Vertical;
                        buttonListVertical.DataTextField = "SKUName";
                        buttonListVertical.DataValueField = "SKUID";
                        buttonListVertical.DataBound += new EventHandler(SelectionControl_DataBound);

                        return buttonListVertical;

                    // Text box
                    case OptionCategorySelectionTypeEnum.TextBox:

                        TextBox text = new TextBoxWithLabel();
                        text.ID = "txtText";
                        text.CssClass = "TextBoxField";

                        return text;

                    // Text area
                    case OptionCategorySelectionTypeEnum.TextArea:

                        TextBox textarea = new TextBoxWithLabel();
                        textarea.ID = "txtArea";
                        textarea.CssClass = "TextAreaField";
                        textarea.TextMode = TextBoxMode.MultiLine;

                        return textarea;

                    // Form control
                    case OptionCategorySelectionTypeEnum.FormControl:
                        if (!string.IsNullOrEmpty(OptionCategory.CategoryFormControlName))
                        {
                            FormUserControlInfo fui = FormUserControlInfoProvider.GetFormUserControlInfo(OptionCategory.CategoryFormControlName);
                            if (fui != null)
                            {
                                // Load control
                                FormEngineUserControl fc = (FormEngineUserControl)Page.LoadUserControl(FormUserControlInfoProvider.GetFormUserControlUrl(fui));
                                if (fc != null)
                                {
                                    fc.ID = "FormControlField";

                                    return fc;
                                }
                            }
                        }

                        return null;
                }
            }

            return null;
        }


        private void SelectionControl_DataBound(object sender, EventArgs e)
        {
            if (OptionCategory.CategoryDisplayPrice)
            {
                // Check if datasource has data
                if (!DataHelper.DataSourceIsEmpty(((ListControl)sender).DataSource))
                {
                    foreach (DataRow row in ((DataSet)((ListControl)sender).DataSource).Tables[0].Rows)
                    {
                        int skuId = ValidationHelper.GetInteger(row["SKUId"], 0);
                        ListItem item = ((ListControl)sender).Items.FindByValue(skuId.ToString());

                        // Append price to option's text
                        if (item != null)
                        {
                            item.Text += GetPrice(row);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Recalculates and formats price.
        /// </summary>
        /// <param name="row">Data row to create price label for.</param>
        private string GetPrice(DataRow row)
        {
            SKUInfo sku = new SKUInfo(row);
            CurrencyInfo currency = null;
            double price = 0;

            // Use product option currency
            if (UseDefaultCurrency)
            {
                // Get site main currency
                currency = CurrencyInfoProvider.GetMainCurrency(sku.SKUSiteID);

                // Get product price
                price = sku.SKUPrice;
            }
            // Use cart currency
            else
            {
                // Get cart currency
                currency = LocalShoppingCartObj.Currency;

                // Get price in site main currency
                price = SKUInfoProvider.GetSKUPrice(sku, LocalShoppingCartObj, true, ShowPriceIncludingTax);

                // Get price in cart currency
                price = LocalShoppingCartObj.ApplyExchangeRate(price);
            }

            string preffix = (price >= 0) ? "+ " : "- ";
            price = Math.Abs(price);

            // Round price
            price = CurrencyInfoProvider.RoundTo(price, currency);

            // Format price
            string formattedPrice = CurrencyInfoProvider.GetFormattedPrice(price, currency);

            return " (" + preffix + formattedPrice + ")";
        }


        /// <summary>
        /// Sets category default options as 'Selected' in selection control.
        /// </summary>        
        private void SetDefaultSKUOptions()
        {
            if ((SelectionControl != null) && (OptionCategory != null))
            {
                // Dropdown list - single selection
                // Radiobutton list - single selection
                if ((SelectionControl is LocalizedDropDownList) ||
                    (SelectionControl is LocalizedRadioButtonList))
                {
                    try
                    {
                        ((ListControl)SelectionControl).SelectedValue = OptionCategory.CategoryDefaultOptions;
                    }
                    catch
                    {
                    }
                }
                // Checkbox list - multiple selection
                else if (SelectionControl is LocalizedCheckBoxList)
                {
                    try
                    {
                        if (OptionCategory.CategoryDefaultOptions != "")
                        {
                            foreach (string skuId in OptionCategory.CategoryDefaultOptions.Split(','))
                            {
                                // Ensure value is not empty
                                string value = (skuId != "") ? skuId : "0";

                                ListItem item = ((CheckBoxList)SelectionControl).Items.FindByValue(value);
                                if (item != null)
                                {
                                    item.Selected = true;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                // Text type
                else if (SelectionControl is TextBoxWithLabel)
                {
                    TextBoxWithLabel tb = SelectionControl as TextBoxWithLabel;
                    tb.Text = OptionCategory.CategoryDefaultOptions;
                }
                // Form control
                else if (SelectionControl is FormEngineUserControl)
                {
                    ((FormEngineUserControl)SelectionControl).Value = OptionCategory.CategoryDefaultOptions;
                }
            }
        }


        /// <summary>
        /// Dataset with the product options data which should be loaded to the product options selector.
        /// </summary>
        private InfoDataSet<SKUInfo> ProductOptionsData
        {
            get
            {
                if (mProductOptionsData == null)
                {
                    if (IsLiveSite)
                    {
                        // Get cache minutes
                        int cacheMinutes = SettingsKeyProvider.GetIntValue(CMSContext.CurrentSiteName + ".CMSCacheMinutes");

                        // Try to get data from cache
                        using (CachedSection<InfoDataSet<SKUInfo>> cs = new CachedSection<InfoDataSet<SKUInfo>>(ref mProductOptionsData, cacheMinutes, true, null, "skuoptions", SKUID, OptionCategoryId))
                        {
                            if (cs.LoadData)
                            {
                                // Get the data
                                mProductOptionsData = SKUInfoProvider.GetSKUOptions(OptionCategoryId, true);

                                // Save to the cache
                                if (cs.Cached)
                                {
                                    // Get dependencies
                                    List<string> dependencies = new List<string>();
                                    dependencies.Add("ecommerce.optioncategory|byid|" + OptionCategoryId);
                                    dependencies.Add("ecommerce.sku|all");

                                    // Set dependencies
                                    cs.CacheDependency = CacheHelper.GetCacheDependency(dependencies);
                                }

                                cs.Data = mProductOptionsData;
                            }
                        }
                    }
                    else
                    {
                        // Get the data
                        mProductOptionsData = SKUInfoProvider.GetSKUOptions(OptionCategoryId, true);
                    }
                }

                return mProductOptionsData;
            }
        }


        /// <summary>
        /// Loads data (SKU options) to the selection control.
        /// </summary>
        private void LoadSKUOptions()
        {
            // Only for none-text types
            if (SelectionControl != null)
            {
                 if (SelectionControl is TextBoxWithLabel)
                {
                    if (!DataHelper.DataSourceIsEmpty(ProductOptionsData))
                    {
                        DataRow dr = ProductOptionsData.Tables[0].Rows[0];
                        TextOptionSKUID = ValidationHelper.GetInteger(ProductOptionsData.Tables[0].Rows[0]["SKUID"], 0);

                        if (OptionCategory.CategoryDisplayPrice)
                        {
                            TextBoxWithLabel tb = SelectionControl as TextBoxWithLabel;
                            tb.LabelText = GetPrice(dr);
                        }
                    }
                }
                else if (SelectionControl is ListControl)
                {
                    ((ListControl)SelectionControl).DataSource = ProductOptionsData;
                    SelectionControl.DataBind();

                    // Add '(none)' record when it is allowed
                    if ((OptionCategory != null) && (OptionCategory.CategoryDefaultRecord != ""))
                    {
                        ListItem noneRecord = new ListItem(OptionCategory.CategoryDefaultRecord, "0");
                        ((ListControl)SelectionControl).Items.Insert(0, noneRecord);
                    }
                  }
            }
        }


        /// <summary>
        /// Returns hash table with product options.
        /// </summary>
        private Hashtable GetProductOptions()
        {            
            Hashtable optionsTable = new Hashtable();

            // Get options and load them to the hashtable
            foreach (SKUInfo sku in ProductOptionsData)
            { 
                optionsTable.Add(sku.SKUID, sku);
            }


            return optionsTable;
        }

        #endregion
    }
}