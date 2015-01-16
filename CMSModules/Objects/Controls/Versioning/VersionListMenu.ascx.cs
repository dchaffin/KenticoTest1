using System;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Objects_Controls_Versioning_VersionListMenu : CMSContextMenuControl
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string menuId = ContextMenu.MenuID;
        string parentElemId = ContextMenu.ParentElementClientID;

        var info = CMSContext.EditedObject as BaseInfo;

        if ((info != null) && UserInfoProvider.IsAuthorizedPerObject(info.ObjectType, PermissionsEnum.Modify, CMSContext.CurrentSiteName, CMSContext.CurrentUser))
        {
            imgRestoreChilds.ImageUrl = UIHelper.GetImageUrl(Page, "CMSModules/CMS_RecycleBin/restorechilds.png");
            pnlRestoreChilds.Attributes.Add("onclick", "if(confirm('" + ResHelper.GetString("objectversioning.versionlist.confirmfullrollback") + "')) { ContextVersionAction_" + parentElemId + "('fullrollback', GetContextMenuParameter('" + menuId + "'));} return false;");
        }
        else
        {
            imgRestoreChilds.ImageUrl = UIHelper.GetImageUrl(Page, "CMSModules/CMS_RecycleBin/restorechildsdisabled.png");
        }
    }
}
