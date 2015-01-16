using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("Objects/Ecommerce_TaxClass/new.png", "TaxClass_Edit.HeaderCaption_New", "CMS_Ecommerce_Configuration_TaxClasses_General")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_New : CMSTaxClassesPage
{
    protected int taxclassid = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        rfvTaxClassDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvTaxClassName.ErrorMessage = GetString("general.requirescodename");

        string currentTaxClass = GetString("TaxClass_Edit.NewItemCaption");

        // initializes page title control		
        string[,] pageTitleTabs = new string[2, 3];
        pageTitleTabs[0, 0] = GetString("TaxClass_Edit.ItemListLink");
        pageTitleTabs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/TaxClasses/TaxClass_List.aspx?SiteId=" + SiteID;
        pageTitleTabs[0, 2] = "";
        pageTitleTabs[1, 0] = currentTaxClass;
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        CheckConfigurationModification();

        string errorMessage = new Validator()
            .NotEmpty(txtTaxClassDisplayName.Text.Trim(), GetString("general.requiresdisplayname"))
            .NotEmpty(txtTaxClassName.Text.Trim(), GetString("general.requirescodename")).Result;

        if (!ValidationHelper.IsCodeName(txtTaxClassName.Text.Trim()))
        {
            errorMessage = GetString("General.ErrorCodeNameInIdentifierFormat");
        }

        if (errorMessage == "")
        {
            TaxClassInfo taxClassObj = TaxClassInfoProvider.GetTaxClassInfo(txtTaxClassName.Text.Trim(), SiteInfoProvider.GetSiteName(ConfiguredSiteID));

            if (taxClassObj == null)
            {
                // If taxClassName value is unique for configured site
                taxClassObj = new TaxClassInfo();

                taxClassObj.TaxClassDisplayName = txtTaxClassDisplayName.Text.Trim();
                taxClassObj.TaxClassName = txtTaxClassName.Text.Trim();
                taxClassObj.TaxClassZeroIfIDSupplied = chkTaxZero.Checked;
                taxClassObj.TaxClassSiteID = ConfiguredSiteID;

                TaxClassInfoProvider.SetTaxClassInfo(taxClassObj);

                URLHelper.Redirect("TaxClass_Frameset.aspx?taxclassid=" + Convert.ToString(taxClassObj.TaxClassID) + "&saved=1&siteId=" + SiteID);
            }
            else
            {
                // Show error message
                ShowError(GetString("TaxClass_Edit.TaxClassNameExists"));
            }
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }
}