using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

public partial class CMSFormControls_LiveSelectors_SpellCheck : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize modal page
        CurrentMaster.Title.TitleText = GetString("SpellCheck.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Content/EditMenu/spellcheck.png");
    }
}