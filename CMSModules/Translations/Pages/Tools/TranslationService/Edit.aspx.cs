using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.TranslationServices;

// Edited object
[EditedObject(TranslationServicesObjectType.TRANSLATIONSERVICE, "serviceId")]

// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "translationservice.translationservice.list", "~/CMSModules/Translations/Pages/Tools/TranslationService/List.aspx", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "translationservice.translationservice.new", NewObject = true)]

// Title
[Title(ImageUrl = "Objects/CMS_TranslationService/object.png", ResourceString = "translationservice.translationservice.edit", HelpTopic = "translationservices_edit", ExistingObject = true)]
[Title(ImageUrl = "Objects/CMS_TranslationService/new.png", ResourceString = "translationservice.translationservice.new", HelpTopic = "translationservices_edit", NewObject = true)]

public partial class CMSModules_Translations_Pages_Tools_TranslationService_Edit : CMSTranslationsPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.ItemID = QueryHelper.GetInteger("serviceid", 0);
    }

    #endregion
}
