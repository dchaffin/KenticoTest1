using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Configuration.TaxClasses.General")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_General : CMSTaxClassesPage
{
    protected int taxclassid = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        rfvTaxClassDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvTaxClassName.ErrorMessage = GetString("general.requirescodename");

        // Get taxClass id from querystring		
        taxclassid = QueryHelper.GetInteger("taxclassid", 0);
        if (taxclassid > 0)
        {
            TaxClassInfo taxClassObj = TaxClassInfoProvider.GetTaxClassInfo(taxclassid);
            EditedObject = taxClassObj;

            if (taxClassObj != null)
            {
                CheckEditedObjectSiteID(taxClassObj.TaxClassSiteID);
                // Fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    LoadData(taxClassObj);

                    // Show that the taxClass was created or updated successfully
                    if (QueryHelper.GetString("saved", "") == "1")
                    {
                        // Show message
                        ShowChangesSaved();
                    }
                }
            }
            else
            {
                btnOk.Enabled = false;

                // Show error message
                ShowError(GetString("TaxClass_Edit.TaxClassDoesNotExists"));

                return;
            }
        }
    }


    /// <summary>
    /// Load data from edited taxClass object to form.
    /// </summary>
    /// <param name="taxClassObj">TaxClass object</param>
    protected void LoadData(TaxClassInfo taxClassObj)
    {
        txtTaxClassDisplayName.Text = taxClassObj.TaxClassDisplayName;
        txtTaxClassName.Text = taxClassObj.TaxClassName;
        chkTaxZero.Checked = taxClassObj.TaxClassZeroIfIDSupplied;
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
            // TaxClassName must be unique
            TaxClassInfo taxClassObj = TaxClassInfoProvider.GetTaxClassInfo(txtTaxClassName.Text.Trim(), SiteInfoProvider.GetSiteName(ConfiguredSiteID));

            // If taxClassName value is unique
            if ((taxClassObj == null) || (taxClassObj.TaxClassID == taxclassid))
            {
                // If taxClassName value is unique -> determine whether it is update or insert 
                if (taxClassObj == null)
                {
                    // Get TaxClassInfo object by primary key
                    taxClassObj = TaxClassInfoProvider.GetTaxClassInfo(taxclassid);
                }
                if (taxClassObj != null)
                {
                    taxClassObj.TaxClassDisplayName = txtTaxClassDisplayName.Text.Trim();
                    taxClassObj.TaxClassName = txtTaxClassName.Text.Trim();
                    taxClassObj.TaxClassZeroIfIDSupplied = chkTaxZero.Checked;

                    TaxClassInfoProvider.SetTaxClassInfo(taxClassObj);

                    // Show message
                    ShowChangesSaved();
                }
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