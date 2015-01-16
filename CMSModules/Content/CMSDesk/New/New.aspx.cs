using System;
using System.Data;
using System.Collections;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

[Title(ImageUrl = "CMSModules/CMS_Content/Menu/New.png", ResourceString = "Content.NewTitle")]
[Security("CMS.Content", UIElements = "New")]
public partial class CMSModules_Content_CMSDesk_New_New : CMSContentPage
{
    #region "Variables"

    CMSAbstractNewDocumentControl ctrl = null;

    #endregion


    #region "Page events"

    protected override void CreateChildControls()
    {
        ctrl = this.LoadUserControl("~/CMSModules/Content/Controls/DocTypeSelection.ascx") as CMSAbstractNewDocumentControl;

        if (ctrl == null)
        {
            throw new Exception("Document type selector does not exist.");
        }

        ctrl.ID = "dt";
        plc.Controls.Add(ctrl);

        base.CreateChildControls();
    }


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        DocumentManager.RegisterSaveChangesScript = false;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureChildControls();

        ctrl.SelectionUrl = URLHelper.AppendQuery(ResolveUrl("~/CMSModules/Content/CMSDesk/Edit/Edit.aspx"), URLHelper.Url.Query);
        
        // Create new document by default
        if (string.IsNullOrEmpty(QueryHelper.GetString("action", null)))
        {
            ctrl.SelectionUrl = URLHelper.AddParameterToUrl(ctrl.SelectionUrl, "action", "new");
        }

        if (ModuleEntry.IsModuleLoaded(ModuleEntry.ECOMMERCE))
        {
            string url = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_New.aspx";
            url = URLHelper.AddParameterToUrl(url, "content", "1");
            ctrl.ProductSelectionUrl = URLHelper.AppendQuery(ResolveUrl(url), URLHelper.Url.Query);
        }
        ctrl.NewVariantUrl = URLHelper.AppendQuery(ResolveUrl("~/CMSModules/OnlineMarketing/Pages/Content/ABTesting/ABVariant/NewPage.aspx"), URLHelper.Url.Query);

        // Current Node ID
        ctrl.ParentNodeID = QueryHelper.GetInteger("parentnodeid", 0);
        ctrl.ParentCulture = QueryHelper.GetString("parentculture", null);

        // Handle conversion
        int convertDocumentId = QueryHelper.GetInteger("convertdocumentid", 0);
        if (convertDocumentId > 0)
        {
            ctrl.ConvertDocumentID = convertDocumentId;

            SetTitle(null, GetString("Content.ConvertTitle"), null, null);
            ShowInformation(GetString("Convert.ConvertInfo"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script
        ScriptHelper.RegisterScriptFile(this, "~/CMSModules/Content/CMSDesk/New/New.js");
    }

    #endregion
}