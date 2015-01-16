using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.MessageBoard;
using CMS.UIControls;

public partial class CMSModules_MessageBoards_CMSPages_Unsubscribe : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = CurrentMaster.Title;
        title.TitleImage = GetImageUrl("Design/Controls/Subscriptions/unsubscription.png");
        title.TitleText = HTMLHelper.HTMLEncode(string.Format(GetString("board.unsubscription"), (unsubscription.SubscriptionSubject != null) ? ScriptHelper.GetString(unsubscription.SubscriptionSubject.BoardDisplayName) : null)); ;
    }
}