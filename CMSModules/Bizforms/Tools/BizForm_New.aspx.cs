using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Linq;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_BizForms_Tools_BizForm_New : CMSBizFormPage
{
    private const string bizFormNamespace = "BizForm";
    private string mFormTablePrefix = null;


    /// <summary>
    /// Returns prefix for bizform table name.
    /// </summary>
    private string FormTablePrefix
    {
        get
        {
            if (string.IsNullOrEmpty(mFormTablePrefix))
            {
                mFormTablePrefix = String.Format("Form_{0}_", ValidationHelper.GetIdentifier(CMSContext.CurrentSiteName));
            }

            return mFormTablePrefix;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'CreateForm' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.form", "CreateForm"))
        {
            RedirectToCMSDeskAccessDenied("cms.form", "CreateForm");
        }

        // Validator initializations
        rfvFormDisplayName.ErrorMessage = GetString("BizForm_Edit.ErrorEmptyFormDispalyName");
        rfvFormName.ErrorMessage = GetString("BizForm_Edit.ErrorEmptyFormName");
        rfvTableName.ErrorMessage = GetString("BizForm_Edit.ErrorEmptyFormTableName");

        // Control initializations
        lblFormDisplayName.Text = GetString("BizForm_Edit.FormDisplayNameLabel");
        lblFormName.Text = GetString("BizForm_Edit.FormNameLabel");
        lblTableName.Text = GetString("BizForm_Edit.TableNameLabel");
        lblPrefix.Text = FormTablePrefix + "&nbsp;";
        // Remove prefix length from maximum allowed length of table name
        txtTableName.MaxLength = 100 - FormTablePrefix.Length;
        btnOk.Text = GetString("General.OK");

        // Page title control initialization
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = GetString("BizForm_Edit.ItemListLink");
        breadcrumbs[0, 1] = "~/CMSModules/BizForms/Tools/BizForm_List.aspx";
        breadcrumbs[0, 2] = "";
        breadcrumbs[1, 0] = GetString("BizForm_Edit.NewItemCaption");
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";

        CurrentMaster.Title.TitleText = GetString("BizForm_New.HeaderCaption");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Form/new.png");
        CurrentMaster.Title.Breadcrumbs = breadcrumbs;
        CurrentMaster.Title.HelpTopicName = "new_bizform";
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!BizFormInfoProvider.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.BizForms, VersionActionEnum.Insert))
        {
            ShowError(GetString("LicenseVersion.BizForm"));
            return;
        }

        DataClassInfo dci = null;
        BizFormInfo bizFormObj = null;
        
        string errorMessage = new Validator().NotEmpty(txtFormDisplayName.Text, rfvFormDisplayName.ErrorMessage).
            NotEmpty(txtFormName.Text, rfvFormName.ErrorMessage).
            NotEmpty(txtTableName.Text, rfvTableName.ErrorMessage).
            IsIdentifier(txtFormName.Text, GetString("BizForm_Edit.ErrorFormNameInIdentifierFormat")).
            IsIdentifier(txtTableName.Text, GetString("BizForm_Edit.ErrorFormTableNameInIdentifierFormat")).Result;

        if (String.IsNullOrEmpty(errorMessage))
        {
            using (var tr = new CMSTransactionScope())
            {
                // Prepare the values
                string formDisplayName = txtFormDisplayName.Text.Trim();

                bizFormObj = new BizFormInfo();
                bizFormObj.FormDisplayName = formDisplayName;
                bizFormObj.FormName = txtFormName.Text.Trim();
                bizFormObj.FormSiteID = CMSContext.CurrentSiteID;
                bizFormObj.FormEmailAttachUploadedDocs = true;
                bizFormObj.FormItems = 0;
                bizFormObj.FormClearAfterSave = false;
                bizFormObj.FormLogActivity = true;

                // Ensure the code name
                bizFormObj.Generalized.EnsureCodeName();

                // Table name is combined from prefix ('BizForm_<sitename>_') and custom table name
                string safeFormName = ValidationHelper.GetIdentifier(bizFormObj.FormName);
                string className = bizFormNamespace + "." + safeFormName;

                // Generate the table name
                string tableName = txtTableName.Text.Trim();
                if (String.IsNullOrEmpty(tableName) || (tableName == BaseInfo.CODENAME_AUTOMATIC))
                {
                    tableName = safeFormName;
                }
                tableName = FormTablePrefix + tableName;

                TableManager tm = new TableManager(null);

                // TableName wont be longer than 60 letters and will be unique
                if (tableName.Length > 60)
                {
                    int x = 1;

                    while (tm.TableExists(tableName.Substring(0, 59) + x.ToString()))
                    {
                        x++;
                    }

                    tableName = tableName.Substring(0, 59) + x.ToString();
                }

                // If first letter of safeFormName is digit, add "PK" to beginning
                string primaryKey = (Char.IsDigit(safeFormName.First()) ? "PK" + safeFormName + "ID" : safeFormName + "ID");

                try
                {
                    // Create new table in DB
                    tm.CreateTable(tableName, primaryKey);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;

                    // Table with the same name already exists
                    ShowError(string.Format(GetString("bizform_edit.errortableexists"), tableName));
                    return;
                }

                // Change table owner
                try
                {
                    string owner = SqlHelperClass.GetDBSchema(CMSContext.CurrentSiteName);
                    if ((!String.IsNullOrEmpty(owner)) && (owner.ToLowerCSafe() != "dbo"))
                    {
                        tm.ChangeDBObjectOwner(tableName, owner);
                        tableName = owner + "." + tableName;
                    }
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException("BIZFORM_NEW", "E", ex);
                }

                // Convert default datetime to string in english format
                string defaultDateTime = DateTime.Now.ToString(CultureHelper.EnglishCulture.DateTimeFormat);

                try
                {
                    // Add FormInserted and FormUpdated columns to the table
                    tm.AddTableColumn(tableName, "FormInserted", "datetime", false, defaultDateTime);
                    tm.AddTableColumn(tableName, "FormUpdated", "datetime", false, defaultDateTime);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;

                    // Column wasn't added successfully
                    ShowError(errorMessage);

                    return;
                }

                // Create the BizForm class
                dci = BizFormInfoProvider.CreateBizFormDataClass(className, formDisplayName, tableName, primaryKey);

                try
                {
                    // Create new bizform dataclass
                    using (CMSActionContext context = new CMSActionContext())
                    {
                        // Disable logging of tasks
                        context.DisableLogging();

                        DataClassInfoProvider.SetDataClass(dci);
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;

                    // Class with the same name already exists
                    ShowError(errorMessage);

                    return;
                }

                // Create new bizform
                bizFormObj.FormClassID = dci.ClassID;

                try
                {
                    // Create new bizform
                    BizFormInfoProvider.SetBizFormInfo(bizFormObj);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;

                    ShowError(errorMessage);

                    return;
                }

                tr.Commit();

                if (String.IsNullOrEmpty(errorMessage))
                {
                    // Redirect to edit dialog
                    URLHelper.Redirect(string.Format("BizForm_Frameset.aspx?formId={0}&newform=1", bizFormObj.FormID));
                }
            }
        }
        else
        {
            ShowError(errorMessage);
        }
    }
}