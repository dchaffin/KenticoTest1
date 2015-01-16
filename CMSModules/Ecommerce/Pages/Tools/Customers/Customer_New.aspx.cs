using System;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[Title("Objects/Ecommerce_Customer/new.png", "customers_edit.newitemcaption", "new_customer")]
[Breadcrumbs(2)]
[Breadcrumb(0, "customers_edit.itemlistlink", "~/CMSModules/Ecommerce/Pages/Tools/Customers/Customer_List.aspx", "")]
[Breadcrumb(1, "customers_edit.newitemcaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Customers_Customer_New : CMSCustomersPage
{
    protected override void OnLoad(EventArgs e)
    {
        // Highlight menu button
        AddMenuButtonSelectScript("Customers", "");

        // Initialize OK button
        btnOk.Click += delegate(object sender, EventArgs args)
        {
            // Check permissions
            if (!ECommerceContext.IsUserAuthorizedToModifyCustomer())
            {
                RedirectToAccessDenied("CMS.Ecommerce", "ModifyCustomers OR EcommerceModify");
            }

            // Create new customer
            int customerId = ucCustomerNew.Save();

            // Redirect to customer edit
            if (customerId > 0)
            {
                URLHelper.Redirect(string.Format("Customer_Edit_Frameset.aspx?customerid={0}&saved=1", customerId));
            }
        };

        base.OnLoad(e);
    }
}