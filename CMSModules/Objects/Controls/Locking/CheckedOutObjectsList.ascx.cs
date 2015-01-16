using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSModules_Objects_Controls_Locking_CheckedOutObjectsList : CMSUserControl
{
    ContextResolver contextResolver = ContextResolver.GetInstance();
    ObjectSettingsInfo tmpObjectSettings = null;
    BaseInfo tmpInfo = null;


    protected override void OnLoad(System.EventArgs e)
    {
        base.OnLoad(e);

        if (!CMSObjectHelper.UseCheckinCheckout)
        {
            grid.StopProcessing = true;
            return;
        }

        var lockableTypes = ObjectTypeLists.AllObjectTypes.Where(t => TranslationHelper.GetReadOnlyObject(t).TypeInfo.SupportsLocking);

        var lockableTypesFormatted = lockableTypes.Select(t => string.Format("N'{0}'", t)).Join(",");
        grid.WhereCondition = string.Format("ObjectCheckedOutByUserID = {0} AND ObjectSettingsObjectType IN ({1})", CMSContext.CurrentUser.UserID, lockableTypesFormatted);

        grid.ZeroRowsText = GetString("mydesk.ui.nocheckedobjects");
        grid.OnExternalDataBound += new OnExternalDataBoundEventHandler(grid_OnExternalDataBound);

        btnCheckIn.Click += new EventHandler(btnCheckIn_Click); ;
        btnUndoCheckOut.Click += new EventHandler(btnUndoCheckOut_Click);
    }


    protected override void OnPreRender(System.EventArgs e)
    {
        base.OnPreRender(e);

        if (CMSObjectHelper.UseCheckinCheckout)
        {
            var setHdnValuesScript = string.Format("function {0}_setHdnValues(objectType, objectId) {{ document.getElementById('{1}').value = objectType; document.getElementById('{2}').value = objectId; }}", ClientID, hdnObjectType.ClientID, hdnObjectId.ClientID);
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), ClientID + "_setHdnValues", ScriptHelper.GetScript(setHdnValuesScript));

            var refreshScript = string.Format("function refresh() {{ {0} }}", grid.GetReloadScript());
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "refresh", ScriptHelper.GetScript(refreshScript));

            ScriptHelper.RegisterDialogScript(Page);
        }
    }


    protected object grid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView drv = null;
        if (parameter is DataRowView)
        {
            drv = (DataRowView)parameter;
        }
        else if (parameter is GridViewRow)
        {
            drv = (DataRowView)((GridViewRow)parameter).DataItem;
        }

        var objectSettingsId = ValidationHelper.GetInteger(drv["ObjectSettingsID"], 0);

        if ((tmpObjectSettings == null) || (tmpObjectSettings.ObjectSettingsID != objectSettingsId))
        {
            tmpObjectSettings = ObjectSettingsInfoProvider.GetObjectSettingsInfo(objectSettingsId);
            tmpInfo = CMSObjectHelper.GetObjectById(tmpObjectSettings.ObjectSettingsObjectType, tmpObjectSettings.ObjectSettingsObjectID);
        }

        contextResolver.SetNamedSourceData("EditedObject", tmpInfo);

        switch (sourceName.ToLowerCSafe())
        {
            case "edit":
                var editButton = (CMSImageButton)sender;

                var url = tmpInfo.Generalized.GetEditingPageURL();

                if (!string.IsNullOrEmpty(url))
                {
                    url = contextResolver.ResolveMacros(url);
                    url = CMSContext.ResolveDialogUrl(url);

                    var queryString = URLHelper.GetQuery(url);
                    url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(queryString));

                    editButton.OnClientClick = string.Format("modalDialog('{0}', 'objectEdit', '85%', '85%');", url);
                }
                else
                {
                    editButton.Enabled = false;
                }
                break;

            case "checkin":
                var checkinButton = (CMSImageButton)sender;

                checkinButton.ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/16/checkin.png");

                if (tmpInfo.TypeInfo.SupportsLocking)
                {
                    checkinButton.Attributes["onclick"] = GetConfirmScript(GetString("ObjectEditMenu.CheckInConfirmation"), tmpObjectSettings.ObjectSettingsObjectType, tmpObjectSettings.ObjectSettingsObjectID, btnCheckIn);
                }
                else
                {
                    checkinButton.Enabled = false;
                }
                break;

            case "undocheckout":
                var undoCheckoutButton = (CMSImageButton)sender;

                undoCheckoutButton.ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/16/undocheckout.png");

                if (tmpInfo.TypeInfo.SupportsLocking)
                {
                    undoCheckoutButton.Attributes["onclick"] = GetConfirmScript(CMSObjectManager.GetUndoCheckOutConfirmation(tmpInfo, null), tmpObjectSettings.ObjectSettingsObjectType, tmpObjectSettings.ObjectSettingsObjectID, btnUndoCheckOut);
                }
                else
                {
                    undoCheckoutButton.Enabled = false;
                }
                break;
        }

        return parameter;
    }


    private string GetConfirmScript(string message, string objectType, int objectId, Control targetControl)
    {
        var argument = string.Format("{0};{1}", objectType, objectId);

        var script = new StringBuilder();
        script.AppendFormat("if (!confirm({0})) return false; ", ScriptHelper.GetString(message));
        script.AppendFormat("{0}_setHdnValues('{1}', {2}); ", ClientID, objectType, objectId);
        script.Append(ControlsHelper.GetPostBackEventReference(targetControl, argument), "; return false");

        return script.ToString();
    }


    protected void btnCheckIn_Click(object sender, EventArgs e)
    {
        var info = GetInfoFromHiddenValues();

        try
        {
            new ObjectVersionManager().CheckIn(info);
        }
        catch (ObjectVersioningException)
        {
            ShowInformation(GetString("objecteditmenu.checkinerror"));
            return;
        }

        grid.ReloadData();
        ShowChangesSaved();
    }


    protected void btnUndoCheckOut_Click(object sender, EventArgs e)
    {
        var info = GetInfoFromHiddenValues();

        try
        {
            new ObjectVersionManager().UndoCheckOut(info);
        }
        catch (ObjectVersioningException)
        {
            ShowInformation(GetString("objecteditmenu.undocheckouterror"));
            return;
        }

        grid.ReloadData();
    }


    private BaseInfo GetInfoFromHiddenValues()
    {
        var objectType = ValidationHelper.GetString(hdnObjectType.Value, null);
        var objectId = ValidationHelper.GetInteger(hdnObjectId.Value, 0);

        return CMSObjectHelper.GetObjectById(objectType, objectId);
    }
}
