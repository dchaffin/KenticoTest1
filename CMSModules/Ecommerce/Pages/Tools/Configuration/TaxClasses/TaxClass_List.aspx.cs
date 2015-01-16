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
using CMS.ExtendedControls;

[Title("Objects/Ecommerce_TaxClass/object.png", "TaxClass_List.HeaderCaption", "tax_classes_list")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_List : CMSTaxClassesPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (UseGlobalObjects && ExchangeTableInfoProvider.IsExchangeRateFromGlobalMainCurrencyMissing(CMSContext.CurrentSiteID))
        {
            ShowError(GetString("com.NeedExchangeRateFromGlobal"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);

        // Prepare the new tax class header element
        actions.ActionsList.Add(new HeaderAction()
        {
            Text = GetString("TaxClass_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("TaxClass_New.aspx?siteId=" + SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_TaxClass/add.png"),
            ControlType = HeaderActionTypeEnum.Hyperlink
        });

        // Show copy from global link when not configuring global tax classes
        if (ConfiguredSiteID != 0)
        {
            // Show "Copy from global" link only if there is at least one global tax class
            DataSet ds = TaxClassInfoProvider.GetTaxClasses("TaxClassSiteID IS NULL", null, 1, "TaxClassID");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                actions.ActionsList.Add(new HeaderAction()
                {
                    Text = GetString("general.copyfromglobal"),
                    OnClientClick = "return ConfirmCopyFromGlobal();",
                    ImageUrl = GetImageUrl("Objects/Ecommerce_TaxClass/fromglobal.png"),
                    CommandName = "copyFromGlobal",
                    ControlType = HeaderActionTypeEnum.LinkButton
                });

                // Register javascript to confirm action 
                string script = "function ConfirmCopyFromGlobal() {return confirm(" + ScriptHelper.GetString(GetString("com.ConfirmTaxClassFromGlobal")) + ");}";
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmCopyFromGlobal", ScriptHelper.GetScript(script));
            }
        }

        UniGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);
        UniGrid.ZeroRowsText = GetString("general.nodatafound");

        // Configuring global records from specific site
        if ((ConfiguredSiteID == 0) && (SiteID != 0))
        {
            ShowInformation(GetString("com.UsingGlobalSettings"));
        }

        // Filter records by site
        UniGrid.WhereCondition = "ISNULL(TaxClassSiteID, 0) = " + ConfiguredSiteID;
    }

    #endregion


    #region "Event Handlers"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "copyfromglobal":
                CopyFromGlobal();
                UniGrid.ReloadData();
                break;
        }
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
            URLHelper.Redirect("TaxClass_Frameset.aspx?taxclassid=" + Convert.ToString(actionArgument) + "&siteId=" + SiteID);
        }
        else if (actionName == "delete")
        {
            // Check permissions
            CheckConfigurationModification();

            TaxClassInfoProvider.DeleteTaxClassInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    ///  Copies site-specific tax classes from global list.
    /// </summary>
    protected void CopyFromGlobal()
    {
        CheckConfigurationModification();
        TaxClassInfoProvider.CopyFromGlobal(ConfiguredSiteID);
    }

    #endregion
}