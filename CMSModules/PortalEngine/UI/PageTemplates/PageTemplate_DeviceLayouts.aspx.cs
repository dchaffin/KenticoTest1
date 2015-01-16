using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_DeviceLayouts : CMSEditTemplatePage
{
    #region "Variables"

    private HeaderAction createDeviceLayout = null;
    private bool isAuthorised = false;

    protected string aliasPath = QueryHelper.GetString("aliaspath", string.Empty);

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions
        isAuthorised = (CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Design", "Design"));

        // Setup device layouts grid
        deviceLayoutsGrid.WhereCondition = "ProfileID IN (SELECT ProfileID FROM CMS_TemplateDeviceLayout WHERE PageTemplateID = " + PageTemplateID + ")";
        deviceLayoutsGrid.OnAction += new OnActionEventHandler(deviceLayoutsGrid_OnAction);

        // "Create device layout" button
        string dialogUrl = URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageLayouts/PageLayout_CustomDeviceLayout.aspx?dialog=1&templateid=" + PageTemplateID);

        if (isAuthorised)
        {
            // Create header button
            createDeviceLayout = new HeaderAction()
            {
                ControlType = HeaderActionTypeEnum.LinkButton,
                Text = GetString("devicelayout.create"),
                Tooltip = GetString("devicelayout.create.tooltip"),
                OnClientClick = "modalDialog('" + dialogUrl + "', 'createCustomDeviceLayout', 500, 480); return false;",
                ImageUrl = GetImageUrl("Objects/CMS_DeviceProfile/add.png"),
                SmallImageUrl = GetImageUrl("Objects/CMS_DeviceProfile/add.png")
            };

            AddHeaderAction(createDeviceLayout);
        }

        string script = @"
function GetEditDeviceUrl(deviceProfileId) {
    return '" + URLHelper.ResolveUrl("~/CMSModules/PortalEngine/UI/PageTemplates/DeviceLayout_Frameset.aspx") + "?deviceprofileid=' + deviceProfileId + '&templateid=" + PageTemplateID + "&aliaspath=" + aliasPath + @"';
}";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "editScript", script, true);
    }


    protected void deviceLayoutsGrid_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                int deviceProfileId = ValidationHelper.GetInteger(actionArgument, 0);
                if (deviceProfileId > 0)
                {
                    // Check permissions
                    if (isAuthorised)
                    {
                        // Delete layout
                        PageTemplateDeviceLayoutInfo deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(PageTemplateID, deviceProfileId);
                        PageTemplateDeviceLayoutInfoProvider.DeleteTemplateDeviceLayoutInfo(deviceLayout);
                    }
                    else
                    {
                        ShowError(GetString("general.actiondenied"));
                    }
                }
                break;
        }
    }

    #endregion
}

