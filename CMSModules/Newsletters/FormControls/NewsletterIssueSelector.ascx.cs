using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.FormEngine;
using CMS.SettingsProvider;

public partial class CMSModules_Newsletters_FormControls_NewsletterIssueSelector : FormEngineUserControl
{
    #region "Variables"

    private int mValue = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Enables or disables showing variants in uniselector.
    /// </summary>
    public bool ShowVariants
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowVariants"), true);
        }
        set
        {
            SetValue("ShowVariants", value);
        }
    }


    /// <summary>
    /// Gets or Sets return column name. Default is IssueID.
    /// </summary>
    public string ReturnColumnName
    {
        get
        {
            string returnColumnName = ValidationHelper.GetString(GetValue("ReturnColumnName"), "");

            if (String.IsNullOrEmpty(returnColumnName))
            {
                returnColumnName = "IssueID";
            }

            return returnColumnName;
        }
        set
        {
            SetValue("ReturnColumnName", value);
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            EnsureChildControls();
            base.Enabled = value;
            usNewsletters.Enabled = value;
            usIssues.Enabled = value;
        }
    }


    /// <summary>
    /// Gets value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return usIssues.ValueDisplayName;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (FieldInfo.DataType == CMS.FormEngine.FormFieldDataTypeEnum.GUID)
            {
                return usIssues.Value;
            }

            return (ValidationHelper.GetInteger(usIssues.Value, 0) == 0? null : usIssues.Value);
        }
        set
        {
            mValue = ValidationHelper.GetInteger(value, 0);
        }
    }

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        if (usNewsletters == null)
        {
            pnlUpdate.LoadContainer();
        }
        base.CreateChildControls();
        if (!StopProcessing)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads dropdown lists.
    /// </summary>
    protected void ReloadData()
    {

        usNewsletters.WhereCondition = "NewsletterSiteID = " + CMSContext.CurrentSiteID;
        usNewsletters.ButtonRemoveSelected.CssClass = "XLongButton";
        usNewsletters.ButtonAddItems.CssClass = "XLongButton";
        usNewsletters.ReturnColumnName = "NewsletterID";
        usNewsletters.DropDownSingleSelect.SelectedIndexChanged += new EventHandler(DropDownSingleSelect_SelectedIndexChanged);
        usNewsletters.DropDownSingleSelect.AutoPostBack = true;

        usIssues.WhereCondition = GetIssueWhereCondition(usNewsletters.Value);;
        usIssues.ButtonRemoveSelected.CssClass = "XLongButton";
        usIssues.ButtonAddItems.CssClass = "XLongButton";
        usIssues.ReturnColumnName = ReturnColumnName;
        
        // Initialize both dropdown lists according to incoming issue ID
        if (!RequestHelper.IsPostBack())
        {
            if (mValue > 0)
            {
                // Retrieve newsletter ID from issue info
                IssueInfo issue = IssueInfoProvider.GetIssueInfo(mValue);
                int issueNewsletterID = 0;
                if (issue != null)
                {
                    issueNewsletterID = issue.IssueNewsletterID;
                }
                usNewsletters.Value = issueNewsletterID;
                usIssues.WhereCondition = GetIssueWhereCondition(issueNewsletterID);;
                usIssues.Reload(true);
                usIssues.DropDownSingleSelect.SelectedValue = mValue.ToString();
                usIssues.Value = mValue;
            }
        }
    }

    protected void DropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Reload issues dropdown list
        usIssues.WhereCondition = GetIssueWhereCondition(usNewsletters.Value);
        usIssues.Reload(true);
        mValue = 0;
    }


    /// <summary>
    /// Returns WHERE condition for issue dropdown list
    /// </summary>
    /// <param name="value">Newsletter ID</param>
    private string GetIssueWhereCondition(object value)
    {
        string where = "IssueNewsletterID=" + ValidationHelper.GetInteger(value, 0);

        // Filter variants
        if (!ShowVariants)
        {
            where = SqlHelperClass.AddWhereCondition(where, "(IssueVariantOfIssueID <= 0 OR IssueVariantOfIssueID IS NULL)");
        }

        return where;
    }

    #endregion
}