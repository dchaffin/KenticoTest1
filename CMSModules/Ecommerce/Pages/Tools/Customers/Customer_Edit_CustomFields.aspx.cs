using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Customers.CustomFields")]
public partial class CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_CustomFields : CMSCustomersPage
{
    protected int customerId;
    protected CustomerInfo customerObj;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Set edit mode
        customerId = QueryHelper.GetInteger("customerId", 0);
        if (customerId > 0)
        {
            customerObj = CustomerInfoProvider.GetCustomerInfo(customerId);
            // Check if customer belongs to current site
            if (!CheckCustomerSiteID(customerObj))
            {
                customerObj = null;
            }

            EditedObject = customerObj;
            formCustomerCustomFields.Info = customerObj;
            formCustomerCustomFields.OnAfterValidate += formCustomerCustomFields_OnAfterValidate;
            formCustomerCustomFields.OnAfterSave += formCustomerCustomFields_OnAfterSave;
        }
        else
        {
            formCustomerCustomFields.Enabled = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (formCustomerCustomFields.BasicForm != null)
        {
            if (formCustomerCustomFields.BasicForm.FieldControls.Count <= 0)
            {
                // Hide submit button if no field are present
                formCustomerCustomFields.BasicForm.SubmitButton.Visible = false;
            }
            else
            {
                // Set submit button's css class
                formCustomerCustomFields.BasicForm.SubmitButton.CssClass = "ContentButton";
            }
        }
    }


    private void formCustomerCustomFields_OnAfterValidate(object sender, EventArgs e)
    {
        if (customerObj == null)
        {
            return;
        }

        // Check module permissions
        if (!ECommerceContext.IsUserAuthorizedToModifyCustomer())
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyCustomers");

            // Prevent saving data
            formCustomerCustomFields.StopProcessing = true;
        }
    }


    private void formCustomerCustomFields_OnAfterSave(object sender, EventArgs e)
    {
        // Display 'Changes saved' information
        ShowChangesSaved();
    }
}