using System;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSMessages_Redirect : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string url = QueryHelper.GetText("url", String.Empty);

        // Check for relative url or for hash
        if (url.StartsWithCSafe("~") || url.StartsWithCSafe("/") || QueryHelper.ValidateHash("hash"))
        {
            titleElem.TitleText = GetString("Redirect.Header");
            titleElem.TitleImage = GetImageUrl("Others/Messages/redirect.png");
            lblInfo.Text = GetString("Redirect.Info");

            string target = QueryHelper.GetText("target", String.Empty);
            string frame = QueryHelper.GetText("frame", String.Empty);
            bool urlIsRelative = url.StartsWithCSafe("~/");

            string script = String.Empty;
            url = ResolveUrl(url);

            // Information about the target page
            lnkTarget.Text = url;
            lnkTarget.NavigateUrl = url;
            lnkTarget.Target = target;

            string redirectUrlString = ScriptHelper.GetString(url);

            // Generate redirect script
            if (urlIsRelative && frame.EqualsCSafe("top", true))
            {
                script += "if (self.location != top.location) { top.location = " + redirectUrlString + "; } else { document.location = " + redirectUrlString + " }";
            }
            else if ((target == String.Empty) && (url != String.Empty))
            {
                script += "if (IsCMSDesk()) { window.open(" + redirectUrlString + "); } else { document.location = " + redirectUrlString + "; }";
            }

            ltlScript.Text += ScriptHelper.GetScript(script);
        }
        else
        {
            URLHelper.Redirect(ResolveUrl("~/CMSMessages/Error.aspx?title=" + ResHelper.GetString("general.badhashtitle") + "&text=" + ResHelper.GetString("general.badhashtext")));
        }
    }
}