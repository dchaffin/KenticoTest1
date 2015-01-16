using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_List : CMSOrdersPage
{
    #region "Variables"

    protected int customerId;

    #endregion


    #region "Page Events"

    protected override void OnPreInit(EventArgs e)
    {
        customerId = QueryHelper.GetInteger("customerid", 0);
        CustomerID = customerId;

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // New order action
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.Hyperlink,
            Text = GetString("Order_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("Order_New.aspx") + "?customerid=" + customerId,
            ImageUrl = GetImageUrl("Objects/Ecommerce_Order/add.png")
        });

        // Set master title
        if (customerId <= 0)
        {
            CurrentMaster.Title.TitleText = GetString("Order_New.Orders");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Ecommerce_Order/object.png");
            CurrentMaster.Title.HelpName = "helpTopic";
            CurrentMaster.Title.HelpTopicName = "orders_list";

            AddMenuButtonSelectScript("Orders", "");
        }
        else
        {
            AddMenuButtonSelectScript("Customers", "");
        }
    }

    #endregion
}