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

public partial class CMSModules_Translations_FormControls_TranslationPriority : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Returns the instructions of the submission.
    /// </summary>
    public int Priority
    {
        get
        {
            return ValidationHelper.GetInteger(drpPriority.SelectedValue, 1);
        }
        set
        {
            EnsureChildControls();

            string val = ValidationHelper.GetString(value, "1");
            ListItem sel = drpPriority.Items.FindByValue(val);
            if (sel != null)
            {
                drpPriority.ClearSelection();
                sel.Selected = true;
            }
        }
    }


    /// <summary>
    /// Gets or sets the priority of the translation.
    /// </summary>
    public override object Value
    {
        get
        {
            return Priority;
        }
        set
        {
            Priority = ValidationHelper.GetInteger(value, 0);
        }
    }

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        if (!StopProcessing)
        {
            if (drpPriority.Items.Count == 0)
            {
                ListItem normalPriority = new ListItem(TranslationServiceHelper.GetPriorityText(1), "1");
                normalPriority.Selected = true;

                drpPriority.Items.Add(new ListItem(TranslationServiceHelper.GetPriorityText(0), "0"));
                drpPriority.Items.Add(normalPriority);
                drpPriority.Items.Add(new ListItem(TranslationServiceHelper.GetPriorityText(2), "2"));
            }
        }
    }

    #endregion
}