using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

using CMS.SettingsProvider;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.TranslationServices;
using CMS.ExtendedControls;
using CMS.FormControls;

public partial class CMSModules_Translations_FormControls_TranslationStatus : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            drpStatus.Enabled = value;
        }
    }


    /// <summary>
    /// Returns ClientID of the DropDownList with authentication.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpStatus.ClientID;
        }
    }


    /// <summary>
    /// Returns the instructions of the submission.
    /// </summary>
    private int Status
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpStatus.SelectedValue, "");
        }
        set
        {
            Status = ValidationHelper.GetInteger(value, 0);
            ReloadData();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        return true;
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        ReloadData();
    }


    /// <summary>
    /// Loads drop down list with data.
    /// </summary>
    private void ReloadData()
    {
        if (drpStatus.Items.Count == 0)
        {
            drpStatus.Items.Add(new ListItem(GetString("translations.status." + TranslationStatusEnum.WaitingForTranslation), "0"));
            drpStatus.Items.Add(new ListItem(GetString("translations.status." + TranslationStatusEnum.TranslationReady), "1"));
            drpStatus.Items.Add(new ListItem(GetString("translations.status." + TranslationStatusEnum.TranslationCompleted), "2"));
            drpStatus.Items.Add(new ListItem(GetString("translations.status." + TranslationStatusEnum.SubmissionError), "3"));
            drpStatus.Items.Add(new ListItem(GetString("translations.status." + TranslationStatusEnum.ProcessingError), "4"));
            drpStatus.Items.Add(new ListItem(GetString("translations.status." + TranslationStatusEnum.TranslationCanceled), "5"));
        }

        // Preselect value
        ListItem selectedItem = drpStatus.Items.FindByValue(Status.ToString());
        if (selectedItem != null)
        {
            drpStatus.ClearSelection();
            selectedItem.Selected = true;
        }
        else
        {
            drpStatus.SelectedIndex = 0;
        }
    }

    #endregion
}