using System;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Messaging_Dialogs_MessageUserSelector_ContactList : CMSPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.Messaging))
        {
            contactListElem.StopProcessing = true;
            contactListElem.Visible = false;
        }
    }
}