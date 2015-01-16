using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;

public partial class CMSModules_PortalEngine_FormControls_PageTemplates_SelectPageTemplate : FormEngineUserControl
{
    #region "Variables"

    private bool mShowOnlySiteTemplates = true;

    private PageTemplateInfo pti = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Enables or disables the control.
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
            btnClear.Enabled = value;
            txtTemplate.Enabled = value;
            btnSelect.Enabled = value;
        }
    }


    /// <summary>
    /// Selected page template info object
    /// </summary>
    public PageTemplateInfo PageTemplateInfo
    {
        get
        {
            int templateId = ValidationHelper.GetInteger(hdnSelected.Value, 0);
            
            if ((pti != null) && (pti.PageTemplateId != templateId))
            {
                pti = null;
            }
            if (pti == null)
            {
                // Load by template ID in the hidden field
                pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);
            }
            else if (templateId <= 0)
            {
                // Clear the page template info in case no value is selected
                pti = null;
            }

            return pti;
        }
    }


    /// <summary>
    /// Gets or sets page template ID.
    /// </summary>
    public int PageTemplateID
    {
        get
        {
            // Get the page template code name
            PageTemplateInfo pti = PageTemplateInfo;
            if (pti == null)
            {
                return 0;
            }
            else
            {
                return pti.PageTemplateId;
            }
        }
        set
        {
            // Get the page template
            pti = PageTemplateInfoProvider.GetPageTemplateInfo(value);

            // Update the selected ID
            UpdateSelectedID();
        }
    }


    /// <summary>
    /// Gets or sets field value (Page template code name).
    /// </summary>
    public override object Value
    {
        get
        {
            // Get the page template code name
            PageTemplateInfo pti = PageTemplateInfo;
            if (pti == null)
            {
                return null;
            }
            else
            {
                return pti.CodeName;
            }
        }
        set
        {
            if (!RequestHelper.IsPostBack() || (this.Form == null))
            {
                // Get the page template
                pti = PageTemplateInfoProvider.GetPageTemplateInfo((string)value);

                // Update the selected ID
                UpdateSelectedID();
            }
        }
    }


    /// <summary>
    /// Updates the selected ID
    /// </summary>
    private void UpdateSelectedID()
    {
        if (pti != null)
        {
            hdnSelected.Value = pti.PageTemplateId.ToString();
        }
        else
        {
            hdnSelected.Value = "";
        }
    }


    /// <summary>
    /// Gets ClientID of the textbox with template.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtTemplate.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show site page templates only.
    /// </summary>
    public bool ShowOnlySiteTemplates
    {
        get
        {
            return mShowOnlySiteTemplates;
        }
        set
        {
            mShowOnlySiteTemplates = value;
        }
    }


    /// <summary>
    /// Root page template category ID
    /// </summary>
    public int RootCategoryID
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load event handler
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        WindowHelper.Add("ShowOnlySiteTemplates", ShowOnlySiteTemplates);

        btnSelect.OnClientClick = "return PTS_Select('" + ClientID + "', '" + RootCategoryID + "');";
        btnClear.OnClientClick = "return PTS_Clear('" + ClientID + "');";

        btnSelect.Text = GetString("general.select");
        btnClear.Text = GetString("general.clear");

        if (RequestHelper.IsPostBack() && (hdnSelected.Value == ""))
        {
            hdnSelected.Value = Request.Form[hdnSelected.UniqueID];
        }
    }


    /// <summary>
    /// Select button click
    /// </summary>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        pti = null;
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        LoadData();

        base.OnPreRender(e);

        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "PageTemplateSelector", ScriptHelper.GetScript(
@"
function PTS_Select(selectorId, rootCategoryId) {
    var hid = document.getElementById(selectorId + '_hdnSelected');
    var url = '" + ResolveUrl("~/CMSModules/PortalEngine/UI/Layout/PageTemplateSelector.aspx") + @"?selectorid=' + selectorId + '&selectedPageTemplateId=' + hid.value + '&rootcategoryid=' + rootCategoryId;
    modalDialog(url, 'PageTemplateSelection', '90%', '85%'); 
    return false;
} 

function OnSelectPageTemplate(templateId, selectorId) { 
    var hid = document.getElementById(selectorId + '_hdnSelected');
    hid.value = templateId; 
    " + Page.ClientScript.GetPostBackEventReference(btnSelect, null) + @"
}
function PTS_Clear(selectorId) { 
    document.getElementById(selectorId + '_txtTemplate').value = ''; 
    document.getElementById(selectorId + '_hdnSelected').value = 0; return false;
}
"
        ));
    }


    /// <summary>
    /// Loads the control data.
    /// </summary>
    public void LoadData()
    {
        PageTemplateInfo pti = PageTemplateInfo;
        if (pti != null)
        {
            txtTemplate.Text = ResHelper.LocalizeString(pti.DisplayName);
        }
        else
        {
            txtTemplate.Text = "";
        }
    }

    #endregion
}