using System;
using System.Collections.Generic;
using System.Web;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.DocumentEngine;
using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.Newsletter;
using CMS.WorkflowEngine;

/// <summary>
/// Online marketing methods - wrapping methods for macro resolver.
/// </summary>
public static class OnlineMarketingMethods
{
    /// <summary>
    /// Registers all online marketing methods to macro resolver.
    /// </summary>
    public static void RegisterMethods()
    {
        // Get e-mail domain method
        MacroMethod getEmailDomain = new MacroMethod("GetEmailDomain", GetEmailDomain)
        {
            Comment = "Returns e-mail domain name.",
            Type = typeof(string),
            MinimumParameters = 1
        };
        getEmailDomain.AddParameter("email", typeof(string), "E-mail address.");
        MacroMethods.RegisterMethod(getEmailDomain);

        // Get score method
        MacroMethod getScore = new MacroMethod("GetScore", GetScore)
        {
            Comment = "Returns contact's points in specified score on current site.",
            Type = typeof(int),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        getScore.AddParameter("contact", typeof(object), "Contact info object.");
        getScore.AddParameter("scoreName", typeof(string), "Name of the score to get contact's points of.");
        MacroMethods.RegisterMethod(getScore);

        // Is in contact group method
        MacroMethod isInContactGroup = new MacroMethod("IsInContactGroup", IsInContactGroup)
        {
            Comment = "Returns true if contact is in contact group.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        isInContactGroup.AddParameter("contact", typeof(object), "Contact info object.");
        isInContactGroup.AddParameter("groupNames", typeof(string), "Name of the contact group(s) separated by semicolon to test whether contact is in.");
        isInContactGroup.AddParameter("allGroups", typeof(bool), "If true contact has to be in all specified groups, if false, it is sufficient if the contact is in any of the specified groups.");
        MacroMethods.RegisterMethod(isInContactGroup);

        // First activity of type method
        MacroMethod firstActivityOfType = new MacroMethod("FirstActivityOfType", FirstActivityOfType)
        {
            Comment = "Returns contact's first activity of specified activity type.",
            Type = typeof(ActivityInfo),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        firstActivityOfType.AddParameter("contact", typeof(object), "Contact info object.");
        firstActivityOfType.AddParameter("activityType", typeof(string), "Name of the activity type, optional.");
        MacroMethods.RegisterMethod(firstActivityOfType);

        // Last activity of type method
        MacroMethod lastActivityOfType = new MacroMethod("LastActivityOfType", LastActivityOfType)
        {
            Comment = "Returns contact's last activity of specified activity type.",
            Type = typeof(ActivityInfo),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        lastActivityOfType.AddParameter("contact", typeof(object), "Contact info object.");
        lastActivityOfType.AddParameter("activityType", typeof(string), "Name of the activity type, optional.");
        MacroMethods.RegisterMethod(lastActivityOfType);

        MacroMethod belongsToAccount = new MacroMethod("BelongsToAccount", BelongsToAccount)
        {
            Comment = "Returns true if the contact belongs to the specified account.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        belongsToAccount.AddParameter("contact", typeof(object), "Contact info object.");
        belongsToAccount.AddParameter("accountID", typeof(int), "ID of the account.");
        MacroMethods.RegisterMethod(belongsToAccount);

        MacroMethod cameToLandingPage = new MacroMethod("CameToLandingPage", CameToLandingPage)
        {
            Comment = "Returns true if the contact came to specified landing page.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        cameToLandingPage.AddParameter("contact", typeof(object), "Contact info object.");
        cameToLandingPage.AddParameter("page", typeof(string), "Node ID or node alias path of the landing page.");
        MacroMethods.RegisterMethod(cameToLandingPage);

        MacroMethod didActivity = new MacroMethod("DidActivity", DidActivity)
        {
            Comment = "Returns true if the contact did a specified activity.",
            Type = typeof(bool),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        didActivity.AddParameter("contact", typeof(object), "Contact info object.");
        didActivity.AddParameter("activityType", typeof(string), "Name of the activity to check.");
        didActivity.AddParameter("cancelActivityType", typeof(string), "Name of the activity which cancels the original activity (for example UnsubscribeNewsletter is a cancelling event for SubscribeNewsletter etc.).");
        didActivity.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        didActivity.AddParameter("whereCondition", typeof(string), "Additional WHERE condition.");
        MacroMethods.RegisterMethod(didActivity);

        MacroMethod didActivities = new MacroMethod("DidActivities", DidActivities)
        {
            Comment = "Returns true if the contact did any/all of the specified activities.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        didActivities.AddParameter("contact", typeof(object), "Contact info object.");
        didActivities.AddParameter("activityTypes", typeof(string), "Name of the activity(ies) to check separated with semicolon.");
        didActivities.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        didActivities.AddParameter("allActivities", typeof(string), "If true, all specified types has to be found for the method to return true. If false, at least one of any specified types is sufficient to retrun true for the method.");
        MacroMethods.RegisterMethod(didActivities);

        MacroMethod isFromCountry = new MacroMethod("IsFromCountry", IsFromCountry)
        {
            Comment = "Returns true if the contact's country matches one of the specified countries.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        isFromCountry.AddParameter("contact", typeof(object), "Contact info object.");
        isFromCountry.AddParameter("countryList", typeof(string), "List of country names separated with semicolon.");
        MacroMethods.RegisterMethod(isFromCountry);

        MacroMethod isFromState = new MacroMethod("IsFromState", IsFromState)
        {
            Comment = "Returns true if the contact's state matches one of the specified state.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        isFromState.AddParameter("contact", typeof(object), "Contact info object.");
        isFromState.AddParameter("stateList", typeof(string), "List of state names separated with semicolon.");
        MacroMethods.RegisterMethod(isFromState);

        MacroMethod searchedForKeywords = new MacroMethod("SearchedForKeywords", SearchedForKeywords)
        {
            Comment = "Returns true if the contact did search for specified keywords in last X days.",
            Type = typeof(bool),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        searchedForKeywords.AddParameter("contact", typeof(object), "Contact info object.");
        searchedForKeywords.AddParameter("keywords", typeof(string), "Keywords separated with comma or semicolon (if null or empty, than method returns true iff contact did any search in last X days).");
        searchedForKeywords.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        searchedForKeywords.AddParameter("op", typeof(string), "Operator to be used in WHERE condition (use AND for all keywords match, use OR for any keword match).");
        MacroMethods.RegisterMethod(searchedForKeywords);

        MacroMethod spentMoney = new MacroMethod("SpentMoney", SpentMoney)
        {
            Comment = "Returns true if the contact spent specified amount of money (counted by order total price in main currency) in last X days.",
            Type = typeof(bool),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        spentMoney.AddParameter("contact", typeof(object), "Contact info object.");
        spentMoney.AddParameter("lowerBound", typeof(double), "Inclusive lower bound of the amount of money (in main currency) spent by contact.");
        spentMoney.AddParameter("upperBound", typeof(double), "Inclusive upper bound of the amount of money (in main currency) spent by contact.");
        spentMoney.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(spentMoney);

        MacroMethod purchasedNumberOfProducts = new MacroMethod("PurchasedNumberOfProducts", PurchasedNumberOfProducts)
        {
            Comment = "Returns true if the contact purchased specified number of products in last X days.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        purchasedNumberOfProducts.AddParameter("contact", typeof(object), "Contact info object.");
        purchasedNumberOfProducts.AddParameter("lowerBound", typeof(int), "Inclusive lower bound of the number of products bought.");
        purchasedNumberOfProducts.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(purchasedNumberOfProducts);

        MacroMethod isInRoles = new MacroMethod("IsInRoles", IsInRoles)
        {
            Comment = "Returns true if the contact is in any/all of the specified roles (i.e. if any of the user assigned to the contact is in any/all specified roles).",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        isInRoles.AddParameter("contact", typeof(object), "Contact info object.");
        isInRoles.AddParameter("roleNames", typeof(string), "Name of the roles separated with semicolon.");
        isInRoles.AddParameter("allRoles", typeof(bool), "If true, contact has to in all specified roles. If false, it is sufficient if the contact is at least in one of the roles.");
        MacroMethods.RegisterMethod(isInRoles);

        MacroMethod isInCommunityGroup = new MacroMethod("IsInCommunityGroup", IsInCommunityGroup)
        {
            Comment = "Returns true if the contact is in any/all of the specified community groups (i.e. if any of the user assigned to the contact is in any/all specified community groups).",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        isInCommunityGroup.AddParameter("contact", typeof(object), "Contact info object.");
        isInCommunityGroup.AddParameter("groupNames", typeof(string), "Name of the groups separated with semicolon.");
        isInCommunityGroup.AddParameter("allGroups", typeof(bool), "If true, contact has to in all specified groups. If false, it is sufficient if the contact is at least in one of the groups.");
        MacroMethods.RegisterMethod(isInCommunityGroup);

        MacroMethod visitedPage = new MacroMethod("VisitedPage", VisitedPage)
        {
            Comment = "Returns true if the contact visited specified page.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        visitedPage.AddParameter("contact", typeof(object), "Contact info object.");
        visitedPage.AddParameter("nodeAliasPath", typeof(string), "Page node alias path.");
        visitedPage.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(visitedPage);

        MacroMethod submittedForm = new MacroMethod("SubmittedForm", SubmittedForm)
        {
            Comment = "Returns true if the contact submitted specified form.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        submittedForm.AddParameter("contact", typeof(object), "Contact info object.");
        submittedForm.AddParameter("formName", typeof(string), "Name of the on-line form.");
        submittedForm.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(submittedForm);

        MacroMethod openedNewsletter = new MacroMethod("OpenedNewsletter", OpenedNewsletter)
        {
            Comment = "Returns true if the contact opened specified newsletter.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        openedNewsletter.AddParameter("contact", typeof(object), "Contact info object.");
        openedNewsletter.AddParameter("newsletterName", typeof(string), "Newsletter name");
        openedNewsletter.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(openedNewsletter);

        MacroMethod openedNewsletterIssue = new MacroMethod("OpenedNewsletterIssue", OpenedNewsletterIssue)
        {
            Comment = "Returns true if the contact opened specified newsletter issue.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        openedNewsletterIssue.AddParameter("contact", typeof(object), "Contact info object.");
        openedNewsletterIssue.AddParameter("issueGuid", typeof(Guid), "Newsletter issue GUID.");
        openedNewsletterIssue.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(openedNewsletterIssue);

        MacroMethod votedInPoll = new MacroMethod("VotedInPoll", VotedInPoll)
        {
            Comment = "Returns true if the contact voted in the poll.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        votedInPoll.AddParameter("contact", typeof(object), "Contact info object.");
        votedInPoll.AddParameter("pollName", typeof(string), "Poll name.");
        votedInPoll.AddParameter("pollAnswer", typeof(string), "Poll answer text.");
        votedInPoll.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(votedInPoll);

        MacroMethod loggedIn = new MacroMethod("LoggedIn", LoggedIn)
        {
            Comment = "Returns true if the contact logged in.",
            Type = typeof(bool),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        loggedIn.AddParameter("contact", typeof(object), "Contact info object.");
        loggedIn.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(loggedIn);

        MacroMethod subscribedToNewsletter = new MacroMethod("SubscribedToNewsletter", SubscribedToNewsletter)
        {
            Comment = "Returns true if the contact subscribed for specific newsletter.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        subscribedToNewsletter.AddParameter("contact", typeof(object), "Contact info object.");
        subscribedToNewsletter.AddParameter("newsletterName", typeof(string), "Newsletter name.");
        subscribedToNewsletter.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(subscribedToNewsletter);

        MacroMethod registeredForEvent = new MacroMethod("RegisteredForEvent", RegisteredForEvent)
        {
            Comment = "Returns true if the contact registered for specific event.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        registeredForEvent.AddParameter("contact", typeof(object), "Contact info object.");
        visitedPage.AddParameter("nodeAliasPath", typeof(string), "Page node alias path.");
        registeredForEvent.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(registeredForEvent);

        MacroMethod clickedLinkInNewsletter = new MacroMethod("ClickedLinkInNewsletter", ClickedLinkInNewsletter)
        {
            Comment = "Returns true if the contact clicked link in specified newsletter.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        clickedLinkInNewsletter.AddParameter("contact", typeof(object), "Contact info object.");
        clickedLinkInNewsletter.AddParameter("newsletterName", typeof(string), "Name of the newsletter.");
        clickedLinkInNewsletter.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(clickedLinkInNewsletter);

        MacroMethod clickedLinkInNewsletterIssue = new MacroMethod("ClickedLinkInNewsletterIssue", ClickedLinkInNewsletterIssue)
        {
            Comment = "Returns true if the contact clicked link in specified newsletter issue.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        clickedLinkInNewsletterIssue.AddParameter("contact", typeof(object), "Contact info object.");
        clickedLinkInNewsletterIssue.AddParameter("issueGuid", typeof(Guid), "Newsletter issue GUID.");
        clickedLinkInNewsletterIssue.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(clickedLinkInNewsletterIssue);

        MacroMethod purchasedProduct = new MacroMethod("PurchasedProduct", PurchasedProduct)
        {
            Comment = "Returns true if the contact purchased specified product.",
            Type = typeof(bool),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(ContactInfo) }
        };
        purchasedProduct.AddParameter("contact", typeof(object), "Contact info object.");
        purchasedProduct.AddParameter("productName", typeof(string), "Name of the product.");
        purchasedProduct.AddParameter("lastXDays", typeof(int), "Constraint for last X days (if zero or negative value is given, no constraint is applied).");
        MacroMethods.RegisterMethod(purchasedProduct);

        MacroMethod getLastNewsletterIssue = new MacroMethod("GetLastNewsletterIssue", GetLastNewsletterIssue)
        {
            Comment = "Returns last newsletter issue that was sent to the contact of the state.",
            Type = typeof(IssueInfo),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(AutomationStateInfo) }
        };
        getLastNewsletterIssue.AddParameter("state", typeof(AutomationStateInfo), "Process instance to get last newsletter issue from.");
        MacroMethods.RegisterMethod(getLastNewsletterIssue);

        MacroMethod linkedToObject = new MacroMethod("LinkedToObject", ActivityLinkedToObject)
        {
            Comment = "Returns if activity is linked to given object by GUID or codename.",
            Type = typeof(bool),
            MinimumParameters = 3,
            AllowedTypes = new List<Type>() { typeof(ActivityInfo) }
        };
        linkedToObject.AddParameter("activity", typeof(ActivityInfo), "Activity info object.");
        linkedToObject.AddParameter("objectType", typeof(string), "Object type.");
        linkedToObject.AddParameter("objectIdentifier", typeof(string), "Object code name or GUID.");
        MacroMethods.RegisterMethod(linkedToObject);
    }


    /// <summary>
    /// Returns true if the contact is in any/all of the specified roles (i.e. if any of the user assigned to the contact is in any/all specified roles).
    /// </summary>
    /// <param name="parameters">
    /// Contact which should be checked;
    /// Name of the roles;
    /// If true, contact has to in all specified roles. If false, it is sufficient if the contact is at least in one of the roles;
    /// </param>
    public static object IsInRoles(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.IsInRoles(parameters[0], ValidationHelper.GetString(parameters[1], ""), false);

            case 3:
                return OnlineMarketingFunctions.IsInRoles(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetBoolean(parameters[2], false));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact is in any/all of the specified community groups (i.e. if any of the user assigned to the contact is in any/all specified community groups).
    /// </summary>
    /// <param name="parameters">
    /// Contact which should be checked;
    /// Name of the groups separated with semicolon
    /// If true, contact has to in all specified groups. If false, it is sufficient if the contact is at least in one of the groups;
    /// </param>
    public static object IsInCommunityGroup(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.IsInCommunityGroup(parameters[0], ValidationHelper.GetString(parameters[1], ""), false);

            case 3:
                return OnlineMarketingFunctions.IsInCommunityGroup(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetBoolean(parameters[2], false));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns contact's last activity of specified activity type.
    /// </summary>
    /// <param name="parameters">ID of current contact; Name of the activity type</param>
    public static object LastActivityOfType(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return OnlineMarketingFunctions.LastActivityOfType(parameters[0], null);

            case 2:
                return OnlineMarketingFunctions.LastActivityOfType(parameters[0], parameters[1]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns contact's first activity of specified activity type.
    /// </summary>
    /// <param name="parameters">ID of current contact; Name of the activity type</param>
    public static object FirstActivityOfType(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return OnlineMarketingFunctions.FirstActivityOfType(parameters[0], null);

            case 2:
                return OnlineMarketingFunctions.FirstActivityOfType(parameters[0], parameters[1]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns TRUE if the contact is in specified contact group on current site.
    /// </summary>
    /// <param name="parameters">
    /// ID of current contact; 
    /// Name of the contact group(s) separated by semicolon;
    /// If true contact has to be in all specified groups, if false, it is sufficient if the contact is in any of the specified groups.
    /// </param>
    public static object IsInContactGroup(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.IsInContactGroup(parameters[0], parameters[1], false);

            case 3:
                return OnlineMarketingFunctions.IsInContactGroup(parameters[0], parameters[1], ValidationHelper.GetBoolean(parameters[2], false));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns contact's points in specified score on current site.
    /// </summary>
    /// <param name="parameters">ID of current contact; Name of the score</param>
    public static object GetScore(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.GetScore(parameters[0], parameters[1]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns e-mail domain name.
    /// </summary>
    /// <param name="parameters">E-mail address</param>
    public static object GetEmailDomain(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return OnlineMarketingFunctions.GetEmailDomain(parameters[0]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if contact belongs to specified account.
    /// </summary>
    /// <param name="parameters">Contact info object; Account name</param>
    public static object BelongsToAccount(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.BelongsToAccount(parameters[0], parameters[1]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact did any/all of the specified activities.
    /// </summary>
    /// <param name="parameters">
    /// Contact the activities of which should be checked;
    /// Name of the activity(ies) to check separated with semicolon;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// If true, all specified types has to be found for the method to return true. If false, at least one of any specified types is sufficient to return true for the method;
    /// </param>
    public static object DidActivities(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.DidActivities(parameters[0], ValidationHelper.GetString(parameters[1], ""), 0, false);

            case 3:
                return OnlineMarketingFunctions.DidActivities(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetInteger(parameters[2], 0), false);

            case 4:
                return OnlineMarketingFunctions.DidActivities(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetInteger(parameters[2], 0), ValidationHelper.GetBoolean(parameters[3], false));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if contact belongs to specified account.
    /// </summary>
    /// <param name="parameters">
    /// Contact the activities of which should be checked;
    /// Name of the activity to check;
    /// Name of the activity which cancels the original activity (for example UnsubscribeNewsletter is a cancelling event for SubscribeNewsletter etc.);
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// Additional WHERE condition;
    /// </param>
    public static object DidActivity(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return OnlineMarketingFunctions.DidActivity(parameters[0], null, null, 0, null);

            case 2:
                return OnlineMarketingFunctions.DidActivity(parameters[0], ValidationHelper.GetString(parameters[1], ""), null, 0, null);

            case 3:
                return OnlineMarketingFunctions.DidActivity(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetString(parameters[2], ""), 0, null);

            case 4:
                return OnlineMarketingFunctions.DidActivity(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetString(parameters[2], ""), ValidationHelper.GetInteger(parameters[3], 0), null);

            case 5:
                return OnlineMarketingFunctions.DidActivity(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetString(parameters[2], ""), ValidationHelper.GetInteger(parameters[3], 0), ValidationHelper.GetString(parameters[4], ""));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if contact came to specified landing page.
    /// </summary>
    /// <param name="parameters">Contact; Node ID or alias path of the page</param>
    public static object CameToLandingPage(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                int nodeId = ValidationHelper.GetInteger(parameters[1], 0);
                string nodeIds = null;
                if (nodeId <= 0)
                {
                    string alias = ValidationHelper.GetString(parameters[1], "");
                    if (!string.IsNullOrEmpty(alias))
                    {
                        TreeNodeDataSet ds = new TreeProvider().SelectNodes(TreeProvider.ALL_SITES, alias, TreeProvider.ALL_CULTURES, true);
                        if (!DataHelper.DataSourceIsEmpty(ds))
                        {
                            nodeIds = TextHelper.Join(",", SystemDataHelper.GetStringValues(ds.Tables[0], "NodeID"));
                        }
                    }
                }

                if (nodeId > 0)
                {
                    return OnlineMarketingFunctions.DidActivity(parameters[0], "landingpage", null, 0, "ActivityNodeID = " + nodeId);
                }
                else if (!string.IsNullOrEmpty(nodeIds))
                {
                    return OnlineMarketingFunctions.DidActivity(parameters[0], "landingpage", null, 0, "ActivityNodeID IN (" + nodeIds + ")");
                }
                return false;

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact's country matches one of the specified countries.
    /// </summary>
    /// <param name="parameters">Contact; List of countries</param>
    public static object IsFromCountry(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.IsFromCountry(parameters[0], ValidationHelper.GetString(parameters[1], ""));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact's state matches one of the specified countries.
    /// </summary>
    /// <param name="parameters">Contact; List of states</param>
    public static object IsFromState(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.IsFromState(parameters[0], ValidationHelper.GetString(parameters[1], ""));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact did search for specified keywords in last X days.
    /// </summary>
    /// <param name="parameters">
    /// Contact the activities of which should be checked;
    /// Keywords separated with comma or semicolon (if null or empty, than method returns true iff contact did any search in last X days);
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// Operator to be used in WHERE condition (use AND for all keywords match, use OR for any keyword match);
    /// </param>
    public static object SearchedForKeywords(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return OnlineMarketingFunctions.SearchedForKeywords(parameters[0], null, 0, false);

            case 2:
                return OnlineMarketingFunctions.SearchedForKeywords(parameters[0], ValidationHelper.GetString(parameters[1], ""), 0, false);

            case 3:
                return OnlineMarketingFunctions.SearchedForKeywords(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetInteger(parameters[2], 0), false);

            case 4:
                return OnlineMarketingFunctions.SearchedForKeywords(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetInteger(parameters[2], 0), ValidationHelper.GetBoolean(parameters[3], false));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact spent specified amount of money (counted by order total price in main currency) in last X days.
    /// </summary>
    /// <param name="parameters">
    /// Contact the activities of which should be checked;
    /// Inclusive lower bound of the amount of money (in main currency) spent by contact;
    /// Inclusive upper bound of the amount of money (in main currency) spent by contact;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object SpentMoney(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 3:
                return OnlineMarketingFunctions.SpentMoney(parameters[0], ValidationHelper.GetDouble(parameters[1], 0), ValidationHelper.GetDouble(parameters[2], 0), 0);

            case 4:
                return OnlineMarketingFunctions.SpentMoney(parameters[0], ValidationHelper.GetDouble(parameters[1], 0), ValidationHelper.GetDouble(parameters[2], 0), ValidationHelper.GetInteger(parameters[3], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact purchased specified number of products in last X days.
    /// </summary>
    /// <param name="parameters">
    /// Contact the activities of which should be checked;
    /// Inclusive lower bound of the number of products bought;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object PurchasedNumberOfProducts(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.PurchasedNumberOfProducts(parameters[0], ValidationHelper.GetInteger(parameters[1], 0), 0);

            case 3:
                return OnlineMarketingFunctions.PurchasedNumberOfProducts(parameters[0], ValidationHelper.GetInteger(parameters[1], 0), ValidationHelper.GetInteger(parameters[2], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact visited specified page.
    /// </summary>
    /// <param name="parameters">
    /// Contact which should be checked;
    /// Page node alias path;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object VisitedPage(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.VisitedPage(parameters[0], ValidationHelper.GetString(parameters[1], ""), null, 0);

            case 3:
                return OnlineMarketingFunctions.VisitedPage(parameters[0], ValidationHelper.GetString(parameters[1], ""), null, ValidationHelper.GetInteger(parameters[2], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact submitted specified form.
    /// </summary>
    /// <param name="parameters">
    /// Contact which should be checked;
    /// Form name;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object SubmittedForm(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.SubmittedForm(parameters[0], ValidationHelper.GetString(parameters[1], ""), 0);

            case 3:
                return OnlineMarketingFunctions.SubmittedForm(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetInteger(parameters[2], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact opened specified newsletter.
    /// </summary>
    /// <param name="parameters">
    /// Contact which should be checked;
    /// Newsletter ID;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object OpenedNewsletter(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.OpenedNewsletter(parameters[0], ValidationHelper.GetString(parameters[1], ""), 0);

            case 3:
                return OnlineMarketingFunctions.OpenedNewsletter(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetInteger(parameters[2], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact voted in the poll.
    /// </summary>
    /// <param name="parameters">
    /// Contact which should be checked;
    /// Poll ID;
    /// Poll answer ID;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object VotedInPoll(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.VotedInPoll(parameters[0], ValidationHelper.GetString(parameters[1], ""), null, 0);

            case 3:
                return OnlineMarketingFunctions.VotedInPoll(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetString(parameters[2], ""), 0);

            case 4:
                return OnlineMarketingFunctions.VotedInPoll(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetString(parameters[2], ""), ValidationHelper.GetInteger(parameters[3], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact logged in.
    /// </summary>
    /// <param name="parameters">
    /// Contact which should be checked;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object LoggedIn(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.LoggedIn(parameters[0], ValidationHelper.GetInteger(parameters[1], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact subscribed for specific newsletter.
    /// </summary>
    /// <param name="parameters">
    /// Contact which should be checked;
    /// Newsletter ID;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object SubscribedToNewsletter(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.SubscribedToNewsletter(parameters[0], ValidationHelper.GetString(parameters[1], ""), 0);

            case 3:
                return OnlineMarketingFunctions.SubscribedToNewsletter(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetInteger(parameters[2], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact registered for specific event.
    /// </summary>
    /// <param name="parameters">
    /// Contact which should be checked;
    /// Node alias path of the document;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object RegisteredForEvent(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.RegisteredForEvent(parameters[0], ValidationHelper.GetString(parameters[1], ""), 0);

            case 3:
                return OnlineMarketingFunctions.RegisteredForEvent(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetInteger(parameters[2], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact clicked link in specified newsletter.
    /// </summary>
    /// <param name="parameters">
    /// Contact which should be checked;
    /// Newsletter ID;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object ClickedLinkInNewsletter(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.ClickedLinkInNewsletter(parameters[0], ValidationHelper.GetString(parameters[1], ""), 0);

            case 3:
                return OnlineMarketingFunctions.ClickedLinkInNewsletter(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetInteger(parameters[2], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact clicked link in the specified newsletter issue.
    /// </summary>
    /// <param name="parameters">
    /// Contact that should be checked;
    /// Newsletter issue GUID;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object ClickedLinkInNewsletterIssue(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.InteractedWithNewsletterIssue(parameters[0], ValidationHelper.GetGuid(parameters[1], Guid.Empty), true, 0);

            case 3:
                return OnlineMarketingFunctions.InteractedWithNewsletterIssue(parameters[0], ValidationHelper.GetGuid(parameters[1], Guid.Empty), true, ValidationHelper.GetInteger(parameters[2], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact opened the specified newsletter issue
    /// </summary>
    /// <param name="parameters">
    /// Contact that should be checked;
    /// Newsletter issue GUID;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object OpenedNewsletterIssue(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.InteractedWithNewsletterIssue(parameters[0], ValidationHelper.GetGuid(parameters[1], Guid.Empty), false, 0);

            case 3:
                return OnlineMarketingFunctions.InteractedWithNewsletterIssue(parameters[0], ValidationHelper.GetGuid(parameters[1], Guid.Empty), false, ValidationHelper.GetInteger(parameters[2], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns true if the contact purchased specified product.
    /// </summary>
    /// <param name="parameters">
    /// Contact which should be checked;
    /// ID of the product;
    /// Constraint for last X days (if zero or negative value is given, no constraint is applied);
    /// </param>
    public static object PurchasedProduct(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return OnlineMarketingFunctions.PurchasedProduct(parameters[0], ValidationHelper.GetString(parameters[1], ""), 0);

            case 3:
                return OnlineMarketingFunctions.PurchasedProduct(parameters[0], ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetInteger(parameters[2], 0));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns GUID of last newsletter that was sent to the user in scope of given automation state. 
    /// </summary>
    /// <param name="parameters">Automation state</param>
    public static object GetLastNewsletterIssue(params object[] parameters)
    {
        if (parameters.Length != 1)
        {
            throw new NotSupportedException();
        }
        return OnlineMarketingFunctions.GetLastNewsletterIssue(parameters[0] as AutomationStateInfo);
    }


    /// <summary>
    /// Returns if activity is linked to object of type given in parameter and codename given in second parameter.
    /// </summary>
    /// <param name="parameters">
    /// Activity which should be checked;
    /// Object type;
    /// Code name or GUID of object;
    /// </param>
    public static object ActivityLinkedToObject(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 3:
                return OnlineMarketingFunctions.ActivityLinkedToObject(parameters[0] as ActivityInfo, parameters[1] as string, parameters[2] as string);

            default:
                throw new NotSupportedException();
        }
    }
}