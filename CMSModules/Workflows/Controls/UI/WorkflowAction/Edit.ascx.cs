using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.SettingsProvider;
using CMS.ExtendedControls;

public partial class CMSModules_Workflows_Controls_UI_WorkflowAction_Edit : CMSAdminEditControl
{
    #region "Properties"

    /// <summary>
    /// Indicates whether allowed objects field should be hidden.
    /// </summary>
    public bool HideAllowedObjects
    {
        get;
        set;
    }


    /// <summary>
    /// Forced allowed objects for action.
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
            return formElem.ObjectType;
        }
        set
        {
            formElem.ObjectType = value;
        }
    }


    /// <summary>
    /// Edited workflow action.
    /// </summary>
    public WorkflowActionInfo CurrentAction
    {
        get
        {
            return (WorkflowActionInfo)formElem.EditedObject;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ucFile.ObjectID = CurrentAction.ActionID;
        ucFile.ObjectType = ObjectType;
        ucFile.Category = CMSConstants.OBJECT_CATEGORY_THUMBNAIL;

        ucIcon.ObjectID = CurrentAction.ActionID;
        ucIcon.ObjectType = ObjectType;
        ucIcon.Category = CMSConstants.OBJECT_CATEGORY_ICON;

        pnlImages.Visible = (formElem.Mode != FormModeEnum.Insert);
        
        if (!String.IsNullOrEmpty(AllowedObjects))
        {
            // Ensure correct allowed objects format
            CurrentAction.ActionAllowedObjects = ";" + AllowedObjects.Trim(';') + ";";
        }

        if (HideAllowedObjects)
        {
            aAlowedObjects.Visible = false;
        }
        else if (!RequestHelper.IsPostBack())
        {
            objectTypeSelector.ObjectType = CurrentAction.ActionAllowedObjects;
        }

    }

    #endregion
}

