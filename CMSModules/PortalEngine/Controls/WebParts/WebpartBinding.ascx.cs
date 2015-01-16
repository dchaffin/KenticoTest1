using System;
using System.Data;
using System.Collections;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.DocumentEngine;

public partial class CMSModules_PortalEngine_Controls_WebParts_WebpartBinding : CMSUserControl
{
    #region "Variables"

    protected string mAliasPath = null;
    protected int mPageTemplateId = 0;
    protected string mZoneId = null;
    protected Guid mInstanceGUID = Guid.Empty;
    protected string mWebpartId = null;
    private string mCultureCode = null;

    /// <summary>
    /// Current page info.
    /// </summary>
    private PageInfo pi = null;

    /// <summary>
    /// Page template info.
    /// </summary>
    private PageTemplateInfo pti = null;


    /// <summary>
    /// Current bindings.
    /// </summary>
    private Hashtable bindings = null;

    /// <summary>
    /// Current web part.
    /// </summary>
    private WebPartInstance webPart = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Page alias path.
    /// </summary>
    public string AliasPath
    {
        get
        {
            return mAliasPath;
        }
        set
        {
            mAliasPath = value;
        }
    }


    /// <summary>
    /// Preferred culture code to use along with alias path.
    /// </summary>
    public string CultureCode
    {
        get
        {
            if (string.IsNullOrEmpty(mCultureCode))
            {
                mCultureCode = CMSContext.PreferredCultureCode;
            }
            return mCultureCode;
        }
        set
        {
            mCultureCode = value;
        }
    }


    /// <summary>
    /// Page template ID.
    /// </summary>
    public int PageTemplateId
    {
        get
        {
            return mPageTemplateId;
        }
        set
        {
            mPageTemplateId = value;
        }
    }


    /// <summary>
    /// Zone ID.
    /// </summary>
    public string ZoneId
    {
        get
        {
            return mZoneId;
        }
        set
        {
            mZoneId = value;
        }
    }


    /// <summary>
    /// Web part ID.
    /// </summary>
    public string WebpartId
    {
        get
        {
            return mWebpartId;
        }
        set
        {
            mWebpartId = value;
        }
    }


    /// <summary>
    /// Instance GUID.
    /// </summary>
    public Guid InstanceGUID
    {
        get
        {
            return mInstanceGUID;
        }
        set
        {
            mInstanceGUID = value;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        gvBinding.RowDataBound += gvBinding_RowDataBound;

        base.OnInit(e);

        gvBinding.Columns[0].HeaderText = GetString("general.action");
        gvBinding.Columns[1].HeaderText = GetString("WebPartBinding.HeaderLocalProperty");
        gvBinding.Columns[2].HeaderText = GetString("WebPartBinding.HeaderSourceProperty");
        gvBinding.GridLines = GridLines.Horizontal;

        BindData();
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterWOpenerScript(Page);
        // delete confirmation
        ltlScript.Text = ScriptHelper.GetScript("var deleteConfirmation = '" + GetString("WebPartBinding.DeleteConfirmation") + "';");

        btnOk.Click += btnOK_Click;
        gvBinding.RowDeleting += gvBinding_RowDeleting;

        btnOnOK.Click += btnOnOK_Click;
        btnOnApply.Click += btnOnApply_Click;

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ApplyButton", ScriptHelper.GetScript("function OnApplyButton(){" + Page.ClientScript.GetPostBackEventReference(btnOnApply, "") + "}"));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "OKButton", ScriptHelper.GetScript("function OnOKButton(){" + Page.ClientScript.GetPostBackEventReference(btnOnOK, "") + "}"));

        lblCaption.Text = GetString("WebPartBinding.Caption");
        lblProperty.Text = GetString("WebPartBinding.lblProperty");
        lblSourceId.Text = GetString("WebPartBinding.lblSourceId");
        lblSourceProprety.Text = GetString("WebPartBinding.lblSourceProperty");
        rfvSourceId.ErrorMessage = GetString("WebPartBinding.rfvSourceId");
        rfvSourceProperty.ErrorMessage = GetString("WebPartBinding.rfvSourceProperty");
    }


    private void gvBinding_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        ImageButton btn = (ImageButton)e.Row.FindControl("lnkDelete");
        if (btn != null)
        {
            btn.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Delete.png");
        }
    }


    private void gvBinding_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        BindData();
    }


    private void btnOK_Click(object sender, EventArgs e)
    {
        WebPartBindingInfo bi = new WebPartBindingInfo();
        bi.SourceProperty = txtSourceProperty.Text;
        bi.SourceWebPart = txtSourceId.Text;
        bi.TargetProperty = drpProperty.SelectedValue;
        webPart.Bindings[drpProperty.SelectedValue.ToLowerCSafe()] = bi;
        Save();
        BindData();
    }


    private void btnOnApply_Click(object sender, EventArgs e)
    {
        Save();
        URLHelper.Redirect(URLHelper.Url.AbsoluteUri);
    }


    private void btnOnOK_Click(object sender, EventArgs e)
    {
        Save();
        // Close the window
        ltlScript.Text = ScriptHelper.GetScript("CloseDialog();");
    }


    /// <summary>
    /// Saves webpart properties.
    /// </summary>
    public void Save()
    {
        // Update page template
        PageTemplateInfoProvider.SetPageTemplateInfo(pti);
        txtSourceId.Text = "";
        txtSourceProperty.Text = "";
        drpProperty.SelectedIndex = 0;
    }


    public void BindData()
    {
        if (WebpartId != "")
        {
            // Get page info
            pi = CMSWebPartPropertiesPage.GetPageInfo(AliasPath, PageTemplateId, CultureCode);
            if (pi == null)
            {
                Visible = false;
                return;
            }

            // Get page template info
            pti = pi.UsedPageTemplateInfo;
            if (pti != null)
            {
                // Get web part instance
                webPart = pti.GetWebPart(InstanceGUID, WebpartId);
                if (webPart == null)
                {
                    CMSPage.EditedObject = null;
                    return;
                }

                // Get the web part info
                WebPartInfo wpi = WebPartInfoProvider.GetBaseWebPart(webPart.WebPartType);
                if (wpi == null)
                {
                    return;
                }

                // Get webpart properties (XML)
                string wpProperties = wpi.WebPartProperties;
                FormInfo fi = PortalFormHelper.GetWebPartFormInfo(wpi.WebPartName + FormHelper.CORE, wpi.WebPartProperties, null, null, false);

                // Get datarow with required columns
                DataRow dr = fi.GetDataRow();

                // Bind drop down list
                if (!RequestHelper.IsPostBack())
                {
                    DataTable dropTable = new DataTable();
                    dropTable.Columns.Add("name");

                    foreach (DataColumn column in dr.Table.Columns)
                    {
                        dropTable.Rows.Add(column.ColumnName);
                    }

                    dropTable.DefaultView.Sort = "name";
                    drpProperty.DataTextField = "name";
                    drpProperty.DataValueField = "name";
                    drpProperty.DataSource = dropTable.DefaultView;
                    drpProperty.DataBind();
                }

                // Bind grid view
                DataTable table = new DataTable();
                table.Columns.Add("LocalProperty");
                table.Columns.Add("SourceProperty");
                bindings = webPart.Bindings;

                foreach (DataColumn column in dr.Table.Columns)
                {
                    string propertyName = column.ColumnName.ToLowerCSafe();
                    if (bindings.ContainsKey(propertyName))
                    {
                        WebPartBindingInfo bi = (WebPartBindingInfo)bindings[propertyName];
                        table.Rows.Add(column.ColumnName, bi.SourceWebPart + "." + bi.SourceProperty);
                    }
                }

                gvBinding.DataSource = table;
                gvBinding.DataBind();
            }
        }
    }


    /// <summary>
    /// Delete selected item.
    /// </summary>
    protected void lnkDelete_OnCommand(object sender, CommandEventArgs e)
    {
        string propertyName = e.CommandArgument.ToString().ToLowerCSafe();
        if (bindings.ContainsKey(propertyName))
        {
            bindings.Remove(propertyName);
            Save();
        }
    }
}