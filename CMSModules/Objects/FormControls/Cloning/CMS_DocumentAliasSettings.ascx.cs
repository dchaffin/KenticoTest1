using System;
using System.Data;
using System.Collections;

using CMS.FormControls;
using CMS.SettingsProvider;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DocumentEngine;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_DocumentAliasSettings : CloneSettingsControl
{
    #region "Properties"

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
    /// Hides code name.
    /// </summary>
    public override bool HideCodeName
    {
        get
        {
            return true;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            urlElem.URLPath = InfoToClone.Generalized.GetUniqueCodeName();
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[DocumentObjectType.DOCUMENTALIAS + ".aliasurlpath"] = urlElem.URLPath;
        return result;
    }

    #endregion
}