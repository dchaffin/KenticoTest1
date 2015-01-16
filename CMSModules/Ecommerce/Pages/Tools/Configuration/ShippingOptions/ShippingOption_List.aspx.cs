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

[Title("Objects/Ecommerce_ShippingOption/object.png", "COM_ShippingOption_List.HeaderCaption", "shipping_options_list")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_List : CMSShippingOptionsPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        UniGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);
        UniGrid.OnExternalDataBound += new OnExternalDataBoundEventHandler(UniGrid_OnExternalDataBound);
        UniGrid.ZeroRowsText = GetString("general.nodatafound");

        // Init site selector
        SelectSite.Selector.SelectedIndexChanged += new EventHandler(Selector_SelectedIndexChanged);

        if (!RequestHelper.IsPostBack())
        {
            // Init site selector
            SelectSite.SiteID = SiteFilterStartupValue;
        }

        if (AllowGlobalObjects && ExchangeTableInfoProvider.IsExchangeRateFromGlobalMainCurrencyMissing(CMSContext.CurrentSiteID))
        {
            ShowWarning(GetString("com.NeedExchangeRateFromGlobal"), null, null);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Prepare the new shipping option header element
        hdrActions.ActionsList.Add(new HeaderAction()
        {
            Text = GetString("COM_ShippingOption_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("ShippingOption_New.aspx?siteId=" + SelectSite.SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_ShippingOption/add.png"),
            ControlType = HeaderActionTypeEnum.Hyperlink
        });

        CurrentMaster.DisplaySiteSelectorPanel = AllowGlobalObjects;

        InitWhereCondition();
    }


    protected override void OnPreRender(EventArgs e)
    {
        bool both = (SelectSite.SiteID == UniSelector.US_GLOBAL_OR_SITE_RECORD);

        // Hide header actions if (both) selected
        hdrActions.Enabled = !both;
        lblWarnNew.Visible = both;

        base.OnPreRender(e);
        UniGrid.NamedColumns["ShippingOptionSiteID"].Visible = AllowGlobalObjects;
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("ShippingOption_Edit_Frameset.aspx?shippingOptionID=" + Convert.ToString(actionArgument) + "&siteId=" + SelectSite.SiteID);
        }
        else if (actionName == "delete")
        {
            ShippingOptionInfo shippingInfoObj = ShippingOptionInfoProvider.GetShippingOptionInfo(ValidationHelper.GetInteger(actionArgument, 0));
            // Nothing to delete
            if (shippingInfoObj == null)
            {
                return;
            }

            // Check permissions
            CheckConfigurationModification(shippingInfoObj.ShippingOptionSiteID);

            // Check dependencies
            if (ShippingOptionInfoProvider.CheckDependencies(shippingInfoObj.ShippingOptionID))
            {
                // Show error message
                ShowError(GetString("Ecommerce.DeleteDisabled"));

                return;
            }
            // Delete ShippingOptionInfo object from database
            ShippingOptionInfoProvider.DeleteShippingOptionInfo(shippingInfoObj);
        }
    }


    private object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "shippoptenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);
            case "shippingoptionsiteid":
                return UniGridFunctions.ColoredSpanYesNo(parameter == DBNull.Value);
            case "shippoptcharge":
                DataRowView row = (DataRowView)parameter;
                double value = ValidationHelper.GetDouble(row["ShippingOptionCharge"], 0);
                int siteId = ValidationHelper.GetInteger(row["ShippingOptionSiteID"], 0);

                return CurrencyInfoProvider.GetFormattedPrice(value, siteId);
        }

        return parameter;
    }


    /// <summary>
    /// Handles the SiteSelector's selection changed event.
    /// </summary>
    protected void Selector_SelectedIndexChanged(object sender, EventArgs e)
    {
        InitWhereCondition();
        UniGrid.ReloadData();

        // Save selected value
        StoreSiteFilterValue(SelectSite.SiteID);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates where condition for UniGrid and reloads it.
    /// </summary>
    private void InitWhereCondition()
    {
        UniGrid.WhereCondition = SelectSite.GetSiteWhereCondition("ShippingOptionSiteID");
    }

    #endregion
}