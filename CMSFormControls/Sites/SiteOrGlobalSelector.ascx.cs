using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.UIControls;

/// <summary>
/// Enables to select from 3 values:
/// (global and this site)  -5
/// (global)                -4
/// CURRENT_SITE            SITE_ID
/// </summary>
public partial class CMSFormControls_Sites_SiteOrGlobalSelector : FormEngineUserControl
{
    #region "Variables"

    private string selectedValue = null;
    private int mCurrentSiteID = 0;
    private string mCurrentSiteName = null;
    private bool mShowSiteAndGlobal = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns current site ID. If not found, then processing of control is halted.
    /// </summary>
    private int CurrentSiteID
    {
        get
        {
            if ((mCurrentSiteID == 0) && (!StopProcessing))
            {
                mCurrentSiteID = CMSContext.CurrentSiteID;
            }

            StopProcessing = (mCurrentSiteID == 0);
            return (int)mCurrentSiteID;
        }
    }


    /// <summary>
    /// Returns current site display name. If not found, then processing of control is halted.
    /// </summary>
    private string CurrentSiteName
    {
        get
        {
            if ((mCurrentSiteName == null) && (!StopProcessing) && (CMSContext.CurrentSite != null))
            {
                mCurrentSiteName = CMSContext.CurrentSite.DisplayName;
            }

            StopProcessing = String.IsNullOrEmpty(mCurrentSiteName);
            return mCurrentSiteName;
        }
    }


    /// <summary>
    /// Returns selector control internaly used for selecting sites.
    /// </summary>
    public DropDownList Selector
    {
        get
        {
            return dropDownList;
        }
    }


    /// <summary>
    /// Selected site. Returns current site ID if global objects are not allowed.
    /// </summary>
    public override object Value
    {
        get
        {
            return dropDownList.SelectedValue;
        }
        set
        {
            int id = ValidationHelper.GetInteger(value, 0);

            if (id > 0)
            {
                selectedValue = CurrentSiteID.ToString();
                dropDownList.SelectedValue = selectedValue;
            }
            else
            {
                selectedValue = ValidationHelper.GetString(value, CurrentSiteID.ToString());
                dropDownList.SelectedValue = selectedValue;
            }
        }
    }


    /// <summary>
    /// ID of the selected site.
    /// </summary>
    public int SiteID
    {
        get
        {
            return ValidationHelper.GetInteger(Value, CurrentSiteID);
        }
        set
        {
            Value = value;
        }
    }


    /// <summary>
    /// Gets or sets CSS class of current control.
    /// </summary>
    public string DropDownCSSClass
    {
        get
        {
            return dropDownList.CssClass;
        }
        set
        {
            dropDownList.CssClass = value;
        }
    }


    /// <summary>
    /// Gets or sets if dropdown list causes autopostback
    /// </summary>
    public bool AutoPostBack
    {
        get
        {
            return dropDownList.AutoPostBack;
        }
        set
        {
            dropDownList.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Gets or sets visibility to 'global and this site' value.
    /// </summary>
    public bool ShowSiteAndGlobal
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSiteAndGlobal"), mShowSiteAndGlobal);
        }
        set
        {
            SetValue("ShowSiteAndGlobal", value);
        }
    }

    #endregion


    #region "Methods and events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!StopProcessing)
        {
            if (dropDownList.Items.Count <= 0)
            {
                if (ShowSiteAndGlobal)
                {
                    dropDownList.Items.Add(new ListItem(GetString("general.globalandsite"), UniSelector.US_GLOBAL_OR_SITE_RECORD.ToString()));
                }
                dropDownList.Items.Add(new ListItem(GetString("general.global"), UniSelector.US_GLOBAL_RECORD.ToString()));
                dropDownList.Items.Add(new ListItem(ResHelper.LocalizeString(CurrentSiteName), CurrentSiteID.ToString()));
                dropDownList.SelectedValue = selectedValue;
            }
        }
    }


    /// <summary>
    /// Returns WHERE condition for specified column.
    /// </summary>
    public string GetSiteWhereCondition(string siteColumnName)
    {
        if (!StopProcessing)
        {
            string where = string.Format("{0} = {1}", siteColumnName, SiteID);

            switch (SiteID)
            {
                case (UniSelector.US_GLOBAL_OR_SITE_RECORD): // (Global and site)
                    where = string.Format("{0} IS NULL OR {0} = {1}", siteColumnName, CurrentSiteID);
                    break;

                case (UniSelector.US_GLOBAL_RECORD): // (Global)
                case (UniSelector.US_NONE_RECORD):
                    where = string.Format("{0} IS NULL", siteColumnName);
                    break;
            }

            return where;
        }
        return null;
    }

    #endregion
}