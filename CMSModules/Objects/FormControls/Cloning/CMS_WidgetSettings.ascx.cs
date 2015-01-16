using System;
using System.Collections;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_WidgetSettings : CloneSettingsControl
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
            string refreshLink = URLHelper.ResolveUrl("~/CMSModules/Widgets/UI/WidgetTree.aspx?widgetid={0}&reload=true");
            if (QueryHelper.Contains("reloadall"))
            {
                script = "wopener.location = '" + refreshLink + "';";
            }
            else
            {
                script += "wopener.parent.parent.frames['widgettree'].location.href ='" + refreshLink + "';";
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

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblWidgetCategory.ToolTip = GetString("clonning.settings.widget.category.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            WidgetInfo wi = InfoToClone as WidgetInfo;
            if (wi != null)
            {
                categorySelector.Value = wi.WidgetCategoryID.ToString();
            }
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[PortalObjectType.WIDGET + ".categoryid"] = categorySelector.Value;
        return result;
    }

    #endregion
}