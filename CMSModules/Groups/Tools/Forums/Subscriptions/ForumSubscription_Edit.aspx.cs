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

public partial class CMSModules_Groups_Tools_Forums_Subscriptions_ForumSubscription_Edit : CMSGroupForumPage
{
    protected int subscriptionId = 0;
    protected int forumId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "Forums - Edit subscription";

        string currentForumSubscription = "";
        ForumSubscriptionInfo forumSubscriptionObj = new ForumSubscriptionInfo();
        int subscriptionId = QueryHelper.GetInteger("subscriptionid", 0);
        subscriptionEdit.SubscriptionID = subscriptionId;
        subscriptionEdit.OnSaved += new EventHandler(subscriptionEdit_OnSaved);
        subscriptionEdit.IsLiveSite = false;

        // get forumSubscription id from querystring
        if (subscriptionId > 0)
        {
            forumSubscriptionObj = ForumSubscriptionInfoProvider.GetForumSubscriptionInfo(subscriptionId);
            currentForumSubscription = HTMLHelper.HTMLEncode(forumSubscriptionObj.SubscriptionEmail);

            // initializes page title control		
            string[,] tabs = new string[2,3];
            tabs[0, 0] = GetString("ForumSubscription_Edit.ItemListLink");
            tabs[0, 1] = "~/CMSModules/Groups/Tools/Forums/Subscriptions/ForumSubscription_List.aspx?forumid=" + forumSubscriptionObj.SubscriptionForumID;
            tabs[0, 2] = "";
            tabs[1, 0] = (string.IsNullOrEmpty(currentForumSubscription)) ? GetString("ForumSubscription_List.NewItemCaption") : currentForumSubscription;
            tabs[1, 1] = "";
            tabs[1, 2] = "";

            CurrentMaster.Title.Breadcrumbs = tabs;
        }
        else
        {
            forumId = ValidationHelper.GetInteger(Request.QueryString["forumid"], 0);
            if (forumId == 0)
            {
                return;
            }

            subscriptionEdit.ForumID = forumId;

            // initializes page title control		
            string[,] tabs = new string[2,3];
            tabs[0, 0] = GetString("ForumSubscription_Edit.ItemListLink");
            tabs[0, 1] = "~/CMSModules/Groups/Tools/Forums/Subscriptions/ForumSubscription_List.aspx?forumid=" + forumId.ToString();
            tabs[0, 2] = "";
            tabs[1, 0] = (string.IsNullOrEmpty(currentForumSubscription)) ? GetString("ForumSubscription_List.NewItemCaption") : currentForumSubscription;
            tabs[1, 1] = "";
            tabs[1, 2] = "";
            CurrentMaster.Title.Breadcrumbs = tabs;
            CurrentMaster.Title.TitleText = GetString("forumsubscription_edit.newitemcaption");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Forums_ForumSubscription/new.png");
        }

        subscriptionEdit.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(subscriptionEdit_OnCheckPermissions);
    }


    private void subscriptionEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int groupId = 0;
        ForumInfo fi = ForumInfoProvider.GetForumInfo(ValidationHelper.GetInteger(Request.QueryString["forumid"], 0));
        if (fi != null)
        {
            ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(fi.ForumGroupID);
            if (fgi != null)
            {
                groupId = fgi.GroupGroupID;
            }
        }

        // Check permissions
        CheckPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }


    private void subscriptionEdit_OnSaved(object sender, EventArgs e)
    {
        if (subscriptionEdit.SubscriptionID != 0)
        {
            URLHelper.Redirect("ForumSubscription_Edit.aspx?subscriptionid=" + Convert.ToString(subscriptionEdit.SubscriptionID) + "&saved=1");
        }
    }
}