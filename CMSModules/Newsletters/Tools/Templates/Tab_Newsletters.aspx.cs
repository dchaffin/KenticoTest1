using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.Newsletter;
using CMS.CMSHelper;

public partial class CMSModules_Newsletters_Tools_Templates_Tab_Newsletters : CMSNewsletterTemplatesPage
{
    #region "Variables"

    private EmailTemplateInfo emailTemplateInfo;
    private DataSet templateNewsletters;
    private int templateId;
    private string currentValues;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        templateId = QueryHelper.GetInteger("templateid", 0);

        if (templateId <= 0)
        {
            pnlAvailability.Visible = false;
            return;
        }

        EditedObject = emailTemplateInfo = EmailTemplateInfoProvider.GetEmailTemplateInfo(templateId);

        // Initialize newsletter selector
        usNewsletters.WhereCondition = String.Format("NewsletterSiteID={0} AND NewsletterType='{1}' AND NewsletterTemplateID<>{2}", CMSContext.CurrentSiteID,
            SqlHelperClass.GetSafeQueryString(NewsletterType.TemplateBased), emailTemplateInfo.TemplateID);

        if (!RequestHelper.IsPostBack())
        {
            LoadSiteBindings();
        }

        usNewsletters.OnSelectionChanged += usSites_OnSelectionChanged;
    }

    #endregion


    #region "Control event handlers"

    /// <summary>
    /// Uniselector event handler.
    /// </summary>
    protected void usSites_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveSiteBindings();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Load control.
    /// </summary>
    private void LoadSiteBindings()
    {
        GetCurrentNewsletters();
        usNewsletters.Value = currentValues;
        usNewsletters.Reload(true);
    }


    /// <summary>
    /// Loads current newsletters from DB.
    /// </summary>
    private void GetCurrentNewsletters()
    {
        string where = string.Format("TemplateID = {0}", emailTemplateInfo.TemplateID);
        templateNewsletters = EmailTemplateNewsletterInfoProvider.GetEmailTemplatesNewsletters(where, "NewsletterID");

        if (!DataHelper.DataSourceIsEmpty(templateNewsletters))
        {
            currentValues = TextHelper.Join(";", SystemDataHelper.GetStringValues(templateNewsletters.Tables[0], "NewsletterID"));
        }
        else
        {
            currentValues = string.Empty;
        }
    }


    /// <summary>
    /// Save changes.
    /// </summary>
    private void SaveSiteBindings()
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

        string newValues = ValidationHelper.GetString(usNewsletters.Value, null);
        RemoveOldRecords(newValues, currentValues);
        AddNewRecords(newValues, currentValues);
        currentValues = newValues;
    }


    /// <summary>
    /// Remove newsletters from template.
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
                    EmailTemplateNewsletterInfoProvider.RemoveNewsletterFromTemplate(templateId, ValidationHelper.GetInteger(item, 0));
                }
            }
        }
    }


    /// <summary>
    /// Add newsletters to template.
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
                    EmailTemplateNewsletterInfoProvider.AddNewsletterToTemplate(templateId, ValidationHelper.GetInteger(item, 0));
                }
            }
        }
    }

    #endregion
}