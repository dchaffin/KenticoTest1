using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_General : CMSEditTemplatePage
{
    #region "Variables"

    private bool dialog = false;

    protected string pageTemplateWebParts = "";
    protected bool pageTemplateIsReusable = false;

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        dialog = QueryHelper.GetBoolean("dialog", false);

        // Check template ID
        if (PageTemplateID <= 0)
        {
            return;
        }

        // Register refresh script
        string refreshScript = ScriptHelper.GetScript(
@"
function RefreshContent() {
    if ((parent != null) && (parent.Refresh != null)) parent.Refresh();
    var txtDisplayName = document.getElementById('" + txtTemplateDisplayName.TextBox.ClientID + @"');
    var wopener = parent.wopener; if ((wopener != null) && (wopener.SetTemplateName)) wopener.SetTemplateName(txtDisplayName.value);
}
"
        );

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "pageTemplateRefreshScript", refreshScript);

        // Hide teaser
        lblUploadFile.Visible = false;
        UploadFile.Visible = false;

        lblTemplateDisplayName.Text = GetString("Administration-PageTemplate_General.TemplateDisplayName");
        lblTemplateCodeName.Text = GetString("Administration-PageTemplate_General.TemplateCodeName");
        lblTemplateCategory.Text = GetString("Administration-PageTemplate_General.Category");
        lblTemplateDescription.Text = GetString("Administration-PageTemplate_General.TemplateDescription");
        lblTemplateType.Text = GetString("Administration-PageTemplate_General.Type");
        lblShowAsMasterTemplate.Text = GetString("Administration-PageTemplate_General.ShowAsMasterTemplate");
        lblInheritLevels.Text = GetString("Administration-PageTemplate_General.InheritLevels");
        lblUploadFile.Text = GetString("Administration-PageTemplate_General.lblUpload");

        rfvTemplateDisplayName.ErrorMessage = GetString("Administration-PageTemplate_General.ErrorEmptyTemplateDisplayName");
        rfvTemplateCodeName.ErrorMessage = GetString("Administration-PageTemplate_General.ErrorEmptyTemplateCodeName");

        pnlObjectLocking.ObjectManager.OnSaveData += new EventHandler<SimpleObjectManagerEventArgs>(ObjectManager_OnSaveData);
        pnlObjectLocking.ObjectManager.OnAfterAction += new EventHandler<SimpleObjectManagerEventArgs>(ObjectManager_OnAfterAction);

        if (!RequestHelper.IsPostBack())
        {
            FillDropDown();
        }

        if (PageTemplate != null)
        {
            plcAdHoc.Visible = (PageTemplate.PageTemplateType == PageTemplateTypeEnum.Portal) && PageTemplate.IsReusable;

            if (!RequestHelper.IsPostBack())
            {
                // Load the initial data
                LoadData();

                lvlElem.Value = PageTemplate.InheritPageLevels;
            }
            else
            {
                // Load just the flags
                pageTemplateIsReusable = PageTemplate.IsReusable;
                pageTemplateWebParts = PageTemplate.WebParts;
            }

            // Show teaser if needed
            if (!PageTemplate.DisplayName.ToLowerCSafe().StartsWithCSafe("ad-hoc"))
            {
                lblUploadFile.Visible = true;
                UploadFile.Visible = true;
                UploadFile.ObjectID = PageTemplateID;
                UploadFile.ObjectType = PortalObjectType.PAGETEMPLATE;
                UploadFile.Category = MetaFileInfoProvider.OBJECT_CATEGORY_THUMBNAIL;
            }

            FileSystemDialogConfiguration config = new FileSystemDialogConfiguration();
            config.DefaultPath = "CMSTemplates";
            config.AllowedExtensions = "aspx";
            config.ShowFolders = false;

            FileSystemSelector.DialogConfig = config;
            FileSystemSelector.AllowEmptyValue = false;
            FileSystemSelector.SelectedPathPrefix = "~/CMSTemplates/";
            FileSystemSelector.ValidationError = GetString("Administration-PageTemplate_General.ErrorEmptyTemplateFileName");

            // Script for dynamic hiding of inherited levels
            string script =
@"
function HideOrShowInheritLevels() {                
    var tr = document.getElementById('" + inheritLevels.ClientID + @"');                
    if (tr) {                     
        var checkbox = document.getElementById('" + chkShowAsMasterTemplate.ClientID + @"') ;                               
        if(checkbox != null) {
            if(checkbox.checked == 1) {
                tr.style.display = 'none';
            }
            else {
                tr.style.display = '';
            }
        }
    }            
}
"
            ;

            // Hide inherited levels
            if (chkShowAsMasterTemplate.Checked)
            {
                inheritLevels.Style.Add("display", "none");
            }
            else
            {
                inheritLevels.Style.Clear();
            }

            // Register script to page
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "showOrHide", ScriptHelper.GetScript(script));

            chkShowAsMasterTemplate.Attributes.Add("onclick", "HideOrShowInheritLevels();");
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (pnlObjectLocking.IsLocked)
        {
            UploadFile.Enabled = false;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Fills the page type dropdown list.
    /// </summary>
    protected void FillDropDown()
    {
        drpPageType.Items.Add(new ListItem(GetString("Administration-PageTemplate_General.PortalPage"), "portal"));
        drpPageType.Items.Add(new ListItem(GetString("Administration-PageTemplate_General.AspxPage"), "aspx"));
        drpPageType.Items.Add(new ListItem(GetString("PageTemplate.CombinedPage"), "combined"));

        // Hide dashboard type if current template is edited from CMSDesk
        if (!dialog)
        {
            drpPageType.Items.Add(new ListItem(GetString("PageTemplate.Dashboard"), "dashboard"));
        }

        drpPageType.Items.Add(new ListItem(GetString("PageTemplate.MVC"), "mvc"));
    }


    /// <summary>
    /// Gets the selected page template mode.
    /// </summary>
    private PageTemplateTypeEnum SelectedPageType
    {
        get
        {
            switch (drpPageType.SelectedValue)
            {
                case "portal":
                    return PageTemplateTypeEnum.Portal;

                case "aspx":
                    return PageTemplateTypeEnum.Aspx;

                case "combined":
                    return PageTemplateTypeEnum.AspxPortal;

                case "dashboard":
                    return PageTemplateTypeEnum.Dashboard;

                case "mvc":
                    return PageTemplateTypeEnum.MVC;

                default:
                    return PageTemplateTypeEnum.Unknown;
            }
        }
    }


    /// <summary>
    /// Load data of edited module from DB into TextBoxes.
    /// </summary>
    protected void LoadData()
    {
        if (PageTemplate != null)
        {
            txtTemplateDisplayName.Text = PageTemplate.DisplayName;
            txtTemplateCodeName.Text = PageTemplate.CodeName;
            txtTemplateDescription.Text = PageTemplate.Description;

            // Select category
            categorySelector.Value = PageTemplate.CategoryID.ToString();

            switch (PageTemplate.PageTemplateType)
            {
                case PageTemplateTypeEnum.Portal:
                    // Portal only
                    drpPageType.SelectedValue = "portal";
                    break;

                case PageTemplateTypeEnum.Aspx:
                    // ASPX only
                    drpPageType.SelectedValue = "aspx";
                    FileSystemSelector.Value = PageTemplate.FileName;
                    break;


                case PageTemplateTypeEnum.AspxPortal:
                    // Combined
                    drpPageType.SelectedValue = "combined";
                    FileSystemSelector.Value = PageTemplate.FileName;
                    break;

                case PageTemplateTypeEnum.Dashboard:
                    drpPageType.SelectedValue = "dashboard";
                    break;

                case PageTemplateTypeEnum.MVC:
                    // MVC
                    drpPageType.SelectedValue = "mvc";
                    txtController.Text = PageTemplate.PageTemplateDefaultController;
                    txtAction.Text = PageTemplate.PageTemplateDefaultAction;
                    break;
            }

            radAspx_CheckedChange(null, null);

            chkShowAsMasterTemplate.Checked = PageTemplate.ShowAsMasterTemplate;
            chkAdHoc.Checked = PageTemplate.PageTemplateCloneAsAdHoc;

            pageTemplateIsReusable = PageTemplate.IsReusable;
            pageTemplateWebParts = PageTemplate.WebParts;
        }
    }


    /// <summary>
    /// Shows controls for layout editing and hides controls for template file editing.
    /// </summary>
    /// <param name="portal">Show portal controls</param>
    /// <param name="aspx">Show ASPX controls</param>
    protected void SetPageType(PageTemplateTypeEnum type)
    {
        plcAspx.Visible = ((type == PageTemplateTypeEnum.Aspx) || (type == PageTemplateTypeEnum.AspxPortal));
        plcPortal.Visible = (type == PageTemplateTypeEnum.Portal);
        plcAdHoc.Visible = (type == PageTemplateTypeEnum.Portal) && PageTemplate.IsReusable;
        plcMvc.Visible = (type == PageTemplateTypeEnum.MVC);
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Reloads data after save.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if (e.ActionName != ComponentEvents.SAVE)
        {
            LoadData();
        }

        if (dialog)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "parentWOpenerRefresh", ScriptHelper.GetScript("if (parent && parent.wopener && parent.wopener.refresh) { parent.wopener.refresh(); }"));
        }
    }


    /// <summary>
    /// Save button action.
    /// </summary>
    protected void ObjectManager_OnSaveData(object sender, SimpleObjectManagerEventArgs e)
    {
        // Template has to exist
        if (PageTemplate == null)
        {
            return;
        }

        // Limit text length
        txtTemplateCodeName.Text = TextHelper.LimitLength(txtTemplateCodeName.Text.Trim(), 100, "");
        txtTemplateDisplayName.Text = TextHelper.LimitLength(txtTemplateDisplayName.Text.Trim(), 200, "");

        // Finds whether required fields are not empty
        string result = String.Empty;

        result = new Validator().NotEmpty(txtTemplateDisplayName.Text, GetString("Administration-PageTemplate_General.ErrorEmptyTemplateDisplayName")).NotEmpty(txtTemplateCodeName.Text, GetString("Administration-PageTemplate_General.ErrorEmptyTemplateCodeName"))
            .IsCodeName(txtTemplateCodeName.Text, GetString("general.invalidcodename"))
            .Result;

        if ((result == String.Empty) && (SelectedPageType == PageTemplateTypeEnum.Aspx || SelectedPageType == PageTemplateTypeEnum.AspxPortal))
        {
            if (!FileSystemSelector.IsValid())
            {
                result = FileSystemSelector.ValidationError;
            }
        }

        // If name changed, check if new name is unique
        if ((result == String.Empty) && (CMSString.Compare(PageTemplate.CodeName, txtTemplateCodeName.Text, true) != 0))
        {
            if (PageTemplateInfoProvider.PageTemplateNameExists(txtTemplateCodeName.Text))
            {
                result = GetString("general.codenameexists");
            }
        }


        // Check dashboard prerequisites
        if ((PageTemplate.PageTemplateId > 0) && (SelectedPageType == PageTemplateTypeEnum.Dashboard))
        {
            // Check live site usage
            TreeProvider tp = new TreeProvider(CMSContext.CurrentUser);
            NodeSelectionParameters parameters = new NodeSelectionParameters()
            {
                ClassNames = TreeProvider.ALL_CLASSNAMES,
                SiteName = TreeProvider.ALL_SITES,
                Columns = "NodeID",
                Where = String.Format("DocumentPageTemplateID = {0} OR NodeTemplateID = {0} OR NodeWireframeTemplateID = {0}", PageTemplate.PageTemplateId)
            };

            DataSet ds = tp.SelectNodes(parameters);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                result = GetString("template.dahsboardliveused");
            }

            // Check valid zones
            if (String.IsNullOrEmpty(result))
            {
                PageTemplateInstance inst = PageTemplate.TemplateInstance;
                if (inst != null)
                {
                    foreach (WebPartZoneInstance zone in inst.WebPartZones)
                    {
                        switch (zone.WidgetZoneType)
                        {
                            case WidgetZoneTypeEnum.Dashboard:
                            case WidgetZoneTypeEnum.None:
                                continue;
                        }

                        result = GetString("template.dashboardinvalidzone");
                        break;
                    }
                }
            }
        }

        if (String.IsNullOrEmpty(result))
        {
            // Update page template info                        
            PageTemplate.DisplayName = txtTemplateDisplayName.Text;
            PageTemplate.CodeName = txtTemplateCodeName.Text;
            PageTemplate.Description = txtTemplateDescription.Text;
            PageTemplate.CategoryID = Convert.ToInt32(categorySelector.Value);

            if (SelectedPageType == PageTemplateTypeEnum.MVC)
            {
                // MVC template
                PageTemplate.IsPortal = false;
                PageTemplate.FileName = String.Empty;

                PageTemplate.ShowAsMasterTemplate = false;
                PageTemplate.PageTemplateCloneAsAdHoc = false;

                PageTemplate.PageTemplateDefaultController = txtController.Text;
                PageTemplate.PageTemplateDefaultAction = txtAction.Text;
            }
            else if (SelectedPageType == PageTemplateTypeEnum.Portal)
            {
                // Portal template of various types
                PageTemplate.IsPortal = true;
                PageTemplate.FileName = String.Empty;

                // Save inherit levels
                if (!chkShowAsMasterTemplate.Checked)
                {
                    PageTemplate.InheritPageLevels = ValidationHelper.GetString(lvlElem.Value, "");
                }
                else
                {
                    PageTemplate.InheritPageLevels = "/";
                }

                // Show hide inherit levels radio buttons
                PageTemplate.ShowAsMasterTemplate = chkShowAsMasterTemplate.Checked;
                PageTemplate.PageTemplateCloneAsAdHoc = chkAdHoc.Checked;
            }
            else
            {
                // ASPX page templates
                PageTemplate.IsPortal = false;
                PageTemplate.FileName = FileSystemSelector.Value.ToString();

                PageTemplate.ShowAsMasterTemplate = false;
                PageTemplate.PageTemplateCloneAsAdHoc = false;

                PageTemplate.InheritPageLevels = "";
            }

            PageTemplate.PageTemplateType = SelectedPageType;

            // Set ad-hoc status
            PageTemplate.IsReusable = pageTemplateIsReusable;
            if (pageTemplateIsReusable)
            {
                PageTemplate.PageTemplateNodeGUID = Guid.Empty;
            }

            PageTemplate.WebParts = pageTemplateWebParts;

            try
            {
                // Save the template and update the header
                PageTemplateInfoProvider.SetPageTemplateInfo(PageTemplate);
                ScriptHelper.RegisterStartupScript(this, typeof(string), "pageTemplateSaveScript", ScriptHelper.GetScript("RefreshContent()"));
                ShowChangesSaved();
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowError(ResHelper.GetStringFormat("general.sourcecontrolerror", ex.Message));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        else
        {
            rfvTemplateDisplayName.Visible = false;
            rfvTemplateCodeName.Visible = false;

            ShowError(result);
        }
    }


    /// <summary>
    /// Handles onCheckedChange event of radAspx radio button.
    /// </summary>
    protected void radAspx_CheckedChange(object sender, EventArgs e)
    {
        SetPageType(SelectedPageType);
    }

    #endregion
}