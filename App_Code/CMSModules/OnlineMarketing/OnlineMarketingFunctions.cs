using System;
using System.Data;
using System.Web;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.WorkflowEngine;
using CMS.UIControls;
using CMS.Newsletter;
using CMS.WebAnalytics;

/// <summary>
/// Online marketing functions.
/// </summary>
public static class OnlineMarketingFunctions
{
    /// <summary>
    /// Returns contact's last activity of specified activity type.
    /// </summary>
    /// <param name="contact">Contact info object</param>
    /// <param name="activityType">Activity type</param>
    public static ActivityInfo LastActivityOfType(object contact, object activityType)
    {
        ContactInfo ci = (ContactInfo)contact;
        string type = ValidationHelper.GetString(activityType, string.Empty);

        if (ci != null)
        {
            return ActivityInfoProvider.GetContactsLastActivity(ci.ContactID, type);
        }

        return null;
    }


    /// <summary>
    /// Returns contact's first activity of specified activity type.
    /// </summary>
    /// <param name="contact">Contact info object</param>
    /// <param name="activityType">Activity type</param>
    public static ActivityInfo FirstActivityOfType(object contact, object activityType)
    {
        ContactInfo ci = (ContactInfo)contact;
        string type = ValidationHelper.GetString(activityType, string.Empty);

        if (ci != null)
        {
            return ActivityInfoProvider.GetContactsFirstActivity(ci.ContactID, type);
        }

        return null;
    }


    /// <summary>
    /// Returns TRUE if the contact is in any/all specified contact group(s) on current site.
    /// </summary>
    /// <param name="contact">Contact info object</param>
    /// <param name="contactGroupName">Contact group name(s) separated by semicolon</param>
    /// <param name="allGroups">If true contact has to be in all specified groups, if false, it is sufficient if the contact is in any of the specified groups.</param>
    public static bool IsInContactGroup(object contact, object contactGroupName, bool allGroups)
    {
        ContactInfo ci = (ContactInfo)contact;
        string groupNames = ValidationHelper.GetString(contactGroupName, string.Empty);
        int siteId = CMSContext.CurrentSiteID;

        if ((ci != null) && (siteId > 0) && (!string.IsNullOrEmpty(groupNames)))
        {
            string where = null;
            string[] names = groupNames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string groupName in names)
            {
                where = SqlHelperClass.AddWhereCondition("ContactGroupName='" + SqlHelperClass.GetSafeQueryString(groupName, false) + "'", where, "OR");
            }
            where = SqlHelperClass.AddWhereCondition("ContactGroupSiteID=" + siteId + " AND ContactGroupID IN (SELECT ContactGroupMemberContactGroupID FROM OM_ContactGroupMember WHERE ContactGroupMemberRelatedID=" + ci.ContactID + " AND ContactGroupMemberType=0)", where);

            // Get contact group ID if the name matches and contact is member of the group
            DataSet ds = ContactGroupInfoProvider.GetContactGroups(where, null, 0, "DISTINCT ContactGroupName");
            if (allGroups)
            {
                return ds.Tables[0].Rows.Count >= names.Length;
            }
            else
            {
                return !DataHelper.DataSourceIsEmpty(ds);
            }
        }

        return false;
    }


    /// <summary>
    /// Returns contact's points in specified score on current site.
    /// </summary>
    /// <param name="contact">Contact info object</param>
    /// <param name="scoreName">Score name</param>
    public static int GetScore(object contact, object scoreName)
    {
        ContactInfo ci = (ContactInfo)contact;
        string score = ValidationHelper.GetString(scoreName, string.Empty);
        int siteId = CMSContext.CurrentSiteID;

        if ((ci != null) && (siteId > 0) && (!string.IsNullOrEmpty(score)))
        {
            // Prepare DB query
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@ContactID", ci.ContactID);
            parameters.Add("@ScoreName", score);
            parameters.Add("@SiteID", siteId);

            string where = "(Expiration IS NULL OR Expiration > GetDate()) AND (ContactID=@ContactID) AND (ScoreName=@ScoreName) AND (ScoreSiteID=@SiteID)";

            // Get sum of points of the contact in specified score
            DataSet ds = SqlHelperClass.ExecuteQuery("om.score.selectdatajoined", parameters, "SUM(Value) AS Score", where, null, -1);

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                return ValidationHelper.GetInteger(ds.Tables[0].Rows[0][0], 0);
            }
        }

        return 0;
    }


    /// <summary>
    /// Returns e-mail domain name.
    /// </summary>
    /// <param name="email">E-mail address</param>
    public static string GetEmailDomain(object email)
    {
        return ContactHelper.GetEmailDomain(ValidationHelper.GetString(email, string.Empty));
    }


    /// <summary>
    /// Returns true if contact belongs to specified account.
    /// </summary>
    /// <param name="contact">Contact info object</param>
    /// <param name="accountName">Account ID</param>
    public static bool BelongsToAccount(object contact, object accountID)
    {
        ContactInfo ci = contact as ContactInfo;
        if (ci == null)
        {
            return false;
        }

        int accId = ValidationHelper.GetInteger(accountID, 0);
        if (accId > 0)
        {
            return AccountContactInfoProvider.GetAccountContactInfo(accId, ci.ContactID) != null;
        }

        return false;
    }


    /// <summary>
    /// Returns true if the contact did any/all of the specified activities.
    /// </summary>
    /// <param name="contact">Contact the activities of which should be checked</param>
    /// <param name="activityType">Name of the activity(ies) to check separated with semicolon</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    /// <param name="allActivities">If true, all specified types has to be found for the method to return true. If false, at least one of any specified types is sufficient to return true for the method.</param>
    public static bool DidActivities(object contact, string activityTypes, int lastXDays, bool allActivities)
    {
        ContactInfo ci = contact as ContactInfo;
        if (ci == null)
        {
            return false;
        }

        string where = null;
        string[] activities = activityTypes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string activityType in activities)
        {
            where = SqlHelperClass.AddWhereCondition(where, "ActivityType = N'" + SqlHelperClass.GetSafeQueryString(activityType, false) + "'", "OR");
        }

        if (lastXDays > 0)
        {
            where = SqlHelperClass.AddWhereCondition(where, "DATEADD(day, " + lastXDays + ", ActivityCreated) >= GETDATE()");
        }
        where = SqlHelperClass.AddWhereCondition(where, "ActivityActiveContactID = " + ci.ContactID);

        DataSet ds = ActivityInfoProvider.GetActivities(where, null, 0, "DISTINCT ActivityType");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            if (allActivities)
            {
                return ds.Tables[0].Rows.Count == activities.Length;
            }
            else
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Returns true if the contact did a specified activity.
    /// </summary>
    /// <param name="contact">Contact the activities of which should be checked</param>
    /// <param name="activityType">Name of the activity to check (can specify more than one separated with semicolon, all of the types match than)</param>
    /// <param name="cancelActivityType">Name of the activity which cancels the original activity (for example UnsubscribeNewsletter is a cancelling event for SubscribeNewsletter etc.)</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    /// <param name="whereCondition">Additional WHERE condition</param>
    public static bool DidActivity(object contact, string activityType, string cancelActivityType, int lastXDays, string whereCondition)
    {
        ContactInfo ci = contact as ContactInfo;
        if (ci == null)
        {
            return false;
        }

        string where = GetActivityWhere(activityType, cancelActivityType, lastXDays, whereCondition, ci.ContactID);

        DataSet ds = ActivityInfoProvider.GetActivities(where, null, 1, "ActivityID");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            return true;
        }

        return false;
    }


    private static string GetActivityWhere(string activityType, string cancelActivityType, int lastXDays, string whereCondition, int contactId)
    {
        string where = "ActivityActiveContactID = " + contactId;
        if (!string.IsNullOrEmpty(activityType))
        {
            string[] types = activityType.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string typeWhere = null;
            foreach (string type in types)
            {
                typeWhere = SqlHelperClass.AddWhereCondition(typeWhere, "ActivityType = N'" + SqlHelperClass.GetSafeQueryString(type, false) + "'", "OR");
            }
            where = SqlHelperClass.AddWhereCondition(where, typeWhere);
        }
        if (!string.IsNullOrEmpty(cancelActivityType))
        {
            // Exclude activities which has been cancelled by its counter activity (subscribe vs. unsubscribe)
            where = SqlHelperClass.AddWhereCondition(where, "NOT EXISTS(SELECT ActivityID FROM OM_Activity AS A2 WHERE A2.ActivityType = N'" + SqlHelperClass.GetSafeQueryString(cancelActivityType, false) + "' AND ActivityItemID = A2.ActivityItemID AND A2.ActivityCreated > ActivityCreated)");
        }
        if (lastXDays > 0)
        {
            where = SqlHelperClass.AddWhereCondition(where, "DATEADD(day, " + lastXDays + ", ActivityCreated) >= GETDATE()");
        }
        if (!string.IsNullOrEmpty(whereCondition))
        {
            where = SqlHelperClass.AddWhereCondition(where, whereCondition);
        }
        return where;
    }


    /// <summary>
    /// Returns true if the contact's country matches one of the specified countries.
    /// </summary>
    /// <param name="contact">Contact the activities of which should be checked</param>
    /// <param name="countryList">Countries list (separated with semicolon)</param>
    public static bool IsFromCountry(object contact, string countryList)
    {
        ContactInfo ci = contact as ContactInfo;
        if (ci == null)
        {
            return false;
        }

        CountryInfo country = CountryInfoProvider.GetCountryInfo(ci.ContactCountryID);
        if (country == null)
        {
            return false;
        }

        string[] countries = countryList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string c in countries)
        {
            if (c.EqualsCSafe(country.CountryDisplayName, true) || c.EqualsCSafe(country.CountryName, true))
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Returns true if the contact's country matches one of the specified countries.
    /// </summary>
    /// <param name="contact">Contact the activities of which should be checked</param>
    /// <param name="stateList">State list (separated with semicolon)</param>
    public static bool IsFromState(object contact, string stateList)
    {
        ContactInfo ci = contact as ContactInfo;
        if (ci == null)
        {
            return false;
        }

        StateInfo state = StateInfoProvider.GetStateInfo(ci.ContactStateID);
        if (state == null)
        {
            return false;
        }

        string[] states = stateList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in states)
        {
            if (s.EqualsCSafe(state.StateDisplayName, true) || s.EqualsCSafe(state.StateName, true))
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Returns true if the contact did search (internal or external) for specified keywords in last X days.
    /// </summary>
    /// <param name="contact">Contact the activities of which should be checked</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    /// <param name="keywords">Keywords separated with comma or semicolon (if null or empty, than method returns true iff contact did any search in last X days)</param>
    /// <param name="allKeywords">If true all keywords has to be matched, if false at least one match has to be found</param>
    public static bool SearchedForKeywords(object contact, string keywords, int lastXDays, bool allKeywords)
    {
        // Make sure correct operator is passed
        string op = null;
        if (allKeywords)
        {
            op = "AND";
        }
        else
        {
            op = "OR";
        }

        string where = null;
        string[] words = keywords.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length > 0)
        {
            string innerWhere = null;
            foreach (string item in words)
            {
                innerWhere = SqlHelperClass.AddWhereCondition(innerWhere, "SearchKeywords LIKE N'%" + SqlHelperClass.GetSafeQueryString(item) + "%'", op);
            }
            where = SqlHelperClass.AddWhereCondition(where, "ActivityID IN (SELECT SearchActivityID FROM OM_Search WHERE " + innerWhere + ")");
        }
        if (!string.IsNullOrEmpty(keywords))
        {
            where = SqlHelperClass.AddWhereCondition(where, "ActivityID IN (SELECT SearchActivityID FROM OM_Search WHERE SearchKeywords = N'" + SqlHelperClass.GetSafeQueryString(keywords, false) + "')", "OR");
        }

        return DidActivity(contact, PredefinedActivityType.INTERNAL_SEARCH + ";" + PredefinedActivityType.EXTERNAL_SEARCH, null, lastXDays, where);
    }


    /// <summary>
    /// Returns true if the contact spent specified amount of money (counted by order total price in main currency) in last X days.
    /// </summary>
    /// <param name="contact">Contact the activities of which should be checked</param>
    /// <param name="lowerBound">Inclusive lower bound of the amount of money (in main currency) spent by contact</param>
    /// <param name="upperBound">Inclusive upper bound of the amount of money (in main currency) spent by contact</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    public static bool SpentMoney(object contact, double lowerBound, double upperBound, int lastXDays)
    {
        ContactInfo ci = contact as ContactInfo;
        if (ci == null)
        {
            return false;
        }

        string where = String.Format("ActivityType = '{0}' AND ActivityActiveContactID = {1}", PredefinedActivityType.PURCHASE, ci.ContactID);
        if (lastXDays > 0)
        {
            where = SqlHelperClass.AddWhereCondition(where, "DATEADD(day, " + lastXDays + ", ActivityCreated) >= GETDATE()");
        }

        DataSet ds = ActivityInfoProvider.GetActivities(where, null, 1, "SUM(CAST(ActivityValue AS FLOAT))");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            double amount = ValidationHelper.GetDouble(ds.Tables[0].Rows[0][0], -1);
            if ((amount > -1) && (amount >= lowerBound) && (amount <= upperBound))
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Returns true if the contact purchased specified number of products in last X days.
    /// </summary>
    /// <param name="contact">Contact the activities of which should be checked</param>
    /// <param name="lowerBound">Inclusive lower bound of the number of products bought</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    public static bool PurchasedNumberOfProducts(object contact, int lowerBound, int lastXDays)
    {
        ContactInfo ci = contact as ContactInfo;
        if (ci == null)
        {
            return false;
        }

        string where = String.Format("ActivityType = '{0}' AND ActivityActiveContactID = {1}", PredefinedActivityType.PURCHASEDPRODUCT, ci.ContactID);
        if (lastXDays > 0)
        {
            where = SqlHelperClass.AddWhereCondition(where, "DATEADD(day, " + lastXDays + ", ActivityCreated) >= GETDATE()");
        }

        DataSet ds = ActivityInfoProvider.GetActivities(where, null, 1, "COUNT(ActivityID)");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            int amount = ValidationHelper.GetInteger(ds.Tables[0].Rows[0][0], -1);
            if ((amount > -1) && (amount >= lowerBound))
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Returns true if the contact is in any/all of the specified roles (i.e. if any of the user assigned to the contact is in any/all specified roles).
    /// </summary>
    /// <param name="contact">Contact which should be checked</param>
    /// <param name="roleNames">Name of the roles separated with semicolon</param>
    /// <param name="allRoles">If true, contact has to in all specified roles. If false, it is sufficient if the contact is at least in one of the roles.</param>
    public static bool IsInRoles(object contact, string roleNames, bool allRoles)
    {
        ContactInfo ci = contact as ContactInfo;
        if (ci == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(roleNames))
        {
            string[] roles = roleNames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string roleName in roles)
            {
                bool isInRole = false;
                foreach (UserInfo user in ci.Users)
                {
                    isInRole = user.IsInRole(roleName, CMSContext.CurrentSiteName, true, true);
                    if (isInRole && !allRoles)
                    {
                        // In role and it is sufficient if at least one matches => true
                        return true;
                    }
                }

                if (!isInRole && allRoles)
                {
                    // Not in role, but should be in all roles
                    return false;
                }
            }
        }

        if (allRoles)
        {
            // It means either no roles were specified or contact is in all roles
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Returns true if the contact is in any/all of the specified community groups (i.e. if any of the user assigned to the contact is in any/all specified community groups).
    /// </summary>
    /// <param name="contact">Contact which should be checked</param>
    /// <param name="groupNames">Name of the groups separated with semicolon</param>
    /// <param name="allGroups">If true, contact has to in all specified groups. If false, it is sufficient if the contact is at least in one of the groups.</param>
    public static bool IsInCommunityGroup(object contact, string groupNames, bool allGroups)
    {
        ContactInfo ci = contact as ContactInfo;
        if (ci == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(groupNames))
        {
            string[] groups = groupNames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string groupName in groups)
            {
                bool isInGroup = false;
                foreach (UserInfo user in ci.Users)
                {
                    BaseInfo group = ModuleCommands.CommunityGetGroupInfoByName(groupName, CMSContext.CurrentSiteName);
                    if (group != null)
                    {
                        isInGroup = ModuleCommands.CommunityIsMemberOfGroup(user.UserID, group.Generalized.ObjectID);
                        if (isInGroup && !allGroups)
                        {
                            // In role and it is sufficient if at least one matches => true
                            return true;
                        }
                    }
                }

                if (!isInGroup && allGroups)
                {
                    // Not in role, but should be in all roles
                    return false;
                }
            }
        }

        if (allGroups)
        {
            // It means either no roles were specified or contact is in all roles
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Returns true if the contact submitted specified form.
    /// </summary>
    /// <param name="contact">Contact which should be checked</param>
    /// <param name="formName">Form name</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    public static bool SubmittedForm(object contact, string formName, int lastXDays)
    {
        int formId = ValidationHelper.GetInteger(CMSMacroMethods.GetObjectID(PredefinedObjectType.BIZFORM, formName, CMSContext.CurrentSiteName, true), 0);
        return DidActivity(contact, PredefinedActivityType.BIZFORM_SUBMIT, null, lastXDays, "ActivityItemID = " + formId);
    }


    /// <summary>
    /// Returns true if the contact opened specified newsletter.
    /// </summary>
    /// <param name="contact">Contact which should be checked</param>
    /// <param name="newsletterName">Newsletter name</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    public static bool OpenedNewsletter(object contact, string newsletterName, int lastXDays)
    {
        int newsletterId = ValidationHelper.GetInteger(CMSMacroMethods.GetObjectID(PredefinedObjectType.NEWSLETTER, newsletterName, CMSContext.CurrentSiteName), 0);
        return DidActivity(contact, PredefinedActivityType.NEWSLETTER_OPEN, null, lastXDays, "ActivityItemID = " + newsletterId);
    }


    /// <summary>
    /// Returns true if the contact voted in the poll.
    /// </summary>
    /// <param name="contact">Contact which should be checked</param>
    /// <param name="pollName">Poll name</param>
    /// <param name="answer">Poll answer text</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    public static bool VotedInPoll(object contact, string pollName, string answer, int lastXDays)
    {
        int pollId = ValidationHelper.GetInteger(CMSMacroMethods.GetObjectID(PredefinedObjectType.POLL, pollName, CMSContext.CurrentSiteName, true), 0);
        if (!string.IsNullOrEmpty(answer))
        {
            ContactInfo ci = contact as ContactInfo;
            if (ci == null)
            {
                return false;
            }

            string where = GetActivityWhere(PredefinedActivityType.POLL_VOTING, null, lastXDays, "ActivityItemID = " + pollId, ci.ContactID);
            DataSet ds = ActivityInfoProvider.GetActivities(where, null, 0, "ActivityValue");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string[] answers = ValidationHelper.GetString(dr[0], "").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string a in answers)
                    {
                        BaseInfo pollAnswer = CMSObjectHelper.GetObjectById("polls.pollanswer", ValidationHelper.GetInteger(a, 0));
                        if (pollAnswer != null)
                        {
                            if (pollAnswer.GetStringValue("AnswerText", "").EqualsCSafe(answer, true))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        else
        {
            return DidActivity(contact, PredefinedActivityType.POLL_VOTING, null, lastXDays, "ActivityItemID = " + pollId);
        }
    }


    /// <summary>
    /// Returns true if the contact logged in.
    /// </summary>
    /// <param name="contact">Contact which should be checked</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    public static bool LoggedIn(object contact, int lastXDays)
    {
        return DidActivity(contact, PredefinedActivityType.USER_LOGIN, null, lastXDays, null);
    }


    /// <summary>
    /// Returns true if the contact subscribed for specific newsletter.
    /// </summary>
    /// <param name="contact">Contact which should be checked</param>
    /// <param name="newsletterName">Newsletter name</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    public static bool SubscribedToNewsletter(object contact, string newsletterName, int lastXDays)
    {
        int newsletterId = ValidationHelper.GetInteger(CMSMacroMethods.GetObjectID(PredefinedObjectType.NEWSLETTER, newsletterName, CMSContext.CurrentSiteName, true), 0);
        return DidActivity(contact, PredefinedActivityType.NEWSLETTER_SUBSCRIBING, PredefinedActivityType.NEWSLETTER_UNSUBSCRIBING, lastXDays, "ActivityItemID = " + newsletterId);
    }


    /// <summary>
    /// Returns true if the contact clicked link in specified newsletter.
    /// </summary>
    /// <param name="contact">Contact which should be checked</param>
    /// <param name="newsletterName">Newsletter name</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    public static bool ClickedLinkInNewsletter(object contact, string newsletterName, int lastXDays)
    {
        int newsletterId = ValidationHelper.GetInteger(CMSMacroMethods.GetObjectID(PredefinedObjectType.NEWSLETTER, newsletterName, CMSContext.CurrentSiteName, true), 0);
        return DidActivity(contact, PredefinedActivityType.NEWSLETTER_CLICKTHROUGH, null, lastXDays, "ActivityItemID = " + newsletterId);
    }


    /// <summary>
    /// Returns true if the contact clicked link in the specified newsletter issue.
    /// </summary>
    /// <param name="contact">Contact that should be checked</param>
    /// <param name="issueGuid">Newsletter issue GUID</param>
    /// <param name="click">Indicates whether contact should have clicked the newsletter issue or just open it.</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    public static bool InteractedWithNewsletterIssue(object contact, Guid issueGuid, bool click, int lastXDays)
    {
        ContactInfo ci = contact as ContactInfo;
        if (ci != null)
        {
            IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueGuid, ci.ContactSiteID);
            if (issue != null)
            {
                string where = String.Format("ActivityItemID = {0} AND ActivityItemDetailID = {1}", issue.IssueNewsletterID, issue.IssueID);
                return DidActivity(contact, click ? PredefinedActivityType.NEWSLETTER_CLICKTHROUGH : PredefinedActivityType.NEWSLETTER_OPEN, null, lastXDays, where);
            }
        }
        return false;
    }


    /// <summary>
    /// Returns true if the contact purchased specified product.
    /// </summary>
    /// <param name="contact">Contact which should be checked</param>
    /// <param name="skuName">Name of the product</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    public static bool PurchasedProduct(object contact, string skuName, int lastXDays)
    {
        int skuId = ValidationHelper.GetInteger(CMSMacroMethods.GetObjectID(PredefinedObjectType.SKU, skuName, CMSContext.CurrentSiteName, true), 0);
        return DidActivity(contact, PredefinedActivityType.PURCHASEDPRODUCT, null, lastXDays, "ActivityItemID = " + skuId);
    }


    /// <summary>
    /// Returns true if the contact visited specified page.
    /// </summary>
    /// <param name="contact">Contact which should be checked</param>
    /// <param name="nodeAliasPath">Page node alias path</param>
    /// <param name="culture">Culture of the document</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    public static bool VisitedPage(object contact, string nodeAliasPath, string culture, int lastXDays)
    {
        ContactInfo ci = contact as ContactInfo;
        if (ci == null)
        {
            return false;
        }

        if (ci.ContactSiteID == 0)
        {
            return false;
        }

        TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);
        TreeNode node = tree.SelectSingleNode(SiteInfoProvider.GetSiteName(ci.ContactSiteID), nodeAliasPath, CMSContext.PreferredCultureCode);
        if (node != null)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return DidActivity(contact, PredefinedActivityType.PAGE_VISIT, null, lastXDays, "ActivityNodeID = " + node.NodeID);
            }
            else
            {
                return DidActivity(contact, PredefinedActivityType.PAGE_VISIT, null, lastXDays, "ActivityNodeID = " + node.NodeID + " AND ActivityCulture = '" + culture + "'");
            }
        }
        return false;
    }


    /// <summary>
    /// Returns true if the contact registered for specific event.
    /// </summary>
    /// <param name="contact">Contact which should be checked</param>
    /// <param name="nodeAliasPath">Node alias path of BookingEvent document</param>
    /// <param name="lastXDays">Constraint for last X days (if zero or negative value is given, no constraint is applied)</param>
    public static bool RegisteredForEvent(object contact, string nodeAliasPath, int lastXDays)
    {
        return DidActivity(contact, PredefinedActivityType.EVENT_BOOKING, null, lastXDays, "ActivityItemDetailID IN (SELECT DocumentID FROM CMS_Document WHERE DocumentNodeID IN (SELECT NodeID FROM CMS_Tree WHERE NodeAliasPath = N'" + SqlHelperClass.GetSafeQueryString(nodeAliasPath, false) + "'))");
    }


    /// <summary>
    /// Returns last newsletter issue that was sent to the contact of the state.
    /// </summary>
    /// <param name="state">Automation state</param>
    public static IssueInfo GetLastNewsletterIssue(AutomationStateInfo state)
    {
        // Check if automation state is null
        if (state == null)
        {
            return null;
        }

        // Get newsletter issue GUID
        var newsletterGuid = ValidationHelper.GetGuid(state.StateCustomData[SendNewsletterIssueAction.LAST_SENT_NEWSLETTER_ISSUE_KEY], Guid.Empty);
        if (newsletterGuid == Guid.Empty)
        {
            return null;
        }

        return IssueInfoProvider.GetIssueInfo(newsletterGuid, state.StateSiteID);
    }


    /// <summary>
    /// Returns if activity is linked to object of type given in parameter and codename given in second parameter.
    /// </summary>
    /// <param name="activityInfo">Activity that should be checked</param>
    /// <param name="objectType">Object type</param>
    /// <param name="identifier">Code name or GUID of object</param>
    public static bool ActivityLinkedToObject(ActivityInfo activityInfo, string objectType, string identifier)
    {
        if (activityInfo == null)
        {
            return false;
        }

        int id = 0;
        switch (objectType)
        {

            case PredefinedObjectType.POLL:
            case PredefinedObjectType.NEWSLETTER:
            case PredefinedObjectType.BIZFORM:
            case PredefinedObjectType.FORUM:
            case PredefinedObjectType.GROUPMEMBER:
            case PredefinedObjectType.BOARDMESSAGE:
            case PredefinedObjectType.BOARD:
            case PredefinedObjectType.SKU:
                id = activityInfo.ActivityItemID;
                break;
            case PredefinedObjectType.NEWSLETTERISSUE:
                var issue = IssueInfoProvider.GetOriginalIssue(activityInfo.ActivityItemDetailID);
                if (issue != null)
                {
                    id = issue.IssueID;
                }
                break;
            case PredefinedObjectType.BLOGDOCUMENT:
                id = activityInfo.ActivityItemDetailID;
                objectType = PredefinedObjectType.NODE;
                break;
            case PredefinedObjectType.DOCUMENT:
            case PredefinedObjectType.NODE:
                id = activityInfo.ActivityNodeID;
                break;
            default:
                id = activityInfo.ActivityItemID;
                break;
        }

        return (id == ValidationHelper.GetInteger(CMSMacroMethods.GetObjectID(objectType, identifier, CMSContext.CurrentSiteName, true), -1));
    }
}