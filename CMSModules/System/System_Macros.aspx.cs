using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.EmailEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.ResourceManager;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_System_System_Macros : SiteManagerPage
{
    private const string EVENTLOG_SOURCE_REFRESHSECURITYPARAMS = "Macros - Refresh security parameters";

    private NameValueCollection processedObjects = new NameValueCollection();


    /// <summary>
    /// Gets the log context for the async control.
    /// </summary>
    public LogContext AsyncLogContext
    {
        get
        {
            return EnsureAsyncLogContext();
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        InitRefreshSecurityParamsForm();
        InitAsyncDialog();
    }


    #region "Async log"

    /// <summary>
    /// Ensures and returns the log context for the async control.
    /// </summary>
    private LogContext EnsureAsyncLogContext()
    {
        var log = LogContext.EnsureLog(ucAsync.ProcessGUID);
        log.Reversed = true;
        log.LineSeparator = "<br />";

        return log;
    }


    /// <summary>
    /// Inits the async dialog.
    /// </summary>
    private void InitAsyncDialog()
    {
        btnCancel.Text = GetString("general.cancel");
        btnCancel.OnClientClick = ucAsync.GetCancelScript(true) + " return false;";

        ucAsync.OnRequestLog += (sender, args) =>
        {
            ucAsync.Log = AsyncLogContext.Log;
        };

        ucAsync.OnCancel += (sender, args) =>
        {
            EventLogProvider.LogInformation((string)ucAsync.Parameter, "CANCELLED");

            plcAsyncLog.Visible = false;
            AsyncLogContext.Close();

            ShowConfirmation(GetString("general.actioncanceled"));
        };

        ucAsync.OnFinished += (sender, args) =>
        {
            EventLogProvider.LogInformation((string)ucAsync.Parameter, "FINISHED");

            plcAsyncLog.Visible = false;
            AsyncLogContext.Close();

            ShowConfirmation(GetString("general.actionfinished"));
        };
    }


    /// <summary>
    /// Runs the specified action asynchronously.
    /// </summary>
    /// <param name="actionName">Action name</param>
    /// <param name="action">Action</param>
    private void RunAsync(string actionName, AsyncAction action)
    {
        // Set action name as process parameter
        ucAsync.Parameter = actionName;

        EnsureAsyncLogContext();

        // Log async action start
        EventLogProvider.LogInformation(actionName, "STARTED");

        // Run async action
        ucAsync.RunAsync(action, WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "Refresh security params"

    /// <summary>
    /// Inits the "Refresh security parameters" form.
    /// </summary>
    private void InitRefreshSecurityParamsForm()
    {
        pnlRefreshSecurityParams.GroupingText = GetString("macros.refreshsecurityparams");

        // Init old salt text box
        if (chkRefreshAll.Checked)
        {
            txtOldSalt.Enabled = false;
            txtOldSalt.Text = GetString("macros.refreshsecurityparams.refreshalldescription");
        }
        else
        {
            txtOldSalt.Enabled = true;
        }

        chkRefreshAll.CheckedChanged += (sender, args) =>
        {
            // Clear the textbox after enabling it
            if (!chkRefreshAll.Checked)
            {
                txtOldSalt.Text = null;
            }
        };

        // Init new salt text box
        if (chkUseCurrentSalt.Checked)
        {
            txtNewSalt.Enabled = false;

            var customSalt = SettingsHelper.AppSettings["CMSHashStringSalt"];
            if (string.IsNullOrEmpty(customSalt))
            {
                txtNewSalt.Text = GetString("macros.refreshsecurityparams.currentsaltisconnectionstring");
            }
            else
            {
                txtNewSalt.Text = GetString("macros.refreshsecurityparams.currentsaltiscustomvalue");
            }
        }
        else
        {
            txtNewSalt.Enabled = true;
        }

        chkUseCurrentSalt.CheckedChanged += (sender, args) =>
        {
            // Clear the textbox after enabling it
            if (!chkUseCurrentSalt.Checked)
            {
                txtNewSalt.Text = null;
            }
        };

        // Init submit button
        btnRefreshSecurityParams.Text = pnlRefreshSecurityParams.GroupingText;
        btnRefreshSecurityParams.Click += (sender, args) =>
        {
            var oldSaltInput = txtOldSalt.Text.Trim();
            var newSaltInput = txtNewSalt.Text.Trim();

            if (!chkRefreshAll.Checked && string.IsNullOrEmpty(oldSaltInput))
            {
                ShowError(GetString("macros.refreshsecurityparams.oldsaltempty"));
                return;
            }

            if (!chkUseCurrentSalt.Checked && string.IsNullOrEmpty(newSaltInput))
            {
                ShowError(GetString("macros.refreshsecurityparams.newsaltempty"));
                return;
            }

            plcAsyncLog.Visible = true;
            var objectTypes = GetObjectTypesWithMacros(null, null);

            RunAsync(EVENTLOG_SOURCE_REFRESHSECURITYPARAMS, p => RefreshSecurityParams(objectTypes, oldSaltInput, newSaltInput));
        };
    }


    /// <summary>
    /// Refreshes the security parameters in macros for all the objects of the specified object types.
    /// Signs all the macros with the current user if the old salt is not specified.
    /// </summary>
    /// <param name="objectTypes">Object types</param>
    /// <param name="oldSalt">Old salt </param>
    /// <param name="newSalt">New salt</param>
    private void RefreshSecurityParams(IEnumerable<string> objectTypes, string oldSalt, string newSalt)
    {
        var oldSaltSpecified = !string.IsNullOrEmpty(oldSalt) && !chkRefreshAll.Checked;
        var newSaltSpecified = !string.IsNullOrEmpty(newSalt) && !chkUseCurrentSalt.Checked;

        processedObjects.Clear();

        using (CMSActionContext context = new CMSActionContext())
        {
            context.LogEvents = false;
            context.LogSynchronization = false;

            foreach (var objectType in objectTypes)
            {
                var objectTypeResourceKey = TypeHelper.GetObjectTypeResourceKey(objectType);
                var niceObjectType = GetString(objectTypeResourceKey);
                if (niceObjectType == objectTypeResourceKey)
                {
                    if (objectType.StartsWithCSafe("bizformitem.bizform.", true))
                    {
                        DataClassInfo dci = DataClassInfoProvider.GetDataClass(objectType.Substring("bizformitem.".Length));
                        if (dci != null)
                        {
                            niceObjectType = "on-line form " + dci.ClassDisplayName;
                        }
                    }
                    else
                    {
                        niceObjectType = objectType;
                    }
                }

                LogContext.AppendLine(string.Format(GetString("macros.refreshsecurityparams.processing"), niceObjectType));

                try
                {
                    var infos = InfoObjectCollection.New(objectType);
                    foreach (var info in infos)
                    {
                        bool refreshed = false;
                        if (oldSaltSpecified)
                        {
                            refreshed = MacroResolver.RefreshSecurityParameters(info, oldSalt, newSaltSpecified ? newSalt : ValidationHelper.HashStringSalt, true);
                        }
                        else
                        {
                            if (chkRefreshAll.Checked && newSaltSpecified)
                            {
                                // Do not check integrity, but use new salt
                                refreshed = MacroResolver.RefreshSecurityParameters(info, CMSContext.CurrentUser.UserName, true, newSalt);
                            }
                            else
                            {
                                // Do not check integrity, sign everything with current user, current salt
                                refreshed = MacroResolver.RefreshSecurityParameters(info, CMSContext.CurrentUser.UserName, true);
                            }
                        }

                        if (refreshed)
                        {
                            var objectName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(info.Generalized.ObjectDisplayName));
                            processedObjects.Add(niceObjectType, objectName);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogContext.AppendLine(e.Message);
                    EventLogProvider.LogException(EVENTLOG_SOURCE_REFRESHSECURITYPARAMS, "ERROR", e);
                }
            }
        }

        EventLogProvider.LogInformation(EVENTLOG_SOURCE_REFRESHSECURITYPARAMS, "PROCESSEDOBJECTS", GetProcessedObjectsForEventLog());
    }


    /// <summary>
    /// Builds and returns the list of object types that can contain macros.
    /// </summary>
    /// <param name="include">Object types to include in the list</param>
    /// <param name="exclude">Object types to exclude from the list</param>
    /// <remarks>
    /// Excludes the object types that cannot contain macros.
    /// </remarks>
    private IEnumerable<string> GetObjectTypesWithMacros(IEnumerable<string> include, IEnumerable<string> exclude)
    {
        // Get the system object types
        var objectTypes = ObjectTypeLists.AllObjectTypes.AsEnumerable();

        // Include custom table object types
        objectTypes = objectTypes.Union(DataClassInfoProvider.GetCustomTableClasses(null, null, 0, "ClassName").Tables[0].Select().Select(r => r["ClassName"].ToString()));

        // Include biz form object types
        objectTypes = objectTypes.Union(BizFormInfoProvider.GetAllBizForms().Tables[0].Select().Select(r => "bizformitem.bizform." + r["FormName"].ToString()));

        // Include object types
        if (include != null)
        {
            objectTypes = objectTypes.Union(include);
        }

        // Exclude object types
        if (exclude != null)
        {
            objectTypes = objectTypes.Except(exclude);
        }

        // Exclude object types that do not contain macros
        objectTypes = objectTypes.Except(new[] {
            PredefinedObjectType.STATISTICS,
            
            EmailObjectType.EMAIL,
                        
            LicenseObjectType.LICENSEKEY,

            PredefinedObjectType.ACTIVITY,
            "om.pagevisit",
            "om.search",

            PredefinedObjectType.CHATONLINEUSER,
            PredefinedObjectType.EVENT,

            ResourceObjectType.RESOURCESTRING,
            ResourceObjectType.RESOURCETRANSLATION,
            ResourceObjectType.RESOURCETRANSLATED,

            "cms.objectsettings",

            SiteObjectType.USERROLELIST,
            SiteObjectType.MEMBERSHIPLIST,

            DocumentObjectType.USERDOCUMENTSLIST,

            "temp.file"
        });

        objectTypes = objectTypes.Where(t =>
        {
            try
            {
                var typeInfo = TranslationHelper.GetReadOnlyObject(t).TypeInfo;
                return (!typeInfo.Inherited && !typeInfo.IsBinding);
            }
            catch (Exception)
            {
                return false;
            }
        });

        return objectTypes;
    }


    /// <summary>
    /// Gets the list of processed objects formatted for use in the event log.
    /// </summary>
    /// <returns></returns>
    private string GetProcessedObjectsForEventLog()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in processedObjects.AllKeys)
        {
            sb.Append("<br />", string.Format("{0} '{1}'", item, processedObjects[item]));
        }
        string result = sb.ToString();
        if (!string.IsNullOrEmpty(result))
        {
            result = result.Substring(6);
        }
        return result;
    }

    #endregion
}
