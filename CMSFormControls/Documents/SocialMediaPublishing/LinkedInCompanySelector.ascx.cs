using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SocialNetworking;
using CMS.SettingsProvider;
using CMS.CMSHelper;

public partial class CMSFormControls_Documents_SocialMediaPublishing_LinkedInCompanySelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value of access_token.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            uniSelector.Value = value;
        }
    }

    #endregion
}