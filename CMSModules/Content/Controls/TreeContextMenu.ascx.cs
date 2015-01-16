using System;
using System.Data;
using System.Collections;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.LicenseProvider;
using CMS.PortalEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Content_Controls_TreeContextMenu : CMSContextMenuControl
{
    #region "Variables"

    private string mDocumentTypeOrderBy = "MenuItemOrder, ClassDisplayName";

    #endregion


    #region "Properties"

    /// <summary>
    /// Allows to restrict selection of document types in New... submenu.
    /// </summary>
    public string DocumentTypeWhere
    {
        get;
        set;
    }


    /// <summary>
    /// Allows to explicitly order document types in New... submenu.
    /// </summary>
    public string DocumentTypeOrderBy
    {
        get
        {
            return mDocumentTypeOrderBy;
        }
        set
        {
            mDocumentTypeOrderBy = value;
        }
    }


    /// <summary>
    /// Allows to hide Properties menu item.
    /// </summary>
    public bool HidePropertiesItem
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        if (ContextMenu != null)
        {
            ContextMenu.Dynamic = true;
            ContextMenu.OnReloadData += ContextMenu_OnReloadData;
        }

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string loadingContent = "<div class=\"TreeContextMenu TreeNewContextMenu\"><div class=\"ItemPadding\">" + ResHelper.GetString("ContextMenu.Loading") + "</div></div>";
        menuNew.LoadingContent = loadingContent;
        menuProperties.LoadingContent = loadingContent;

        menuNew.OnReloadData += menuNew_OnReloadData;
        repNew.ItemDataBound += repNew_ItemDataBound;

        plcProperties.Visible = !HidePropertiesItem;

        // Main menu
        iNew.ImageUrl = GetImageUrl("CMSModules/CMS_Content/ContextMenu/New.png");
        iNew.Attributes.Add("onclick", "NewItem(GetContextMenuParameter('nodeMenu'), true);");

        iDelete.ImageUrl = GetImageUrl("CMSModules/CMS_Content/ContextMenu/Delete.png");
        iDelete.Attributes.Add("onclick", "DeleteItem(GetContextMenuParameter('nodeMenu'), true);");

        iCopy.ImageUrl = GetImageUrl("CMSModules/CMS_Content/ContextMenu/Copy.png");
        iCopy.Attributes.Add("onclick", "CopyRef(GetContextMenuParameter('nodeMenu'));");

        iMove.ImageUrl = GetImageUrl("CMSModules/CMS_Content/ContextMenu/Move.png");
        iMove.Attributes.Add("onclick", "MoveRef(GetContextMenuParameter('nodeMenu'));");

        iUp.ImageUrl = GetImageUrl("CMSModules/CMS_Content/ContextMenu/Up.png");
        iUp.Attributes.Add("onclick", "MoveUp(GetContextMenuParameter('nodeMenu'));");

        iDown.ImageUrl = GetImageUrl("CMSModules/CMS_Content/ContextMenu/Down.png");
        iDown.Attributes.Add("onclick", "MoveDown(GetContextMenuParameter('nodeMenu'));");

        iSort.ImageUrl = GetImageUrl("CMSModules/CMS_Content/ContextMenu/Sort.png");

        // Refresh subsection
        iRefresh.ImageUrl = GetImageUrl("CMSModules/CMS_Content/ContextMenu/Refresh.png");
        iRefresh.Attributes.Add("onclick", "RefreshTree(GetContextMenuParameter('nodeMenu'), null);");

        // Sort menu
        iAlphaAsc.Attributes.Add("onclick", "SortAlphaAsc(GetContextMenuParameter('nodeMenu'));");
        iAlphaDesc.Attributes.Add("onclick", "SortAlphaDesc(GetContextMenuParameter('nodeMenu'));");
        iDateAsc.Attributes.Add("onclick", "SortDateAsc(GetContextMenuParameter('nodeMenu'));");
        iDateDesc.Attributes.Add("onclick", "SortDateDesc(GetContextMenuParameter('nodeMenu'));");

        // Up menu
        iTop.Text = GetString("UpMenu.IconTop");
        iTop.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Top.png");
        iTop.Attributes.Add("onclick", "MoveTop(GetContextMenuParameter('nodeMenu'));");

        // Down menu
        iBottom.Text = GetString("DownMenu.IconBottom");
        iBottom.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Bottom.png");
        iBottom.Attributes.Add("onclick", "MoveBottom(GetContextMenuParameter('nodeMenu'));");

        // New menu
        iNewLink.ImageUrl = GetImageUrl("CMSModules/CMS_Content/ContextMenu/New/Link.png");
        iNewLink.Attributes.Add("onclick", "LinkRef(GetContextMenuParameter('nodeMenu'));");

        // Variant 
        iNewVariant.ImageUrl = GetImageUrl("objects/cms_variant/object_small.png");
        iNewVariant.Attributes.Add("onclick", "NewVariant(GetContextMenuParameter('nodeMenu'), true);");
    }


    protected void repNew_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Panel pnlItem = (Panel)e.Item.FindControl("pnlItem");
        if (pnlItem != null)
        {
            int count = ValidationHelper.GetInteger(((DataRowView)e.Item.DataItem)["Count"], 0) - 1;
            if (e.Item.ItemIndex == count)
            {
                pnlItem.CssClass = "ItemLast";
            }

            pnlItem.Attributes.Add("onclick", "NewItem(GetContextMenuParameter('nodeMenu'), " + ((DataRowView)e.Item.DataItem)["ClassID"] + ", true);");
        }
    }

    #endregion


    #region "Events of context menus"

    protected void ContextMenu_OnReloadData(object sender, EventArgs e)
    {
        int nodeId = ValidationHelper.GetInteger(ContextMenu.Parameter, 0);

        // Get the node
        TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);
        TreeNode node = tree.SelectSingleNode(nodeId);

        if (node != null)
        {
            // Hide Properties menu item with separator
            plcProperties.Visible = !HidePropertiesItem && !node.IsWireframe();
            if (plcProperties.Visible)
            {
                iProperties.ImageUrl = GetImageUrl("CMSModules/CMS_Content/ContextMenu/Properties.png");
                iProperties.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'general');");

                // Properties menu
                iGeneral.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'general');");
                iUrls.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'urls');");
                iTemplate.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'template');");
                iMetadata.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'metadata');");
                iCategories.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'categories');");
                iMenu.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'menu');");
                iWorkflow.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'workflow');");
                iVersions.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'versions');");
                iRelated.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'relateddocs');");
                iLinked.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'linkeddocs');");
                iSecurity.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'security');");
                iAttachments.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'attachments');");
                iLanguages.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'languages');");
                iWireframe.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'wireframe');");
                iVariants.Attributes.Add("onclick", "Properties(GetContextMenuParameter('nodeMenu'), 'variants');");

                // Hide language item
                if (!CultureInfoProvider.IsSiteMultilignual(CMSContext.CurrentSiteName) || !CultureInfoProvider.LicenseVersionCheck())
                {
                    pnlUILanguages.Visible = false;
                }

                // Hide wireframe tab if wireframe is not present
                if (node.NodeWireframeTemplateID <= 0)
                {
                    pnlUIWireframe.Visible = false;
                }

                if (DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), "") != "")
                {
                    // Check license and whether content personalization is enabled and whether exists at least one variant for current template
                    if (!LicenseHelper.IsFeatureAvailableInUI(FeatureEnum.ContentPersonalization, ModuleEntry.ONLINEMARKETING)
                        || !ResourceSiteInfoProvider.IsResourceOnSite("CMS.ContentPersonalization", CMSContext.CurrentSiteName)
                        || !PortalContext.ContentPersonalizationEnabled
                        || (ModuleCommands.OnlineMarketingGetContentPersonalizationVariantId(node.GetUsedPageTemplateId(), String.Empty) <= 0))
                    {
                        pnlUICPVariants.Visible = false;
                    }
                }

                // No workflow
                if (node.GetWorkflow() == null)
                {
                    // Hide menu items
                    pnlUIWorkflow.Visible = false;
                    pnlUIVersions.Visible = false;
                }
            }
        }
        else
        {
            iNoNode.Visible = true;
            plcFirstLevelContainer.Visible = false;
        }
    }


    protected void menuNew_OnReloadData(object sender, EventArgs e)
    {
        int nodeId = ValidationHelper.GetInteger(menuNew.Parameter, 0);

        // Get the node
        TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);
        TreeNode node = tree.SelectSingleNode(nodeId);

        iNewVariant.Visible = false;

        if (node != null)
        {
            CurrentUserInfo curUser = CMSContext.CurrentUser;
            if (!curUser.IsAuthorizedPerUIElement("CMS.Content", "New"))
            {
                DisplayErrorMessage(String.Format(ResHelper.GetString("CMSSiteManager.AccessDeniedOnUIElementName"), "New"));
                return;
            }

            if (curUser.IsAuthorizedToCreateNewDocument(node, null))
            {
                // Check user permissions for "Create" permission
                bool hasNodeAllowCreate = (curUser.IsAuthorizedPerTreeNode(node, NodePermissionsEnum.Create) == AuthorizationResultEnum.Allowed);
                bool isAuthorizedToCreateInContent = curUser.IsAuthorizedPerResource("CMS.Content", "Create");

                // AB test variant settings
                if (SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSABTestingEnabled")
                    && curUser.IsAuthorizedPerResource("cms.ABTest", "Read")
                    && ModuleEntry.IsModuleLoaded("cms.onlinemarketing")
                    && ResourceSiteInfoProvider.IsResourceOnSite("CMS.ABTest", CMSContext.CurrentSiteName)
                    && LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(),FeatureEnum.ABTesting)
                    && (node.NodeAliasPath != "/"))
                {
                    if (isAuthorizedToCreateInContent || curUser.IsAuthorizedPerClassName(node.NodeClassName, "Create") || (curUser.IsAuthorizedPerClassName(node.NodeClassName, "CreateSpecific") && hasNodeAllowCreate))
                    {
                        iNewVariant.Visible = true;
                        if (!iNewLink.Visible)
                        {
                            pnlNewVariantSeparator.Visible = true;
                        }
                    }
                }

                string where = "ClassID IN (SELECT ChildClassID FROM CMS_AllowedChildClasses WHERE ParentClassID=" + ValidationHelper.GetInteger(node.GetValue("NodeClassID"), 0) + ") " +
                               "AND ClassID IN (SELECT ClassID FROM CMS_ClassSite WHERE SiteID = " + CMSContext.CurrentSiteID + ")";

                if (!string.IsNullOrEmpty(DocumentTypeWhere))
                {
                    where = SqlHelperClass.AddWhereCondition(where, DocumentTypeWhere);
                }

                // Get the allowed child classes
                DataSet ds = DataClassInfoProvider.GetClasses(string.Format("ClassID, ClassName, ClassDisplayName, (CASE WHEN (ClassName = N'{0}' OR ClassName = N'CMS.Wireframe')  THEN 0 ELSE 1 END) AS MenuItemOrder", DataClassInfoProvider.DefaultPageClassName), where, DocumentTypeOrderBy, 50);

                DataTable resultTable = null;
                ArrayList rows = new ArrayList();

                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    resultTable = ds.Tables[0].DefaultView.ToTable();

                    for (int i = 0; i < resultTable.Rows.Count; ++i)
                    {
                        DataRow dr = resultTable.Rows[i];
                        string doc = ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, "ClassName"), "");

                        // Document type is not allowed, remove it from the data set
                        if (!isAuthorizedToCreateInContent && !curUser.IsAuthorizedPerClassName(doc, "Create") && (!curUser.IsAuthorizedPerClassName(doc, "CreateSpecific") || !hasNodeAllowCreate))
                        {
                            rows.Add(dr);
                        }
                        else if (doc.EqualsCSafe("cms.wireframe", true) && !curUser.IsAuthorizedPerResource("CMS.Design", "Wireframing"))
                        {
                            rows.Add(dr);
                        }
                    }

                    // Remove the document types
                    foreach (DataRow dr in rows)
                    {
                        resultTable.Rows.Remove(dr);
                    }

                    bool classesRemoved = false;

                    // Leave only first 15 rows
                    while (resultTable.Rows.Count > 15)
                    {
                        resultTable.Rows.RemoveAt(resultTable.Rows.Count - 1);
                        classesRemoved = true;
                    }

                    if (!DataHelper.DataSourceIsEmpty(resultTable))
                    {
                        // Add show more item
                        if (classesRemoved)
                        {
                            DataRow dr = resultTable.NewRow();
                            dr["ClassID"] = 0;
                            dr["ClassName"] = "more";
                            dr["ClassDisplayName"] = ResHelper.GetString("class.showmore");
                            resultTable.Rows.InsertAt(dr, resultTable.Rows.Count);
                        }

                        // Create temp column
                        int rowCount = resultTable.Rows.Count;
                        DataColumn tmpColumn = new DataColumn("Count");
                        tmpColumn.DefaultValue = rowCount;
                        resultTable.Columns.Add(tmpColumn);
                    }
                    else
                    {
                        DisplayErrorMessage("Content.NoPermissions");
                    }
                }
                else
                {
                    pnlNewVariantSeparator.Visible = true;
                    DisplayErrorMessage("NewMenu.NoChildAllowed");
                }

                repNew.DataSource = resultTable;
                repNew.DataBind();

                if (DataHelper.DataSourceIsEmpty(ds))
                {
                    DisplayErrorMessage("NewMenu.NoChildAllowed");
                }
            }
            else
            {
                DisplayErrorMessage("Content.NoPermissions");
            }
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Displays error message (if any permission is not present)
    /// </summary>
    /// <param name="message">Message to display</param>
    private void DisplayErrorMessage(String message)
    {
        iNewLink.Visible = false;

        pnlSepNewLinked.Visible = false;
        pnlNoChild.Visible = true;

        ltlNoChild.Text = ResHelper.GetString(message);
    }

    #endregion
}