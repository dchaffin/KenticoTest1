using System;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;

public partial class CMSModules_Ecommerce_Pages_Content_Product_Product_Edit_Frameset : CMSContentProductPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterScriptFile(Page, "cmsedit.js");

        frmHeader.Attributes.Add("src", "Product_Edit_Header.aspx" + URLHelper.Url.Query);

        // Reverse frames when RTL
        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        // Remember product tab
        CMSContext.ViewMode = ViewModeEnum.Product;
    }
}