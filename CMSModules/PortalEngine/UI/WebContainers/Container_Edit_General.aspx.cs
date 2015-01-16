using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebContainers_Container_Edit_General : CMSModalDesignPage
{
    #region "Variables"

    private bool dialogMode;
    private bool tabMode;

    private WebPartContainerInfo webPartContainer;

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;

        dialogMode = QueryHelper.GetBoolean("editonlycode", false);
        tabMode = QueryHelper.GetBoolean("tabmode", false);

        if (dialogMode)
        {
            // Check hash
            if (!QueryHelper.ValidateHash("hash", "saved;containerid;name;selectorid;tabmode;aliaspath;instanceGUID", true, true, true))
            {
                URLHelper.Redirect(ResolveUrl(string.Format("~/CMSMessages/Error.aspx?title={0}&text={1}", ResHelper.GetString("dialogs.badhashtitle"), ResHelper.GetString("dialogs.badhashtext"))));
            }

            // Check permissions for web part properties UI
            if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "Design", "Design.WebPartProperties", "WebPartProperties.General", "WebPartProperties.EditContainers" }, CMSContext.CurrentSiteName))
            {
                RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Design.WebPartProperties;WebPartProperties.General;WebPartProperties.EditContainers");
            }
        }
        else
        {
            CheckAccessToSiteManager();
        }

        var containerId = QueryHelper.GetInteger("containerid", 0);
        webPartContainer = WebPartContainerInfoProvider.GetWebPartContainerInfo(containerId);

        if (webPartContainer == null)
        {
            string containerName = QueryHelper.GetString("name", null);
            webPartContainer = WebPartContainerInfoProvider.GetWebPartContainerInfo(containerName);
        }

        string frameSetUrl = dialogMode ? "Container_Edit_Frameset.aspx" : null;
        SetEditedObject(webPartContainer, frameSetUrl);

        base.OnPreInit(e);
    }


    protected override void CreateChildControls()
    {
        if (webPartContainer != null)
        {
            Guid instanceGuid = QueryHelper.GetGuid("instanceguid", Guid.Empty);
            ucHierarchy.DialogMode = dialogMode;

            CMSContext.EditedObject = webPartContainer;
            ucHierarchy.PreviewObjectName = webPartContainer.ContainerName;
            ucHierarchy.ShowPanelSeparator = !dialogMode || (dialogMode && tabMode);
            ucHierarchy.IgnoreSessionValues = dialogMode;

            String containerName = webPartContainer != null ? webPartContainer.ContainerName : "";
            String parameter = instanceGuid != Guid.Empty ? "&previewguid=" + instanceGuid : "&previewobjectidentifier=" + containerName;
            ucHierarchy.PreviewURLSuffix = parameter;
        }

        base.CreateChildControls();
    }

    #endregion
}
