using System;

using CMS.Blogs;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;


public partial class CMSModules_Blogs_Controls_BlogCommentDetail : BlogCommentDetail
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Controls initialization
        lnkApprove.Text = GetString("general.approve");
        lnkReject.Text = GetString("general.reject");
        lnkEdit.Text = GetString("general.edit");
        lnkDelete.Text = GetString("general.delete");

        lnkEdit.Visible = ShowEditButton;
        lnkDelete.Visible = ShowDeleteButton;
        lnkApprove.Visible = ShowApproveButton;
        lnkReject.Visible = ShowRejectButton;

        LoadData();

        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DeleteCommentConfirmation", ScriptHelper.GetScript("function ConfirmDelete(){ return confirm(" + ScriptHelper.GetString(GetString("BlogCommentDetail.DeleteConfirmation")) + ");}"));
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public void LoadData()
    {
        if (mCommentsDataRow != null)
        {
            // Load comment data
            BlogCommentInfo bci = new BlogCommentInfo(mCommentsDataRow);
            if (bci != null)
            {
                CommentID = bci.CommentID;

                // Set user picture
                if (BlogpPoperties.EnableUserPictures)
                {
                    userPict.UserID = bci.CommentUserID;
                    userPict.Width = BlogpPoperties.UserPictureMaxWidth;
                    userPict.Height = BlogpPoperties.UserPictureMaxHeight;
                    userPict.Visible = true;
                    userPict.RenderOuterDiv = true;
                    userPict.OuterDivCSSClass = "CommentUserPicture";

                    // Gravatar support
                    string avType = SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".CMSAvatarType");
                    if (avType == AvatarInfoProvider.USERCHOICE)
                    {
                        UserInfo ui = UserInfoProvider.GetUserInfo(bci.CommentUserID);
                        if (ui != null)
                        {
                            avType = ui.UserSettings.UserAvatarType;
                        }
                        else
                        {
                            avType = AvatarInfoProvider.GRAVATAR;
                        }
                    }

                    userPict.UserEmail = bci.CommentEmail;
                    userPict.UserAvatarType = avType;
                }
                else
                {
                    userPict.Visible = false;
                }

                if (!String.IsNullOrEmpty(bci.CommentUrl))
                {
                    lnkName.Text = HTMLHelper.HTMLEncode(bci.CommentUserName);
                    lnkName.NavigateUrl = bci.CommentUrl;
                    // Add no follow attribute if it is required
                    if (HTMLHelper.UseNoFollowForUsersLinks(CMSContext.CurrentSiteName))
                    {
                        lnkName.Attributes.Add("rel", "nofollow");
                    }
                    lblName.Visible = false;
                }
                else
                {
                    lblName.Text = HTMLHelper.HTMLEncode(bci.CommentUserName);
                    lnkName.Visible = false;
                }

                lblText.Text = GetHTMLEncodedCommentText(bci);
                lblDate.Text = CMSContext.ConvertDateTime(bci.CommentDate, this).ToString();

                string url = "~/CMSModules/Blogs/Controls/Comment_Edit.aspx";
                if (IsLiveSite)
                {
                    url = "~/CMSModules/Blogs/CMSPages/Comment_Edit.aspx";
                }

                lnkEdit.OnClientClick = "EditComment('" + ResolveUrl(url) + "?commentID=" + CommentID + "');return false;";

                // Initialize report abuse
                ucInlineAbuseReport.ReportTitle = ResHelper.GetString("BlogCommentDetail.AbuseReport", SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".CMSDefaultCulture")) + bci.CommentText;
                ucInlineAbuseReport.ReportObjectID = CommentID;
                ucInlineAbuseReport.CMSPanel.Roles = AbuseReportRoles;
                ucInlineAbuseReport.CMSPanel.SecurityAccess = AbuseReportSecurityAccess;
                ucInlineAbuseReport.CMSPanel.OwnerID = AbuseReportOwnerID;
            }
        }
    }


    /// <summary>
    /// Returns HTML encoded comment text.
    /// </summary>
    private static string GetHTMLEncodedCommentText(BlogCommentInfo bci)
    {
        if (bci != null)
        {
            string comment = HTMLHelper.HTMLEncodeLineBreaks(bci.CommentText);

            // Trackback comment
            if (bci.CommentIsTrackback)
            {
                string from = "";
                if (string.IsNullOrEmpty(bci.CommentUserName))
                {
                    // Use blog post URL
                    from = bci.CommentUrl;
                }
                else
                {
                    // Use user name
                    from = bci.CommentUserName;
                }
                return HTMLHelper.HTMLEncode(string.Format(ResHelper.GetString("blog.comments.pingbackfrom"), from)) + "<br />" + comment;
            }
            // Normal comment
            else
            {
                return comment;
            }
        }
        return "";
    }


    protected void lnkDelete_Click(object sender, EventArgs e)
    {
        FireOnCommentAction("delete", CommentID);
    }


    protected void lnkApprove_Click(object sender, EventArgs e)
    {
        FireOnCommentAction("approve", CommentID);
    }


    protected void lnkReject_Click(object sender, EventArgs e)
    {
        FireOnCommentAction("reject", CommentID);
    }
}