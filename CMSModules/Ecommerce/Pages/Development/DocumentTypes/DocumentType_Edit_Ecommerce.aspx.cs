using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSModules_Ecommerce_Pages_Development_DocumentTypes_DocumentType_Edit_Ecommerce : CMSEcommercePage
{
    #region "Variables"

    private int docTypeId = 0;
    private DataClassInfo dci = null;
    private List<FormFieldInfo> fields = null;
    private ArrayList mCustomProperties = null;

    #endregion


    #region "Properties"

    private ArrayList CustomProperties
    {
        get
        {
            if (mCustomProperties == null)
            {
                mCustomProperties = new ArrayList();

                LoadCustomProperties(mCustomProperties);
            }

            return mCustomProperties;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        RequireSite = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        pnlFieldsMappings.GroupingText = GetString("com.fieldsmapping");
        pnlAutoCreation.GroupingText = GetString("com.productautocreation");
        pnlProductSection.GroupingText = GetString("com.documentproductrelation");

        docTypeId = QueryHelper.GetInteger("documenttypeid", 0);
        dci = DataClassInfoProvider.GetDataClass(docTypeId);

        // Get class fields
        if (dci != null)
        {
            FormInfo fi = FormHelper.GetFormInfo(dci.ClassName, false);
            fields = fi.GetFields(true, true);
        }

        if (!RequestHelper.IsPostBack())
        {
            FillDropDownLists();

            if (dci != null)
            {
                // Set checkboxes
                chkGenerateSKU.Checked = dci.ClassCreateSKU;
                chkIsProduct.Checked = dci.ClassIsProduct;
                chkIsProductSection.Checked = dci.ClassIsProductSection;

                // Select mapping fields
                SelectMappings();

                // Select specified department
                if (string.IsNullOrEmpty(dci.ClassSKUDefaultDepartmentName))
                {
                    departmentElem.DepartmentID = dci.ClassSKUDefaultDepartmentID;
                }
                else
                {
                    departmentElem.DepartmentName = dci.ClassSKUDefaultDepartmentName;
                }

                // Select default product type
                productTypeElem.Value = dci.ClassSKUDefaultProductType;
            }
        }

        // Create tables 
        CreateTable();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide custom properties when no defined
        plcCustom.Visible = CustomProperties.Count > 0;

        // Hide product related properties when class does not represent a product
        pnlProductTypeProperties.Visible = chkIsProduct.Checked;
    }

    #endregion


    #region "Events"

    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Validate form
        string errorMessage = ValidateForm();

        if (errorMessage == "")
        {
            dci = DataClassInfoProvider.GetDataClass(docTypeId);
            if (dci != null)
            {
                // Set checkboxes
                dci.ClassCreateSKU = chkGenerateSKU.Checked;
                dci.ClassIsProductSection = chkIsProductSection.Checked;
                dci.ClassIsProduct = chkIsProduct.Checked;

                if (dci.ClassIsProduct)
                {
                    #region "Set mappings"

                    // Handle image path field
                    SetValueFromDropDown("SKUImagePath", drpImage);

                    // Handle name field
                    SetValueFromDropDown("SKUName", drpName);

                    // Handle weight field
                    SetValueFromDropDown("SKUWeight", drpWeight);

                    // Handle height field
                    SetValueFromDropDown("SKUHeight", drpHeight);

                    // Handle width field
                    SetValueFromDropDown("SKUWidth", drpWidth);

                    // Handle depth field
                    SetValueFromDropDown("SKUDepth", drpDepth);

                    // Handle price field
                    SetValueFromDropDown("SKUPrice", drpPrice);

                    // Handle description field
                    SetValueFromDropDown("SKUShortDescription", drpShortDescription);

                    // Handle description field
                    SetValueFromDropDown("SKUDescription", drpDescription);

                    // Handle custom properties fields
                    foreach (FormFieldInfo field in CustomProperties)
                    {
                        string fieldName = field.Name;
                        DropDownList drp = (DropDownList)pnlCustomProperties.FindControl(fieldName + "FieldName");
                        if (drp != null)
                        {
                            SetValueFromDropDown(fieldName, drp);
                        }
                    }

                    #endregion

                    // Set document type product defaults
                    dci.ClassSKUDefaultDepartmentName = departmentElem.DepartmentName;
                    dci.ClassSKUDefaultProductType = (string)productTypeElem.Value;
                }

                try
                {
                    // Save changes to database
                    DataClassInfoProvider.SetDataClass(dci);
                    ShowChangesSaved();
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
        }

        // Show error message
        if (errorMessage != "")
        {
            ShowError(errorMessage);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Validates form and returns error message when occures.
    /// </summary>
    protected string ValidateForm()
    {
        string errorMessage = "";

        if (chkGenerateSKU.Checked)
        {
            // Product name and Product price fields must be mapped
            if ((drpName.SelectedValue == "") || (drpPrice.SelectedValue == ""))
            {
                errorMessage = GetString("DocType.Ecommerce.MappingMissing");
            }
        }

        return errorMessage;
    }


    /// <summary>
    /// Selects specified mappings in dropdownlists.
    /// </summary>
    protected void SelectMappings()
    {
        if (dci != null)
        {
            drpName.SelectedValue = ValidationHelper.GetString(dci.SKUMappings["SKUName"], "0");
            drpImage.SelectedValue = ValidationHelper.GetString(dci.SKUMappings["SKUImagePath"], "0");
            drpWeight.SelectedValue = ValidationHelper.GetString(dci.SKUMappings["SKUWeight"], "0");
            drpHeight.SelectedValue = ValidationHelper.GetString(dci.SKUMappings["SKUHeight"], "0");
            drpWidth.SelectedValue = ValidationHelper.GetString(dci.SKUMappings["SKUWidth"], "0");
            drpDepth.SelectedValue = ValidationHelper.GetString(dci.SKUMappings["SKUDepth"], "0");
            drpPrice.SelectedValue = ValidationHelper.GetString(dci.SKUMappings["SKUPrice"], "0");
            drpShortDescription.SelectedValue = ValidationHelper.GetString(dci.SKUMappings["SKUShortDescription"], "0");
            drpDescription.SelectedValue = ValidationHelper.GetString(dci.SKUMappings["SKUDescription"], "0");
        }
    }


    /// <summary>
    /// Fills dropdownlists with data.
    /// </summary>
    protected void FillDropDownLists()
    {
        FillDropDownList(drpImage);
        FillDropDownList(drpHeight);
        FillDropDownList(drpDepth);
        FillDropDownList(drpWeight);
        FillDropDownList(drpWidth);
        FillDropDownList(drpName);
        FillDropDownList(drpPrice);
        FillDropDownList(drpShortDescription);
        FillDropDownList(drpDescription);
    }


    /// <summary>
    /// Fills given dropdown list with data.
    /// </summary>
    /// <param name="dropDown">Drop down list to fill</param>
    private void FillDropDownList(DropDownList dropDown)
    {
        // Fill dropdownlists with data
        dropDown.Items.Add(new ListItem(GetString("general.selectnone"), ""));
        dropDown.Items.Add(new ListItem("DocumentName", "DocumentName"));

        if (fields != null)
        {
            foreach (FormFieldInfo ffi in fields)
            {
                if (ffi.Name.ToLowerCSafe() != "documentname")
                {
                    dropDown.Items.Add(new ListItem(ffi.Name, ffi.Name));
                }
            }
        }
    }


    /// <summary>
    /// Loads custom properties to given list.
    /// </summary>
    /// <param name="properties">ArrayList to load properties to</param>
    private void LoadCustomProperties(ArrayList properties)
    {
        properties.Clear();

        // Load DataClass
        DataClassInfo productClass = DataClassInfoProvider.GetDataClass("ecommerce.sku");

        if (productClass != null)
        {
            // Load XML definition
            FormInfo fi = FormHelper.GetFormInfo(productClass.ClassName, false);
            // Get all fields
            List<IFormItem> itemList = fi.GetFormElements(true, true, true);

            if (itemList != null)
            {
                FormFieldInfo formField = null;

                // Store each field to array
                foreach (object item in itemList)
                {
                    if (item is FormFieldInfo)
                    {
                        formField = ((FormFieldInfo)(item));
                        properties.Add(formField);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Creates table.
    /// </summary>
    private void CreateTable()
    {
        pnlCustomProperties.Controls.Clear();

        Table table = new Table();
        TableRow row = new TableRow();
        TableCell labelCell = new TableCell();
        TableCell valueCell = new TableCell();
        Label label = new Label();

        // Prepare title
        pnlCustomProperties.Controls.Add(table);
        label.Text = GetString("com.productedit.customproperties");
        label.Font.Bold = true;
        labelCell.Controls.Add(label);
        row.Cells.Add(labelCell);
        row.Cells.Add(valueCell);
        table.Rows.Add(row);

        // Create row for each field
        foreach (FormFieldInfo field in CustomProperties)
        {
            string value = null;
            row = new TableRow();
            label = new Label();
            DropDownList drpColumns = new DropDownList();

            // Get value
            if (dci != null)
            {
                value = ValidationHelper.GetString(dci.SKUMappings[field.Name], "0");
            }

            // Prepare caption
            string caption = string.IsNullOrEmpty(field.Caption) ? field.Name : field.Caption;
            label.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(caption) + ResHelper.Colon);

            // Create cell with label
            labelCell = new TableCell();
            labelCell.Controls.Add(label);
            labelCell.CssClass = "FieldLabel";
            labelCell.Style.Add("width", "175px");
            row.Cells.Add(labelCell);

            // Create cell with drop down list
            valueCell = new TableCell();
            valueCell.Controls.Add(drpColumns);

            // Fill drop down list with values
            FillDropDownList(drpColumns);
            drpColumns.SelectedValue = value;
            drpColumns.CssClass = "DropDownField";
            drpColumns.ID = field.Name + "FieldName";

            row.Cells.Add(valueCell);

            // Add row to the table
            table.Rows.Add(row);
        }
    }


    /// <summary>
    /// Sets the value from given drop down list info dataclass under specified key.
    /// </summary>
    /// <param name="key">The key (SKU column name)</param>
    /// <param name="dropDown">Drop down list which value is to be stored</param>
    private void SetValueFromDropDown(string key, DropDownList dropDown)
    {
        if (dropDown.SelectedIndex > 0)
        {
            dci.SKUMappings[key] = dropDown.SelectedValue;
        }
        else if (dci.SKUMappings.Contains(key))
        {
            dci.SKUMappings.Remove(key);
        }
    }

    #endregion
}