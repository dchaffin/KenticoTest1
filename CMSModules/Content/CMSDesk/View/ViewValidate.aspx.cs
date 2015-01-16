using System;
using System.Web;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.PortalEngine;

public partial class CMSModules_Content_CMSDesk_View_ViewValidate : CMSContentPage, ICallbackEventHandler
{
    #region "Variables"

    protected string viewpage = null;
    protected string framescroll = "auto";
    protected string validatepage = "../Validation/Default.aspx";
    protected string loadScript = "FocusFrame();";

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        DocumentManager.RedirectForNonExistingDocument = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureSplitModeCrossoverScript();
        viewpage = ResolveUrl("~/CMSPages/blank.htm");

        if (!RequestHelper.IsPostBack())
        {
            pnlTabs.SelectedTabIndex = ViewTabCode.FromEnum(UIContext.ViewTab);
        }

        pnlTabs.CssClass = "Dialog_Tabs LightTabs";

        tabPreview.HeaderText = ResHelper.GetString("general.view");

        if (QueryHelper.GetBoolean("hidevalidate", false))
        {
            tabValidate.Visible = false;
        }
        else
        {
            tabValidate.HeaderText = ResHelper.GetString("general.validate");
            validatepage = URLHelper.AppendQuery(validatepage, URLHelper.Url.Query);
        }

        viewpage = QueryHelper.GetString("viewpage", string.Empty);
        viewpage = HttpUtility.UrlDecode(viewpage);

        if (!String.IsNullOrEmpty(viewpage))
        {
            viewpage = URLHelper.AddParameterToUrl(viewpage, URLHelper.LanguageParameterName, QueryHelper.GetString(URLHelper.LanguageParameterName, null));
            viewpage = URLHelper.AddParameterToUrl(viewpage, URLHelper.LanguageParameterName + ObjectLifeTimeFunctions.OBJECT_LIFE_TIME_KEY, QueryHelper.GetString(URLHelper.LanguageParameterName + ObjectLifeTimeFunctions.OBJECT_LIFE_TIME_KEY, null));
            
            // Add device name if sent via query string
            String deviceName = QueryHelper.GetString(DeviceProfileInfoProvider.DEVICENAME_QUERY_PARAM, String.Empty);
            if (deviceName != String.Empty)
            {
                viewpage = URLHelper.AddParameterToUrl(viewpage, DeviceProfileInfoProvider.DEVICENAME_QUERY_PARAM, deviceName);
            }

            // Split mode enabled
            if (CMSContext.DisplaySplitMode && !DeviceProfileActive)
            {
                // Ensure identifiers for splitview toolbar
                viewpage = URLHelper.AddParameterToUrl(viewpage, "nodeid", NodeID.ToString());
                viewpage = URLHelper.AddParameterToUrl(viewpage, "culture", CultureCode);
                viewpage = URLHelper.AddParameterToUrl(viewpage, "mode", Mode);
                viewpage = GetSplitViewUrl(viewpage);
            }
            else
            {
                viewpage = ResolveUrl(viewpage);
            }
        }

        // Ensure device profile is loaded from cookies in CMSDesk
        viewpage = URLHelper.UpdateParameterInUrl(viewpage, DeviceProfileInfoProvider.DEVICES_QUERY_PARAM, "1");

        ucView.ViewPage = viewpage;

        const string script = @"
$j(""#{0}_head a"").bind(""click"",function(){{
    var elem = document.getElementById('pageview'); 
    elem.src = elem.src; 
    CMSViewValidate.ToogleRotationCheckbox(true); 
}}); 
$j(""#{1}_head a"").bind(""click"",function(){{
    CMSViewValidate.ToogleRotationCheckbox(false); 
}});{2}";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "RefreshPageViewFrame", String.Format(script, tabPreview.ClientID, tabValidate.ClientID, (pnlTabs.SelectedTabIndex != 0 ? "CMSViewValidate.ToogleRotationCheckbox(false);" : "")), true);

        pnlTabs.OnClientTabClick = "setTimeout(function () { if (CMSViewValidate.ResizeContentArea) { CMSViewValidate.ResizeContentArea(); } }, 1);" + Page.ClientScript.GetCallbackEventReference(this, "ui.index", "$j.noop", "null");

        chkRotate.Text = GetString("View.RotateDevicePreview");
        chkRotate.Checked = ValidationHelper.GetBoolean(CookieHelper.GetValue("CMSDeviceProfileRotate"), false);
        chkRotate.Visible = true;
        ucView.RotateDevice = chkRotate.Checked;
    }


    protected override void OnPreRender(EventArgs e)
    {
        DeviceProfileInfo dpi = CMSContext.CurrentDeviceProfile;
        if (!((dpi != null) && (dpi.ProfilePreviewWidth > 0) && (dpi.ProfilePreviewHeight > 0)))
        {
            chkRotate.Visible = false;
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "ICallbackEventHandler Members"

    public string GetCallbackResult()
    {
        return null;
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
        UIContext.ViewTab = ViewTabCode.ToEnum(ValidationHelper.GetInteger(eventArgument, 0));
    }

    #endregion
}