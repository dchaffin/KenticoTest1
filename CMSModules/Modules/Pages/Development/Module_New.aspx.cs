using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("Objects/CMS_Module/new.png", "Administration-Module_New.Title", "new_modulegenral_tab")]
[Breadcrumbs(2)]
[Breadcrumb(0, "Administration-Module_New.Modules", "~/CMSModules/Modules/Pages/Development/Module_List.aspx", "")]
[Breadcrumb(1, "Administration-Module_New.CurrentModule")]
public partial class CMSModules_Modules_Pages_Development_Module_New : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialization of controls
        lbModuleDisplayName.Text = GetString("Administration-Module_New.ModuleDisplayName");
        lbModuleCodeName.Text = GetString("Administration-Module_New.ModuleCodeName");

        rfvModuleDisplayName.ErrorMessage = GetString("Administration-Module_New.ErrorEmptyModuleDisplayName");
        rfvModuleCodeName.ErrorMessage = GetString("Administration-Module_New.ErrorEmptyModuleCodeName");
    }


    /// <summary>
    /// Handles btnOK's OnClick event - Save resource info.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // finds whether required fields are not empty
        string result = new Validator().NotEmpty(tbModuleDisplayName.Text.Trim(), GetString("Administration-Module_New.ErrorEmptyModuleDisplayName")).NotEmpty(tbModuleCodeName.Text, GetString("Administration-Module_New.ErrorEmptyModuleCodeName"))
            .IsCodeName(tbModuleCodeName.Text, GetString("general.invalidcodename"))
            .Result;

        if (result == "")
        {
            // finds if the resource code name is unique
            if (ResourceInfoProvider.GetResourceInfo(tbModuleCodeName.Text) == null)
            {
                //Save resource info
                ResourceInfo ri = new ResourceInfo();
                ri.ResourceName = tbModuleCodeName.Text;
                ri.ResourceDisplayName = tbModuleDisplayName.Text.Trim();

                ResourceInfoProvider.SetResourceInfo(ri);

                URLHelper.Redirect("Module_Edit_Frameset.aspx?moduleID=" + ri.ResourceId + "&saved=1");
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
}