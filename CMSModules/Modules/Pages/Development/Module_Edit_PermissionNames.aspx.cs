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
using CMS.ExtendedControls;

public partial class CMSModules_Modules_Pages_Development_Module_Edit_PermissionNames : SiteManagerPage
{
    #region "Variables"

    private int mModuleId;

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        mModuleId = QueryHelper.GetInteger("moduleId", 0);

        if (mModuleId > 0)
        {
            unigridPermissionList.WhereCondition = string.Format(@"ResourceID = {0}", mModuleId);
            unigridPermissionList.OnAction += new OnActionEventHandler(UniGridPermissionList_OnAction);
            unigridPermissionList.ZeroRowsText = GetString("general.nodatafound");

            CurrentMaster.HeaderActions.Actions = GetActions();
        }
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGridPermissionList_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect(string.Format(@"Module_Edit_PermissionName_Edit_Frameset.aspx?moduleId={0}&permissionId={1}", mModuleId, actionArgument));
        }
        else if (actionName == "delete")
        {
            PermissionNameInfoProvider.DeletePermissionInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
        else if (actionName == "moveup")
        {
            PermissionNameInfoProvider.MovePermissionUp(ValidationHelper.GetInteger(actionArgument, 0));
        }
        else if (actionName == "movedown")
        {
            PermissionNameInfoProvider.MovePermissionDown(ValidationHelper.GetInteger(actionArgument, 0));
        }
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Gets header actions.
    /// </summary>
    /// <returns>Two dimensional string array of actions.</returns>
    private string[,] GetActions()
    {
        string[,] actions = new string[1,6];

        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("Administration-Module_Edit_PermissionNames.NewPermission");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl(string.Format(@"Module_Edit_PermissionName_Edit.aspx?moduleID={0}", mModuleId));
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("Objects/CMS_Module/add.png");

        return actions;
    }

    #endregion
}