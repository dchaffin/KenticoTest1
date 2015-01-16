using System;
using System.Collections;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DataEngine;
using CMS.UIControls;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_ClassSettings : CloneSettingsControl
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
    /// Excluded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return PredefinedObjectType.ALTERNATIVEFORM;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true if custom settings are valid against given clone setting.
    /// </summary>
    /// <param name="settings">Clone settings</param>
    public override bool IsValid(CloneSettings settings)
    {
        if (!ValidationHelper.IsIdentifier(txtTableName.Text))
        {
            ShowError(GetString("BizForm_Edit.ErrorFormTableNameInIdentifierFormat"));
            return false;
        }
        return true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        lblTableName.ToolTip = GetString("clonning.settings.class.tablename.tooltip");
        lblCloneAlternativeForms.ToolTip = GetString("clonning.settings.class.alternativeform");

        if (!RequestHelper.IsPostBack())
        {
            TableManager tm = new TableManager(null);

            txtTableName.Text = tm.GetUniqueTableName(InfoToClone.GetStringValue("ClassTableName", ""));
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[SettingsObjectType.CLASS + ".data"] = false;
        result[SettingsObjectType.CLASS + ".tablename"] = txtTableName.Text;
        result[SettingsObjectType.CLASS + ".alternativeforms"] = chkCloneAlternativeForms.Checked;
        return result;
    }

    #endregion
}