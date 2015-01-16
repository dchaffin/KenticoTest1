using System;
using System.Web.UI;

using CMS.DocumentEngine;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

[Security(Resource = "CMS.Content", UIElements = "Properties.Wireframe")]
[RegisterTitle("content.ui.wireframe")]
public partial class CMSModules_Content_CMSDesk_Properties_Wireframe : CMSPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register the scripts
        ScriptHelper.RegisterProgress(Page);

        // Set tab mode
        UIContext.PropertyTab = PropertyTabEnum.Wireframe;

        wfElem.EditMenu = menuElem;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        pnlContent.Enabled = !DocumentManager.ProcessingAction;
    }

    #endregion
}