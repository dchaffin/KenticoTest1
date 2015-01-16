using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.DocumentEngine;
using CMS.UIControls;

public partial class CMSModules_Widgets_LiveDialogs_WidgetProperties : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Public user is not allowed for widgets
        if (!CMSContext.CurrentUser.IsAuthenticated())
        {
            RedirectToAccessDenied(GetString("widgets.security.notallowed"));
        }

        string widgetId = QueryHelper.GetString("widgetid", String.Empty);
        string aliasPath = QueryHelper.GetString("aliasPath", String.Empty);
        string zoneId = QueryHelper.GetString("zoneid", String.Empty);
        Guid instanceGUID = QueryHelper.GetGuid("instanceguid", Guid.Empty);
        bool isNewWidget = QueryHelper.GetBoolean("isnew", false);
        bool inline = QueryHelper.GetBoolean("inline", false);

        // Set page title 
        Page.Title = GetString(isNewWidget ? "widgets.propertiespage.titlenew" : "widgets.propertiespage.title");

        if ((widgetId != string.Empty) && (aliasPath != string.Empty))
        {
            // Get pageinfo
            PageInfo pi = null;
            try
            {
                pi = PageInfoProvider.GetPageInfo(CMSContext.CurrentSiteName, aliasPath, CMSContext.PreferredCultureCode, null, CMSContext.CurrentSite.CombineWithDefaultCulture);
            }
            catch (PageNotFoundException)
            {
                // Do not throw exception if page info not found (e.g. bad alias path)
            }

            if (pi == null)
            {
                return;
            }

            // Get template instance
            PageTemplateInstance templateInstance = CMSPortalManager.GetTemplateInstanceForEditing(pi);

            // Get widget from instance
            WidgetInfo wi = null;
            if (!isNewWidget)
            {
                // Get the instance of widget
                WebPartInstance widgetInstance = templateInstance.GetWebPart(instanceGUID, widgetId);
                if (widgetInstance == null)
                {
                    return;
                }

                // Get widget info by widget name(widget type)
                wi = WidgetInfoProvider.GetWidgetInfo(widgetInstance.WebPartType);
            }
            // Widget instance hasn't created yet
            else
            {
                wi = WidgetInfoProvider.GetWidgetInfo(ValidationHelper.GetInteger(widgetId, 0));
            }


            if (wi != null)
            {
                WebPartZoneInstance zone = templateInstance.GetZone(zoneId);
                if (zone != null)
                {
                    CurrentUserInfo currentUser = CMSContext.CurrentUser;

                    bool checkSecurity = true;

                    // Check security
                    // It is group zone type but widget is not allowed in group
                    if (zone.WidgetZoneType == WidgetZoneTypeEnum.Group)
                    {
                        // Should always be, only group widget are allowed in group zone
                        if (wi.WidgetForGroup)
                        {
                            if (!currentUser.IsGroupAdministrator(pi.NodeGroupID))
                            {
                                RedirectToAccessDenied(GetString("widgets.security.notallowed"));
                            }

                            // All ok, don't check classic security
                            checkSecurity = false;
                        }
                    }

                    if (checkSecurity && !WidgetRoleInfoProvider.IsWidgetAllowed(wi, currentUser.UserID, currentUser.IsAuthenticated()))
                    {
                        RedirectToAccessDenied(GetString("widgets.security.notallowed"));
                    }
                }
            }
        }
        // If all ok, set up frames
        frameHeader.Attributes.Add("src", "widgetproperties_header.aspx" + URLHelper.Url.Query);
        if (inline && !isNewWidget)
        {
            frameContent.Attributes.Add("src", ResolveUrl("~/CMSPages/Blank.htm"));
        }
        else
        {
            frameContent.Attributes.Add("src", "widgetproperties_properties_frameset.aspx" + URLHelper.Url.Query);
        }
    }
}