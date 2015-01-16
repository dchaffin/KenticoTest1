using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;



public partial class CMSModules_BannerManagement_FormControls_BannerCategorySelector : FormEngineUserControl
{
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
            base.Enabled = value;
            usBannerCategory.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return usBannerCategory.Value;
        }
        set
        {
            if (usBannerCategory == null)
            {
                pnlUpdate.LoadContainer();
            }
            usBannerCategory.Value = value;
        }
    }

    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            usBannerCategory.StopProcessing = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            ReloadData();
        }

        usBannerCategory.IsLiveSite = IsLiveSite;
    }


    private void ReloadData()
    {
        usBannerCategory.FilterControl = "~/CMSModules/BannerManagement/Controls/Filters/SiteSelector.ascx";
    }
}
