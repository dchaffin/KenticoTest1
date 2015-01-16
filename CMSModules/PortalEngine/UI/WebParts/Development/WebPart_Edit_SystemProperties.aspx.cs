using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.URLRewritingEngine;
using CMS.PortalControls;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_SystemProperties : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int webPartId = QueryHelper.GetInteger("webpartid", 0);

        // Default values XML load
        ucDefaultValueEditor.XMLCreated += new EventHandler(ucDefaultValueEditor_XMLCreated);

        // If saved is found in query string
        if (!RequestHelper.IsPostBack() && (QueryHelper.GetInteger("saved", 0) == 1))
        {
            ShowChangesSaved();
        }

        // Load the form definition
        string before = PortalFormHelper.GetWebPartProperties(WebPartTypeEnum.Standard, PropertiesPosition.Before);
        string after = PortalFormHelper.GetWebPartProperties(WebPartTypeEnum.Standard, PropertiesPosition.After);

        string formDef = FormHelper.CombineFormDefinitions(before, after);

        // Get the web part info
        WebPartInfo wi = WebPartInfoProvider.GetWebPartInfo(webPartId);
        EditedObject = wi;
        if (wi != null)
        {
            // Load default values for current web part
            XmlDocument xmlDefault = LoadDefaultValuesXML(wi, formDef);

            // Set field editor        
            if (String.IsNullOrEmpty(wi.WebPartDefaultValues))
            {
                ucDefaultValueEditor.DefaultValueXMLDefinition = "<form></form>";
            }
            else
            {
                // WebPartDefaultValues contains changed fields versus default XML settings (stored in files)
                ucDefaultValueEditor.DefaultValueXMLDefinition = wi.WebPartDefaultValues;
            }

            ucDefaultValueEditor.SourceXMLDefinition = xmlDefault.DocumentElement.OuterXml;
        }
    }


    /// <summary>
    /// Load XML with default values (remove keys already overridden in properties tab).
    /// </summary>
    /// <param name="wi">Web part info</param>
    /// <param name="formDef">String XML definition of default values of webpart</param>
    private XmlDocument LoadDefaultValuesXML(WebPartInfo wi, string formDef)
    {
        // Test if there is any default properties set
        string properties = "<form></form>";
        XmlDocument xmlProperties = new XmlDocument();

        // If inherited web part load parent properties
        if (wi.WebPartParentID > 0)
        {
            WebPartInfo parentInfo = WebPartInfoProvider.GetWebPartInfo(wi.WebPartParentID);
            if (parentInfo != null)
            {
                properties = parentInfo.WebPartProperties;
            }
        }
        else
        {
            properties = wi.WebPartProperties;
        }
        xmlProperties.LoadXml(properties);

        // Load default system xml 
        XmlDocument xmlDefault = new XmlDocument();
        xmlDefault.LoadXml(formDef);

        // Filter overridden properties - remove properties with same name as in system XML
        XmlNodeList defaultList = xmlDefault.SelectNodes(@"//field");
        foreach (XmlNode node in defaultList)
        {
            string columnName = node.Attributes["column"].Value.ToString();

            XmlNodeList propertiesList = xmlProperties.SelectNodes("//field[@column=\"" + columnName + "\"]");
            //This property already set in properties tab
            if (propertiesList.Count > 0)
            {
                node.ParentNode.RemoveChild(node);
            }
        }

        // Filter empty categories            
        XmlNodeList nodes = xmlDefault.DocumentElement.ChildNodes;
        for (int i = 0; i < nodes.Count; i++)
        {
            XmlNode node = nodes[i];
            if (node.Name.ToLowerCSafe() == "category")
            {
                // Find next category
                if (i < nodes.Count - 1)
                {
                    XmlNode nextNode = nodes[i + 1];
                    if (nextNode.Name.ToLowerCSafe() == "category")
                    {
                        // Delete actual category
                        node.ParentNode.RemoveChild(node);
                        i--;
                    }
                }
            }
        }

        // Test if last category is not empty           
        nodes = xmlDefault.DocumentElement.ChildNodes;
        if (nodes.Count > 0)
        {
            XmlNode lastNode = nodes[nodes.Count - 1];
            if (lastNode.Name.ToLowerCSafe() == "category")
            {
                lastNode.ParentNode.RemoveChild(lastNode);
            }
        }
        return xmlDefault;
    }


    /// <summary>
    /// Event loaded after ok button clicked.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event args</param>
    private void ucDefaultValueEditor_XMLCreated(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(ucDefaultValueEditor.ErrorMessage))
        {
            ShowError(ucDefaultValueEditor.ErrorMessage);
            return;
        }

        int webPartId = QueryHelper.GetInteger("webpartid", 0);
        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(webPartId);
        if (wpi != null)
        {
            if (wpi.WebPartParentID > 0)
            {
                //Remove overloaded default values and replace them with new ones
                string filteredDef = String.Empty;
                //Load parent 
                WebPartInfo parentInfo = WebPartInfoProvider.GetWebPartInfo(wpi.WebPartParentID);
                if (parentInfo != null)
                {
                    // Remove properties from collection with same name as in default system xml
                    filteredDef = ucDefaultValueEditor.FitlerDefaultValuesDefinition(wpi.WebPartDefaultValues, parentInfo.WebPartProperties, false);
                }
                // FilteredDef now contains only properties inherited from parent web part
                // Combine with changed default system values
                wpi.WebPartDefaultValues = FormHelper.CombineFormDefinitions(filteredDef, ucDefaultValueEditor.DefaultValueXMLDefinition);
            }
            else
            {
                wpi.WebPartDefaultValues = ucDefaultValueEditor.DefaultValueXMLDefinition;
            }
            WebPartInfoProvider.SetWebPartInfo(wpi);
        }

        string url = URLHelper.RemoveParameterFromUrl(URLRewriter.CurrentURL, "saved");
        url = URLHelper.AddParameterToUrl(url, "saved", "1");
        URLHelper.Redirect(url);
    }
}