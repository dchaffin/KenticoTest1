using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

public partial class CMSModules_Workflows_Controls_UI_WorkflowAction_List : CMSAdminListControl
{
        
    #region "Properties"

    /// <summary>
    /// Only actions with specified allowed objects will be listed.
    /// </summary>
    public string AllowedObjects
    {
        get;
        set;
    }


    /// <summary>
    /// Current object type.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return gridElem.ObjectType;
        }
        set
        {
            gridElem.ObjectType = value;
        }
    }


    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
    }


    private void gridElem_OnBeforeDataReload()
    {
        // Hide allowed objects column when not in development mode
        gridElem.NamedColumns["scope"].Visible = SettingsKeyProvider.DevelopmentMode;
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "enabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter, true);

            case "scope":
                string types = ValidationHelper.GetString(parameter, "");
                var typeList = TypeHelper.GetTypes(types);
                if (typeList.Count == 0)
                {
                    return GetString("general.selectall");
                }
                string list = "";
                typeList.ForEach(s => list += ResHelper.GetString(ResHelper.GetAPIString("ObjectTasks." + s.Replace(".", "_").Replace("#", "_"), s)) + ", ");
                return list.Trim().TrimEnd(',');
        }

        return parameter;
    }

    #endregion
}