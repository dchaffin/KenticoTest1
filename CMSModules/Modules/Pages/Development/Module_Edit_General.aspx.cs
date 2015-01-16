using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Modules_Pages_Development_Module_Edit_General : SiteManagerPage
{
    protected int moduleId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        lbModuleDisplayName.Text = GetString("Administration-Module_New.ModuleDisplayName");
        lbModuleCodeName.Text = GetString("Administration-Module_New.ModuleCodeName");
        lblModuleDescription.Text = GetString("Administration-Module_New.ModuleDescription");
        rfvModuleDisplayName.ErrorMessage = GetString("Administration-Module_New.ErrorEmptyModuleDisplayName");
        rfvModuleCodeName.ErrorMessage = GetString("Administration-Module_New.ErrorEmptyModuleCodeName");

        moduleId = QueryHelper.GetInteger("moduleID", 0);
        if (moduleId > 0)
        {
            if (!RequestHelper.IsPostBack())
            {
                LoadData();
            }
        }
    }


    /// <summary>
    /// Handles btnOK's OnClick event - Update resource info.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Finds whether required fields are not empty
        string result = new Validator().NotEmpty(tbModuleDisplayName.Text.Trim(), GetString("Administration-Module_New.ErrorEmptyModuleDisplayName")).NotEmpty(tbModuleCodeName.Text, GetString("Administration-Module_New.ErrorEmptyModuleCodeName"))
            .IsCodeName(tbModuleCodeName.Text, GetString("general.invalidcodename"))
            .Result;

        if (result == "")
        {
            // Check unique name
            ResourceInfo ri = ResourceInfoProvider.GetResourceInfo(tbModuleCodeName.Text);
            if ((ri == null) || (ri.ResourceId == moduleId))
            {
                // Get object
                if (ri == null)
                {
                    ri = ResourceInfoProvider.GetResourceInfo(moduleId);
                    if (ri == null)
                    {
                        ri = new ResourceInfo();
                    }
                }

                // Update resource info
                ri.ResourceId = moduleId;
                ri.ResourceName = tbModuleCodeName.Text;
                ri.ResourceDescription = txtModuleDescription.Text.Trim();
                ri.ResourceDisplayName = tbModuleDisplayName.Text.Trim();

                ResourceInfoProvider.SetResourceInfo(ri);

                // Update root UIElementInfo of the module
                UIElementInfo elemInfo = UIElementInfoProvider.GetRootUIElementInfo(ri.ResourceId);
                if (elemInfo == null)
                {
                    elemInfo = new UIElementInfo();
                }
                elemInfo.ElementResourceID = ri.ResourceId;
                elemInfo.ElementDisplayName = ri.ResourceDisplayName;
                elemInfo.ElementName = ri.ResourceName.ToLowerCSafe().Replace(".", "");
                elemInfo.ElementIsCustom = false;
                UIElementInfoProvider.SetUIElementInfo(elemInfo);

                // Show message
                ShowChangesSaved();
            }
            else
            {
                // Show error message
                ShowError(GetString("Administration-Module_New.UniqueCodeName"));
            }
        }
        else
        {
            // Show error message
            ShowError(result);
        }
    }


    /// <summary>
    /// Loads data of edited module from DB into TextBoxes.
    /// </summary>
    protected void LoadData()
    {
        ResourceInfo ri = ResourceInfoProvider.GetResourceInfo(moduleId);

        if (ri != null)
        {
            tbModuleCodeName.Text = ri.ResourceName;
            tbModuleDisplayName.Text = ri.ResourceDisplayName;
            txtModuleDescription.Text = ri.ResourceDescription;
        }
    }
}