using System;
using System.Web.UI.WebControls;

using CMS.Ecommerce;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;

public partial class CMSModules_Ecommerce_FormControls_SelectProductType : FormEngineUserControl
{
    private bool mAutoPostback = false;
    private string mValue = null;
    
    
    #region "Properties - general"
    
    /// <summary>
    /// Gets or sets enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            drpProductType.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating whether a postback to the server automatically occurs when the selection is changed by the user.
    /// </summary>
    public bool AutoPostBack
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoPostBack"), mAutoPostback);
        }
        set
        {
            SetValue("AutoPostBack", value);
            mAutoPostback = value;
            drpProductType.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Gets or sets the selected product type as a string.
    /// </summary>
    public override object Value
    {
        get
        {
            return mValue;
        }
        set
        {
            mValue = ValidationHelper.GetString(value, null);
            drpProductType.SelectedValue = mValue;
        }
    }


    /// <summary>
    /// Gets a value that indicates if price is being edited for product option.
    /// </summary>
    public bool IsProductOption
    {
        get
        {
            // Try to get the value from SKU form data
            if ((Form != null) && Form.Data.ContainsColumn("SKUOptionCategoryID"))
            {
                return ValidationHelper.GetInteger(Form.Data.GetValue("SKUOptionCategoryID"), 0) > 0;
            }

            return false;
        }
    }


    public bool IsTextProductOption
    {
        get
        {
            // Try to get the value from SKU form data
            if ((Form != null) && Form.Data.ContainsColumn("SKUOptionCategoryID"))
            {
                var optionCategoryId = ValidationHelper.GetInteger(Form.Data.GetValue("SKUOptionCategoryID"), 0);
                var optionCategory = OptionCategoryInfoProvider.GetOptionCategoryInfo(optionCategoryId);
                if (optionCategory != null)
                {
                    return optionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.TextBox || optionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.TextArea;
                }
            }

            return false;
        }
    }

    #endregion


    #region "Properties - options"

    /// <summary>
    /// Gets or sets a value that indicates if "all" option is available for selection.
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowAll"), mAllowAll);
        }
        set
        {
            SetValue("AllowAll", value);
            mAllowAll = value;
        }
    }
    private bool mAllowAll = false;


    /// <summary>
    /// Gets or sets the text for "all" option.
    /// </summary>
    public string AllItemResourceString
    {
        get
        {
            string text = ValidationHelper.GetString(GetValue("AllItemResourceString"), mAllItemResourceString);
            return text ?? "general.selectall";
        }
        set
        {
            SetValue("AllItemResourceString", value);
            mAllItemResourceString = value;
        }
    }
    private string mAllItemResourceString = null;


    /// <summary>
    /// Gets or sets a value that indicates if "none" option is available for selection.
    /// </summary>
    public bool AllowNone
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowNone"), mAllowNone);
        }
        set
        {
            SetValue("AllowNone", value);
            mAllowNone = value;
        }
    }
    private bool mAllowNone = false;


    /// <summary>
    /// Gets or sets the text for "none" option.
    /// </summary>
    public string NoneItemResourceString
    {
        get
        {
            string text = ValidationHelper.GetString(GetValue("NoneItemResourceString"), mNoneItemResourceString);
            return text ?? "general.selectnone";
        }
        set
        {
            SetValue("NoneItemResourceString", value);
            mNoneItemResourceString = value;
        }
    }
    private string mNoneItemResourceString = null;


    /// <summary>
    /// Indicates if standard product type option is available for selection.
    /// </summary>
    public bool AllowStandardProduct
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowStandardProduct"), mAllowStandardProduct);
        }
        set
        {
            SetValue("AllowStandardProduct", value);
            mAllowStandardProduct = value;
        }
    }
    private bool mAllowStandardProduct = true;


    /// <summary>
    /// Indicates if membership product type option is available for selection.
    /// </summary>
    public bool AllowMembership
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowMembership"), mAllowMembership);
        }
        set
        {
            SetValue("AllowMembership", value);
            mAllowMembership = value;
        }
    }
    private bool mAllowMembership = true;


    /// <summary>
    /// Indicates if e-product product type option is available for selection.
    /// </summary>
    public bool AllowEproduct
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEproduct"), mAllowEproduct);
        }
        set
        {
            SetValue("AllowEproduct", value);
            mAllowEproduct = value;
        }
    }
    private bool mAllowEproduct = true;


    /// <summary>
    /// Indicates if donation product type option is available for selection.
    /// </summary>
    public bool AllowDonation
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowDonation"), mAllowDonation);
        }
        set
        {
            SetValue("AllowDonation", value);
            mAllowDonation = value;
        }
    }
    private bool mAllowDonation = true;


    /// <summary>
    /// Indicates if bundle product type option is available for selection.
    /// </summary>
    public bool AllowBundle
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowBundle"), mAllowBundle);
        }
        set
        {
            SetValue("AllowBundle", value);
            mAllowBundle = value;
        }
    }
    private bool mAllowBundle = true;


    /// <summary>
    /// Indicates if text product type option is available for selection.
    /// </summary>
    public bool AllowText
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowText"), mAllowText);
        }
        set
        {
            SetValue("AllowText", value);
            mAllowText = value;
        }
    }
    private bool mAllowText = false;

    #endregion


    #region "Events"

    /// <summary>
    /// Selection changed event.
    /// </summary>
    public event EventHandler SelectionChanged;


    private void RaiseSelectionChanged()
    {
        if (SelectionChanged != null)
        {
            SelectionChanged(this, EventArgs.Empty);
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (drpProductType.Items.Count == 0)
        {
            InitOptions();
            TryPreselectByForm();
            drpProductType.SelectedValue = ValidationHelper.GetString(Value, null);
        }
        else
        {
            Value = drpProductType.SelectedValue;
        }

        drpProductType.Enabled = Enabled;
        drpProductType.AutoPostBack = AutoPostBack;

        drpProductType.SelectedIndexChanged += (sender, args) =>
        {
            Value = drpProductType.SelectedValue;
            RaiseSelectionChanged();
        };
    }

    #endregion


    #region "Initialization"

    /// <summary>
    /// Initializes the selector options.
    /// </summary>
    public void InitOptions()
    {
        if (AllowNone && !IsTextProductOption)
        {
            AddStringOption(NoneItemResourceString, "NONE");
        }

        if (AllowAll && !IsTextProductOption)
        {
            AddStringOption(AllItemResourceString, "ALL");
        }

        if (AllowStandardProduct && !IsTextProductOption)
        {
            AddProductTypeOption("com.producttype.product", SKUProductTypeEnum.Product);
        }

        // Add membership option when membership feature is available and membership option is allowed
        if (LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.Membership) && AllowMembership && !IsTextProductOption)
        {
            AddProductTypeOption("com.producttype.membership", SKUProductTypeEnum.Membership);
        }

        if (AllowEproduct && !IsTextProductOption)
        {
            AddProductTypeOption("com.producttype.eproduct", SKUProductTypeEnum.EProduct);
        }

        if (AllowDonation && !IsProductOption && !IsTextProductOption)
        {
            AddProductTypeOption("com.producttype.donation", SKUProductTypeEnum.Donation);
        }

        if (AllowBundle && !IsProductOption && !IsTextProductOption)
        {
            AddProductTypeOption("com.producttype.bundle", SKUProductTypeEnum.Bundle);
        }

        if (AllowText && IsTextProductOption)
        {
            AddProductTypeOption("com.producttype.text", SKUProductTypeEnum.Text);
            return;
        }
    }


    private void AddStringOption(string text, string value)
    {
        drpProductType.Items.Add(new ListItem(GetString(text), value));
    }


    private void AddProductTypeOption(string text, SKUProductTypeEnum value)
    {
        AddStringOption(text, SKUInfoProvider.GetSKUProductTypeString(value));
    }


    private void TryPreselectByForm()
    {
        if (Form == null)
        {
            return;
        }

        if (Form.AdditionalData.ContainsKey("DataClassID"))
        {
            var dataClassId = ValidationHelper.GetInteger(Form.AdditionalData["DataClassID"], 0);
            var dataClass = DataClassInfoProvider.GetDataClass(dataClassId);
            if (dataClass != null)
            {
                Value = dataClass.ClassSKUDefaultProductType;
            }
        }
    }

    #endregion
}