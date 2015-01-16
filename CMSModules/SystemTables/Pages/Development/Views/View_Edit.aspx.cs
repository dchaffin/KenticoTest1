using System;
using System.Web;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_SystemTables_Pages_Development_Views_View_Edit : SiteManagerPage
{
    #region "Private variables"

    /// <summary>
    /// Type of database object (0 - view; 1 - stored procedure).
    /// </summary>
    private int objType = -1;

    private string objName = null;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Query param validation
        if (!QueryHelper.ValidateHash("hash", "saved"))
        {
            ShowError(GetString("sysdev.views.corruptedparameters"));
            editSQL.Visible = false;
            return;
        }

        objName = HttpUtility.HtmlDecode(QueryHelper.GetString("objname", null));
        objType = QueryHelper.GetInteger("objtype", -1);

        // Check if edited view exists and redirect to error page if not
        TableManager tm = new TableManager(null);

        if (!String.IsNullOrEmpty(objName) && !tm.ViewExists(objName))
        {
            EditedObject = null;
        }

        // Init edit area
        editSQL.ObjectName = objName;
        editSQL.HideSaveButton = objName != null;
        editSQL.IsView = true;
        editSQL.OnSaved += new EventHandler(editSQL_OnSaved);
        bool loadedCorrectly = true;
        if (!RequestHelper.IsPostBack())
        {
            if (QueryHelper.GetBoolean("saved", false))
            {
                ShowChangesSaved();
            }

            loadedCorrectly = editSQL.SetupControl();
        }

        if (objName == null)
        {
            string[,] breadcrumbs = new string[2,3];
            breadcrumbs[0, 0] = GetString("sysdev.views");
            breadcrumbs[0, 1] = "~/CMSModules/SystemTables/Pages/Development/Views/Views_List.aspx";
            breadcrumbs[0, 2] = "";
            breadcrumbs[1, 0] = GetString("general.new");
            breadcrumbs[1, 1] = "";
            breadcrumbs[1, 2] = "";

            CurrentMaster.Title.Breadcrumbs = breadcrumbs;
            CurrentMaster.Title.HelpName = "helpTopic";
            CurrentMaster.Title.HelpTopicName = "systemtables_views_new";
        }
        else
        {
            // Header actions
            string[,] actions = new string[2,11];

            // Save button
            actions[0, 0] = HeaderActions.TYPE_SAVEBUTTON;
            actions[0, 1] = GetString("General.Save");
            actions[0, 5] = GetImageUrl("CMSModules/CMS_Content/EditMenu/" + (loadedCorrectly ? "save.png" : "savedisabled.png"));
            actions[0, 6] = "save";
            actions[0, 8] = loadedCorrectly.ToString();
            actions[0, 9] = loadedCorrectly.ToString();

            // Undo button
            actions[1, 0] = HeaderActions.TYPE_HYPERLINK;
            actions[1, 1] = GetString("General.Rollback");
            actions[1, 5] = GetImageUrl("CMSModules/CMS_Content/EditMenu/undo.png");
            actions[1, 6] = "rollback";
            actions[1, 10] = editSQL.RollbackAvailable.ToString();

            CurrentMaster.HeaderActions.Actions = actions;
            CurrentMaster.HeaderActions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);
        }
    }


    #region "Event handlers"

    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "rollback":
                editSQL.Rollback();
                editSQL.SaveQuery();
                break;
            case "save":
                editSQL.SaveQuery();
                break;
        }
    }


    private void editSQL_OnSaved(object sender, EventArgs e)
    {
        if (objName == null)
        {
            string query = "?objname=" + editSQL.ObjectName;
            query += "&hash=" + QueryHelper.GetHash(query) + "&saved=1";
            URLHelper.Redirect("~/CMSModules/SystemTables/Pages/Development/Views/ViewEdit_Frameset.aspx" + query);
        }

        ShowChangesSaved();
    }

    #endregion
}