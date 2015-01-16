using System.Collections;
using System.Data;
using System;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;


/// <summary>
/// Class containing resolvers for specific types of e-mail templates.
/// </summary>
public static class EmailTemplateMacros
{
    #region "Enumeration"

    /// <summary>
    /// Email type enumeration. Determines the type of a resolver used in the e-mail template.
    /// </summary>
    public enum EmailType
    {
        General,
        Automation,
        Blog,
        BlogSubscription,
        Boards,
        BoardsSubscription,
        BookingEvent,
        Ecommerce,
        EcommerceEproductExpiration,
        Forum,
        ForumSubscribtion,
        Friends,
        GroupMember,
        GroupInvitation,
        GroupMemberInvitation,
        Password,
        ProjectManagement,
        Messaging,
        Registration,
        RegistrationApproval,
        MembershipRegistration,
        MembershipExpiration,
        MembershipChangePassword,
        MembershipUnlockAccount,
        ForgottenPassword,
        Newsletter,
        WorkflowEmail,
        Scoring,
        TranslationServices,
        Reporting
    }

    #endregion


    #region "Variables"

    private static ContextResolver mEmailTemplateResolver = null;
    private static ContextResolver mShoppingCartResolver = null;
    private static ContextResolver mNewsletterResolver = null;
    private static ContextResolver mNewsletterOptInResolver = null;
    private static ContextResolver mEcommerceResolver = null;
    private static ContextResolver mEcommerceEproductExpirationResolver = null;
    private static ContextResolver mWorkflowBaseResolver = null;
    private static ContextResolver mWorkflowSimpleResolver = null;
    private static ContextResolver mWorkflowEmailResolver = null;
    private static ContextResolver mWorkflowBaseDocumentResolver = null;
    private static ContextResolver mWorkflowSimpleDocumentResolver = null;
    private static ContextResolver mAutomationSimpleResolver = null;
    private static ContextResolver mAutomationEmailResolver = null;
    private static ContextResolver mBlogResolver = null;
    private static ContextResolver mBlogSubscribeResolver = null;
    private static ContextResolver mBookingResolver = null;
    private static ContextResolver mForumsResolver = null;
    private static ContextResolver mForumsSubscribeResolver = null;
    private static ContextResolver mFriendsResolver = null;
    private static ContextResolver mMessagingResolver = null;
    private static ContextResolver mRegistrationResolver = null;
    private static ContextResolver mRegistrationApprovedResolver = null;
    private static ContextResolver mMembershipRegistrationResolver = null;
    private static ContextResolver mMembershipExpirationResolver = null;
    private static ContextResolver mPasswordResolver = null;
    private static ContextResolver mForgottenPasswordResolver = null;
    private static ContextResolver mGroupMemberResolver = null;
    private static ContextResolver mBoardsResolver = null;
    private static ContextResolver mBoardsSubscribeResolver = null;
    private static ContextResolver mGroupsInvitationResolver = null;
    private static ContextResolver mGroupsAcceptedInvitationResolver = null;
    private static ContextResolver mProjectManagementResolver = null;
    private static ContextResolver mScoringResolver = null;
    private static ContextResolver mTranslationsResolver = null;
    private static ContextResolver mContactResolver = null;
    private static ContextResolver mContactActivityResolver = null;
    private static ContextResolver mContactScoreResolver = null;
    private static ContextResolver mVariantResolver = null;
    private static ContextResolver mAnalyticsResolver = null;
    private static ContextResolver mMembershipChangePasswordResolver = null;
    private static ContextResolver mMembershipUnlockAccountResolver = null;
    private static ContextResolver mReportingResolver = null;
    private static ContextResolver mDeviceProfilesResolver = null;

    #endregion


    #region "Methods"

    /// <summary>
    /// Sets additional context values to resolver.
    /// </summary>
    /// <param name="resolver">Context resolver</param>
    private static void SetContext(ContextResolver resolver)
    {
        resolver.Context.DisableContextObjectMacros = true;
    }


    /// <summary>
    /// Returns the EmailType enum constant from string representation.
    /// </summary>
    /// <param name="emailType">String representation of email type</param>
    public static EmailType GetEmailTypeEnum(string emailType)
    {
        // Get a enum representation and return it as result
        switch (emailType.ToLowerCSafe())
        {
            case "general":
                return EmailType.General;

            case "automation":
                return EmailType.Automation;

            case "blog":
                return EmailType.Blog;

            case "blogsubscription":
                return EmailType.BlogSubscription;

            case "boards":
                return EmailType.Boards;

            case "boardssubscription":
                return EmailType.BoardsSubscription;

            case "bookingevent":
                return EmailType.BookingEvent;

            case "ecommerce":
                return EmailType.Ecommerce;

            case "ecommerceeproductexpiration":
                return EmailType.EcommerceEproductExpiration;

            case "forum":
                return EmailType.Forum;

            case "forumsubscribtion":
                return EmailType.ForumSubscribtion;

            case "friends":
                return EmailType.Friends;

            case "groupmember":
                return EmailType.GroupMember;

            case "groupinvitation":
                return EmailType.GroupInvitation;

            case "groupmemberinvitation":
                return EmailType.GroupMemberInvitation;

            case "password":
                return EmailType.Password;

            case "projectmanagement":
                return EmailType.ProjectManagement;

            case "messaging":
                return EmailType.Messaging;

            case "registration":
                return EmailType.Registration;

            case "registrationapproval":
                return EmailType.RegistrationApproval;

            case "membershipregistration":
                return EmailType.MembershipRegistration;

            case "membershipexpiration":
                return EmailType.MembershipExpiration;

            case "forgottenpassword":
                return EmailType.ForgottenPassword;

            case "membershipchangepassword":
                return EmailType.MembershipChangePassword;

            case "membershipunlockaccount":
                return EmailType.MembershipUnlockAccount;

            case "newsletter":
                return EmailType.Newsletter;

            case "workflow":
                return EmailType.WorkflowEmail;

            case "scoring":
                return EmailType.Scoring;

            case "translationservices":
                return EmailType.TranslationServices;

            case "reporting":
                return EmailType.Reporting;

            default:
                return EmailType.General;
        }
    }


    /// <summary>
    /// Returns string representation of the EmailType enum constant.
    /// </summary>
    /// <param name="emailType">EmailType enum constant</param>
    public static string GetEmailTypeString(EmailType emailType)
    {
        // Get a enum representation and return it as result
        switch (emailType)
        {
            case EmailType.General:
                return "general";

            case EmailType.Automation:
                return "automation";

            case EmailType.Blog:
                return "blog";

            case EmailType.BlogSubscription:
                return "blogsubscription";

            case EmailType.Boards:
                return "boards";

            case EmailType.BoardsSubscription:
                return "boardssubscription";

            case EmailType.BookingEvent:
                return "bookingevent";

            case EmailType.Ecommerce:
                return "ecommerce";

            case EmailType.EcommerceEproductExpiration:
                return "ecommerceeproductexpiration";

            case EmailType.Forum:
                return "forum";

            case EmailType.ForumSubscribtion:
                return "forumsubscribtion";

            case EmailType.Friends:
                return "friends";

            case EmailType.GroupMember:
                return "groupmember";

            case EmailType.GroupInvitation:
                return "groupinvitation";

            case EmailType.GroupMemberInvitation:
                return "groupmemberinvitation";

            case EmailType.Password:
                return "password";

            case EmailType.ProjectManagement:
                return "projectmanagement";

            case EmailType.Messaging:
                return "messaging";

            case EmailType.Registration:
                return "registration";

            case EmailType.RegistrationApproval:
                return "registrationapproval";

            case EmailType.MembershipRegistration:
                return "membershipregistration";

            case EmailType.MembershipExpiration:
                return "membershipexpiration";

            case EmailType.ForgottenPassword:
                return "forgottenpassword";

            case EmailType.MembershipUnlockAccount:
                return "membershipunlockaccount";

            case EmailType.Newsletter:
                return "newsletter";

            case EmailType.WorkflowEmail:
                return "workflow";

            case EmailType.MembershipChangePassword:
                return "membershipchangepassword";

            case EmailType.Scoring:
                return "scoring";

            case EmailType.TranslationServices:
                return "translationservices";

            case EmailType.Reporting:
                return "reporting";

            default:
                return "general";
        }
    }


    /// <summary>
    /// Returns appropriate resolver for given e-mail type.
    /// </summary>
    /// <param name="type">E-mail type</param>
    public static ContextResolver GetEmailTemplateResolver(EmailType type)
    {
        switch (type)
        {
            case EmailType.Automation:
                return AutomationEmailResolver;

            case EmailType.Blog:
                return BlogsResolver;

            case EmailType.BlogSubscription:
                return BlogSubscriptionResolver;

            case EmailType.Boards:
                return BoardsResolver;

            case EmailType.BoardsSubscription:
                return BoardsSubscriptionResolver;

            case EmailType.BookingEvent:
                return BookingResolver;

            case EmailType.Ecommerce:
                return EcommerceResolver;

            case EmailType.EcommerceEproductExpiration:
                return EcommerceEproductExpirationResolver;

            case EmailType.Forum:
                return ForumsResolver;

            case EmailType.ForumSubscribtion:
                return ForumsSubscribtionResolver;

            case EmailType.Friends:
                return FriendsResolver;

            case EmailType.GroupMember:
                return GroupMemberResolver;

            case EmailType.GroupInvitation:
                return GroupsInvitationResolver;

            case EmailType.GroupMemberInvitation:
                return GroupsAcceptedInvitationResolver;

            case EmailType.Password:
                return PasswordResolver;

            case EmailType.MembershipUnlockAccount:
                return MembershipUnlockAccountResolver;

            case EmailType.ProjectManagement:
                return ProjectManagement;

            case EmailType.Messaging:
                return MessagingResolver;

            case EmailType.Registration:
                return RegistrationResolver;

            case EmailType.RegistrationApproval:
                return RegistrationApprovedResolver;

            case EmailType.MembershipRegistration:
                return MembershipRegistrationResolver;

            case EmailType.MembershipExpiration:
                return MembershipExpirationResolver;

            case EmailType.ForgottenPassword:
                return ForgottenPasswordResolver;

            case EmailType.Newsletter:
                return NewsletterResolver;

            case EmailType.WorkflowEmail:
                return WorkflowEmailResolver;

            case EmailType.Scoring:
                return ScoringResolver;

            case EmailType.MembershipChangePassword:
                return MembershipChangePasswordResolver;

            case EmailType.TranslationServices:
                return TranslationsResolver;

            case EmailType.Reporting:
                return ReportingResolver;

            default:
                return EmailTemplateResolver;
        }
    }


    /// <summary>
    /// Reigsters flat values in the resolver (adds them to values which are already registered).
    /// </summary>
    /// <param name="resolver">Resolver object</param>
    /// <param name="names">Names of the macros - values will be accessible by this names in the resolver</param>
    private static void RegisterValues(this MacroResolver resolver, string[] names)
    {
        if ((resolver != null) && (names != null))
        {
            object[,] sourceParameters = null;
            int i = 0;

            if (resolver.SourceParameters == null)
            {
                // Create new parameters
                sourceParameters = new string[names.Length, 2];
            }
            else
            {
                // Add parameters to current resolver
                int currentSize = resolver.SourceParameters.GetUpperBound(0);
                sourceParameters = new string[names.Length + currentSize + 1, 2];

                // Copy current values
                for (int j = 0; j <= currentSize; j++)
                {
                    sourceParameters[j, 0] = resolver.SourceParameters[j, 0];
                    i++;
                }
            }

            foreach (string name in names)
            {
                sourceParameters[i++, 0] = name;
            }

            resolver.SourceParameters = sourceParameters;
        }
    }


    /// <summary>
    /// Registers NamedDataSource to a given resolver under the specified name.
    /// </summary>
    /// <param name="resolver">Resolver object</param>
    /// <param name="name">Name of the macro - object will be accessible by this name in the resolver</param>
    /// <param name="classObject">Class object to register</param>
    private static void RegisterObject(this MacroResolver resolver, string name, object classObject)
    {
        resolver.SetNamedSourceData(name, classObject);
    }


    /// <summary>
    /// Registers NamedDataSource to a given resolver under the specified name.
    /// </summary>
    /// <param name="resolver">Resolver object</param>
    /// <param name="name">Name of the macro - object will be accessible by this name in the resolver</param>
    /// <param name="className">Class name of the object (for the AutoCompletion to work properly)</param>
    private static void RegisterObject(this MacroResolver resolver, string name, string className)
    {
        BaseInfo obj = CMSObjectHelper.GetReadOnlyObject(className);
        if (obj != null)
        {
            resolver.SetNamedSourceData(name, obj);
        }
        else
        {
            resolver.SetNamedSourceData(name, DataClassFactory.NewDataClass(className));
        }
    }

    #endregion


    #region "Resolvers"

    /// <summary>
    /// Newsletter resolver.
    /// </summary>
    public static ContextResolver NewsletterResolver
    {
        get
        {
            if (mNewsletterResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("NewsletterIssue", PredefinedObjectType.NEWSLETTERISSUE);
                resolver.RegisterObject("Newsletter", PredefinedObjectType.NEWSLETTER);
                resolver.RegisterObject("Subscriber", PredefinedObjectType.NEWSLETTERSUBSCRIBER);

                resolver.SetNamedSourceData("Email", string.Empty);
                resolver.SetNamedSourceData("FirstName", string.Empty);
                resolver.SetNamedSourceData("LastName", string.Empty);
                resolver.SetNamedSourceData("UnsubscribeLink", string.Empty);

                // Disable context macros
                SetContext(resolver);

                mNewsletterResolver = resolver;
            }

            return mNewsletterResolver;
        }
    }


    /// <summary>
    /// Newsletter opt-in resolver.
    /// </summary>
    public static ContextResolver NewsletterOptInResolver
    {
        get
        {
            if (mNewsletterOptInResolver == null)
            {
                // Use data from Newsletter resolver
                ContextResolver resolver = NewsletterResolver.CreateContextChild();

                // Set additional macros
                resolver.SetNamedSourceData("ActivationLink", string.Empty);

                // Disable context macros
                SetContext(resolver);

                mNewsletterOptInResolver = resolver;
            }

            return mNewsletterOptInResolver;
        }
    }


    /// <summary>
    /// Default e-mail template resolver.
    /// </summary>
    public static ContextResolver EmailTemplateResolver
    {
        get
        {
            if (mEmailTemplateResolver == null)
            {
                // Use data from ShoppingCart resolver
                ContextResolver resolver = ShoppingCartResolver.CreateContextChild();

                // Add newsletter data
                resolver.RegisterObject("Newsletter_Subscriber", PredefinedObjectType.NEWSLETTERSUBSCRIBER);
                resolver.RegisterObject("Newsletter_Newsletter", PredefinedObjectType.NEWSLETTER);
                resolver.RegisterObject("Newsletter_NewsletterIssue", PredefinedObjectType.NEWSLETTERISSUE);

                // Set additional context macros
                resolver.CurrentDocument = TreeNode.New("CMS.root");
                resolver.CurrentPageInfo = new PageInfo();
                resolver.CurrentPageInfo.UsedPageTemplateInfo = new PageTemplateInfo();

                // Disable context macros
                SetContext(resolver);

                mEmailTemplateResolver = resolver;
            }

            return mEmailTemplateResolver;
        }
    }


    /// <summary>
    /// Returns membership change password e-mail template macro resolver.
    /// </summary>
    public static ContextResolver MembershipChangePasswordResolver
    {
        get
        {
            if (mMembershipChangePasswordResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterValues(new string[] { "ResetPasswordUrl", "CancelUrl" });

                // Disable context macros
                SetContext(resolver);

                mMembershipChangePasswordResolver = resolver;
            }

            return mMembershipChangePasswordResolver;
        }
    }


    /// <summary>
    /// Blog e-mail template macro resolver.
    /// </summary>
    public static ContextResolver BlogsResolver
    {
        get
        {
            if (mBlogResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                // Blog document type doesn't exists, fill with the root doc. type
                if (DataHelper.DataSourceIsEmpty(DataClassInfoProvider.GetDataClass("cms.blog")))
                {
                    resolver.RegisterObject("Blog", TreeNode.New("CMS.Root"));
                }
                else
                {
                    resolver.RegisterObject("Blog", TreeNode.New("CMS.Blog"));
                }

                // BlogPost document type doesn't exists, fill with the root doc. type
                if (DataHelper.DataSourceIsEmpty(DataClassInfoProvider.GetDataClass("cms.blogpost")))
                {
                    resolver.RegisterObject("BlogPost", TreeNode.New("CMS.Root"));
                }
                else
                {
                    resolver.RegisterObject("BlogPost", TreeNode.New("CMS.BlogPost"));
                }
                resolver.RegisterObject("Comment", PredefinedObjectType.BLOGCOMMENT);
                resolver.RegisterObject("CommentUser", PredefinedObjectType.USER);
                resolver.RegisterObject("CommentUserSettings", SiteObjectType.USERSETTINGS);

                // Register flat values
                resolver.RegisterValues(new string[] { "UserFullName", "CommentUrl", "Comments", "CommentDate", "BlogPostTitle", "BlogLink", "BlogPostLink", "UnsubscriptionLink" });

                // Disable context macros
                SetContext(resolver);

                // Set BlogResolver
                mBlogResolver = resolver;
            }

            return mBlogResolver;
        }
    }


    /// <summary>
    /// Blog e-mail template macro resolver.
    /// </summary>
    public static ContextResolver BlogSubscriptionResolver
    {
        get
        {
            if (mBlogSubscribeResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                // Blog document type doesn't exists, fill with the root doc. type
                if (DataHelper.DataSourceIsEmpty(DataClassInfoProvider.GetDataClass("cms.blog")))
                {
                    resolver.RegisterObject("Blog", TreeNode.New("CMS.Root"));
                }
                else
                {
                    resolver.RegisterObject("Blog", TreeNode.New("CMS.Blog"));
                }

                // BlogPost document type doesn't exists, fill with the root doc. type
                if (DataHelper.DataSourceIsEmpty(DataClassInfoProvider.GetDataClass("cms.blogpost")))
                {
                    resolver.RegisterObject("BlogPost", TreeNode.New("CMS.Root"));
                }
                else
                {
                    resolver.RegisterObject("BlogPost", TreeNode.New("CMS.BlogPost"));
                }

                // Register flat values
                resolver.RegisterValues(new string[] { "UserFullName", "BlogPostTitle", "BlogLink", "BlogPostLink", "UnsubscriptionLink", "SubscriptionLink", "OptInInterval" });

                // Disable context macros
                SetContext(resolver);

                // Set BlogResolver
                mBlogSubscribeResolver = resolver;
            }

            return mBlogSubscribeResolver;
        }
    }


    /// <summary>
    /// Project management e-mail template macro resolver.
    /// </summary>
    public static ContextResolver ProjectManagement
    {
        get
        {
            if (mProjectManagementResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("ProjectTask", PredefinedObjectType.PROJECTTASK);
                resolver.RegisterObject("Project", PredefinedObjectType.PROJECT);
                resolver.RegisterObject("ProjectTaskStatus", PredefinedObjectType.PROJECTTASKSTATUS);

                resolver.SetNamedSourceData("Owner", PredefinedObjectType.USER);
                resolver.SetNamedSourceData("Assignee", PredefinedObjectType.USER);

                resolver.RegisterValues(new string[] { "TaskURL", "ProjectTaskDescriptionPlain", "ReminderMessage", "ReminderMessagePlain" });

                // Disable context macros
                SetContext(resolver);

                mProjectManagementResolver = resolver;
            }

            return mProjectManagementResolver;
        }
    }


    /// <summary>
    /// Boards e-mail template macro resolver.
    /// </summary>
    public static ContextResolver BoardsResolver
    {
        get
        {
            if (mBoardsResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("Board", "board.board");
                resolver.RegisterObject("Message", "board.message");
                resolver.RegisterObject("MessageUser", PredefinedObjectType.USER);
                resolver.RegisterObject("MessageUserSettings", SiteObjectType.USERSETTINGS);

                // Register flat values
                resolver.RegisterValues(new string[] { "UnsubscriptionLink", "DocumentLink" });

                // Disable context macros
                SetContext(resolver);

                mBoardsResolver = resolver;
            }

            return mBoardsResolver;
        }
    }


    /// <summary>
    /// Boards subscription e-mail template macro resolver.
    /// </summary>
    public static ContextResolver BoardsSubscriptionResolver
    {
        get
        {
            if (mBoardsSubscribeResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("Board", "board.board");

                // Register flat values
                resolver.RegisterValues(new string[] { "UnsubscriptionLink", "SubscriptionLink", "OptInInterval", "DocumentLink" });

                // Disable context macros
                SetContext(resolver);

                mBoardsSubscribeResolver = resolver;
            }

            return mBoardsSubscribeResolver;
        }
    }


    /// <summary>
    /// Booking e-mail template macro resolver.
    /// </summary>
    public static ContextResolver BookingResolver
    {
        get
        {
            if (mBookingResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                // Register "cms.bookingevent" object if exists
                if (CMSObjectHelper.GetReadOnlyObject("cms.bookingevent") != null)
                {
                    resolver.RegisterObject("Event", "cms.bookingevent");
                }
                resolver.RegisterObject("Attendee", "cms.eventattendee");

                // Set additional macros
                resolver.SetNamedSourceData("EventDateString", string.Empty);

                // Disable context macros
                SetContext(resolver);

                mBookingResolver = resolver;
            }

            return mBookingResolver;
        }
    }


    /// <summary>
    /// Forums e-mail template macro resolver.
    /// </summary>
    public static ContextResolver ForumsResolver
    {
        get
        {
            if (mForumsResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("ForumPost", PredefinedObjectType.FORUMPOST);
                resolver.RegisterObject("Forum", PredefinedObjectType.FORUM);
                resolver.RegisterObject("ForumGroup", PredefinedObjectType.FORUMGROUP);
                resolver.RegisterObject("Subscriber", "forums.forumsubscription");

                resolver.RegisterValues(new string[]
                                            {
                                                "ForumDisplayName", "PostSubject", "Link", "ForumName", "PostText", "PostUsername",
                                                "PostTime", "GroupDisplayname", "GroupName", "GroupDescription", "ForumDescription", "UnsubscribeLink"
                                            });

                // Disable context macros
                SetContext(resolver);

                mForumsResolver = resolver;
            }

            return mForumsResolver;
        }
    }


    /// <summary>
    /// Forums subscription e-mail template macro resolver.
    /// </summary>
    public static ContextResolver ForumsSubscribtionResolver
    {
        get
        {
            if (mForumsSubscribeResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                // Register objects
                resolver.RegisterObject("ForumPost", PredefinedObjectType.FORUMPOST);
                resolver.RegisterObject("Forum", PredefinedObjectType.FORUM);
                resolver.RegisterObject("ForumGroup", PredefinedObjectType.FORUMGROUP);

                // Register flat values
                resolver.RegisterValues(new string[] { "ForumDisplayName", "Subject", "Link", "UnsubscribeLink", "SubscribeLink", "Separator", "OptInInterval" });

                // Disable context macros
                SetContext(resolver);

                // Save the resolver for future use
                mForumsSubscribeResolver = resolver;
            }
            return mForumsSubscribeResolver;
        }
    }


    /// <summary>
    /// Friends e-mail template macro resolver.
    /// </summary>
    public static ContextResolver FriendsResolver
    {
        get
        {
            if (mFriendsResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("Sender", PredefinedObjectType.USER);
                resolver.RegisterObject("Recipient", PredefinedObjectType.USER);
                resolver.RegisterObject("Friendship", PredefinedObjectType.FRIEND);

                resolver.RegisterValues(new string[] { "ManagementUrl", "ProfileUrl", "FormatedSenderName", "FormatedRecipientName" });

                // Disable context macros
                SetContext(resolver);

                mFriendsResolver = resolver;
            }

            return mFriendsResolver;
        }
    }


    /// <summary>
    /// Groups e-mail template macro resolver.
    /// </summary>
    public static ContextResolver GroupMemberResolver
    {
        get
        {
            if (mGroupMemberResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("Group", PredefinedObjectType.GROUP);
                resolver.RegisterObject("MemberUser", PredefinedObjectType.USER);

                // Disable context macros
                SetContext(resolver);

                mGroupMemberResolver = resolver;
            }

            return mGroupMemberResolver;
        }
    }


    /// <summary>
    /// Groups invitation e-mail template macro resolver.
    /// </summary>
    public static ContextResolver GroupsInvitationResolver
    {
        get
        {
            if (mGroupsInvitationResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("Invitation", "community.invitation");
                resolver.RegisterObject("Group", PredefinedObjectType.GROUP);

                resolver.RegisterValues(new string[] { "AcceptionURL", "InvitedBy" });

                // Disable context macros
                SetContext(resolver);

                mGroupsInvitationResolver = resolver;
            }

            return mGroupsInvitationResolver;
        }
    }


    /// <summary>
    /// Groups accepted invitation e-mail template macro resolver.
    /// </summary>
    public static ContextResolver GroupsAcceptedInvitationResolver
    {
        get
        {
            if (mGroupsAcceptedInvitationResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("Sender", PredefinedObjectType.USER);
                resolver.RegisterObject("Recipient", PredefinedObjectType.USER);
                resolver.RegisterObject("GroupMember", PredefinedObjectType.GROUPMEMBER);
                resolver.RegisterObject("Group", PredefinedObjectType.GROUP);

                // Disable context macros
                SetContext(resolver);

                mGroupsAcceptedInvitationResolver = resolver;
            }

            return mGroupsAcceptedInvitationResolver;
        }
    }


    /// <summary>
    /// Password e-mail template macro resolver.
    /// </summary>
    public static ContextResolver PasswordResolver
    {
        get
        {
            if (mPasswordResolver == null)
            {
                ContextResolver resolver = RegistrationResolver.CreateContextChild();

                resolver.RegisterValues(new string[] { "Password" });

                // Disable context macros
                SetContext(resolver);

                mPasswordResolver = resolver;
            }

            return mPasswordResolver;
        }
    }


    /// <summary>
    /// Forgotten password e-mail template macro resolver.
    /// </summary>
    public static ContextResolver ForgottenPasswordResolver
    {
        get
        {
            if (mForgottenPasswordResolver == null)
            {
                ContextResolver resolver = PasswordResolver.CreateContextChild();

                resolver.RegisterValues(new string[] { "LogonURL" });

                mForgottenPasswordResolver = resolver;
            }

            return mForgottenPasswordResolver;
        }
    }


    /// <summary>
    /// Registration e-mail template macro resolver.
    /// </summary>
    public static ContextResolver RegistrationResolver
    {
        get
        {
            if (mRegistrationResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterValues(new string[] { "FirstName", "LastName", "Email", "UserName" });

                // Disable context macros
                SetContext(resolver);

                mRegistrationResolver = resolver;
            }

            return mRegistrationResolver;
        }
    }


    /// <summary>
    /// Registration approved e-mail template macro resolver.
    /// </summary>
    public static ContextResolver RegistrationApprovedResolver
    {
        get
        {
            if (mRegistrationApprovedResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterValues(new string[] { "HomePageURL" });

                // Disable context macros
                SetContext(resolver);

                mRegistrationApprovedResolver = resolver;
            }

            return mRegistrationApprovedResolver;
        }
    }


    /// <summary>
    /// Returns membership registration e-mail template macro resolver.
    /// </summary>
    public static ContextResolver MembershipRegistrationResolver
    {
        get
        {
            if (mMembershipRegistrationResolver == null)
            {
                ContextResolver resolver = PasswordResolver.CreateContextChild();

                resolver.RegisterValues(new string[] { "ConfirmAddress" });

                mMembershipRegistrationResolver = resolver;
            }

            return mMembershipRegistrationResolver;
        }
    }


    /// <summary>
    /// Returns membership expiration e-mail template macro resolver.
    /// </summary>
    public static ContextResolver MembershipExpirationResolver
    {
        get
        {
            if (mMembershipExpirationResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                // Expiring memberships
                resolver.SetNamedSourceData("MembershipsTable", (new DataTable()).Rows);

                // Disable context macros
                SetContext(resolver);

                mMembershipExpirationResolver = resolver;
            }

            return mMembershipExpirationResolver;
        }
    }


    /// <summary>
    /// Returns messaging e-mail template macro resolver.
    /// </summary>
    public static ContextResolver MessagingResolver
    {
        get
        {
            if (mMessagingResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("Sender", PredefinedObjectType.USER);
                resolver.RegisterObject("Recipient", PredefinedObjectType.USER);
                resolver.RegisterObject("Message", "messaging.message");

                resolver.RegisterValues(new string[] { "LogonUrl" });

                // Disable context macros
                SetContext(resolver);

                mMessagingResolver = resolver;
            }

            return mMessagingResolver;
        }
    }


    /// <summary>
    /// Returns base empty workflow macro resolver.
    /// </summary>
    public static ContextResolver WorkflowBaseResolver
    {
        get
        {
            if (mWorkflowBaseResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("CurrentUser", PredefinedObjectType.USER);

                // Disable context macros
                SetContext(resolver);

                mWorkflowBaseResolver = resolver;
            }

            return mWorkflowBaseResolver;
        }
    }


    /// <summary>
    /// Returns base empty workflow macro resolver with document field.
    /// </summary>
    public static ContextResolver WorkflowBaseDocumentResolver
    {
        get
        {
            if (mWorkflowBaseDocumentResolver == null)
            {
                ContextResolver resolver = WorkflowBaseResolver.CreateContextChild();

                resolver.RegisterObject("Document", TreeNode.New("CMS.Root"));

                // Disable context macros
                SetContext(resolver);

                mWorkflowBaseDocumentResolver = resolver;
            }

            return mWorkflowBaseDocumentResolver;
        }
    }


    /// <summary>
    /// Returns simple empty workflow macro resolver.
    /// </summary>
    public static ContextResolver WorkflowSimpleResolver
    {
        get
        {
            if (mWorkflowSimpleResolver == null)
            {
                ContextResolver resolver = WorkflowBaseResolver.CreateContextChild();

                resolver.RegisterObject("CurrentStep", PredefinedObjectType.WORKFLOWSTEP);
                resolver.RegisterObject("Workflow", PredefinedObjectType.WORKFLOW);
                resolver.RegisterObject("ActionDefinition", PredefinedObjectType.WORKFLOWACTION);

                // Disable context macros
                SetContext(resolver);

                mWorkflowSimpleResolver = resolver;
            }

            return mWorkflowSimpleResolver;
        }
    }


    /// <summary>
    /// Returns simple workflow macro resolver with document field.
    /// </summary>
    public static ContextResolver WorkflowSimpleDocumentResolver
    {
        get
        {
            if (mWorkflowSimpleDocumentResolver == null)
            {
                ContextResolver resolver = WorkflowSimpleResolver.CreateContextChild();

                resolver.RegisterObject("Document", TreeNode.New("CMS.Root"));

                // Disable context macros
                SetContext(resolver);

                mWorkflowSimpleDocumentResolver = resolver;
            }

            return mWorkflowSimpleDocumentResolver;
        }
    }


    /// <summary>
    /// Returns simple automation macro resolver with Contact field.
    /// </summary>
    public static ContextResolver AutomationSimpleResolver
    {
        get
        {
            if (mAutomationSimpleResolver == null)
            {
                ContextResolver resolver = WorkflowSimpleResolver.CreateContextChild();

                resolver.RegisterObject("Contact", PredefinedObjectType.CONTACT);
                resolver.RegisterObject("AutomationState", PredefinedObjectType.AUTOMATIONSTATE);

                // Disable context macros
                SetContext(resolver);

                mAutomationSimpleResolver = resolver;
            }

            return mAutomationSimpleResolver;
        }
    }


    /// <summary>
    /// Returns automation e-mail macro resolver with On-line marketing fields.
    /// </summary>
    public static ContextResolver AutomationEmailResolver
    {
        get
        {
            if (mAutomationSimpleResolver == null)
            {
                ContextResolver resolver = AutomationSimpleResolver.CreateContextChild();

                resolver.RegisterObject("Score", PredefinedObjectType.SCORE);
                resolver.RegisterObject("Activity", PredefinedObjectType.ACTIVITY);

                // Disable context macros
                SetContext(resolver);

                mAutomationEmailResolver = resolver;
            }

            return mAutomationEmailResolver;
        }
    }


    /// <summary>
    /// Returns workflow e-mail template macro resolver.
    /// </summary>
    public static ContextResolver WorkflowEmailResolver
    {
        get
        {
            if (mWorkflowEmailResolver == null)
            {
                ContextResolver resolver = WorkflowSimpleResolver.CreateContextChild();

                resolver.RegisterObject("Document", TreeNode.New("CMS.root"));
                resolver.RegisterObject("OriginalStep", PredefinedObjectType.WORKFLOWSTEP);

                resolver.RegisterValues(new string[] { "ApplicationURL", "ApprovedBy", "ApprovedWhen", "OriginalStepName", "CurrentStepName", "Comment", "FirstName", "LastName", "UserName", "Email", "FullName", "DocumentPreviewUrl", "DocumentEditUrl", "DocumentActionName" });

                // Prioritized properties
                resolver.PrioritizeProperty("ApplicationURL");
                resolver.PrioritizeProperty("Comment");
                resolver.PrioritizeProperty("DocumentPreviewUrl");
                resolver.PrioritizeProperty("DocumentEditUrl");
                resolver.PrioritizeProperty("DocumentActionName");

                // Disable context macros
                SetContext(resolver);

                mWorkflowEmailResolver = resolver;
            }

            return mWorkflowEmailResolver;
        }
    }


    /// <summary>
    /// Returns scoring e-mail template macro resolver.
    /// </summary>
    public static ContextResolver ScoringResolver
    {
        get
        {
            if (mScoringResolver == null)
            {
                ContextResolver resolver = ContactResolver.CreateContextChild();

                resolver.RegisterObject("Score", "om.score");

                resolver.SetNamedSourceData("ScoreValue", string.Empty);

                mScoringResolver = resolver;
            }

            return mScoringResolver;
        }
    }


    /// <summary>
    /// Returns translation services e-mail template macro resolver.
    /// </summary>
    public static ContextResolver TranslationsResolver
    {
        get
        {
            if (mTranslationsResolver == null)
            {
                ContextResolver resolver = ContactResolver.CreateContextChild();

                resolver.RegisterObject("Submission", "cms.translationsubmission");

                mTranslationsResolver = resolver;
            }

            return mTranslationsResolver;
        }
    }


    /// <summary>
    /// Returns contact resolver.
    /// </summary>
    public static ContextResolver ContactResolver
    {
        get
        {
            if (mContactResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("Contact", "OM.Contact");

                // Disable context macros
                SetContext(resolver);

                mContactResolver = resolver;
            }

            return mContactResolver;
        }
    }


    /// <summary>
    /// Returns contact activity resolver (for trigger macro).
    /// </summary>
    public static ContextResolver ContactActivityResolver
    {
        get
        {
            if (mContactActivityResolver == null)
            {
                ContextResolver resolver = ContactResolver.CreateContextChild();
                resolver.RegisterObject("Activity", "OM.Activity");

                mContactActivityResolver = resolver;
            }

            return mContactActivityResolver;
        }
    }


    /// <summary>
    /// Returns contact score resolver (for trigger macro).
    /// </summary>
    public static ContextResolver ContactScoreResolver
    {
        get
        {
            if (mContactScoreResolver == null)
            {
                ContextResolver resolver = ContactResolver.CreateContextChild();
                resolver.RegisterObject("Score", "OM.Score");

                mContactScoreResolver = resolver;
            }

            return mContactScoreResolver;
        }
    }


    /// <summary>
    /// Returns variant resolver.
    /// </summary>
    public static ContextResolver VariantResolver
    {
        get
        {
            if (mVariantResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.PrioritizeProperty("CurrentUser");
                resolver.PrioritizeProperty("OnlineMarketingContext");
                resolver.PrioritizeProperty("CurrentDevice");

                mVariantResolver = resolver;
            }

            return mVariantResolver;
        }
    }


    /// <summary>
    /// Returns web analytics resolver.
    /// </summary>
    public static ContextResolver AnalyticsResolver
    {
        get
        {
            if (mAnalyticsResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.PrioritizeProperty("CurrentUser");
                resolver.PrioritizeProperty("OnlineMarketingContext");
                resolver.PrioritizeProperty("CurrentDevice");

                mAnalyticsResolver = resolver;
            }

            return mAnalyticsResolver;
        }
    }


    /// <summary>
    /// Returns membership unlock account e-mail template macro resolver.
    /// </summary>
    public static ContextResolver MembershipUnlockAccountResolver
    {
        get
        {
            if (mMembershipUnlockAccountResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("User", PredefinedObjectType.USER);
                resolver.RegisterValues(new string[] { "ResetPasswordUrl", "UnlockAccountUrl" });

                resolver.PrioritizeProperty("ResetPasswordUrl");
                resolver.PrioritizeProperty("UnlockAccountUrl");

                // Disable context macros
                SetContext(resolver);

                mMembershipUnlockAccountResolver = resolver;
            }

            return mMembershipUnlockAccountResolver;
        }
    }


    /// <summary>
    /// Returns reporting e-mail template macro resolver.
    /// </summary>
    public static ContextResolver ReportingResolver
    {
        get
        {
            if (mReportingResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.RegisterObject("Report", PredefinedObjectType.REPORT);
                resolver.RegisterObject("ReportSubscription", PredefinedObjectType.REPORTSUBSCRIPTION);

                resolver.SetNamedSourceData("SubscriptionBody", String.Empty);
                resolver.SetNamedSourceData("DefaultSubscriptionCSS", String.Empty);
                resolver.SetNamedSourceData("UnsubscriptionLink", String.Empty);
                resolver.SetNamedSourceData("ItemName", String.Empty);

                // Disable context macros
                SetContext(resolver);

                mReportingResolver = resolver;
            }

            return mReportingResolver;
        }
    }



    /// <summary>
    /// Returns device profiles macro resolver.
    /// </summary>
    public static ContextResolver DeviceProfilesResolver
    {
        get
        {
            if (mDeviceProfilesResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                resolver.PrioritizeProperty("CurrentDevice");
                resolver.ShowOnlyPrioritized = true;

                mDeviceProfilesResolver = resolver;
            }

            return mDeviceProfilesResolver;
        }
    }

    #endregion


    #region "E-commerce resolvers"

    /// <summary>
    /// Shopping cart resolver.
    /// </summary>
    public static ContextResolver ShoppingCartResolver
    {
        get
        {
            if (mShoppingCartResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                // Data
                resolver.RegisterObject("Order", PredefinedObjectType.ORDER);
                resolver.RegisterObject("OrderStatus", PredefinedObjectType.ORDERSTATUS);
                resolver.RegisterObject("BillingAddress", "ecommerce.address");
                resolver.RegisterObject("ShippingAddress", "ecommerce.address");
                resolver.RegisterObject("CompanyAddress", "ecommerce.address");
                resolver.RegisterObject("ShippingOption", PredefinedObjectType.SHIPPINGOPTION);
                resolver.RegisterObject("PaymentOption", PredefinedObjectType.PAYMENTOPTION);
                resolver.RegisterObject("Currency", PredefinedObjectType.CURRENCY);
                resolver.RegisterObject("Customer", PredefinedObjectType.CUSTOMER);
                resolver.RegisterObject("DiscountCoupon", PredefinedObjectType.DISCOUNTCOUPON);
                resolver.RegisterObject("ShoppingCart", "ecommerce.shoppingcart");

                // Content tables                
                DataTable table = new DataTable();
                resolver.SetNamedSourceData("ContentTable", table.Rows);
                resolver.SetNamedSourceData("ContentTaxesTable", table.Rows);
                resolver.SetNamedSourceData("ShippingTaxesTable", table.Rows);

                // E-product table
                resolver.SetNamedSourceData("EproductsTable", table.Rows);

                // Disable context macros
                SetContext(resolver);

                mShoppingCartResolver = resolver;
            }

            return mShoppingCartResolver;
        }
    }


    /// <summary>
    /// E-commerce e-mail template macro resolver.
    /// </summary>
    public static ContextResolver EcommerceResolver
    {
        get
        {
            if (mEcommerceResolver == null)
            {
                // Copy all the data from ShoppingCart resolver
                ContextResolver resolver = ShoppingCartResolver.CreateContextChild();

                resolver.RegisterValues(new string[] { "TotalPrice", "TotalShipping", "NewOrderLink" });

                mEcommerceResolver = resolver;
            }

            return mEcommerceResolver;
        }
    }


    /// <summary>
    /// E-commerce expiring e-product e-mail template macro resolver.
    /// </summary>
    public static ContextResolver EcommerceEproductExpirationResolver
    {
        get
        {
            if (mEcommerceEproductExpirationResolver == null)
            {
                ContextResolver resolver = ContextResolver.GetInstance();

                // Expriring e-products
                resolver.SetNamedSourceData("EproductsTable", (new DataTable()).Rows);

                // Disable context macros
                SetContext(resolver);

                mEcommerceEproductExpirationResolver = resolver;
            }

            return mEcommerceEproductExpirationResolver;
        }
    }

    #endregion

}
