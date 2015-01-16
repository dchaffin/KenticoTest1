using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_DefaultValueEditor : CMSUserControl
{
    #region "Variables"

    private string mDefaultValuesXMLDefinition;
    private int mParentWebPartID;
    private string mSourceXMLDefinition = String.Empty;
    private DataRow dr = null;

    /// <summary>
    /// Default values xml definition.
    /// </summary>
    private XmlDocument xml = null;

    /// <summary>
    /// Loads FormInfo from external class.
    /// </summary>
    /// <returns>Form info</returns>
    public delegate FormInfo EditorLoadedEventHandler();


    /// <summary>
    /// Editor loaded event handler.
    /// </summary>
    public event EditorLoadedEventHandler OnEditorLoaded;


    /// <summary>
    /// When post is added.
    /// </summary>
    public event EventHandler XMLCreated;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the value that indicates whether invisible fields should be loaded
    /// </summary>
    public bool LoadHiddenFields
    {
        get;
        set;
    }


    /// <summary>
    /// Default values XML definition.
    /// </summary>
    public string DefaultValueXMLDefinition
    {
        get
        {
            return mDefaultValuesXMLDefinition;
        }
        set
        {
            mDefaultValuesXMLDefinition = value;
        }
    }


    /// <summary>
    /// Direct form XML definition if no parent web part ID is set.
    /// </summary>
    public string SourceXMLDefinition
    {
        get
        {
            return mSourceXMLDefinition;
        }
        set
        {
            mSourceXMLDefinition = value;
        }
    }


    /// <summary>
    /// Parent web part id.
    /// </summary>
    public int ParentWebPartID
    {
        get
        {
            return mParentWebPartID;
        }
        set
        {
            mParentWebPartID = value;
        }
    }


    /// <summary>
    /// Text of error message, if any error occurs
    /// </summary>
    public String ErrorMessage
    {
        get;
        set;
    }


    /// <summary>
    /// Controls.
    /// </summary>
    public new Hashtable Controls
    {
        get
        {
            if (ViewState["Controls"] == null)
            {
                ViewState["Controls"] = new Hashtable();
            }

            return (Hashtable)ViewState["Controls"];
        }
        set
        {
            ViewState["Controls"] = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Resource strings
        btnOk.Text = GetString("General.Ok");

        // Generate table with value fields
        GenerateEditor();

        // JavaScript to enable or disable inherit + load default values
        string javascript = ScriptHelper.GetScript(@"
function CheckClick(obj, itemId, defaultValue, Itype){
    var mItem = document.getElementById(itemId);
    if (mItem != null){
        if (Itype == 'textbox'){
            if (obj.checked) {
            mItem.disabled = true; mItem.value = defaultValue;
            }else{
            mItem.disabled = false;
            }
        }
        // Boolean type
        if (Itype == 'calendar'){ 
            var txtObj = document.getElementById(itemId + '_txtDateTime');
            var imgObj = document.getElementById(itemId + '_imgCalendar');
            var btnObj = document.getElementById(itemId + '_btnNow'); 
            if (obj.checked) { 
            mItem.disabled = true; txtObj.disabled = true; btnObj.disabled=true; imgObj.disabled = true; txtObj.value = defaultValue; imgObj.src='" + GetImageUrl("Design/Controls/Calendar/calendardisabled.png") + @"'; 
            }else{
            mItem.disabled = false; txtObj.disabled = false; btnObj.disabled=false; imgObj.disabled = false; imgObj.src='" + GetImageUrl("Design/Controls/Calendar/calendar.png") + @"'; 
            }
        }
        // Checkbox type
        if (Itype == 'checkbox'){ 
            var upSpan = document.getElementById(itemId + '_upperSpan');
            if (obj.checked) { 
            mItem.disabled = true; upSpan.disabled = true; mItem.checked = defaultValue; 
            }else{ 
            mItem.disabled = false; upSpan.disabled = false;
            }
        }
    }
}
");

        // Register client script to the page
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DefaultValueInheritCheckbox", javascript);
    }


    /// <summary>
    /// Generate editor table.
    /// </summary>
    public void GenerateEditor()
    {
        FormInfo fi = null;

        // Call handlers
        if (OnEditorLoaded != null)
        {
            fi = OnEditorLoaded();
        }
        else
        {
            // Get parent web part info
            WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(ParentWebPartID);

            if (wpi != null)
            {
                // Create form info and load xml definition
                fi = PortalFormHelper.GetWebPartFormInfo(wpi.WebPartName + FormHelper.CORE, wpi.WebPartProperties, null, null, false);
            }
            else
            {
                fi = new FormInfo(SourceXMLDefinition);
            }
        }

        if (fi != null)
        {
            dr = fi.GetDataRow(false);

            // Get definition elements
            var infos = fi.GetFormElements(true, LoadHiddenFields);

            // create table part
            Literal table1 = new Literal();
            pnlEditor.Controls.Add(table1);
            table1.Text = "<table cellpadding=\"3\">";

            // Hashtable counter
            int i = 0;
            bool categoryExists = false;

            // Check all items in object array
            foreach (object contrl in infos)
            {
                // Generate row for form category
                if (contrl is FormCategoryInfo)
                {
                    // Load category info
                    FormCategoryInfo fci = contrl as FormCategoryInfo;
                    if (fci != null)
                    {
                        CreateCategory(fci.CategoryCaption);
                        categoryExists = true;
                    }
                }
                else
                {
                    // Ensure the default category
                    if (!categoryExists)
                    {
                        CreateCategory(GetString("General.General"));
                        categoryExists = true;
                    }

                    // Get form field info
                    FormFieldInfo ffi = contrl as FormFieldInfo;
                    if (ffi != null)
                    {
                        CreateField(ffi, ref i);
                    }
                }
            }

            // End table part
            Literal table6 = new Literal();
            pnlEditor.Controls.Add(table6);
            table6.Text = "</table>";
        }
    }


    /// <summary>
    /// Creates a field item
    /// </summary>
    /// <param name="ffi">Form field info</param>
    /// <param name="i">Field index</param>
    private void CreateField(FormFieldInfo ffi, ref int i)
    {
        // Check if is defined inherited default value
        bool doNotInherit = IsDefined(ffi.Name);
        // Get default value
        string inheritedDefaultValue = GetDefaultValue(ffi.Name);

        // Current hashtable for client id
        Hashtable currentHashTable = new Hashtable();

        // First item is name
        currentHashTable[0] = ffi.Name;
        currentHashTable[3] = ffi.Caption;

        // Begin new row and column
        Literal table2 = new Literal();
        pnlEditor.Controls.Add(table2);
        table2.Text = "<tr class=\"InheritWebPart\"><td>";

        // Property label
        Label lblName = new Label();
        pnlEditor.Controls.Add(lblName);
        lblName.Text = ResHelper.LocalizeString(ffi.Caption);
        lblName.ToolTip = ResHelper.LocalizeString(ffi.Description);
        if (!lblName.Text.EndsWithCSafe(":"))
        {
            lblName.Text += ":";
        }

        // New column
        Literal table3 = new Literal();
        pnlEditor.Controls.Add(table3);
        table3.Text = "</td><td>";

        // Type string for JavaScript function
        string jsType = "textbox";

        // Type switcher
        if (FormHelper.IsFieldOfType(ffi, FormFieldControlTypeEnum.CheckBoxControl))
        {
            // Checkbox type field
            CheckBox chk = new CheckBox();
            pnlEditor.Controls.Add(chk);
            chk.Checked = ValidationHelper.GetBoolean(ffi.DefaultValue, false);
            chk.InputAttributes.Add("disabled", "disabled");

            chk.Attributes.Add("id", chk.ClientID + "_upperSpan");

            if (doNotInherit)
            {
                chk.InputAttributes.Remove("disabled");
                chk.Enabled = true;
                chk.Checked = ValidationHelper.GetBoolean(inheritedDefaultValue, false);
            }

            jsType = "checkbox";
            currentHashTable[1] = chk.ClientID;
        }
        else if (FormHelper.IsFieldOfType(ffi, FormFieldControlTypeEnum.CalendarControl))
        {
            // Date time picker
            DateTimePicker dtPick = new DateTimePicker();
            pnlEditor.Controls.Add(dtPick);
            String value = ffi.DefaultValue;
            dtPick.Enabled = false;
            dtPick.SupportFolder = ResolveUrl("~/CMSAdminControls/Calendar");

            if (doNotInherit)
            {
                dtPick.Enabled = true;
                value = inheritedDefaultValue;
            }

            if (ValidationHelper.IsMacro(value))
            {
                dtPick.DateTimeTextBox.Text = value;
            }
            else
            {
                dtPick.SelectedDateTime = ValidationHelper.GetDateTime(value, DataHelper.DATETIME_NOT_SELECTED, CultureHelper.EnglishCulture);
            }

            jsType = "calendar";
            currentHashTable[1] = dtPick.ClientID;
        }
        else
        {
            // Other types represent by textbox
            CMSTextBox txt = new CMSTextBox();
            pnlEditor.Controls.Add(txt);

            // Convert from default culture to current culture if needed (type double, datetime)            
            txt.Text = (String.IsNullOrEmpty(ffi.DefaultValue) || ValidationHelper.IsMacro(ffi.DefaultValue)) ? ffi.DefaultValue : ValidationHelper.GetString(DataHelper.ConvertValue(ffi.DefaultValue, GetDataType(ffi.Name)), String.Empty);

            txt.CssClass = "TextBoxField";
            txt.Enabled = ffi.Enabled;
            txt.Enabled = false;

            if (ffi.DataType == FormFieldDataTypeEnum.LongText)
            {
                txt.TextMode = TextBoxMode.MultiLine;
                txt.Rows = 3;
            }

            if (doNotInherit)
            {
                txt.Enabled = true;
                txt.Text = (String.IsNullOrEmpty(inheritedDefaultValue) || ValidationHelper.IsMacro(ffi.DefaultValue)) ? inheritedDefaultValue : ValidationHelper.GetString(DataHelper.ConvertValue(inheritedDefaultValue, GetDataType(ffi.Name)), String.Empty);
            }

            currentHashTable[1] = txt.ClientID;
        }

        // New column
        Literal table4 = new Literal();
        pnlEditor.Controls.Add(table4);
        table4.Text = "</td><td>" + ffi.DataType.ToString() + "</td><td>";


        // Inherit checkbox
        CheckBox chkInher = new CheckBox();
        pnlEditor.Controls.Add(chkInher);
        chkInher.Checked = true;

        // Uncheck checkbox if this property is not inherited
        if (doNotInherit)
        {
            chkInher.Checked = false;
        }

        chkInher.Text = GetString("DefaultValueEditor.Inherited");

        string defValue = (ffi.DefaultValue == null) ? String.Empty : ffi.DefaultValue;

        if (!String.IsNullOrEmpty(ffi.DefaultValue))
        {
            if (!ValidationHelper.IsMacro(ffi.DefaultValue))
            {
                defValue = ValidationHelper.GetString(DataHelper.ConvertValue(defValue, GetDataType(ffi.Name)), String.Empty);
            }
            else
            {
                defValue = MacroResolver.RemoveSecurityParameters(defValue, true, null);
            }
        }

        // Set default value for JavaScript function
        defValue = "'" + defValue.Replace("\r\n", "\\n") + "'";

        if (jsType == "checkbox")
        {
            defValue = ValidationHelper.GetBoolean(ffi.DefaultValue, false).ToString().ToLowerCSafe();
        }

        // Add JavaScript attribute with js function call
        chkInher.Attributes.Add("onclick", "CheckClick(this, '" + currentHashTable[1].ToString() + "', " + defValue + ", '" + jsType + "' );");

        // Insert current checkbox id
        currentHashTable[2] = chkInher.ClientID;

        // Add current hashtable to the controls hashtable
        ((Hashtable)Controls)[i] = currentHashTable;

        // End current row
        Literal table5 = new Literal();
        pnlEditor.Controls.Add(table5);
        table5.Text = "</td></tr>";

        i++;
    }


    /// <summary>
    /// Returns type of column in form info datarow
    /// </summary>
    /// <param name="columnName">Column name</param>
    private Type GetDataType(String columnName)
    {
        if ((dr != null) && dr.Table.Columns.Contains(columnName))
        {
            return dr.Table.Columns[columnName].DataType;
        }

        return typeof(String);
    }


    /// <summary>
    /// Creates the category item
    /// </summary>
    /// <param name="categoryName">Category name</param>
    private void CreateCategory(string categoryName)
    {
        // Create row html code
        Literal tabCat = new Literal();
        pnlEditor.Controls.Add(tabCat);
        tabCat.Text = "<tr class=\"InheritCategory\"><td>";

        // Create label control and insert it to the page
        Label lblCat = new Label();
        pnlEditor.Controls.Add(lblCat);
        lblCat.Text = ResHelper.LocalizeString(categoryName);
        lblCat.Font.Bold = true;

        // End row html code
        Literal tabCat2 = new Literal();
        pnlEditor.Controls.Add(tabCat2);
        tabCat2.Text = "</td><td></td><td></td>";
    }


    /// <summary>
    /// Returns true if property is set in default values sheet.
    /// </summary>
    public bool IsDefined(string name)
    {
        // Check if xml document exist, if is not created, create it
        if ((xml == null) && !String.IsNullOrEmpty(DefaultValueXMLDefinition))
        {
            xml = new XmlDocument();
            xml.LoadXml(DefaultValueXMLDefinition);
        }

        if ((xml != null) && (xml.DocumentElement != null))
        {
            // Get the field
            XmlNode fieldNode = TableManager.SelectFieldNode(xml.DocumentElement, "name", name);
            if (fieldNode != null)
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Remove values from original  (not) found in alternative.
    /// </summary>
    /// <param name="original">Original XML</param>
    /// <param name="alternative">Alternative XML</param>
    /// <param name="deleteExistingRecord">Whether remove (from original XML) found or not found records (in alternative XML)</param>
    public string FitlerDefaultValuesDefinition(string original, string alternative, bool deleteExistingRecord)
    {
        if (string.IsNullOrEmpty(alternative))
        {
            // Return original form definition
            return original;
        }

        // Parse XML for both form definitions
        XmlDocument xmlOrigDoc = new XmlDocument();
        xmlOrigDoc.LoadXml(original);
        XmlDocument xmlAltDoc = new XmlDocument();
        xmlAltDoc.LoadXml(alternative);

        // Iterate through field nodes in alternative form definition
        if (xmlAltDoc.DocumentElement != null)
        {
            XmlNode altField = null;
            //foreach (XmlNode orgField in xmlOrigDoc.DocumentElement.ChildNodes)
            for (int i = 0; i < xmlOrigDoc.DocumentElement.ChildNodes.Count; i++)
            {
                XmlNode orgField = xmlOrigDoc.DocumentElement.ChildNodes[i];
                altField = null;
                if ((orgField.LocalName.ToLowerCSafe() == "field") && (orgField.Attributes["name"] != null))
                {
                    // Get field name
                    string elemName = orgField.Attributes["name"].Value;

                    // Get field with the same column name from alternative definition
                    altField = TableManager.SelectFieldNode(xmlAltDoc.DocumentElement, "column", elemName);
                }

                // If deleteExistinRecord is set to False and no record is found in alternative XML -> remove from original
                if ((altField == null) && (!deleteExistingRecord))
                {
                    orgField.ParentNode.RemoveChild(orgField);
                    i--;
                }
                // If deleteExistinRecord is set to True and record found in alternative XML -> remove from original
                else if ((altField != null) && (deleteExistingRecord))
                {
                    orgField.ParentNode.RemoveChild(orgField);
                    i--;
                }
            }
        }
        return xmlOrigDoc.OuterXml;
    }


    /// <summary>
    /// Returns default value according to selected name, if value doesn't exists return "".
    /// </summary>
    public string GetDefaultValue(string name)
    {
        // Check if xml document exist, if is not created, create it
        if ((xml == null) && !String.IsNullOrEmpty(DefaultValueXMLDefinition))
        {
            xml = new XmlDocument();
            xml.LoadXml(DefaultValueXMLDefinition);
        }

        if ((xml != null) && (xml.DocumentElement != null))
        {
            // Get the field
            XmlNode fieldNode = TableManager.SelectFieldNode(xml.DocumentElement, "name", name);
            if (fieldNode != null)
            {
                return fieldNode.Attributes["value"].Value;
            }
        }

        return "";
    }


    /// <summary>
    /// OK click handler.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        Hashtable mHash = (Hashtable)Controls;
        Hashtable panelControl = new Hashtable();

        // Load panel controls
        foreach (Control ctrl in pnlEditor.Controls)
        {
            panelControl[ctrl.ClientID] = ctrl;
        }

        XmlDocument xmlDom = new XmlDocument();
        xmlDom.AppendChild(xmlDom.CreateElement("", "defaultvalues", ""));
        XmlElement xmlRoot = xmlDom.DocumentElement;

        // Check all inherited checkboxes and if is some unchecked, get value and set it to the xml
        for (int i = 0; i < mHash.Count; i++)
        {
            // Get current hashtable with client id
            Hashtable currentHash = (Hashtable)mHash[i];
            if (panelControl[currentHash[2].ToString()] != null)
            {
                // Get inherit checkbox
                CheckBox chkInh = panelControl[currentHash[2].ToString()] as CheckBox;
                if ((chkInh != null) && (!chkInh.Checked))
                {
                    XmlElement node = xmlDom.CreateElement("field");
                    String name = currentHash[0].ToString();
                    node.SetAttribute("name", name);

                    // Select what is control type
                    Control currCtrl = (Control)panelControl[currentHash[1].ToString()];

                    if (currCtrl is CheckBox)
                    {
                        node.SetAttribute("value", ((CheckBox)currCtrl).Checked.ToString());
                    }
                    else if (currCtrl is DateTimePicker)
                    {
                        String value = ((DateTimePicker)currCtrl).DateTimeTextBox.Text;
                        if (ValidationHelper.IsMacro(value))
                        {
                            node.SetAttribute("value", value);
                        }
                        else
                        {
                            node.SetAttribute("value", ((DateTimePicker)currCtrl).SelectedDateTime.ToString(CultureHelper.EnglishCulture));
                        }
                    }
                    else if (currCtrl is CMSTextBox)
                    {
                        // Get type of object from datarow
                        Type type = GetDataType(name);

                        // Get value from textbox
                        String value = ((CMSTextBox)currCtrl).Text;

                        // If type is different from string (which needs no validation)
                        if ((type != typeof(String)) && !String.IsNullOrEmpty(value) && !ValidationHelper.IsMacro(value))
                        {
                            // Validate input based on data type
                            if (!DataHelper.IsValid(value, type))
                            {
                                ErrorMessage = String.Format(GetString("datatype.wrongformat"), currentHash[3]);
                                value = "";

                                // To prevent control disable (may occur in some cases)
                                ((TextBox)currCtrl).Enabled = true;
                            }
                            else
                            {
                                value = DataHelper.ConvertValueToDefaultCulture(value, type);
                            }
                        }

                        node.SetAttribute("value", value);
                    }

                    xmlRoot.AppendChild(node);
                }
            }
        }

        DefaultValueXMLDefinition = xmlDom.InnerXml;

        // Call handlers
        if (XMLCreated != null)
        {
            XMLCreated(this, null);
        }
    }

    #endregion
}