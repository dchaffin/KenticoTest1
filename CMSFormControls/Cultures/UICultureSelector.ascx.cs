using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.GlobalHelper;

public partial class CMSFormControls_Cultures_UICultureSelector : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            uniSelector.Value = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// OnLoad override - load dropdown and set current value.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        string[,] specialField = { { GetString("Administration-User_Edit.Default"), "" } };
        uniSelector.SpecialFields = specialField;
        uniSelector.AllowEmpty = false;
        uniSelector.AllowAll = false;
        uniSelector.ResourcePrefix = "UISelector";
        uniSelector.IsLiveSite = IsLiveSite;
    }
}