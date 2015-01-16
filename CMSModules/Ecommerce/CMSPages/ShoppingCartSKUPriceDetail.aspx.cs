using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[Title(ImageUrl = "CMSModules/CMS_Ecommerce/pricedetail.png", ResourceString = "ProductPriceDetail.Title")]
public partial class CMSModules_Ecommerce_CMSPages_ShoppingCartSKUPriceDetail : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize product price detail
        ucSKUPriceDetail.CartItemGuid = QueryHelper.GetGuid("itemguid", Guid.Empty);
        ucSKUPriceDetail.ShoppingCart = ECommerceContext.CurrentShoppingCart;

        btnClose.Text = GetString("General.Close");
        btnClose.OnClientClick = "Close(); return false;";
    }
}