using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalControls;

public partial class CMSWebParts_Wireframe_Forms_DropDown: CMSAbstractWebPart
{
   #region "Properties"

    /// <summary>
    /// Items
    /// </summary>
    public string Items
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Items"), "Item 1\nItem 2\nItem 3");
        }
        set
        {
            this.SetValue("Items", value);
        }
    }


    /// <summary>
    /// Selected item index
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
            ltlText.ItemFormat = "<option value=\"{3}\">{4}</option>";
            ltlText.Text = Items;

            // Width
            string w = WebPartWidth;
            if (!String.IsNullOrEmpty(w))
            {
                ltlText.CssStyle = "width: " + w + ";";
            }

            if (IsDesign)
            {
                ltlText.Attributes += String.Format("onchange=\"SetWebPartProperty('{0}', 'SelectedItem', this.selectedIndex + '')\"", this.ShortClientID);
            }

            resElem.ResizedElementID = ltlText.ClientID;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Register init script
        string script = "document.getElementById('" + ltlText.ClientID + "').selectedIndex = " + this.SelectedItem;

        ScriptHelper.RegisterStartupScript(this, typeof(string), ltlText.ClientID + "_Init", ScriptHelper.GetScript(script));
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