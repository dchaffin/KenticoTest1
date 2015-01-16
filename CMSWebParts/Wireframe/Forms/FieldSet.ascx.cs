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

public partial class CMSWebParts_Wireframe_Forms_FieldSet: CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Title
    /// </summary>
    public string Title
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Title"), "Group name");
        }
        set
        {
            this.SetValue("Title", value);
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
            ltlText.Text = Title;

            // Height
            string h = WebPartHeight;
            if (!String.IsNullOrEmpty(h))
            {
                pnlElem.Height = new Unit(h);
            }

            // Width
            string w = WebPartWidth;
            if (!String.IsNullOrEmpty(w))
            {
                pnlElem.Width = new Unit(w);
            }

            resElem.ResizedElementID = pnlElem.ClientID;
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