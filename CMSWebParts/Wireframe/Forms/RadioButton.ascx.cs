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

public partial class CMSWebParts_Wireframe_Forms_RadioButton: CMSAbstractWebPart
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
    /// Checked
    /// </summary>
    public bool Checked
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("Checked"), false);
        }
        set
        {
            this.SetValue("Checked", value);
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
            // Build the radiobutton
            StringBuilder sb = new StringBuilder();

            sb.Append("<input type=\"radio\"");
            if (IsDesign)
            {
                sb.Append(" onmousedown=\"this.newChecked = this.checked = !this.checked; SetWebPartProperty('", this.ShortClientID, "', 'Checked', this.checked + ''); return false;\" onchange=\"this.checked = this.newChecked;\"");
            }
            else
            {
                sb.Append(PortalHelper.GetDisabledFormElementAttributes());
            }
            if (Checked)
            {
                sb.Append(" checked=\"checked\"");
            }
            sb.Append(" />");

            ltlRadio.Text = sb.ToString();
            ltlText.Text = Text;
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