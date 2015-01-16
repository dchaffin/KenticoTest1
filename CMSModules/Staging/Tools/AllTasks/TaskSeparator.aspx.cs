using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.Synchronization;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Staging_Tools_AllTasks_TaskSeparator : CMSStagingAllTasksPage
{
    #region "Properties"

    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        string siteName = CMSContext.CurrentSiteName;

        // Check enabled servers
        if (!ServerInfoProvider.IsEnabledServer(CMSContext.CurrentSiteID))
        {
            ShowInformation(GetString("ObjectStaging.NoEnabledServer"));
        }
        else
        {
            // Check DLL required for for staging
            if (SiteManagerFunctions.CheckStagingDLL())
            {
                URLHelper.Redirect("Frameset.aspx");
            }

            ShowInformation(GetString("ObjectStaging.RenameDll"));
        }
    }
}