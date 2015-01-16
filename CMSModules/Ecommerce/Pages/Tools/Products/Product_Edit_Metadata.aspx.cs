using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

[RegisterTitle("com.ui.productsmetadata")]
[Security(Resource = "CMS.Ecommerce", UIElements = "Products.Metadata")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Metadata : CMSProductPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Enable split mode
        EnableSplitMode = true;

        // Register the scripts
        ScriptHelper.RegisterProgress(Page);

        // Mark selected tab
        UIContext.ProductTab = ProductTabEnum.Metadata;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // If modify is allowed
        metaDataElem.Enabled = DocumentManager.AllowSave;
    }

    #endregion
}
