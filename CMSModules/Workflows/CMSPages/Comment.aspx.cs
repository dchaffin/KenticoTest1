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

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Workflows_CMSPages_Comment : CMSLiveModalPage
{
    #region "Properties"

    /// <summary>
    /// Action
    /// </summary>
    public string Action
    {
        get
        {
            return QueryHelper.GetString("acname", null);
        }
    }


    /// <summary>
    /// Menu ID
    /// </summary>
    public string MenuID
    {
        get
        {
            return QueryHelper.GetString("menuid", null);
        }
    }


    /// <summary>
    /// Tree node
    /// </summary>
    protected TreeNode Node
    {
        get
        {
            return DocumentManager.Node;
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
            btnOk.OnClientClick = "ProcessAction(" + ScriptHelper.GetString(Action) + "); return CloseDialog();";
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

        switch (Action)
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
        string resName = HTMLHelper.HTMLEncode(Action);
        var step = DocumentManager.Step;
        if ((Action == DocumentComponentEvents.APPROVE) && (step != null) && (step.StepIsEdit))
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