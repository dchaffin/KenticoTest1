using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

[Tabs("CMS.FileImport", "", "fileImportContent")]
public partial class CMSModules_FileImport_Tools_Header : CMSFileImportPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("Tools.FileImport.FileImport");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_FileImport/module.png");
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "CMS_FileImport_ImportFromComputer";
    }
}