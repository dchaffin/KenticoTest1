using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.EventLog;

public partial class CMSModules_Licenses_Pages_License_New : SiteManagerPage
{
    #region "Variables"

    private bool errorOccured = false;

    #endregion


    public override void ShowError(string text)
    {
        errorOccured = true;
        base.ShowError(text);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup page title text and image
        CurrentMaster.Title.TitleText = GetString("Licenses_License_New.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_LicenseKey/new.png");

        CurrentMaster.Title.HelpTopicName = "new_license";
        CurrentMaster.Title.HelpName = "helpTopic";

        lbLicenseKey.Text = GetString("Licenses_License_New.Key");
        rfvLicenseKey.ErrorMessage = GetString("Licenses_License_New.KeyError");

        string[,] pageTitleTabs = new string[2,3];
        pageTitleTabs[0, 0] = GetString("Licenses_License_New.Licenses");
        pageTitleTabs[0, 1] = "~/CMSModules/Licenses/Pages/License_List.aspx";
        pageTitleTabs[1, 0] = GetString("licenses_license_new.new");
        pageTitleTabs[1, 1] = "";
        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        errorOccured = false;
        try
        {
            LicenseKeyInfo lk = new LicenseKeyInfo();
            lk.LoadLicense(tbLicenseKey.Text.Trim(), "");

            if (lk != null)
            {
                switch (lk.ValidationResult)
                {
                    case LicenseValidationEnum.Expired:
                        ShowError(GetString("Licenses_License_New.LicenseNotValid.Expired"));
                        break;

                    case LicenseValidationEnum.Invalid:
                        ShowError(GetString("Licenses_License_New.LicenseNotValid.Invalid"));
                        break;

                    case LicenseValidationEnum.NotAvailable:
                        ShowError(GetString("Licenses_License_New.LicenseNotValid.NotAvailable"));
                        break;

                    case LicenseValidationEnum.WrongFormat:
                        ShowError(GetString("Licenses_License_New.LicenseNotValid.WrongFormat"));
                        break;

                    case LicenseValidationEnum.Valid:
                        if (LicenseKeyInfoProvider.IsLicenseExistForDomain(lk))
                        {
                            // License for domain already exist
                            ShowError(GetString("Licenses_License_New.DomainAlreadyExists").Replace("%%name%%", lk.Domain));
                        }
                        else
                        {
                            // Insert license
                            LicenseKeyInfoProvider.SetLicenseKeyInfo(lk);
                            UserInfoProvider.ClearLicenseValues();
                            Functions.ClearHashtables();
                        }
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("License", "NEW", ex);
            ShowError(GetString("general.saveerror"), ex.Message, null);
        }

        if (!errorOccured)
        {
            URLHelper.Redirect("License_List.aspx");
        }
    }
}