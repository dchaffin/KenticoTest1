using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Customers.General")]
[EditedObject("ecommerce.customer", "customerId")]
public partial class CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_General : CMSCustomersPage
{
    private int currentSiteId = -1;
    private CustomerInfo customerObj = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        currentSiteId = CMSContext.CurrentSiteID;

        customerObj = EditedObject as CustomerInfo;

        // Check if customer belongs to current site
        if (!CheckCustomerSiteID(customerObj))
        {
            EditedObject = null;
        }

        customerEdit.Customer = customerObj;
        customerEdit.OnSaved += new EventHandler(customerEdit_OnSaved);

        if (customerObj != null)
        {
            // Show that the customer was created or updated successfully
            if (!RequestHelper.IsPostBack() && (QueryHelper.GetString("saved", "") == "1"))
            {
                // Show message
                ShowChangesSaved();
            }
        }
    }


    void customerEdit_OnSaved(object sender, EventArgs e)
    {
        // Refresh header (name might be changed)
        ScriptHelper.RefreshTabHeader(this, "Customers.General");
    }
}