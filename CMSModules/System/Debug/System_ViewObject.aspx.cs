using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_System_Debug_System_ViewObject : CMSDebugPage
{
    #region "Variables"

    protected string key = null;
    protected string source = null;

    protected bool wasDeleted = false;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterWOpenerScript(this);

        // Set the title
        CurrentMaster.Title.TitleText = GetString("ViewObject.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/__GLOBAL__/Object.png");
        Page.Title = GetString("ViewObject.Title");

        // Resend all failed
        string[,] actions = new string[1,10];
        actions[0, 0] = HeaderActions.TYPE_LINKBUTTON;
        actions[0, 1] = GetString("general.delete");
        actions[0, 2] = "if (!confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ")) return false;";
        actions[0, 4] = GetString("general.delete");
        actions[0, 5] = GetImageUrl("Design/Controls/UniGrid/Actions/Delete.png");
        actions[0, 6] = "delete";

        actionsElem.Actions = actions;
        actionsElem.ActionPerformed += new CommandEventHandler(actionsElem_ActionPerformed);

        gridDependencies.OnItemDeleted += new EventHandler(gridDependencies_OnItemDeleted);

        source = QueryHelper.GetString("source", "");
        key = QueryHelper.GetString("key", "");

        ReloadData();
    }


    protected void gridDependencies_OnItemDeleted(object sender, EventArgs e)
    {
        wasDeleted = true;

        // Close the window
        ScriptHelper.CloseWindow(Page, true);
    }


    protected void ReloadData()
    {
        object obj = null;

        switch (source.ToLowerCSafe())
        {
            case "cache":
                {
                    // Get the item from cache
                    obj = HttpRuntime.Cache[key];

                    // Take the object from the cache
                    if (obj != null)
                    {
                        pnlActions.Visible = true;

                        if (obj is CacheItemContainer)
                        {
                            // Setup the advanced information
                            CacheItemContainer container = (CacheItemContainer)obj;
                            obj = container.Data;

                            // Get the inner value
                            obj = CacheHelper.GetInnerValue(obj);

                            ltlKey.Text = key;
                            ltlPriority.Text = container.Priority.ToString();
                            if (container.AbsoluteExpiration != Cache.NoAbsoluteExpiration)
                            {
                                ltlExpiration.Text = container.AbsoluteExpiration.ToString();
                            }
                            else
                            {
                                ltlExpiration.Text = container.SlidingExpiration.ToString();
                            }

                            // Additional info
                            if (container.Dependencies is CMSCacheDependency)
                            {
                                CMSCacheDependency cd = (CMSCacheDependency)container.Dependencies;

                                if (!RequestHelper.IsPostBack())
                                {
                                    gridDependencies.PagerControl.UniPager.PageSize = 10;
                                }

                                gridDependencies.AllItems = cd.CacheKeys;
                                gridDependencies.ReloadData();
                            }

                            gridDependencies.Visible = gridDependencies.TotalItems > 0;
                            ltlDependencies.Visible = gridDependencies.TotalItems == 0;

                            pnlCacheItem.Visible = true;
                        }
                    }
                    else if (wasDeleted)
                    {
                        ShowError(GetString("general.wasdeleted"));
                    }
                    else
                    {
                        ShowError(GetString("general.objectnotfound"));
                    }
                }
                break;
        }

        objElem.Object = obj;
    }


    private void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "delete":
                // Delete the item from the cache
                if (!string.IsNullOrEmpty(key))
                {
                    CacheHelper.Remove(key);

                    ReloadData();
                    pnlActions.Visible = false;

                    // Close the window
                    ScriptHelper.CloseWindow(Page, true);
                }
                break;
        }
    }

    #endregion
}