using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.Ecommerce;
using CMS.GlobalHelper;

public partial class CMSModules_Ecommerce_Controls_UI_ProductOptions_SelectOptionCategoryType : CMSUserControl
{
    #region "Events"

    public event EventHandler OnSelectionChanged;

    #endregion


    #region "Properties"

    /// <summary>
    /// Selected type of category
    /// </summary>
    public OptionCategoryTypeEnum CategoryType
    {
        get
        {
            return OptionCategoryInfoProvider.GetOptionCategoryTypeEnum(drpTypes.SelectedValue);
        }
        set
        {
            ListItem item = drpTypes.Items.FindByValue(ValidationHelper.GetString(value, ""));
            if (item != null)
            {
                item.Selected = true;                
            }
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Load data
        if (!URLHelper.IsPostback())
        {
            ReloadData();
        }
    }
                                                                                                                                                                                                    

    /// <summary>
    /// Reloads the list of category types.
    /// </summary>
    private void ReloadData()
    {
        // Clear items
        drpTypes.Items.Clear();

        // Add items
        AddItem("com.optioncategorytype.selectableproducts", OptionCategoryTypeEnum.OptionalProducts);
        AddItem("com.optioncategorytype.selectableattribute", OptionCategoryTypeEnum.Attribute);
        AddItem("com.optioncategorytype.textattribute", OptionCategoryTypeEnum.Text);        
    }


    /// <summary>
    ///  Adds specific type of category to the list.
    /// </summary>
    /// <param name="resString">Category display name represented by the resource string</param>
    /// <param name="categType">Category type</param>
    private void AddItem(string resString, OptionCategoryTypeEnum categType)
    {
        drpTypes.Items.Add(new ListItem(this.GetString(resString), OptionCategoryInfoProvider.GetOptionCategoryTypeString(categType)));
    }


    /// <summary>
    /// Handles change of selected type
    /// </summary>
    protected void drpTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (OnSelectionChanged != null)
        {
            OnSelectionChanged(sender, e);
        }
    }

    #endregion
}
