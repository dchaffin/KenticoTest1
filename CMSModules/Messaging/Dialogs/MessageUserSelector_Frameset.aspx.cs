using System;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Messaging_Dialogs_MessageUserSelector_Frameset : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.Messaging);
        // Set localized page title
        Page.Title = GetString("Messaging.MessageUserSelector.HeaderCaption");
    }
}