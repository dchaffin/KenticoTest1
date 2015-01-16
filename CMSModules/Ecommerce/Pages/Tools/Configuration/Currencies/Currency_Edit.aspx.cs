using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;

// Edited object
[EditedObject(PredefinedObjectType.CURRENCY, "currencyid")]
// Title
[Title("Objects/Ecommerce_Currency/object.png", "Currency_Edit.HeaderCaption", "newedit_currency", ExistingObject = true)]
[Title("Objects/Ecommerce_Currency/new.png", "Currency_New.HeaderCaption", "newedit_currency", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Currencies_Currency_Edit : CMSCurrenciesPage
{
    protected int mCurrencyId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        rfvDisplayName.ErrorMessage = GetString("Currency_Edit.ErrorDisplayName");
        rfvCodeName.ErrorMessage = GetString("Currency_Edit.ErrorCodeName");
        rfvRoundTo.ErrorMessage = GetString("Currency_Edit.ErrorRoundTo");
        revRoundTo.ErrorMessage = GetString("Currency_Edit.ErrorRoundTo");
        rfvCurrencyCode.ErrorMessage = GetString("Currency_Edit.rfvCurrencyCode");
        rfvFormatString.ErrorMessage = GetString("Currency_Edit.rfvFormatString");

        revRoundTo.ValidationExpression = @"^([0-9])+$";

        imgHelp.ImageUrl = GetImageUrl("General/HelpSmall.png");
        imgHelp.ToolTip = GetString("currencyedit.currencycode");

        string currentCurrency = GetString("Currency_Edit.NewItemCaption");

        // Get currency id from querystring
        mCurrencyId = QueryHelper.GetInteger("currencyid", 0);
        if (mCurrencyId > 0)
        {
            CurrencyInfo currencyObj = EditedObject as CurrencyInfo;

            if (currencyObj != null)
            {
                CheckEditedObjectSiteID(currencyObj.CurrencySiteID);
                currentCurrency = ResHelper.LocalizeString(currencyObj.CurrencyDisplayName);

                // Fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    LoadData(currencyObj);

                    // show that the currency was created or updated successfully
                    if (QueryHelper.GetString("saved", "") == "1")
                    {
                        // Show message
                        ShowChangesSaved();
                    }
                }
            }
        }

        // Initialize breadcrumbs
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = GetString("Currency_Edit.ItemListLink");
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/Currencies/Currency_List.aspx?siteId=" + SiteID;
        breadcrumbs[0, 2] = "";
        breadcrumbs[1, 0] = currentCurrency;
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = breadcrumbs;
    }


    /// <summary>
    /// Load data of editing currency.
    /// </summary>
    /// <param name="currencyObj">Currency object</param>
    protected void LoadData(CurrencyInfo currencyObj)
    {
        txtCurrencyDisplayName.Text = currencyObj.CurrencyDisplayName;
        txtCurrencyName.Text = currencyObj.CurrencyName;
        txtCurrencyCode.Text = currencyObj.CurrencyCode;
        txtFormatString.Text = currencyObj.CurrencyFormatString;
        txtCurrencyRoundTo.Text = "";
        if (currencyObj.CurrencyRoundTo != -1)
        {
            txtCurrencyRoundTo.Text = Convert.ToString(currencyObj.CurrencyRoundTo);
        }
        chkCurrencyEnabled.Checked = currencyObj.CurrencyEnabled;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check permissions
        CheckConfigurationModification();

        string errorMessage = ValidateForm();

        // Check whether the main currency is being disabled
        CurrencyInfo main = CurrencyInfoProvider.GetMainCurrency(ConfiguredSiteID);
        if ((main != null) && (main.CurrencyID == mCurrencyId) && !chkCurrencyEnabled.Checked)
        {
            errorMessage = String.Format(GetString("ecommerce.disablemaincurrencyerror"), main.CurrencyDisplayName);
        }

        if (errorMessage == "")
        {
            // CurrencyName must be unique (configured site scope)
            CurrencyInfo currencyObj = CurrencyInfoProvider.GetCurrencyInfo(txtCurrencyName.Text.Trim(), SiteInfoProvider.GetSiteName(ConfiguredSiteID));

            // If currencyName value is unique														
            if ((currencyObj == null) || (currencyObj.CurrencyID == mCurrencyId))
            {
                // If currencyName value is unique -> determine whether it is update or insert 
                if ((currencyObj == null))
                {
                    // Get CurrencyInfo object by primary key
                    currencyObj = CurrencyInfoProvider.GetCurrencyInfo(mCurrencyId);
                    if (currencyObj == null)
                    {
                        // Create new item -> insert
                        currencyObj = new CurrencyInfo();
                        currencyObj.CurrencySiteID = ConfiguredSiteID;
                        currencyObj.CurrencyIsMain = false;
                    }
                }

                currencyObj.CurrencyDisplayName = txtCurrencyDisplayName.Text.Trim();
                currencyObj.CurrencyName = txtCurrencyName.Text.Trim();
                currencyObj.CurrencyCode = txtCurrencyCode.Text.Trim();
                currencyObj.CurrencyFormatString = txtFormatString.Text.Trim();
                // Set null if not specified
                currencyObj.CurrencyRoundTo = ValidationHelper.GetInteger(txtCurrencyRoundTo.Text.Trim(), -1);
                currencyObj.CurrencyEnabled = chkCurrencyEnabled.Checked;

                CurrencyInfoProvider.SetCurrencyInfo(currencyObj);

                URLHelper.Redirect("Currency_Edit.aspx?currencyid=" + Convert.ToString(currencyObj.CurrencyID) + "&saved=1&siteId=" + SiteID);
            }
            else
            {
                // Show error message
                ShowError(GetString("Currency_Edit.CurrencyNameExists"));
            }
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }


    /// <summary>
    /// Validates form and returns error message if some error occurs, otherwise returns empty string.
    /// </summary>
    private string ValidateForm()
    {
        string errorMessage = new Validator()
            .NotEmpty(txtCurrencyName.Text.Trim(), GetString("Currency_Edit.ErrorCodeName"))
            .NotEmpty(txtCurrencyDisplayName.Text.Trim(), GetString("Currency_Edit.ErrorDisplayName"))
            .NotEmpty(txtFormatString.Text.Trim(), GetString("Currency_Edit.ErrorFormatString"))
            .IsRegularExp(txtCurrencyRoundTo.Text, "^([0-9]){1,2}$", GetString("Currency_Edit.ErrorRoundTo")).Result;

        int digits = ValidationHelper.GetInteger(txtCurrencyRoundTo.Text.Trim(), -1);
        if (digits > 15 || digits < 0)
        {
            errorMessage = GetString("Currency_Edit.ErrorRoundTo");
        }

        // Validate currency code name
        if ((errorMessage == "") && (!ValidationHelper.IsCodeName(txtCurrencyName.Text.Trim())))
        {
            errorMessage = GetString("General.ErrorCodeNameInIdentifierFormat");
        }

        // Validate currency format string
        if (errorMessage == "")
        {
            try
            {
                // Test for double exception
                string.Format(txtFormatString.Text.Trim(), 1.234);

                string.Format(txtFormatString.Text.Trim(), "12.12");
            }
            catch
            {
                errorMessage = GetString("Currency_Edit.ErrorCurrencyFormatString");
            }
        }

        return errorMessage;
    }
}