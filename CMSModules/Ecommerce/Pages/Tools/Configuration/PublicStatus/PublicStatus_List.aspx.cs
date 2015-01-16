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

[Title("Objects/Ecommerce_PublicStatus/object.png", "PublicStatus_List.HeaderCaption", "public_status_list")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_PublicStatus_PublicStatus_List : CMSPublicStatusesPage
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
            Text = GetString("PublicStatus_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("PublicStatus_Edit.aspx?siteId=" + SiteID),
            ImageUrl = GetImageUrl("Objects/Ecommerce_PublicStatus/add.png"),
            ControlType = HeaderActionTypeEnum.Hyperlink
        });

        // Show copy from global link when not configuring global statuses.
        if (ConfiguredSiteID != 0)
        {
            // Show "Copy from global" link only if there is at least one global status
            DataSet ds = PublicStatusInfoProvider.GetPublicStatuses(0, false);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                actions.ActionsList.Add(new HeaderAction()
                {
                    Text = GetString("general.copyfromglobal"),
                    OnClientClick = "return ConfirmCopyFromGlobal();",
                    ImageUrl = GetImageUrl("Objects/Ecommerce_PublicStatus/fromglobal.png"),
                    CommandName = "copyFromGlobal",
                    ControlType = HeaderActionTypeEnum.LinkButton
                });

                // Register javascript to confirm generate 
                string script = "function ConfirmCopyFromGlobal() {return confirm(" + ScriptHelper.GetString(GetString("com.ConfirmPublicStatusFromGlobal")) + ");}";
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
        gridElem.WhereCondition = "ISNULL(PublicStatusSiteID, 0) = " + ConfiguredSiteID;
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
            URLHelper.Redirect("PublicStatus_Edit.aspx?publicStatusId=" + ValidationHelper.GetInteger(actionArgument, 0) + "&siteId=" + SiteID);
        }
        else if (actionName == "delete")
        {
            CheckConfigurationModification();

            if (PublicStatusInfoProvider.CheckDependencies(ValidationHelper.GetInteger(actionArgument, 0)))
            {
                // Show error message
                ShowError(GetString("Ecommerce.DeleteDisabled"));

                return;
            }

            // Delete PublicStatusInfo object from database
            PublicStatusInfoProvider.DeletePublicStatusInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "publicstatusenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "publicstatusname":
                return HTMLHelper.HTMLEncode(Convert.ToString(parameter));
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
        PublicStatusInfoProvider.CopyFromGlobal(ConfiguredSiteID);
    }

    #endregion
}