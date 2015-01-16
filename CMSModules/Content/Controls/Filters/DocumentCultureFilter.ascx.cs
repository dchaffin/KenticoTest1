using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Content_Controls_Filters_DocumentCultureFilter : CMSAbstractBaseFilterControl
{
    #region "Variables"

    private DataSet mSiteCultures = null;
    private string mDefaultSiteCulture = null;
    private string currentSiteName = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Default culture of the site.
    /// </summary>
    private string DefaultSiteCulture
    {
        get
        {
            return mDefaultSiteCulture ?? (mDefaultSiteCulture = CultureHelper.GetDefaultCulture(currentSiteName));
        }
    }


    /// <summary>
    /// Site cultures.
    /// </summary>
    private DataSet SiteCultures
    {
        get
        {
            if (mSiteCultures == null)
            {
                mSiteCultures = CultureInfoProvider.GetSiteCultures(currentSiteName).Copy();
                if (!DataHelper.DataSourceIsEmpty(mSiteCultures))
                {
                    DataTable cultureTable = mSiteCultures.Tables[0];
                    DataRow[] defaultCultureRow = cultureTable.Select("CultureCode='" + DefaultSiteCulture + "'");

                    // Ensure default culture to be first
                    DataRow dr = cultureTable.NewRow();
                    if (defaultCultureRow.Length > 0)
                    {
                        dr.ItemArray = defaultCultureRow[0].ItemArray;
                        cultureTable.Rows.InsertAt(dr, 0);
                        cultureTable.Rows.Remove(defaultCultureRow[0]);
                    }
                }
            }
            return mSiteCultures;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GenerateWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        currentSiteName = CMSContext.CurrentSiteName;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Fill drop down lists
        InitDropdownLists();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes drop down lists.
    /// </summary>
    private void InitDropdownLists()
    {
        // Init cultures
        cultureElem.DropDownCultures.CssClass = "SelectorDropDown";
        cultureElem.DropDownCultures.Width = 306;
        cultureElem.AddDefaultRecord = false;
        cultureElem.UpdatePanel.RenderMode = UpdatePanelRenderMode.Inline;
        cultureElem.SpecialFields = new string[,]
                                        {
                                            { GetString("transman.anyculture"), "##ANY##" },
                                            { GetString("transman.allcultures"), "##ALL##" }
                                        };

        // Init operands
        if (drpLanguage.Items.Count == 0)
        {
            drpLanguage.Items.Add(new ListItem(GetString("transman.translatedto"), "="));
            drpLanguage.Items.Add(new ListItem(GetString("transman.nottranslatedto"), "<>"));
        }
    }


    /// <summary>
    /// Creates where condition according to values selected in filter.
    /// </summary>
    private string GenerateWhereCondition()
    {
        string where = "";

        string val = ValidationHelper.GetString(cultureElem.Value, string.Empty);
        if (val == string.Empty)
        {
            val = "##ANY##";
        }

        if (val != "##ANY##")
        {
            switch (val)
            {
                case "##ALL##":
                    where = SqlHelperClass.AddWhereCondition(where, "((SELECT COUNT(*) FROM View_CMS_Tree_Joined AS TreeView WHERE TreeView.NodeID = View_CMS_Tree_Joined_Versions.NodeID) " + SqlHelperClass.GetSafeQueryString(drpLanguage.SelectedValue, false) + " " + SiteCultures.Tables[0].Rows.Count + ")");
                    break;

                default:
                    string oper = (drpLanguage.SelectedValue == "<>") ? "NOT" : "";
                    where = SqlHelperClass.AddWhereCondition(where, "NodeID " + oper + " IN (SELECT NodeID FROM View_CMS_Tree_Joined AS TreeView WHERE TreeView.NodeID = NodeID AND DocumentCulture = '" + SqlHelperClass.GetSafeQueryString(val, false) + "')");
                    break;
            }
        }
        else if (drpLanguage.SelectedValue == "<>")
        {
            where = SqlHelperClass.NO_DATA_WHERE;
        }

        return where;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        drpLanguage.SelectedIndex = 0;
        cultureElem.Value = "##ANY##";
    }

    #endregion
}
