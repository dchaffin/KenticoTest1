using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("Objects/CMS_CustomTable/object.png", "customtable.list.Title", "custom_tables_list", HelpName = "helpTopic")]
[Actions(1)]
[Action(0, "Objects/CMS_CustomTable/add.png", "customtable.list.NewCustomTable", "CustomTable_New.aspx")]
public partial class CMSModules_CustomTables_CustomTable_List : CMSCustomTablesPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize grid
        uniGrid.OnAction += uniGrid_OnAction;
        uniGrid.ZeroRowsText = GetString("general.nodatafound");
    }

    #endregion


    #region "Grid events"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("CustomTable_Edit.aspx?customtableid=" + actionArgument);
        }
        else if (actionName == "delete")
        {
            int classId = ValidationHelper.GetInteger(actionArgument, 0);

            if (classId > 0)
            {
                // If no item depends on the current class
                if (!DataClassInfoProvider.CheckDependencies(classId))
                {
                    // Delete the class
                    DataClassInfoProvider.DeleteDataClass(classId);
                    CustomTableItemProvider.ClearCustomLicHash();
                }
            }
            else
            {
                // Display error on deleting
                ShowError(GetString("customtable.delete.hasdependencies"));
            }
        }
    }

    #endregion
}