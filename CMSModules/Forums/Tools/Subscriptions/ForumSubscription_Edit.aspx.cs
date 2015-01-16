using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Forums;
using CMS.GlobalHelper;
using CMS.UIControls;


public partial class CMSModules_Forums_Tools_Subscriptions_ForumSubscription_Edit : CMSForumsPage
{
    protected int subscriptionId = 0;
    protected int forumId = 0;
    protected bool isNewItem = false;


    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Forums - Edit subscription";

        subscriptionId = QueryHelper.GetInteger("subscriptionid", 0);
        subscriptionEdit.SubscriptionID = subscriptionId;
        subscriptionEdit.OnSaved += new EventHandler(subscriptionEdit_OnSaved);
        subscriptionEdit.IsLiveSite = false;

        string currentForumSubscription = "";
        ForumSubscriptionInfo forumSubscriptionObj = ForumSubscriptionInfoProvider.GetForumSubscriptionInfo(subscriptionId);

        // get forumSubscription id from querystring
        if (forumSubscriptionObj != null)
        {
            currentForumSubscription = HTMLHelper.HTMLEncode(forumSubscriptionObj.SubscriptionEmail);

            // initializes page title control		
            string[,] tabs = new string[2,3];
            tabs[0, 0] = GetString("ForumSubscription_Edit.ItemListLink");
            tabs[0, 1] = "~/CMSModules/Forums/Tools/Subscriptions/ForumSubscription_List.aspx?forumid=" + forumSubscriptionObj.SubscriptionForumID;
            tabs[0, 2] = "";
            tabs[1, 0] = currentForumSubscription;
            tabs[1, 1] = "";
            tabs[1, 2] = "";

            CurrentMaster.Title.Breadcrumbs = tabs;
        }
        else
        {
            forumId = QueryHelper.GetInteger("forumid", 0);
            if (forumId == 0)
            {
                return;
            }
            subscriptionEdit.ForumID = forumId;
            isNewItem = true;

            // Initializes page title control		
            string[,] tabs = new string[2,3];
            tabs[0, 0] = GetString("ForumSubscription_Edit.ItemListLink");
            tabs[0, 1] = "~/CMSModules/Forums/Tools/Subscriptions/ForumSubscription_List.aspx?forumid=" + forumId.ToString();
            tabs[0, 2] = "";
            tabs[1, 0] = GetString("ForumSubscription_Edit.NewItemCaption");
            tabs[1, 1] = "";
            tabs[1, 2] = "";
            CurrentMaster.Title.Breadcrumbs = tabs;
        }

        ForumContext.CheckSite(0, forumId, 0);
    }


    protected void subscriptionEdit_OnSaved(object sender, EventArgs e)
    {
        if (isNewItem)
        {
            URLHelper.Redirect("ForumSubscription_Edit.aspx?subscriptionid=" + subscriptionEdit.SubscriptionID + "&saved=1&checked=" + subscriptionEdit.SendEmailConfirmation.ToString());
        }
    }
}