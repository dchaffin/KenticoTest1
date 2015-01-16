using System;

using CMS.UIControls;
using CMS.LicenseProvider;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSModules_Messaging_CMSPages_MessageUserSelector_ContactList : LivePage
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

    protected void Page_Load(object sender, EventArgs e)
    {
        // Add styles
        RegisterDialogCSSLink();
    }
}