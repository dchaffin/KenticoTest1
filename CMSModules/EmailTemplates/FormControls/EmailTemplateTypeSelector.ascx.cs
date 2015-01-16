using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_EmailTemplates_FormControls_EmailTemplateTypeSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mAllowAll = true;
    private bool mAllowEmpty = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return drpEmailType.SelectedValue;
        }
        set
        {
            EnsureChildControls();
            drpEmailType.SelectedValue = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// Specifies, whether the selector allows all selection.
    /// </summary>
    public virtual bool AllowAll
    {
        get
        {
            return mAllowAll;
        }
        set
        {
            mAllowAll = value;
        }
    }


    /// <summary>
    /// Specifies, whether the selector allows empty selection.
    /// </summary>
    public virtual bool AllowEmpty
    {
        get
        {
            return mAllowEmpty;
        }
        set
        {
            mAllowEmpty = value;
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

    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        // Fill the e-mail type enumeration
        ReloadData();
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        if (drpEmailType.Items.Count == 0)
        {
            if (AllowEmpty)
            {
                drpEmailType.Items.Add(new ListItem(ResHelper.GetString("general.selectnone"), ""));
            }
            if (AllowAll)
            {
                drpEmailType.Items.Add(new ListItem(ResHelper.GetString("general.selectall"), ""));
            }
            DataHelper.FillWithEnum<EmailTemplateMacros.EmailType>(drpEmailType, "emailtemplate.type.", EmailTemplateMacros.GetEmailTypeString, true);
        }
    }

    #endregion
}