using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

using CMS.CMSHelper;
using CMS.Controls.Configuration;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DocumentEngine;

public partial class CMSModules_Widgets_Dialogs_WidgetProperties_Header : CMSDeskPage
{
    #region "Variables"

    protected string widgetId = QueryHelper.GetString("widgetid", "");
    protected string widgetName = QueryHelper.GetString("widgetname", "");
    protected string aliasPath = QueryHelper.GetString("aliasPath", "");
    protected int templateId = QueryHelper.GetInteger("templateid", 0);
    protected Guid instanceGuid = QueryHelper.GetGuid("instanceguid", Guid.Empty);
    protected VariantModeEnum variantMode = VariantModeEnum.None;
    protected int variantId = QueryHelper.GetInteger("variantid", 0);
    private bool isInline = false;

    #endregion


    /// <summary>
    /// OnInit override - do not require site.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        RequireSite = false;
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        CMSDialogHelper.RegisterDialogHelper(Page);
        ScriptHelper.RegisterScriptFile(Page, "Dialogs/HTMLEditor.js");
        SetBrowserClass();

        ScriptHelper.RegisterWOpenerScript(Page);

        // Ensure the design mode for the dialog
        if (String.IsNullOrEmpty(aliasPath))
        {
            PortalContext.SetRequestViewMode(ViewModeEnum.Design);
        }

        isInline = QueryHelper.GetBoolean("inline", false);
        bool isNew = QueryHelper.GetBoolean("isnew", false);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ScriptHelper.NEWWINDOW_SCRIPT_KEY, ScriptHelper.NewWindowScript);

        // initialize page title
        PageTitle.TitleText = GetString("Widgets.Properties.Title");
        PageTitle.TitleImage = GetImageUrl("CMSModules/CMS_PortalEngine/Widgetproperties.png");

        if (!RequestHelper.IsPostBack())
        {
            InitalizeMenu();
        }

        // If inline edit register postback for get widget data (from JS editor)
        if (isInline && !isNew)
        {
            if (!RequestHelper.IsPostBack())
            {
                ltlScript.Text = ScriptHelper.GetScript("function DoHiddenPostback(){" + Page.ClientScript.GetPostBackEventReference(btnHidden, "") + "}");
                ltlScript.Text += ScriptHelper.GetScript("GetSelected('" + hdnSelected.ClientID + "');");
            }
        }

        // Show the Tabs element when displaying a widget variant
        if (variantId > 0)
        {
            pnlTabs.Visible = true;
        }
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (pnlTabs.Visible)
        {
            tabsElem.DoTabSelection();
        }
    }


    /// <summary>
    /// Initializes menu.
    /// </summary>
    protected void InitalizeMenu()
    {
        string zoneId = QueryHelper.GetString("zoneid", "");
        string culture = QueryHelper.GetString("culture", CMSContext.PreferredCultureCode);
        bool isNewWidget = QueryHelper.GetBoolean("isnew", false);
        WidgetZoneTypeEnum zoneType = WidgetZoneTypeEnum.None;

        if (!String.IsNullOrEmpty(widgetId) || !String.IsNullOrEmpty(widgetName))
        {
            WidgetInfo wi = null;

            // get pageinfo
            PageInfo pi = null;
            try
            {
                pi = CMSWebPartPropertiesPage.GetPageInfo(aliasPath, templateId, culture);
            }
            catch (PageNotFoundException)
            {
                // Do not throw exception if page info not found (e.g. bad alias path)
            }

            if (pi == null)
            {
                Visible = false;
                return;
            }

            // Get template instance
            PageTemplateInstance templateInstance = CMSPortalManager.GetTemplateInstanceForEditing(pi);

            if (templateInstance != null)
            {
                // Get zone type
                WebPartZoneInstance zoneInstance = templateInstance.GetZone(zoneId);

                if (zoneInstance != null)
                {
                    zoneType = zoneInstance.WidgetZoneType;
                }
                if (!isNewWidget)
                {
                    // Get web part
                    WebPartInstance widget = templateInstance.GetWebPart(instanceGuid, widgetId);

                    if ((widget != null) && widget.IsWidget)
                    {
                        // WebPartType = codename, get widget by codename 
                        wi = WidgetInfoProvider.GetWidgetInfo(widget.WebPartType);

                        // Set the variant mode (MVT/Content personalization)
                        variantMode = widget.VariantMode;
                    }
                }
            }
            // New widget
            if (isNewWidget)
            {
                int id = ValidationHelper.GetInteger(widgetId, 0);
                if (id > 0)
                {
                    wi = WidgetInfoProvider.GetWidgetInfo(id);
                }
                else if (!String.IsNullOrEmpty(widgetName))
                {
                    wi = WidgetInfoProvider.GetWidgetInfo(widgetName);
                }
            }

            // Get widget info from name if not found yet
            if ((wi == null) && (!String.IsNullOrEmpty(widgetName)))
            {
                wi = WidgetInfoProvider.GetWidgetInfo(widgetName);
            }

            if (wi != null)
            {
                PageTitle.TitleText = GetString("Widgets.Properties.Title") + " (" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(wi.WidgetDisplayName)) + ")";
            }

            // Use live or non live dialogs
            string documentationUrl = String.Empty;

            // If no zonetype defined or not inline dont show documentation 
            switch (zoneType)
            {
                case WidgetZoneTypeEnum.Dashboard:
                case WidgetZoneTypeEnum.Editor:
                case WidgetZoneTypeEnum.Group:
                case WidgetZoneTypeEnum.User:
                    documentationUrl = ResolveUrl("~/CMSModules/Widgets/Dialogs/WidgetDocumentation.aspx");
                    break;

                // If no zone set dont create documentation link
                default:
                    if (isInline)
                    {
                        documentationUrl = ResolveUrl("~/CMSModules/Widgets/Dialogs/WidgetDocumentation.aspx");
                    }
                    else
                    {
                        return;
                    }
                    break;
            }

            // Generate documentation link
            Literal ltr = new Literal();
            PageTitle.RightPlaceHolder.Controls.Add(ltr);

            // Ensure correct parameters in documentation url
            documentationUrl += URLHelper.GetQuery(URLHelper.CurrentURL);
            if (!String.IsNullOrEmpty(widgetName))
            {
                documentationUrl = URLHelper.UpdateParameterInUrl(documentationUrl, "widgetname", widgetName);
            }
            if (!String.IsNullOrEmpty(widgetId))
            {
                documentationUrl = URLHelper.UpdateParameterInUrl(documentationUrl, "widgetid", widgetId);
            }
            string docScript = "NewWindow('" + documentationUrl + "', 'WebPartPropertiesDocumentation', 800, 800); return false;";

            ltr.Text += "<a onclick=\"" + docScript + "\" href=\"#\"><img src=\"" + ResolveUrl(GetImageUrl("General/HelpLargeDark.png")) + "\" style=\"border-width: 0px;\"></a>";

            tabsElem.OnTabCreated += tabElem_OnTabCreated;
            tabsElem.UrlTarget = "widgetpropertiescontent";
            tabsElem.OpenTabContentAfterLoad = !isInline;
        }
    }


    /// <summary>
    /// Tabs the elem_ on tab created.
    /// </summary>
    protected TabItem tabElem_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        // Show/hide context help for some tabs
        String script = "SetTabsContext('');";
        String defaultParam = tab.OnClientClick;

        switch (element.ElementName.ToLowerCSafe())
        {
            case "widgetproperties.variant":
                script = "SetTabsContext('variants');";
                String varName = (variantMode == VariantModeEnum.MVT) ? "mvtvariant_edit" : "cpvariant_edit";
                defaultParam = "if (window.SetHelpTopic) { window.SetHelpTopic('helpTopic', '" + varName + "')}";

                bool isNewVariant = QueryHelper.GetBoolean("isnewvariant", false);
                int variantId = QueryHelper.GetInteger("variantid", 0);

                // Set the variant mode into the tab url
                if ((variantMode != VariantModeEnum.None) && !string.IsNullOrEmpty(tab.RedirectUrl))
                {
                    tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "variantmode", VariantModeFunctions.GetVariantModeString(variantMode));
                }

                if ((variantId <= 0) || isNewVariant)
                {
                    return null;
                }
                break;
        }

        tab.OnClientClick = script + defaultParam;
        return tab;
    }


    /// <summary>
    /// Save widget state to definition.
    /// </summary>
    protected void btnHidden_Click(object sender, EventArgs e)
    {
        string value = HttpUtility.UrlDecode(hdnSelected.Value);
        if (!String.IsNullOrEmpty(value))
        {
            // Parse defininiton 
            Hashtable parameters = CMSDialogHelper.GetHashTableFromString(value);
            // Trim control name
            if (parameters["name"] != null)
            {
                widgetName = parameters["name"].ToString();
            }

            InitalizeMenu();

            SessionHelper.SetValue("WidgetDefinition", value);
        }

        string dialogUrl = "~/CMSModules/Widgets/Dialogs/widgetproperties_properties_frameset.aspx" + URLHelper.Url.Query;
        ltlScript.Text = ScriptHelper.GetScript("if (window.parent.frames['widgetpropertiescontent']) { window.parent.frames['widgetpropertiescontent'].location= '" + ResolveUrl(dialogUrl) + "';} ");
    }
}