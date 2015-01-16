using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

[Title("Objects/Ecommerce_Customer/object.png", "Customers_Edit.ItemListLink", "customers_list")]
[Action(0, "Objects/Ecommerce_Customer/add.png", "Customers_List.NewItemCaption", "~/CMSModules/Ecommerce/Pages/Tools/Customers/Customer_New.aspx")]
public partial class CMSModules_Ecommerce_Pages_Tools_Customers_Customer_List : CMSCustomersPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;

        string anoWhere = "(CustomerUserID IS NULL) AND CustomerSiteID = " + CMSContext.CurrentSiteID;
        string regWhere = string.Format("(CustomerUserID IS NOT NULL) AND (CustomerUserID IN (SELECT UserID FROM CMS_UserSite WHERE SiteID = {0}))", CMSContext.CurrentSiteID);
        UniGrid.WhereCondition = SqlHelperClass.AddWhereCondition(anoWhere, regWhere, "OR");

        // Check if user is global administrator
        if (CurrentUser.IsGlobalAdministrator)
        {
            // Display customers without site binding
            UniGrid.WhereCondition = SqlHelperClass.AddWhereCondition(UniGrid.WhereCondition, "(CustomerUserID IS NULL) AND (CustomerSiteID IS NULL)", "OR");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Ensure customers menu button to be selected
        AddMenuButtonSelectScript("Customers", "");
    }

    #endregion


    #region "Events"

    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "customeruserid":
                return UniGridFunctions.ColoredSpanYesNo(parameter != DBNull.Value);

            case "hassitebinding":
                // Has site binding action
                {
                    ImageButton button = ((ImageButton)sender);

                    if (!CurrentUser.IsGlobalAdministrator)
                    {
                        button.Visible = false;
                    }
                    else
                    {
                        button.OnClientClick = "return false;";
                        button.Style.Add("cursor", "default");

                        // Show warning for customers not bound to site
                        int userId = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["CustomerUserID"], 0);
                        int siteId = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["CustomerSiteID"], 0);
                        button.Visible = (userId == 0) && (siteId == 0);
                    }
                }
                break;
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("Customer_Edit_Frameset.aspx?customerid=" + Convert.ToString(actionArgument));
        }
        else if (actionName == "delete")
        {
            int customerId = ValidationHelper.GetInteger(actionArgument, 0);

            // Check module permissions
            if (!ECommerceContext.IsUserAuthorizedToModifyCustomer())
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyCustomers");
                return;
            }

            // Check customers dependencies
            if (CustomerInfoProvider.CheckDependencies(customerId))
            {
                ShowError(GetString("Ecommerce.DeleteDisabled"));
                return;
            }

            // Delete CustomerInfo object from database
            CustomerInfoProvider.DeleteCustomerInfo(customerId);

            UniGrid.ReloadData();
        }
    }

    #endregion
}