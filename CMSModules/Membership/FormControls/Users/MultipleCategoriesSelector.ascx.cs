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

public partial class CMSModules_Membership_FormControls_Users_MultipleCategoriesSelector : FormEngineUserControl
{
    /// <summary>
    /// Gets coma separated ID of selected categories.
    /// </summary>
    public override object Value
    {
        get
        {
            if (FieldInfo != null)
            {
                return FieldInfo.AllowEmpty ? null : string.Empty;
            }

            return null;
        }
        set
        {
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            categorySelector.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Loads control value
    /// </summary>
    /// <param name="value">Value to load</param>
    public override void LoadControlValue(object value)
    {
        categorySelector.ReloadData(true);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        
        CheckFieldEmptiness = false;
        if (Form != null)
        {
            Form.OnAfterDataLoad += Form_OnAfterDataLoad;
            Form.OnAfterSave += Form_OnAfterSave;
            if (Form.Data != null)
            {
                int documentId = ValidationHelper.GetInteger(Form.Data.GetValue("DocumentID"), 0);
                categorySelector.DocumentID = documentId;
                categorySelector.UserID = CMSContext.CurrentUser.UserID;
            }
        }
    }


    private void Form_OnAfterSave(object sender, EventArgs e)
    {
        // Set document ID - insert mode
        int documentId = ValidationHelper.GetInteger(Form.Data.GetValue("DocumentID"), 0);
        if (documentId > 0)
        {
            categorySelector.DocumentID = documentId;
        }

        categorySelector.Save();
    }


    private void Form_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Set document ID - edit mode
        int documentId = ValidationHelper.GetInteger(Form.Data.GetValue("DocumentID"), 0);
        if (documentId > 0)
        {
            categorySelector.DocumentID = documentId;
        }
    }


    public override bool IsValid()
    {
        var isValid = base.IsValid();

        // Check emptiness
        if ((FieldInfo != null) && !FieldInfo.AllowEmpty)
        {
            var isEmpty = string.IsNullOrEmpty(ValidationHelper.GetString(categorySelector.UniSelector.Value, ""));
            if (isEmpty)
            {
                ValidationError = ResHelper.GetString("BasicForm.ErrorEmptyValue");
            }

            isValid &= !isEmpty;
        }

        return isValid;
    }
}