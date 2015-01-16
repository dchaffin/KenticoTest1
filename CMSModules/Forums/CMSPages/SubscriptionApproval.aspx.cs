using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.Forums;

public partial class CMSModules_Forums_CMSPages_SubscriptionApproval : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get object display name
        ForumInfo fi = ForumInfoProvider.GetForumInfo((subscriptionApproval.SubscriptionObject != null) ? subscriptionApproval.SubscriptionObject.SubscriptionForumID : 0);
        ForumPostInfo fpi = ForumPostInfoProvider.GetForumPostInfo((subscriptionApproval.SubscriptionObject != null) ? subscriptionApproval.SubscriptionObject.SubscriptionPostID : 0);
        string name = (fpi != null) ? TextHelper.LimitLength(fpi.PostSubject, 50) : (fi != null) ? fi.ForumDisplayName : null;

        // Set text to display according to subscription object
        string objectText = "forum.subscriptionconfirmation";
        if (fpi != null)
        {
            objectText = "forumpost.subscriptionconfirmation";
        }

        PageTitle title = CurrentMaster.Title;
        title.TitleImage = GetImageUrl("Design/Controls/Subscriptions/subscription_approval.png");
        title.TitleText = HTMLHelper.HTMLEncode(string.Format(GetString(objectText), (name != null) ? ScriptHelper.GetString(name) : null));
    }
}