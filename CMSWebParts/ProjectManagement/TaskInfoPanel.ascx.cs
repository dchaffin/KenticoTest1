using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.ProjectManagement;
using CMS.SettingsProvider;

public partial class CMSWebParts_ProjectManagement_TaskInfoPanel : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the task detail URL page.
    /// </summary>
    public string TaskDetailUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TaskDetailUrl"), String.Empty);
        }
        set
        {
            SetValue("TaskDetailUrl", value);
        }
    }


    /// <summary>
    /// Text of linked label.
    /// </summary>
    public string InfoText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InfoText"), String.Empty);
        }
        set
        {
            SetValue("InfoText", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether finished tasks should be included to the count of active tasks.
    /// </summary>
    public bool IncludeFinishedTasks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IncludeFinishedTasks"), false);
        }
        set
        {
            SetValue("IncludeFinishedTasks", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether not started tasks should be included to the count of active tasks.
    /// </summary>
    public bool IncludeNotStartedTasks
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IncludeNotStartedTasks"), false);
        }
        set
        {
            SetValue("IncludeNotStartedTasks", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Setup control.
    /// </summary>
    protected void SetupControl()
    {
        if (!StopProcessing)
        {
            // Get active tasks
            int count = ProjectTaskInfoProvider.GetUserActiveTasksCount(CMSContext.CurrentUser.UserID, null, IncludeFinishedTasks, IncludeNotStartedTasks);

            // Fill link text
            lnkProjects.Text = String.Format(InfoText, count);
            lnkProjects.ToolTip = lnkProjects.Text;

            // Set specified task detail page 
            if (!String.IsNullOrEmpty(TaskDetailUrl))
            {
                lnkProjects.NavigateUrl = URLHelper.ResolveUrl(TaskDetailUrl);
            }
            // Set default task detail page
            else
            {
                lnkProjects.NavigateUrl = URLHelper.ResolveUrl(SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".CMSTaskDetailPage"));
            }
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion
}