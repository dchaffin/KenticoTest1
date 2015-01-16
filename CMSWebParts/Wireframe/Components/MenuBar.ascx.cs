using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalControls;

public partial class CMSWebParts_Wireframe_Components_MenuBar : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Text
    /// </summary>
    public string Items
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Items"), "Home\nNews\nProducts\nAbout");
        }
        set
        {
            this.SetValue("Items", value);
        }
    }


    /// <summary>
    /// Selected item
    /// </summary>
    public string SelectedItem
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("SelectedItem"), "");
        }
        set
        {
            this.SetValue("SelectedItem", value);
        }
    }


    /// <summary>
    /// ItemCssClass
    /// </summary>
    public string ItemCssClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ItemCssClass"), "");
        }
        set
        {
            this.SetValue("ItemCssClass", value);
        }
    }


    /// <summary>
    /// Selected item CSS class
    /// </summary>
    public string SelectedItemCssClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("SelectedItemCssClass"), "");
        }
        set
        {
            this.SetValue("SelectedItemCssClass", value);
        }
    }

    #endregion


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
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<li class=\"", ItemCssClass," {1}\">{0}</li>");

            ltlItems.ItemFormat = sb.ToString();
            ltlItems.SelectedItemText = SelectedItemCssClass;

            ltlItems.Text = Items;
            ltlItems.SelectedItems = SelectedItem;
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
}