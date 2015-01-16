using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.GlobalHelper;

public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Configuration_Frameset : CMSEcommerceSharedConfigurationPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string headerUrl = "Configuration_Header.aspx";
        if (SiteID != CMSContext.CurrentSiteID)
        {
            headerUrl += "?SiteId=" + SiteID.ToString();
        }

        configHeader.Attributes["src"] = headerUrl;
        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }
    }
}