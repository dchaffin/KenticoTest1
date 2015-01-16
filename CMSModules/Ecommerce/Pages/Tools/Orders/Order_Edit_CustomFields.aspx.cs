using System;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Orders.CustomFields")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_CustomFields : CMSOrdersPage
{
    protected int orderId;


    protected override void OnPreInit(EventArgs e)
    {
        CustomerID = QueryHelper.GetInteger("customerid", 0);
        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Set edit mode
        orderId = QueryHelper.GetInteger("orderId", 0);
        if (orderId > 0)
        {
            OrderInfo oi = OrderInfoProvider.GetOrderInfo(orderId);

            if (oi != null)
            {
                // Check order site ID
                CheckOrderSiteID(oi.OrderSiteID);
            }

            formOrderCustomFields.Info = oi;
            EditedObject = formOrderCustomFields.Info;
            formOrderCustomFields.OnAfterValidate += formOrderCustomFields_OnAfterValidate;
            formOrderCustomFields.OnAfterSave += formOrderCustomFields_OnAfterSave;
        }
        else
        {
            formOrderCustomFields.Enabled = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (formOrderCustomFields.BasicForm != null)
        {
            if (formOrderCustomFields.BasicForm.FieldControls.Count <= 0)
            {
                // Hide submit button if no field is present
                formOrderCustomFields.BasicForm.SubmitButton.Visible = false;
            }
            else
            {
                // Set submit button's css class
                formOrderCustomFields.BasicForm.SubmitButton.CssClass = "ContentButton";
            }
        }
    }


    private void formOrderCustomFields_OnAfterValidate(object sender, EventArgs e)
    {
        // Check 'EcommerceModify' permission
        if ((CMSContext.CurrentSite != null) && (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders")))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");

            // Prevent saving data
            formOrderCustomFields.StopProcessing = true;
        }
    }


    private void formOrderCustomFields_OnAfterSave(object sender, EventArgs e)
    {
        // Display 'changes saved' information
        ShowChangesSaved();
    }
}