using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.PortalControls;

public partial class CMSFormControls_Cultures_SiteCultureSelectorAll : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return SiteCultureSelector.Value;
        }
        set
        {
            SiteCultureSelector.Value = value;
        }
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Ensure the script manager
        PortalHelper.EnsureScriptManager(Page);

        if (StopProcessing)
        {
            SiteCultureSelector.StopProcessing = true;
        }
        else
        {
            SiteCultureSelector.ReloadData();
        }
    }
}