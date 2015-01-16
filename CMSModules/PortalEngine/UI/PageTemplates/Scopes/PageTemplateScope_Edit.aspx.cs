using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_PageTemplates_Scopes_PageTemplateScope_Edit : CMSEditTemplatePage
{
    #region "Variables"

    private int scopeID = 0;
    private int siteID = 0;
    private int cultureId = 0;
    private int classId = 0;
    private PageTemplateScopeInfo ptsi = null;

    #endregion


    #region "Page and controls events"

    /// <summary>
    /// Page load event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CMSContext.CurrentUser.UserSiteManagerAdmin)
        {
            RedirectToAccessDenied(GetString("template.scopes.denied"));
        }

        // Show changes saved message
        if (QueryHelper.GetInteger("saved", 0) == 1 && !URLHelper.IsPostback())
        {
            ShowChangesSaved();
        }

        // Get template id, scope id and site id
        scopeID = QueryHelper.GetInteger("scopeid", 0);
        siteID = QueryHelper.GetInteger("siteid", 0);

        // Get sitename
        string siteName = string.Empty;
        if (siteID > 0)
        {
            SiteInfo site = SiteInfoProvider.GetSiteInfo(siteID);
            if (site != null)
            {
                siteName = " (" + site.DisplayName + ")";
            }
        }

        // Breakcrumbs initialization        		
        string[,] pageTitleTabs = new string[2,3];
        pageTitleTabs[0, 0] = GetString("template.scopes");
        pageTitleTabs[0, 1] = "~/CMSModules/PortalEngine/UI/PageTemplates/Scopes/PageTemplateScopes_List.aspx?siteid=" + siteID + "&templateid=" + PageTemplateID;
        pageTitleTabs[0, 2] = "";
        pageTitleTabs[1, 0] = GetString("template.scopes.new") + siteName;
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";

        // Help topic
        CurrentMaster.Title.HelpTopicName = "page_templates_scopes_edit";


        // Set up controls
        cultureElem.CurrentSelector.AllowEmpty = false;
        cultureElem.CurrentSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
        cultureElem.ReturnColumnName = "CultureID";
        cultureElem.DisplayAllValue = true;
        cultureElem.SiteID = -1;

        classElem.DisplayAllValue = true;

        if (scopeID > 0)
        {
            // Get scope info
            PageTemplateScopeInfo ptsi = PageTemplateScopeInfoProvider.GetPageTemplateScopeInfo(scopeID);
            if (ptsi != null)
            {
                cultureId = ptsi.PageTemplateScopeCultureID;
                classId = ptsi.PageTemplateScopeClassID;
                pageTitleTabs[1, 0] = ptsi.PageTemplateScopePath + siteName;

                // Load fields
                if (!RequestHelper.IsPostBack())
                {
                    pathElem.Value = ptsi.PageTemplateScopePath;
                    classElem.Value = ptsi.PageTemplateScopeClassID;
                    cultureElem.Value = ptsi.PageTemplateScopeCultureID;
                    levelElem.Value = ptsi.PageTemplateScopeLevels;
                }
            }
        }
        else if (!RequestHelper.IsPostBack())
        {
            pathElem.Value = "/";
        }

        // Select for site scope only available cultures
        if (siteID > 0)
        {
            cultureElem.CurrentSelector.WhereCondition = "CultureID IN (SELECT CultureID FROM CMS_SiteCulture WHERE SiteID = " + siteID + ") OR CultureID = " + cultureId;
            classElem.WhereCondition = "(ClassID IN (SELECT ClassID FROM CMS_ClassSite  WHERE SiteID = " + siteID + ") OR ClassID = " + classId + ")";
        }

        // Breadcrumbs
        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
    }


    /// <summary>
    /// Button OK click event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Validation
        string path = ValidationHelper.GetString(pathElem.Value, "");
        if (string.IsNullOrEmpty(path))
        {
            ShowError("template.scopes.emptypath");
            return;
        }

        // Get scope if needed
        if (scopeID > 0)
        {
            if (ptsi == null)
            {
                ptsi = PageTemplateScopeInfoProvider.GetPageTemplateScopeInfo(scopeID);
            }
        }

        // Create info if needed
        if (ptsi == null)
        {
            ptsi = new PageTemplateScopeInfo();
        }

        // Update database

        // Class
        int classID = ValidationHelper.GetInteger(classElem.Value, 0);
        // No class selected or selected '(all)'
        if (classID <= 0)
        {
            ptsi.SetValue("PageTemplateScopeClassID", null);
        }
        else
        {
            ptsi.PageTemplateScopeClassID = classID;
        }

        // Culture
        int cultureID = ValidationHelper.GetInteger(cultureElem.Value, 0);
        if (cultureID == 0)
        {
            ptsi.SetValue("PageTemplateScopeCultureID", null);
        }
        else
        {
            ptsi.PageTemplateScopeCultureID = cultureID;
        }

        // Levels
        string levels = ValidationHelper.GetString(levelElem.Value, "");
        if (string.IsNullOrEmpty(levels))
        {
            ptsi.SetValue("PageTemplateScopeLevels", null);
        }
        else
        {
            ptsi.PageTemplateScopeLevels = levels;
        }

        // Other columns       
        ptsi.PageTemplateScopePath = ValidationHelper.GetString(pathElem.Value, "");
        ptsi.PageTemplateScopeTemplateID = PageTemplateID;

        if (ptsi.PageTemplateScopeID == 0)
        {
            // Site
            if (siteID != 0)
            {
                ptsi.PageTemplateScopeSiteID = siteID;
            }
        }

        // Insert or update
        PageTemplateScopeInfoProvider.SetPageTemplateScopeInfo(ptsi);

        // Redirect
        string url = URLHelper.CurrentURL;
        url = URLHelper.UpdateParameterInUrl(url, "scopeid", ptsi.PageTemplateScopeID.ToString());
        url = URLHelper.AddParameterToUrl(url, "saved", "1");
        URLHelper.Redirect(ResolveUrl(url));
    }

    #endregion
}