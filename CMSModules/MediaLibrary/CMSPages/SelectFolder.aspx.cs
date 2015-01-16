using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSModules_MediaLibrary_CMSPages_SelectFolder : CMSLiveModalPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        SetBrowserClass();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
    }
}