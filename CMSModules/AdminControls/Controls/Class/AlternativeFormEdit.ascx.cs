using System;

using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.GlobalHelper;

public partial class CMSModules_AdminControls_Controls_Class_AlternativeFormEdit : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// OnBeforeSave event.
    /// </summary>
    public event EventHandler OnBeforeSave
    {
        add 
        {
            form.OnBeforeSave += value;
        }
        remove 
        {
            form.OnBeforeSave -= value; 
        }
    }


    /// <summary>
    /// OnAfterSave event.
    /// </summary>
    public event EventHandler OnAfterSave
    {
        add
        {
            form.OnAfterSave += value;
        }
        remove
        {
            form.OnAfterSave -= value;
        }
    }


    /// <summary>
    /// The URL to which the engine should redirect after creation of the new object.
    /// </summary>
    public string RedirectUrlAfterCreate
    {
        get
        {
            return form.RedirectUrlAfterCreate;
        }
        set
        {
            form.RedirectUrlAfterCreate = value;
        }
    }


    /// <summary>
    /// Edited object.
    /// </summary>
    public BaseInfo EditedObject
    {
        get
        {
            return form.EditedObject;
        }
        set
        {
            form.EditedObject = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        OnAfterSave += (s, ea) => ScriptHelper.RefreshTabHeader(Page, null);
    }

    #endregion
}