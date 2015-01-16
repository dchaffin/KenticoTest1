using System;

using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.GlobalHelper;

public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Advanced_Frameset : CMSProductsAdvancedPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        const string headerUrl = "Product_Edit_Advanced_Header.aspx";
        advancedHeader.Attributes["src"] = headerUrl + URLHelper.Url.Query;

        // Reverse frames when RTL
        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "refreshtree", ScriptHelper.GetScript("function RefreshTree(expandNodeId, selectNodeId) { parent.RefreshTree(expandNodeId, selectNodeId); } function SelectNode(selectNodeId) { parent.SelectNode(selectNodeId); }"));

        EnsureSplitModeCrossoverScript();
    }
}
