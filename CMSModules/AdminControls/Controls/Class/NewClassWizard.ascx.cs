using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;
using CMS.EventLog;

public partial class CMSModules_AdminControls_Controls_Class_NewClassWizard : CMSUserControl
{
    #region "Private fields"

    private string mTheme = null;

    private bool mSystemDevelopmentMode = false;

    private NewClassWizardModeEnum mMode = NewClassWizardModeEnum.DocumentType;

    private string mStep6Description = "documenttype_new_step6.description";

    #endregion


    #region "Private properties"

    /// <summary>
    /// Name of the new created class.
    /// </summary>
    private string ClassName
    {
        get
        {
            object obj = ViewState["ClassName"];
            return (obj == null) ? string.Empty : (string)obj;
        }

        set
        {
            ViewState["ClassName"] = value;
        }
    }


    /// <summary>
    /// Indicates whether steps 3 and 4 were omitted or not.
    /// </summary>
    private bool SomeStepsOmitted
    {
        get
        {
            object obj = ViewState["SomeStepsOmitted"];
            return (obj == null) ? false : (bool)obj;
        }

        set
        {
            ViewState["SomeStepsOmitted"] = value;
        }
    }


    /// <summary>
    /// Indicates whether steps 3 and 4 were omitted or not.
    /// </summary>
    private bool IsContainer
    {
        get
        {
            object obj = ViewState["IsContainer"];
            return (obj == null) ? false : (bool)obj;
        }

        set
        {
            ViewState["IsContainer"] = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Current theme applied.
    /// </summary>
    public string Theme
    {
        get
        {
            return mTheme;
        }
        set
        {
            mTheme = value;
        }
    }


    /// <summary>
    /// Indicates whether wizzard sets the inner field editor to development mode - can edit system fields.
    /// </summary>
    public bool SystemDevelopmentMode
    {
        get
        {
            return mSystemDevelopmentMode;
        }
        set
        {
            mSystemDevelopmentMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the wizzard mode - what item is created.
    /// </summary>
    public NewClassWizardModeEnum Mode
    {
        get
        {
            return mMode;
        }

        set
        {
            mMode = value;
        }
    }


    /// <summary>
    /// Description resource string used in sixth step.
    /// </summary>
    public string Step6Description
    {
        get
        {
            return mStep6Description;
        }
        set
        {
            mStep6Description = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set controls of the first step
        if (!RequestHelper.IsPostBack())
        {
            wzdStep1.EnableViewState = true;
            wzdStep2.EnableViewState = false;
            wzdStep3.EnableViewState = false;
            wzdStep4.EnableViewState = false;
            wzdStep5.EnableViewState = false;
            wzdStep6.EnableViewState = false;
            wzdStep7.EnableViewState = false;
            wzdStep8.EnableViewState = false;

            switch (Mode)
            {
                // If the wizard is running as new document type wizard     
                case NewClassWizardModeEnum.DocumentType:
                    {
                        lblDisplayName.Text = GetString("DocumentType_New.DisplayName");
                        lblFullCodeName.Text = GetString("DocumentType_New.FullCodeName");
                        lblNamespaceName.Text = GetString("DocumentType_New.NamespaceName");
                        lblCodeName.Text = GetString("DocumentType_New.CodeName");

                        // Set validators' error messages
                        rfvDisplayName.ErrorMessage = GetString("DocumentType_New.ErrorEmptyDisplayName");
                        rfvCodeName.ErrorMessage = GetString("DocumentType_New.ErrorEmptyCodeName");
                        rfvNamespaceName.ErrorMessage = GetString("DocumentType_New.ErrorEmptyNamespaceName");
                        revNameSpaceName.ErrorMessage = GetString("DocumentType_New.NamespaceNameIdentifier");
                        revCodeName.ErrorMessage = GetString("DocumentType_New.CodeNameIdentifier");

                        txtNamespaceName.Text = "custom";

                        ucHeader.Description = GetString("DocumentType_New_Step1.Description");
                    }
                    break;

                // If the wizzard is running as new data class wizzard    
                case NewClassWizardModeEnum.Class:
                    {
                        lblDisplayName.Text = GetString("sysdev.class_new.DisplayName");
                        lblFullCodeName.Text = GetString("sysdev.class_new.FullCodeName");
                        lblNamespaceName.Text = GetString("DocumentType_New.NamespaceName");
                        lblCodeName.Text = GetString("sysdev.class_new.CodeName");

                        // Set validators' error messages
                        rfvDisplayName.ErrorMessage = GetString("sysdev.class_new.ErrorEmptyDisplayName");
                        rfvCodeName.ErrorMessage = GetString("sysdev.class_new.ErrorEmptyCodeName");
                        rfvNamespaceName.ErrorMessage = GetString("sysdev.class_new.ErrorEmptyNamespaceName");
                        revNameSpaceName.ErrorMessage = GetString("sysdev.class_new.NamespaceNameIdentifier");
                        revCodeName.ErrorMessage = GetString("sysdev.class_new.CodeNameIdentifier");

                        txtNamespaceName.Text = "CMS";
                        ucHeader.Description = GetString("sysdev.class_new_Step1.Description");
                    }
                    break;

                // If the wizzard is running as new custom table wizzard
                case NewClassWizardModeEnum.CustomTable:
                    {
                        lblDisplayName.Text = GetString("customtable.newwizzard.DisplayName");
                        lblFullCodeName.Text = GetString("customtable.newwizzard.FullCodeName");
                        lblNamespaceName.Text = GetString("customtable.newwizzard.NamespaceName");
                        lblCodeName.Text = GetString("customtable.newwizzard.CodeName");

                        // Set validators' error messages
                        rfvDisplayName.ErrorMessage = GetString("customtable.newwizzard.ErrorEmptyDisplayName");
                        rfvCodeName.ErrorMessage = GetString("customtable.newwizzard.ErrorEmptyCodeName");
                        rfvNamespaceName.ErrorMessage = GetString("customtable.newwizzard.ErrorEmptyNamespaceName");
                        revNameSpaceName.ErrorMessage = GetString("customtable.newwizzard.NamespaceNameIdentifier");
                        revCodeName.ErrorMessage = GetString("customtable.newwizzard.CodeNameIdentifier");

                        txtNamespaceName.Text = "customtable";
                        ucHeader.Description = GetString("customtable.newwizzard.Step1Description");
                    }
                    break;
            }

            // Set regular expression for identifier validation
            revNameSpaceName.ValidationExpression = ValidationHelper.IdentifierRegExp.ToString();
            revCodeName.ValidationExpression = ValidationHelper.IdentifierRegExp.ToString();

            wzdStep1.Title = GetString("general.general");
        }
        else
        {
            // Disable regular expression validators in next steps
            revNameSpaceName.Enabled = false;
            revCodeName.Enabled = false;

            selInherits.WhereCondition = "ClassIsCoupledClass = 1";
        }

        // Set search fields for custom tables
        if (Mode == NewClassWizardModeEnum.CustomTable)
        {
            SearchFields.LoadActualValues = true;
        }

        // Set FieldEditor's properties
        FieldEditor.ClassName = ClassName;
        FieldEditor.Mode = FieldEditorModeEnum.ClassFormDefinition;

        // Restrict controls if custom tables
        if (Mode == NewClassWizardModeEnum.CustomTable)
        {
            FieldEditor.Mode = FieldEditorModeEnum.CustomTable;
        }

        // Set field editor's development mode
        FieldEditor.DevelopmentMode = SystemDevelopmentMode;
        FieldEditor.IsWizard = true;
        if ((!SystemDevelopmentMode) && (Mode == NewClassWizardModeEnum.DocumentType))
        {
            FieldEditor.EnableSystemFields = true;
        }

        wzdNewDocType.ActiveStepChanged += new EventHandler(wzdNewDocType_ActiveStepChanged);

        // Set buttons' text                
        wzdNewDocType.StartNextButtonText = GetString("general.next");
        wzdNewDocType.StepNextButtonText = wzdNewDocType.StartNextButtonText;
        wzdNewDocType.FinishCompleteButtonText = GetString("general.Finish");
    }


    void wzdNewDocType_ActiveStepChanged(object sender, EventArgs e)
    {
        // Field editor needs to be visible to be able to reload controls properly
        if (wzdNewDocType.ActiveStep == wzdStep3)
        {
            FieldEditor.Reload(null);
        }
    }


    protected void Page_PreRender()
    {
        // Set current step title
        ucHeader.Header = GetCurrentStepTitle(wzdNewDocType.ActiveStepIndex);
        ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), wzdNewDocType.ActiveStepIndex + 1);

        // Manage steps by mode
        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                if (IsContainer)
                {
                    if (wzdNewDocType.ActiveStepIndex == 4)
                    {
                        ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 3);
                    }
                    else if (wzdNewDocType.ActiveStepIndex == 5)
                    {
                        ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 4);
                    }
                    else if (wzdNewDocType.ActiveStepIndex == 7)
                    {
                        ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 5);
                    }
                }
                break;

            case NewClassWizardModeEnum.Class:
                if (wzdNewDocType.ActiveStepIndex == 7)
                {
                    ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 4);
                }
                break;

            case NewClassWizardModeEnum.CustomTable:
                if (wzdNewDocType.ActiveStepIndex == 5)
                {
                    ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 4);
                }

                if (wzdNewDocType.ActiveStepIndex == 6)
                {
                    ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 5);
                }

                if (wzdNewDocType.ActiveStepIndex == 7)
                {
                    ucHeader.Title = string.Format(GetString("DocumentType_New.Step"), 6);
                }
                break;
        }
    }


    protected void radExistingTable_CheckedChanged(object sender, EventArgs e)
    {
        if (radNewTable.Checked)
        {
            txtPKName.Text = "ItemID";

            txtTableName.Visible = true;
            drpExistingTables.Visible = false;

            this.chkItemGUID.Checked = true;
            this.chkItemOrder.Checked = true;
            this.chkItemCreatedBy.Checked = true;
            this.chkItemCreatedWhen.Checked = true;
            this.chkItemModifiedBy.Checked = true;
            this.chkItemModifiedWhen.Checked = true;
        }
        else
        {
            txtPKName.Text = ResHelper.GetString("General.Automatic");

            txtTableName.Visible = false;
            drpExistingTables.Visible = true;

            LoadAvailableTables();

            this.chkItemGUID.Checked = false;
            this.chkItemOrder.Checked = false;
            this.chkItemCreatedBy.Checked = false;
            this.chkItemCreatedWhen.Checked = false;
            this.chkItemModifiedBy.Checked = false;
            this.chkItemModifiedWhen.Checked = false;
        }
    }

    #endregion


    #region "Step processing"

    /// <summary>
    /// 'Next' button is clicked.
    /// </summary>
    protected void wzdNewDocType_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        switch (e.CurrentStepIndex)
        {
            // Step 1   
            case 0:
                ProcessStep1(e);
                break;

            // Step 2
            case 1:
                ProcessStep2(e);
                break;

            // Step 3
            case 2:
                ProcessStep3(e);
                break;

            // Step 4
            case 3:
                ProcessStep4(e);
                break;

            // Step 5
            case 4:
                ProcessStep5(e);
                break;

            // Step 6
            case 5:
                ProcessStep6(e);
                break;

            // STEP 7
            case 6:
                ProcessStep7(e);
                break;
        }
    }


    /// <summary>
    /// Processes the step 1 of the wizard
    /// </summary>
    private void ProcessStep1(WizardNavigationEventArgs e)
    {
        // Actions after next button click

        // Validate checkboxes first
        string errorMessage = "";

        // Display proper error message based on development mode wizard setting
        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                errorMessage = new Validator().NotEmpty(txtDisplayName.Text.Trim(), GetString("DocumentType_New.ErrorEmptyDisplayName")).
                    NotEmpty(txtCodeName.Text.Trim(), GetString("DocumentType_New.ErrorEmptyCodeName")).
                    NotEmpty(txtNamespaceName.Text.Trim(), GetString("DocumentType_New.ErrorEmptyNamespaceName")).
                    IsCodeName(txtCodeName.Text.Trim(), GetString("DocumentType_New.CodeNameIdentifier")).
                    IsIdentifier(txtNamespaceName.Text.Trim(), GetString("DocumentType_New.NamespaceNameIdentifier")).Result;
                break;

            case NewClassWizardModeEnum.Class:
                errorMessage = new Validator().NotEmpty(txtDisplayName.Text.Trim(), GetString("sysdev.class_new.ErrorEmptyDisplayName")).
                    NotEmpty(txtCodeName.Text.Trim(), GetString("sysdev.class_new.ErrorEmptyCodeName")).
                    NotEmpty(txtNamespaceName.Text.Trim(), GetString("sysdev.class_new.ErrorEmptyNamespaceName")).
                    IsCodeName(txtCodeName.Text.Trim(), GetString("sysdev.class_new.CodeNameIdentifier")).
                    IsIdentifier(txtNamespaceName.Text.Trim(), GetString("sysdev.class_new.NamespaceNameIdentifier")).Result;
                break;

            case NewClassWizardModeEnum.CustomTable:
                errorMessage = new Validator().NotEmpty(txtDisplayName.Text.Trim(), GetString("customtable.newwizzard.ErrorEmptyDisplayName")).
                    NotEmpty(txtCodeName.Text.Trim(), GetString("customtable.newwizzard.ErrorEmptyCodeName")).
                    NotEmpty(txtNamespaceName.Text.Trim(), GetString("customtable.newwizzard.ErrorEmptyNamespaceName")).
                    IsCodeName(txtCodeName.Text.Trim(), GetString("customtable.newwizzard.CodeNameIdentifier")).
                    IsIdentifier(txtNamespaceName.Text.Trim(), GetString("customtable.newwizzard.NamespaceNameIdentifier")).Result;
                break;
        }


        if (errorMessage == "")
        {
            // Set new class info
            DataClassInfo dci = new DataClassInfo();
            dci.ClassDisplayName = txtDisplayName.Text.Trim();
            dci.ClassName = txtNamespaceName.Text.Trim() + "." + txtCodeName.Text.Trim();

            // Set class type according development mode setting
            switch (Mode)
            {
                case NewClassWizardModeEnum.DocumentType:
                    dci.ClassIsDocumentType = true;
                    dci.ClassUsePublishFromTo = true;
                    break;

                case NewClassWizardModeEnum.Class:
                    dci.ClassShowAsSystemTable = false;
                    dci.ClassShowTemplateSelection = false;
                    dci.ClassIsDocumentType = false;
                    dci.ClassCreateSKU = false;
                    dci.ClassIsProduct = false;
                    dci.ClassIsMenuItemType = false;
                    dci.ClassUsesVersioning = false;
                    dci.ClassUsePublishFromTo = false;
                    break;

                case NewClassWizardModeEnum.CustomTable:
                    dci.ClassShowAsSystemTable = false;
                    dci.ClassShowTemplateSelection = false;
                    dci.ClassIsDocumentType = false;
                    dci.ClassCreateSKU = false;
                    dci.ClassIsProduct = false;
                    dci.ClassIsMenuItemType = false;
                    dci.ClassUsesVersioning = false;
                    dci.ClassUsePublishFromTo = false;
                    // Sets custom table
                    dci.ClassIsCustomTable = true;
                    break;
            }

            try
            {
                using (var tr = new CMSTransactionScope())
                {
                    // Insert new class into DB
                    DataClassInfoProvider.SetDataClass(dci);

                    // Set permissions and queries
                    switch (Mode)
                    {
                        case NewClassWizardModeEnum.DocumentType:
                            // Ensure default permissions
                            PermissionNameInfoProvider.CreateDefaultClassPermissions(dci.ClassID);

                            // Generate special document queries, for possible premature exit from wizzard
                            if (SqlGenerator.GenerateDefaultQueriesToDB)
                            {
                                SqlGenerator.GenerateQuery(dci.ClassName, QueryProvider.QUERYNAME_SEARCHTREE, SqlOperationTypeEnum.SearchTree, false, false);
                                SqlGenerator.GenerateQuery(dci.ClassName, QueryProvider.QUERYNAME_SELECTDOCUMENTS, SqlOperationTypeEnum.SelectDocuments, false, false);
                                SqlGenerator.GenerateQuery(dci.ClassName, QueryProvider.QUERYNAME_SELECTVERSIONS, SqlOperationTypeEnum.SelectVersions, false, false);
                            }
                            break;

                        case NewClassWizardModeEnum.Class:
                            break;

                        case NewClassWizardModeEnum.CustomTable:
                            // Ensure default custom table permissions
                            PermissionNameInfoProvider.CreateDefaultCustomTablePermissions(dci.ClassID);
                            break;
                    }

                    tr.Commit();
                }
            }
            catch (Exception ex)
            {
                // No movement to the next step
                e.Cancel = true;

                // Class with the same class name already exists
                lblErrorStep1.Visible = true;
                lblErrorStep1.Text = ex.Message;

                EventLogProvider.LogException("NewClassWizard", "CREATE", ex);
            }

            // Prepare next step (2)       
            if (lblErrorStep1.Text == "")
            {
                // Disable previous steps' viewstates
                DisablePreviousStepsViewStates(e.CurrentStepIndex);

                // Enable next step's viewstate
                EnableNextStepViewState(e.CurrentStepIndex);

                // Save ClassName to viewstate to use in the next steps
                ClassName = dci.ClassName;

                // Prefill textboxes in the next step with default values
                txtTableName.Text = txtNamespaceName.Text.Trim() + "_" + txtCodeName.Text.Trim();
                txtPKName.Text = FirstLetterToUpper(txtCodeName.Text.Trim() + "ID");

                wzdStep2.Title = GetString("DocumentType_New_Step2.Title");

                // Prepare next step by mode setting
                switch (Mode)
                {
                    case NewClassWizardModeEnum.DocumentType:
                        {
                            // Document type
                            lblPKName.Text = GetString("DocumentType_New.PrimaryKeyName");
                            lblTableName.Text = GetString("DocumentType_New.TableName");
                            radCustom.Text = GetString("DocumentType_New.Custom");

                            // Display container option based on the development mode setting
                            radContainer.Text = GetString("DocumentType_New.Container");
                            radContainer.Visible = true;

                            ucHeader.Description = GetString("DocumentType_New_Step2.Description");

                            // Setup the inheritance selector
                            plcDocTypeOptions.Visible = true;
                            selInherits.WhereCondition = "ClassIsCoupledClass = 1 AND ClassID <> " + dci.ClassID;
                        }
                        break;

                    case NewClassWizardModeEnum.Class:
                        {
                            // Standard class
                            lblPKName.Text = GetString("sysdev.class_new.PrimaryKeyName");
                            lblTableName.Text = GetString("sysdev.class_new.TableName");
                            radCustom.Text = GetString("sysdev.class_new.Custom");
                            lblIsMNTable.Text = GetString("sysdev.class_new.MNTable");

                            radContainer.Visible = false;
                            if (SystemDevelopmentMode)
                            {
                                plcMNClassOptions.Visible = true;
                            }

                            ucHeader.Description = GetString("sysdev.class_new_Step2.Description");
                        }
                        break;

                    case NewClassWizardModeEnum.CustomTable:
                        {
                            // Custom table
                            lblPKName.Text = GetString("customtable.newwizzard.PrimaryKeyName");
                            lblTableName.Text = GetString("customtable.newwizzard.TableName");

                            radCustom.Visible = false;
                            radContainer.Visible = false;

                            radNewTable.Text = GetString("customtable.newwizard.newtable");
                            radExistingTable.Text = GetString("customtable.newwizard.existingtable");

                            plcExisting.Visible = true;

                            // Custom tables have always ItemID as primary key
                            txtPKName.Text = "ItemID";

                            // Primary key name can't be edited
                            txtPKName.Enabled = false;

                            // Show custom tables columns options
                            plcCustomTablesOptions.Visible = true;

                            lblItemGUID.Text = GetString("customtable.newwizzard.lblItemGUID");
                            lblItemCreatedBy.Text = GetString("customtable.newwizzard.lblItemCreatedBy");
                            lblItemCreatedWhen.Text = GetString("customtable.newwizzard.lblItemCreatedWhen");
                            lblItemModifiedBy.Text = GetString("customtable.newwizzard.lblItemModifiedBy");
                            lblItemModifiedWhen.Text = GetString("customtable.newwizzard.lblItemModifiedWhen");
                            lblItemOrder.Text = GetString("customtable.newwizzard.lblItemOrder");

                            ucHeader.Description = GetString("customtable.newwizzard.Step2Description");
                        }
                        break;
                }
            }
        }
        else
        {
            // No movement to the next step
            e.Cancel = true;

            // Textboxes are not filled correctly
            lblErrorStep1.Visible = true;
            lblErrorStep1.Text = errorMessage;
        }
    }


    /// <summary>
    /// Processes the step 2 of the wizard
    /// </summary>
    private void ProcessStep2(WizardNavigationEventArgs e)
    {
        FormFieldInfo ffiPrimaryKey = null;
        DataClassInfo dci = DataClassInfoProvider.GetDataClass(ClassName);
        FormInfo fi = null;

        if (dci != null)
        {
            TableManager tm = new TableManager(null);

            using (var tr = new CMSTransactionScope())
            {
                var genDci = dci.Generalized;

                // New document type has custom attributes -> no wizard steps will be omitted
                if (radCustom.Checked)
                {
                    // Actions after next button click
                    bool fromExisting = (Mode == NewClassWizardModeEnum.CustomTable) && radExistingTable.Checked;

                    string tableName = txtTableName.Text.Trim();
                    if (fromExisting)
                    {
                        tableName = drpExistingTables.SelectedValue;
                    }

                    // Validate checkboxes first
                    string tableNameError = new Validator()
                        .NotEmpty(tableName, GetString("DocumentType_New.ErrorEmptyTableName"))
                        .IsIdentifier(tableName, GetString("class.ErrorIdentifier"))
                        .Result;

                    string primaryKeyNameEmpty = new Validator().NotEmpty(txtPKName.Text.Trim(), GetString("DocumentType_New.ErrorEmptyPKName")).Result;

                    bool columnExists = DocumentHelper.ColumnExistsInSystemTable(txtPKName.Text.Trim());

                    // Textboxes are filled correctly
                    if ((tableNameError == "") && (primaryKeyNameEmpty == "") && (!columnExists))
                    {
                        try
                        {
                            if (fromExisting)
                            {
                                // Custom table from existing table - validate the table name
                                if (!tm.TableExists(tableName))
                                {
                                    e.Cancel = true;

                                    // Table with the same name already exists
                                    ShowError(GetString("customtable.newwizard.tablenotexists"));
                                }

                                // Check primary key
                                List<string> primaryKeys = tm.GetPrimaryKeyColumns(tableName);
                                if ((primaryKeys == null) || (primaryKeys.Count != 1))
                                {
                                    e.Cancel = true;

                                    ShowError(GetString("customtable.newwizard.musthaveprimarykey"));
                                }
                            }
                            else if (SystemDevelopmentMode && (Mode == NewClassWizardModeEnum.Class))
                            {
                                // Standard class in development mode
                                tm.CreateTable(tableName, txtPKName.Text.Trim(), !chbIsMNTable.Checked);
                            }
                            else
                            {
                                tm.CreateTable(tableName, txtPKName.Text.Trim());
                            }
                        }
                        catch (Exception ex)
                        {
                            // No movement to the next step
                            e.Cancel = true;

                            // Table with the same name already exists
                            ShowError(ex.Message);
                        }

                        if ((lblErrorStep2.Text == "") && !e.Cancel)
                        {
                            // Change table owner                        
                            try
                            {
                                string owner = "";

                                // Get site related DB object owner setting when creating new wizard and global otherwise
                                switch (Mode)
                                {
                                    case NewClassWizardModeEnum.DocumentType:
                                    case NewClassWizardModeEnum.Class:
                                    case NewClassWizardModeEnum.CustomTable:
                                        owner = SqlHelperClass.GetDBSchema(CMSContext.CurrentSiteName);
                                        break;
                                }


                                if ((owner != "") && (owner.ToLowerCSafe() != "dbo"))
                                {
                                    tm.ChangeDBObjectOwner(tableName, owner);
                                    tableName = DataHelper.GetSafeOwner(owner) + "." + tableName;
                                }
                            }
                            catch
                            {
                            }

                            if (fromExisting)
                            {
                                // From existing DB table
                                dci.ClassXmlSchema = tm.GetXmlSchema(tableName);

                                string formDef = FormHelper.GetXmlFormDefinitionFromXmlSchema(dci.ClassXmlSchema, false);
                                fi = new FormInfo(formDef);
                            }
                            else
                            {
                                // Create empty form definition
                                fi = new FormInfo("<form></form>");
                                ffiPrimaryKey = new FormFieldInfo();

                                // Fill FormInfo object
                                ffiPrimaryKey.Name = txtPKName.Text;
                                ffiPrimaryKey.Caption = txtPKName.Text;
                                ffiPrimaryKey.DataType = FormFieldDataTypeEnum.Integer;
                                ffiPrimaryKey.DefaultValue = "";
                                ffiPrimaryKey.Description = "";
                                ffiPrimaryKey.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                                ffiPrimaryKey.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
                                ffiPrimaryKey.PrimaryKey = true;
                                ffiPrimaryKey.System = false;
                                ffiPrimaryKey.Visible = false;
                                ffiPrimaryKey.Size = 0;
                                ffiPrimaryKey.AllowEmpty = false;

                                // Add field to form definition
                                fi.AddFormField(ffiPrimaryKey);

                                dci.ClassXmlSchema = tm.GetXmlSchema(tableName);
                            }

                            dci.ClassTableName = tableName;
                            dci.ClassFormDefinition = fi.GetXmlDefinition();
                            dci.ClassIsCoupledClass = true;

                            dci.ClassInheritsFromClassID = ValidationHelper.GetInteger(selInherits.Value, 0);

                            // Update class in DB
                            using (CMSActionContext context = new CMSActionContext())
                            {
                                // Disable logging into event log
                                context.LogEvents = false;

                                DataClassInfoProvider.SetDataClass(dci);

                                // Ensure inherited fields
                                if (dci.ClassInheritsFromClassID > 0)
                                {
                                    DataClassInfo parentCi = DataClassInfoProvider.GetDataClass(dci.ClassInheritsFromClassID);
                                    if (parentCi != null)
                                    {
                                        FormHelper.UpdateInheritedClass(parentCi, dci);
                                    }
                                }
                            }

                            if (Mode == NewClassWizardModeEnum.CustomTable)
                            {
                                #region "Custom tables optional columns"

                                // Created by
                                if (chkItemCreatedBy.Checked && !fi.FieldExists("ItemCreatedBy"))
                                {
                                    FormFieldInfo ffi = new FormFieldInfo();

                                    // Fill FormInfo object
                                    ffi.Name = "ItemCreatedBy";
                                    ffi.Caption = "Created by";
                                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                                    ffi.DefaultValue = "";
                                    ffi.Description = "";
                                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                                    ffi.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
                                    ffi.PrimaryKey = false;
                                    ffi.System = true;
                                    ffi.Visible = false;
                                    ffi.Size = 0;
                                    ffi.AllowEmpty = true;

                                    fi.AddFormField(ffi);
                                }

                                // Created when
                                if (chkItemCreatedWhen.Checked && !fi.FieldExists("ItemCreatedWhen"))
                                {
                                    FormFieldInfo ffi = new FormFieldInfo();

                                    // Fill FormInfo object
                                    ffi.Name = "ItemCreatedWhen";
                                    ffi.Caption = "Created when";
                                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                                    ffi.DefaultValue = "";
                                    ffi.Description = "";
                                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                                    ffi.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
                                    ffi.PrimaryKey = false;
                                    ffi.System = true;
                                    ffi.Visible = false;
                                    ffi.Size = 0;
                                    ffi.AllowEmpty = true;

                                    fi.AddFormField(ffi);
                                }

                                // Modified by
                                if (chkItemModifiedBy.Checked && !fi.FieldExists("ItemModifiedBy"))
                                {
                                    FormFieldInfo ffi = new FormFieldInfo();

                                    // Fill FormInfo object
                                    ffi.Name = "ItemModifiedBy";
                                    ffi.Caption = "Modified by";
                                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                                    ffi.DefaultValue = "";
                                    ffi.Description = "";
                                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                                    ffi.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
                                    ffi.PrimaryKey = false;
                                    ffi.System = true;
                                    ffi.Visible = false;
                                    ffi.Size = 0;
                                    ffi.AllowEmpty = true;

                                    fi.AddFormField(ffi);
                                }

                                // Modified when
                                if (chkItemModifiedWhen.Checked && !fi.FieldExists("ItemModifiedWhen"))
                                {
                                    FormFieldInfo ffi = new FormFieldInfo();

                                    // Fill FormInfo object
                                    ffi.Name = "ItemModifiedWhen";
                                    ffi.Caption = "Modified when";
                                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                                    ffi.DefaultValue = "";
                                    ffi.Description = "";
                                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                                    ffi.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
                                    ffi.PrimaryKey = false;
                                    ffi.System = true;
                                    ffi.Visible = false;
                                    ffi.Size = 0;
                                    ffi.AllowEmpty = true;

                                    fi.AddFormField(ffi);
                                }

                                // Item order
                                if (chkItemOrder.Checked && !fi.FieldExists("ItemOrder"))
                                {
                                    FormFieldInfo ffi = new FormFieldInfo();

                                    // Fill FormInfo object
                                    ffi.Name = "ItemOrder";
                                    ffi.Caption = "Order";
                                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                                    ffi.DefaultValue = "";
                                    ffi.Description = "";
                                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                                    ffi.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
                                    ffi.PrimaryKey = false;
                                    ffi.System = true;
                                    ffi.Visible = false;
                                    ffi.Size = 0;
                                    ffi.AllowEmpty = true;

                                    fi.AddFormField(ffi);
                                }

                                #endregion

                                if (chkItemGUID.Checked && !fi.FieldExists("ItemGUID"))
                                {
                                    FormFieldInfo ffiGuid = new FormFieldInfo();

                                    // Fill FormInfo object
                                    ffiGuid.Name = "ItemGUID";
                                    ffiGuid.Caption = "GUID";
                                    ffiGuid.DataType = FormFieldDataTypeEnum.GUID;
                                    ffiGuid.DefaultValue = "";
                                    ffiGuid.Description = "";
                                    ffiGuid.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                                    ffiGuid.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
                                    ffiGuid.PrimaryKey = false;
                                    ffiGuid.System = true;
                                    ffiGuid.Visible = false;
                                    ffiGuid.Size = 0;
                                    ffiGuid.AllowEmpty = false;

                                    fi.AddFormField(ffiGuid);
                                }

                                // Update table structure - columns could be added
                                bool old = TableManager.UpdateSystemFields;

                                TableManager.UpdateSystemFields = true;

                                string schema = fi.GetXmlDefinition();
                                tm.UpdateTableBySchema(tableName, schema);

                                TableManager.UpdateSystemFields = old;

                                // Update xml schema and form definition
                                dci.ClassFormDefinition = schema;
                                dci.ClassXmlSchema = tm.GetXmlSchema(dci.ClassTableName);

                                using (CMSActionContext context = new CMSActionContext())
                                {
                                    // Disable logging into event log
                                    context.LogEvents = false;

                                    DataClassInfoProvider.SetDataClass(dci);
                                }
                            }

                            // Remember that no steps were omitted
                            SomeStepsOmitted = false;


                            // Prepare next step (3)

                            // Disable previous steps' viewstates
                            DisablePreviousStepsViewStates(e.CurrentStepIndex);

                            // Enable next step's viewstate
                            EnableNextStepViewState(e.CurrentStepIndex);

                            // Set field editor class name
                            FieldEditor.ClassName = ClassName;

                            // Fill field editor in the next step
                            FieldEditor.Reload(null);

                            wzdStep3.Title = GetString("general.fields");

                            // Set new step header based on the development mode setting
                            switch (Mode)
                            {
                                case NewClassWizardModeEnum.DocumentType:
                                    ucHeader.Description = GetString("DocumentType_New_Step3.Description");
                                    break;

                                case NewClassWizardModeEnum.Class:
                                    ucHeader.Description = GetString("sysdev.class_new_Step3.Description");
                                    break;

                                case NewClassWizardModeEnum.CustomTable:
                                    ucHeader.Description = GetString("customtable.newwizzard.Step3Description");
                                    break;
                            }
                        }
                    }
                    // Some textboxes are not filled correctly
                    else
                    {
                        // Prepare current step (2)

                        // No movement to the next step
                        e.Cancel = true;

                        // Show errors
                        lblTableNameError.Text = tableNameError;
                        lblPKNameError.Text = primaryKeyNameEmpty;

                        if (columnExists)
                        {
                            lblErrorStep2.Text = GetString("DocumentType_New_Step2.ErrorColumnExists");
                            lblErrorStep2.Visible = true;
                        }

                        wzdStep2.Title = GetString("DocumentType_New_Step2.Title");

                        // Reset the header
                        switch (Mode)
                        {
                            case NewClassWizardModeEnum.DocumentType:
                                ucHeader.Description = GetString("DocumentType_New_Step2.Description");
                                break;

                            case NewClassWizardModeEnum.Class:
                                ucHeader.Description = GetString("sysdev.class_new_Step2.Description");
                                break;

                            case NewClassWizardModeEnum.CustomTable:
                                ucHeader.Description = GetString("customtable.newwizzard.Step2Description");
                                break;
                        }
                    }
                }
                // New document type is only the container -> some wizard steps will be omitted
                else
                {
                    // Actions after next button click

                    dci.ClassIsCoupledClass = false;

                    // Update class in DB
                    using (CMSActionContext context = new CMSActionContext())
                    {
                        // Disable logging into event log
                        context.LogEvents = false;

                        DataClassInfoProvider.SetDataClass(dci);
                    }

                    // Remember that some steps were omitted
                    SomeStepsOmitted = true;
                    IsContainer = true;


                    // Prepare next step (5) - skip steps 3 and 4

                    // Disable previous steps' viewstates
                    DisablePreviousStepsViewStates(3);

                    // Enable next step's viewstate
                    EnableNextStepViewState(3);

                    PrepareStep5();
                    // Go to the step 5 (indexed from 0)  
                    wzdNewDocType.ActiveStepIndex = 4;
                }

                // Create new icon if the wizard is used to create new document type
                if (Mode == NewClassWizardModeEnum.DocumentType)
                {
                    string sourceFile = "";
                    string destFile = GetDocumentTypeIconUrl(ClassName, false);
                    string sourceLargeFile = "";
                    string destLargeFile = GetDocumentTypeIconPath(ClassName, "48x48", false);

                    // If class is not coupled class
                    if (SomeStepsOmitted)
                    {
                        sourceFile = GetDocumentTypeIconPath("defaultcontainer", "", true);
                        sourceLargeFile = GetDocumentTypeIconPath("defaultcontainer", "48x48", true);
                    }
                    else
                    {
                        sourceFile = GetDocumentTypeIconPath("default", "", true);
                        sourceLargeFile = GetDocumentTypeIconPath("default", "48x48", true);
                    }

                    if (SettingsKeyProvider.DevelopmentMode)
                    {
                        // Ensure '.gif' image for large icon in development mode
                        sourceLargeFile = sourceLargeFile.Replace(".png", ".gif");
                    }

                    // Ensure same extension
                    if (sourceFile.ToLowerCSafe().EndsWithCSafe(".gif"))
                    {
                        destFile = destFile.Replace(".png", ".gif");
                    }
                    // Ensure same extension
                    if (sourceLargeFile.ToLowerCSafe().EndsWithCSafe(".gif"))
                    {
                        destLargeFile = destLargeFile.Replace(".png", ".gif");
                    }

                    if (!FileHelper.FileExists(destFile))
                    {
                        try
                        {
                            // Create new document type icon via copying default icon
                            File.Copy(Server.MapPath(sourceFile), Server.MapPath(destFile), false);
                        }
                        catch
                        {
                            FieldEditor.ShowError(string.Format(GetString("DocumentType_New_Step2.ErrorCopyIcon"), sourceFile, destFile));
                        }
                    }

                    // Copy large file icon
                    if (!FileHelper.FileExists(destLargeFile))
                    {
                        try
                        {
                            // Create new document type large icon via copying default icon
                            File.Copy(Server.MapPath(sourceLargeFile), Server.MapPath(destLargeFile), false);
                        }
                        catch
                        {
                            FieldEditor.ShowError(string.Format(GetString("DocumentType_New_Step2.ErrorCopyIcon"), sourceLargeFile, destLargeFile));
                        }
                    }
                }

                if (!e.Cancel)
                {
                    tr.Commit();
                }
            }
        }
    }


    /// <summary>
    /// Processes the step 3 of the wizard
    /// </summary>
    private void ProcessStep3(WizardNavigationEventArgs e)
    {
        // Actions after next button click
        DataClassInfo dci = DataClassInfoProvider.GetDataClass(ClassName);

        var genDci = dci.Generalized;

        List<FormFieldInfo> ffiVisibleFields = null;

        // Ensure actual form info
        FormHelper.ClearFormInfos(true);

        // Get and load form definition
        FormInfo fi = FormHelper.GetFormInfo(dci.ClassName, false);

        // Get all visible fields
        ffiVisibleFields = fi.GetFields(true, false);

        if (fi.GetFields(true, true).Count() < 2)
        {
            e.Cancel = true;
            FieldEditor.ShowError(GetString("DocumentType_New_Step3.TableMustHaveCustomField"));
        }
        else
        {
            // Generate basic queries
            if (SqlGenerator.GenerateDefaultQueriesToDB)
            {
                SqlGenerator.GenerateQuery(ClassName, QueryProvider.QUERYNAME_SELECT, SqlOperationTypeEnum.SelectQuery, false, false);
                SqlGenerator.GenerateQuery(ClassName, QueryProvider.QUERYNAME_DELETE, SqlOperationTypeEnum.DeleteQuery, false, false);
                SqlGenerator.GenerateQuery(ClassName, QueryProvider.QUERYNAME_INSERT, SqlOperationTypeEnum.InsertQuery, true, false);
                SqlGenerator.GenerateQuery(ClassName, QueryProvider.QUERYNAME_UPDATE, SqlOperationTypeEnum.UpdateQuery, false, false);
                SqlGenerator.GenerateQuery(ClassName, QueryProvider.QUERYNAME_SELECTALL, SqlOperationTypeEnum.SelectAll, false, false);
            }

            // Different behavior by mode
            switch (Mode)
            {
                case NewClassWizardModeEnum.DocumentType:
                    {
                        TableManager tm = new TableManager(null);

                        // Create new view if doesn't exist
                        string viewName = DataHelper.GetViewName(dci.ClassTableName, null);

                        // Create view for document types
                        if (!tm.ViewExists(viewName))
                        {
                            tm.CreateView(viewName, SqlGenerator.GetSqlQuery(ClassName, SqlOperationTypeEnum.SelectView, null));
                        }

                        // Generate additional queries when creating new document type
                        if (SqlGenerator.GenerateDefaultQueriesToDB)
                        {
                            SqlGenerator.GenerateQuery(ClassName, QueryProvider.QUERYNAME_SEARCHTREE, SqlOperationTypeEnum.SearchTree, false, false);
                            SqlGenerator.GenerateQuery(ClassName, QueryProvider.QUERYNAME_SELECTDOCUMENTS, SqlOperationTypeEnum.SelectDocuments, false, false);
                            SqlGenerator.GenerateQuery(ClassName, QueryProvider.QUERYNAME_SELECTVERSIONS, SqlOperationTypeEnum.SelectVersions, false, false);
                        }

                        // If new document type is created prepare next step otherwise skip steps 4, 5 and 6

                        // Disable previous steps' viewstates
                        DisablePreviousStepsViewStates(e.CurrentStepIndex);

                        // Enable next step's viewstate
                        EnableNextStepViewState(e.CurrentStepIndex);

                        // Add implicit value to the list
                        lstFields.Items.Add(new ListItem(GetString("DocumentType_New_Step4.ImplicitDocumentName"), ""));

                        if (ffiVisibleFields != null)
                        {
                            // Add visible fields' names to the list except primary-key field
                            foreach (FormFieldInfo ffi in ffiVisibleFields)
                            {
                                if (!ffi.PrimaryKey)
                                {
                                    lstFields.Items.Add(new ListItem(ffi.Name, ffi.Name));
                                }
                            }
                        }

                        lblSelectField.Text = GetString("DocumentType_New_Step4.DocumentName");
                        wzdStep4.Title = GetString("DocumentType_New_Step4.Title");
                        ucHeader.Description = GetString("DocumentType_New_Step4.Description");
                    }
                    break;

                case NewClassWizardModeEnum.Class:
                    {
                        // Update class in DB
                        using (CMSActionContext context = new CMSActionContext())
                        {
                            // Disable logging into event log
                            context.LogEvents = false;

                            DataClassInfoProvider.SetDataClass(dci);
                        }

                        // Remember that some steps were omitted
                        SomeStepsOmitted = true;

                        // Prepare next step (7) - skip steps 4, 5 and 6

                        // Disable previous steps' viewstates
                        DisablePreviousStepsViewStates(5);

                        // Enable next step's viewstate
                        EnableNextStepViewState(5);

                        PrepareStep8();
                        // Go to the step 8 (indexed from 0)  
                        wzdNewDocType.ActiveStepIndex = 7;
                    }
                    break;

                case NewClassWizardModeEnum.CustomTable:
                    {
                        // Generate additional delete all query
                        if (SqlGenerator.GenerateDefaultQueriesToDB)
                        {
                            SqlGenerator.GenerateQuery(ClassName, QueryProvider.QUERYNAME_DELETEALL, SqlOperationTypeEnum.DeleteAll, false, false);
                        }

                        // Update class in DB
                        using (CMSActionContext context = new CMSActionContext())
                        {
                            // Disable logging into event log
                            context.LogEvents = false;

                            DataClassInfoProvider.SetDataClass(dci);
                        }

                        // Remember that some steps were omitted, 
                        SomeStepsOmitted = true;

                        // Prepare next step (6) - skip steps 4, 5 

                        // Disable previous steps' viewstates
                        DisablePreviousStepsViewStates(4);

                        // Enable next step's viewstate
                        EnableNextStepViewState(4);

                        PrepareStep6();
                        // Go to the step 6 (indexed from 0) 
                        wzdNewDocType.ActiveStepIndex = 5;
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// Processes the step 4 of the wizard
    /// </summary>
    private void ProcessStep4(WizardNavigationEventArgs e)
    {
        // Actions after next button click
        DataClassInfo dci = DataClassInfoProvider.GetDataClass(ClassName);
        dci.ClassNodeNameSource = lstFields.SelectedValue;

        // Update node name source in DB
        using (CMSActionContext context = new CMSActionContext())
        {
            // Disable logging into event log
            context.LogEvents = false;

            DataClassInfoProvider.SetDataClass(dci);
        }

        // Prepare next step (5)

        // Disable previous steps' viewstates
        DisablePreviousStepsViewStates(e.CurrentStepIndex);

        // Enable next step's viewstate
        EnableNextStepViewState(e.CurrentStepIndex);

        PrepareStep5();
    }


    /// <summary>
    /// Processes the step 5 of the wizard
    /// </summary>
    private void ProcessStep5(WizardNavigationEventArgs e)
    {
        // Actions after next button click

        int parentClassID = 0;
        int childClassID = DataClassInfoProvider.GetDataClass(ClassName).ClassID;

        // Add parent classes
        string selectedClasses = ValidationHelper.GetString(usParentTypes.Value, String.Empty);
        if (!String.IsNullOrEmpty(selectedClasses))
        {
            string[] classes = selectedClasses.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (classes != null)
            {
                // Add all new items to site
                foreach (string item in classes)
                {
                    parentClassID = ValidationHelper.GetInteger(item, 0);
                    AllowedChildClassInfoProvider.AddAllowedChildClass(parentClassID, childClassID);
                }
            }
        }

        // Prepare next step (6)

        // Disable previous steps' viewstates
        DisablePreviousStepsViewStates(e.CurrentStepIndex);

        // Enable next step's viewstate
        EnableNextStepViewState(e.CurrentStepIndex);

        PrepareStep6();
    }


    /// <summary>
    /// Processes the step 5 of the wizard
    /// </summary>
    private void ProcessStep6(WizardNavigationEventArgs e)
    {
        // Actions after next button click

        int siteId = 0;
        DataClassInfo dci = DataClassInfoProvider.GetDataClass(ClassName);
        int classId = dci.ClassID;
        bool isCustomTable = dci.ClassIsCustomTable;
        bool licenseCheck = true;

        string selectedSite = ValidationHelper.GetString(usSites.Value, String.Empty);
        if (selectedSite == "0")
        {
            selectedSite = "";
        }
        string[] sites = selectedSite.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if ((licenseCheck) && (sites != null))
        {
            foreach (string site in sites)
            {
                siteId = ValidationHelper.GetInteger(site, 0);

                SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
                if (si != null)
                {
                    if (isCustomTable)
                    {
                        if (!CustomTableItemProvider.LicenseVersionCheck(si.DomainName, FeatureEnum.CustomTables, VersionActionEnum.Insert))
                        {
                            lblErrorStep6.Visible = true;
                            lblErrorStep6.Text = GetString("LicenseVersion.CustomTables");
                            e.Cancel = true;
                            licenseCheck = false;
                        }
                    }

                    if (licenseCheck)
                    {
                        ClassSiteInfoProvider.AddClassToSite(classId, siteId);

                        // Clear custom tables count
                        if (isCustomTable)
                        {
                            CustomTableItemProvider.ClearCustomLicHash();
                        }
                    }
                }
            }
        }

        // If is moving to the new step
        if (licenseCheck)
        {
            // Create default transformations
            switch (Mode)
            {
                case NewClassWizardModeEnum.DocumentType:

                    if (!SomeStepsOmitted)
                    {
                        CreateDefaultTransformations(classId, true);
                    }
                    else
                    {
                        // Apply on if new document type was created
                        lblTableCreated.Visible = false;
                    }
                    break;

                case NewClassWizardModeEnum.CustomTable:
                    CreateDefaultTransformations(classId, false);
                    break;
            }
        }
        else
        {
            PrepareStep5();
            return;
        }


        if (((Mode == NewClassWizardModeEnum.DocumentType) && (dci.ClassIsCoupledClass)) || (Mode == NewClassWizardModeEnum.CustomTable))
        {
            PrepareStep7();
            SearchFields.DisplayOkButton = false;
            SearchFields.ItemID = dci.ClassID;
            SearchFields.ReloadSearch(true);
        }
        else
        {
            DisablePreviousStepsViewStates(6);
            EnableNextStepViewState(6);
            PrepareStep8();
            wzdNewDocType.ActiveStepIndex = 7;

            // Explicitly log the synchronization with all changes
            using (CMSActionContext context = new CMSActionContext())
            {
                // Disable logging into event log
                context.LogEvents = false;

                SynchronizationHelper.LogObjectUpdate(dci);
            }
        }
    }


    /// <summary>
    /// Processes the step 7 of the wizard
    /// </summary>
    private void ProcessStep7(WizardNavigationEventArgs e)
    {
        // Prepare next step (7) - Finish step
        DataClassInfo dci = DataClassInfoProvider.GetDataClass(ClassName);
        SearchFields.SaveData();

        // Disable previous steps' viewstates
        DisablePreviousStepsViewStates(e.CurrentStepIndex);

        // Enable next step's viewstate
        EnableNextStepViewState(e.CurrentStepIndex);

        PrepareStep8();

        // Explicitly log the synchronization with all changes
        using (CMSActionContext context = new CMSActionContext())
        {
            // Disable logging into event log
            context.LogEvents = false;

            SynchronizationHelper.LogObjectUpdate(dci);
        }
    }


    /// <summary>
    /// Finish button is clicked.
    /// </summary>
    protected void wzdNewDocType_FinishButtonClick(object sender, EventArgs e)
    {
        string editUrl = "";
        string newObjId = DataClassInfoProvider.GetDataClass(ClassName).ClassID.ToString();

        // Redirect to the document type edit site
        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                editUrl = "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit.aspx?documenttypeid=" + newObjId;
                break;

            case NewClassWizardModeEnum.Class:
                editUrl = "~/CMSModules/SystemDevelopment/Development/Classes/Class_Edit.aspx?classid=" + newObjId;
                break;

            case NewClassWizardModeEnum.CustomTable:
                editUrl = "~/CMSModules/CustomTables/CustomTable_Edit.aspx?customtableid=" + newObjId;
                break;
        }

        URLHelper.Redirect(editUrl);
    }

    #endregion


    #region "Step preparing methods"
    
    /// <summary>
    /// Prepares and fills controls in the step 5.
    /// </summary>
    private void PrepareStep5()
    {
        wzdStep5.Title = GetString("DocumentType_New_Step5.Title");
        ucHeader.Description = GetString("DocumentType_New_Step5.Description");
    }


    /// <summary>
    /// Prepares and fills controls in the step 6.
    /// </summary>
    private void PrepareStep6()
    {
        wzdStep6.Title = GetString("DocumentType_New_Step6.Title");
        ucHeader.Description = GetString(Step6Description);

        // Preselect current site
        usSites.Value = CMSContext.CurrentSiteID;

        // Reload to have preselect site visible
        usSites.Reload(true);
    }


    /// <summary>
    /// Prepares and fills controls in the step 7.
    /// </summary>
    private void PrepareStep7()
    {
        wzdStep7.Title = GetString("DocumentType_New_Step7.Title");
        ucHeader.Description = GetString("DocumentType_New_Step7.Description");
    }


    /// <summary>
    /// Prepares and fills controls in the step 8.
    /// </summary>
    private void PrepareStep8()
    {
        ucHeader.DescriptionVisible = false;
        wzdStep8.Title = GetString("documenttype_new_step8.title");
        lblInfoStep8.Text = GetString("documenttype_new_step8.info");

        // Display final messages based on mode
        switch (Mode)
        {
            case NewClassWizardModeEnum.DocumentType:
                lblEditingFormCreated.Text = GetString("documenttype_new_step8_finished.editingformcreated");
                lblQueryCreated.Text = GetString("documenttype_new_step8_finished.querycreated");
                lblDocumentCreated.Text = GetString("documenttype_new_step8_finished.documentcreated");
                lblChildTypesAdded.Text = GetString("documenttype_new_step8_finished.childtypesadded");
                lblSitesSelected.Text = GetString("documenttype_new_step8_finished.sitesselected");
                lblTransformationCreated.Text = GetString("documenttype_new_step8_finished.transformationcreated");
                lblPermissionNameCreated.Text = GetString("documenttype_new_step8_finished.permissionnamecreated");
                lblDefaultIconCreated.Text = GetString("documenttype_new_step8_finished.defaulticoncreated");
                lblSearchSpecificationCreated.Text = GetString("documenttype_new_step8_finished.searchspecificationcreated");

                // Hide some messages if creating container document type
                lblTransformationCreated.Visible = !IsContainer;
                lblSearchSpecificationCreated.Visible = !IsContainer;
                break;

            case NewClassWizardModeEnum.Class:
                lblQueryCreated.Text = GetString("documenttype_new_step8_finished.querycreated");
                lblDocumentCreated.Text = GetString("sysdev.class_new_step8_finished.documentcreated");
                lblTableCreated.Text = GetString("sysdev.class_new_step8_finished.tablecreated");
                break;

            case NewClassWizardModeEnum.CustomTable:
                lblDocumentCreated.Text = GetString("customtable.newwizzard.CustomTableCreated");
                lblSitesSelected.Text = GetString("customtable.newwizzard.SitesSelected");
                lblTransformationCreated.Text = GetString("customtable.newwizzard.TransformationCreated");
                lblQueryCreated.Text = GetString("customtable.newwizzard.QueryCreated");
                lblPermissionNameCreated.Text = GetString("customtable.newwizzard.PermissionNameCreated");
                lblSearchSpecificationCreated.Text = GetString("customtable.newwizzard.searchspecificationcreated");
                break;
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Creates default transformations for the classid.
    /// </summary>
    /// <param name="classId">Class id</param>
    private void CreateDefaultTransformations(int classId, bool isDocument)
    {
        using (CMSActionContext context = new CMSActionContext())
        {
            // Disable logging of tasks
            context.DisableLogging();

            // Create default transformation
            TransformationInfo ti = new TransformationInfo();

            string classFormDefinition = DataClassInfoProvider.GetDataClass(ClassName).ClassFormDefinition;

            ti.TransformationName = ValidationHelper.GetCodeName(GetString("TransformationName.Default"));
            ti.TransformationFullName = ClassName + "." + ti.TransformationName;
            ti.TransformationCode = TransformationInfoProvider.GenerateTransformationCode(classFormDefinition, TransformationTypeEnum.Ascx, ClassName);
            ti.TransformationType = TransformationTypeEnum.Ascx;
            ti.TransformationClassID = classId;

            // Set default transformation in DB
            TransformationInfoProvider.SetTransformation(ti);

            // Create preview transformation which has the same transformation code
            ti = ti.Clone(true);

            ti.TransformationName = ValidationHelper.GetCodeName(GetString("TransformationName.Preview"));
            ti.TransformationFullName = ClassName + "." + ti.TransformationName;

            // Set default transformation in DB
            TransformationInfoProvider.SetTransformation(ti);

            // Test if class is standard document type
            if (isDocument)
            {
                // Create RSS transformation
                ti = ti.Clone(true);

                ti.TransformationName = ValidationHelper.GetCodeName(GetString("TransformationName.RSS"));
                ti.TransformationFullName = ClassName + "." + ti.TransformationName;
                ti.TransformationCode = TransformationInfoProvider.GenerateTransformationCode(classFormDefinition, TransformationTypeEnum.Ascx, ClassName, DefaultTransformationTypeEnum.RSS);

                // Set RSS transformation in DB
                TransformationInfoProvider.SetTransformation(ti);

                // Create Atom transformation
                ti = ti.Clone(true);

                ti.TransformationName = ValidationHelper.GetCodeName(GetString("TransformationName.Atom"));
                ti.TransformationFullName = ClassName + "." + ti.TransformationName;
                ti.TransformationCode = TransformationInfoProvider.GenerateTransformationCode(classFormDefinition, TransformationTypeEnum.Ascx, ClassName, DefaultTransformationTypeEnum.Atom);

                // Set Atom transformation in DB
                TransformationInfoProvider.SetTransformation(ti);
            }
        }
    }

    
    /// <summary>
    /// Disable Viewstates of the current and previous steps.
    /// </summary>
    /// <param name="actualViewState">Current step</param>
    private void DisablePreviousStepsViewStates(int currentStep)
    {
        switch (currentStep)
        {
            // Step 1
            case 0:
                wzdStep1.EnableViewState = false;
                break;

            // Step 2
            case 1:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                break;

            // Step 3
            case 2:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                wzdStep3.EnableViewState = false;
                break;

            // Step 4
            case 3:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                wzdStep3.EnableViewState = false;
                wzdStep4.EnableViewState = false;
                break;

            // Step 5
            case 4:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                wzdStep3.EnableViewState = false;
                wzdStep4.EnableViewState = false;
                wzdStep5.EnableViewState = false;
                break;

            // Step 6
            case 5:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                wzdStep3.EnableViewState = false;
                wzdStep4.EnableViewState = false;
                wzdStep5.EnableViewState = false;
                wzdStep6.EnableViewState = false;
                break;

            // Step 7
            case 6:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                wzdStep3.EnableViewState = false;
                wzdStep4.EnableViewState = false;
                wzdStep5.EnableViewState = false;
                wzdStep6.EnableViewState = false;
                wzdStep7.EnableViewState = false;
                break;

            // Step 8
            case 7:
                wzdStep1.EnableViewState = false;
                wzdStep2.EnableViewState = false;
                wzdStep3.EnableViewState = false;
                wzdStep4.EnableViewState = false;
                wzdStep5.EnableViewState = false;
                wzdStep6.EnableViewState = false;
                wzdStep7.EnableViewState = false;
                wzdStep8.EnableViewState = false;
                break;
        }
    }


    /// <summary>
    /// Enable Viewstate of the next step.
    /// </summary>
    /// <param name="actualViewState">Actual step</param>
    private void EnableNextStepViewState(int actualStep)
    {
        switch (actualStep)
        {
            case 0:
                wzdStep2.EnableViewState = true;
                break;

            case 1:
                wzdStep3.EnableViewState = true;
                break;

            case 2:
                wzdStep4.EnableViewState = true;
                break;

            case 3:
                wzdStep5.EnableViewState = true;
                break;

            case 4:
                wzdStep6.EnableViewState = true;
                break;

            case 5:
                wzdStep7.EnableViewState = true;
                break;

            case 6:
                wzdStep8.EnableViewState = true;
                break;
        }
    }


    /// <summary>
    /// Returns title of the current step.
    /// </summary>
    /// <param name="currentStep">Current step index (counted from 0)</param>
    private string GetCurrentStepTitle(int currentStep)
    {
        string currentStepTitle = "";

        switch (currentStep)
        {
            case 0:
                currentStepTitle = wzdStep1.Title;
                break;

            case 1:
                currentStepTitle = wzdStep2.Title;
                break;

            case 2:
                currentStepTitle = wzdStep3.Title;
                break;

            case 3:
                currentStepTitle = wzdStep4.Title;
                break;

            case 4:
                currentStepTitle = wzdStep5.Title;
                break;

            case 5:
                currentStepTitle = wzdStep6.Title;
                break;

            case 6:
                currentStepTitle = wzdStep7.Title;
                break;

            case 7:
                currentStepTitle = wzdStep8.Title;
                break;
        }

        return currentStepTitle;
    }


    /// <summary>
    /// Converts first letter of the input string to uppper case.
    /// </summary>
    /// <param name="text">Input text</param>
    private string FirstLetterToUpper(string text)
    {
        if ((text != null) & (text.Length > 0))
        {
            // Get first char in upper case
            string firstChar = text.Substring(0, 1).ToUpperCSafe();

            return firstChar + text.Remove(0, 1);
        }
        return "";
    }


    protected void LoadAvailableTables()
    {
        string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME NOT IN (SELECT ClassTableName FROM CMS_Class) AND TABLE_NAME NOT IN ('Analytics_Index', 'CMS_Session', 'OM_ScoreContactRule', 'Analytics_ExitPages', 'CMS_WebFarmServerTask', 'sysdiagrams', 'Temp_WebPart')";

        drpExistingTables.DataSource = ConnectionHelper.ExecuteQuery(query, null, QueryTypeEnum.SQLQuery, false);
        drpExistingTables.DataBind();
    }

    #endregion
}