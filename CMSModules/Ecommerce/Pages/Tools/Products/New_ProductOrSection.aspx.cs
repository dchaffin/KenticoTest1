using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.UIControls;

[Title("CMSModules/CMS_Content/Menu/New.png", "com.newproductorsection", "new_product_or_section")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_New_ProductOrSection : CMSProductsPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get parameters
        int parentNodeId = QueryHelper.GetInteger("parentnodeid", 0);
        string parentCulture = QueryHelper.GetString("parentculture", null);

        // Init section type selector
        SectionTypes.SelectionUrl = ResolveUrl("~/CMSModules/Content/CMSDesk/Edit/Edit.aspx?action=new&mode=productssection");
        SectionTypes.NoDataMessage = GetString("com.products.nosectiontypeallowed");
        SectionTypes.Caption = GetString("com.createsection");
        SectionTypes.ParentNodeID = parentNodeId;
        SectionTypes.ParentCulture = parentCulture;
        SectionTypes.ClientTypeClick = "parent.parent.SetSelectedMode('sectionedit');";
        SectionTypes.Grid.Pager.ShowPageSize = false;

        // Init product type selector
        ProductTypes.ProductSelectionUrl = ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/Products/Product_New.aspx");
        ProductTypes.SelectionUrl = ProductTypes.ProductSelectionUrl;
        ProductTypes.NoDataMessage = GetString("com.products.noproducttypeallowed");
        ProductTypes.Caption = GetString("com.createproduct");
        ProductTypes.ParentNodeID = parentNodeId;
        ProductTypes.ParentCulture = parentCulture;
        ProductTypes.Grid.Pager.ShowPageSize = false;

        // Init new link 
        imgNewLink.ImageUrl = GetImageUrl("CMSModules/CMS_Content/Menu/Link.png");
        lnkNewLink.NavigateUrl = "javascript:modalDialog('" + GetLinkDialogUrl(parentNodeId) + "', 'contentselectnode', '90%', '85%')";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script
        ScriptHelper.RegisterJQuery(this);
        ScriptHelper.RegisterScriptFile(this, "~/CMSModules/Content/CMSDesk/New/New.js");
        ScriptHelper.RegisterScriptFile(this, "jquery/jquery-equalheight.js");

        ScriptHelper.RegisterStartupScript(this, typeof(string), "EqualHeight", ScriptHelper.GetScript("$j('div:has(.UniGridGrid):not(:has(.UniGridMain))').equalHeight();"));

        // Create breadcrumbs
        CurrentMaster.Title.Breadcrumbs = CreateNodeBreadcrumbs(ProductTypes.ParentNode, GetString("com.newproductorsection"));
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Show message when no choice
        if ((SectionTypes.ClassesCount == 0) && (!SectionTypes.Grid.FilterIsSet) && (ProductTypes.ClassesCount == 0) && (!ProductTypes.Grid.FilterIsSet))
        {
            pnlProductOrSectionType.Visible = false;
            ShowError(GetString("new.noallowedchild"));
        }
        else if ((SectionTypes.ClassesCount < 0) || (ProductTypes.ClassesCount < 0))
        {
            // Show message when no permission
            pnlProductOrSectionType.Visible = false;
            ShowError(GetString("Content.NoPermissions"));
        }
    }


    /// <summary>
    /// Returns Correct URL of the link dialog.
    /// </summary>
    /// <param name="currentNodeId">ID Of the node to be copied or moved</param>
    private string GetLinkDialogUrl(int nodeId)
    {
        DialogConfiguration config = GetDocumentDialogConfig("linkdoc", null);
        string url = CMSDialogHelper.GetDialogUrl(config, false, false, null, false);

        // Prepare url for link dialog
        url = URLHelper.RemoveParameterFromUrl(url, "hash");
        url = URLHelper.AddParameterToUrl(url, "sourcenodeids", nodeId.ToString());
        url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url));
        url = URLHelper.EncodeQueryString(url);

        return url;
    }
}
