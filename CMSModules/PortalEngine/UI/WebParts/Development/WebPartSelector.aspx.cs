using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPartSelector : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check whether user is global admin
        CurrentUserInfo currentUser = CMSContext.CurrentUser;
        if (!currentUser.UserSiteManagerAdmin)
        {
            RedirectToAccessDenied(GetString("general.notauthorizedtopage"));
        }

        // Set dialog body class
        CurrentMaster.PanelBody.CssClass = "DialogPageBody";

        selectElem.ShowInheritedWebparts = false;
        selectElem.ShowWidgetOnlyWebparts = true;

        // Proceeds the current item selection
        string javascript = @"
function SelectCurrentWebPart() 
{
    SelectWebPart(selectedValue) ;
}
function SelectWebPart(value)
{
    if( value != null )
    {                    
        if ( wopener.OnSelectWebPart )
        {
            wopener.OnSelectWebPart(value);
        }	            
        CloseDialog();
	}
	else
	{
        alert(document.getElementById('" + hdnMessage.ClientID + @"').value);		    
	}                
}            
// Cancel action
function Cancel()
{
    CloseDialog();
} ";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "WebPartSelector", ScriptHelper.GetScript(javascript));

        // Set name of selection function
        selectElem.SelectFunction = "SelectWebPart";

        // Set the title and icon
        string title = GetString("portalengine-webpartselection.title");
        Page.Title = title;
        CurrentMaster.Title.TitleText = title;
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_WebPart/object.png");

        // Remove default css class
        if (CurrentMaster.PanelBody != null)
        {
            Panel pnl = CurrentMaster.PanelBody.FindControl("pnlContent") as Panel;
            if (pnl != null)
            {
                pnl.CssClass = String.Empty;
            }
        }
    }
}