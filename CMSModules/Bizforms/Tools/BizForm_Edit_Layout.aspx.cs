using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_BizForms_Tools_BizForm_Edit_Layout : CMSBizFormPage
{
    protected int bizFormId = 0;
    protected HeaderAction attachments = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = string.Empty;

        // Check 'ReadForm' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.form", "ReadForm"))
        {
            RedirectToCMSDeskAccessDenied("cms.form", "ReadForm");
        }

        bizFormId = QueryHelper.GetInteger("formid", 0);
        layoutElem.FormType = CMSModules_AdminControls_Controls_Class_Layout.FORMTYPE_BIZFORM;
        layoutElem.ObjectID = bizFormId;
        layoutElem.ObjectType = FormObjectType.BIZFORM;
        layoutElem.ObjectCategory = MetaFileInfoProvider.OBJECT_CATEGORY_LAYOUT;
        layoutElem.IsLiveSite = false;
        layoutElem.OnBeforeSave += layoutElem_OnBeforeSave;
        layoutElem.OnAfterSave += layoutElem_OnAfterSave;

        // Load CSS style sheet to editor area
        if (CMSContext.CurrentSite != null)
        {
            int cssId = CMSContext.CurrentSite.SiteDefaultEditorStylesheet;
            if (cssId == 0) // Use default site CSS if none editor CSS is specified
            {
                cssId = CMSContext.CurrentSite.SiteDefaultStylesheetID;
            }
            layoutElem.CssStyleSheetID = cssId;
        }

        BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(bizFormId);
        EditedObject = bfi;
    }


    private void layoutElem_OnAfterSave(object sender, EventArgs e)
    {
        BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(bizFormId);
        
        // Delete all attachments if layout was deleted
        if (bfi != null)
        {
            DataClassInfo dci = DataClassInfoProvider.GetDataClass(bfi.FormClassID);
            if ((dci != null) && string.IsNullOrEmpty(dci.ClassFormLayout))
            {
                // Delete all attachments
                MetaFileInfoProvider.DeleteFiles(bfi.FormID, FormObjectType.BIZFORM, MetaFileInfoProvider.OBJECT_CATEGORY_LAYOUT);
            }
        }

        // Log synchronization
        SynchronizationHelper.LogObjectChange(bfi, TaskTypeEnum.UpdateObject);
    }


    private void layoutElem_OnBeforeSave(object sender, EventArgs e)
    {
        // Check 'EditForm' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.form", "EditForm"))
        {
            RedirectToCMSDeskAccessDenied("cms.form", "EditForm");
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Add attachment button to header actions
        InitHeaderActions();

        if (layoutElem.CustomLayoutEnabled)
        {
            // Register paste attachment script
            string script =
@"// Pasting image URL to CKEditor - requires other function 'InsertHTML' -  see Layout control
function PasteImage(imageurl) {
    imageurl = '<img src=""' + imageurl + '"" />';
    return InsertHTML(imageurl);
}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "PasteScript_" + ClientID, script, true);
        }
        else
        {
            attachments.Enabled = false;
        }
    }


    /// <summary>
    /// Initializes header actions.
    /// </summary>
    protected void InitHeaderActions()
    {
        bool isAuthorized = CurrentUser.IsAuthorizedPerResource("cms.form", "EditForm");

        int attachCount = 0;
        if (isAuthorized)
        {
            // Get number of attachments
            InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(bizFormId, FormObjectType.BIZFORM, MetaFileInfoProvider.OBJECT_CATEGORY_LAYOUT, null, null, "MetafileID", -1);
            attachCount = ds.Items.Count;

            string script = @"
function UpdateAttachmentCount(count) {
    var counter = document.getElementById('attachmentCount');
    if (counter != null) {
        if (count > 0) { counter.innerHTML = ' (' + count + ')'; }
        else { counter.innerHTML = ''; }
    }
}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UpdateAttachmentScript_" + this.ClientID, script, true);

            // Register dialog scripts
            ScriptHelper.RegisterDialogScript(Page);
        }

        // Prepare metafile dialog URL
        string metaFileDialogUrl = ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
        string query = string.Format("?objectid={0}&objecttype={1}", bizFormId, FormObjectType.BIZFORM);
        metaFileDialogUrl += string.Format("{0}&category={1}&hash={2}", query, MetaFileInfoProvider.OBJECT_CATEGORY_LAYOUT, QueryHelper.GetHash(query));

        // Init attachment button
        attachments = new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.LinkButton,
            Text = GetString("general.attachments") + string.Format("<span id='attachmentCount'>{0}</span>", (attachCount > 0) ? " (" + attachCount.ToString() + ")" : string.Empty),
            Tooltip = GetString("general.attachments"),
            OnClientClick = string.Format(@"if (modalDialog) {{modalDialog('{0}', 'Attachments', '700', '500');}}", metaFileDialogUrl) + " return false;",
            ImageUrl = isAuthorized ? GetImageUrl("Objects/CMS_MetaFile/attachment.png") : GetImageUrl("Objects/CMS_MetaFile/attachment_disabled.png"),
            Enabled = isAuthorized
        };
        layoutElem.HeaderActions.ActionsList.Add(attachments);
    }
}