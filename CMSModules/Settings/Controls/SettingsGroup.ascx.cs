using System;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

public partial class CMSModules_Settings_Controls_SettingsGroup : SettingsGroup
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Unigrid
        gridElem.HideControlForZeroRows = false;
        gridElem.OrderBy = "KeyOrder";
        gridElem.OnAction += new OnActionEventHandler(KeyAction);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.ZeroRowsText = GetString("settings.group.nokeysfound");

        if (Category != null)
        {
            string catIdStr = Category.CategoryID.ToString();

            // Header actions
            HeaderActions actions = cpCategory.HeaderActions;
            actions.UseBasicStyles = true;
            actions.IconCssClass = string.Empty;
            
            // Edit action
            actions.ActionsList.Add(new HeaderAction()
            {
                Text = "",
                Tooltip = GetString("general.edit"),
                ImageUrl = GetImageUrl("Objects/CMS_CustomSettings/edit.png"),
                CommandName = "edit",
                CommandArgument = catIdStr,
                ControlType = HeaderActionTypeEnum.LinkButton
            });

            // Delete action
            actions.ActionsList.Add(new HeaderAction()
            {
                Text = "",
                Tooltip = GetString("general.delete"),
                ImageUrl = GetImageUrl("Objects/CMS_CustomSettings/delete.png"),
                OnClientClick = string.Format("return confirm({0})", ScriptHelper.GetString(GetString("Development.CustomSettings.GroupDeleteConfirmation"))),
                CommandName = "delete",
                CommandArgument = catIdStr,
                ControlType = HeaderActionTypeEnum.LinkButton
            });

            // Move up action
            actions.ActionsList.Add(new HeaderAction()
            {
                Text = "",
                Tooltip = GetString("general.moveup"),
                ImageUrl = GetImageUrl("Objects/CMS_CustomSettings/up.png"),
                CommandName = "moveup",
                CommandArgument = catIdStr,
                ControlType = HeaderActionTypeEnum.LinkButton
            });

            // Move down action
            actions.ActionsList.Add(new HeaderAction()
            {
                Text = "",
                Tooltip = GetString("general.movedown"),
                ImageUrl = GetImageUrl("Objects/CMS_CustomSettings/down.png"),
                CommandName = "movedown",
                CommandArgument = catIdStr,
                ControlType = HeaderActionTypeEnum.LinkButton
            });

            cpCategory.HeaderActions.ActionPerformed += new CommandEventHandler(CategoryActionPerformed);

            // Panel title for group
            cpCategory.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Category.CategoryDisplayName));

            // Filter out only records for this group
            gridElem.WhereClause = "KeyCategoryID = " + Category.CategoryID;

            // Setup "Add key" link
            lnkNewKey.Text = ResHelper.GetString("Development.CustomSettings.NewKey");
            lnkNewKey.Click += new EventHandler(CreateNewKey);
            imgNewKey.ImageUrl = GetImageUrl("Objects/CMS_CustomSettings/list.png");
        }

        // Apply site filter if required.
        if (!string.IsNullOrEmpty(gridElem.WhereClause))
        {
            gridElem.WhereClause += " AND ";
        }

        if (mSiteId > 0)
        {
            gridElem.WhereClause += string.Format("SiteID = {0}", mSiteId);
        }
        else
        {
            gridElem.WhereClause += "SiteID IS NULL";
        }
    }


    /// <summary>
    /// OnExternal databoud event handling (because of macro resolving).
    /// </summary>
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.ToLowerCSafe() == "keydisplayname")
        {
            return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(parameter, "")));
        }
        else if (sourceName.ToLowerCSafe() == "ishidden")
        {
            ImageButton ib = ((ImageButton)sender);
            ib.Visible = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["KeyIsHidden"], false);
            ib.Style["cursor"] = "default";
            ib.Attributes["onclick"] = "javascript:return false;";
        }

        return parameter;
    }
}