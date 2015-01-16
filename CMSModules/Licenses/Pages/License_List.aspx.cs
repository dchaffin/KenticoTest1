using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Licenses_Pages_License_List : SiteManagerPage
{
    /// <summary>
    /// Client portal URL
    /// </summary>
    private const string CLIENT_PORTAL = "http://client.kentico.com/";


    /// <summary>
    /// Support e-mail address
    /// </summary>
    private const string SUPPORT_MAIL = "support@kentico.com";


    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup page title text and image
        CurrentMaster.Title.TitleText = GetString("Licenses_License_List.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_LicenseKey/object.png");

        CurrentMaster.Title.HelpTopicName = "licenses_list";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Prepare the new class header element
        string[,] actions = new string[3,8];
        actions[0, 0] = "HyperLink";
        actions[0, 1] = GetString("Licenses_License_List.New");
        actions[0, 3] = "~/CMSModules/Licenses/Pages/License_New.aspx";
        actions[0, 5] = GetImageUrl("Objects/CMS_LicenseKey/add.png");
        actions[1, 0] = "HyperLink";
        actions[1, 1] = GetString("license.list.export");
        actions[1, 3] = "~/CMSModules/Licenses/Pages/License_Export_Domains.aspx";
        actions[1, 5] = GetImageUrl("Objects/CMS_LicenseKey/export.png");

        // Not supported in 5.0 final
        //actions[2, 1] = GetString("license.list.import");
        //actions[2, 3] = "~/CMSModules/Licenses/Pages/License_Import_Licenses.aspx";
        //actions[2, 5] = GetImageUrl("Objects/CMS_LicenseKey/import.png");

        CurrentMaster.HeaderActions.Actions = actions;

        UniGridLicenses.GridView.PageSize = 20;
        UniGridLicenses.OnAction += new OnActionEventHandler(UniGridLicenses_OnAction);
        UniGridLicenses.OnExternalDataBound += new OnExternalDataBoundEventHandler(UniGridLicenses_OnExternalDataBound);
        UniGridLicenses.ZeroRowsText = GetString("general.nodatafound");

        // Check if valid license for current domain exists
        LicenseValidationEnum validationResult = LicenseHelper.ValidateLicenseForDomain(URLHelper.GetCurrentDomain());
        if (validationResult != LicenseValidationEnum.Valid)
        {
            // Build invalid license string
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} <strong>{1}</strong><br /><br />", GetString("InvalidLicense.Result"), LicenseHelper.GetValidationResultString(validationResult))
              .AppendFormat("<strong>{0}</strong><br /><ul>", GetString("invalidlicense.howto"))
              .AppendFormat("<li>{0} ", GetString("invalidlicense.howto.option1.firstpart"))
              .AppendFormat("<a target=\"_blank\" href=\"{0}\" title=\"{1}\">{2}</a>", CLIENT_PORTAL, CLIENT_PORTAL, GetString("invalidlicense.clientportal"))
              .AppendFormat("{0}</li>", GetString("invalidlicense.howto.option1.secondpart"))
              .AppendFormat("<li>{0} <a href=\"mailto:{1}\">{2}</a>.</li>", GetString("invalidlicense.howto.option2"), SUPPORT_MAIL, SUPPORT_MAIL)
              .AppendFormat("<li>{0} ", GetString("invalidlicense.howto.option3.firstpart"))
              .AppendFormat("<a target=\"_blank\" href=\"{0}\" title=\"{1}\">{2}</a>", CLIENT_PORTAL, CLIENT_PORTAL, GetString("invalidlicense.clientportal"))
              .AppendFormat("{0}</li></ul>", GetString("invalidlicense.howto.option3.secondpart"));

            ShowWarning(GetString("invalidlicense.currentdomain"), sb.ToString(), null);
        }
    }


    /// <summary>
    /// External data binding handler.
    /// </summary>
    private object UniGridLicenses_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "editionname":
                string edition = ValidationHelper.GetString(parameter, "").ToLowerCSafe();
                try
                {
                    return LicenseHelper.GetEditionName(LicenseKeyInfoProvider.CharToEdition(edition));
                }
                catch
                {
                    return "#UNKNOWN#";
                }

            case "expiration":
                return GetString(Convert.ToString(parameter));

            case "licenseservers":
                int count = ValidationHelper.GetInteger(parameter, -1);
                if (count == LicenseKeyInfo.SERVERS_UNLIMITED)
                {
                    return GetString("general.unlimited");
                }
                else if (count > 0)
                {
                    return count.ToString();
                }
                return String.Empty;
        }
        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGridLicenses_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            LicenseKeyInfoProvider.DeleteLicenseKeyInfo(Convert.ToInt32(actionArgument));
        }
        else if (actionName == "view")
        {
            URLHelper.Redirect("License_View.aspx?licenseid=" + actionArgument.ToString());
        }
    }
}