using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSSiteManager_SplashScreen : CMSDeskPage
{
    private string mContinueURL = null;

    /// <summary>
    /// Gets continue URL
    /// </summary>
    public string ContinueURL
    {
        get
        {
            if (mContinueURL == null)
            {
                mContinueURL = QueryHelper.GetString("continueurl", string.Empty);
            }

            return mContinueURL;
        }
        set
        {
            mContinueURL = value;
        }
    }


    /// <summary>
    /// Indicates if splashscreen is for Site Manager
    /// </summary>
    public bool IsSiteManager
    {
        get
        {
            return ContinueURL.ToLowerCSafe().Contains("cmssitemanager");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initializes the controls
        InitializeControls();

        // Ensure different class for Site Manager
        if (IsSiteManager)
        {
            pnlMain.CssClass += " SM";
        }
    }


    private void InitializeControls()
    {
        string version = CMSContext.SYSTEM_VERSION.Replace(".", "_");
        ltlTitle.Text = GetString("splashscreen.title");
        imgTitle.ImageUrl = GetImageUrl("Others/SplashScreen/title.png");

        chkDontShowAgain.Text = GetString("splashscreen.donotshowagain");
        btnContinue.Text = GetString("general.continue") + " >";
        lnkKenticoCom.Text = "<a href=\"http://www.kentico.com\" target=\"_blank\">www.kentico.com</a>";

        // Licensing section initialization
        imgLicensing.ImageUrl = GetImageUrl("Others/SplashScreen/licensing.png");

        string linkBuy = "<a href=\"http://www.kentico.com/Purchase\" target=\"_blank\">www.kentico.com/Purchase</a>";
        string linkEnterLicense = "<a href=\"" + ResolveUrl("~/CMSModules/Licenses/Pages/License_New.aspx") + "\" target=\"_blank\">" + GetString("splashscreen.here") + "</a>";

        // If license for the current domain exist and is valid
        LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(URLHelper.GetCurrentDomain());
        if (lki != null)
        {
            TimeSpan dt = lki.ExpirationDate - CMSContext.CurrentUser.DateTimeNow;

            if (dt.Days > 0)
            {
                ltlLicensingLn1.Text = string.Format(GetString("splashscreen.licensing.text.trial"), dt.Days.ToString());
            }
            else
            {
                ltlLicensingLn1.Text = GetString("splashscreen.licensing.text.trialexpired");
            }
            ltlLicensingLn2.Text = string.Format(GetString("splashscreen.licensing.textLn2"), linkBuy);
            ltlLicensingLn3.Text = string.Format(GetString("splashscreen.licensing.textLn3"), linkEnterLicense);
        }

        // Support and help initialization
        string documentation = "<a href=\"http://devnet.kentico.com/Documentation/" + version + ".aspx\" target=\"_blank\">http://devnet.kentico.com/Documentation/" + version + ".aspx</a>";
        imgDocumentation.ImageUrl = GetImageUrl("Others/SplashScreen/documentation.png");
        ltlDocumentation.Text = string.Format(GetString("splashscreen.documentation.text"), documentation);

        imgOnlineHelp.ImageUrl = GetImageUrl("Others/SplashScreen/help.png");
        ltlOnlineHelp.Text = GetString("splashscreen.onlinehelp.text");

        imgSupport.ImageUrl = GetImageUrl("Others/SplashScreen/support.png");
        ltlSupport.Text = string.Format(GetString("splashscreen.support.text"), "<a href=\"http://www.kentico.com/Support\" target=\"_blank\">www.kentico.com/Support</a>");

        imgDevNet.ImageUrl = GetImageUrl("Others/SplashScreen/devnet.png");
        ltlDevNet.Text = string.Format(GetString("splashscreen.devnet.text"), "<a href=\"http://devnet.kentico.com\" target=\"_blank\">devnet.kentico.com</a>");

        if (CurrentUser.IsGlobalAdministrator)
        {
            try
            {
                if (File.Exists(Server.MapPath("~/CMSAPIExamples/Default.aspx")))
                {
                    plcApiExample.Visible = true;
                    imgApiExamples.ImageUrl = GetImageUrl("General/Code.png");
                    ltlApiExamples.Text = string.Format(GetString("splashscreen.apiexamples.text"), ResolveUrl("~/CMSAPIExamples/Default.aspx"));
                }
            }
            catch
            {
            }
        }

        // Gettingstarted initialization
        string quickstartguide = "<a href=\"http://devnet.kentico.com/docs/" + version + "/kenticocms_quickguide.pdf\" target=\"_blank\">" + GetString("splashscreen.gettingstarted.quickstartguide") + "</a>";
        string tutorial = "<a href=\"http://devnet.kentico.com/docs/" + version + "/KenticoCMS_Tutorial.pdf\" target=\"_blank\">" + GetString("splashscreen.gettingstarted.tutorial") + "</a>";
        string tutorialASPX = string.Format(GetString("splashscreen.gettingstarted.tutorialaspx"), "<a href=\"http://devnet.kentico.com/docs/" + version + "/KenticoCMS_Tutorial_aspx.pdf\" target=\"_blank\">" + GetString("splashscreen.here") + "</a>");

        imgGettingStartedQuick.ImageUrl = GetImageUrl("Others/SplashScreen/quickstart.png");
        ltlGettingStartedQuick.Text = quickstartguide;

        imgGettingStartedTutorial.ImageUrl = GetImageUrl("Others/SplashScreen/stepbystep.png");
        ltlGettingStartedTutorial.Text = tutorial + " " + tutorialASPX;
    }


    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ContinueURL))
        {
            ContinueURL = "~/default.aspx";
        }

        if (chkDontShowAgain.Checked)
        {
            // Update user
            UserInfo ui = UserInfoProvider.GetUserInfo(CMSContext.CurrentUser.UserID);
            ui.UserSettings.UserShowSplashScreen = false;
            UserInfoProvider.SetUserInfo(ui);
        }

        URLHelper.Redirect(HTMLHelper.HTMLEncode(ResolveUrl(ContinueURL)));
    }
}