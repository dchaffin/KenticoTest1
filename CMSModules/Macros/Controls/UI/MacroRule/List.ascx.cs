using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.SettingsProvider;

public partial class CMSModules_Macros_Controls_UI_MacroRule_List : CMSAdminListControl
{
    #region "Variables"

    protected string mResourceName = "";

    #endregion


    #region "Properties"

    /// <summary>
    /// Name of the resource for which the rule is edited.
    /// </summary>
    public string ResourceName
    {
        get
        {
            return mResourceName;
        }
        set
        {
            mResourceName = value;
        }
    }


    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return this.gridElem;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            this.gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set value to the control so that filter control can get it
        this.gridElem.RememberStateByParam = "module";
        this.gridElem.SetValue("ResourceName", ResourceName);
        this.gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);

        // Hide category column and its filter when user is not global administrator
        if (!CurrentUser.IsGlobalAdministrator || string.IsNullOrEmpty(ResourceName))
        {
            this.gridElem.GridColumns.Columns[2].Visible = false;
            this.gridElem.GridColumns.Columns[2].Filter = null;

            // Use default WHERE conditions when category filter is not displayed
            if (!string.IsNullOrEmpty(ResourceName))
            {
                this.gridElem.WhereCondition = "MacroRuleResourceName = N'" + SqlHelperClass.GetSafeQueryString(ResourceName, false) + "'";
            }
            else
            {
                // Only global rules should be displayed
                this.gridElem.WhereCondition = "MacroRuleResourceName = '' OR MacroRuleResourceName IS NULL";
            }
        }
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "category":
                string cat = ValidationHelper.GetString(parameter, "");
                if (string.IsNullOrEmpty(cat))
                {
                    return GetString("general.global");
                }
                return cat;
        }
        return parameter;
    }

    #endregion
}