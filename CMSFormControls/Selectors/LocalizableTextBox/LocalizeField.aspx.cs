using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.ResourceManager;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSFormControls_Selectors_LocalizableTextBox_LocalizeField : CMSModalPage
{
    #region "Variables"

    private string hdnValue = null;
    private string textbox = null;
    private string hdnIsMacro = null;
    private string btnLocalizeField = null;
    private string btnLocalizeString = null;
    private string btnRemoveLocalization = null;
    private string plainText = null;
    private string identifier = null;
    private string resourceKeyPrefix = null;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions
        if (CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Localization", "LocalizeStrings"))
        {
            // Set title
            CurrentMaster.Title.TitleText = GetString("localizable.localizefield");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_UICulture/new.png");
            CurrentMaster.Title.HelpTopicName = "localize_field";
            CurrentMaster.Title.HelpName = "helpTopic";

            // Validate hash
            Regex re = RegexHelper.GetRegex(@"[\w\d_$$]*");
            identifier = QueryHelper.GetString("params", "");

            if (!QueryHelper.ValidateHash("hash") || !re.IsMatch(identifier))
            {
                pnlContent.Visible = false;
                return;
            }

            // Load dialog parameters
            Hashtable parameters = (Hashtable)WindowHelper.GetItem(identifier);
            if (parameters != null)
            {
                hdnValue = ValidationHelper.GetString(parameters["HiddenValue"], String.Empty);
                textbox = ValidationHelper.GetString(parameters["TextBoxID"], String.Empty);
                hdnIsMacro = ValidationHelper.GetString(parameters["HiddenIsMacro"], String.Empty);
                plainText = ValidationHelper.GetString(parameters["TextBoxValue"], String.Empty);
                btnLocalizeField = ValidationHelper.GetString(parameters["ButtonLocalizeField"], String.Empty);
                btnLocalizeString = ValidationHelper.GetString(parameters["ButtonLocalizeString"], String.Empty);
                btnRemoveLocalization = ValidationHelper.GetString(parameters["ButtonRemoveLocalization"], String.Empty);
                resourceKeyPrefix = ValidationHelper.GetString(parameters["ResourceKeyPrefix"], String.Empty);
            }
            btnOk.Click += btnOk_Click;

            lstExistingOrNew.Items[0].Text = GetString("localizable.createnew");
            lstExistingOrNew.Items[1].Text = GetString("localizable.useexisting");

            // Disable option to use existing resource string for user who is not admin
            if (!CurrentUser.UserSiteManagerAdmin)
            {
                lstExistingOrNew.Items[1].Enabled = false;
            }

            // If "create new" is selected
            if (lstExistingOrNew.SelectedIndex == 0)
            {
                if (!SettingsKeyProvider.DevelopmentMode)
                {
                    lblPrefix.Text = resourceKeyPrefix;
                    lblPrefix.Visible = true;
                }
                lblSelectKey.ResourceString = GetString("localizable.newkey");
                resourceSelector.Visible = false;
                txtNewResource.Visible = true;

                if (!RequestHelper.IsPostBack())
                {
                    txtNewResource.Text = ResHelper.GetUniqueResStringKey(plainText, resourceKeyPrefix, SettingsKeyProvider.DevelopmentMode);
                }
            }
            // If "use existing" is selected
            else
            {
                lblSelectKey.ResourceString = GetString("localizable.existingkey");
                resourceSelector.Visible = true;
                txtNewResource.Visible = false;
            }
        }
        // Dialog is not available for unauthorized user
        else
        {
            ShowError(GetString("security.accesspage.onlyglobaladmin"));
            pnlControls.Visible = false;
        }
    }


    /// <summary>
    /// Button OK clicked.
    /// </summary>
    private void btnOk_Click(object sender, EventArgs e)
    {
        // Check permissions
        if (CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Localization", "LocalizeStrings"))
        {
            string key = null;
            ResourceStringInfo ri = null;

            // Check if current user's culture exists
            UICultureInfo uiCulture = null;
            string cultureCode = CultureHelper.PreferredUICulture;
            try
            {
                uiCulture = UICultureInfoProvider.GetUICultureInfo(CultureHelper.PreferredUICulture);
            }
            // Use default UI culture
            catch
            {
                cultureCode = CultureHelper.DefaultUICulture;
            }
            // Use default UI culture
            if (uiCulture == null)
            {
                cultureCode = CultureHelper.DefaultUICulture;
            }

            // Creating new resource string
            if (lstExistingOrNew.SelectedIndex == 0)
            {
                if (SettingsKeyProvider.DevelopmentMode)
                {
                    key = txtNewResource.Text.Trim();
                }
                else
                {
                    key = resourceKeyPrefix + txtNewResource.Text.Trim();
                }
                ri = SqlResourceManager.GetResourceStringInfo(key);

                // Resource string doesn't exists yet
                if (ri == null)
                {
                    string error = new Validator().NotEmpty(key, GetString("Administration-UICulture_String_New.EmptyKey")).IsCodeName(key, GetString("Administration-UICulture_String_New.InvalidCodeName")).Result;
                    if (!String.IsNullOrEmpty(error))
                    {
                        ShowError(error);
                    }
                    else
                    {
                        // Save ResourceString
                        ri = new ResourceStringInfo();
                        ri.StringKey = key;
                        ri.UICultureCode = cultureCode;
                        ri.TranslationText = plainText;
                        ri.StringIsCustom = !SettingsKeyProvider.DevelopmentMode;
                        SqlResourceManager.SetResourceStringInfo(ri);

                        ScriptHelper.RegisterStartupScript(this, typeof(string), "localizeField", ScriptHelper.GetScript("wopener.SetResourceAndOpen('" + hdnValue + "', '" + key + "', '" + textbox + "', " + ScriptHelper.GetString(plainText) + ", '" + hdnIsMacro + "', '" + btnLocalizeField + "', '" + btnLocalizeString + "', '" + btnRemoveLocalization + "'); CloseDialog();"));
                    }
                }
                // If resource string already exists with different translation
                else
                {
                    ShowError(GetString("localize.alreadyexists"));
                }
            }
            // Using existing resource string
            else
            {
                key = ValidationHelper.GetString(resourceSelector.Value, String.Empty).Trim();
                ri = SqlResourceManager.GetResourceStringInfo(key);

                // Key not found in DB
                if (ri == null)
                {
                    // Try to find it in .resx file and save it in DB
                    FileResourceManager resourceManager = ResHelper.GetFileManager(cultureCode);
                    if (resourceManager != null)
                    {
                        string translation = resourceManager.GetString(key);
                        if (!String.IsNullOrEmpty(translation))
                        {
                            ri = new ResourceStringInfo();
                            ri.StringKey = key;
                            ri.StringIsCustom = !SettingsKeyProvider.DevelopmentMode;
                            ri.UICultureCode = cultureCode;
                            ri.TranslationText = translation;
                            SqlResourceManager.SetResourceStringInfo(ri);

                            ScriptHelper.RegisterStartupScript(this, typeof(string), "localizeField", ScriptHelper.GetScript("wopener.SetResource('" + hdnValue + "', '" + key + "', '" + textbox + "', " + ScriptHelper.GetString(translation) + ", '" + hdnIsMacro + "', '" + btnLocalizeField + "', '" + btnLocalizeString + "', '" + btnRemoveLocalization + "'); CloseDialog();"));
                        }
                        else
                        {
                            ShowError(GetString("localize.doesntexist"));
                        }
                    }
                    else
                    {
                        ShowError(GetString("localize.doesntexist"));
                    }
                }
                // Send to parent window selected resource key
                else
                {
                    string existingTranslation = GetString(key);
                    ScriptHelper.RegisterStartupScript(this, typeof(string), "localizeField", ScriptHelper.GetScript("wopener.SetResource('" + hdnValue + "', '" + key + "', '" + textbox + "', " + ScriptHelper.GetString(existingTranslation) + ", '" + hdnIsMacro + "', '" + btnLocalizeField + "', '" + btnLocalizeString + "', '" + btnRemoveLocalization + "'); CloseDialog();"));
                }
            }
        }
        else
        {
            ShowError(GetString("general.actiondenied"));
            pnlControls.Visible = false;
        }
    }

    #endregion
}