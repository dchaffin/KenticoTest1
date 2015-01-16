using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_WebAnalytics_Tools_Analytics_Statistics : CMSWebAnalyticsPage
{
    #region "Variables"

    /// <summary>
    /// List of special conversions not displayed either in custom node list or in ui list.
    /// </summary>
    private const string additionalConversions = "'visitfirst';'visitreturn';'referringsite_direct';'referringsite_search';'referringsite_referring';'referringsite_local';'avgtimeonpage'";


    /// <summary>
    /// Where condition used for select custom conversions.
    /// </summary>
    private string customWhereCondition = String.Empty;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        treeElem.OnNodeCreated += new CMSAdminControls_UI_UIProfiles_UIMenu.NodeCreatedEventHandler(menuElem_OnNodeCreated);

        // Get NOT custom conversions from UI elements 
        UIElementInfo root = UIElementInfoProvider.GetRootUIElementInfo("CMS.WebAnalytics");
        if (root != null)
        {
            // Get all UI elements to filter custom reports
            DataSet dsElems = UIElementInfoProvider.GetUIElements("ElementIDPath LIKE '" + DataHelper.EscapeLikeQueryPatterns(root.ElementIDPath, true, true, true) + "/%'", String.Empty, 0, "ElementName,ElementTargetUrl");
            if (!DataHelper.DataSourceIsEmpty(dsElems) && (dsElems.Tables.Count > 0))
            {
                // Condition for custom reports
                customWhereCondition = "StatisticsCode NOT IN (";
                foreach (DataRow dr in dsElems.Tables[0].Rows)
                {
                    string codeName = ValidationHelper.GetString(dr["ElementName"], String.Empty);
                    customWhereCondition += "N'" + SqlHelperClass.GetSafeQueryString(codeName, false) + "',";
                }

                // Add special cases - dont want to show them in UI or Custom report section
                customWhereCondition += additionalConversions.Replace(';', ',');

                customWhereCondition = customWhereCondition.TrimEnd(new char[] { ',' });
                customWhereCondition += ")";

                // Filter AB Testing
                customWhereCondition += " AND (StatisticsCode NOT LIKE 'abconversion;%') AND (StatisticsCode NOT LIKE 'mvtconversion;%') AND (StatisticsCode NOT LIKE 'campconversion;%') ";
            }
        }
    }


    private TreeNode menuElem_OnNodeCreated(UIElementInfo uiElement, TreeNode defaultNode)
    {
        String elementName = uiElement.ElementName.ToLowerCSafe();

        // Remove Optimalization node when module OnlineMarketing not present        
        if (elementName == "optimalization")
        {
            if (!ModuleEntry.IsModuleLoaded(ModuleEntry.ONLINEMARKETING))
            {
                return null;
            }
        }

        String imagesUrl = "CMSModules/CMS_WebAnalytics/";
        if (uiElement != null)
        {
            if (!IsToolsUIElementAvailable(uiElement))
            {
                return null;
            }
        }

        // Add all custom reports
        if (elementName == "custom")
        {
            customWhereCondition = SqlHelperClass.AddWhereCondition(customWhereCondition, " StatisticsSiteID = " + CMSContext.CurrentSiteID);
            DataSet ds = StatisticsInfoProvider.GetCodeNames(customWhereCondition, "StatisticsCode ASC", 0, "StatisticsCode");

            // If no custom reports found - hide Custom Reports node
            if (DataHelper.DataSourceIsEmpty(ds))
            {
                return null;
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TreeNode childNode = new TreeNode();

                string codeName = ValidationHelper.GetString(dr["StatisticsCode"], String.Empty).ToLowerCSafe();
                string name = GetString("analytics_codename." + codeName);
                string dataCodeName = GetDataCodeName(codeName);
                string reportCodeName = GetReportCodeNames(codeName);

                childNode.Text = "<span id=\"node_" + codeName + "\" class=\"ContentTreeItem\" name=\"treeNode\"><span class=\"Name\">" + name + "</span></span>";
                childNode.NavigateUrl = "~/CMSModules/WebAnalytics/Tools/Analytics_Report.aspx?statCodeName=" + codeName + "&dataCodeName=" + dataCodeName + "&reportCodeName=" + reportCodeName + "&isCustom=1";
                childNode.Target = treeElem.TargetFrame;

                // Icon
                String iconPath = imagesUrl + codeName.Replace(".", "_") + ".png";
                String corePath = URLHelper.ResolveUrl(UIHelper.GetImagePath(Page, iconPath, false, false));
                
                if (FileHelper.FileExists(corePath))
                {
                    childNode.ImageUrl = GetImageUrl(iconPath);
                }
                else
                {
                    childNode.ImageUrl = GetImageUrl(imagesUrl + "statistics.png");
                }
                defaultNode.ChildNodes.Add(childNode);
            }
        }
        return defaultNode;
    }


    /// <summary>
    /// Returns generic report code names (based on analytics code name).
    /// </summary>
    /// <param name="statCodeName">Analytics code name (pageviews, pageviews.multilingual...)</param>
    private static string GetReportCodeNames(string statCodeName)
    {
        string result = "";
        result += statCodeName + ".yearreport";
        result += ";" + statCodeName + ".monthreport";
        result += ";" + statCodeName + ".weekreport";
        result += ";" + statCodeName + ".dayreport";
        result += ";" + statCodeName + ".hourreport";
        return result;
    }


    /// <summary>
    /// Returns data code name from analytics code name.
    /// </summary>
    /// <param name="statCodeName">Analytics code name (pageviews, pageviews.multilingual...)</param>
    private static string GetDataCodeName(string statCodeName)
    {
        int pos = statCodeName.IndexOfCSafe('.');
        if (pos > 0)
        {
            return statCodeName.Substring(0, pos);
        }

        return statCodeName;
    }

    #endregion
}