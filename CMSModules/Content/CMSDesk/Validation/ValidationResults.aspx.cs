using System;
using System.Data;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.DocumentEngine;
using CMS.UIControls;

public partial class CMSModules_Content_CMSDesk_Validation_ValidationResults : CMSPage
{
    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash"))
        {
            RedirectToAccessDenied(ResHelper.GetString("dialogs.badhashtitle"));
        }

        // Set CSS classes
        CurrentMaster.PanelFooter.CssClass = "FloatRight";

        // Add close button
        CurrentMaster.PanelFooter.Controls.Add(new LocalizedButton
                                                   {
                                                       ID = "btnClose",
                                                       ResourceString = "general.close",
                                                       EnableViewState = false,
                                                       OnClientClick = "return CloseDialog();",
                                                       CssClass = "SubmitButton"
                                                   });
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int documentId = QueryHelper.GetInteger("docid", 0);
        string titlePart = null;
        if (documentId > 0)
        {
            TreeNode doc = DocumentHelper.GetDocument(documentId, null);
            if (doc != null)
            {
                titlePart = HTMLHelper.HTMLEncode(doc.GetDocumentName());
            }
        }

        SetTitle("Design/Controls/Validation/warning.png", String.Format(GetString("validation.validationdialogresults"), titlePart), null, null);

        string key = QueryHelper.GetString("datakey", null);
        if (!String.IsNullOrEmpty(key))
        {
            viewDataSet.AdditionalContent = "";
            DataSet ds = WindowHelper.GetItem(key) as DataSet;
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                viewDataSet.DataSet = ds;
            }
        }

        ScriptHelper.RegisterDialogScript(Page);
        RegisterModalPageScripts();
    }

    #endregion
}