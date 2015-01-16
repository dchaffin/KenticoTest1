using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

[Title("Objects/Ecommerce_Currency/object.png", "Currency_List.HeaderCaption", "currencies_list")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Currencies_Currency_List : CMSCurrenciesPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);

        // New item action
        actions.ActionsList.Add(new HeaderAction()
        {
            Text = GetString("Currency_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("Currency_Edit.aspx?siteId=" + SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_Currency/add.png"),
            ControlType = HeaderActionTypeEnum.Hyperlink
        });

        // Show copy from global link when not configuring global currencies.
        if (ConfiguredSiteID != 0)
        {
            // Show "Copy from global" link only if there is at least one global currency
            DataSet ds = CurrencyInfoProvider.GetCurrencies("CurrencySiteID IS NULL", null, 1, "CurrencyID");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                actions.ActionsList.Add(new HeaderAction()
                {
                    Text = GetString("general.copyfromglobal"),
                    OnClientClick = "return ConfirmCopyFromGlobal();",
                    ImageUrl = GetImageUrl("Objects/Ecommerce_Currency/fromglobal.png"),
                    CommandName = "copyFromGlobal",
                    ControlType = HeaderActionTypeEnum.LinkButton
                });

                // Register javascript to confirm generate 
                string script = "function ConfirmCopyFromGlobal() {return confirm(" + ScriptHelper.GetString(GetString("com.ConfirmCurrencyFromGlobal")) + ");}";
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmCopyFromGlobal", ScriptHelper.GetScript(script));
            }
        }

        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.ZeroRowsText = GetString("general.nodatafound");

        // Configuring global records from specific site
        if ((ConfiguredSiteID == 0) && (SiteID != 0))
        {
            ShowInformation(GetString("com.UsingGlobalSettings"));
        }

        // Filter records by site
        gridElem.WhereCondition = "ISNULL(CurrencySiteID, 0) = " + ConfiguredSiteID;
    }

    #endregion


    #region "Event Handlers"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "copyfromglobal":
                CopyFromGlobal();
                gridElem.ReloadData();
                break;
        }
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "currismain":
                return UniGridFunctions.ColoredSpanYes(parameter);
            case "currenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);
        }
        return "";
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
            URLHelper.Redirect("Currency_Edit.aspx?currencyid=" + Convert.ToString(actionArgument) + "&siteId=" + SiteID);
        }
        else if (actionName == "delete")
        {
            // Check permissions
            CheckConfigurationModification();

            int currencyId = ValidationHelper.GetInteger(actionArgument, 0);

            if (CurrencyInfoProvider.CheckDependencies(currencyId))
            {
                // Show error message
                ShowError(GetString("Ecommerce.DeleteDisabled"));

                return;
            }

            CurrencyInfo ci = CurrencyInfoProvider.GetCurrencyInfo(currencyId);

            if (ci != null)
            {
                // An attempt to delete main currency
                if (ci.CurrencyIsMain)
                {
                    // Show error message
                    ShowError(string.Format(GetString("com.deletemaincurrencyerror"), ci.CurrencyDisplayName));

                    return;
                }

                // Delete CurrencyInfo object from database
                CurrencyInfoProvider.DeleteCurrencyInfo(ci);
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    ///  Copies site-specific currencies from global currency list.
    /// </summary>
    protected void CopyFromGlobal()
    {
        CheckConfigurationModification();
        CurrencyInfoProvider.CopyFromGlobal(ConfiguredSiteID);
    }

    #endregion
}