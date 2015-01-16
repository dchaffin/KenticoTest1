using System;
using System.Data.SqlClient;

using CMS.DataEngine;
using CMS.DatabaseHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;


[CheckLicence(FeatureEnum.DBSeparation)]
public partial class CMSModules_System_System_DBseparation : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool separated = (String.IsNullOrEmpty(DBSeparationHelper.ConnStringSeparate));
        if (separated)
        {
            var connectionString = new SqlConnectionStringBuilder(SqlHelper.ConnectionString);
            if (connectionString.IntegratedSecurity)
            {
                btnLaunch.Visible = false;
                ShowWarning(GetString("separationDB.errorwinauth"), null, null);
            }
            else
            {
                btnLaunch.ResourceString = "separationDB.launchseparation";
                ShowInformation(GetString("separationDB.separatedescription"));
                btnLaunch.OnClientClick = "modalDialog('" + Page.ResolveUrl("~/CMSInstall/SeparateDB.aspx") + "','DBSeparation', 607, 607); return false;";
            }
        }
        else
        {
            btnLaunch.ResourceString = "separationDB.launchjoin";
            ShowInformation(GetString("separationDB.joindescription"));
            btnLaunch.OnClientClick = "modalDialog('" + Page.ResolveUrl("~/CMSInstall/JoinDB.aspx") + "','DBJoin', 607, 607); return false;";
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshPageScript", ScriptHelper.GetScript("function RefreshPage() { window.location.replace(window.location.href); }"));
        ScriptHelper.RegisterDialogScript(Page);
    }
}