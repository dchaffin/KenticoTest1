using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "USAePayConfiguration")]
public partial class CMSModules_USAePay_CMSPages_Configuration : CMSToolsPage
{
    private CurrentUserInfo currentUser = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        currentUser = CMSContext.CurrentUser;

        if (currentUser == null)
        {
            return;
        }

        if (!currentUser.IsAuthorizedPerResource("omni.usaepay", "read"))
        {
            RedirectToCMSDeskAccessDenied("omni.usaepay", "Read");
        }
        
        CurrentMaster.Title.TitleText = GetString("omni.usaepay.TitleText");
        CurrentMaster.Title.TitleImage = GetImageUrl("~/CMSModules/USAePay/USAePay.png");

        InitializeControls();
    }

    private void InitializeControls()
    {
     
    }
}