using System;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Objects_Dialogs_Comment : CMSModalPage
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
            ShowError(GetString("doc.notauthorizedaction"));
        }
    }


    private void InitHeader()
    {
        string imageName = null;
        string helpTopic = "objects_dialog";

        switch (ActionName)
        {
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

        CurrentMaster.Title.TitleText = GetString("objecteditmenu.iconcomment" + resName);
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Content/EditMenu/" + imageName + ".png");
        CurrentMaster.Title.HelpTopicName = helpTopic;
        CurrentMaster.Title.HelpName = "helpTopic";
    }

    #endregion
}