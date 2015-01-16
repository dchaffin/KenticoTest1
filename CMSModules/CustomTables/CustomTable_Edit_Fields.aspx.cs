using System;
using System.Data;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.URLRewritingEngine;

public partial class CMSModules_CustomTables_CustomTable_Edit_Fields : CMSCustomTablesPage
{
    #region "Variables"

    protected DataClassInfo dci = null;
    protected string className = null;
    private EventLogProvider mEventLog = null;
    private FormInfo mFormInfo = null;
    private HeaderAction btnGenerateGuid;

    #endregion


    #region "Properties"

    /// <summary>
    /// Event log.
    /// </summary>
    public EventLogProvider EventLog
    {
        get
        {
            if (mEventLog == null)
            {
                mEventLog = new EventLogProvider();
            }
            return mEventLog;
        }
    }


    /// <summary>
    /// Form info.
    /// </summary>
    public FormInfo FormInfo
    {
        get
        {
            if ((mFormInfo == null) && (dci != null))
            {
                mFormInfo = FormHelper.GetFormInfo(dci.ClassName, true);
            }
            return mFormInfo;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        int classId = QueryHelper.GetInteger("customtableid", 0);
        dci = DataClassInfoProvider.GetDataClass(classId);
        // Set edited object
        EditedObject = dci;
        CurrentMaster.BodyClass += " FieldEditorBody";

        btnGenerateGuid = new HeaderAction()
        {
            Tooltip = GetString("customtable.GenerateGUID"),
            Text = GetString("customtable.GenerateGUIDField"),
            ImageUrl = GetImageUrl("/Design/Controls/UniGrid/Actions/Rebuild.png"),
            Visible = false,
            CommandName = "createguid",
        };
        FieldEditor.HeaderActions.AddAction(btnGenerateGuid);
        FieldEditor.HeaderActions.ActionPerformed += (s, ea) => { if (ea.CommandName == "createguid") CreateGUID(); };

        // Class exists
        if (dci != null)
        {
            className = dci.ClassName;
            if (dci.ClassIsCoupledClass)
            {
                FieldEditor.Visible = true;
                FieldEditor.ClassName = className;
                FieldEditor.Mode = FieldEditorModeEnum.CustomTable;
                FieldEditor.OnFieldNameChanged += FieldEditor_OnFieldNameChanged;
            }
            else
            {
                FieldEditor.ShowError(GetString("customtable.ErrorNoFields"));
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Class exists
        if (dci != null)
        {
            if (dci.ClassIsCoupledClass)
            {
                // GUID column is not present
                if ((FormInfo.GetFormField("ItemGUID") == null))
                {
                    btnGenerateGuid.Visible = true;
                    FieldEditor.ShowInformation(GetString("customtable.GUIDColumMissing"));
                }
            }

            if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("gen", false))
            {
                FieldEditor.ShowInformation(GetString("customtable.GUIDFieldGenerated"));
            }
        }
    }


    private void FieldEditor_OnFieldNameChanged(object sender, string oldFieldName, string newFieldName)
    {
        if (dci != null)
        {
            // Rename field in layout(s)
            FormHelper.RenameFieldInFormLayout(dci.ClassID, oldFieldName, newFieldName);
        }
    }


    /// <summary>
    /// Adds GUID field to form definition.
    /// </summary>
    private void CreateGUID()
    {
        try
        {
            // Create GUID field
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

            FormInfo.AddFormField(ffiGuid);

            // Update table structure - columns could be added
            bool old = TableManager.UpdateSystemFields;
            TableManager.UpdateSystemFields = true;
            string schema = FormInfo.GetXmlDefinition();

            TableManager tm = new TableManager(null);
            tm.UpdateTableBySchema(dci.ClassTableName, schema);

            TableManager.UpdateSystemFields = old;

            // Update xml schema and form definition
            dci.ClassFormDefinition = schema;
            dci.ClassXmlSchema = tm.GetXmlSchema(dci.ClassTableName);

            dci.Generalized.LogEvents = false;

            // Save the data
            DataClassInfoProvider.SetDataClass(dci);

            dci.Generalized.LogEvents = true;

            // Generate default queries
            SqlGenerator.GenerateDefaultQueries(dci, true, false);

            // Clear cached data
            CMSObjectHelper.RemoveReadOnlyObjects(CustomTableItemProvider.GetObjectType(className), true);
            CustomTableItemProvider.Remove(className, true);

            // Clear the object type hashtable
            ProviderStringDictionary.ReloadDictionaries(className, true);

            // Clear the classes hashtable
            ProviderStringDictionary.ReloadDictionaries("cms.class", true);

            // Clear class strucures
            ClassStructureInfo.Remove(className, true);

            // Ensure GUIDs for all items
            CustomTableItemProvider tableProvider = new CustomTableItemProvider();
            tableProvider.UpdateSystemFields = false;
            tableProvider.LogSynchronization = false;
            DataSet dsItems = tableProvider.GetItems(className, null, null);
            if (!DataHelper.DataSourceIsEmpty(dsItems))
            {
                foreach (DataRow dr in dsItems.Tables[0].Rows)
                {
                    CustomTableItem item = CustomTableItem.New(dr, className, tableProvider);
                    item.ItemGUID = Guid.NewGuid();
                    item.Update();
                }
            }

            // Log event
            UserInfo currentUser = CMSContext.CurrentUser;
            EventLog.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "Custom table", "GENERATEGUID", currentUser.UserID, currentUser.UserName, 0, null, null, string.Format(ResHelper.GetAPIString("customtable.GUIDGenerated", "Field 'ItemGUID' for custom table '{0}' was created and GUID values were generated."), dci.ClassName), 0, null);

            URLHelper.Redirect(URLHelper.AddParameterToUrl(URLRewriter.CurrentURL, "gen", "1"));
        }
        catch (Exception ex)
        {
            FieldEditor.ShowError(GetString("customtable.ErrorGUID") + ex.Message);

            // Log event
            EventLog.LogEvent("Custom table", "GENERATEGUID", ex);
        }
    }
}