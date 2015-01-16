using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSDesk_Administration_Administration : CMSAdministrationPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("Header.Administration");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Administration/module.png");
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
        if (!IsAdministrationUIElementAvailable(uiElement))
        {
            return null;
        }
        return defaultItem;
    }
}