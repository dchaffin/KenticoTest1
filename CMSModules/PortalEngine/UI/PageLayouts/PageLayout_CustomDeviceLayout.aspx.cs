using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.FormControls;

public partial class CMSModules_PortalEngine_UI_PageLayouts_PageLayout_CustomDeviceLayout : CMSModalPage
{
    #region "Variables"

    private int templateId = 0;
    private int deviceProfileId = 0;
    private bool showNewDeviceSelector = true;

    #endregion


    #region "Page methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the page variables
        templateId = QueryHelper.GetInteger("templateid", 0);
        deviceProfileId = QueryHelper.GetInteger("deviceprofileid", 0);

        if (deviceProfileId > 0)
        {
            // Display device profile label
            showNewDeviceSelector = false;

            DeviceProfileInfo deviceLayout = DeviceProfileInfoProvider.GetDeviceProfileInfo(deviceProfileId);
            if (deviceLayout != null)
            {
                lblNewDevice.Text = GetString("devicelayout.createdevicelayout") + ": <strong>" + HTMLHelper.HTMLEncode(deviceLayout.ProfileDisplayName) + "</strong>";
            }
        }
        else
        {
            // Display device profile selector
            pnlNewDevice.Visible = true;
            lblNewDevice.Visible = false;
        }

        // Setup the page title
        CurrentMaster.Title.TitleText = GetString("devicelayout.new");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_DeviceProfile/new.png");
        CurrentMaster.Title.HelpTopicName = "CMS_Design_page_device_layout";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Source device profile selector
        ucDeviceProfile.WhereCondition = "ProfileID IN (SELECT ProfileID FROM CMS_TemplateDeviceLayout WHERE PageTemplateID = " + templateId + ")";
        ucDeviceProfile.SpecialFields = new string[,] { { GetString("devicelayout.defaultdeviceprofile"), "0" } };

        // New device profile selector
        // Do not display profiles which have layout already defined
        ucNewDeviceProfile.WhereCondition = "(ProfileEnabled = 1) AND (ProfileID NOT IN (SELECT ProfileID FROM CMS_TemplateDeviceLayout WHERE (PageTemplateID = " + templateId + ")))";
        ucNewDeviceProfile.OrderBy = "ProfileOrder";
        ucNewDeviceProfile.SpecialFields = new string[,] { { GetString("devicelayout.selectdeviceprofile"), "" } };
        ucNewDeviceProfile.OnBeforeClientChanged = "EnableSelection(this.value.length > 0);";

        // Javascript
        rbtnDevice.Attributes.Add("onclick", "EnableSelection(true)");
        rbtnLayout.Attributes.Add("onclick", "EnableSelection(true)");
        rbtnEmptyLayout.Attributes.Add("onclick", "EnableSelection(true)");

        if (!RequestHelper.IsPostBack())
        {
            rbtnDevice.Checked = true;
        }

        // Register the page javascript
        string script = @"

function EnableSelection(enabled) {
    var pnlContent = jQuery('#" + pnlContent.ClientID + @" :input');
    if (enabled) {
        pnlContent.removeAttr('disabled');
        
        var rbtnDevice = jQuery('#" + rbtnDevice.ClientID + @"');
        var rbtnLayout = jQuery('#" + rbtnLayout.ClientID + @"');
        var pnlLayout = jQuery('#" + pnlLayout.ClientID + @" :input');
        var pnlDevice = jQuery('#" + pnlDevice.ClientID + @" :input');
        if (rbtnDevice.is(':checked')) {
            pnlLayout.attr('disabled', 'disabled');
        }
        else if (rbtnLayout.is(':checked')) {
            pnlDevice.attr('disabled', 'disabled');
        }
        else {
            pnlLayout.attr('disabled', 'disabled');
            pnlDevice.attr('disabled', 'disabled');
        }
    }
    else {
        pnlContent.attr('disabled', 'disabled');
    }

    jQuery('#" + pnlNewDevice.ClientID + @" :input').removeAttr('disabled');
}

jQuery(document).ready(function () {
    EnableSelection(" + (showNewDeviceSelector ? "false" : "true") + @");
});";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "layoutScript", script, true);
        ScriptHelper.RegisterJQuery(Page);

        // Empty footer container
        if (CurrentMaster.PanelFooter != null)
        {
            CurrentMaster.PanelFooter.Controls.Clear();
        }

        // Add the Cancel button to the footer panel
        LocalizedButton btnOk = new LocalizedButton
        {
            ID = "btnOk",
            ResourceString = "general.ok",
            CssClass = "SubmitButton",
        };
        btnOk.Click += new EventHandler(SaveAction);

        // Add the Cancel button to the footer panel
        LocalizedButton btnCancel = new LocalizedButton
        {
            ID = "btnCancel",
            ResourceString = "general.cancel",
            CssClass = "SubmitButton",
            OnClientClick = "CloseDialog(); return false;"
        };

        // Button container panel
        Panel pnlRight = new Panel
        {
            ID = "pnlRight",
            CssClass = "TextRight"
        };
        pnlRight.Controls.Add(btnOk);
        pnlRight.Controls.Add(btnCancel);

        // Add the Ok, Cancel buttons to the footer controls collection
        if (CurrentMaster.PanelFooter != null)
        {
            CurrentMaster.PanelFooter.Controls.Add(pnlRight);
        }
    }


    /// <summary>
    /// Save button is clicked.
    /// </summary>
    protected void SaveAction(object sender, EventArgs e)
    {
        // Check permissions
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "Design.EditLayout"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Design.EditLayout");
        }

        // New device profile
        int newDeviceProfileId = deviceProfileId;
        if (newDeviceProfileId == 0)
        {
            newDeviceProfileId = ValidationHelper.GetInteger(ucNewDeviceProfile.Value, 0);
        }

        if (newDeviceProfileId == 0)
        {
            // Show error - select device profile first
            ShowError(GetString("devicelayout.selectdeviceprofile.error"));
            rbtnDevice.Checked = true;
            return;
        }

        PageTemplateInfo pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);
        if ((pti != null) && (newDeviceProfileId > 0))
        {
            string layoutCode = null;
            string layoutCSS = null;
            int layoutId = 0;
            LayoutTypeEnum layoutType = LayoutTypeEnum.Ascx;

            if (rbtnDevice.Checked)
            {
                // Copy from device
                int selectedDeviceProfileId = ValidationHelper.GetInteger(ucDeviceProfile.Value, 0);
                if (selectedDeviceProfileId > 0)
                {
                    // Existing device profile
                    PageTemplateDeviceLayoutInfo selectedDeviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(templateId, selectedDeviceProfileId);
                    if (selectedDeviceLayout != null)
                    {
                        layoutId = selectedDeviceLayout.LayoutID;
                        layoutCode = selectedDeviceLayout.LayoutCode;
                        layoutCSS = selectedDeviceLayout.LayoutCSS;
                        layoutType = selectedDeviceLayout.LayoutType;
                    }
                }
                else
                {
                    // Default device
                    layoutType = pti.PageTemplateLayoutType;

                    if (pti.LayoutID > 0)
                    {
                        layoutId = pti.LayoutID;
                    }
                    else
                    {
                        layoutCode = pti.PageTemplateLayout;
                        layoutCSS = pti.PageTemplateCSS;
                    }
                }
            }
            else if (rbtnLayout.Checked)
            {
                // Use existing layout
                int selectedLayoutId = ValidationHelper.GetInteger(ucLayout.Value, 0);
                LayoutInfo selectedLayout = LayoutInfoProvider.GetLayoutInfo(selectedLayoutId);
                if (selectedLayout != null)
                {
                    layoutType = selectedLayout.LayoutType;

                    if (chkCopy.Checked)
                    {
                        // Copy layout code
                        layoutCode = selectedLayout.LayoutCode;
                        layoutCSS = selectedLayout.LayoutCSS;
                    }
                    else
                    {
                        // Copy layout id
                        layoutId = selectedLayoutId;
                    }
                }
            }
            else if (rbtnEmptyLayout.Checked)
            {
                layoutCode = "<cms:CMSWebPartZone ZoneID=\"zoneA\" runat=\"server\" />";
            }

            PageTemplateDeviceLayoutInfo deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(templateId, newDeviceProfileId);

            if (deviceLayout == null)
            {
                // Create a new device profile layout object
                deviceLayout = new PageTemplateDeviceLayoutInfo();
                deviceLayout.PageTemplateID = templateId;
                deviceLayout.ProfileID = newDeviceProfileId;
            }

            // Modify the device profile layout object with updated values
            deviceLayout.LayoutID = layoutId;
            deviceLayout.LayoutType = layoutType;
            deviceLayout.LayoutCode = layoutCode;
            deviceLayout.LayoutCSS = layoutCSS;

            // Save the device profile layout object
            PageTemplateDeviceLayoutInfoProvider.SetTemplateDeviceLayoutInfo(deviceLayout);
            CMSContext.EditedObject = deviceLayout;
            CMSObjectManager.CheckOutNewObject(Page);

            // Register refresh page scripts
            ScriptHelper.RegisterStartupScript(this, typeof(string), "deviceLayoutSaved", "if (wopener.GetEditDeviceUrl) { wopener.refreshPageLocationUrl = wopener.GetEditDeviceUrl(" + deviceLayout.ProfileID + "); } else { window.refreshPageOnClose = true; } CloseDialog();", true);
        }
    }

    #endregion;

}