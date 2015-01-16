using System;
using System.Collections.Generic;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("CMSModules/CMS_Support/Module.png", "Header.Support", "")]
public partial class CMSModules_Support_Pages_support : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        uiGuide.OnGuideItemCreated += uiGuide_OnGuideItemCreated;
    }


    protected List<string> uiGuide_OnGuideItemCreated(UIElementInfo uiElement, List<string> defaultItem)
    {
        switch (uiElement.ElementName.ToLowerCSafe())
        {
            case "support.kenticodevnet":
                // Icon correction
                defaultItem[0] = UIHelper.ResolveImageUrl(GetImagePath("CMSModules/CMS_Support/DevnetLarge.png"));
                break;

            case "support.submitissue":
                // Icon correction
                defaultItem[0] = UIHelper.ResolveImageUrl(GetImagePath("CMSModules/CMS_Support/SubmitIssueLarge.png"));
                break;

            case "support.documentation":
                // Icon correction
                defaultItem[0] = UIHelper.ResolveImageUrl(GetImagePath("General/HelpLarge.png"));

                // URL correction
                defaultItem[2] = "http://devnet.kentico.com/Documentation/" + CMSContext.SYSTEM_VERSION.Replace(".", "_") + ".aspx";
                break;

            case "support.apiexamples":
                // Checks if APIExamples file exists
                if (!File.Exists(Server.MapPath("~/CMSAPIExamples/Default.aspx")))
                {
                    return null;
                }

                // Add target frame
                defaultItem.Add("true");

                // Icon correction
                defaultItem[0] = UIHelper.ResolveImageUrl(GetImagePath("CMSModules/CMS_Support/CodeLarge.png"));
                break;
        }

        return defaultItem;
    }
}