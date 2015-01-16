using System;

using CMS.Controls.Configuration;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Content_CMSDesk_Validation_Header : CMSValidationPage
{
    #region "Variables"

    private string selected = null;
    private int selectedTabIndex = 0;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        DocumentManager.RedirectForNonExistingDocument = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        selected = DataHelper.GetNotEmpty(QueryHelper.GetString("tab", String.Empty).ToLowerCSafe(), ValidationTabCode.ToString(UIContext.ValidationTab).ToLowerCSafe());

        CurrentMaster.Tabs.OnTabCreated += tabElem_OnTabCreated;
        CurrentMaster.Tabs.ModuleName = "CMS.Content";
        CurrentMaster.Tabs.ElementName = "Validation";
        CurrentMaster.Tabs.UrlTarget = "validationedit";
        CurrentMaster.SetRTL();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        CurrentMaster.Tabs.SelectedTab = selectedTabIndex;
        CurrentMaster.Tabs.DoTabSelection();
    }

    #endregion


    #region "Control events"

    protected TabItem tabElem_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        string elementName = element.ElementName.ToLowerCSafe();

        if (elementName.StartsWithCSafe("validation.") && (elementName.Substring("validation.".Length) == selected))
        {
            selectedTabIndex = tabIndex;
        }

        if (tab.RedirectUrl != null)
        {
            tab.RedirectUrl = URLHelper.AppendQuery(URLHelper.RemoveQuery(tab.RedirectUrl), URLHelper.Url.Query);
        }

        return tab;
    }

    #endregion
}