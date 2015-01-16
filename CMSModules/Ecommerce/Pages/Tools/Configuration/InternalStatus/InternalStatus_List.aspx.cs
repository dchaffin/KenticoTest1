using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;

[Title("Objects/Ecommerce_InternalStatus/object.png", "InternalStatus_List.HeaderCaption", "internal_status_list")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_InternalStatus_InternalStatus_List : CMSInternalStatusesPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Init header actions
        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);

        // New item action
        actions.ActionsList.Add(new HeaderAction()
        {
            Text = GetString("InternalStatus_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("InternalStatus_Edit.aspx?siteId=" + SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_InternalStatus/add.png"),
            ControlType = HeaderActionTypeEnum.Hyperlink
        });

        // Show copy from global link when not configuring global statuses.
        if (ConfiguredSiteID != 0)
        {
            // Show "Copy from global" link only if there is at least one global status
            DataSet ds = InternalStatusInfoProvider.GetInternalStatuses("InternalStatusSiteID IS NULL", null, 1, "InternalStatusSiteID");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                actions.ActionsList.Add(new HeaderAction()
                {
                    Text = GetString("general.copyfromglobal"),
                    OnClientClick = "return ConfirmCopyFromGlobal();",
                    ImageUrl = GetImageUrl("Objects/Ecommerce_InternalStatus/fromglobal.png"),
                    CommandName = "copyFromGlobal",
                    ControlType = HeaderActionTypeEnum.LinkButton
                });

                // Register javascript to confirm generate 
                string script = "function ConfirmCopyFromGlobal() {return confirm(" + ScriptHelper.GetString(GetString("com.ConfirmInternalStatusFromGlobal")) + ");}";
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmCopyFromGlobal", ScriptHelper.GetScript(script));
            }
        }

        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.ZeroRowsText = GetString("general.nodatafound");

        // Configuring global records from specific site
        if ((ConfiguredSiteID == 0) && (SiteID != 0))
        {
            ShowInformation(GetString("com.UsingGlobalSettings"));
        }

        // Filter records by site
        gridElem.WhereCondition = "ISNULL(InternalStatusSiteID, 0) = " + ConfiguredSiteID;
    }

    #endregion


    #region "Event Handlers"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "copyfromglobal":
                CopyFromGlobal();
                gridElem.ReloadData();
                break;
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("InternalStatus_Edit.aspx?statusid=" + Convert.ToString(actionArgument) + "&siteId=" + SiteID);
        }
        else if (actionName == "delete")
        {
            CheckConfigurationModification();

            if (InternalStatusInfoProvider.CheckDependencies(ValidationHelper.GetInteger(actionArgument, 0)))
            {
                // Show error message
                ShowError(GetString("Ecommerce.DeleteDisabled"));

                return;
            }

            // delete InternalStatusInfo object from database
            InternalStatusInfoProvider.DeleteInternalStatusInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "internalstatusenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);
        }

        return parameter;
    }

    #endregion


    #region "Methods"

    /// <summary>
    ///  Copies site-specific status options from global status list.
    /// </summary>
    protected void CopyFromGlobal()
    {
        CheckConfigurationModification();
        InternalStatusInfoProvider.CopyFromGlobal(ConfiguredSiteID);
    }

    #endregion
}