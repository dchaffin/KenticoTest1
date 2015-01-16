using System;
using System.Web;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Pages_Tools_Header : CMSEcommercePage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        uiToolbarElem.OnButtonCreating += uiToolbarElem_OnButtonCreating;
        uiToolbarElem.OnButtonFiltered += uiToolbarElem_OnButtonFiltered;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        string scriptRedir = ScriptHelper.GetScript(@" 
            function redir(url, target) 
            { 
                if (url != '') 
                { 
                    if (target != '') 
                    { 
                        if ((target == '_blank')||(target == '_new')) 
                        { 
		                    window.open(url); 
                            return true; 
                        }
                        else 
                        { 
		                    try 
		                    { 
			                    if ( !parent.frames[target].CheckChanges() ) return false; 
		                    } 
		                    catch (ex) {} 
		                    parent.frames[target].location.href = url; return true; } 
                        } 
                    else 
                    {
                        parent.location.href = url; 
                        return true; 
                    } 
                }  
                return true; 
            } ");

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "RedirectScript", scriptRedir);

        string script = null;

        // Call the script for tab which is selected
        UIElementInfo selected = uiToolbarElem.SelectedUIElement;
        if (selected != null)
        {
            string url = selected.ElementTargetURL;
            url = URLHelper.EnsureHashToQueryParameters(url);

            // If target url contains javascript, execute this script
            if (url.StartsWithCSafe("javascript:", true))
            {
                script = url.Remove(0, 11); // 11 - length of "javascript:"
            }
            else
            {
                // Append query
                url = URLHelper.ResolveUrl(URLHelper.AppendQuery(url, URLHelper.Url.Query));
                script = "redir(" + ScriptHelper.GetString(url) + ",'" + uiToolbarElem.TargetFrameset + "'); ";
            }
        }
        // No tab is visible
        else
        {
            script = "redir('" + URLHelper.ResolveUrl(UIHelper.GetInformationUrl("uiprofile.uinotavailable")) + "','" + uiToolbarElem.TargetFrameset + "'); ";
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "TabSelection", ScriptHelper.GetScript(script));
        ScriptHelper.RegisterTitleScript(this, GetString("cmsdesk.ui.ecommerce"));
    }

    #endregion


    #region "Event handlers"

    private void uiToolbarElem_OnButtonCreating(object sender, UniMenuArgs eventArgs)
    {
        string elementName = eventArgs.UIElement.ElementName;
        if (elementName.CompareToCSafe("products", true) == 0)
        {
            if (ECommerceSettings.ProductsTree(CurrentSiteName) == ProductsTreeModeEnum.None)
            {
                eventArgs.UIElement.ElementTargetURL = "javascript: SelectItem('Products',  'Products/Product_List.aspx');";
            }
        }

        // Resolve hash in dashboard url
        if (elementName.CompareToCSafe("ecommercedashboard", true) == 0)
        {
            string url = eventArgs.UIElement.ElementTargetURL;
            url = URLHelper.EnsureHashToQueryParameters(url);
            eventArgs.UIElement.ElementTargetURL = url;
        }
    }


    private bool uiToolbarElem_OnButtonFiltered(object sender, UniMenuArgs eventArgs)
    {
        // Hide reports button when reporting module is not loaded
        if (eventArgs.UIElement.ElementName.CompareToCSafe("ecreports", true) == 0)
        {
            return ModuleEntry.IsModuleLoaded(ModuleEntry.REPORTING);
        }

        return true;
    }

    #endregion
}