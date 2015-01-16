using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.UIControls;

[Title("Objects/Newsletter_Subscriber/object.png", "newsletters.subscribers", "new_subscriber", HelpName="helpTopic")]
public partial class CMSModules_Newsletters_Tools_Subscribers_Subscriber_New : CMSNewsletterSubscribersPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Controls' initialization
        lblSubscriberLastName.Text = GetString("Subscriber_Edit.SubscriberLastNameLabel");
        lblSubscriberFirstName.Text = GetString("Subscriber_Edit.SubscriberFirstNameLabel");
        rfvSubscriberEmail.ErrorMessage = GetString("Subscriber_Edit.ErrorEmptyEmail");

        // Initialize page title control
        string[,] tabs = new string[2,3];
        tabs[0, 0] = GetString("Subscriber_Edit.ItemListLink");
        tabs[0, 1] = "~/CMSModules/Newsletters/Tools/Subscribers/Subscriber_List.aspx";
        tabs[0, 2] = "newslettersContent";
        tabs[1, 0] = GetString("Subscriber_Edit.NewItemCaption");
        tabs[1, 1] = "";
        tabs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = tabs;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check "Manage subscribers" permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "managesubscribers"))
        {
            RedirectToCMSDeskAccessDenied("cms.newsletter", "managesubscribers");
        }

        string email = txtSubscriberEmail.Text.Trim();

        // Check entered values
        string errorMessage = new Validator().NotEmpty(email, GetString("Subscriber_Edit.ErrorEmptyEmail"))
            .IsEmail(email, GetString("NewsletterSubscription.ErrorInvalidEmail"))
            .Result;

        if (string.IsNullOrEmpty(errorMessage))
        {
            if (!SubscriberInfoProvider.EmailExists(email))
            {
                // Create new subscriber info and set values
                SubscriberInfo subscriberObj = new SubscriberInfo();

                subscriberObj.SubscriberID = 0;
                subscriberObj.SubscriberLastName = txtSubscriberLastName.Text.Trim();
                subscriberObj.SubscriberEmail = email;
                subscriberObj.SubscriberFirstName = txtSubscriberFirstName.Text.Trim();
                subscriberObj.SubscriberFullName = (subscriberObj.SubscriberFirstName + " " + subscriberObj.SubscriberLastName).Trim();
                subscriberObj.SubscriberSiteID = CMSContext.CurrentSiteID;
                subscriberObj.SubscriberGUID = Guid.NewGuid();

                if (SubscriberInfoProvider.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.Subscribers, VersionActionEnum.Insert))
                {
                    // Save subscriber to DB
                    SubscriberInfoProvider.SetSubscriberInfo(subscriberObj);
                    // Redirect to edit page
                    URLHelper.Redirect("Subscriber_Frameset.aspx?subscriberid=" + Convert.ToString(subscriberObj.SubscriberID) + "&saved=1");
                }
                else
                {
                    ShowError(GetString("LicenseVersionCheck.Subscribers"));
                }
            }
            else
            {
                ShowError(GetString("Subscriber_Edit.EmailAlreadyExists"));
            }
        }
        else
        {
            ShowError(errorMessage);
        }
    }
}