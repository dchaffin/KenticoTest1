using System;
using System.Collections;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_PageTemplateSettings : CloneSettingsControl
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
            string refreshLink = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/PageTemplate_Tree.aspx?templateid={0}&reload=true");
            if (QueryHelper.Contains("reloadall"))
            {
                script = "wopener.location = '" + refreshLink + "';";
            }
            else
            {
                script += "wopener.parent.parent.frames['pt_tree'].location.href ='" + refreshLink + "';";
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
            return PredefinedObjectType.MVTVARIANT + ";" + PredefinedObjectType.MVTCOMBINATION;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblFiles.ToolTip = GetString("clonning.settings.layouts.appthemesfolder.tooltip");
        lblTemplateScope.ToolTip = GetString("clonning.settings.pagetemplate.templatescope.tooltip");
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[PortalObjectType.PAGETEMPLATE + ".appthemes"] = chkFiles.Checked;
        result[PortalObjectType.PAGETEMPLATE + ".templatecope"] = chkTemplateScope.Checked;
        return result;
    }

    #endregion
}