using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_MyDesk_MyDesk : CMSMyDeskPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize page
        CurrentMaster.Title.TitleText = GetString("Header.MyDesk");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_MyDesk/module.png");

        guide.OnGuideItemCreated += new CMSAdminControls_UI_UIProfiles_UIGuide.GuideItemCreatedEventHandler(guide_OnGuideItemCreated);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // No modules allowed
        if (guide.GuideEmpty)
        {
            RedirectToUINotAvailable();
        }
    }


    private List<string> guide_OnGuideItemCreated(UIElementInfo uiElement, List<string> defaultItem)
    {
        if (!IsMyDeskUIElementAvailable(uiElement.ElementName))
        {
            return null;
        }
        return defaultItem;
    }
}