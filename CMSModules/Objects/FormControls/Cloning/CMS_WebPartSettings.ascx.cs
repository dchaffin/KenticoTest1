using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.IO;
using CMS.UIControls;
using CMS.CMSHelper;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_WebPartSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Returns close script which should be run when cloning is sucessfully done.
    /// </summary>
    public override string CloseScript
    {
        get
        {
            string script = String.Empty;
            string refreshLink = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/Development/WebPart_Tree.aspx?webpartid={0}&reload=true");
            if (QueryHelper.Contains("reloadall"))
            {
                script = "wopener.location = '" + refreshLink + "';";
            }
            else
            {
                script += "wopener.parent.parent.frames['webparttree'].location.href ='" + refreshLink + "';";
            }
            script += "CloseDialog();";
            return script;
        }
    }


    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Excluded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return PortalObjectType.WEBPARTLAYOUT;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblFiles.ToolTip = GetString("clonning.settings.webpart.files.tooltip");
        lblFileName.ToolTip = GetString("clonning.settings.webpart.filename.tooltip");
        lblWebPartCategory.ToolTip = GetString("clonning.settings.webpart.category.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            FillDropDownList(0, 0);

            WebPartInfo wpi = InfoToClone as WebPartInfo;
            if (wpi != null)
            {
                if (wpi.WebPartParentID > 0)
                {
                    WebPartInfo wparent = WebPartInfoProvider.GetWebPartInfo(wpi.WebPartParentID);
                    if (wparent != null)
                    {
                        // Hide files box for inherited webparts
                        plcFiles.Visible = false;
                    }
                }

                if (plcFiles.Visible)
                {
                    string absolutePath = WebPartInfoProvider.WebPartsDirectory + "/" + wpi.WebPartFileName.TrimStart('/');
                    txtFileName.Text = FileHelper.GetUniqueFileName(absolutePath).Substring(WebPartInfoProvider.WebPartsDirectory.Length).TrimStart('/');
                }

                drpWebPartCategories.SelectedValue = wpi.WebPartCategoryID.ToString();

                if (CMSApplicationModule.IsPrecompiledWebsite)
                {
                    txtFileName.Text = wpi.WebPartFileName;
                    chkFiles.Checked = false;

                    txtFileName.Enabled = chkFiles.Enabled = false;
                    txtFileName.ToolTip = chkFiles.ToolTip = GetString("general.copyfiles.precompiled");
                }
            }
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[PortalObjectType.WEBPART + ".categoryid"] = drpWebPartCategories.SelectedValue;
        result[PortalObjectType.WEBPART + ".filename"] = txtFileName.Text;
        result[PortalObjectType.WEBPART + ".files"] = chkFiles.Checked;
        result[PortalObjectType.WEBPART + ".appthemes"] = chkAppThemes.Checked;
        result[PortalObjectType.WEBPART + ".layouts"] = chkWebpartLayouts.Checked;
        return result;
    }


    /// <summary>
    /// Fills existing category names in drop down list, recursive.
    /// </summary>
    /// <param name="shift">Sub-category offset in drop down list</param>
    /// <param name="parentCategoryID">ID of parent category</param>
    protected void FillDropDownList(int shift, int parentCategoryID)
    {
        if (parentCategoryID == 0)
        {
            shift = 0;
        }
        else
        {
            shift++;
        }

        DataSet ds = WebPartCategoryInfoProvider.GetCategories(parentCategoryID);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ListItem category = new ListItem();
                category.Text = string.Empty;
                for (int i = 0; i < shift; i++)
                {
                    category.Text += "\xA0\xA0\xA0";
                }

                category.Text += ResHelper.LocalizeString(ValidationHelper.GetString(dr["CategoryDisplayName"], ""));
                category.Value = ValidationHelper.GetString(dr["CategoryID"], "");

                if (ValidationHelper.GetInteger(dr["CategoryLevel"], 0) == 0)
                {
                    category.Enabled = false;
                }

                drpWebPartCategories.Items.Add(category);

                FillDropDownList(shift, ValidationHelper.GetInteger(dr[0], 0));
            }
        }
    }

    #endregion
}