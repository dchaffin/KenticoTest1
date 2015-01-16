using System;
using System.Web.UI;

using CMS.DocumentEngine;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

[Security(Resource = "CMS.Content", UIElements = "Properties.MetaData")]
[RegisterTitle("content.ui.propertiesmetadata")]
public partial class CMSModules_Content_CMSDesk_Properties_MetaData : CMSPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Enable split mode
        EnableSplitMode = true;

        // Register the scripts
        ScriptHelper.RegisterProgress(Page);

        // Set tab mode
        UIContext.PropertyTab = PropertyTabEnum.Metadata;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        pnlContent.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        pnlContent.Enabled = DocumentManager.AllowSave;
    }

    #endregion
}