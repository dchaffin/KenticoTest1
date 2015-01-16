using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Settings_Development_CustomSettings_CustomSettings_Menu : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);

        // Root of tree flag
        string treeRoot = QueryHelper.GetString("treeroot", "customsettings");

        // Decide which category to use as root: custom settings or settings
        switch (treeRoot)
        {
            case ("settings"):
                treeSettings.CategoryName = "CMS.Settings";
                treeSettings.RootIsClickable = true;
                menuElem.WholeSettings = true;
                break;
            case ("customsettings"):
            default:
                treeSettings.CategoryName = "CMS.CustomSettings";
                menuElem.WholeSettings = false;
                break;
        }

        menuElem.AfterAction += new OnActionEventHandler(menuElem_AfterAction);

        // Prepare root info
        SettingsCategoryInfo rootCat = SettingsCategoryInfoProvider.GetSettingsCategoryInfoByName(treeSettings.CategoryName);

        StringBuilder sb = new StringBuilder("");
        StringBuilder sbAfter = new StringBuilder("");
        // Create links for JS
        sb.Append("var frameURL = '").Append(ResolveUrl("~/CMSModules/Settings/Development/CustomSettings/CustomSettingsCategory_Default.aspx?treeroot=" + ScriptHelper.GetString(treeRoot, false))).Append("';");
        sb.Append("var newURL = '").Append(ResolveUrl("~/CMSModules/Settings/Development/CustomSettings/CustomSettingsCategory_Edit.aspx?treeroot=" + ScriptHelper.GetString(treeRoot, false))).Append("';");
        sb.Append("var rootParentId = ").Append(rootCat != null ? rootCat.CategoryParentID : 0).Append(";");

        // Disable delete on custom settings category
        if (treeRoot == "settings")
        {
            DataSet ds = SettingsCategoryInfoProvider.GetSettingsCategories("CategoryName = 'CMS.CustomSettings'", null, 1, "CategoryID");
            int customSettingsId = 0;
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                customSettingsId = ValidationHelper.GetInteger(ds.Tables[0].Rows[0]["CategoryID"], 0);
            }
            sb.Append("var customSettingsId = ").Append(customSettingsId).Append(";");
        }
        else
        {
            sb.Append("var customSettingsId = 0;");
        }


        if (!RequestHelper.IsPostBack())
        {
            int categoryId = QueryHelper.GetInteger("categoryid", -1);
            treeSettings.SelectPath = "/";

            SettingsCategoryInfo category = SettingsCategoryInfoProvider.GetSettingsCategoryInfo(categoryId);
            // Select requested category
            if (category != null)
            {
                treeSettings.SelectPath = category.CategoryIDPath;
                menuElem.ElementID = category.CategoryID;
                menuElem.ParentID = category.CategoryParentID;
                menuElem.Value = category.CategoryID + "|" + category.CategoryParentID;

                // Select category after page load
                sbAfter.Append("SelectNode(" + ScriptHelper.GetString(category.CategoryName) + ");");
                sbAfter.Append("enableMenu(").Append(category.CategoryID).Append(",").Append(category.CategoryParentID).Append(");");
            }
            //  Select root
            else
            {
                if (rootCat != null)
                {
                    menuElem.ElementID = rootCat.CategoryID;
                    menuElem.ParentID = rootCat.CategoryParentID;
                    menuElem.Value = rootCat.CategoryID + "|" + rootCat.CategoryParentID;
                }

                // Select category after page load
                sbAfter.Append("SelectNode(").Append(ScriptHelper.GetString(treeSettings.CategoryName)).Append(");");
                sbAfter.Append("enableMenu(").Append(rootCat.CategoryID).Append(",").Append(rootCat.CategoryParentID).Append(");");
            }
        }
        sb.Append("var postParentId = ").Append(rootCat.CategoryParentID).Append(";");

        ltlScript.Text = ScriptHelper.GetScript(sb.ToString());
        ltlAfterScript.Text = ScriptHelper.GetScript(sbAfter.ToString());
    }


    /// <summary>
    /// Raised after menu action (new, delete, up or down) has been taken.
    /// </summary>
    protected void menuElem_AfterAction(string actionName, object actionArgument)
    {
        string[] split = actionArgument.ToString().Split('|');
        int categoryId = ValidationHelper.GetInteger(split[0], 0);

        SettingsCategoryInfo category = SettingsCategoryInfoProvider.GetSettingsCategoryInfo(categoryId);
        if (category != null)
        {
            treeSettings.SelectPath = category.CategoryIDPath;
            treeSettings.SelectedItem = category.CategoryName;
            switch (actionName.ToLowerCSafe())
            {
                case "delete":
                    treeSettings.ExpandPath = category.CategoryIDPath + "/";
                    // Reload header and content after save
                    StringBuilder sb = new StringBuilder();

                    sb.Append("if (window.parent != null) {");
                    sb.Append("if (window.parent.frames['customsettingsmain'] != null) {");
                    sb.Append("window.parent.frames['customsettingsmain'].location = '" + ResolveUrl("CustomSettingsCategory_Default.aspx") + "?categoryid=" + categoryId + "';");
                    sb.Append("}");
                    sb.Append("}");

                    ltlScript.Text += ScriptHelper.GetScript(sb.ToString());
                    treeSettings.ExpandPath = category.CategoryIDPath + "/";

                    // Update menu actions parameters
                    menuElem.Value = category.CategoryID + "|" + category.CategoryParentID;
                    break;

                case "moveup":
                    if (split.Length == 2)
                    {
                        ltlScript.Text += ScriptHelper.GetScript("window.tabIndex = " + split[1] + ";");
                    }
                    break;

                case "movedown":
                    if (split.Length == 2)
                    {
                        ltlScript.Text += ScriptHelper.GetScript("window.tabIndex = " + split[1] + ";");
                    }
                    break;
            }
            ltlScript.Text += ScriptHelper.GetScript("SelectNode(" + ScriptHelper.GetString(category.CategoryName) + ");");
            ltlScript.Text += ScriptHelper.GetScript("var postParentId = " + category.CategoryParentID + ";");

            // Load data
            treeSettings.ReloadData();
        }
    }
}