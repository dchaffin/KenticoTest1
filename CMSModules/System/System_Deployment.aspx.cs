using System;
using System.Threading;
using System.Collections;
using System.Security.Principal;
using System.Collections.Generic;

using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.VirtualPathHelper;

using IOExceptions = System.IO;

public partial class CMSModules_System_System_Deployment : SiteManagerPage
{
    #region "Variables"

    private static readonly Hashtable mErrors = new Hashtable();
    private List<string> deletePaths = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current log context.
    /// </summary>
    public LogContext CurrentLog
    {
        get
        {
            return EnsureLog();
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mErrors["TranslateError_" + ctlAsync.ProcessGUID], string.Empty);
        }
        set
        {
            mErrors["TranslateError_" + ctlAsync.ProcessGUID] = value;
        }
    }

    #endregion


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        deletePaths = new List<string>();

        ctlAsync.OnFinished += ctlAsync_OnFinished;
        ctlAsync.OnError += ctlAsync_OnError;
        ctlAsync.OnRequestLog += ctlAsync_OnRequestLog;

        titleElemAsync.TitleText = GetString("Deployment.Processing");
        titleElemAsync.TitleImage = GetImageUrl("CMSModules/CMS_System/deploy.png");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        chkSaveCSS.Checked = CssStylesheetInfoProvider.StoreCSSStyleSheetsInExternalStorage;
        chkSaveLayouts.Checked = LayoutInfoProvider.StoreLayoutsInExternalStorage;
        chkSavePageTemplate.Checked = PageTemplateInfoProvider.StorePageTemplatesInExternalStorage;
        chkSaveWebpartLayout.Checked = WebPartLayoutInfoProvider.StoreWebPartLayoutsInExternalStorage;
        chkSaveTransformation.Checked = TransformationInfoProvider.StoreTransformationsInExternalStorage;
        chkSaveWebpartContainer.Checked = WebPartContainerInfoProvider.StoreWebPartContainersInExternalStorage;

        if (chkSaveCSS.Checked || chkSaveLayouts.Checked || chkSavePageTemplate.Checked ||
            chkSaveWebpartLayout.Checked || chkSaveTransformation.Checked || chkSaveWebpartContainer.Checked)
        {
            lblSynchronization.Visible = true;
            btnSynchronize.Visible = true;
        }

        bool deploymentMode = VirtualPathHelper.DeploymentMode;
        chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveTransformation.Enabled = chkSaveWebpartLayout.Enabled = !deploymentMode;

        if (StorageHelper.IsExternalStorage("~/"))
        {
            ShowWarning(GetString("Deployment.AzureDisabled"), null, null);
            btnSaveAll.Enabled = false;
            btnSourceControl.Enabled = false;
            chkSaveCSS.Enabled = chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveTransformation.Enabled = chkSaveWebpartLayout.Enabled = chkSaveWebpartContainer.Enabled = false;
        }

        if (VirtualPathHelper.DeploymentMode)
        {
            lblDeploymentInfo.Text = GetString("Deployment.SaveAllToDBInfo");
            btnSaveAll.ResourceString = "Deployment.SaveAllToDB";
            lblSourceControlInfo.Text = GetString("Deployment.SourceControlInfoDeploymentMode");
        }
        else
        {
            lblDeploymentInfo.Text = GetString("Deployment.SaveAllInfo");
            btnSaveAll.ResourceString = "Deployment.SaveAll";
            lblSourceControlInfo.Text = GetString("Deployment.SourceControlInfo");
        }
    }


    #region "Deployment methods"

    private void SaveToExternalStorage(InfoObjectCollection collection)
    {
        if (collection == null)
        {
            return;
        }

        foreach (BaseInfo info in collection)
        {
            AddLog(string.Format(GetString("Deployment.Deploying"), GetString("objecttype." + collection.ObjectType.Replace(".", "_")), info.Generalized.ObjectDisplayName));
            try
            {
                info.Generalized.SaveExternalColumns();
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
        }
    }


    private void SaveToDB(InfoObjectCollection collection)
    {
        if (collection == null)
        {
            return;
        }

        foreach (BaseInfo info in collection)
        {
            AddLog(string.Format(GetString("Deployment.Deploying"), GetString("objecttype." + collection.ObjectType.Replace(".", "_")), info.Generalized.ObjectDisplayName));
            try
            {
                info.Generalized.IgnoreExternalColumns = true;
                info.Generalized.UpdateExternalColumns();
                info.Generalized.IgnoreExternalColumns = false;
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
        }
    }


    private void DeleteDir(string path)
    {
        if (deletePaths != null)
        {
            deletePaths.Add(path);
        }
    }


    private void DeleteDirs()
    {
        if (deletePaths != null)
        {
            foreach (var path in deletePaths)
            {
                DirectoryInfo dir = DirectoryInfo.New(URLHelper.GetPhysicalPath(path));
                if (dir.Exists)
                {
                    DirectoryHelper.DeleteDirectory(dir.FullName, true);
                }
            }
        }
    }


    private void ProcessTransformations(bool storeInDB, bool deleteFiles)
    {
        if (TransformationInfoProvider.StoreTransformationsInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + PredefinedObjectType.TRANSFORMATION.Replace(".", "_"))));
            }
            SaveToDB(CMSContext.Current.Transformations);

            if (deleteFiles)
            {
                DeleteDir(TransformationInfoProvider.TransformationsDirectory);
                TransformationInfoProvider.StoreTransformationsInExternalStorage = false;
            }
        }
        else
        {
            TransformationInfoProvider.StoreTransformationsInExternalStorage = true;
            SaveToExternalStorage(CMSContext.Current.Transformations);
        }
    }


    private void ProcessWebpartLayouts(bool storeInDB, bool deleteFiles)
    {
        if (WebPartLayoutInfoProvider.StoreWebPartLayoutsInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + PredefinedObjectType.WEBPARTLAYOUT.Replace(".", "_"))));
            }
            SaveToDB(CMSContext.Current.WebPartLayouts);
            if (deleteFiles)
            {
                DeleteDir(WebPartLayoutInfoProvider.WebPartLayoutsDirectory);
                WebPartLayoutInfoProvider.StoreWebPartLayoutsInExternalStorage = false;
            }
        }
        else
        {
            WebPartLayoutInfoProvider.StoreWebPartLayoutsInExternalStorage = true;
            SaveToExternalStorage(CMSContext.Current.WebPartLayouts);
        }
    }


    private void ProcessWebpartContainers(bool storeInDB, bool deleteFiles)
    {
        if (WebPartContainerInfoProvider.StoreWebPartContainersInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + PredefinedObjectType.WEBPARTCONTAINER.Replace(".", "_"))));
            }
            SaveToDB(CMSContext.Current.GlobalObjects[PredefinedObjectType.WEBPARTCONTAINER]);
            if (deleteFiles)
            {
                DeleteDir(WebPartContainerInfoProvider.WebPartContainersDirectory);
                WebPartContainerInfoProvider.StoreWebPartContainersInExternalStorage = false;
            }
        }
        else
        {
            WebPartContainerInfoProvider.StoreWebPartContainersInExternalStorage = true;
            SaveToExternalStorage(CMSContext.Current.GlobalObjects[PredefinedObjectType.WEBPARTCONTAINER]);
        }
    }


    private void ProcessLayouts(bool storeInDB, bool deleteFiles)
    {
        if (LayoutInfoProvider.StoreLayoutsInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + PredefinedObjectType.PAGELAYOUT.Replace(".", "_"))));
            }
            SaveToDB(CMSContext.Current.GlobalObjects[PredefinedObjectType.PAGELAYOUT]);
            if (deleteFiles)
            {
                DeleteDir(LayoutInfoProvider.LayoutsDirectory);
                LayoutInfoProvider.StoreLayoutsInExternalStorage = false;
            }
        }
        else
        {
            LayoutInfoProvider.StoreLayoutsInExternalStorage = true;
            SaveToExternalStorage(CMSContext.Current.GlobalObjects[PredefinedObjectType.PAGELAYOUT]);
        }
    }


    private void ProcessCSS(bool storeInDB, bool deleteFiles)
    {
        if (CssStylesheetInfoProvider.StoreCSSStyleSheetsInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + SiteObjectType.CSSSTYLESHEET.Replace(".", "_"))));
            }
            SaveToDB(CMSContext.Current.GlobalObjects[SiteObjectType.CSSSTYLESHEET]);
            if (deleteFiles)
            {
                DeleteDir(CssStylesheetInfoProvider.CSSStylesheetsDirectory);
                CssStylesheetInfoProvider.StoreCSSStyleSheetsInExternalStorage = false;
            }
        }
        else
        {
            CssStylesheetInfoProvider.StoreCSSStyleSheetsInExternalStorage = true;
            SaveToExternalStorage(CMSContext.Current.GlobalObjects[SiteObjectType.CSSSTYLESHEET]);
        }
    }


    private void ProcessTemplates(bool storeInDB, bool deleteFiles)
    {
        if (PageTemplateInfoProvider.StorePageTemplatesInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + PredefinedObjectType.PAGETEMPLATE.Replace(".", "_"))));
            }
            if (CMSContext.Current.GlobalObjects[PredefinedObjectType.PAGETEMPLATE] != null)
            {
                foreach (PageTemplateInfo info in CMSContext.Current.GlobalObjects[PredefinedObjectType.PAGETEMPLATE])
                {
                    // Save all the device layouts
                    ProcessTemplateToDB(info);
                }

                // AdHoc templates
                foreach (PageTemplateInfo info in PageTemplateInfoProvider.GetTemplates("PageTemplateIsReusable = 0", null))
                {
                    ProcessTemplateToDB(info);
                }
            }

            if (deleteFiles)
            {
                DeleteDir("~/CMSVirtualFiles/Templates/");
                PageTemplateInfoProvider.StorePageTemplatesInExternalStorage = false;
            }
        }
        else
        {
            PageTemplateInfoProvider.StorePageTemplatesInExternalStorage = true;
            foreach (PageTemplateInfo info in CMSContext.Current.GlobalObjects[PredefinedObjectType.PAGETEMPLATE])
            {
                ProcessTemplateToFS(info);
            }

            // AdHoc templates
            foreach (PageTemplateInfo info in PageTemplateInfoProvider.GetTemplates("ISNULL(PageTemplateIsReusable, 0) = 0", null))
            {
                ProcessTemplateToFS(info);
            }
        }
    }


    private static void ProcessTemplateToFS(PageTemplateInfo info)
    {
        var devices = info.Children[PortalObjectType.TEMPLATEDEVICELAYOUT];
        if (devices != null)
        {
            foreach (PageTemplateDeviceLayoutInfo device in devices)
            {
                device.Generalized.SaveExternalColumns();
            }
        }

        if ((info.IsPortal || info.PageTemplateType == PageTemplateTypeEnum.Dashboard) && !string.IsNullOrEmpty(info.PageTemplateLayout))
        {
            info.Generalized.SaveExternalColumns();
        }
    }


    private static void ProcessTemplateToDB(PageTemplateInfo info)
    {
        var devices = info.Children[PortalObjectType.TEMPLATEDEVICELAYOUT];
        if (devices != null)
        {
            foreach (PageTemplateDeviceLayoutInfo device in devices)
            {
                device.Generalized.IgnoreExternalColumns = true;
                device.Generalized.UpdateExternalColumns();
                device.Generalized.IgnoreExternalColumns = false;
            }
        }

        if ((info.IsPortal || info.PageTemplateType == PageTemplateTypeEnum.Dashboard) && !string.IsNullOrEmpty(info.PageTemplateLayout))
        {
            info.Generalized.IgnoreExternalColumns = true;
            info.Generalized.UpdateExternalColumns();
            info.Generalized.IgnoreExternalColumns = false;
        }
    }


    /// <summary>
    /// Does the deployment of the given object
    /// </summary>
    private void SaveExternally(object parameter)
    {
        try
        {
            bool[] store = (bool[])parameter;

            if (!VirtualPathHelper.DeploymentMode)
            {
                ProcessLayouts(!store[1], true);
                ProcessTemplates(!store[2], true);
                ProcessTransformations(!store[3], true);
                ProcessWebpartLayouts(!store[4], true);
            }

            ProcessWebpartContainers(!store[5], true);
            ProcessCSS(!store[0], true);

            // Delete dirs at the end because of restart
            DeleteDirs();
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                AddError(ResHelper.GetString("Deployment.DeploymentCanceled"));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (IOExceptions.IOException ex)
        {
            LogExceptionToEventLog(ex);
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
    }


    /// <summary>
    /// Does the deployment of the given object
    /// </summary>
    private void Deploy(object parameter)
    {
        try
        {
            bool targetMode = VirtualPathHelper.DeploymentMode;

            // In the direction DB -> FS we need to set the deployment mode before processing
            if (!targetMode)
            {
                VirtualPathHelper.DeploymentMode = true;
            }

            ProcessLayouts(targetMode, true);
            ProcessTemplates(targetMode, true);
            ProcessTransformations(targetMode, true);
            ProcessWebpartLayouts(targetMode, true);

            // In the direction FS -> DB we need to set the deployment mode after processing
            if (targetMode)
            {
                VirtualPathHelper.DeploymentMode = false;
            }

            // Delete dirs at the end because of restart
            DeleteDirs();
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                AddError(ResHelper.GetString("Deployment.DeploymentCanceled"));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (IOExceptions.IOException ex)
        {
            LogExceptionToEventLog(ex);
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
    }


    private void Synchronize(object parameter)
    {
        try
        {
            if (chkSaveLayouts.Checked)
            {
                ProcessLayouts(true, false);
            }

            if (chkSavePageTemplate.Checked)
            {
                ProcessTemplates(true, false);
            }

            if (chkSaveTransformation.Checked)
            {
                ProcessTransformations(true, false);
            }
            
            if (chkSaveWebpartLayout.Checked)
            {
                ProcessWebpartLayouts(true, false);
            }

            if (chkSaveWebpartContainer.Checked)
            {
                ProcessWebpartContainers(true, false);
            }

            if (chkSaveCSS.Checked)
            {
                ProcessCSS(true, false);
            }
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                AddError(ResHelper.GetString("Deployment.DeploymentCanceled"));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (IOExceptions.IOException ex)
        {
            LogExceptionToEventLog(ex);
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
    }


    protected void btnSynchronize_Click(object sender, EventArgs e)
    {
        pnlLog.Visible = true;

        CurrentError = string.Empty;
        CurrentLog.Close();
        EnsureLog();

        ctlAsync.RunAsync(Synchronize, WindowsIdentity.GetCurrent());
    }


    protected void btnSourceControl_Click(object sender, EventArgs e)
    {
        pnlLog.Visible = true;

        CurrentError = string.Empty;
        CurrentLog.Close();
        EnsureLog();

        ctlAsync.Parameter = new bool[] { chkSaveCSS.Checked, chkSaveLayouts.Checked, chkSavePageTemplate.Checked, chkSaveTransformation.Checked, chkSaveWebpartLayout.Checked, chkSaveWebpartContainer.Checked };
        ctlAsync.RunAsync(SaveExternally, WindowsIdentity.GetCurrent());
    }


    protected void btnSaveAll_Click(object sender, EventArgs e)
    {
        pnlLog.Visible = true;

        CurrentError = string.Empty;
        CurrentLog.Close();
        EnsureLog();

        ctlAsync.RunAsync(Deploy, WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// When exception occures, log it to event log.
    /// </summary>
    /// <param name="ex">Exception to log</param>
    private void LogExceptionToEventLog(Exception ex)
    {
        EventLogProvider log = new EventLogProvider();

        log.LogEvent(EventLogProvider.EVENT_TYPE_ERROR, DateTime.Now, "System deployment", "DEPLOYMENT", CurrentUser.UserID, CurrentUser.UserName, 0, null, HTTPHelper.UserHostAddress, EventLogProvider.GetExceptionLogMessage(ex), CMSContext.CurrentSiteID, HTTPHelper.GetAbsoluteUri());

        AddError(ResHelper.GetString("Deployment.DeploymentFailed") + ": " + ex.Message);
    }


    private void ctlAsync_OnRequestLog(object sender, EventArgs e)
    {
        ctlAsync.Log = CurrentLog.Log;
    }


    private void ctlAsync_OnError(object sender, EventArgs e)
    {
        if (ctlAsync.Status == AsyncWorkerStatusEnum.Running)
        {
            ctlAsync.Stop();
        }
        ctlAsync.Parameter = null;
        ShowError(CurrentError);
        CurrentLog.Close();

        pnlLog.Visible = false;
    }


    private void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        CurrentLog.Close();

        if (!string.IsNullOrEmpty(CurrentError))
        {
            ctlAsync.Parameter = null;
            ShowError(CurrentError);
        }

        pnlLog.Visible = false;

        if (VirtualPathHelper.DeploymentMode)
        {
            ShowConfirmation(GetString("Deployment.ObjectsSavedSuccessfully"));
        }
        else
        {
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Ensures the logging context.
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext log = LogContext.EnsureLog(ctlAsync.ProcessGUID);
        log.Reversed = true;
        log.LineSeparator = "<br />";
        return log;
    }


    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        EnsureLog();
        LogContext.AppendLine(newLog);
    }


    /// <summary>
    /// Adds the error to collection of errors.
    /// </summary>
    /// <param name="error">Error message</param>
    protected void AddError(string error)
    {
        AddLog(error);
        CurrentError = (error + "<br />" + CurrentError);
    }

    #endregion
}