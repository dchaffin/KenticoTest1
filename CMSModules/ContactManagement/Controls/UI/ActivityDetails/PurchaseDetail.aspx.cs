using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

[Title("Objects/OM_Activity/object.png", "om.activitydetails.viewinvoicedetail", null)]
[Security(Resource = ModuleEntry.CONTACTMANAGEMENT, Permission = "ReadActivities")]
public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_PurchaseDetail : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (ActivityHelper.AuthorizedReadActivity(CMSContext.CurrentSiteID, true))
        {
            if (!QueryHelper.ValidateHash("hash"))
            {
                return;
            }

            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.ECOMMERCE))
            {
                return;
            }

            int orderId = QueryHelper.GetInteger("orderid", 0);

            // Get order object
            BaseInfo order = CMSObjectHelper.GetObjectById(PredefinedObjectType.ORDER, orderId);
            if (order != null)
            {
                ltl.Text = order.GetStringValue("OrderInvoice", "");
            }
        }
    }
}