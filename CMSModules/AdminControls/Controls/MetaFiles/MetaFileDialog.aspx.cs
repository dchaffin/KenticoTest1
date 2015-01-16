using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("Objects/CMS_MetaFile/attachment.png", "general.attachments", "metafile_attachments", HelpName = "helpTopic")]
public partial class CMSModules_AdminControls_Controls_MetaFiles_MetaFileDialog : CMSToolsModalPage
{
    #region "Properties"

    protected int objectId = -1;
    protected string mObjectType;
    protected string mObjectCategory;


    /// <summary>
    /// Object ID
    /// </summary>
    protected int ObjectId
    {
        get
        {
            if (objectId < 0)
            {
                objectId = QueryHelper.GetInteger("objectid", 0);
            }
            return objectId;
        }
    }


    /// <summary>
    /// Metafile object type
    /// </summary>
    protected string ObjectType
    {
        get
        {
            if (string.IsNullOrEmpty(mObjectType))
            {
                mObjectType = QueryHelper.GetString("objecttype", string.Empty);
            }
            return mObjectType;
        }
    }


    /// <summary>
    /// Metafile category
    /// </summary>
    protected string ObjectCategory
    {
        get
        {
            if (string.IsNullOrEmpty(mObjectCategory))
            {
                mObjectCategory = QueryHelper.GetString("category", string.Empty);
            }
            return mObjectCategory;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        CurrentUserInfo user = CMSContext.CurrentUser;

        // Check the license and site availability for Newsletters
        switch (ObjectType)
        {
            case PredefinedObjectType.NEWSLETTERISSUE:
            case PredefinedObjectType.NEWSLETTERISSUEVARIANT:
            case PredefinedObjectType.NEWSLETTERTEMPLATE:
                {
                    // Check the license
                    if (!string.IsNullOrEmpty(DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), string.Empty)))
                    {
                        LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.Newsletters);
                    }

                    // Check site availability
                    if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Newsletter", CMSContext.CurrentSiteName))
                    {
                        RedirectToResourceNotAvailableOnSite("CMS.Newsletter");
                    }

                    // Check UI permissions for CMS Desk -> Tools -> Newsletter
                    if (!user.IsAuthorizedPerUIElement("CMS.Tools", "Newsletter"))
                    {
                        RedirectToCMSDeskUIElementAccessDenied("CMS.Tools", "Newsletter");
                    }
                }
                break;
        }

        // Check permissions
        switch (ObjectType)
        {
            case PredefinedObjectType.NEWSLETTERISSUE:
            case PredefinedObjectType.NEWSLETTERISSUEVARIANT:
                // Check 'AuthorIssues' permission
                if (!user.IsAuthorizedPerResource("CMS.Newsletter", "AuthorIssues"))
                {
                    RedirectToCMSDeskAccessDenied("CMS.Newsletter", "AuthorIssues");
                }
                break;
            case PredefinedObjectType.NEWSLETTERTEMPLATE:
                // Check 'Managetemplates' permission
                if (!user.IsAuthorizedPerResource("CMS.Newsletter", "managetemplates"))
                {
                    RedirectToCMSDeskAccessDenied("CMS.Newsletter", "managetemplates");
                }
                break;
            case PredefinedObjectType.BIZFORM:
                // Check 'EditForm' permission
                if (!user.IsAuthorizedPerResource("cms.form", "EditForm"))
                {
                    RedirectToCMSDeskAccessDenied("cms.form", "EditForm");
                }
                break;
            case PredefinedObjectType.EMAILTEMPLATE:
                // Check "Modify" permission
                if (!user.IsAuthorizedPerResource("CMS.EmailTemplates", "Modify"))
                {
                    RedirectToAccessDenied("CMS.EmailTemplates", "Modify");
                }
                break;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash", "category"))
        {
            // Get site ID, if 0 then global object files are required
            int siteId = QueryHelper.GetInteger("siteid", -1);
            if (siteId < 0)
            {
                // If not set use current site ID
                siteId = CMSContext.CurrentSiteID;
            }

            // Initialize attachment list
            AttachmentList.Visible = true;
            AttachmentList.ObjectID = ObjectId;
            if (siteId > 0)
            {
                AttachmentList.SiteID = siteId;
            }
            AttachmentList.AllowPasteAttachments = QueryHelper.GetBoolean("allowpaste", true);
            AttachmentList.ObjectType = ObjectType;
            AttachmentList.Category = ObjectCategory;
            AttachmentList.ItemsPerPage = 10;
            AttachmentList.OnAfterDelete += AttachmentList_OnAfterChange;
            AttachmentList.OnAfterUpload += AttachmentList_OnAfterChange;
            AttachmentList.IsLiveSite = false;
            AttachmentList.UploadMode = MultifileUploaderModeEnum.DirectMultiple;
        }
        else
        {
            AttachmentList.StopProcessing = true;
            AttachmentList.Visible = false;

            // Redirect to error page
            string url = ResolveUrl("~/CMSMessages/Error.aspx?title=" + GetString("dialogs.badhashtitle") + "&text=" + GetString("dialogs.badhashtext") + "&cancel=1");
            ScriptHelper.RegisterStartupScript(this, typeof(string), "BadHashRedirection", string.Format("window.location = '{0}';", url), true);
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!AttachmentList.StopProcessing)
        {
            // Register 'paste' script
            string script = @"
function UpdateAttachmentCount() {
    var hdnCount = document.getElementById('" + hdnCount.ClientID + @"');
    if((wopener!=null)&&(hdnCount!=null)&&(hdnCount.value >= 0)&&(wopener.UpdateAttachmentCount)){wopener.UpdateAttachmentCount(hdnCount.value);}
}
function PasteImage(imageurl) {
    UpdateAttachmentCount();    
    if((wopener!=null)&&(wopener.PasteImage)){wopener.PasteImage(imageurl);}
    return CloseDialog();
}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "PasteImageScript", script, true);
        }
    }

    #endregion


    #region "Other events"

    protected void AttachmentList_OnAfterChange(object sender, EventArgs e)
    {
        // Get number of attachments
        InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(ObjectId, ObjectType, ObjectCategory, null, null, "MetafileID", -1);

        // Register script to update hdnCount value (it is used to update attachment count in wopener)
        ScriptHelper.RegisterStartupScript(this, typeof(string), "UpdateCountHolder", "document.getElementById('" + hdnCount.ClientID + "').value=" + ds.Items.Count.ToString(), true);
    }

    #endregion
}