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

public partial class CMSWebParts_Text_staticHTML : CMSAbstractWebPart
{
    /// <summary>
    /// Gets or sets the text (HTML code) to be displayed.
    /// </summary>
    public string Text
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Text"), ltlText.Text);
        }
        set
        {
            SetValue("Text", value);
            ltlText.Text = value;
            ltlText.EnableViewState = (ResolveDynamicControls && ControlsHelper.ResolveDynamicControls(this));
        }
    }


    /// <summary>
    /// Enables or disables resolving of inline controls.
    /// </summary>
    public bool ResolveDynamicControls
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ResolveDynamicControls"), true);
        }
        set
        {
            SetValue("ResolveDynamicControls", value);
        }
    }


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
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
            ltlText.EnableViewState = (ResolveDynamicControls && ControlsHelper.ResolveDynamicControls(this));
        }
    }
}