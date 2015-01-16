using System;
using System.Collections.Generic;

using CMS.GlobalHelper;
using CMS.UIControls;

using Group = CMS.UIControls.UniMenuConfig.Group;

public partial class CMSModules_ImportExport_SiteManager_NewSite_DefineSiteStructure_menu : SiteManagerPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterScriptFile(Page, @"~/CMSModules/ImportExport/SiteManager/NewSite/DefineSiteStructure/menu.js");
        ScriptHelper.RegisterDialogScript(this);

        // Initialize content management menu
        Group group = new Group();
        group.Caption = GetString("ContentMenu.ContentManagement");
        group.ControlPath = "~/CMSAdminControls/UI/UniMenu/NewSite/NewSiteMenu.ascx";
        menuLeft.Groups.Add(group);
    }

    #endregion
}