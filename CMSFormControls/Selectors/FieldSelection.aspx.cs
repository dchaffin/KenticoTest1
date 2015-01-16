using System;

using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSFormControls_Selectors_FieldSelection : DesignerPage
{
    #region "Properties"

    /// <summary>
    /// Class name
    /// </summary>
    public string ClassName
    {
        get
        {
            return QueryHelper.GetString("classname", null);
        }
    }


    /// <summary>
    /// Field type
    /// </summary>
    public FormFieldDataTypeEnum FieldType
    {
        get;
        set;
    }


    /// <summary>
    /// Class type
    /// </summary>
    /// <remarks>0 - Custom tables, 1 - Document types, 2 - System tables</remarks>
    public int ClassType
    {
        get
        {
            return QueryHelper.GetInteger("classtype", 1);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check parameters
        if (!QueryHelper.ValidateHash("hash"))
        {
            ShowError("Validation of query hash failed.");
            pnlUpdate.Visible = false;
            btnOk.Visible = false;
        }
        else
        {
            // Register script for pendingCallbacks repair
            ScriptHelper.FixPendingCallbacks(Page);
            string parentClientId = QueryHelper.GetString("cid", string.Empty);

            ScriptHelper.RegisterStartupScript(this, typeof(string), "selectField", ScriptHelper.GetScript(
    @"
function SelectFields(drpId) {
    var drpElem = document.getElementById(drpId);
    var value = drpElem.options[drpElem.selectedIndex].value;
    var text = drpElem.options[drpElem.selectedIndex].text;
    if ((wopener != null) && (wopener.SetValue_" + parentClientId + @" != null)) {
        wopener.SetValue_" + parentClientId + @"(value, text);
    }
    CloseDialog();
}"));

            // Setup uniselector
            selectionElem.UniSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
            selectionElem.DisplayClearButton = false;
            selectionElem.DropDownSingleSelect.AutoPostBack = true;
            selectionElem.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;
            selectionElem.IsLiveSite = false;
            switch (ClassType)
            {
                // Custom tables
                case 0:
                    selectionElem.UniSelector.ObjectType = SettingsObjectType.CUSTOMTABLE;
                    CurrentMaster.Title.TitleText = GetString("attach.customtablefield");
                    lblClassNames.ResourceString = "objecttype.cms_customtable";
                    FieldType = FormFieldDataTypeEnum.Unknown;
                    break;

                // Document types
                case 1:
                default:
                    selectionElem.UniSelector.ObjectType = SettingsObjectType.DOCUMENTTYPE;
                    selectionElem.ShowOnlyCoupled = true;
                    CurrentMaster.Title.TitleText = GetString("attach.documenttypefield");
                    lblClassNames.ResourceString = "objecttype.cms_documenttype";
                    // Backward compatibility
                    FieldType = FormFieldDataTypeEnum.DocumentAttachments;
                    break;

                // System tables
                case 2:
                    selectionElem.UniSelector.ObjectType = SettingsObjectType.SYSTEMTABLE;
                    CurrentMaster.Title.TitleText = GetString("attach.systemtablefield");
                    lblClassNames.ResourceString = "objecttype.cms_systemtable";
                    FieldType = FormFieldDataTypeEnum.Unknown;
                    break;
            }
        }

        // Initialize master page
        CurrentMaster.Title.TitleImage = GetImageUrl("Design/Selectors/fieldselection.png");
        // Initialize buttons' functions
        btnOk.OnClientClick = "SelectFields('" + drpField.ClientID + "');  return false;";
        btnCancel.OnClientClick = "CloseDialog();  return false;";

        // Bind fields dropdown list
        if (!RequestHelper.IsPostBack())
        {
            selectionElem.ReloadData(false);
            selectionElem.DropDownSingleSelect.SelectedIndex = 0;
            LoadFields();
        }

        // Class name
        if (!string.IsNullOrEmpty(ClassName))
        {
            selectionElem.Value = ClassName;
            plcClassNames.Visible = false;
        }
    }


    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        LoadFields();
    }

    #endregion


    #region "Private methods"

    private void LoadFields()
    {
        // Clear dropdown list
        drpField.Items.Clear();
        // Get dataclass info
        string className = ValidationHelper.GetString(selectionElem.Value, null);
        if (!string.IsNullOrEmpty(className))
        {
            DataClassInfo dci = DataClassInfoProvider.GetDataClass(className);

            // Get fields of type file
            FormInfo fi = FormHelper.GetFormInfo(dci.ClassName, false);
            FormFieldInfo[] ffi = null;
            if (FieldType != FormFieldDataTypeEnum.Unknown)
            {
                ffi = fi.GetFields(FieldType).ToArray();
            }
            else
            {
                ffi = fi.GetFields(true, true).ToArray();
            }
            // Sort fields by caption
            Array.Sort(ffi, (ffi1, ffi2) => ffi1.Caption.CompareToCSafe(ffi2.Caption));

            // Bind fields to dropdown list
            drpField.DataSource = ffi;
            drpField.DataTextField = "Caption";
            drpField.DataValueField = "Guid";
            drpField.DataBind();

            // Enable/disable controls
            bool fieldsFound = (drpField.Items.Count != 0);
            btnOk.Enabled = fieldsFound;
            drpField.Enabled = fieldsFound;
            UpdatePanels();
        }
        else
        {
            drpField.Enabled = false;
        }
    }


    private void UpdatePanels()
    {
        pnlUpdate.Update();
        pnlUpdateButtons.Update();
    }

    #endregion
}