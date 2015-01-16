using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Modules_Pages_Development_Module_UI_Tree : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int moduleId = QueryHelper.GetInteger("moduleid", 0);
        int parentId = QueryHelper.GetInteger("parentId", 0);

        ScriptHelper.RegisterJQuery(Page);

        // Use images according to culture
        if (CultureHelper.IsUICultureRTL())
        {
            uniTree.LineImagesFolder = GetImageUrl("RTL/Design/Controls/Tree", false, false);
        }
        else
        {
            uniTree.LineImagesFolder = GetImageUrl("Design/Controls/Tree", false, false);
        }

        if (!RequestHelper.IsPostBack())
        {
            // Get module root element
            UIElementInfo elemInfo = UIElementInfoProvider.GetRootUIElementInfo(moduleId);
            if (elemInfo != null)
            {
                uniTree.SelectPath = elemInfo.ElementIDPath;
                uniTree.ExpandPath = elemInfo.ElementIDPath + "/";
                menuElem.Value = elemInfo.ElementID + "|" + elemInfo.ElementParentID;
            }
            else
            {
                // Get current resource
                ResourceInfo resInfo = ResourceInfoProvider.GetResourceInfo(moduleId);
                if (resInfo != null)
                {
                    // Create new UI element 
                    elemInfo = new UIElementInfo();
                    elemInfo.ElementResourceID = moduleId;
                    elemInfo.ElementDisplayName = resInfo.ResourceDisplayName;
                    elemInfo.ElementName = resInfo.ResourceName.ToLowerCSafe().Replace(".", "");
                    elemInfo.ElementIsCustom = false;

                    UIElementInfoProvider.SetUIElementInfo(elemInfo);
                    uniTree.SelectPath = elemInfo.ElementIDPath;
                    uniTree.ExpandPath = elemInfo.ElementIDPath;
                    menuElem.Value = elemInfo.ElementID + "|0";
                }
            }
        }

        menuElem.ResourceID = moduleId;
        menuElem.AfterAction += new OnActionEventHandler(menuElem_AfterAction);

        // Create and set UIElements provider
        UniTreeProvider elementProvider = new UniTreeProvider();
        elementProvider.ObjectType = "CMS.UIElement";
        elementProvider.DisplayNameColumn = "ElementDisplayName";
        elementProvider.IDColumn = "ElementID";
        elementProvider.LevelColumn = "ElementLevel";
        elementProvider.OrderColumn = "ElementOrder";
        elementProvider.ParentIDColumn = "ElementParentID";
        elementProvider.PathColumn = "ElementIDPath";
        elementProvider.ValueColumn = "ElementID";
        elementProvider.ChildCountColumn = "ElementChildCount";
        elementProvider.WhereCondition = "ElementResourceID = " + moduleId;
        elementProvider.Columns = "ElementID,ElementLevel,ElementOrder,ElementParentID,ElementIDPath,ElementChildCount,ElementDisplayName";

        uniTree.UsePostBack = false;
        uniTree.ProviderObject = elementProvider;
        uniTree.ExpandTooltip = GetString("general.expand");
        uniTree.CollapseTooltip = GetString("general.collapse");

        uniTree.NodeTemplate = "<span id=\"node_##NODEID##\" onclick=\"SelectNode(##NODEID##,##PARENTNODEID##," + moduleId + "); return false;\" name=\"treeNode\" class=\"ContentTreeItem\"><span class=\"Name\">##NODENAME##</span></span>";
        uniTree.SelectedNodeTemplate = "<span id=\"node_##NODEID##\" onclick=\"SelectNode(##NODEID##,##PARENTNODEID##," + moduleId + "); return false;\" name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\"><span class=\"Name\">##NODENAME##</span></span>";

        if (!RequestHelper.IsPostBack())
        {
            string selectedPath = QueryHelper.GetString("path", String.Empty);
            int elementId = QueryHelper.GetInteger("elementId", 0);

            if (!string.IsNullOrEmpty(selectedPath))
            {
                uniTree.SelectPath = selectedPath;
            }

            if (elementId > 0)
            {
                menuElem.ElementID = elementId;
                menuElem.ParentID = parentId;
                menuElem.Value = elementId + "|" + parentId;
            }
        }

        // Load data
        uniTree.ReloadData();

        string script = "var frameURL = '" + ResolveUrl("~/CMSModules/Modules/Pages/Development/Module_UI_EditFrameset.aspx") + "';";
        script += "var newURL = '" + ResolveUrl("~/CMSModules/Modules/Pages/Development/Module_UI_New.aspx") + "';";
        script += "var postParentId = " + parentId + ";";

        ltlScript.Text = ScriptHelper.GetScript(script);
    }


    protected void menuElem_AfterAction(string actionName, object actionArgument)
    {
        string[] split = actionArgument.ToString().Split('|');
        int elementId = ValidationHelper.GetInteger(split[0], 0);

        UIElementInfo elemInfo = UIElementInfoProvider.GetUIElementInfo(elementId);
        if (elemInfo != null)
        {
            uniTree.SelectPath = elemInfo.ElementIDPath;
            switch (actionName.ToLowerCSafe())
            {
                case "delete":
                    uniTree.ExpandPath = elemInfo.ElementIDPath + "/";
                    // Reload header and content after save
                    StringBuilder sb = new StringBuilder();

                    sb.Append("if (window.parent != null) {");
                    sb.Append("if (window.parent.frames['uicontent'] != null) {");
                    if (elemInfo.ElementParentID > 0)
                    {
                        // If not root element load edit frameset
                        sb.Append("window.parent.frames['uicontent'].location = '" + ResolveUrl("~/CMSModules/Modules/Pages/Development/Module_UI_EditFrameset.aspx") + "?moduleID=" + elemInfo.ElementResourceID + "&elementId=" + elemInfo.ElementID + "&parentId=" + elemInfo.ElementParentID + "';");
                    }
                    else
                    {
                        // Else load root info page
                        sb.Append("window.parent.frames['uicontent'].location = '" + ResolveUrl("~/CMSModules/Modules/Pages/Development/Module_UI_New.aspx") + "?moduleID=" + elemInfo.ElementResourceID + "&parentId=0';");
                    }
                    sb.Append("}");
                    sb.Append("}");
                    ltlScript.Text += ScriptHelper.GetScript(sb.ToString());
                    // Update menu actions parameters
                    menuElem.Value = elemInfo.ElementID + "|" + elemInfo.ElementParentID;
                    break;
                case "moveup":
                case "movedown":
                    if (split.Length == 2)
                    {
                        ltlScript.Text += ScriptHelper.GetScript("window.tabIndex = " + split[1] + ";");
                    }
                    break;
            }
            ltlScript.Text += ScriptHelper.GetScript("var postParentId = " + elemInfo.ElementParentID + ";");
            // Load data
            uniTree.ReloadData();
        }
    }
}