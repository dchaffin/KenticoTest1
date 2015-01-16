using System;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_CssStylesheets_Pages_CssStylesheet_General : CMSModalPage
{
    #region "Variables"

    protected int cssStylesheetId = 0;
    private CssStylesheetInfo si = null;
    private SiteInfo mSite = null;
    private bool requiresDialog = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Site info object, used for test whether css sheet belongs to site
    /// </summary>
    private SiteInfo CurrentSite
    {
        get
        {
            if (mSite == null)
            {
                int siteId = QueryHelper.GetInteger("siteid", 0);
                if (siteId > 0)
                {
                    mSite = SiteInfoProvider.GetSiteInfo(siteId);
                }
                if (mSite == null)
                {
                    mSite = CMSContext.CurrentSite;
                }
            }
            return mSite;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;

        // Check for UI permissions
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "Properties", "Properties.General", "General.Design", "Design.EditCSSStylesheets" }, CMSContext.CurrentSiteName))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Properties;Properties.General;General.Design;Design.EditCSSStylesheets");
        }

        // Page has been opened in CMSDesk and only stylesheet style editing is allowed
        requiresDialog = QueryHelper.GetBoolean("editonlycode", false);

        if (requiresDialog)
        {
            // Check hash
            if (!QueryHelper.ValidateHash("hash", "saved;cssstylesheetid;selectorid;tabmode;siteid;aliaspath;line", true, true, true))
            {
                URLHelper.Redirect(ResolveUrl(string.Format("~/CMSMessages/Error.aspx?title={0}&text={1}", ResHelper.GetString("dialogs.badhashtitle"), ResHelper.GetString("dialogs.badhashtext"))));
            }

            // Check 'Design Web site' permission if opened from CMS Desk
            if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Design", "Design"))
            {
                RedirectToCMSDeskAccessDenied("CMS.Design", "Design");
            }
        }
        else
        {
            CheckAccessToSiteManager();
        }

        string stylesheet = QueryHelper.GetString("cssstylesheetid", "0");

        // If default stylesheet defined and selected, choose it
        if (stylesheet == "default")
        {
            si = CMSContext.CurrentSiteStylesheet;
        }

        // Default stylesheet not selected try to find stylesheet selected
        if (si != null)
        {
            cssStylesheetId = si.StylesheetID;
        }
        else
        {
            cssStylesheetId = ValidationHelper.GetInteger(stylesheet, 0);
            if (cssStylesheetId > 0)
            {
                // Get the stylesheet
                si = CssStylesheetInfoProvider.GetCssStylesheetInfo(cssStylesheetId);
            }
        }

        SetEditedObject(si, "CssStylesheet_Edit.aspx");

        var checkSiteAssociation = QueryHelper.GetBoolean("checksiteassociation", true);

        if ((si != null) && requiresDialog && checkSiteAssociation)
        {
            // Check if stylesheet is under current site
            int siteId = (CurrentSite != null) ? CurrentSite.SiteID : 0;
            string where = SqlHelperClass.AddWhereCondition("SiteID = " + siteId, "StylesheetID = " + si.StylesheetID);
            DataSet ds = CssStylesheetSiteInfoProvider.GetCssStylesheetSites("StylesheetID", where, null, 1);
            if (DataHelper.DataSourceIsEmpty(ds))
            {
                URLHelper.Redirect(ResolveUrl(string.Format("~/CMSMessages/Error.aspx?title={0}&text={1}", ResHelper.GetString("cssstylesheet.errorediting"), ResHelper.GetString("cssstylesheet.notallowedtoedit"))));
            }
        }

        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register client ID for autoresize of codemirror
        ucHierarchy.RegisterEnvelopeClientID();
    }


    protected override void CreateChildControls()
    {
        ScriptHelper.RegisterWOpenerScript(Page);

        if (si != null)
        {
            CMSContext.EditedObject = si;
            ucHierarchy.PreviewObjectName = si.StylesheetName;
        }

        ucHierarchy.IgnoreSessionValues = requiresDialog;
        ucHierarchy.DialogMode = requiresDialog;
        ucHierarchy.ShowPanelSeparator = true;
        ucHierarchy.StorePreviewScrollPosition = true;
        ucHierarchy.DefaultPreviewPath = "/";

        base.CreateChildControls();
    }

    #endregion
}
