using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Widgets_LiveDialogs_WidgetDocumentation : CMSLiveModalPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("WebPartDocumentDialog.Documentation");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_PortalEngine/Documentation.png");
        CurrentMaster.Title.TitleCssClass = "WebPartDocumentationHeader";
        ucWebPartDocumentation.PageTemplateID = QueryHelper.GetInteger("templateID", 0);
        ucWebPartDocumentation.AliasPath = QueryHelper.GetString("aliaspath", String.Empty);
        ucWebPartDocumentation.CultureCode = QueryHelper.GetString("culture", CMSContext.PreferredCultureCode);
        ucWebPartDocumentation.IsInline = QueryHelper.GetBoolean("Inline", false);
        ucWebPartDocumentation.ZoneID = QueryHelper.GetString("zoneid", String.Empty);
        ucWebPartDocumentation.IsNew = QueryHelper.GetBoolean("isNew", false);
        ucWebPartDocumentation.WidgetID = QueryHelper.GetString("widgetId", String.Empty);
        ucWebPartDocumentation.WebpartID = QueryHelper.GetString("webPartId", String.Empty);
        ucWebPartDocumentation.InstanceGUID = QueryHelper.GetGuid("instanceGuid", Guid.Empty);
        ucWebPartDocumentation.DashboardName = QueryHelper.GetString("dashboard", String.Empty);
        ucWebPartDocumentation.DashboardSiteName = QueryHelper.GetString("sitename", String.Empty);
    }
}