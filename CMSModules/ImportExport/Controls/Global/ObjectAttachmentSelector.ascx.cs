using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ImportExport_Controls_Global_ObjectAttachmentSelector : CMSUserControl
{
    protected int mPreviewWidth = 140;
    protected int mPreviewHeight = 108;
    protected int mPanelHeight = 250;

    protected string mObjectType = SiteObjectType.WEBTEMPLATE;


    /// <summary>
    /// Preview width.
    /// </summary>
    public int PreviewWidth
    {
        get
        {
            return mPreviewWidth;
        }
        set
        {
            mPreviewWidth = value;
        }
    }


    /// <summary>
    /// Preview height.
    /// </summary>
    public int PreviewHeight
    {
        get
        {
            return mPreviewHeight;
        }
        set
        {
            mPreviewHeight = value;
        }
    }


    /// <summary>
    /// Panel height.
    /// </summary>
    public int PanelHeight
    {
        get
        {
            return mPanelHeight;
        }
        set
        {
            mPanelHeight = value;
        }
    }


    /// <summary>
    /// ID column name.
    /// </summary>
    public string IDColumn
    {
        get;
        set;
    }


    /// <summary>
    /// Column containing the thumbnail metafile GUID
    /// </summary>
    public string ThumbnailGUIDColumn 
    { 
        get; 
        set; 
    }


    /// <summary>
    /// Display name column name.
    /// </summary>
    public string DisplayNameColumn
    {
        get;
        set;
    }


    /// <summary>
    /// Description column name.
    /// </summary>
    public string DescriptionColumn
    {
        get;
        set;
    }


    /// <summary>
    /// Template ID.
    /// </summary>
    public int SelectedId
    {
        get
        {
            return ValidationHelper.GetInteger(hdnLastSelected.Value, 0);
        }
        set
        {
            hdnLastSelected.Value = value.ToString();
        }
    }


    /// <summary>
    /// Data source.
    /// </summary>
    public object DataSource
    {
        get
        {
            return rptItems.DataSource;
        }
        set
        {
            rptItems.DataSource = value;
        }
    }


    /// <summary>
    /// Object type.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return mObjectType;
        }
        set
        {
            mObjectType = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ltlScript.Text = ScriptHelper.GetScript("var hdnLastSelected=document.getElementById('" + hdnLastSelected.ClientID + "');");
        ltlScriptAfter.Text = ScriptHelper.GetScript("SelectItem(hdnLastSelected.value);");
    }


    /// <summary>
    /// Bind data.
    /// </summary>
    public void BindData()
    {
        rptItems.DataBind();
    }


    /// <summary>
    /// Gets metafile preview.
    /// </summary>
    /// <param name="thumbnailGuid">Thumbnail GUID</param>
    protected string GetPreviewImage(object thumbnailGuid)
    {
        Guid guid = ValidationHelper.GetGuid(thumbnailGuid, Guid.Empty);

        if (guid == Guid.Empty)
        {
            return ResolveUrl("~/CMSPages/GetMetaFile.aspx?fileguid=" + guid);
        }
        else
        {
            return ResolveUrl("~/CMSPages/GetMetaFile.aspx?fileguid=" + guid);
        }
    }
}