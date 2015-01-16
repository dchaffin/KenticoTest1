using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.PortalControls;

public partial class CMSWebParts_General_headhtml : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the head HTML code.
    /// </summary>
    public string HTMLCode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HTMLCode"), "");
        }
        set
        {
            SetValue("HTMLCode", value);
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
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            Page.Header.Controls.Add(new LiteralControl(HTMLCode));
        }
    }
}