using System;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;

public partial class CMSModules_Content_CMSDesk_Properties_CreateWireframe : CMSContentPage
{
    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        DocumentManager.OnSaveData += DocumentManager_OnSaveData;
        DocumentManager.HandleWorkflow = false;
    }


    protected override void OnInit(EventArgs e)
    {
        // Load the root category of the selector
        DataClassInfo dci = DataClassInfoProvider.GetDataClass("cms.wireframe");
        if (dci != null)
        {
            selTemplate.RootCategory = dci.ClassPageTemplateCategoryID;
            selTemplate.SetDefaultTemplate(dci.ClassDefaultPageTemplateID);
        }

        selTemplate.IsWireframe = true;
        menuElem.ShowSpellCheck = false;

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script files
        ScriptHelper.RegisterProgress(this);
        ScriptHelper.RegisterScriptFile(this, "cmsedit.js");

        // Hide error label
        lblError.Style.Add("display", "none");
    }

    #endregion


    #region "Other events"

    protected void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        TreeNode node = e.Node;

        // Wireframe
        string errorMessage = null;

        // Ensure the template
        node.EnsureGUID();

        PageTemplateInfo pti = selTemplate.EnsureTemplate(node.DocumentName, node.NodeGUID, ref errorMessage);
        
        if (String.IsNullOrEmpty(errorMessage))
        {
            if (pti != null)
            {
                node.NodeWireframeTemplateID = pti.PageTemplateId;
            }

            // Wireframe mode for wireframes
            CMSContext.ViewMode = ViewModeEnum.Wireframe;

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Refresh", ScriptHelper.GetScript(String.Format(
                "RefreshTree({0}, {0}); SelectNode({0});",
                node.NodeID
            )));
        }
        else
        {
            e.IsValid = false;
            e.ErrorMessage = errorMessage;
        }
    }

    #endregion
}
