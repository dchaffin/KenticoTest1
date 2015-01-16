using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_General : SiteManagerPage
{
    private int webPartId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        lbWebPartCategory.Text = GetString("Development-WebPart_Edit.WebPartCategory");
        lblWebPartType.Text = GetString("Development-WebPart_Edit.WebPartType");
        lblUploadFile.Text = GetString("Development-WebPart_Edit.lblUpload");
        btnOk.Text = GetString("general.ok");
        rfvWebPartDisplayName.ErrorMessage = GetString("Development-WebPart_Edit.ErrorDisplayName");
        rfvWebPartName.ErrorMessage = GetString("Development-WebPart_Edit.ErrorWebPartName");

        lblLoadGeneration.Text = GetString("LoadGeneration.Title");
        plcDevelopment.Visible = SettingsKeyProvider.DevelopmentMode;

        // Get the webpart ID
        webPartId = QueryHelper.GetInteger("webpartID", 0);
        WebPartInfo wi = null;

        // fill in the form, edit webpart
        if (!RequestHelper.IsPostBack())
        {
            // Fill webpart type drop down list
            DataHelper.FillListControlWithEnum(typeof(WebPartTypeEnum), drpWebPartType, "Development-WebPart_Edit.Type", null);

            // edit web part
            if (webPartId > 0)
            {
                wi = WebPartInfoProvider.GetWebPartInfo(webPartId);
                EditedObject = wi;
                if (wi != null)
                {
                    txtWebPartDescription.Text = wi.WebPartDescription;
                    txtWebPartDisplayName.Text = wi.WebPartDisplayName;
                    FileSystemSelector.Value = wi.WebPartFileName;
                    txtWebPartName.Text = wi.WebPartName;
                    drpWebPartType.SelectedValue = wi.WebPartType.ToString();
                    chkSkipInsertProperties.Checked = wi.WebPartSkipInsertProperties;

                    drpGeneration.Value = wi.WebPartLoadGeneration;
                    drpModule.Value = wi.WebPartResourceID;

                    if (wi.WebPartParentID != 0)
                    {
                        WebPartInfo parentInfo = WebPartInfoProvider.GetWebPartInfo(wi.WebPartParentID);

                        if (parentInfo != null)
                        {
                            txtInheritedName.Text = parentInfo.WebPartDisplayName;
                        }

                        plcFileSystemSelector.Visible = false;
                        plcInheritedName.Visible = true;
                        lblWebPartFileName.ResourceString = "DevelopmentWebPartGeneral.InheritedWebPart";
                    }


                    WebPartCategoryInfo ci = WebPartCategoryInfoProvider.GetWebPartCategoryInfoById(wi.WebPartCategoryID);
                    if (ci != null)
                    {
                        categorySelector.Value = ci.CategoryID.ToString();
                    }

                    // Init file uploader
                    lblUploadFile.Visible = true;

                    attachmentFile.Visible = true;
                    attachmentFile.ObjectID = webPartId;
                    attachmentFile.ObjectType = PortalObjectType.WEBPART;
                    attachmentFile.Category = MetaFileInfoProvider.OBJECT_CATEGORY_THUMBNAIL;
                }
            }
            else
            {
                int parentCategoryId = QueryHelper.GetInteger("parentId", 0);
                categorySelector.Value = parentCategoryId.ToString();

                lblUploadFile.Visible = false;
                attachmentFile.Visible = false;
            }
        }

        // Initialize the master page elements
        Title = "Web parts - Edit";

        FileSystemDialogConfiguration config = new FileSystemDialogConfiguration();
        config.StartingPath = "~/CMSWebParts/";
        config.DefaultPath = "CMSWebParts";
        config.AllowedExtensions = "ascx";
        config.ShowFolders = false;

        FileSystemSelector.DialogConfig = config;
        FileSystemSelector.AllowEmptyValue = false;
        FileSystemSelector.SelectedPathPrefix = "~/CMSWebParts/";
    }


    /// <summary>
    /// Updates existing or creates new web part.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Validate the text box fields
        string errorMessage = new Validator().IsCodeName(txtWebPartName.Text, GetString("general.invalidcodename")).Result;

        if (errorMessage != String.Empty)
        {
            ShowError(errorMessage);
            return;
        }

        WebPartInfo wi = WebPartInfoProvider.GetWebPartInfo(webPartId);
        if (wi != null)
        {
            string webpartPath = GetWebPartPhysicalPath(FileSystemSelector.Value.ToString());

            txtWebPartName.Text = TextHelper.LimitLength(txtWebPartName.Text.Trim(), 100, "");
            txtWebPartDisplayName.Text = TextHelper.LimitLength(txtWebPartDisplayName.Text.Trim(), 100, "");

            // Perform validation
            errorMessage = new Validator().NotEmpty(txtWebPartName.Text, rfvWebPartName.ErrorMessage).IsCodeName(txtWebPartName.Text, GetString("general.invalidcodename"))
                .NotEmpty(txtWebPartDisplayName.Text, rfvWebPartDisplayName.ErrorMessage).Result;

            // Check file name
            if (wi.WebPartParentID <= 0)
            {
                if (!FileSystemSelector.IsValid())
                {
                    errorMessage += FileSystemSelector.ValidationError;
                }
            }

            if (errorMessage != String.Empty)
            {
                ShowError(errorMessage);
                return;
            }


            string oldDisplayName = wi.WebPartDisplayName;
            string oldCodeName = wi.WebPartName;
            int oldCategory = wi.WebPartCategoryID;

            // Remove starting CMSwebparts folder
            string filename = FileSystemSelector.Value.ToString().Trim();
            if (filename.ToLowerCSafe().StartsWithCSafe("~/cmswebparts/"))
            {
                filename = filename.Substring("~/cmswebparts/".Length);
            }


            // If name changed, check if new name is unique
            if (CMSString.Compare(wi.WebPartName, txtWebPartName.Text, true) != 0)
            {
                WebPartInfo webpart = WebPartInfoProvider.GetWebPartInfo(txtWebPartName.Text);
                if (webpart != null)
                {
                    ShowError(GetString("Development.WebParts.WebPartNameAlreadyExist").Replace("%%name%%", txtWebPartName.Text));
                    return;
                }
            }

            wi.WebPartName = txtWebPartName.Text;
            wi.WebPartDisplayName = txtWebPartDisplayName.Text;
            wi.WebPartDescription = txtWebPartDescription.Text.Trim();
            wi.WebPartFileName = filename;
            wi.WebPartType = ValidationHelper.GetInteger(drpWebPartType.SelectedValue, 0);
            wi.WebPartSkipInsertProperties = chkSkipInsertProperties.Checked;

            wi.WebPartLoadGeneration = drpGeneration.Value;
            wi.WebPartResourceID = ValidationHelper.GetInteger(drpModule.Value, 0);

            FileSystemSelector.Value = wi.WebPartFileName;

            wi.WebPartCategoryID = ValidationHelper.GetInteger(categorySelector.Value, 0);

            WebPartInfoProvider.SetWebPartInfo(wi);

            // if DisplayName or Category was changed, then refresh web part tree and header
            if ((oldCodeName != wi.WebPartName) || (oldDisplayName != wi.WebPartDisplayName) || (oldCategory != wi.WebPartCategoryID))
            {
                ltlScript.Text += ScriptHelper.GetScript(
                    "parent.parent.frames['webparttree'].location.replace('WebPart_Tree.aspx?webpartid=" + wi.WebPartID + "'); \n" +
                    "parent.frames['webparteditheader'].location.replace(parent.frames['webparteditheader'].location.href); \n"
                    );
            }

            ShowChangesSaved();
        }
    }


    private string GetWebPartPhysicalPath(string webpartPath)
    {
        webpartPath = webpartPath.Trim();

        if (webpartPath.StartsWithCSafe("~/"))
        {
            return Server.MapPath(webpartPath);
        }

        string fileName = webpartPath.Trim('/').Replace('/', '\\');
        return Path.Combine(Server.MapPath(WebPartInfoProvider.WebPartsDirectory), fileName);
    }
}