using System;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.Polls;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Polls_Tools_Polls_Answer_List : CMSPollsPage
{
    #region "Private variables"

    protected int pollId = 0;
    protected PollInfo pi = null;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get poll id from querystring		
        pollId = QueryHelper.GetInteger("pollId", 0);
        pi = PollInfoProvider.GetPollInfo(pollId);
        EditedObject = pi;

        // Check global and site read permmision
        CheckPollsReadPermission(pi.PollSiteID);

        if (CheckPollsModifyPermission(pi.PollSiteID, false))
        {
            string[,] actions = new string[2, 7];

            // New item link
            actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
            actions[0, 1] = GetString("Polls_Answer_List.NewItemCaption");
            actions[0, 2] = null;
            actions[0, 3] = ResolveUrl("Polls_Answer_Edit.aspx?pollId=" + pollId.ToString());
            actions[0, 4] = null;
            actions[0, 5] = GetImageUrl("Objects/Polls_PollAnswer/add.png");

            // Reset answer button
            actions[1, 0] = HeaderActions.TYPE_LINKBUTTON;
            actions[1, 1] = GetString("Polls_Answer_List.ResetButton");
            actions[1, 2] = "return confirm(" + ScriptHelper.GetString(GetString("Polls_Answer_List.ResetConfirmation")) + ");";
            actions[1, 3] = null;
            actions[1, 4] = null;
            actions[1, 5] = GetImageUrl("CMSModules/CMS_Polls/resetanswers.png");
            actions[1, 6] = "btnReset_Click";

            CurrentMaster.HeaderActions.Actions = actions;
            CurrentMaster.HeaderActions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);

            AnswerList.AllowEdit = true;
        }

        AnswerList.OnEdit += new EventHandler(AnswerList_OnEdit);
        AnswerList.PollId = pollId;
        AnswerList.IsLiveSite = false;
        AnswerList.AllowEdit = CheckPollsModifyPermission(pi.PollSiteID, false);
    }


    /// <summary>
    /// AnswerList edit action handler.
    /// </summary>
    private void AnswerList_OnEdit(object sender, EventArgs e)
    {
        URLHelper.Redirect("Polls_Answer_Edit.aspx?answerId=" + AnswerList.SelectedItemID);
    }


    /// <summary>
    /// Header action handler.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event args</param>
    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "btnreset_click": // Reset all answer counts

                // Check 'Modify' permission
                CheckPollsModifyPermission(pi.PollSiteID);

                if (pollId > 0)
                {
                    PollAnswerInfoProvider.ResetAnswers(pollId);
                    AnswerList.ReloadData();
                }
                break;
        }
    }
}