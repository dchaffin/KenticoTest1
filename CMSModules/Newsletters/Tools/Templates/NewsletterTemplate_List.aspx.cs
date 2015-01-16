using System;
using System.Collections.Generic;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

// Title
[Title("Objects/Newsletter_EmailTemplate/object.png", "newsletters.templates", "CMS_Newsletter_Templates", HelpName="helpTopic")]
public partial class CMSModules_Newsletters_Tools_Templates_NewsletterTemplate_List : CMSNewsletterTemplatesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Add subscriber link
        string[,] actions = new string[1,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("NewsletterTemplate_List.NewItemCaption");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl("NewsletterTemplate_New.aspx");
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("Objects/Newsletter_EmailTemplate/add.png");
        CurrentMaster.HeaderActions.Actions = actions;

        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.WhereCondition = "(TemplateSiteID = " + CMSContext.CurrentSiteID + ")";
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
    }


    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
                // Initialize template type column
            case "templatetype":
                switch (parameter.ToString().ToLowerCSafe())
                {
                    case "u":
                        return GetString("NewsletterTemplate_List.Unsubscription");

                    case "s":
                        return GetString("NewsletterTemplate_List.Subscription");

                    case "d":
                        return GetString("NewsletterTemplate_List.OptIn");

                    default:
                        return GetString("NewsletterTemplate_List.Issue");
                }
        }
        return parameter;
    }


    /// <summary>
    /// Increment counter at the end of string.
    /// </summary>
    /// <param name="s">String</param>
    /// <param name="lpar">Left parathenses</param>
    /// <param name="rpar">Right parathenses</param>
    private string Increment(string s, string lpar, string rpar)
    {
        int i = 1;
        s = s.Trim();
        if ((rpar == String.Empty) || s.EndsWithCSafe(rpar))
        {
            int leftpar = s.LastIndexOfCSafe(lpar);
            if (lpar == rpar)
            {
                leftpar = s.LastIndexOfCSafe(lpar, leftpar - 1);
            }

            if (leftpar >= 0)
            {
                i = ValidationHelper.GetSafeInteger(s.Substring(leftpar + lpar.Length, s.Length - leftpar - lpar.Length - rpar.Length), 0);
                // Remove parathenses only if parathenses found
                if (i > 0)
                {
                    s = s.Remove(leftpar);
                }
                i++;
            }
        }

        s += lpar + i + rpar;
        return s;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        string templateId = actionArgument.ToString();

        switch (actionName.ToLowerCSafe())
        {
                // Edit the template
            case "edit":
                URLHelper.Redirect("NewsletterTemplate_Edit.aspx?templateid=" + templateId);
                break;

                // Delete the template
            case "delete":
                // Check 'Manage templates' permission
                if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "managetemplates"))
                {
                    RedirectToCMSDeskAccessDenied("cms.newsletter", "managetemplates");
                }

                // Check if the template is used in a newsletter
                string where = string.Format("(NewsletterTemplateID={0}) OR (NewsletterSubscriptionTemplateID={0}) OR (NewsletterUnsubscriptionTemplateID={0}) OR (NewsletterOptInTemplateID={0})", templateId);

                DataSet newsByEmailtempl = NewsletterInfoProvider.GetNewsletters(where, null, 1, "NewsletterID");
                if (DataHelper.DataSourceIsEmpty(newsByEmailtempl))
                {
                    // Check if the template is used in an issue
                    DataSet newsletterIssues = IssueInfoProvider.GetIssues("IssueTemplateID = " + templateId, null, 1, "IssueID");
                    if (DataHelper.DataSourceIsEmpty(newsletterIssues))
                    {
                        // Delete EmailTemplate object from database
                        EmailTemplateInfoProvider.DeleteEmailTemplateInfo(ValidationHelper.GetInteger(templateId, 0));
                    }
                    else
                    {
                        ShowError(GetString("NewsletterTemplate_List.TemplateInUseByNewsletterIssue"));
                    }
                }
                else
                {
                    ShowError(GetString("NewsletterTemplate_List.TemplateInUseByNewsletter"));
                }
                break;
        }
    }
}