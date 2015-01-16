using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Forums;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Forums_Tools_Subscriptions_ForumSubscription_New : CMSForumsPage
{
    protected int forumId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        forumId = ValidationHelper.GetInteger(Request.QueryString["forumid"], 0);
        if (forumId == 0)
        {
            btnOk.Enabled = false;
            ShowError(GetString("ForumSubscription_Edit.NoForumIdGiven"));
            return;
        }

        rfvSubscriptionEmail.ErrorMessage = GetString("ForumSubscription_Edit.EnterSomeEmail");

        // control initializations				
        btnOk.Text = GetString("General.OK");

        InitializeMasterPage();
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        Title = "Forums - New forum subscription";

        // initializes page title control		
        string[,] tabs = new string[2,3];
        tabs[0, 0] = GetString("ForumSubscription_Edit.ItemListLink");
        tabs[0, 1] = "~/CMSModules/Forums/Tools/Subscriptions/ForumSubscription_List.aspx?forumid=" + forumId.ToString();
        tabs[0, 2] = "";
        tabs[1, 0] = GetString("ForumSubscription_Edit.NewItemCaption");
        tabs[1, 1] = "";
        tabs[1, 2] = "";

        CurrentMaster.Title.Breadcrumbs = tabs;

        CurrentMaster.Title.TitleText = GetString("ForumSubscription_New.HeaderCaption");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Forums_ForumSubscription/new.png");
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // check 'Read' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.forums", "Modify"))
        {
            RedirectToCMSDeskAccessDenied("cms.forums", "Modify");
        }

        string errorMessage = new Validator().NotEmpty(txtSubscriptionEmail.Text, GetString("ForumSubscription_Edit.EnterSomeEmail")).Result;

        if (errorMessage == "")
        {
            ForumSubscriptionInfo subscription = new ForumSubscriptionInfo();

            if (subscription == null)
            {
                ShowError(GetString("ForumSubscription_Edit.SubscriptionDoesNotExist"));
                return;
            }

            subscription.SubscriptionEmail = txtSubscriptionEmail.Text.Trim();
            if (ValidationHelper.IsEmail(subscription.SubscriptionEmail))
            {
                subscription.SubscriptionForumID = forumId;
                ForumSubscriptionInfoProvider.SetForumSubscriptionInfo(subscription);

                URLHelper.Redirect("ForumSubscription_Edit.aspx?subscriptionid=" + Convert.ToString(subscription.SubscriptionID) + "&saved=1");
            }
            else
            {
                ShowError(GetString("ForumSubscription_Edit.EmailIsNotValid"));
            }
        }
        else
        {
            ShowError(errorMessage);
        }
    }
}