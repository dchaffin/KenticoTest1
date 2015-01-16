using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.UIControls;

public partial class CMSModules_Settings_FormControls_SelectCulture : FormEngineUserControl
{
    #region "Public properties"

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
            base.Enabled = value;
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Returns ClientID of the DropDownList with culture.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.DropDownSingleSelect.ClientID;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        uniSelector.IsLiveSite = false;
        uniSelector.DisplayNameFormat = "{%CultureName%}";
        uniSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
        uniSelector.ReturnColumnName = "CultureCode";
        uniSelector.OrderBy = "CultureName";
        uniSelector.ObjectType = "cms.culture";
        uniSelector.ResourcePrefix = "cultureselector";
        uniSelector.AllowEmpty = false;
    }
}