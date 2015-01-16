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
using CMS.UIControls;
using CMS.SiteProvider;

[Security(Resource = "CMS.Ecommerce", UIElements = "Orders.General")]
[Security(Resource = "CMS.Ecommerce", UIElements = "Customers")]
[Security(Resource = "CMS.Ecommerce", UIElements = "Customers.General")]
[Title(ResourceString = "com.customer.edit", ImageUrl = "Objects/Ecommerce_Customer/object.png")]
[EditedObject("ecommerce.customer", "customerId")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Customer : CMSOrdersModalPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Check module permissions
        if (!ECommerceContext.IsUserAuthorizedForPermission("ReadCustomers"))
        {
            RedirectToCMSDeskAccessDenied("CMS.Ecommerce", "EcommerceRead OR ReadCustomers");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        btnOk.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.cancel");

        CustomerInfo customer = EditedObject as CustomerInfo;

        // Check if customer can be edited on current site
        if (CheckCustomerSiteID(customer))
        {
            customerEdit.Customer = customer;
            customerEdit.OnSaved += customerEdit_OnSaved;
        }
        else
        {
            EditedObject = null;
        }
    }

    #endregion


    #region "Event handlers"

    protected void customerEdit_OnSaved(object sender, EventArgs e)
    {
        // Refresh the opening page
        string javaScript = "if(window.wopener != null){ window.wopener.location.replace(window.wopener.location); } CloseDialog();";
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "RefreshParentScript", ScriptHelper.GetScript(javaScript));
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Save customer
        customerEdit.Save();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true if customer is anonymous on current site or has login on current site.
    /// </summary>
    /// <param name="customer">Customer object</param>
    protected bool CheckCustomerSiteID(CustomerInfo customer)
    {
        if (customer == null)
        {
            return false;
        }

        int currentSiteId = CMSContext.CurrentSiteID;

        if (customer.CustomerSiteID == currentSiteId)
        {
            return true;
        }

        if ((customer.CustomerUserID > 0) && (UserSiteInfoProvider.GetUserSiteInfo(customer.CustomerUserID, currentSiteId) != null))
        {
            return true;
        }

        if (CurrentUser.IsGlobalAdministrator && (customer.CustomerUserID == 0) && (customer.CustomerSiteID == 0))
        {
            return true;
        }

        return false;
    }

    #endregion
}