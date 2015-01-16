using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections.Generic;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.PortalControls;
using CMS.FormEngine;

public partial class CMSModules_Widgets_Dialogs_WidgetSelector : CMSModalPage, ICallbackEventHandler
{
    #region "Variables"

    private string callbackResult = string.Empty;

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Public user is not allowed for widgets
        if (!CMSContext.CurrentUser.IsAuthenticated())
        {
            RedirectToAccessDenied(GetString("widgets.security.notallowed"));
        }

        string aliasPath = QueryHelper.GetString("aliaspath", "");
        selectElem.AliasPath = aliasPath;
        selectElem.CultureCode = QueryHelper.GetString("culture", CMSContext.PreferredCultureCode);
        selectElem.PageTemplateId = QueryHelper.GetInteger("templateid", 0);
        selectElem.ZoneId = QueryHelper.GetString("zoneid", "");
        selectElem.ZoneType = WidgetZoneTypeCode.ToEnum(QueryHelper.GetString("zonetype", ""));

        // Ensure the design mode for the dialog
        if (String.IsNullOrEmpty(aliasPath))
        {
            PortalContext.SetRequestViewMode(ViewModeEnum.Design);
        }

        bool isInline = QueryHelper.GetBoolean("inline", false);
        selectElem.IsInline = isInline;

        ScriptHelper.RegisterWOpenerScript(Page);

        btnOk.OnClientClick = "SelectCurrentWidget(); return false;";

        // Base tag is added in master page
        AddBaseTag = false;

        // Proceeds the current item selection
        string javascript = @"
function SelectCurrentWidget() 
{                
    SelectWidget(selectedValue, selectedSkipDialog);
}
function SelectWidget(value, skipDialog)
{
    if ((value != null) && (value != ''))
    {";

        if (isInline)
        {
            javascript += @"
        if (skipDialog) {
            AddInlineWidgetWithoutDialog(value);
        }
        else {
            var editor = wopener.currentEditor || wopener.CMSPlugin.currentEditor;
            if (editor) {
                editor.getCommand('InsertWidget').open(value);
            }
            refreshPageOnClose = false;
            CloseDialog();
        }";
        }
        else
        {
            javascript += @"
	    if (wopener.OnSelectWidget)
        {                    
            wopener.OnSelectWidget(value, skipDialog);                      
        }

        CloseDialog();";
        }

        javascript += @"  
	}
	else
	{
        alert(document.getElementById('" + hdnMessage.ClientID + @"').value);		    
	}                
}            
// Cancel action
function Cancel()
{
    CloseDialog();
} ";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "WidgetSelector", ScriptHelper.GetScript(javascript));
        selectElem.SelectFunction = "SelectWidget";
        selectElem.IsLiveSite = false;

        // Set the title and icon
        CurrentMaster.Title.TitleText = GetString("widgets.selectortitle");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Widget/object.png");

        // Remove default css class
        if (CurrentMaster.PanelBody != null)
        {
            Panel pnl = CurrentMaster.PanelBody.FindControl("pnlContent") as Panel;
            if (pnl != null)
            {
                pnl.CssClass = String.Empty;
            }
        }

        // Register scripts for inline widgets with the property 'Skip initial configuration' set (insert widget without configuration dialog)
        string script = @"
// Inline widgets
function AddInlineWidgetWithoutDialog(widgetId) {
    " + Page.ClientScript.GetCallbackEventReference(this, "widgetId", "OnReceiveAddInlineWidgetScript", null) + @";
    }

function OnReceiveAddInlineWidgetScript(rvalue, context) {
    if ((rvalue != null) && (rvalue != '')) {
        setTimeout(rvalue, 1);
    }
}";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "inlinewidgets11", script, true);
    }

    #endregion


    #region "Callback handling"

    /// <summary>
    /// Raises the callback event.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        if ((eventArgument == null))
        {
            return;
        }

        int widgetId = ValidationHelper.GetInteger(eventArgument, 0);
        if (widgetId > 0)
        {
            // Get the insert widget script
            callbackResult = AddInlineWidgetWithoutDialog(widgetId);
        }
    }


    /// <summary>
    /// Prepares the callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        return callbackResult;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Adds the inline widget without the properties dialog.
    /// </summary>
    /// <param name="widgetId">The widget id</param>
    private string AddInlineWidgetWithoutDialog(int widgetId)
    {
        string script = string.Empty;

        if (widgetId > 0)
        {
            // New widget - load widget info by id
            WidgetInfo wi = WidgetInfoProvider.GetWidgetInfo(widgetId);

            if ((wi != null) && wi.WidgetForInline)
            {
                // Test permission for user
                CurrentUserInfo currentUser = CMSContext.CurrentUser;
                if (!WidgetRoleInfoProvider.IsWidgetAllowed(widgetId, currentUser.UserID, currentUser.IsAuthenticated()))
                {
                    return string.Empty;
                }

                // If user is editor, more properties are shown
                WidgetZoneTypeEnum zoneType = WidgetZoneTypeEnum.User;
                if (currentUser.IsEditor)
                {
                    zoneType = WidgetZoneTypeEnum.Editor;
                }

                WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(wi.WidgetWebPartID);

                // Get the properties xml according to the zone type
                FormInfo zoneTypeDefinition = PortalFormHelper.GetPositionFormInfo(zoneType);

                // Merge the parent web part properties definition with the widget properties definition
                string widgetProperties = FormHelper.MergeFormDefinitions(wpi.WebPartProperties, wi.WidgetProperties);

                // Create the FormInfo for the current widget properties definition
                FormInfo fi = PortalFormHelper.GetWidgetFormInfo(wi.WidgetName, Enum.GetName(typeof(WidgetZoneTypeEnum), zoneType), widgetProperties, zoneTypeDefinition, true);
                DataRow dr = null;

                if (fi != null)
                {
                    // Check if there are some editable properties
                    List<FormFieldInfo> mFields = fi.GetFields(true, true);

                    // Get data rows with required columns
                    dr = PortalHelper.CombineWithDefaultValues(fi, wi);

                    // Load default values for new widget
                    fi.LoadDefaultValues(dr, FormResolveTypeEnum.Visible);

                    // Override default value and set title as widget display name
                    DataHelper.SetDataRowValue(dr, "WidgetTitle", wi.WidgetDisplayName);
                }

                // Save inline widget script
                script = PortalHelper.GetAddInlineWidgetScript(wi, dr, fi.GetFields(true, true));

                script += @"
refreshPageOnClose = false; 
CloseDialog();";

                if (!string.IsNullOrEmpty(script))
                {
                    // Add to recently used widgets collection
                    CMSContext.CurrentUser.UserSettings.UpdateRecentlyUsedWidget(wi.WidgetName);
                }
            }
        }

        return script;
    }

    #endregion
}