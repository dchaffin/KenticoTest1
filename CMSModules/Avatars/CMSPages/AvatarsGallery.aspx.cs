using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSModules_Avatars_CMSPages_AvatarsGallery : CMSLiveModalPage
{
    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("avat.selectavatar");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Avatar/object.png");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        btnOk.Enabled = avatarsGallery.HasData();
    }

    #endregion
}