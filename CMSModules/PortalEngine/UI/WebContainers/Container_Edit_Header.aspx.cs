using System;

using CMS.GlobalHelper;
using CMS.IO;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebContainers_Container_Edit_Header : CMSModalDesignPage
{
    #region "Variables"

    private bool dialogMode = false;
    private int containerId = 0;
    private string containerName = null;

    private WebPartContainerInfo webPartContainer = null;

    #endregion


    #region "Page methods"

    protected override void OnPreInit(EventArgs e)
    {
        dialogMode = QueryHelper.GetBoolean("editonlycode", false);
        containerId = QueryHelper.GetInteger("containerid", 0);
        containerName = QueryHelper.GetString("name", null);

        RequireSite = false;

        if (dialogMode)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/TabsHeader.master";
        }
        else
        {
            CheckAccessToSiteManager();
        }

        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        InitEditeObject();
        InitMasterPage();
    }


    private void InitEditeObject()
    {
        if (PersistentEditedObject != null)
        {
            webPartContainer = PersistentEditedObject as WebPartContainerInfo;
            return;
        }

        if (containerId > 0)
        {
            webPartContainer = WebPartContainerInfoProvider.GetWebPartContainerInfo(containerId);
        }
        else
        {
            webPartContainer = WebPartContainerInfoProvider.GetWebPartContainerInfo(containerName);
        }

        PersistentEditedObject = webPartContainer;
    }


    private void InitMasterPage()
    {
        if (dialogMode)
        {
            SetTitle("Objects/CMS_WebPartContainer/object.png", GetString("container_edit.editcontainer"), "newedit_container", "helpTopic");

            string selector = QueryHelper.GetString("selectorid", string.Empty);
            if (!string.IsNullOrEmpty(selector) && RequestHelper.IsPostBack())
            {
                ScriptHelper.RegisterWOpenerScript(this);

                // Add selector update script
                string updateScript = string.Format(@"if (wopener && wopener.US_SelectNewValue_{0}) {{ wopener.US_SelectNewValue_{0}('{1}'); }}", selector, webPartContainer.ContainerName);
                ScriptHelper.RegisterStartupScript(this, GetType(), "UpdateSelector", updateScript, true);
            }
        }
        else
        {
            SetTitle("Objects/CMS_WebPartContainer/object.png", GetString("container_edit.headercaption"), "newedit_container", "helpTopic");

            // Set breadcrumbs
            InitBreadcrumbs(2);
            SetBreadcrumb(0, GetString("container_edit.itemlistlink"), ResolveUrl("Container_List.aspx"), "_parent", null);
            SetBreadcrumb(1, webPartContainer.ContainerDisplayName, null, null, null);
        }

        // Set tabs
        InitTabs("content");
        int i = 0;

        string url = URLHelper.RemoveParameterFromUrl("Container_Edit_General.aspx" + URLHelper.Url.Query, "saved");
        if (containerId == 0)
        {
            url = URLHelper.AddParameterToUrl(url, "name", webPartContainer.ContainerName);
        }

        SetTab(i++, GetString("general.general"), url, "SetHelpTopic('helpTopic', 'newedit_container');");

        if (!dialogMode)
        {
            if (!StorageHelper.IsExternalStorage(webPartContainer.GetThemePath()))
            {
                SetTab(i++, GetString("stylesheet.theme"), "Container_Edit_Theme.aspx?containerId=" + webPartContainer.ContainerID, "SetHelpTopic('helpTopic', 'webpartcontainer_theme_tab');");
            }

            SetTab(i++, GetString("general.sites"), "Container_Edit_Sites.aspx?containerId=" + webPartContainer.ContainerID, "SetHelpTopic('helpTopic', 'webpartcontainer_sites_tab');");
        }
    }

    #endregion
}
