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

[Title("Objects/Ecommerce_DiscountCoupon/object.png", "DiscounCoupon_Edit.ItemListLink", "discount_coupons_list")]
public partial class CMSModules_Ecommerce_Pages_Tools_DiscountCoupons_DiscountCoupon_List : CMSDiscountCouponsPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.ZeroRowsText = GetString("general.nodatafound");

        // Init site selector
        SelectSite.Selector.SelectedIndexChanged += new EventHandler(Selector_SelectedIndexChanged);

        if (AllowGlobalObjects && ExchangeTableInfoProvider.IsExchangeRateFromGlobalMainCurrencyMissing(CMSContext.CurrentSiteID))
        {
            ShowWarning(GetString("com.NeedExchangeRateFromGlobal"), null, null);
        }

        if (!RequestHelper.IsPostBack())
        {
            // Init site selector
            SelectSite.SiteID = SiteFilterStartupValue;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // New coupon action
        hdrActions.ActionsList.Add(new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.Hyperlink,
            Text = GetString("DiscounCoupon_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("DiscountCoupon_New.aspx?siteId=" + SelectSite.SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_DiscountCoupon/add.png")
        });

        // Set site selector visibility
        CurrentMaster.DisplaySiteSelectorPanel = AllowGlobalObjects;

        AddMenuButtonSelectScript("DiscountCoupons", "");

        // Init condition for unigrid
        InitWhereCondition();
    }


    protected override void OnPreRender(EventArgs e)
    {
        bool both = (SelectSite.SiteID == UniSelector.US_GLOBAL_OR_SITE_RECORD);

        // Hide header actions if (both) selected
        hdrActions.Enabled = !both;
        lblWarnNew.Visible = both;

        base.OnPreRender(e);
        gridElem.NamedColumns["DiscountCouponSiteID"].Visible = AllowGlobalObjects;
    }

    #endregion


    #region "Event Handlers"

    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "couponvalue":
                DataRowView row = (DataRowView)parameter;
                double value = ValidationHelper.GetDouble(row["DiscountCouponValue"], 0);
                bool isFlat = ValidationHelper.GetBoolean(row["DiscountCouponIsFlatValue"], false);
                int siteId = ValidationHelper.GetInteger(row["DiscountCouponSiteID"], 0);

                if (isFlat)
                {
                    return CurrencyInfoProvider.GetFormattedPrice(value, siteId);
                }
                else
                {
                    return value.ToString() + "%";
                }

            case "discountcouponsiteid":
                return UniGridFunctions.ColoredSpanYesNo(parameter == DBNull.Value);
        }
        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("DiscountCoupon_Edit_Frameset.aspx?discountid=" + Convert.ToString(actionArgument) + "&siteId=" + SelectSite.SiteID);
        }
        else if (actionName == "delete")
        {
            int id = ValidationHelper.GetInteger(actionArgument, 0);

            DiscountCouponInfo discountCouponInfoObj = DiscountCouponInfoProvider.GetDiscountCouponInfo(id);
            // Nothing to delete
            if (discountCouponInfoObj == null)
            {
                return;
            }

            // Check module permissions
            if (!ECommerceContext.IsUserAuthorizedToModifyDiscountCoupon(discountCouponInfoObj))
            {
                if (discountCouponInfoObj.IsGlobal)
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
                }
                else
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyDiscounts");
                }
            }

            if (DiscountCouponInfoProvider.CheckDependencies(id))
            {
                ShowError(GetString("Ecommerce.DeleteDisabled"));
                return;
            }

            // Delete DiscountCouponInfo object from database
            DiscountCouponInfoProvider.DeleteDiscountCouponInfo(discountCouponInfoObj);
        }
    }


    /// <summary>
    /// Handles the SiteSelector's selection changed event.
    /// </summary>
    protected void Selector_SelectedIndexChanged(object sender, EventArgs e)
    {
        InitWhereCondition();
        gridElem.ReloadData();

        // Save selected value
        StoreSiteFilterValue(SelectSite.SiteID);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates where condition for UniGrid.
    /// </summary>
    private void InitWhereCondition()
    {
        gridElem.WhereCondition = SelectSite.GetSiteWhereCondition("DiscountCouponSiteID");
    }

    #endregion
}