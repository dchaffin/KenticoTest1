using System;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using CMS.CMSHelper;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.URLRewritingEngine;
using CMS.WorkflowEngine;
using CMS.PortalEngine;
using CMS.Scheduler;
using CMS.DocumentEngine;


/// <summary>
/// Class carrying the code to perform the upgrade procedure.
/// </summary>
public static class UpgradeProcedure
{
    #region "Variables"

    // path to upgrade package
    private static string mUpgradePackagePath = null;

    private static string mWebsitePath = null;

    #endregion


    #region "Methods"

    /// <summary>
    /// Runs the update procedure.
    /// </summary>
    /// <param name="conn">Connection to use</param>
    public static void Update(GeneralConnection conn)
    {
        if (SqlHelper.IsDatabaseAvailable)
        {
            try
            {
                string version = SettingsKeyProvider.GetStringValue("CMSDataVersion");
                switch (version.ToLowerCSafe())
                {
                    case "7.0rc":
                        Update70RC();
                        break;

                    case "6.0":
                        Update60();
                        break;

                    case "5.5r2":
                        Update55R2();
                        break;
                }
            }
            catch
            {
            }
        }
    }

    #endregion


    #region "Update 7.0 RC"

    public static void Update70RC()
    {
        EventLogProvider.LogInformation("Upgrade to 7.0", "Upgrade - Start");

        DataClassInfo dci = null;

        #region "Ecommerce - SKU"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.sku");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = null;

                    ffi = fi.GetFormField("SKURetailPrice");
                    if (ffi != null)
                    {
                        ffi.Caption = "{$com.sku.listprice$}";
                        fi.UpdateFormField("SKURetailPrice", ffi);
                    }

                    ffi = fi.GetFormField("SKUValidity");
                    if (ffi != null)
                    {
                        ffi.Settings["AutoPostBack"] = "True";
                        fi.UpdateFormField("SKUValidity", ffi);
                    }

                    ffi = fi.GetFormField("SKUConversionValue");
                    if (ffi != null)
                    {
                        ffi.Settings.Clear();
                        ffi.Settings["controlname"] = "textbox_double_validator";
                        fi.UpdateFormField("SKUConversionValue", ffi);
                    }

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_SKU");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Refresh views
                    tm.RefreshCustomViews("COM_SKU");
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Ecommerce.SKU - Upgrade", "Upgrade", ex);
        }

        #endregion

        #region "Newsletter - Subscriber"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("newsletter.subscriber");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "SubscriberCustomData";
                    ffi.DataType = FormFieldDataTypeEnum.LongText;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("Newsletter_Subscriber");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Refresh views
                    tm.RefreshCustomViews("Newsletter_Subscriber");
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Newsletter.Subscriber - Upgrade", "Upgrade", ex);
        }

        #endregion

        #region "OM - Contact"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("OM.Contact");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = null;

                    ffi = fi.GetFormField("ContactStatusID");
                    if (ffi != null)
                    {
                        ffi.Caption = "Status";
                        fi.UpdateFormField("ContactStatusID", ffi);
                    }

                    ffi = fi.GetFormField("ContactOwnerUserID");
                    if (ffi != null)
                    {
                        ffi.Caption = "Owner";
                        fi.UpdateFormField("ContactOwnerUserID", ffi);
                    }

                    ffi = fi.GetFormField("ContactMergedWithContactID");
                    if (ffi != null)
                    {
                        ffi.Visible = false;
                        fi.UpdateFormField("ContactMergedWithContactID", ffi);
                    }

                    ffi = fi.GetFormField("ContactIsAnonymous");
                    if (ffi != null)
                    {
                        ffi.Visible = false;
                        fi.UpdateFormField("ContactIsAnonymous", ffi);
                    }

                    ffi = fi.GetFormField("ContactSiteID");
                    if (ffi != null)
                    {
                        ffi.Visible = false;
                        fi.UpdateFormField("ContactSiteID", ffi);
                    }

                    ffi = fi.GetFormField("ContactGUID");
                    if (ffi != null)
                    {
                        ffi.Visible = false;
                        fi.UpdateFormField("ContactGUID", ffi);
                    }

                    ffi = fi.GetFormField("ContactLastModified");
                    if (ffi != null)
                    {
                        ffi.Visible = false;
                        fi.UpdateFormField("ContactLastModified", ffi);
                    }

                    ffi = fi.GetFormField("ContactGlobalContactID");
                    if (ffi != null)
                    {
                        ffi.Caption = "Global Contact ID";
                        ffi.Visible = false;
                        fi.UpdateFormField("ContactGlobalContactID", ffi);
                    }

                    ffi = fi.GetFormField("ContactSalesForceLeadID");
                    if (ffi != null)
                    {
                        ffi.Caption = "SalesForce - Lead ID";
                        fi.UpdateFormField("ContactSalesForceLeadID", ffi);
                    }

                    ffi = fi.GetFormField("ContactSalesForceLeadReplicationDisabled");
                    if (ffi != null)
                    {
                        ffi.Caption = "SalesForce - Replication disabled";
                        fi.UpdateFormField("ContactSalesForceLeadReplicationDisabled", ffi);
                    }

                    ffi = fi.GetFormField("ContactSalesForceLeadReplicationDateTime");
                    if (ffi != null)
                    {
                        ffi.Caption = "SalesForce - Replication date";
                        ffi.Settings["DisplayNow"] = "True";
                        ffi.Settings["EditTime"] = "True";
                        fi.UpdateFormField("ContactSalesForceLeadReplicationDateTime", ffi);
                    }

                    ffi = fi.GetFormField("ContactSalesForceLeadReplicationSuspensionDateTime");
                    if (ffi != null)
                    {
                        ffi.Caption = "SalesForce - Replication suspension date";
                        ffi.Settings["DisplayNow"] = "True";
                        ffi.Settings["EditTime"] = "True";
                        fi.UpdateFormField("ContactSalesForceLeadReplicationSuspensionDateTime", ffi);
                    }

                    ffi = new FormFieldInfo();
                    ffi.Name = "ContactSalesForceLeadReplicationRequired";
                    ffi.Caption = "SalesForce - Replication required";
                    ffi.DefaultValue = "false";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("OM_Contact");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Refresh views
                    tm.RefreshCustomViews("OM_Contact");
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("OM.Contact - Upgrade", "Upgrade", ex);
        }

        #endregion

        #region "Workflow - Timeout connection"

        var allSteps = WorkflowStepInfoProvider.GetWorkflowSteps("StepType IN(2,3,9,100,101)", null, -1, "StepDefinition");
        var sourcePointDBGuids = new List<string>();
        foreach (var step in allSteps)
        {
            var sourcePoint = step.StepDefinition.SourcePoints.Find(s => s.Type == SourcePointTypeEnum.Timeout);
            if (sourcePoint != null)
            {
                sourcePointDBGuids.Add(sourcePoint.Guid.ToString());
            }
        }

        string where = SqlHelperClass.GetWhereCondition<string>("TransitionSourcePointGUID", sourcePointDBGuids, true);
        ConnectionHelper.ExecuteQuery("cms.workflowtransition.updateall", null, where, null, -1, "TransitionType = 1");

        #endregion

        #region "Workflow - Change IDs to GUIDs in scheduled task data"

        var allWorkflowTimerTasks = TaskInfoProvider.GetTasks("TaskClass = 'CMS.DocumentEngine.WorkflowTimer'", null, 0, null);
        foreach (var workFlowTimerTask in allWorkflowTimerTasks)
        {
            string[] data = workFlowTimerTask.TaskData.Split(';');
            if ((data != null) && (data.Length == 3))
            {
                int documentID = ValidationHelper.GetInteger(data[0], 0);
                CMS.DocumentEngine.TreeNode document = DocumentHelper.GetDocument(documentID, null);

                int stepID = ValidationHelper.GetInteger(data[1], 0);
                WorkflowStepInfo step = WorkflowStepInfoProvider.GetWorkflowStepInfo(stepID);

                if ((document == null) || (step == null))
                {
                    using (CMSActionContext ctx = new CMSActionContext() { LogEvents = false })
                    {
                        // Scheduled task cannot be converted
                        workFlowTimerTask.Delete();
                    }
                }
                else
                {
                    data[0] = document.DocumentGUID.ToString();
                    data[1] = step.StepGUID.ToString();

                    workFlowTimerTask.TaskName = RegexHelper.GetRegex(String.Format("{0}$", documentID)).Replace(workFlowTimerTask.TaskName, data[0]);
                    workFlowTimerTask.TaskData = String.Format("{0};{1};{2}", data[0], data[1], data[2]);

                    using (CMSActionContext ctx = new CMSActionContext() { LogEvents = false })
                    {
                        workFlowTimerTask.Update();
                    }
                }
            }
        }

        #endregion

        #region "Refresh macros security parameters"

        // Get object types
        List<string> objectTypes = new List<string>() {
            PredefinedObjectType.TRANSFORMATION,
            PredefinedObjectType.UIELEMENT,
            SiteObjectType.FORMUSERCONTROL,
            SettingsObjectType.SETTINGSKEY,
            SettingsObjectType.CLASS,
            PortalObjectType.PAGETEMPLATE,
            WorkflowObjectType.WORKFLOWACTION
        };

        foreach (string type in objectTypes)
        {
            try
            {
                var infos = InfoObjectCollection.New(type);
                foreach (var info in infos)
                {
                    using (CMSActionContext ctx = new CMSActionContext() { LogEvents = false })
                    {
                        MacroResolver.RefreshSecurityParameters(info, "administrator", true);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Subscribe macros - Upgrade", "Upgrade", ex);
            }
        }

        #endregion

        // Set the path to the upgrade package
        mUpgradePackagePath = HttpContext.Current.Server.MapPath("~/CMSSiteUtils/Import/upgrade_70RC_70.zip");

        mWebsitePath = HttpContext.Current.Server.MapPath("~/");

        TableManager dtm = new TableManager(null);

        // Update all views
        dtm.RefreshDocumentViews();

        // Set data version
        ObjectHelper.SetSettingsKeyValue("CMSDataVersion", "7.0");

        // Clear hashtables
        CMSObjectHelper.ClearHashtables();

        // Clear the cache
        CacheHelper.ClearCache(null, true);

        // Drop the routes
        CMSMvcHandler.DropAllRoutes();

        // Init the Mimetype helper (required for the Import)
        MimeTypeHelper.LoadMimeTypes();

        CMSThread thread = new CMSThread(Upgrade70RCImport);
        thread.Start();
    }


    private static void Upgrade70RCImport()
    {
        // Import
        try
        {
            RequestStockHelper.Remove("CurrentDomain", true);

            SiteImportSettings importSettings = new SiteImportSettings(CMSContext.CurrentUser)
            {
                DefaultProcessObjectType = ProcessObjectEnum.All,
                SourceFilePath = mUpgradePackagePath,
                WebsitePath = mWebsitePath
            };

            ImportProvider.ImportObjectsData(importSettings);

            using (CMSActionContext ctx = new CMSActionContext() { LogEvents = false })
            {
                // Regenerate time zones
                TimeZoneInfoProvider.GenerateTimeZoneRules();
            }

            HandleSeparability();

            EventLogProvider.LogInformation("Upgrade to 7.0", "Upgrade - Finish");
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Upgrade to 7.0", "Upgrade", ex);
        }
    }

    #endregion


    #region "Update 6.0"

    public static void Update60()
    {
        EventLogProvider.LogInformation("Upgrade to 7.0", "Upgrade - Start");

        DataClassInfo dci = null;

        #region "CMS - User"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("cms.user");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = null;

                    ffi = fi.GetFormField("UserID");
                    if (ffi != null)
                    {
                        ffi.Visibility = "none";

                        fi.UpdateFormField("UserID", ffi);
                    }

                    ffi = fi.GetFormField("UserName");
                    if (ffi != null)
                    {
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("UserName", ffi);
                    }

                    ffi = fi.GetFormField("FirstName");
                    if (ffi != null)
                    {
                        ffi.Visibility = "none";

                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("FirstName", ffi);
                    }

                    ffi = fi.GetFormField("MiddleName");
                    if (ffi != null)
                    {
                        ffi.Visibility = "none";

                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("MiddleName", ffi);
                    }

                    ffi = fi.GetFormField("FullName");
                    if (ffi != null)
                    {
                        ffi.TranslateField = true;

                        fi.UpdateFormField("FullName", ffi);
                    }

                    ffi = fi.GetFormField("UserSiteManagerDisabled");
                    if (ffi != null)
                    {
                        ffi.Caption = "UserSiteManagerDisabled";
                        ffi.Visible = false;

                        fi.UpdateFormField("UserSiteManagerDisabled", ffi);
                    }

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("CMS_User");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Refresh views
                    tm.RefreshCustomViews("CMS_User");
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CMS.User - Upgrade", "Upgrade", ex);
        }

        #endregion

        #region "CMS - UserSettings"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("cms.usersettings");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = null;

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserAvatarType";
                    ffi.Caption = "User avatar type";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.Size = 200;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "avatartypeselector";
                    ffi.Settings["EditText"] = "False";
                    ffi.Settings["Options"] = "cms.avatar;CMS.Avatar  gravatar;Gravatar  userchoice;User can choose";

                    fi.AddFormField(ffi);

                    ffi = fi.GetFormField("UserSkype");
                    if (ffi != null)
                    {
                        ffi.Caption = "UserSkype";

                        fi.UpdateFormField("UserSkype", ffi);
                    }

                    ffi = fi.GetFormField("UserIM");
                    if (ffi != null)
                    {
                        ffi.Caption = "UserIM";

                        fi.UpdateFormField("UserIM", ffi);
                    }

                    ffi = fi.GetFormField("UserPhone");
                    if (ffi != null)
                    {
                        ffi.Caption = "UserPhone";

                        fi.UpdateFormField("UserPhone", ffi);
                    }

                    ffi = fi.GetFormField("UserPosition");
                    if (ffi != null)
                    {
                        ffi.Caption = "UserPosition";

                        fi.UpdateFormField("UserPosition", ffi);
                    }

                    ffi = fi.GetFormField("UserPasswordRequestHash");
                    if (ffi != null)
                    {
                        ffi.Caption = "UserPasswordRequestHash";

                        fi.UpdateFormField("UserPasswordRequestHash", ffi);
                    }

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserWebPartToolbarEnabled";
                    ffi.Caption = "UserSettingsID";
                    ffi.DefaultValue = "true";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "labelcontrol";

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserWebPartToolbarPosition";
                    ffi.Caption = "UserSettingsID";
                    ffi.DefaultValue = "right";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.Size = 10;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.TranslateField = true;
                    ffi.Settings["controlname"] = "labelcontrol";

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserInvalidLogOnAttempts";
                    ffi.Caption = "UserSettingsID";
                    ffi.DefaultValue = "0";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "labelcontrol";

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserInvalidLogOnAttemptsHash";
                    ffi.Caption = "UserSettingsID";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.Size = 100;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "labelcontrol";

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserPasswordLastChanged";
                    ffi.Caption = "UserSettingsID";
                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "labelcontrol";

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserAccountLockReason";
                    ffi.Caption = "UserSettingsID";
                    ffi.DefaultValue = "0";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "labelcontrol";

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("CMS_UserSettings");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Refresh views
                    tm.RefreshCustomViews("CMS_UserSettings");
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("CMS.UserSettings - Upgrade", "Upgrade", ex);
        }

        #endregion

        #region "Community - Group"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("Community.Group");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = null;

                    ffi = fi.GetFormField("GroupLogActivity");
                    if (ffi != null)
                    {
                        ffi.Visibility = "none";

                        fi.UpdateFormField("GroupLogActivity", ffi);
                    }

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("Community_Group");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Refresh views
                    tm.RefreshCustomViews("Community_Group");
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Community.Group - Upgrade", "Upgrade", ex);
        }

        #endregion

        #region "Ecommerce - Customer"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.customer");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = null;

                    ffi = new FormFieldInfo();
                    ffi.Name = "CustomerCreated";
                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "calendarcontrol";

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_Customer");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Refresh views
                    tm.RefreshCustomViews("COM_Customer");
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Ecommerce.Customer - Upgrade", "Upgrade", ex);
        }

        #endregion

        #region "Ecommerce - OrderItem"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.orderitem");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = null;

                    ffi = fi.GetFormField("OrderItemGuid");
                    if (ffi != null)
                    {
                        ffi.Caption = "OrderItemGuid";

                        fi.UpdateFormField("OrderItemGuid", ffi);
                    }

                    ffi = fi.GetFormField("OrderItemParentGuid");
                    if (ffi != null)
                    {
                        ffi.Caption = "OrderItemParentGuid";

                        fi.UpdateFormField("OrderItemParentGuid", ffi);
                    }

                    ffi = fi.GetFormField("OrderItemBundleGUID");
                    if (ffi != null)
                    {
                        ffi.Caption = "OrderItemBundleGUID";

                        fi.UpdateFormField("OrderItemBundleGUID", ffi);
                    }

                    ffi = fi.GetFormField("OrderItemSKU");
                    if (ffi != null)
                    {
                        ffi.Caption = "OrderItemSKU";

                        fi.UpdateFormField("OrderItemSKU", ffi);
                    }

                    ffi = fi.GetFormField("OrderItemTotalPriceInMainCurrency");
                    if (ffi != null)
                    {
                        ffi.Caption = "OrderItemTotalPriceInMainCurrency";

                        fi.UpdateFormField("OrderItemTotalPriceInMainCurrency", ffi);
                    }

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_OrderItem");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Refresh views
                    tm.RefreshCustomViews("COM_OrderItem");
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Ecommerce.OrderItem - Upgrade", "Upgrade", ex);
        }

        #endregion

        #region "Ecommerce - SKU"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.sku");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormInfo fiNew = new FormInfo(null);

                    FormFieldInfo ffi = null;
                    FormCategoryInfo fci = null;

                    fci = new FormCategoryInfo();
                    fci.CategoryName = "com.sku.generalcategory";
                    fci.CategoryCaption = "{$com.sku.generalcategory$}";
                    fci.Visible = true;

                    fiNew.AddFormCategory(fci);

                    ffi = fi.GetFormField("SKUID");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        fi.RemoveFormField("SKUID");

                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUGUID");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        fi.RemoveFormField("SKUGUID");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUOptionCategoryID");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        fi.RemoveFormField("SKUOptionCategoryID");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUOrder");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        fi.RemoveFormField("SKUOrder");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUSiteID");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        fi.RemoveFormField("SKUSiteID");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUName");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.name$}";
                        ffi.Visible = true;
                        ffi.TranslateField = true;
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "True";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.RemoveFormField("SKUName");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUNumber");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.number$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "True";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.RemoveFormField("SKUNumber");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUPrice");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.price$}";
                        ffi.CaptionStyle = "margin-bottom: 20px; display: block;";
                        ffi.InputControlStyle = "margin-bottom: 20px;";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["ShowErrorsOnNewLine"] = "False";
                        ffi.Settings["RangeErrorMessage"] = "com.productedit.priceinvalid";
                        ffi.Settings["FormatValueAsInteger"] = "False";
                        ffi.Settings["AllowZero"] = "True";
                        ffi.Settings["EmptyErrorMessage"] = "com.productedit.priceinvalid";
                        ffi.Settings["controlname"] = "priceselector";
                        ffi.Settings["AllowNegative"] = "False";
                        ffi.Settings["AllowEmpty"] = "False";
                        ffi.Settings["ShowCurrencyCode"] = "True";

                        fi.RemoveFormField("SKUPrice");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKURetailPrice";
                    ffi.Caption = "{$com.sku.listprice$}";
                    ffi.Visible = true;
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["ShowErrorsOnNewLine"] = "False";
                    ffi.Settings["RangeErrorMessage"] = "com.productedit.priceinvalid";
                    ffi.Settings["FormatValueAsInteger"] = "False";
                    ffi.Settings["AllowZero"] = "True";
                    ffi.Settings["EmptyErrorMessage"] = "com.productedit.priceinvalid";
                    ffi.Settings["controlname"] = "priceselector";
                    ffi.Settings["AllowNegative"] = "False";
                    ffi.Settings["AllowEmpty"] = "True";
                    ffi.Settings["ShowCurrencyCode"] = "True";

                    fiNew.AddFormField(ffi);

                    ffi = fi.GetFormField("SKUDepartmentID");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.departmentid$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["AddWithoutDepartmentRecord"] = "False";
                        ffi.Settings["DropDownListMode"] = "True";
                        ffi.Settings["controlname"] = "departmentselector";
                        ffi.Settings["AddAllMyRecord"] = "False";
                        ffi.Settings["AddNoneRecord"] = "True";
                        ffi.Settings["AddAllItemsRecord"] = "False";
                        ffi.Settings["UseDepartmentNameForSelection"] = "False";
                        ffi.Settings["ShowAllSites"] = "False";

                        fi.RemoveFormField("SKUDepartmentID");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUManufacturerID");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.manufacturerid$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["AddNoneRecord"] = "True";
                        ffi.Settings["controlname"] = "manufacturerselector";
                        ffi.Settings["AddAllItemsRecord"] = "False";
                        ffi.Settings["DisplayOnlyEnabled"] = "True";

                        fi.RemoveFormField("SKUManufacturerID");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUSupplierID");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.supplierid$}";
                        ffi.CaptionStyle = "margin-bottom: 20px; display: block;";
                        ffi.InputControlStyle = "margin-bottom: 20px;";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["AddNoneRecord"] = "True";
                        ffi.Settings["controlname"] = "supplierselector";
                        ffi.Settings["AddAllItemsRecord"] = "False";
                        ffi.Settings["DisplayOnlyEnabled"] = "True";

                        fi.RemoveFormField("SKUSupplierID");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUImagePath");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.imagepath$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["controlname"] = "productimageselector";

                        fi.RemoveFormField("SKUImagePath");
                        fiNew.AddFormField(ffi);
                    }


                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUShortDescription";
                    ffi.Caption = "{$com.sku.shortdescription$}";
                    ffi.Visible = true;
                    ffi.DataType = FormFieldDataTypeEnum.LongText;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["Width"] = "100";
                    ffi.Settings["Height"] = "50";
                    ffi.Settings["HeightUnitType"] = "PX";
                    ffi.Settings["controlname"] = "htmlareacontrol";
                    ffi.Settings["Dialogs_Content_Hide"] = "False";
                    ffi.Settings["WidthUnitType"] = "PERCENTAGE";
                    ffi.Settings["Autoresize_Hashtable"] = "True";
                    ffi.Settings["MediaDialogConfiguration"] = "True";

                    fiNew.AddFormField(ffi);

                    ffi = fi.GetFormField("SKUDescription");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.description$}";
                        ffi.CaptionStyle = "margin-bottom: 20px; display: block;";
                        ffi.InputControlStyle = "margin-bottom: 20px;";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["HeightUnitType"] = "PX";
                        ffi.Settings["Dialogs_Web_Hide"] = "False";
                        ffi.Settings["Dialogs_Attachments_Hide"] = "False";
                        ffi.Settings["Width"] = "100";
                        ffi.Settings["Dialogs_Anchor_Hide"] = "False";
                        ffi.Settings["Dialogs_Libraries_Hide"] = "False";
                        ffi.Settings["controlname"] = "htmlareacontrol";
                        ffi.Settings["Dialogs_Content_Hide"] = "False";
                        ffi.Settings["Dialogs_Email_Hide"] = "False";
                        ffi.Settings["WidthUnitType"] = "PERCENTAGE";
                        ffi.Settings["Autoresize_Hashtable"] = "True";
                        ffi.Settings["MediaDialogConfiguration"] = "True";

                        fi.RemoveFormField("SKUDescription");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUProductType");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.producttype$}";
                        ffi.Visible = true;
                        ffi.Settings["AllowStandardProduct"] = "True";
                        ffi.Settings["AllowDonation"] = "True";
                        ffi.Settings["AllowMembership"] = "True";
                        ffi.Settings["AllowNone"] = "False";
                        ffi.Settings["controlname"] = "producttypeselector";
                        ffi.Settings["AllowBundle"] = "True";
                        ffi.Settings["AllowText"] = "True";
                        ffi.Settings["AutoPostBack"] = "True";
                        ffi.Settings["AllowEproduct"] = "True";
                        ffi.Settings["AllowAll"] = "False";

                        fi.RemoveFormField("SKUProductType");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUCustomData");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        fi.RemoveFormField("SKUCustomData");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUCreated");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        fi.RemoveFormField("SKUCreated");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKULastModified");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        fi.RemoveFormField("SKULastModified");
                        fiNew.AddFormField(ffi);
                    }

                    fci = new FormCategoryInfo();
                    fci.CategoryName = "com.sku.representingcategory";
                    fci.CategoryCaption = "{$com.sku.representingcategory$}";
                    fci.Visible = true;

                    fiNew.AddFormCategory(fci);

                    ffi = fi.GetFormField("SKUMembershipGUID");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.membershipguid$}";
                        ffi.Visible = true;
                        ffi.Settings["AddNoneRecord"] = "True";
                        ffi.Settings["controlname"] = "membershipselector";
                        ffi.Settings["UseCodeNameForSelection"] = "False";
                        ffi.Settings["UseGUIDForSelection"] = "True";

                        fi.RemoveFormField("SKUMembershipGUID");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUValidity");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.validity$}";
                        ffi.Visible = true;
                        ffi.ControlCssClass = "BorderedWrapper";
                        ffi.Settings["AutoPostBack"] = "True";
                        ffi.Settings["ValidUntilFieldName"] = "SKUValidUntil";
                        ffi.Settings["controlname"] = "validityselector";
                        ffi.Settings["ValidForFieldName"] = "SKUValidFor";

                        fi.RemoveFormField("SKUValidity");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUValidFor");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        fi.RemoveFormField("SKUValidFor");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUValidUntil");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        fi.RemoveFormField("SKUValidUntil");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUMaxDownloads");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        fi.RemoveFormField("SKUMaxDownloads");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUEproductFilesCount";
                    ffi.Caption = "{$com.sku.eproductfilescount$}";
                    ffi.Visible = true;
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "eproductfilesselector";

                    fiNew.AddFormField(ffi);

                    ffi = fi.GetFormField("SKUMinPrice");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.minprice$}";
                        ffi.Visible = true;
                        ffi.Settings.Clear();
                        ffi.Settings["ShowErrorsOnNewLine"] = "False";
                        ffi.Settings["RangeErrorMessage"] = "com.productedit.priceinvalid";
                        ffi.Settings["FormatValueAsInteger"] = "False";
                        ffi.Settings["AllowZero"] = "True";
                        ffi.Settings["EmptyErrorMessage"] = "com.productedit.priceinvalid";
                        ffi.Settings["controlname"] = "priceselector";
                        ffi.Settings["AllowNegative"] = "False";
                        ffi.Settings["AllowEmpty"] = "True";
                        ffi.Settings["ShowCurrencyCode"] = "True";

                        fi.RemoveFormField("SKUMinPrice");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUMaxPrice");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.maxprice$}";
                        ffi.Visible = true;
                        ffi.Settings["ShowErrorsOnNewLine"] = "False";
                        ffi.Settings["RangeErrorMessage"] = "com.productedit.priceinvalid";
                        ffi.Settings["FormatValueAsInteger"] = "False";
                        ffi.Settings["AllowZero"] = "True";
                        ffi.Settings["EmptyErrorMessage"] = "com.productedit.priceinvalid";
                        ffi.Settings["controlname"] = "priceselector";
                        ffi.Settings["AllowNegative"] = "False";
                        ffi.Settings["AllowEmpty"] = "True";
                        ffi.Settings["ShowCurrencyCode"] = "True";

                        fi.RemoveFormField("SKUMaxPrice");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUPrivateDonation");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.privatedonation$}";
                        ffi.Visible = true;

                        fi.RemoveFormField("SKUPrivateDonation");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUBundleInventoryType");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.bundleinventorytype$}";
                        ffi.Visible = true;
                        ffi.Settings["controlname"] = "bundleinventorytypeselector";

                        fi.RemoveFormField("SKUBundleInventoryType");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUBundleItemsCount";
                    ffi.Caption = "{$com.sku.bundleitemscount$}";
                    ffi.Visible = true;
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "bundleitemsselector";

                    fiNew.AddFormField(ffi);

                    fci = new FormCategoryInfo();
                    fci.CategoryName = "com.sku.statuscategory";
                    fci.CategoryCaption = "{$com.sku.statuscategory$}";
                    fci.Visible = true;

                    fiNew.AddFormCategory(fci);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUInStoreFrom";
                    ffi.Caption = "{$com.sku.instorefrom$}";
                    ffi.DefaultValue = "##TODAY##";
                    ffi.Visible = true;
                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["DisplayNow"] = "True";
                    ffi.Settings["TimeZoneType"] = "inherit";
                    ffi.Settings["controlname"] = "calendarcontrol";
                    ffi.Settings["EditTime"] = "False";

                    fiNew.AddFormField(ffi);

                    ffi = fi.GetFormField("SKUPublicStatusID");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.publicstatusid$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["AddNoneRecord"] = "True";
                        ffi.Settings["UseStatusNameForSelection"] = "False";
                        ffi.Settings["controlname"] = "publicstatusselector";
                        ffi.Settings["AddAllItemsRecord"] = "False";
                        ffi.Settings["DisplayOnlyEnabled"] = "True";
                        ffi.Settings["AppendGlobalItems"] = "False";

                        fi.RemoveFormField("SKUPublicStatusID");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUInternalStatusID");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.internalstatusid$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["AddNoneRecord"] = "True";
                        ffi.Settings["UseStatusNameForSelection"] = "False";
                        ffi.Settings["controlname"] = "internalstatusselector";
                        ffi.Settings["AddAllItemsRecord"] = "False";
                        ffi.Settings["DisplayOnlyEnabled"] = "True";
                        ffi.Settings["AppendGlobalItems"] = "False";

                        fi.RemoveFormField("SKUInternalStatusID");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUEnabled");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.enabled$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.DefaultValue = "true";

                        fi.RemoveFormField("SKUEnabled");
                        fiNew.AddFormField(ffi);
                    }

                    fci = new FormCategoryInfo();
                    fci.CategoryName = "com.sku.shippingcategory";
                    fci.CategoryCaption = "{$com.sku.shippingcategory$}";
                    fci.Visible = true;

                    fiNew.AddFormCategory(fci);

                    ffi = fi.GetFormField("SKUNeedsShipping");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.needsshipping$}";
                        ffi.Visible = true;

                        fi.RemoveFormField("SKUNeedsShipping");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUWeight");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.weight$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["FilterType"] = "0|3";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["ValidChars"] = ".";
                        ffi.Settings["controlname"] = "textboxcontrol";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["Trim"] = "True";

                        fi.RemoveFormField("SKUWeight");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUHeight");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.height$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["FilterType"] = "0|3";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["ValidChars"] = ".";
                        ffi.Settings["controlname"] = "textboxcontrol";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["Trim"] = "True";

                        fi.RemoveFormField("SKUHeight");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUWidth");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.width$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["FilterType"] = "0|3";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["ValidChars"] = ".";
                        ffi.Settings["controlname"] = "textboxcontrol";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["Trim"] = "True";

                        fi.RemoveFormField("SKUWidth");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUDepth");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.depth$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["FilterType"] = "0|3";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["ValidChars"] = ".";
                        ffi.Settings["controlname"] = "textboxcontrol";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["Trim"] = "True";

                        fi.RemoveFormField("SKUDepth");
                        fiNew.AddFormField(ffi);
                    }

                    fci = new FormCategoryInfo();
                    fci.CategoryName = "com.sku.inventorycategory";
                    fci.CategoryCaption = "{$com.sku.inventorycategory$}";
                    fci.Visible = true;

                    fiNew.AddFormCategory(fci);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUTrackInventory";
                    ffi.DefaultValue = "false";
                    ffi.Visible = false;
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "checkboxcontrol";

                    fiNew.AddFormField(ffi);

                    ffi = fi.GetFormField("SKUSellOnlyAvailable");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.sellonlyavailable$}";
                        ffi.Visible = true;
                        ffi.Description = "Description";
                        ffi.Visibility = "none";
                        ffi.Settings["controlname"] = "checkboxcontrol";

                        fi.RemoveFormField("SKUSellOnlyAvailable");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUAvailableItems");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.availableitems$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["FilterType"] = "0|3";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["ValidChars"] = "-";
                        ffi.Settings["controlname"] = "textboxcontrol";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["Trim"] = "True";

                        fi.RemoveFormField("SKUAvailableItems");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUReorderAt";
                    ffi.Caption = "{$com.sku.reorderat$}";
                    ffi.Visible = true;
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["FilterType"] = "0|3";
                    ffi.Settings["AutoCompleteEnableCaching"] = "False";
                    ffi.Settings["FilterMode"] = "False";
                    ffi.Settings["ValidChars"] = "-";
                    ffi.Settings["controlname"] = "textboxcontrol";
                    ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                    ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                    ffi.Settings["Trim"] = "False";

                    fiNew.AddFormField(ffi);

                    ffi = fi.GetFormField("SKUAvailableInDays");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.availableindays$}";
                        ffi.CaptionStyle = "margin-bottom: 20px; display: block;";
                        ffi.InputControlStyle = "margin-bottom: 20px;";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["FilterType"] = "0|3";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["ValidChars"] = "-";
                        ffi.Settings["controlname"] = "textboxcontrol";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["Trim"] = "True";

                        fi.RemoveFormField("SKUAvailableInDays");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUMinItemsInOrder";
                    ffi.Caption = "{$com.sku.minitemsinorder$}";
                    ffi.Visible = true;
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                    ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                    ffi.Settings["controlname"] = "textboxcontrol";
                    ffi.Settings["FilterMode"] = "False";
                    ffi.Settings["FilterType"] = "0";
                    ffi.Settings["AutoCompleteEnableCaching"] = "False";
                    ffi.Settings["Trim"] = "True";

                    fiNew.AddFormField(ffi);

                    ffi = fi.GetFormField("SKUMaxItemsInOrder");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.maxitemsinorder$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["controlname"] = "textboxcontrol";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["FilterType"] = "0";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";
                        ffi.Settings["Trim"] = "True";

                        fi.RemoveFormField("SKUMaxItemsInOrder");
                        fiNew.AddFormField(ffi);
                    }

                    fci = new FormCategoryInfo();
                    fci.CategoryName = "com.sku.analyticscategory";
                    fci.CategoryCaption = "{$com.sku.analyticscategory$}";
                    fci.VisibleMacro = "{%ProductSiteID > 0%}";
                    fci.Visible = true;

                    fiNew.AddFormCategory(fci);

                    ffi = fi.GetFormField("SKUConversionName");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.conversionname$}";
                        ffi.ControlCssClass = "NoWrap";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["controlname"] = "conversionselector";

                        fi.RemoveFormField("SKUConversionName");
                        fiNew.AddFormField(ffi);
                    }

                    ffi = fi.GetFormField("SKUConversionValue");
                    if (ffi != null)
                    {
                        ffi = ffi.Clone() as FormFieldInfo;
                        ffi.Caption = "{$com.sku.conversionvalue$}";
                        ffi.Visible = true;
                        ffi.Visibility = "none";
                        ffi.Settings["controlname"] = "textbox_double_validator";

                        fi.RemoveFormField("SKUConversionValue");
                        fiNew.AddFormField(ffi);
                    }

                    fci = new FormCategoryInfo();
                    fci.CategoryName = "com.sku.variantcategory";
                    fci.CategoryCaption = "{$com.sku.variantcategory$}";
                    fci.Visible = true;

                    fiNew.AddFormCategory(fci);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUParentSKUID";
                    ffi.Visible = false;
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "dropdownlistcontrol";

                    fiNew.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUAllowAllVariants";
                    ffi.DefaultValue = "false";
                    ffi.Visible = false;
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "dropdownlistcontrol";

                    fiNew.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUInheritsDiscounts";
                    ffi.DefaultValue = "false";
                    ffi.Visible = false;
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "dropdownlistcontrol";

                    fiNew.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUInheritsTaxClasses";
                    ffi.DefaultValue = "false";
                    ffi.Visible = false;
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "dropdownlistcontrol";

                    fiNew.AddFormField(ffi);

                    if (fi.ItemsList.Count > 0)
                    {
                        var altforms = AlternativeFormInfoProvider.GetAlternativeForms("FormClassID=" + dci.ClassID, null);
                        foreach (AlternativeFormInfo afi in altforms.Where<AlternativeFormInfo>(x => x.FormHideNewParentFields))
                        {
                            // Hide all form field info from alternative forms
                            foreach (IFormItem formItem in fi.ItemsList)
                            {
                                if (formItem is FormFieldInfo)
                                {
                                    afi.HideField(formItem as FormFieldInfo);
                                }
                            }

                            AlternativeFormInfoProvider.SetAlternativeFormInfo(afi);
                        }

                        // Add all other custom fields at the bottom of form definition
                        foreach (IFormItem formItem in fi.ItemsList)
                        {
                            if (formItem is FormFieldInfo)
                            {
                                FormFieldInfo ffiCurrent = formItem as FormFieldInfo;

                                fiNew.AddFormField(ffiCurrent);
                            }
                        }
                    }

                    dci.ClassFormDefinition = fiNew.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_SKU");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Refresh views
                    tm.RefreshCustomViews("COM_SKU");
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Ecommerce.SKU - Upgrade", "Upgrade", ex);
        }

        #endregion

        #region "Newsletter - Subscriber"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("newsletter.subscriber");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = null;

                    ffi = new FormFieldInfo();
                    ffi.Name = "SubscriberCustomData";
                    ffi.DataType = FormFieldDataTypeEnum.LongText;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SubscriberFullName";
                    ffi.Caption = "SubscriberFullName";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.Size = 440;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["Trim"] = "False";
                    ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                    ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                    ffi.Settings["FilterMode"] = "False";
                    ffi.Settings["AutoCompleteEnableCaching"] = "False";
                    ffi.Settings["controlname"] = "textboxcontrol";

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("Newsletter_Subscriber");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Refresh views
                    tm.RefreshCustomViews("Newsletter_Subscriber");
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Newsletter.Subscriber - Upgrade", "Upgrade", ex);
        }

        #endregion

        #region "Contact management - Contact"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("OM.Contact");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = null;

                    ffi = fi.GetFormField("ContactFirstName");
                    if (ffi != null)
                    {
                        ffi.Caption = "First name";
                        ffi.Visibility = "none";
                        ffi.DisplayInSimpleMode = true;
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactFirstName", ffi);
                    }

                    ffi = fi.GetFormField("ContactMiddleName");
                    if (ffi != null)
                    {
                        ffi.Caption = "Middle name";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactMiddleName", ffi);
                    }

                    ffi = fi.GetFormField("ContactLastName");
                    if (ffi != null)
                    {
                        ffi.Caption = "Last name";
                        ffi.Visibility = "none";
                        ffi.DisplayInSimpleMode = true;
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactLastName", ffi);
                    }

                    ffi = fi.GetFormField("ContactSalutation");
                    if (ffi != null)
                    {
                        ffi.Caption = "Salutation";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactSalutation", ffi);
                    }

                    ffi = fi.GetFormField("ContactTitleBefore");
                    if (ffi != null)
                    {
                        ffi.Caption = "Title before";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactTitleBefore", ffi);
                    }

                    ffi = fi.GetFormField("ContactTitleAfter");
                    if (ffi != null)
                    {
                        ffi.Caption = "Title after";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactTitleAfter", ffi);
                    }

                    ffi = fi.GetFormField("ContactJobTitle");
                    if (ffi != null)
                    {
                        ffi.Caption = "Job title";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactJobTitle", ffi);
                    }

                    ffi = new FormFieldInfo();
                    ffi.Name = "ContactCompanyName";
                    ffi.Caption = "Company name";
                    ffi.Visible = true;
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.Size = 100;
                    ffi.PublicField = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "textboxcontrol";
                    ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                    ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                    ffi.Settings["FilterMode"] = "False";
                    ffi.Settings["Trim"] = "False";
                    ffi.Settings["AutoCompleteEnableCaching"] = "False";

                    fi.AddFormField(ffi);

                    ffi = fi.GetFormField("ContactAddress1");
                    if (ffi != null)
                    {
                        ffi.Caption = "Address 1";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactAddress1", ffi);
                    }

                    ffi = fi.GetFormField("ContactAddress2");
                    if (ffi != null)
                    {
                        ffi.Caption = "Address 2";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactAddress2", ffi);
                    }

                    ffi = fi.GetFormField("ContactCity");
                    if (ffi != null)
                    {
                        ffi.Caption = "City";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactCity", ffi);
                    }

                    ffi = fi.GetFormField("ContactZIP");
                    if (ffi != null)
                    {
                        ffi.Caption = "ZIP code";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactZIP", ffi);
                    }

                    ffi = fi.GetFormField("ContactStateID");
                    if (ffi != null)
                    {
                        ffi.Caption = "State";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactStateID", ffi);
                    }

                    ffi = fi.GetFormField("ContactCountryID");
                    if (ffi != null)
                    {
                        ffi.Caption = "Country";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactCountryID", ffi);
                    }

                    ffi = fi.GetFormField("ContactMobilePhone");
                    if (ffi != null)
                    {
                        ffi.Caption = "Mobile phone";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactMobilePhone", ffi);
                    }

                    ffi = fi.GetFormField("ContactHomePhone");
                    if (ffi != null)
                    {
                        ffi.Caption = "Home phone";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactHomePhone", ffi);
                    }

                    ffi = fi.GetFormField("ContactBusinessPhone");
                    if (ffi != null)
                    {
                        ffi.Caption = "Business phone";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactBusinessPhone", ffi);
                    }

                    ffi = fi.GetFormField("ContactEmail");
                    if (ffi != null)
                    {
                        ffi.Caption = "E-mail address";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactEmail", ffi);
                    }

                    ffi = fi.GetFormField("ContactWebSite");
                    if (ffi != null)
                    {
                        ffi.Caption = "Web URL";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactWebSite", ffi);
                    }

                    ffi = fi.GetFormField("ContactBirthday");
                    if (ffi != null)
                    {
                        ffi.Caption = "Birthday";
                        ffi.Visibility = "none";
                        ffi.Settings["DisplayNow"] = "True";
                        ffi.Settings["TimeZoneType"] = "inherit";
                        ffi.Settings["EditTime"] = "True";

                        fi.UpdateFormField("ContactBirthday", ffi);
                    }

                    ffi = fi.GetFormField("ContactGender");
                    if (ffi != null)
                    {
                        ffi.Caption = "Gender";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactGender", ffi);
                    }

                    ffi = fi.GetFormField("ContactStatusID");
                    if (ffi != null)
                    {
                        ffi.Caption = "Status";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactStatusID", ffi);
                    }

                    ffi = fi.GetFormField("ContactCampaign");
                    if (ffi != null)
                    {
                        ffi.Caption = "Campaign";
                        ffi.Inheritable = true;
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["controlname"] = "textboxcontrol";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactCampaign", ffi);
                    }

                    ffi = fi.GetFormField("ContactNotes");
                    if (ffi != null)
                    {
                        ffi.Caption = "Notes";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Wrap"] = "True";
                        ffi.Settings["IsTextArea"] = "True";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactNotes", ffi);
                    }

                    ffi = fi.GetFormField("ContactOwnerUserID");
                    if (ffi != null)
                    {
                        ffi.Caption = "Owner";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["controlname"] = "textboxcontrol";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactOwnerUserID", ffi);
                    }


                    ffi = fi.GetFormField("ContactMonitored");
                    if (ffi != null)
                    {
                        ffi.Caption = "Track activities";
                        ffi.DefaultValue = "false";
                        ffi.Visibility = "none";

                        fi.UpdateFormField("ContactMonitored", ffi);
                    }

                    ffi = fi.GetFormField("ContactMergedWithContactID");
                    if (ffi != null)
                    {
                        ffi.Caption = "Merged into";
                        ffi.Visible = false;
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactMergedWithContactID", ffi);
                    }

                    ffi = fi.GetFormField("ContactIsAnonymous");
                    if (ffi != null)
                    {
                        ffi.Caption = "Is anonymous";
                        ffi.Visible = false;

                        fi.UpdateFormField("ContactIsAnonymous", ffi);
                    }

                    ffi = fi.GetFormField("ContactSiteID");
                    if (ffi != null)
                    {
                        ffi.Caption = "Site ID";
                        ffi.Visible = false;
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactSiteID", ffi);
                    }

                    ffi = fi.GetFormField("ContactGUID");
                    if (ffi != null)
                    {
                        ffi.Caption = "Contact GUID";
                        ffi.Visible = false;

                        fi.UpdateFormField("ContactGUID", ffi);
                    }

                    ffi = fi.GetFormField("ContactLastModified");
                    if (ffi != null)
                    {
                        ffi.Caption = "Last modified";
                        ffi.Visibility = "none";
                        ffi.Settings["DisplayNow"] = "True";
                        ffi.Settings["EditTime"] = "True";

                        fi.UpdateFormField("ContactLastModified", ffi);
                    }

                    ffi = fi.GetFormField("ContactCreated");
                    if (ffi != null)
                    {
                        ffi.Caption = "Created";
                        ffi.Visibility = "none";
                        ffi.Settings["DisplayNow"] = "True";
                        ffi.Settings["EditTime"] = "True";

                        fi.UpdateFormField("ContactCreated", ffi);
                    }

                    ffi = fi.GetFormField("ContactMergedWhen");
                    if (ffi != null)
                    {
                        ffi.Caption = "Merged when";
                        ffi.Visibility = "none";
                        ffi.Settings["DisplayNow"] = "True";
                        ffi.Settings["EditTime"] = "True";

                        fi.UpdateFormField("ContactMergedWhen", ffi);
                    }

                    ffi = fi.GetFormField("ContactGlobalContactID");
                    if (ffi != null)
                    {
                        ffi.Caption = "Global Contact ID";
                        ffi.Visible = false;
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactGlobalContactID", ffi);
                    }

                    ffi = fi.GetFormField("ContactBounces");
                    if (ffi != null)
                    {
                        ffi.Caption = "Bounces";
                        ffi.Visibility = "none";
                        ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                        ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                        ffi.Settings["FilterMode"] = "False";
                        ffi.Settings["Trim"] = "False";
                        ffi.Settings["AutoCompleteEnableCaching"] = "False";

                        fi.UpdateFormField("ContactBounces", ffi);
                    }

                    ffi = fi.GetFormField("ContactLastLogon");
                    if (ffi != null)
                    {
                        ffi.Caption = "Last logon";
                        ffi.Visibility = "none";
                        ffi.Settings["DisplayNow"] = "True";
                        ffi.Settings["EditTime"] = "True";

                        fi.UpdateFormField("ContactLastLogon", ffi);
                    }

                    ffi = new FormFieldInfo();
                    ffi.Name = "ContactSalesForceLeadID";
                    ffi.Caption = "SalesForce - Lead ID";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.Size = 18;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["AutoCompleteShowOnlyCurrentWordInCompletionListItem"] = "False";
                    ffi.Settings["AutoCompleteFirstRowSelected"] = "False";
                    ffi.Settings["controlname"] = "textboxcontrol";
                    ffi.Settings["FilterMode"] = "False";
                    ffi.Settings["Trim"] = "False";
                    ffi.Settings["AutoCompleteEnableCaching"] = "False";

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "ContactSalesForceLeadReplicationDisabled";
                    ffi.Caption = "SalesForce - Replication disabled";
                    ffi.DefaultValue = "false";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "checkboxcontrol";

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "ContactSalesForceLeadReplicationDateTime";
                    ffi.Caption = "SalesForce - Replication date";
                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["DisplayNow"] = "True";
                    ffi.Settings["controlname"] = "calendarcontrol";
                    ffi.Settings["EditTime"] = "True";

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "ContactSalesForceLeadReplicationSuspensionDateTime";
                    ffi.Caption = "SalesForce - Replication suspension date";
                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["DisplayNow"] = "True";
                    ffi.Settings["controlname"] = "calendarcontrol";
                    ffi.Settings["EditTime"] = "True";

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "ContactSalesForceLeadReplicationRequired";
                    ffi.Caption = "SalesForce - Replication required";
                    ffi.DefaultValue = "false";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                    ffi.AllowEmpty = true;
                    ffi.System = true;
                    ffi.PublicField = false;
                    ffi.Visible = false;
                    ffi.Visibility = "none";
                    ffi.Settings["controlname"] = "checkboxcontrol";

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("OM_Contact");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Refresh views
                    tm.RefreshCustomViews("OM_Contact");
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("OM.Contact - Upgrade", "Upgrade", ex);
        }

        #endregion

        // Set the path to the upgrade package
        mUpgradePackagePath = HttpContext.Current.Server.MapPath("~/CMSSiteUtils/Import/upgrade_60_70.zip");

        mWebsitePath = HttpContext.Current.Server.MapPath("~/");

        TableManager dtm = new TableManager(null);

        // Update all views
        dtm.RefreshDocumentViews();

        // Set data version
        ObjectHelper.SetSettingsKeyValue("CMSDataVersion", "7.0");

        // Clear hashtables
        CMSObjectHelper.ClearHashtables();

        // Clear the cache
        CacheHelper.ClearCache(null, true);

        // Drop the routes
        CMSMvcHandler.DropAllRoutes();

        // Init the Mimetype helper (required for the Import)
        MimeTypeHelper.LoadMimeTypes();

        CMSThread thread = new CMSThread(Upgrade60Import);
        thread.Start();
    }


    private static void Upgrade60Import()
    {
        // Import
        try
        {
            RequestStockHelper.Remove("CurrentDomain", true);

            SiteImportSettings importSettings = new SiteImportSettings(CMSContext.CurrentUser)
            {
                DefaultProcessObjectType = ProcessObjectEnum.All,
                SourceFilePath = mUpgradePackagePath,
                WebsitePath = mWebsitePath
            };

            ImportProvider.ImportObjectsData(importSettings);

            using (CMSActionContext ctx = new CMSActionContext() { LogEvents = false })
            {
                // Regenerate time zones
                TimeZoneInfoProvider.GenerateTimeZoneRules();
            }

            HandleSeparability();

            ImportMetaFiles(Path.Combine(mWebsitePath, "App_Data\\CMSTemp\\Upgrade"));
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Upgrade to 7.0", "Upgrade", ex);
        }
        finally
        {
            try
            {
                #region "Refresh macros security parameters"

                // Get object types
                List<string> objectTypes = new List<string>() {
                    SettingsObjectType.CLASS,
                    SettingsObjectType.SETTINGSKEY,
                    SiteObjectType.FORMUSERCONTROL,
                    PortalObjectType.PAGELAYOUT,
                    PortalObjectType.DEVICEPROFILE,
                    PortalObjectType.PAGETEMPLATE,
                    PredefinedObjectType.EMAILTEMPLATE,
                    SiteObjectType.INLINECONTROL,
                    PredefinedObjectType.TRANSFORMATION,
                    PredefinedObjectType.UIELEMENT,
                    PredefinedObjectType.ALTERNATIVEFORM,
                    SchedulerObjectType.SCHEDULEDTASK,
                    SchedulerObjectType.OBJECTSCHEDULEDTASK,
                    WorkflowObjectType.WORKFLOWACTION,
                    WorkflowObjectType.AUTOMATIONACTION,
                    WorkflowObjectType.WORKFLOWSTEP,
                    WorkflowObjectType.AUTOMATIONSTEP,
                    SettingsObjectType.DOCUMENTTYPE,
                    PredefinedObjectType.WEBPART,
                    PredefinedObjectType.WEBPARTCATEGORY,
                    PredefinedObjectType.WIDGET,
                    PredefinedObjectType.WIDGETCATEGORY
                };

                foreach (string type in objectTypes)
                {
                    try
                    {
                        var infos = InfoObjectCollection.New(type);
                        foreach (var info in infos)
                        {
                            using (CMSActionContext ctx = new CMSActionContext() { LogEvents = false })
                            {
                                MacroResolver.RefreshSecurityParameters(info, "administrator", true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLogProvider.LogException("Refresh macros - Upgrade", "Upgrade", ex);
                    }
                }

                EventLogProvider.LogInformation("Upgrade to 7.0", "Upgrade - Finish");

                #endregion
            }
            catch (Exception ex)
            {

                EventLogProvider.LogException("Upgrade to 7.0", "Upgrade", ex);
            }
        }
    }

    #endregion


    #region "Update 5.5r2"

    public static void Update55R2()
    {
        EventLogProvider evp = new EventLogProvider();
        evp.LogEvent("I", DateTime.Now, "Upgrade to 6.0", "Upgrade - Start");

        DataClassInfo dci = null;


        #region "CMS.UserSettings"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("cms.usersettings");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "UserAuthenticationGUID";
                    ffi.DataType = FormFieldDataTypeEnum.GUID;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserBounces";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.TextBoxControl;
                    ffi.Visible = false;
                    ffi.Caption = "UserBounces";

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserLinkedInID";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 100;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserLogActivities";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "UserPasswordRequestHash";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 100;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("CMS_UserSettings");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("CMS_UserSettings");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("CMS.UserSettings - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Ecommerce - Customer"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.customer");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "CustomerSiteID";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.TextBoxControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    TableManager tm = new TableManager(dci.ClassConnectionString);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_Customer");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);

                    tm.RefreshCustomViews("COM_Customer");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Ecommerce.Customer - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Ecommerce - Order"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.order");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "OrderCulture";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 10;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderIsPaid";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderTotalPriceInMainCurrency";
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = fi.GetFormField("OrderStatusID");
                    if (ffi != null)
                    {
                        ffi.AllowEmpty = true;
                        fi.UpdateFormField("OrderStatusID", ffi);
                    }

                    ffi = fi.GetFormField("OrderShippingAddressID");
                    if (ffi != null)
                    {
                        ffi.AllowEmpty = true;
                        fi.UpdateFormField("OrderShippingAddressID", ffi);
                    }

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_Order");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("COM_Order");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Ecommerce.Order - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Ecommerce - OrderItem"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.orderitem");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemBundleGUID";
                    ffi.DataType = FormFieldDataTypeEnum.GUID;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemIsPrivate";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemPrice";
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemSendNotification";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemSKU";
                    ffi.DataType = FormFieldDataTypeEnum.LongText;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemText";
                    ffi.DataType = FormFieldDataTypeEnum.LongText;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemTotalPriceInMainCurrency";
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "OrderItemValidTo";
                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_OrderItem");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("COM_OrderItem");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Ecommerce.OrderItem - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Ecommerce - Shopping cart item"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.shoppingcartitem");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "CartItemBundleGUID";
                    ffi.DataType = FormFieldDataTypeEnum.GUID;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "CartItemIsPrivate";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "CartItemPrice";
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "CartItemText";
                    ffi.DataType = FormFieldDataTypeEnum.LongText;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "CartItemValidTo";
                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = fi.GetFormField("CartItemGuid");
                    if (ffi != null)
                    {
                        ffi.AllowEmpty = true;
                        fi.UpdateFormField("CartItemGuid", ffi);
                    }

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_ShoppingCartSKU");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("COM_ShoppingCartSKU");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Ecommerce.ShoppingCartItem - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Ecommerce - SKU"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("ecommerce.sku");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "SKUBundleInventoryType";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 50;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUConversionName";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 100;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUConversionValue";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 200;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUMaxDownloads";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUMaxItemsInOrder";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUMaxPrice";
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUMembershipGUID";
                    ffi.DataType = FormFieldDataTypeEnum.GUID;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUMinPrice";
                    ffi.DataType = FormFieldDataTypeEnum.Decimal;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUNeedsShipping";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUPrivateDonation";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUProductType";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 50;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUSiteID";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUValidFor";
                    ffi.DataType = FormFieldDataTypeEnum.Integer;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUValidity";
                    ffi.DataType = FormFieldDataTypeEnum.Text;
                    ffi.Size = 50;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = new FormFieldInfo();
                    ffi.Name = "SKUValidUntil";
                    ffi.DataType = FormFieldDataTypeEnum.DateTime;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    ffi = fi.GetFormField("SKUDepartmentID");
                    if (ffi != null)
                    {
                        ffi.AllowEmpty = true;
                        fi.UpdateFormField("SKUDepartmentID", ffi);
                    }

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("COM_SKU");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("COM_SKU");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Ecommerce.SKU - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Community - Group"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("Community.Group");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "GroupLogActivity";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.CheckBoxControl;
                    ffi.Visible = true;
                    ffi.DefaultValue = "true";
                    ffi.Caption = "GroupLogActivity";

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("Community_Group");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("Community_Group");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Community.Group - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "Newsletter - Subscriber"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("newsletter.subscriber");
            if (dci != null)
            {
                FormInfo fi = new FormInfo(dci.ClassFormDefinition);
                if (fi != null)
                {
                    FormFieldInfo ffi = new FormFieldInfo();
                    ffi.Name = "SubscriberBounces";
                    ffi.DataType = FormFieldDataTypeEnum.Boolean;
                    ffi.AllowEmpty = true;
                    ffi.PublicField = false;
                    ffi.System = true;
                    ffi.FieldType = FormFieldControlTypeEnum.LabelControl;
                    ffi.Visible = false;

                    fi.AddFormField(ffi);

                    dci.ClassFormDefinition = fi.GetXmlDefinition();

                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    dci.ClassXmlSchema = tm.GetXmlSchema("Newsletter_Subscriber");

                    DataClassInfoProvider.SetDataClass(dci);

                    // Generate queries
                    SqlGenerator.GenerateDefaultQueries(dci, true, false);
                    tm.RefreshCustomViews("Newsletter_Subscriber");
                }
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("Newsletter.Subscriber - Upgrade", "Upgrade", ex);
        }

        #endregion


        #region "CMS.Document"

        try
        {
            dci = DataClassInfoProvider.GetDataClass("cms.document");
            if (dci != null)
            {
                SearchSettings ss = dci.ClassSearchSettingsInfos;
                SearchSettingsInfo ssi = ss.GetSettingsInfo("42f446ee-9818-4596-8124-54a38f64aa05");
                if (ssi != null)
                {
                    ssi.Searchable = true;
                    ss.SetSettingsInfo(ssi);
                }

                DataClassInfoProvider.SetDataClass(dci);
            }
        }
        catch (Exception ex)
        {
            evp.LogEvent("CMS.Document - Upgrade", "Upgrade", ex);
        }

        #endregion


        // Set the path to the upgrade package
        mUpgradePackagePath = HttpContext.Current.Server.MapPath("~/CMSSiteUtils/Import/upgrade_55R2_60.zip");

        mWebsitePath = HttpContext.Current.Server.MapPath("~/");

        TableManager dtm = new TableManager(null);

        // Update all views
        dtm.RefreshDocumentViews();

        // Set data version
        ObjectHelper.SetSettingsKeyValue("CMSDataVersion", "6.0");

        // Clear hashtables
        CMSObjectHelper.ClearHashtables();

        // Clear the cache
        CacheHelper.ClearCache(null, true);

        // Drop the routes
        CMSMvcHandler.DropAllRoutes();

        // Init the Mimetype helper (required for the Import)
        MimeTypeHelper.LoadMimeTypes();

        CMSThread thread = new CMSThread(Upgrade55R2Import);
        thread.Start();
    }


    private static void Upgrade55R2Import()
    {
        EventLogProvider evp = new EventLogProvider();
        // Import
        try
        {
            RequestStockHelper.Remove("CurrentDomain", true);

            SiteImportSettings importSettings = new SiteImportSettings(CMSContext.CurrentUser)
            {
                DefaultProcessObjectType = ProcessObjectEnum.All,
                SourceFilePath = mUpgradePackagePath,
                WebsitePath = mWebsitePath
            };

            ImportProvider.ImportObjectsData(importSettings);

            // Regenerate time zones
            TimeZoneInfoProvider.GenerateTimeZoneRules();

            HandleSeparability();

            evp.LogEvent("I", DateTime.Now, "Upgrade to 6.0", "Upgrade - Finish");
        }
        catch (Exception ex)
        {
            evp.LogEvent("Upgrade to 6.0", "Upgrade", ex);
        }
    }

    #endregion


    #region "Private methods"

    private static List<String> GetAllFiles(String folder)
    {
        List<String> files = new List<string>();

        files.AddRange(Directory.GetFiles(folder));

        string[] dirs = Directory.GetDirectories(folder);

        foreach (string dir in dirs)
        {
            files.AddRange(GetAllFiles(dir));
        }

        return files;
    }

    private static void HandleSeparability()
    {
        String webPartsPath = mWebsitePath + "CMSWebParts\\";
        List<String> files = new List<string>();

        // Create list of files to remove
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.BIZFORM))
        {
            files.AddRange(GetAllFiles(webPartsPath + "BizForms"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.BLOGS))
        {
            files.AddRange(GetAllFiles(webPartsPath + "Blogs"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.COMMUNITY))
        {
            files.AddRange(GetAllFiles(webPartsPath + "Community"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.ECOMMERCE))
        {
            files.AddRange(GetAllFiles(webPartsPath + "Ecommerce"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.EVENTMANAGER))
        {
            files.AddRange(GetAllFiles(webPartsPath + "Events"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.FORUMS))
        {
            files.AddRange(GetAllFiles(webPartsPath + "Forums"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.MEDIALIBRARY))
        {
            files.AddRange(GetAllFiles(webPartsPath + "MediaLibrary"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.MESSAGEBOARD))
        {
            files.AddRange(GetAllFiles(webPartsPath + "MessageBoards"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.MESSAGING))
        {
            files.AddRange(GetAllFiles(webPartsPath + "Messaging"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.NEWSLETTER))
        {
            files.AddRange(GetAllFiles(webPartsPath + "Newsletters"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.NOTIFICATIONS))
        {
            files.AddRange(GetAllFiles(webPartsPath + "Notifications"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.ONLINEMARKETING))
        {
            files.AddRange(GetAllFiles(webPartsPath + "OnlineMarketing"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.POLLS))
        {
            files.AddRange(GetAllFiles(webPartsPath + "Polls"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.PROJECTMANAGEMENT))
        {
            files.AddRange(GetAllFiles(webPartsPath + "ProjectManagement"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.REPORTING))
        {
            files.AddRange(GetAllFiles(webPartsPath + "Reporting"));
        }
        if (!ModuleEntry.IsModuleLoaded(ModuleEntry.CHAT))
        {
            files.AddRange(GetAllFiles(webPartsPath + "Chat"));
        }

        // Remove web parts for separated modules
        foreach (String file in files)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Upgrade - Remove separated web parts", "Upgrade", ex);
            }
        }
    }

    private static void ImportMetaFiles(String upgradeFolder)
    {
        try
        {
            String xmlPath = Path.Combine(upgradeFolder, "metafiles.xml");
            if (File.Exists(xmlPath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(xmlPath);

                XmlNode metaFilesNode = xDoc.SelectSingleNode("MetaFiles");
                if (metaFilesNode != null)
                {
                    String filesDirectory = Path.Combine(upgradeFolder, "Metafiles");
                    InfoDataSet<MetaFileInfo> metaFilesSet = null;

                    using (CMSActionContext ctx = new CMSActionContext() { LogEvents = false })
                    {
                        foreach (XmlNode metaFile in metaFilesNode)
                        {
                            // Load metafiles information from XML
                            String objType = metaFile.Attributes["ObjectType"].Value;
                            String groupName = metaFile.Attributes["GroupName"].Value;
                            String codeName = metaFile.Attributes["CodeName"].Value;
                            String fileName = metaFile.Attributes["FileName"].Value;
                            String extension = metaFile.Attributes["Extension"].Value;
                            String fileGUID = metaFile.Attributes["FileGUID"].Value;
                            String title = (metaFile.Attributes["Title"] != null) ? metaFile.Attributes["Title"].Value : null;
                            String description = (metaFile.Attributes["Description"] != null) ? metaFile.Attributes["Description"].Value : null;

                            // Try to find correspondent info object
                            BaseInfo infoObject = BaseAbstractInfoProvider.GetInfoByName(objType, codeName);
                            if (infoObject != null)
                            {
                                MetaFileInfo mfInfo = null;
                                int infoObjectId = infoObject.Generalized.ObjectID;
                                String mfFileName = String.Format("{0}.{1}", fileGUID, extension.TrimStart('.'));
                                String mfFilePath = Path.Combine(filesDirectory, mfFileName);

                                // Check if metafile exists or have binary data (issue with new web parts)
                                metaFilesSet = MetaFileInfoProvider.GetMetaFilesWithBinary(infoObjectId, objType, groupName, null, null);
                                
                                if (DataHelper.DataSourceIsEmpty(metaFilesSet))
                                {
                                    // Create new metafile if does not exists
                                    mfInfo = new MetaFileInfo(mfFilePath, infoObjectId, objType, groupName);
                                }
                                else if (!DataHelper.DataSourceIsEmpty(metaFilesSet) && (metaFilesSet.Items[0].MetaFileBinary == null))
                                {
                                    mfInfo = metaFilesSet.Items[0];

                                    // Update binary data (issue with importing web parts)
                                    mfInfo.InputStream = FileStream.New(mfFilePath, FileMode.Open, FileAccess.Read);
                                }
                                
                                if (mfInfo != null)
                                {
                                    mfInfo.MetaFileName = fileName;
                                    if (title != null)
                                    {
                                        mfInfo.MetaFileTitle = title;
                                    }
                                    if (description != null)
                                    {
                                        mfInfo.MetaFileDescription = description;
                                    }

                                    // Save new meta file
                                    MetaFileInfoProvider.SetMetaFileInfo(mfInfo);
                                }
                            }
                        }

                        if (!AzureHelper.IsRunningOnAzure)
                        {
                            // Remove existing files after successful finish
                            String[] files = Directory.GetFiles(upgradeFolder);
                            foreach (String file in files)
                            {
                                File.Delete(file);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Upgrade - Import metafiles", "Upgrade", ex);
        }
    }

    #endregion
}