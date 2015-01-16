using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.Newsletter;

public partial class CMSModules_Newsletters_FormControls_NewsletterSelectorSimple : FormEngineUserControl
{
    private string mValue = null;


    #region "Properties"

    /// <summary>
    /// Gets Value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return usNewsletters.ValueDisplayName;
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
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return usNewsletters.Value;
        }
        set
        {
            EnsureChildControls();
            mValue = ValidationHelper.GetString(value, String.Empty);
            try
            {
                usNewsletters.Value = mValue;
            }
            catch
            {
            }
        }
    }


    /// <summary>
    /// Gets ClientID of the dropdownlist with newsletters.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return usNewsletters.ClientID;
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


    protected void ReloadData()
    {
        usNewsletters.WhereCondition = "NewsletterSiteID = " + CMSContext.CurrentSiteID;
        usNewsletters.ButtonRemoveSelected.CssClass = "XLongButton";
        usNewsletters.ButtonAddItems.CssClass = "XLongButton";

        // Return newsletter name or newsletter ID according to type of field (if no field specified newsletter name is returned)
        if ((FieldInfo != null) && ((FieldInfo.DataType == FormFieldDataTypeEnum.Integer) || (FieldInfo.DataType == FormFieldDataTypeEnum.LongInteger)))
        {
            usNewsletters.AllowEmpty = true;
            usNewsletters.ReturnColumnName = "NewsletterID";
        }
        else
        {
            usNewsletters.ReturnColumnName = "NewsletterName";
        }

        try
        {
            usNewsletters.Value = mValue;
        }
        catch
        {
        }
    }


    /// <summary>
    /// Returns WHERE condition for selected form.
    /// </summary>
    public override string GetWhereCondition()
    {
        // Return correct WHERE condition for integer if none value is selected
        if ((FieldInfo != null) && ((FieldInfo.DataType == FormFieldDataTypeEnum.Integer) || (FieldInfo.DataType == FormFieldDataTypeEnum.LongInteger)))
        {
            int id = ValidationHelper.GetInteger(usNewsletters.Value, 0);
            if (id > 0)
            {
                return base.GetWhereCondition();
            }
        }
        return null;
    }

    #endregion
}