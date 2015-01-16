using System;
using System.Text;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Synchronization;
using CMS.DocumentEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_PortalEngine_Controls_Layout_PlaceholderMenu : CMSAbstractPortalUserControl
{
    protected PageInfo pi = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Use UI culture for strings
        string culture = CMSContext.CurrentUser.PreferredUICultureCode;

        // Register dialog script
        ScriptHelper.RegisterDialogScript(Page);

        // Prepare script to display versions dialog
        StringBuilder script = new StringBuilder();
        script.Append(
            @"
function ShowVersionsDialog(objectType, objectId, objectName) {
  modalDialog('", ResolveUrl("~/CMSModules/Objects/Dialogs/ObjectVersionDialog.aspx"), @"' + '?objecttype=' + objectType + '&objectid=' + objectId + '&objectname=' + objectName,'VersionsDialog','800','600');
}"
            );

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ShowVersionDialog", ScriptHelper.GetScript(script.ToString()));

        // Check if template is ASPX one and initialize PageTemplateInfo variable
        bool isAspx = false;

        PageTemplateInfo pti = null;

        if (mPagePlaceholder != null)
        {
            pi = mPagePlaceholder.PageInfo;
        }

        if (pi != null)
        {
            pti = pi.UsedPageTemplateInfo;
            if (pti != null)
            {
                isAspx = pti.IsAspx;
            }
        }

        bool documentExists = ((pi != null) && (pi.DocumentID > 0));

        if ((mPagePlaceholder != null) && (mPagePlaceholder.ViewMode == ViewModeEnum.DesignDisabled))
        {
            // Hide edit layout and edit template if design mode is disabled
            iLayout.Visible = false;
            iTemplate.Visible = false;
        }
        else
        {
            // Wireframe options
            if (documentExists && PortalHelper.IsWireframingEnabled(CMSContext.CurrentSiteName))
            {
                iSepWireframe.Visible = true;

                if (pi.NodeWireframeTemplateID <= 0)
                {
                    // Create wireframe
                    iWireframe.ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/16/createwireframe.png");
                    iWireframe.Text = ResHelper.GetString("Wireframe.Create", culture);
                    iWireframe.RedirectUrl = String.Format("~/CMSModules/Content/CMSDesk/Properties/CreateWireframe.aspx?nodeid={0}&culture={1}", pi.NodeID, pi.DocumentCulture);
                }
                else
                {
                    // Remove wireframe
                    iWireframe.ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/16/removewireframe.png");
                    iWireframe.Text = ResHelper.GetString("Wireframe.Remove", culture);
                    iWireframe.OnClientClick = "if (!confirm('" + GetString("Wireframe.ConfirmRemove") + "')) return false;";
                    iWireframe.Click += iWireframe_Click;
                }
            }

            if ((mPagePlaceholder != null) && (mPagePlaceholder.LayoutTemplate == null))
            {
                // Edit layout
                iLayout.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Layout.png");
                iLayout.Text = ResHelper.GetString("PlaceholderMenu.IconLayout", culture);
                iLayout.Attributes.Add("onclick", "EditLayout();");

                if ((pti != null) && (pti.LayoutID > 0))
                {
                    LayoutInfo li = LayoutInfoProvider.GetLayoutInfo(pti.LayoutID);

                    // Display layout versions sub-menu
                    if ((li != null) && ObjectVersionManager.DisplayVersionsTab(li))
                    {
                        menuLayout.Visible = true;
                        iLayout.Text = ResHelper.GetString("PlaceholderMenu.IconLayoutMore", culture);
                        lblSharedLayoutVersions.Text = ResHelper.GetString("PlaceholderMenu.SharedLayoutVersions", culture);
                        pnlSharedLayout.Attributes.Add("onclick", GetVersionsDialog(li.ObjectType, li.LayoutId));
                    }
                }
            }
            else
            {
                iLayout.Visible = false;
            }

            if (documentExists)
            {
                // Template properties
                iTemplate.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Template.png");
                iTemplate.Text = ResHelper.GetString("PlaceholderMenu.IconTemplate", culture);
                iTemplate.Attributes.Add("onclick", "EditTemplate(GetContextMenuParameter('pagePlaceholderMenu'));");

                if (pti != null)
                {
                    // Display template versions sub-menu
                    if (ObjectVersionManager.DisplayVersionsTab(pti))
                    {
                        menuTemplate.Visible = true;
                        iTemplate.Text = ResHelper.GetString("PlaceholderMenu.IconTemplateMore", culture);
                        lblTemplateVersions.Text = ResHelper.GetString("PlaceholderMenu.TemplateVersions", culture);
                        pnlTemplateVersions.Attributes.Add("onclick", GetVersionsDialog(pti.ObjectType, pti.PageTemplateId));
                    }
                }
            }
        }

        if (pti != null)
        {
            if ((!isAspx) && documentExists && pti.IsReusable)
            {
                if (!CMSObjectHelper.UseCheckinCheckout || CurrentPageInfo.UsedPageTemplateInfo.Generalized.IsCheckedOutByUser(CMSContext.CurrentUser))
                {
                    iClone.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Clonetemplate.png");
                    iClone.Text = ResHelper.GetString("PlaceholderMenu.IconClone", culture);
                    iClone.OnClientClick = "CloneTemplate(GetContextMenuParameter('pagePlaceholderMenu'));";
                }
            }
            else
            {
                iSaveAsNew.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/saveasnew.png");
                iSaveAsNew.Text = ResHelper.GetString("PageProperties.Save", culture);

                int templateId = pi.UsedPageTemplateInfo.PageTemplateId;
                iSaveAsNew.OnClientClick = String.Format(
                    "modalDialog('{0}?refresh=1&templateId={1}&siteid={2}{3}', 'SaveNewTemplate', 600, 400); return false;",
                    ResolveUrl("~/CMSModules/PortalEngine/UI/Layout/SaveNewPageTemplate.aspx"),
                    templateId,
                    CMSContext.CurrentSiteID,
                    ((CMSContext.ViewMode == ViewModeEnum.Wireframe) ? "&assign=0" : "")
                );
            }
        }

        iRefresh.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Refresh.png");
        iRefresh.Text = ResHelper.GetString("PlaceholderMenu.IconRefresh", culture);
        iRefresh.Attributes.Add("onclick", "RefreshPage();");
    }


    /// <summary>
    /// Remove wireframe event handler
    /// </summary>
    protected void iWireframe_Click(object sender, EventArgs e)
    {
        // Ensure the document node
        if (pi != null)
        {
            DocumentManager.NodeID = pi.NodeID;
            DocumentManager.CultureCode = pi.DocumentCulture;
        }
        TreeNode node = DocumentManager.Node;

        DocumentManager.RemoveWireframe();

        PortalContext.ViewMode = ViewModeEnum.Design;

        ScriptHelper.RegisterStartupScript(this, typeof(string), "Refresh", ScriptHelper.GetScript(String.Format(
            "parent.RefreshTree({0}, {0}); parent.SelectNode({0});",
            node.NodeID
        )));
    }


    /// <summary>
    /// Gets javascript to open modal versions dialog.
    /// </summary>
    /// <param name="objType">Object type</param>
    /// <param name="objId">ID of the object</param>
    private string GetVersionsDialog(string objType, int objId)
    {
        string url = ResolveUrl("~/CMSModules/Objects/Dialogs/ObjectVersionDialog.aspx?objecttype=" + objType + "&objectid=" + objId);
        return "modalDialog('" + URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url)) + "','VersionsDialog','900','600');return false;";
    }
}