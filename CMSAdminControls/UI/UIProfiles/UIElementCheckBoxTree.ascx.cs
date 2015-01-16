using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UIProfiles_UIElementCheckBoxTree : CMSUserControl, ICallbackEventHandler
{
    #region "Variables"

    protected string mGroupPreffix = null;
    protected int mModuleID = 0;
    protected int mRoleID = 0;
    protected int mSiteID = 0;
    private bool mEnabled = true;
    private string mSiteName = null;
    private string mCallbackRef = null;
    private UIElementInfo root = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if all nodes should be expanded.
    /// </summary>
    public bool ExpandAll
    {
        get
        {
            return treeElem.ExpandAll;
        }
        set
        {
            treeElem.ExpandAll = value;
        }
    }


    /// <summary>
    /// Indicates if all nodes should be collapsed.
    /// </summary>
    public bool CollapseAll
    {
        get
        {
            return treeElem.CollapseAll;
        }
        set
        {
            treeElem.CollapseAll = value;
        }
    }


    /// <summary>
    /// Gets or sets the prefix of the exmenet name which should not have the checkbox.
    /// </summary>
    public string GroupPreffix
    {
        get
        {
            return mGroupPreffix;
        }
        set
        {
            mGroupPreffix = value;
        }
    }


    /// <summary>
    /// ID of the module.
    /// </summary>
    public int ModuleID
    {
        get
        {
            return mModuleID;
        }
        set
        {
            mModuleID = value;
        }
    }


    /// <summary>
    /// ID of the role.
    /// </summary>
    public int RoleID
    {
        get
        {
            return mRoleID;
        }
        set
        {
            mRoleID = value;
        }
    }


    /// <summary>
    /// ID of the site.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
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
            treeElem.IsLiveSite = false;
        }
    }


    /// <summary>
    /// Indicates if checkboxes and select/deselect all should be enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// Name of the site.
    /// </summary>
    private string SiteName
    {
        get
        {
            if (mSiteName == null)
            {
                SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteID);
                if (si != null)
                {
                    mSiteName = si.SiteName;
                }
            }
            return mSiteName;
        }
    }


    /// <summary>
    /// Callback reference for selecting items.
    /// </summary>
    private string CallbackRef
    {
        get
        {
            if (String.IsNullOrEmpty(mCallbackRef))
            {
                mCallbackRef = Page.ClientScript.GetCallbackEventReference(this, "hdnValue.value", "callbackHandler", "callbackHandler");
            }

            return mCallbackRef;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        treeElem.OnNodeCreated += new CMSAdminControls_UI_Trees_UniTree.NodeCreatedEventHandler(treeElem_OnNodeCreated);
    }


    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Register scripts
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "UITreeCallbackHandler", ScriptHelper.GetScript(
            "var hdnValue = document.getElementById('" + hdnValue.ClientID + "'); \n\n " +
            "function callbackHandler(content, context) {}"));

        // Use images according to culture
        if (CultureHelper.IsUICultureRTL())
        {
            treeElem.LineImagesFolder = GetImageUrl("RTL/Design/Controls/Tree", false, false);
        }
        else
        {
            treeElem.LineImagesFolder = GetImageUrl("Design/Controls/Tree", false, false);
        }

        string noChkSelectString = GetString("uiprofile.selectallconfirmation");
        string noChkDeselectString = GetString("uiprofile.deselectallconfirmation");
        string selectString = GetString("uiprofile.selectallcurrentconfirmation");
        string deselectString = GetString("uiprofile.deselectallcurrentconfirmation");

        // Register scripts only if enabled
        if (Enabled)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("function SelectAllSubelements(elem, id, hasChkBox) { \n");
            sb.Append("  if ((hasChkBox ? confirm('" + selectString + "') : confirm('" + noChkSelectString + "'))) { \n");
            sb.Append("    hdnValue.value = 's;' + id + ';' + (hasChkBox? 1 : 0); \n");
            sb.Append("    var tab = elem.parents('table'); \n");
            sb.Append("    tab.find('input[type=checkbox]').attr('checked', 'checked'); \n");
            sb.Append("    var node = tab.next(); \n");
            sb.Append("    if ((node.length > 0)&&(node[0].nodeName.toLowerCase() == 'div')) { \n");
            sb.Append("       node.find('input[type=checkbox]').attr('checked', 'checked'); \n");
            sb.Append("    } \n");
            sb.Append("  } \n");
            sb.Append("} \n");
            sb.Append("function DeselectAllSubelements(elem, id, hasChkBox) { \n");
            sb.Append("  if ((hasChkBox ? confirm('" + deselectString + "') : confirm('" + noChkDeselectString + "'))) { \n");
            sb.Append("    hdnValue.value = 'd;' + id + ';' + (hasChkBox? 1 : 0); \n");
            sb.Append("    var tab = elem.parents('table'); \n");
            sb.Append("    tab.find('input[type=checkbox]').removeAttr('checked'); \n");
            sb.Append("    var node = tab.next(); \n");
            sb.Append("    if ((node.length > 0)&&(node[0].nodeName.toLowerCase() == 'div')) { \n");
            sb.Append("       node.find('input[type=checkbox]').removeAttr('checked'); \n");
            sb.Append("    } \n");
            sb.Append("  } \n");
            sb.Append("} \n");

            ScriptHelper.RegisterClientScriptBlock(Page, typeof(Page), "UITreeSelectScripts", ScriptHelper.GetScript(sb.ToString()));
        }
    }


    protected TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        // Get data
        if (itemData != null)
        {
            int id = ValidationHelper.GetInteger(itemData["ElementID"], 0);
            int childCount = ValidationHelper.GetInteger(itemData["ElementChildCount"], 0);
            bool selected = ValidationHelper.GetBoolean(itemData["ElementSelected"], false);
            string displayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(itemData["ElementDisplayName"], "")));
            string elementName = ValidationHelper.GetString(itemData["ElementName"], "").ToLowerCSafe();
            string iconUrl = ValidationHelper.GetString(itemData["ElementIconPath"], "");

            string onClickDeclaration = " var chkElem_" + id + " = document.getElementById('chk_" + id + "'); ";
            string onClickCommon = "  hdnValue.value = " + id + " + ';' + chkElem_" + id + ".checked; " + CallbackRef;
            string onClickSpan = " chkElem_" + id + ".checked = !chkElem_" + id + ".checked; ";

            string nodeText = "";
            if (!String.IsNullOrEmpty(GroupPreffix) && elementName.ToLowerCSafe().StartsWithCSafe(GroupPreffix.ToLowerCSafe()))
            {
                nodeText = "<span>" + displayName + "</span>" +
                           (childCount > 0 ? "&nbsp;<span class=\"UITreeSelectButton\">(<span onclick=\"" + (Enabled ? "SelectAllSubelements($j(this), " + id + ", false);" + CallbackRef + ";" : "return false;") + "\" >" + GetString("uiprofile.selectall") + "</span>,&nbsp;<span onclick=\"" + (Enabled ? "DeselectAllSubelements($j(this), " + id + ", false);" + CallbackRef + ";" : "return false;") + "\" >" + GetString("uiprofile.deselectall") + "</span>)</span>" : "");
            }
            else
            {
                string warning = "";

                if (SiteName != null)
                {
                    if ((ResourceInfoProvider.GetResourceInfo("cms." + elementName) != null) &&
                        !ResourceSiteInfoProvider.IsResourceOnSite("cms." + elementName, SiteName))
                    {
                        warning = "<img style=\"width: 12px; height: 12px; border:none; cursor:help;\" alt=\"warning\" title=\"" + String.Format(GetString("uiprofile.warningmodule"), "cms." + elementName) + "\" src=\"" + GetImageUrl("/Design/Controls/UniGrid/Actions/Warning.png") + "\" />";
                    }
                }

                string icon = "";
                if (!String.IsNullOrEmpty(iconUrl))
                {
                    try
                    {
                        if (ValidationHelper.IsURL(iconUrl) || FileHelper.FileExists(GetImageUrl(iconUrl)))
                        {
                            icon = "<img class=\"Image16\" style=\"border:none;\" alt=\"" + HTMLHelper.HTMLEncode(displayName) + "\" src=\"" + GetImageUrl(iconUrl) + "\" />&nbsp;";
                        }
                        else
                        {
                            icon = "<img class=\"Image16\" style=\"border:none;\" alt=\"" + HTMLHelper.HTMLEncode(displayName) + "\" src=\"" + GetImageUrl("/CMSModules/list.png") + "\" />&nbsp;";
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                nodeText = "<input type=\"checkbox\" id=\"chk_" + id + "\" name=\"chk_" + id + "\" " + (Enabled ? "" : "disabled=\"disabled\" ") + (selected ? "checked=\"checked\"" : "") + " onclick=\"" + (Enabled ? onClickDeclaration + onClickCommon : "return false;") + "\" />" +
                           "<span onclick=\"" + (Enabled ? onClickDeclaration + onClickSpan + onClickCommon : "return false;") + "\" >" + icon + displayName + "</span>" + warning +
                           (childCount > 0 ? "&nbsp;<span class=\"UITreeSelectButton\">(<span onclick=\"" + (Enabled ? "SelectAllSubelements($j(this), " + id + ", true); " + CallbackRef + ";" : "return false;") + "\" >" + GetString("uiprofile.selectall") + "</span>,&nbsp;<span onclick=\"" + (Enabled ? " DeselectAllSubelements($j(this), " + id + ", true);" + CallbackRef + ";" : "return false;") + "\" >" + GetString("uiprofile.deselectall") + "</span>)</span>" : "");
            }

            defaultNode.ToolTip = "";
            defaultNode.Text = nodeText;
        }

        return defaultNode;
    }


    /// <summary>
    /// Reloads the tree data.
    /// </summary>
    public void ReloadData()
    {
        ResourceInfo ri = ResourceInfoProvider.GetResourceInfo(ModuleID);
        if (ri != null)
        {
            // Get the info
            root = UIElementInfoProvider.GetRootUIElementInfo(ri.ResourceName);
            if (root != null)
            {
                // Prepare the parameters
                QueryDataParameters parameters = new QueryDataParameters();
                parameters.Add("@ModuleID", ModuleID);
                parameters.Add("@RoleID", RoleID);

                // Create and set category provider
                UniTreeProvider provider = new UniTreeProvider();
                provider.QueryName = "cms.uielement.selecttree";
                provider.DisplayNameColumn = "ElementName";
                provider.IDColumn = "ElementID";
                provider.LevelColumn = "ElementLevel";
                provider.OrderColumn = "ElementOrder";
                provider.ParentIDColumn = "ElementParentID";
                provider.PathColumn = "ElementIDPath";
                provider.ValueColumn = "ElementID";
                provider.ChildCountColumn = "ElementChildCount";
                provider.Parameters = parameters;

                treeElem.ExpandTooltip = GetString("general.expand");
                treeElem.CollapseTooltip = GetString("general.collapse");
                treeElem.UsePostBack = false;
                treeElem.EnableRootAction = false;
                treeElem.ProviderObject = provider;
                treeElem.ExpandPath = root.ElementIDPath;

                string rootText = HTMLHelper.HTMLEncode(ri.ResourceDisplayName) + "&nbsp;<span class=\"UITreeSelectButton\">(<span onclick=\"" + (Enabled ? "SelectAllSubelements($j(this), " + root.ElementID + ", false); " + CallbackRef + ";" : "return false;") + "\" >" + GetString("uiprofile.selectall") + "</span>,&nbsp;<span onclick=\"" + (Enabled ? "DeselectAllSubelements($j(this), " + root.ElementID + ", false); " + CallbackRef + ";" : "return false;") + "\" >" + GetString("uiprofile.deselectall") + "</span>)</span>";
                treeElem.SetRoot(rootText, root.ElementID.ToString(), ResolveUrl(root.ElementIconPath));

                treeElem.ReloadData();
            }
        }
    }


    /// <summary>
    /// Recursivelly select or deselect all child elements.
    /// </summary>
    /// <param name="select">Determines the type of action</param>
    /// <param name="parentId">ID of the parent UIElement</param>
    /// <param name="excludeRoot">Indicates whether to exclude root element from selection/deselection</param>
    private void SelectDeselectAll(bool select, int parentId, bool excludeRoot)
    {
        // Check manage permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.UIPersonalization", CMSAdminControl.PERMISSION_MODIFY))
        {
            RedirectToAccessDenied("CMS.UIPersonalization", CMSAdminControl.PERMISSION_MODIFY);
        }

        // Get the children and select them
        string where = "ElementIDPath LIKE (SELECT TOP 1 ElementIDPath FROM CMS_UIElement WHERE ElementID = " + parentId + ") + '%' ";
        if (excludeRoot)
        {
            where += " AND NOT ElementID = " + parentId;
        }
        if (!String.IsNullOrEmpty(GroupPreffix))
        {
            where += " AND ElementName NOT LIKE '" + SqlHelperClass.GetSafeQueryString(GroupPreffix, false) + "%'";
        }
        DataSet ds = UIElementInfoProvider.GetUIElements(where, null, 0, "ElementID");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                int id = ValidationHelper.GetInteger(dr["ElementID"], 0);
                if (select)
                {
                    RoleUIElementInfoProvider.AddRoleUIElementInfo(RoleID, id);
                }
                else
                {
                    RoleUIElementInfoProvider.DeleteRoleUIElementInfo(RoleID, id);
                }
            }
        }
    }


    #region "Callback handling"

    public string GetCallbackResult()
    {
        return "";
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
        // Check manage permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.UIPersonalization", CMSAdminControl.PERMISSION_MODIFY))
        {
            return;
        }

        string[] test = eventArgument.Split(';');
        if ((test.Length == 2) || (test.Length == 3))
        {
            if (test.Length == 3)
            {
                bool excludeRoot = !ValidationHelper.GetBoolean(test[2], false);
                if (test[0] == "s")
                {
                    int id = ValidationHelper.GetInteger(test[1], 0);
                    SelectDeselectAll(true, id, excludeRoot);
                }
                else if (test[0] == "d")
                {
                    // Deselect all action
                    int id = ValidationHelper.GetInteger(test[1], 0);
                    SelectDeselectAll(false, id, excludeRoot);
                }
            }
            else if (test.Length == 2)
            {
                // Basic checkbox click
                int id = ValidationHelper.GetInteger(test[0], 0);
                bool chk = ValidationHelper.GetBoolean(test[1], false);

                if (chk)
                {
                    RoleUIElementInfoProvider.AddRoleUIElementInfo(RoleID, id);
                }
                else
                {
                    RoleUIElementInfoProvider.DeleteRoleUIElementInfo(RoleID, id);
                }
            }

            // Invalidate all users
            UserInfo.TYPEINFO.InvalidateAllObjects();
        }
    }

    #endregion
}