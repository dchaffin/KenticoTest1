using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.WebServices;
using CMS.UIControls;
using CMS.SettingsProvider;

public partial class CMSModules_REST_FormControls_GenerateHash : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("rest.generateauthhash");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_REST/GenerateAuthHash.png");

        btnCancel.Text = GetString("general.close");
        btnAuthenticate.Text = GetString("rest.authenticate");
        btnAuthenticate.Click += new EventHandler(btnAuthenticate_Click);
    }


    protected void btnAuthenticate_Click(object sender, EventArgs e)
    {
        StringBuilder result = new StringBuilder();
        string[] urls = txtUrls.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string domain = "";
        txtUrls.Text = "";

        foreach (string url in urls)
        {
            string urlWithoutHash = URLHelper.RemoveParameterFromUrl(url, "hash");
            string newUrl = HttpUtility.UrlDecode(urlWithoutHash);
            string query = URLHelper.GetQuery(newUrl).TrimStart('?');

            int index = newUrl.IndexOfCSafe("/rest");
            if (index >= 0)
            {
                // Extract the domain
                domain = URLHelper.GetDomain(newUrl);

                // Separate the query
                newUrl = URLHelper.RemoveQuery(newUrl.Substring(index));

                // Rewrite the URL to physical URL
                string[] rewritten = RESTService.RewriteRESTUrl(newUrl, query, domain);
                newUrl = rewritten[0].TrimStart('~') + "?" + rewritten[1];

                // Get the hash from real URL
                txtUrls.Text += URLHelper.AddParameterToUrl(urlWithoutHash, "hash", RESTService.GetHashForURL(newUrl, domain)) + Environment.NewLine;
            }
            else
            {
                txtUrls.Text += url + Environment.NewLine;
            }
        }
    }
}