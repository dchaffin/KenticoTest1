using System;

using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

[CheckLicence(FeatureEnum.WorkflowVersioning)]
[Security(Resource = "CMS.MyDesk", UIElements = "CheckedOutDocs;CheckedOutDocuments")]
public partial class CMSModules_MyDesk_CheckedOut_Documents : CMSDocumentsPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ucCheckedOut.SiteName = CMSContext.CurrentSite.SiteName;
    }
}