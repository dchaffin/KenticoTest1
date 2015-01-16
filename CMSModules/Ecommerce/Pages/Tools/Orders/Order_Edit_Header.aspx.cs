using System;
using System.Data;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Controls.Configuration;

using CMS.Ecommerce;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Tabs("CMS.Ecommerce", "Orders", "orderContent")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Header : CMSOrdersPage
{
    #region "Variables"

    private int orderId;
    private int customerId;

    #endregion


    protected override void OnPreInit(EventArgs e)
    {
        customerId = QueryHelper.GetInteger("customerid", 0);
        CustomerID = customerId;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the parameters from querystring
        orderId = QueryHelper.GetInteger("orderid", 0);
        customerId = QueryHelper.GetInteger("customerId", 0);

        // Initializes page title control		
        string[,] pageTitleTabs = new string[2, 3];
        pageTitleTabs[0, 0] = GetString("Order_Edit.Orders");
        pageTitleTabs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_List.aspx";
        if (customerId > 0)
        {
            pageTitleTabs[0, 1] += "?customerId=" + customerId.ToString();
            pageTitleTabs[0, 2] = "CustomerContent";
        }
        else
        {
            pageTitleTabs[0, 2] = "ecommerceContent";
        }
        pageTitleTabs[1, 0] = orderId.ToString();
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";

        CMSMasterPage master = (CMSMasterPage)CurrentMaster;

        master.Title.Breadcrumbs = pageTitleTabs;
        master.Title.HelpTopicName = "general_tab11";
        master.Title.HelpName = "helpTopic";

        master.Tabs.OnTabCreated += Tabs_OnTabCreated;

        // Set master title
        if (customerId <= 0)
        {
            master.Title.TitleText = GetString("com.order.edit");
            master.Title.TitleImage = GetImageUrl("Objects/Ecommerce_Order/object.png");

            AddMenuButtonSelectScript("Orders", "");
        }
    }


    protected TabItem Tabs_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        if (element.ElementName.ToLowerCSafe() == "orders.customfields")
        {
            // Check if order has any custom fields
            FormInfo formInfo = FormHelper.GetFormInfo("ecommerce.order", false);
            if (!formInfo.GetFormElements(true, false, true).Any())
            {
                return null;
            }
        }

        return tab;
    }
}