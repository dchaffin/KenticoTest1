using System;
using System.Collections;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DataEngine;
using CMS.UIControls;
using CMS.IO;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_DocumentTypeSettings : CloneSettingsControl
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

    protected void Page_Load(object sender, EventArgs e)
    {
        lblTableName.ToolTip = GetString("clonning.settings.class.tablename.tooltip");
        lblIcons.ToolTip = GetString("clonning.settings.documenttype.icons.tooltip");
        lblCloneAlternativeForms.ToolTip = GetString("clonning.settings.class.alternativeform");

        if (!RequestHelper.IsPostBack())
        {
            TableManager tm = new TableManager(null);

            string originalTableName = InfoToClone.GetStringValue("ClassTableName", "");
            if (string.IsNullOrEmpty(originalTableName))
            {
                plcTableName.Visible = false;
                plcAlternativeForms.Visible = false;
            }
            else
            {
                txtTableName.Text = tm.GetUniqueTableName(InfoToClone.GetStringValue("ClassTableName", ""));
            }
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

        if (chkIcons.Checked)
        {
            // Set the icon names to copy
            string iconPath = Server.MapPath(GetImagePath("/DocumentTypeIcons/"));
            string fileName = TranslationHelper.GetSafeClassName(((DataClassInfo)InfoToClone).ClassName);
            string smallIcon = iconPath + fileName;
            string largeIcon = iconPath + "48x48\\" + fileName;

            result[SettingsObjectType.DOCUMENTTYPE + ".icons"] = smallIcon + ".png;" + smallIcon + ".gif;" + largeIcon + ".png;" + largeIcon + ".gif";
        }
        return result;
    }

    #endregion
}