using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.ResourceManager;
using CMS.SettingsProvider;
using CMS.Synchronization;
using CMS.UIControls;
using CMS.WorkflowEngine;

public partial class CMSModules_Automation_Pages_Comment : CMSModalPage
{
    #region "Properties"

    /// <summary>
    /// Action
    /// </summary>
    protected string ActionName
    {
        get
        {
            return QueryHelper.GetString("acname", null);
        }
    }


    /// <summary>
    /// Menu ID
    /// </summary>
    protected string MenuID
    {
        get
        {
            return QueryHelper.GetString("menuid", null);
        }
    }

    #endregion


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        autoMan.StateObjectID = QueryHelper.GetInteger("stateId", 0);
        ucComment.ActionName = ActionName;
        ucComment.MenuID = MenuID;
    }


    protected override void OnLoad(EventArgs e)
    {
        // Initialize header
        InitHeader();

        base.OnLoad(e);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (ucComment.Visible)
        {
            btnOk.OnClientClick = "ProcessAction(" + ScriptHelper.GetString(ActionName) + "); return CloseDialog();";
        }
        // Dialog is not available for unauthorized user
        else
        {
            plcContent.Visible = false;
            btnOk.Visible = false;
            ShowError(GetString("ma.notauthorizedaction"));
        }
    }


    private void InitHeader()
    {
        string imageName = null;
        string helpTopic = "process_dialog";

        switch (ActionName)
        {
            case ComponentEvents.AUTOMATION_MOVE_NEXT:
                btnOk.ResourceString = "general.nextstep";
                imageName = "next";
                break;

            case ComponentEvents.AUTOMATION_MOVE_SPEC:
                btnOk.ResourceString = "general.specificstep";
                imageName = "specific";
                break;

            case ComponentEvents.AUTOMATION_MOVE_PREVIOUS:
                btnOk.ResourceString = "general.previousstep";
                imageName = "previous";
                break;
        }

        // Set title
        string resName = HTMLHelper.HTMLEncode(ActionName);
        var step = autoMan.Step;

        CurrentMaster.Title.TitleText = GetString("editmenu.iconcomment" + resName);
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Content/EditMenu/" + imageName + ".png");
        CurrentMaster.Title.HelpTopicName = helpTopic;
        CurrentMaster.Title.HelpName = "helpTopic";
    }

    #endregion
}