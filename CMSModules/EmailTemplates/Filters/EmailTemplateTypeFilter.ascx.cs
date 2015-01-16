using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.Controls;

public partial class CMSModules_EmailTemplates_Filters_EmailTemplateTypeFilter : CMSAbstractBaseFilterControl
{
    #region "Public properties"

    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            if (!string.IsNullOrEmpty(drpEmailType.SelectedValue))
            {
                base.WhereCondition = "EmailTemplateType = '" + SqlHelperClass.GetSafeQueryString(drpEmailType.SelectedValue) + "'";
            }
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpEmailType.SelectedValue;
        }
        set
        {
            drpEmailType.SelectedValue = ValidationHelper.GetString(value, string.Empty);
        }
    }


    /// <summary>
    /// Drop down control
    /// </summary>
    public DropDownList DropDown
    {
        get
        {
            return drpEmailType;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Fill the e-mail type enumeration
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        if (drpEmailType.Items.Count == 0)
        {
            drpEmailType.Items.Add(new ListItem(ResHelper.GetString("general.selectall"), string.Empty));
            DataHelper.FillWithEnum<EmailTemplateMacros.EmailType>(drpEmailType, "emailtemplate.type.", EmailTemplateMacros.GetEmailTypeString, true);
        }
    }


    /// <summary>
    /// Reset filter.
    /// </summary>
    public override void ResetFilter()
    {
        drpEmailType.SelectedIndex = 0;
    }

    #endregion
}