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

public partial class CMSWebParts_Wireframe_Components_TagCloud: CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Text
    /// </summary>
    public string Items
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Items"), "Home\nProducts\nXYZ");
        }
        set
        {
            this.SetValue("Items", value);
            ltlText.Text = value;
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
            ltlText.ItemFormat = "<span class=\"WireframeTag\" style=\"font-size: {10-20}px\">{0}</span> ";
            ltlText.Text = Items;

            // Height
            string h = WebPartHeight;
            if (!String.IsNullOrEmpty(h))
            {
                ltlText.CssStyle += "height: " + h + ";";
            }

            // Width
            string w = WebPartWidth;
            if (!String.IsNullOrEmpty(w))
            {
                ltlText.CssStyle += "width: " + w + ";";
            }

            resElem.ResizedElementID = ltlText.ClientID;
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