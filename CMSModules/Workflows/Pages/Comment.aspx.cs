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

public partial class CMSModules_Workflows_Pages_Comment : CMSModalPage
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

    protected override void OnPreInit(EventArgs e)
    {
        EnsureDocumentManager = true;
        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        // Initialize header
        InitHeader();

        // Set current node
        ucComment.Node = Node;

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
            ShowError(GetString("doc.notauthorizedaction"));
        }
    }


    private void InitHeader()
    {
        string imageName = null;
        string helpTopic = "workflow_dialog";

        switch (ActionName)
        {
            case DocumentComponentEvents.PUBLISH:
                btnOk.ResourceString = "general.publish";
                imageName = "approve";
                break;

            case DocumentComponentEvents.APPROVE:
                btnOk.ResourceString = "general.approve";
                imageName = "approve";
                break;

            case DocumentComponentEvents.REJECT:
                btnOk.ResourceString = "general.reject";
                imageName = "reject";
                break;

            case DocumentComponentEvents.ARCHIVE:
                btnOk.ResourceString = "general.archive";
                imageName = "archive";
                break;

            case DocumentComponentEvents.CHECKIN:
                btnOk.ResourceString = "EditMenu.IconCheckIn";
                imageName = "checkin";
                break;
        }

        // Set title
        string resName = HTMLHelper.HTMLEncode(ActionName);
        var step = DocumentManager.Step;
        if ((ActionName == DocumentComponentEvents.APPROVE) && (step != null) && (step.StepIsEdit))
        {
            resName += "Submit";
        }

        CurrentMaster.Title.TitleText = GetString("editmenu.iconcomment" + resName);
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Content/EditMenu/" + imageName + ".png");
        CurrentMaster.Title.HelpTopicName = helpTopic;
        CurrentMaster.Title.HelpName = "helpTopic";
    }

    #endregion
}