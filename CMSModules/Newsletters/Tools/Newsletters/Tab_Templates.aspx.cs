using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.Newsletter;
using CMS.CMSHelper;

public partial class CMSModules_Newsletters_Tools_Newsletters_Tab_Templates : CMSNewsletterNewslettersPage
{
    #region "Variables"

    private NewsletterInfo newsletterInfo;
    private DataSet templateNewsletters;
    private int newsletterId;
    private string currentValues;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        newsletterId = QueryHelper.GetInteger("newsletterId", 0);
        if (newsletterId <= 0)
        {
            pnlAvailability.Visible = false;
            return;
        }

        EditedObject = newsletterInfo = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);

        if (!RequestHelper.IsPostBack())
        {
            LoadBindings();
        }

        // Show all issue templates from current site
        usTemplates.WhereCondition = String.Format("TemplateType='{0}' AND TemplateSiteID={1} AND TemplateID<>{2}", SqlHelperClass.GetSafeQueryString(EmailTemplateType.Issue), newsletterInfo.NewsletterSiteID, newsletterInfo.NewsletterTemplateID);
        usTemplates.OnSelectionChanged += usTemplates_OnSelectionChanged;
    }

    #endregion


    #region "Control event handlers"

    /// <summary>
    /// Uniselector event handler.
    /// </summary>
    protected void usTemplates_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveBindings();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Load control.
    /// </summary>
    private void LoadBindings()
    {
        GetCurrentNewsletters();
        usTemplates.Value = currentValues;
        usTemplates.Reload(true);
    }


    /// <summary>
    /// Loads current templates from DB.
    /// </summary>
    private void GetCurrentNewsletters()
    {
        string where = string.Format("NewsletterID = {0}", newsletterInfo.NewsletterID);
        templateNewsletters = EmailTemplateNewsletterInfoProvider.GetEmailTemplatesNewsletters(where, "TemplateID");

        if (!DataHelper.DataSourceIsEmpty(templateNewsletters))
        {
            currentValues = TextHelper.Join(";", SystemDataHelper.GetStringValues(templateNewsletters.Tables[0], "TemplateID"));
        }
        else
        {
            currentValues = String.Empty;
        }
    }


    /// <summary>
    /// Save changes.
    /// </summary>
    private void SaveBindings()
    {
        // Check 'Manage templates' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "managetemplates"))
        {
            RedirectToCMSDeskAccessDenied("cms.newsletter", "managetemplates");
        }

        if (RequestHelper.IsPostBack())
        {
            GetCurrentNewsletters();
        }

        string newValues = ValidationHelper.GetString(usTemplates.Value, null);
        RemoveOldRecords(newValues, currentValues);
        AddNewRecords(newValues, currentValues);
        currentValues = newValues;
    }


    /// <summary>
    /// Remove templates from newsletter.
    /// </summary>
    private void RemoveOldRecords(string newValues, string currentValues)
    {
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] modifiedItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (modifiedItems != null)
            {
                foreach (string item in modifiedItems)
                {
                    EmailTemplateNewsletterInfoProvider.RemoveNewsletterFromTemplate(ValidationHelper.GetInteger(item, 0), newsletterId);
                }
            }
        }
    }


    /// <summary>
    /// Add templates to newsletter.
    /// </summary>
    private void AddNewRecords(string newValues, string currentValues)
    {
        string items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] modifiedItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (modifiedItems != null)
            {
                foreach (string item in modifiedItems)
                {
                    EmailTemplateNewsletterInfoProvider.AddNewsletterToTemplate(ValidationHelper.GetInteger(item, 0), newsletterId);
                }
            }
        }
    }

    #endregion
}