using System;
using System.Data;
using System.Xml;

using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.URLRewritingEngine;
using CMS.PortalControls;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Properties : SiteManagerPage
{
    private int webPartId;
    private WebPartInfo wi;


    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "WebPart Edit - Properties";
        CurrentMaster.BodyClass += " FieldEditorBody";

        // get webpart ID
        webPartId = QueryHelper.GetInteger("webpartid", 0);
        FieldEditor.Visible = false;

        // If saved is found in query string
        if (!RequestHelper.IsPostBack() && (QueryHelper.GetInteger("saved", 0) == 1))
        {
            ShowChangesSaved();
        }

        if (webPartId > 0)
        {
            // Get web part info
            wi = WebPartInfoProvider.GetWebPartInfo(webPartId);
            EditedObject = wi;

            // Check if info object exists
            if (wi != null)
            {
                // For inherited webpart display DefaultValue Editor
                if (wi.WebPartParentID > 0)
                {
                    FieldEditor.Visible = false;
                    pnlDefaultEditor.Visible = true;
                    defaultValueEditor.Visible = true;
                    defaultValueEditor.ParentWebPartID = wi.WebPartParentID;
                    if (wi.WebPartParentID > 0)
                    {
                        defaultValueEditor.DefaultValueXMLDefinition = wi.WebPartDefaultValues;
                    }
                    else
                    {
                        defaultValueEditor.DefaultValueXMLDefinition = wi.WebPartProperties;
                    }
                    // Add handler to
                    defaultValueEditor.XMLCreated += new EventHandler(defaultValueEditor_XMLCreated);
                }
                else
                {
                    // Set field editor                
                    FieldEditor.WebPartId = webPartId;
                    FieldEditor.Mode = FieldEditorModeEnum.WebPartProperties;
                    FieldEditor.Visible = true;
                    pnlDefaultEditor.Visible = false;

                    // Check newly created field for widgets update
                    FieldEditor.OnFieldCreated += UpdateWidgetsDefinition;
                }
            }
        }
    }


    /// <summary>
    /// Handles OnFieldCreated action and updates form definition of all widgets based on current webpart.
    /// Newly created field is set to be disabled in widget definition for security reasons.
    /// </summary>
    /// <param name="newField">Newly created field</param>
    protected void UpdateWidgetsDefinition(object sender, FormFieldInfo newField)
    {
        if ((wi != null) && (newField != null))
        {
            // Get widgets based on this webpart
            DataSet ds = WidgetInfoProvider.GetWidgets("WidgetWebPartID = " + webPartId, null, 0, "WidgetID");

            // Continue only if there are some widgets
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    int widgetId = ValidationHelper.GetInteger(dr["WidgetID"], 0);
                    WidgetInfo widget = WidgetInfoProvider.GetWidgetInfo(widgetId);
                    if (widget != null)
                    {
                        // Prepare disabled field definition
                        string disabledField = String.Format("<form><field column=\"{0}\" visible=\"false\" /></form>", newField.Name);

                        // Incorporate disabled field into original definition of widget
                        widget.WidgetProperties = FormHelper.CombineFormDefinitions(widget.WidgetProperties, disabledField);

                        // Update widget
                        WidgetInfoProvider.SetWidgetInfo(widget);
                    }
                }
            }
        }
    }


    /// <summary>
    /// XML created, save it.
    /// </summary>
    protected void defaultValueEditor_XMLCreated(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(defaultValueEditor.ErrorMessage))
        {
            ShowError(defaultValueEditor.ErrorMessage);
            return;
        }

        if (wi != null)
        {
            // Load xml definition
            if (wi.WebPartParentID > 0)
            {
                // Load the form definition
                string before = PortalFormHelper.GetWebPartProperties(WebPartTypeEnum.Standard, PropertiesPosition.Before);
                string after = PortalFormHelper.GetWebPartProperties(WebPartTypeEnum.Standard, PropertiesPosition.Before);

                string formDef = FormHelper.CombineFormDefinitions(before, after);

                // Web part default values contains either properties or changed "system" values
                // First Remove records with same name as "system properties" => only actual webpart's properties remains
                string filteredDef = defaultValueEditor.FitlerDefaultValuesDefinition(wi.WebPartDefaultValues, formDef, false);

                WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(wi.WebPartParentID);

                // Remove records with same name as parent's property - its already stored in webpart's properties
                if (wpi != null)
                {
                    filteredDef = defaultValueEditor.FitlerDefaultValuesDefinition(filteredDef, wpi.WebPartProperties, true);
                }
                // If inherited web part merge webpart's properties hier with default system values
                wi.WebPartDefaultValues = FormHelper.CombineFormDefinitions(filteredDef, defaultValueEditor.DefaultValueXMLDefinition);
            }
            else
            {
                wi.WebPartProperties = defaultValueEditor.DefaultValueXMLDefinition;
            }

            // Sav web part info
            WebPartInfoProvider.SetWebPartInfo(wi);

            // Redirect with saved assign
            string url = URLHelper.RemoveParameterFromUrl(URLRewriter.CurrentURL, "saved");
            url = URLHelper.AddParameterToUrl(url, "saved", "1");
            URLHelper.Redirect(url);
        }
    }
}