using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.PortalEngine;

public partial class CMSModules_PortalEngine_UI_WebParts_WebPartSelector : CMSModalDesignPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UI elements for web part zone
        CurrentUserInfo currentUser = CMSContext.CurrentUser;
        if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "Design", "Design.AddWebParts" }, CMSContext.CurrentSiteName))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Design;Design.AddWebParts");
        }

        // Set dialog body class
        CurrentMaster.PanelBody.CssClass = "DialogPageBody";
        selectElem.ShowInheritedWebparts = true;

        // Proceeds the current item selection
        string javascript =
@"
function SelectCurrentWebPart() 
{
    SelectWebPart(selectedValue, selectedSkipDialog);
}
function SelectWebPart(value, skipDialog)
{
    if ((value != null) && (value != ''))
    {
        if (wopener.OnSelectWebPart)
        {
            wopener.OnSelectWebPart(value, skipDialog);
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

        bool wireframe = QueryHelper.GetBoolean("wireframe", false);
        selectElem.ShowWireframeOnlyWebparts = wireframe || (PortalContext.ViewMode == ViewModeEnum.Wireframe);

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