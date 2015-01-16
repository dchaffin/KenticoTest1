using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Controls_UI_ProductMenu : CMSUserControl
{
    #region "Properties"

    private string Culture
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Culture"), string.Empty);
        }
        set
        {
            SetValue("Culture", value);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Init currently selected culture
        mnuLanguage.SelectedCulture = Culture;
        mnuContent.NewImageUrl = GetImageUrl("CMSModules/CMS_Ecommerce/newproductsection.png");
        mnuContent.DeleteImageUrl = GetImageUrl("CMSModules/CMS_Ecommerce/deleteproductsection.png");
        mnuContent.ShrinkedButtonMinimalWidth = 79;
        mnuContent.SmallButtonMinimalWidth = 80;

        // Hide language menu when only one language present
        DataSet siteCulturesDS = CultureInfoProvider.GetSiteCultures(CMSContext.CurrentSiteName);
        if (!DataHelper.DataSourceIsEmpty(siteCulturesDS) && (siteCulturesDS.Tables[0].Rows.Count < 2))
        {
            pnlLanguageMenu.Visible = false;
            mnuLanguage.StopProcessing = true;
        }
    }

    #endregion
}