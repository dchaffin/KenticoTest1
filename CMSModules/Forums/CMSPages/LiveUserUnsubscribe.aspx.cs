using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.Forums;
using CMS.CMSHelper;
using CMS.ExtendedControls;

public partial class CMSModules_Forums_CMSPages_LiveUserUnsubscribe : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the modal dialog
        SetCulture();
        RegisterEscScript();

        // Get post ID
        int postId = QueryHelper.GetInteger("postid", 0);

        if (CheckSubscription(postId))
        {
            // Set mode
            PostApproveFooter.Mode = PostApprove.Mode = "subscription";

            // Set the post ID
            PostApprove.PostID = PostApproveFooter.PostID = postId;
            PostApprove.UserID = PostApproveFooter.UserID = CMSContext.CurrentUser.UserID;

            // Page title
            CurrentMaster.Title.TitleText = GetString("forums_forumnewpost_header.preview");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Forums_ForumPost/object.png");
        }
        else
        {
            // Redirect to access denied
            RedirectToAccessDenied(CMSDESK_ACCESSDENIED, null, null, null, ResHelper.GetString("forumpost.notsubscribed"));
        }
    }


    /// <summary>
    /// Checks if current user has subscribed given post.
    /// </summary>
    /// <param name="postId">ID of forum post</param>
    /// <returns>true if user has subscriptions for given post, false if not</returns>
    private bool CheckSubscription(int postId)
    {
        DataSet ds = ForumSubscriptionInfoProvider.GetSubscriptions("(SubscriptionUserID = " + CMSContext.CurrentUser.UserID + ") AND (SubscriptionPostID = " + postId + ")", null, 0, "SubscriptionID");

        if (DataHelper.DataSourceIsEmpty(ds))
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// Raises the <see cref="E:PreRender"/> event.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Setup the modal dialog
        RegisterModalPageScripts();
    }

}
