using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_Edit : CMSModalPage
{
    #region "Variables"

    private bool dialogMode;
    private TransformationInfo mTransInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Transformation info object
    /// </summary>
    public TransformationInfo TransInfo
    {
        get
        {
            if (mTransInfo == null)
            {
                mTransInfo = TransformationInfoProvider.GetTransformation(QueryHelper.GetInteger("transformationid", 0));
                if (mTransInfo == null)
                {
                    mTransInfo = TransformationInfoProvider.GetTransformation(QueryHelper.GetString("name", ""));
                }
            }
            return mTransInfo;
        }
        set
        {
            mTransInfo = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Page has been opened from CMSDesk
        dialogMode = QueryHelper.GetBoolean("editonlycode", false);

        if (dialogMode)
        {
            // Check hash
            if (!QueryHelper.ValidateHash("hash", "saved;name;selectorid;sitemanager;selectedvalue;tabmode;aliaspath;instanceguid;splitmode", true, true, true))
            {
                URLHelper.Redirect(ResolveUrl(string.Format("~/CMSMessages/Error.aspx?title={0}&text={1}", GetString("dialogs.badhashtitle"), GetString("dialogs.badhashtext"))));
            }
        }
        else
        {
            CheckAccessToSiteManager();
        }
    }


    protected override void CreateChildControls()
    {
        ucHierarchy.DialogMode = dialogMode;
        ucHierarchy.IgnoreSessionValues = dialogMode;
        ucHierarchy.ShowPanelSeparator = true;

        if (TransInfo != null)
        {
            CMSContext.EditedObject = TransInfo;
            Guid instanceGUID = QueryHelper.GetGuid("instanceguid", Guid.Empty);
            ucHierarchy.PreviewURLSuffix = String.Format("&previewguid={0}&previewobjectidentifier={1}", instanceGUID, TransInfo.TransformationFullName);
            ucHierarchy.PreviewObjectName = TransInfo.TransformationFullName;
        }

        base.CreateChildControls();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (dialogMode)
        {
            ScriptHelper.RegisterWOpenerScript(Page);
        }

        // Check permissions for web part properties UI

        // Check for UI permissions
        if (!CurrentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "Design", "Design.WebPartProperties", "WebPartProperties.General", "WebPartProperties.EditTransformations" }, CMSContext.CurrentSiteName))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Design.WebPartProperties;WebPartProperties.General;WebPartProperties.EditTransformations");
        }

        // In case of opening via web part uni selector and transformation is hierarchical - redirect to hierarchical transformation
        string transName = QueryHelper.GetString("name", string.Empty);

        TransformationInfo hti = TransformationInfoProvider.GetTransformation(transName);
        if (hti != null)
        {
            if (hti.TransformationIsHierarchical)
            {
                URLHelper.Redirect(ResolveUrl(string.Format("~/CMSModules/DocumentTypes/Pages/Development/HierarchicalTransformations_Transformations.aspx?transID={0}&editonlycode={1}&aliaspath={2}&instanceguid={3}", hti.TransformationID, QueryHelper.GetInteger("editonlycode", 0),
                        QueryHelper.GetString("aliaspath", String.Empty), QueryHelper.GetGuid("instanceguid", Guid.Empty))));
            }
        }
    }

    #endregion
}