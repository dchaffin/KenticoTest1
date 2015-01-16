using System;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;

public partial class CMSModules_ContactManagement_Pages_Tools_SalesForce_AuthorizationSetupPrologue : CMSSalesForceDialogPage
{

    public string AuthorizationSetupUrl
    {
        get
        {
            string baseUrl = URLHelper.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/SalesForce/AuthorizationSetup.aspx");
            UriBuilder builder = new UriBuilder(URLHelper.GetAbsoluteUrl(baseUrl))
            {
                Port = -1,
                Scheme = "https"
            };
            baseUrl = builder.Uri.AbsoluteUri;
            string url = URLHelper.AddParameterToUrl(baseUrl, "pid", Request.Params["pid"]);

            return URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url));
        }
    }

    public string AuthorizationSetupHandlerUrl
    {
        get
        {
            string baseUrl = URLHelper.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/SalesForce/AuthorizationSetupHandler.ashx");
            UriBuilder builder = new UriBuilder(URLHelper.GetAbsoluteUrl(baseUrl))
            {
                Port = -1,
                Scheme = "https"
            };

            return String.Format("{0}?callback=?", builder.Uri.AbsoluteUri);
        }
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ScriptHelper.RegisterJQuery(Page);
        CurrentMaster.Title.TitleText = GetString("sf.authorization.title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/OM_Account/object.png");
    }

}