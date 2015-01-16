using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSModules_Ecommerce_FormControls_AddressSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mAddNewRecord = false;
    private bool mAddNoneRecord = true;
    private bool mShowBilling = true;
    private bool mShowShipping = true;
    private bool mShowCompany = true;
    private bool mShowOnlyEnabled = true;
    private bool mShowAll = false;
    private bool mRenderInline = false;
    private int mCustomerId = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the ID of the customer the addresses of which should be displayed.
    /// </summary>
    public int CustomerID
    {
        get
        {
            return mCustomerId;
        }
        set
        {
            mCustomerId = value;
        }
    }


    /// <summary>
    /// Indicates whether to render update panel in inline mode.
    /// </summary>
    public bool RenderInline
    {
        get
        {
            return mRenderInline;
        }
        set
        {
            mRenderInline = value;
        }
    }


    /// <summary>
    /// Indicates whether to display all addresses - if true all other settings (ShowOnlyEnabled, ShowBilling, ShowShipping) are ignored.
    /// </summary>
    public bool ShowAll
    {
        get
        {
            return mShowAll;
        }
        set
        {
            mShowAll = value;
        }
    }


    /// <summary>
    /// Indicates whether to display only enabled addresses.
    /// </summary>
    public bool ShowOnlyEnabled
    {
        get
        {
            return mShowOnlyEnabled;
        }
        set
        {
            mShowOnlyEnabled = value;
        }
    }


    /// <summary>
    /// Indicates whether to display billing addresses.
    /// </summary>
    public bool ShowBilling
    {
        get
        {
            return mShowBilling;
        }
        set
        {
            mShowBilling = value;
        }
    }

    /// <summary>
    /// Indicates whether to shipping billing addresses.
    /// </summary>
    public bool ShowShipping
    {
        get
        {
            return mShowShipping;
        }
        set
        {
            mShowShipping = value;
        }
    }


    /// <summary>
    /// Indicates whether to display company addresses.
    /// </summary>
    public bool ShowCompany
    {
        get
        {
            return mShowCompany;
        }
        set
        {
            mShowCompany = value;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return AddressID;
        }
        set
        {
            AddressID = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// Gets or sets the Address ID.
    /// </summary>
    public int AddressID
    {
        get
        {
            return ValidationHelper.GetInteger(uniSelector.Value, 0);
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add new item record to the dropdownlist.
    /// </summary>
    public bool AddNewRecord
    {
        get
        {
            return mAddNewRecord;
        }
        set
        {
            mAddNewRecord = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add none item record to the dropdownlist.
    /// </summary>
    public bool AddNoneRecord
    {
        get
        {
            return mAddNoneRecord;
        }
        set
        {
            mAddNoneRecord = value;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
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
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the dropdownlist.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.DropDownSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Returns inner DropDown control.
    /// </summary>
    public DropDownList DropDownSingleSelect
    {
        get
        {
            return uniSelector.DropDownSingleSelect;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        if (RenderInline)
        {
            pnlUpdate.RenderMode = UpdatePanelRenderMode.Inline;
        }

        uniSelector.EnabledColumnName = "AddressEnabled";
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.AllowEmpty = AddNoneRecord;

        string where = "AddressCustomerID = " + CustomerID;

        if (!ShowAll)
        {
            if (ShowOnlyEnabled)
            {
                where += " AND AddressEnabled = 1";
            }
            if (!ShowBilling || !ShowShipping)
            {
                if (ShowBilling)
                {
                    where += " AND AddressIsBilling = 1";
                }
                if (ShowShipping)
                {
                    where += " AND AddressIsShipping = 1";
                }
                if (ShowCompany)
                {
                    where += " AND AddressIsCompany = 1";
                }
            }
        }

        // Include selected value
        if (AddressID > 0)
        {
            where = SqlHelperClass.AddWhereCondition(where, "AddressID = " + AddressID, "OR");
        }

        uniSelector.WhereCondition = where;

        if (AddNewRecord)
        {
            uniSelector.SpecialFields = new string[,] { { GetString("shoppingcartorderaddresses.newaddress"), "0" } };
        }
    }
}