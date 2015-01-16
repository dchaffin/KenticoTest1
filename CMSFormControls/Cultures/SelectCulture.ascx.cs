using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.FormEngine;

public partial class CMSFormControls_Cultures_SelectCulture : FormEngineUserControl
{
    #region "Variables"

    private bool mDisplayClearButton = true;
    private int mSiteID = 0;
    private string mReturnColumnName = "CultureCode";
    private bool mDisplayAllValue = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets Value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            string value = uniSelector.Value.ToString();

            if (FieldInfo.DataType == FormFieldDataTypeEnum.Text && !string.IsNullOrEmpty(value))
            {
                CultureInfo culture = CultureInfoProvider.GetCultureInfo(value);

                if (culture != null)
                {
                    return culture.CultureName;
                }

            }

            return base.ValueDisplayName;
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
            EnsureChildControls();
            base.Enabled = value;
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the textbox with culture.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            EnsureChildControls();
            return uniSelector.TextBoxSelect.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            EnsureChildControls();
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to display Clear button.
    /// </summary>
    public bool DisplayClearButton
    {
        get
        {
            return mDisplayClearButton;
        }
        set
        {
            EnsureChildControls();
            mDisplayClearButton = value;
            if (uniSelector != null)
            {
                uniSelector.AllowEmpty = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the ID of the site for which the cultures should be returned. 0 means current site. -1 all sites = all cultures.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            EnsureChildControls();
            mSiteID = value;
            if (uniSelector != null)
            {
                uniSelector.WhereCondition = GetWhereConditionInternal();
            }
        }
    }


    /// <summary>
    /// Gets current uniselector control.
    /// </summary>
    public UniSelector CurrentSelector
    {
        get
        {
            EnsureChildControls();
            return uniSelector;
        }
    }


    /// <summary>
    /// Gets drop down control (drop down list mode)
    /// </summary>
    public DropDownList SingleSelectDropDown
    {
        get
        {
            EnsureChildControls();
            return uniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets or sets column name, default value is culture name.
    /// </summary>
    public string ReturnColumnName
    {
        get
        {
            return mReturnColumnName;
        }
        set
        {
            mReturnColumnName = value;
        }
    }


    /// <summary>
    /// Gets or sets whether (all) field is in dropdownlist.
    /// </summary>
    public bool DisplayAllValue
    {
        get
        {
            return mDisplayAllValue;
        }
        set
        {
            mDisplayAllValue = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            EnsureChildControls();
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
        if (DisplayAllValue)
        {
            string[,] specialFields = new string[1, 2];
            specialFields[0, 0] = GetString("general.selectall");
            specialFields[0, 1] = "0";
            uniSelector.SpecialFields = specialFields;
        }

        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.DisplayNameFormat = "{%CultureName%} ({%CultureCode%})";

        // Set default where condition
        string where = GetWhereConditionInternal();
        if (!String.IsNullOrEmpty(where))
        {
            uniSelector.WhereCondition = where;
        }

        uniSelector.ReturnColumnName = ReturnColumnName;
        uniSelector.AllowEmpty = DisplayClearButton;
    }


    public void ReloadUnigridData()
    {
        ReloadData();
        uniSelector.Reload(true);
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // If macro or special value, do not validate
        string value = uniSelector.TextBoxSelect.Text.Trim();
        if (!ContextResolver.ContainsMacro(value) && (value != "") && (value != TreeProvider.ALL_CULTURES))
        {
            // Check if culture exists
            CultureInfo ci = CultureInfoProvider.GetCultureInfo(value);
            if (ci == null)
            {
                ValidationError = GetString("formcontrols_selectculture.notexist").Replace("%%code%%", value);
                return false;
            }
            else
            {
                return true;
            }
        }
        return true;
    }


    /// <summary>
    /// Returns WHERE condition for given site.
    /// </summary>
    private string GetWhereConditionInternal()
    {
        // If site id is -1 return all cultures
        if (SiteID < 0)
        {
            return string.Empty;
        }

        string retval = "CultureID IN (SELECT CultureID FROM CMS_SiteCulture WHERE SiteID = ";
        if (SiteID > 0)
        {
            retval += SiteID;
        }
        else
        {
            retval += CMSContext.CurrentSiteID;
        }
        retval += ")";

        return retval;
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load updat panel container
        if (uniSelector == null)
        {
            pnlUpdate.LoadContainer();
        }
        // Call base method
        base.CreateChildControls();
    }
}