using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;

public partial class CMSFormControls_Documents_SocialMediaPublishing_URLShortenerTypeSelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets enabled state.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return ddlType.Enabled;
        }
        set
        {
            ddlType.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the value of the control - selected URL shortener.
    /// </summary>
    public override object Value
    {
        get
        {
            return ddlType.SelectedValue;
        }
        set
        {
            ddlType.SelectedValue = value.ToString();
        }
    }

    #endregion
}
