using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.ProjectManagement;
using CMS.UIControls;

public partial class CMSModules_ProjectManagement_Pages_Tools_Configuration_Frameset : CMSProjectManagementConfigurationPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterTitleScript(this, GetString("projectmanagement.configuration"));

        CheckPermissions("CMS.ProjectManagement", ProjectManagementPermissionType.MANAGE_CONFIGURATION);

        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        frmHeader.Attributes["src"] = URLHelper.ResolveUrl("Header.aspx?" + URLHelper.GetQuery(URLHelper.CurrentURL));
        frmContent.Attributes["src"] = URLHelper.ResolveUrl("ProjectStatus/List.aspx?" + URLHelper.GetQuery(URLHelper.CurrentURL));
    }
}