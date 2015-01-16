using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.EmailEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_EmailTemplates_Controls_EmailTemplateList : CMSAdminListControl
{
    #region "Variables"

    private int mSiteId = 0;
    private int mGlobalRecordValue = UniSelector.US_GLOBAL_RECORD;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets global record value (value for global item selected in site selector).
    /// </summary>
    public int GlobalRecordValue
    {
        get
        {
            return mGlobalRecordValue;
        }
        set
        {
            mGlobalRecordValue = value;
            gridElem.WhereCondition = CreateWhereCondition();
        }
    }


    /// <summary>
    /// Gets or sets the site ID for which the e-mail templates should be displayed.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
            gridElem.WhereCondition = CreateWhereCondition();
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        if (StopProcessing)
        {
            return;
        }

        // Unigrid initialization
        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.WhereCondition = CreateWhereCondition();
    }


    /// <summary>
    /// Handles the UniGrid's external data bound event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>
    object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "type":
                string type = ValidationHelper.GetString(parameter, string.Empty);
                if (string.IsNullOrEmpty(type))
                {
                    type = EmailTemplateMacros.GetEmailTypeString(EmailTemplateMacros.EmailType.General);
                }

                return HTMLHelper.HTMLEncode(ResHelper.GetString("emailtemplate.type." + type));
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                SelectedItemID = ValidationHelper.GetInteger(actionArgument, 0);
                RaiseOnEdit();
                break;

            case "delete":
                if (!CheckPermissions("CMS.EmailTemplates", PERMISSION_MODIFY))
                {
                    return;
                }

                // Get TemplateID
                int templateId = ValidationHelper.GetInteger(actionArgument, 0);
                EmailTemplateProvider.DeleteEmailTemplate(templateId);
                break;
        }

        RaiseOnAction(actionName, actionArgument);
    }


    /// <summary>
    /// Creates where condition for unigrid according to the parameters.
    /// </summary>
    private string CreateWhereCondition()
    {
        string where = string.Empty;

        if (mSiteId > 0)
        {
            where += "(EmailTemplateSiteID = " + mSiteId + ")";
        }
        else
            // Global selected
            if ((mSiteId == GlobalRecordValue) && CMSContext.CurrentUser.IsGlobalAdministrator)
            {
                where += "(EmailTemplateSiteID is NULL)";
            }
            else
            {
                where += "(EmailTemplateSiteID =" + CMSContext.CurrentSiteID + ")";
            }

        return where;
    }

    #endregion
}