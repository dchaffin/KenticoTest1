using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.URLRewritingEngine;

public partial class CMSWebParts_General_AdminActions : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Indicates whether on-site editing is preferred for document editing if is available
    /// </summary>
    public bool PreferOnSiteEdit
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("PreferOnSiteEdit"), true);
        }
        set
        {
            SetValue("PreferOnSiteEdit", value);
        }
    }


    /// <summary>
    /// Display only to global administrator.
    /// </summary>
    public bool DisplayOnlyToGlobalAdministrator
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayOnlyToGlobalAdministrator"), false);
        }
        set
        {
            SetValue("DisplayOnlyToGlobalAdministrator", value);
        }
    }


    /// <summary>
    /// Check permissions.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), false);
        }
        set
        {
            SetValue("CheckPermissions", value);
        }
    }


    /// <summary>
    /// Separator.
    /// </summary>
    public string Separator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Separator"), null);
        }
        set
        {
            SetValue("Separator", value);
        }
    }


    /// <summary>
    /// Show cms desk link.
    /// </summary>
    public bool ShowCMSDeskLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCMSDeskLink"), true);
        }
        set
        {
            SetValue("ShowCMSDeskLink", value);
        }
    }


    /// <summary>
    /// CMS Desk link text.
    /// </summary>
    public string CMSDeskLinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CMSDeskLinkText"), "CMS Desk");
        }
        set
        {
            SetValue("CMSDeskLinkText", value);
        }
    }


    /// <summary>
    /// CMS Desk text.
    /// </summary>
    public string CMSDeskText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CMSDeskText"), "{0}");
        }
        set
        {
            SetValue("CMSDeskText", value);
        }
    }


    /// <summary>
    /// Show site manager link.
    /// </summary>
    public bool ShowCMSSiteManagerLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCMSSiteManagerLink"), true);
        }
        set
        {
            SetValue("ShowCMSSiteManagerLink", value);
        }
    }


    /// <summary>
    /// Site manager link text.
    /// </summary>
    public string CMSSiteManagerLinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CMSSiteManagerLinkText"), "Site manager");
        }
        set
        {
            SetValue("CMSSiteManagerLinkText", value);
        }
    }


    /// <summary>
    /// Site manager text.
    /// </summary>
    public string CMSSiteManagerText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CMSSiteManagerText"), "{0}");
        }
        set
        {
            SetValue("CMSSiteManagerText", value);
        }
    }


    /// <summary>
    /// Show edit document link.
    /// </summary>
    public bool ShowEditDocumentLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowEditDocumentLink"), false);
        }
        set
        {
            SetValue("ShowEditDocumentLink", value);
        }
    }


    /// <summary>
    /// Edit document link text.
    /// </summary>
    public string EditDocumentLinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EditDocumentLinkText"), "Edit document");
        }
        set
        {
            SetValue("EditDocumentLinkText", value);
        }
    }


    /// <summary>
    /// Edit document text.
    /// </summary>
    public string EditDocumentText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EditDocumentText"), "{0}");
        }
        set
        {
            SetValue("EditDocumentText", value);
        }
    }


    /// <summary>
    /// Default user name for logon page.
    /// </summary>
    public string DefaultUserName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultUserName"), null);
        }
        set
        {
            SetValue("DefaultUserName", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            CurrentUserInfo uinfo = CMSContext.CurrentUser;

            if (uinfo.IsGlobalAdministrator || !DisplayOnlyToGlobalAdministrator)
            {
                // Create new string builder for links
                StringBuilder sb = new StringBuilder();

                // Store current site name
                string curSiteName = CMSContext.CurrentSiteName;
                // Get default user name
                string queryStringKey = (string.IsNullOrEmpty(DefaultUserName)) ? null : "?username=" + DefaultUserName;
                bool separatorNeeded = false;

                // If cms desk link is shown
                if (ShowCMSDeskLink && (!CheckPermissions || uinfo.CheckEditor(curSiteName)))
                {
                    string url = ResolveUrl("~/cmsdesk/default.aspx");
                    if (!string.IsNullOrEmpty(DefaultUserName))
                    {
                        url = URLHelper.AddParameterToUrl(url, "username", DefaultUserName);
                    }
                    if (CMSContext.CurrentPageInfo != null)
                    {
                        url = URLHelper.AddParameterToUrl(url, "nodeid", CMSContext.CurrentPageInfo.NodeID.ToString());
                    }
                    sb.AppendFormat(CMSDeskText, string.Concat("<a href=\"", URLHelper.EncodeQueryString(url), "\">", CMSDeskLinkText, "</a>"));

                    separatorNeeded = true;
                }

                // If site manager link is shown
                if (ShowCMSSiteManagerLink && (!CheckPermissions || uinfo.UserSiteManagerAdmin))
                {
                    // Check if separator needed
                    if (separatorNeeded)
                    {
                        sb.Append(Separator);
                    }

                    string url = ResolveUrl("~/cmssitemanager/default.aspx");
                    if (!string.IsNullOrEmpty(DefaultUserName))
                    {
                        url = URLHelper.AddParameterToUrl(url, "username", DefaultUserName);
                    }
                    sb.AppendFormat(CMSSiteManagerText, string.Concat("<a href=\"", url, "\">", CMSSiteManagerLinkText, "</a>"));

                    separatorNeeded = true;
                }

                // If edit document link is shown
                if (ShowEditDocumentLink && (!CheckPermissions || (uinfo.CheckEditor(curSiteName) && CMSPage.IsUserAuthorizedPerContent() && (uinfo.IsAuthorizedPerDocument(CMSContext.CurrentDocument, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed))))
                {
                    // Check if separator needed
                    if (separatorNeeded)
                    {
                        sb.Append(" " + Separator + " ");
                    }

                    string url = URLHelper.EncodeQueryString(UIHelper.GetDocumentEditUrl(CMSContext.CurrentDocument.NodeID, CMSContext.CurrentDocumentCulture.CultureCode));
                    if (PreferOnSiteEdit && PortalHelper.IsOnSiteEditingEnabled(CurrentSiteName))
                    {
                         url = URLHelper.ResolveUrl(PortalHelper.OnSiteEditRelativeURL);

                        string retUrl = URLHelper.CurrentURL;
                        // handle default alias path
                        if ((ViewMode == CMS.PortalEngine.ViewModeEnum.LiveSite) && (URLRewriter.CurrentPageInfoSource == PageInfoSource.DefaultAliasPath))
                        {
                            string aliasPath = PageInfoProvider.GetDefaultAliasPath(URLHelper.GetCurrentDomain(), CMSContext.CurrentSiteName);
                            if (!String.IsNullOrEmpty(aliasPath))
                            {
                                string query = URLHelper.GetQuery(retUrl);
                                retUrl = URLHelper.ResolveUrl(DocumentURLProvider.GetUrl(aliasPath));
                                retUrl = URLHelper.AppendQuery(retUrl, query);
                            }
                        }

                        url = URLHelper.UpdateParameterInUrl(url, "returnurl", HTMLHelper.HTMLEncode(HttpUtility.UrlEncode(retUrl)));
                    }

                    sb.AppendFormat(EditDocumentText, string.Concat("<a href=\"", url, "\">", EditDocumentLinkText, "</a>"));
                }

                ltlAdminActions.Text = sb.ToString();
            }
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}