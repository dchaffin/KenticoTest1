using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.TranslationServices;

// Actions
[Actions(1)]
[Action(0, "Objects/CMS_TranslationService/add.png", "translationservice.translationservice.new", "Edit.aspx")]

// Title
[Title("Objects/CMS_TranslationService/object.png", "translationservice.translationservice.list", "translationservices_translationservice_list")]

public partial class CMSModules_Translations_Pages_Tools_TranslationService_List : CMSTranslationsPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #endregion
}
