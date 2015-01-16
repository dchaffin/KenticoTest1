using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

public partial class CMSModules_Avatars_Dialogs_AvatarsGallery : CMSModalPage
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