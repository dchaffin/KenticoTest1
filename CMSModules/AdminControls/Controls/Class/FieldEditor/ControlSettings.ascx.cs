using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_ControlSettings : CMSUserControl
{
    #region "Variables"

    private FormInfo fi = null;
    private static Hashtable mSettings = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// FormInfo for specific control.
    /// </summary>
    public FormInfo FormInfo
    {
        get
        {
            return fi;
        }
        set
        {
            form.FormInformation = fi = value;
        }
    }


    /// <summary>
    /// Shows in what control is this form used.
    /// </summary>
    public FormTypeEnum FormType
    {
        get
        {
            return form.FormType;
        }
        set
        {
            form.FormType = value;
        }
    }


    /// <summary>
    /// Field settings hashtable.
    /// </summary>
    public Hashtable Settings
    {
        get
        {
            return mSettings;
        }
        set
        {
            mSettings = new Hashtable(value, StringComparer.InvariantCultureIgnoreCase);
        }
    }


    /// <summary>
    /// Basic form data.
    /// </summary>
    public DataRow FormData
    {
        get
        {
            return form.DataRow;
        }
    }


    /// <summary>
    /// Sets basicform to simple or advanced mode.
    /// </summary>
    public bool SimpleMode
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether to allow mode switching (simple <-> advanced).
    /// </summary>
    public bool AllowModeSwitch
    {
        get
        {
            return form.AllowModeSwitch;
        }
        set
        {
            form.AllowModeSwitch = value;
        }
    }


    /// <summary>
    /// Defines minimum of items needed to be visible to display mode switch
    /// </summary>
    public int MinItemsToAllowSwitch
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        lnkSimple.Click += link_Click;
        lnkAdvanced.Click += link_Click;

        pnlSettings.GroupingText = GetString("templatedesigner.section.settings");
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        lnkSimple.Visible = AllowModeSwitch && !SimpleMode;
        lnkAdvanced.Visible = AllowModeSwitch && SimpleMode;

        if (SimpleMode)
        {
            // Simplify the form
            ReloadForm();
        }
    }


    protected void link_Click(object sender, EventArgs e)
    {
        // Switch the mode
        SimpleMode = !SimpleMode;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Checks if form is loaded with any controls and returns appropriate value.
    /// </summary>
    public bool CheckVisibility()
    {
        Visible = true;

        if (!form.IsAnyFieldVisible() && !AllowModeSwitch)
        {
            Visible = false;
        }

        if (form.FormInformation != null && !form.FormInformation.ItemsList.Any())
        {
            Visible = false;
        }

        return Visible;
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void Reload(bool forceReloadCategories)
    {
        if (fi != null)
        {
            form.SubmitButton.Visible = false;
            form.SiteName = CMSContext.CurrentSiteName;
            form.FormInformation = FormInfo;
            form.Data = GetData();
            form.ShowPrivateFields = true;
            form.ForceReloadCategories = forceReloadCategories;
            form.StopProcessing = false;
        }
        else
        {
            form.DataRow = null;
            form.FormInformation = null;
            form.StopProcessing = true;
        }

        form.ReloadData();
    }


    /// <summary>
    /// Saves basic form data.
    /// </summary>
    public bool SaveData()
    {
        if (form.Visible && form.FieldControls != null && form.FieldControls.Count != 0)
        {
            return form.SaveData(null);
        }

        return true;
    }


    /// <summary>
    /// Loads DataRow for BasicForm with data from FormFieldInfo settings.
    /// </summary>
    private DataRowContainer GetData()
    {
        DataRowContainer result = new DataRowContainer(FormInfo.GetDataRow());

        if (Settings != null)
        {
            foreach (string columnName in result.ColumnNames)
            {
                if (Settings.ContainsKey(columnName) && !String.IsNullOrEmpty(Convert.ToString(Settings[columnName])))
                {
                    result[columnName] = Settings[columnName];
                }
            }
        }
        return result;
    }


    /// <summary>
    /// Sets simple/advanced mode for all forms and reloads their controls.
    /// </summary>
    private void ReloadForm()
    {
        // Check switch visibility - no need to show it if there are no advanced items
        bool mVisible = form.AllowModeSwitch && ContainsAdvancedItems(FormInfo);

        // If there is not enough items, hide switch and set advanced mode
        bool tooFewItems = HasTooFewItems(FormInfo);
        mVisible &= !tooFewItems;

        // Visibility needs to be set via a variable, since if control is in panel that is not visible at the time when this method is called
        // 'Visible' property isn't rewritten (stays false). In that case control tries to set Visible attribute again at PreRender event.
        plcSwitch.Visible = mVisible;

        // And process fields visibility depending on mode
        if (form.IsSimpleMode != (!tooFewItems && SimpleMode))
        {
            form.EnableViewState = false;
            form.IsSimpleMode = !tooFewItems && SimpleMode;
            form.ProcessSimpleModeVisibility();
            form.ReloadData();
        }
    }


    /// <summary>
    /// Checks if FormInfo contains items that are shown only in advanced mode.
    /// </summary>
    private bool ContainsAdvancedItems(FormInfo formInfo)
    {
        if (formInfo == null)
        {
            return false;
        }
        return formInfo.ItemsList.OfType<FormFieldInfo>().Any(y => y.Visible && !y.DisplayInSimpleMode);
    }


    /// <summary>
    /// Indicates whether given FormInfo has too few items to be bothered by mode switching.
    /// </summary>
    private bool HasTooFewItems(FormInfo formInfo)
    {
        if (formInfo == null)
        {
            return false;
        }
        return formInfo.ItemsList.OfType<FormFieldInfo>().Where(y => y.Visible).Count() <= MinItemsToAllowSwitch;
    }

    #endregion


    #region "View State Methods"

    protected override void LoadViewState(object savedState)
    {
        object[] updatedState = (object[])savedState;

        // Load orig ViewState
        if (updatedState[0] != null)
        {
            base.LoadViewState(updatedState[0]);
        }

        // Load Mode settings
        if (updatedState[1] != null)
        {
            SimpleMode = (bool)updatedState[1];
        }
    }


    protected override object SaveViewState()
    {
        object[] updatedState = new object[2];
        updatedState[0] = base.SaveViewState();
        updatedState[1] = SimpleMode;
        return updatedState;
    }

    #endregion
}