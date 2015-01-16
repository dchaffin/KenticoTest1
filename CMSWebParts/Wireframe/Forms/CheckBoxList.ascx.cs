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

public partial class CMSWebParts_Wireframe_Forms_CheckBoxList : CMSAbstractWebPart
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
    /// Selected items
    /// </summary>
    public string SelectedItems
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("SelectedItems"), "");
        }
        set
        {
            this.SetValue("SelectedItems", value);
        }
    }


    /// <summary>
    /// Horizontal
    /// </summary>
    public bool Horizontal
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("Horizontal"), false);
        }
        set
        {
            this.SetValue("Horizontal", value);
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
            // Build the format
            StringBuilder sb = new StringBuilder();

            sb.Append("<input type=\"checkbox\" class=\"", this.ShortClientID, "_check\"");
            if (IsDesign)
            {
                sb.Append(" onmousedown=\"CheckListClick(this, '", this.ShortClientID, "', 'SelectedItems'); return false;\" value=\"{3}\"");
            }
            else
            {
                sb.Append(PortalHelper.GetDisabledFormElementAttributes());
            }
            sb.Append(" {1} /> {0}");

            if (Horizontal)
            {
                sb.Append("&nbsp;");
            }
            else
            {
                sb.Append("<br />");
            }

            ltlText.ItemFormat = sb.ToString();
            ltlText.Text = Items;

            ltlText.SelectedItems = SelectedItems;
            ltlText.SelectedItemText = " checked=\"checked\"";
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