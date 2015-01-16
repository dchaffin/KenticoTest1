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

public partial class CMSWebParts_Wireframe_Forms_TextBox: CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Text
    /// </summary>
    public string Text
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Text"), "");
        }
        set
        {
            this.SetValue("Text", value);
        }
    }


    /// <summary>
    /// Image URL
    /// </summary>
    public string ImageUrl
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ImageUrl"), "");
        }
        set
        {
            this.SetValue("ImageUrl", value);
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
            ltlText.Text = Text;

            // Width
            string w = WebPartWidth;
            if (!String.IsNullOrEmpty(w))
            {
                ltlText.Style += String.Format("width: {0};", w);
            }

            string imgUrl = ImageUrl;
            if (!String.IsNullOrEmpty(imgUrl))
            {
                imgElem.ImageUrl = UIHelper.GetImageUrl(this.Page, imgUrl);
                imgElem.Visible = true;
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