using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[EditedObject(SiteObjectType.STATE, "stateid")]
[ParentObject(SiteObjectType.COUNTRY, "countryid")]
// Breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "Country_State_Edit.ItemListLink", "List.aspx?countryid={%EditedObjectParent.ID%}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, "Country_State_Edit.NewItemCaption", NewObject = true)]
// Help
[Help("newedit_state")]
public partial class CMSModules_Countries_Pages_Development_State_Edit : SiteManagerPage
{
}