using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;


public partial class CMSModules_WebAnalytics_FormControls_SelectConversionDialog : CMSDialogPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterWOpenerScript(Page);

        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/OM_ABTest/Conversion.png");

        ucConversions.Query = "analytics.statistics.selectconversion";
        ucConversions.Columns = "StatisticsObjectName";
        ucConversions.GridName = "~/CMSModules/WebAnalytics/FormControls/SelectConversions.xml";
        ucConversions.OrderBy = "StatisticsObjectName";
        ucConversions.WhereCondition = " StatisticsSiteID = " + CMSContext.CurrentSiteID + " AND (StatisticsCode LIKE N'mvtconversion;%' OR StatisticsCode = N'conversion' OR StatisticsCode LIKE N'abconversion;%')";
        btnCancel.OnClientClick = "CloseDialog()";

        ucConversions.OnExternalDataBound += new OnExternalDataBoundEventHandler(ucConversions_OnExternalDataBound);

        CurrentMaster.Title.TitleText = GetString("om.selectconversion");
        if (URLHelper.IsPostback())
        {
            string filter = txtNameFilter.Text.Trim();
            if (filter != String.Empty)
            {
                ucConversions.WhereCondition = SqlHelperClass.AddWhereCondition(ucConversions.WhereCondition, "StatisticsObjectName LIKE '%" + SqlHelperClass.GetSafeQueryString(filter, false) + "%'");
            }
        }
    }


    private object ucConversions_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "conversion":
                {
                    string name = ValidationHelper.GetString(parameter, String.Empty);
                    string onclick = "wopener.selectConversionValue ('" + ScriptHelper.GetString(QueryHelper.GetString("textboxID", String.Empty), false) + "','" + ScriptHelper.GetString(name, false) + "');CloseDialog()";
                    return "<div class=\"SelectableItem\" onclick=\"" + onclick + "\">" + HTMLHelper.HTMLEncode(TextHelper.LimitLength(name, 100)) + "</div>";
                    ;
                }
        }
        return null;
    }
}