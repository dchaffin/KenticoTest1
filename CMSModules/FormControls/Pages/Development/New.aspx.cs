using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_FormControls_Pages_Development_New : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            drpTypeSelector.ControlType = FormUserControlTypeEnum.Input;
        }
        CurrentMaster.DisplaySiteSelectorPanel = true;
        rfvControlName.ErrorMessage = GetString("Development_FormUserControl_Edit.rfvCodeName");
        rfvControlDisplayName.ErrorMessage = GetString("Development_FormUserControl_Edit.rfvDisplayName");

        // Initialize breadcrumbs
        string[,] breadcrumbs = new string[2,3];
        breadcrumbs[0, 0] = GetString("Development_FormUserControl_Edit.Controls");
        breadcrumbs[0, 1] = "~/CMSModules/FormControls/Pages/Development/List.aspx";
        breadcrumbs[0, 2] = "";
        breadcrumbs[1, 0] = GetString("Development_FormUserControl_Edit.New");
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";

        // Initialize page
        CurrentMaster.Title.Breadcrumbs = breadcrumbs;
        CurrentMaster.Title.TitleText = GetString("Development_FormUserControl_Edit.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_FormControl/new.png");
        CurrentMaster.Title.HelpTopicName = "new_form_control";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Initialize file selector
        FileSystemDialogConfiguration config = new FileSystemDialogConfiguration();
        config.DefaultPath = "CMSFormControls";
        config.AllowedExtensions = "ascx";
        config.ShowFolders = false;
        tbFileName.DialogConfig = config;
        tbFileName.AllowEmptyValue = false;
        tbFileName.SelectedPathPrefix = "~/CMSFormControls/";
        tbFileName.ValidationError = GetString("Development_FormUserControl_Edit.rfvFileName"); 
    }


    /// <summary>
    /// Handles btnOK's OnClick event.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Finds whether required fields are not empty
        string result = new Validator().NotEmpty(txtControlName.Text, rfvControlName.ErrorMessage).NotEmpty(txtControlDisplayName, rfvControlDisplayName.ErrorMessage).Result;

        // Check input file validity
        if (String.IsNullOrEmpty(result) && radNewControl.Checked)
        {
            if (!tbFileName.IsValid())
            {
                result = tbFileName.ValidationError;
            }
        }

        // Try to create new form control if everything is OK
        if (String.IsNullOrEmpty(result))
        {
            FormUserControlInfo fi = new FormUserControlInfo();
            fi.UserControlDisplayName = txtControlDisplayName.Text.Trim();
            fi.UserControlCodeName = txtControlName.Text.Trim();
            fi.UserControlType = drpTypeSelector.ControlType;
            fi.UserControlForText = false;
            fi.UserControlForLongText = false;
            fi.UserControlForInteger = false;
            fi.UserControlForLongInteger = false;
            fi.UserControlForDecimal = false;
            fi.UserControlForDateTime = false;
            fi.UserControlForBoolean = false;
            fi.UserControlForFile = false;
            fi.UserControlForDocAttachments = false;
            fi.UserControlForGUID = false;
            fi.UserControlForVisibility = false;
            fi.UserControlShowInBizForms = false;
            fi.UserControlDefaultDataType = "Text";

            // Inherited user control
            if (radInheritedControl.Checked)
            {
                fi.UserControlParentID = ValidationHelper.GetInteger(drpFormControls.Value, 0);

                // Create empty default values definition
                fi.UserControlParameters = "<form></form>";
                fi.UserControlFileName = "inherited";
            }
            else
            {
                fi.UserControlFileName = tbFileName.Value.ToString();
            }

            try
            {
                FormUserControlInfoProvider.SetFormUserControlInfo(fi);

                // If control was successfully created then redirect to editing page
                URLHelper.Redirect("Frameset.aspx?controlId=" + Convert.ToString(fi.UserControlID));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message.Replace("%%name%%", fi.UserControlCodeName));
            }
        }
        else
        {
            ShowError(result);
        }
    }


    protected void radNewFormControl_CheckedChanged(object sender, EventArgs e)
    {
        plcFileName.Visible = radNewControl.Checked;
        plcFormControls.Visible = radInheritedControl.Checked;
    }
}