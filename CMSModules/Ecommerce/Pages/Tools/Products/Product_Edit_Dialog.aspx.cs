using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Dialog : CMSModalPage
{
    protected string viewpage = "about:blank";

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set title
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Ecommerce_SKU/object.png");
        CurrentMaster.Title.TitleText = GetString("com.sku.unassignedproperties");

        // Open customer edit frameset in popup mode
        viewpage = ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/Products/Product_Edit_Frameset.aspx" + URLHelper.Url.Query);

        // Fill space
        CurrentMaster.PanelContent.Style.Add("width", "100%");
        CurrentMaster.PanelContent.Style.Add("height", "100%");

        // Localize close button
        btnClose.Text = GetString("general.close");
    }
}
