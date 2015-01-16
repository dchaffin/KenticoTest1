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

public partial class CMSWebParts_Wireframe_Components_Grid: CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Data
    /// </summary>
    public string Data
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Data"), "");
        }
        set
        {
            this.SetValue("Data", value);
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
            tbElem.Text = Data;

            tbElem.ItemFormat = "<tr class=\"{2}\">{0}</tr>";
            tbElem.Attributes += " cellspacing=\"0\"";

            // Height
            string h = WebPartHeight;
            if (!String.IsNullOrEmpty(h))
            {
                tbElem.CssStyle += "height: " + h + ";";
            }

            // Width
            string w = WebPartWidth;
            if (!String.IsNullOrEmpty(w))
            {
                tbElem.CssStyle += "width: " + w + ";";
            }

            resElem.ResizedElementID = tbElem.ClientID;

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