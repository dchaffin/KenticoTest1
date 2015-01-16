using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;

public partial class CMSFormControls_Documents_DocumentStylesheetSelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            EnsureChildControls();
            base.Enabled = value;
            stylesheetSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets stylesheet ID.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return ValidationHelper.GetInteger(stylesheetSelector.Value, -1);
        }
        set
        {
            EnsureChildControls();
            stylesheetSelector.Value = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        stylesheetSelector.ReturnColumnName = "StylesheetID";
        stylesheetSelector.SiteId = CMSContext.CurrentSiteID;
    }

    #endregion
}