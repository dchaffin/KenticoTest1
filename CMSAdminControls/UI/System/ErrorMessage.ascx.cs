using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSAdminControls_UI_System_ErrorMessage : ErrorMessageControl
{
    #region "Properties"

    /// <summary>
    /// Error title.
    /// </summary>
    public override string ErrorTitle
    {
        get
        {
            return ptTitle.TitleText;
        }
        set
        {
            ptTitle.TitleText = value;
        }
    }


    /// <summary>
    /// Error message.
    /// </summary>
    public override string ErrorMessage
    {
        get
        {
            return lblMessage.Text;
        }
        set
        {
            lblMessage.Text = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ptTitle.TitleImage = GetImageUrl("Others/Messages/denied.png");
    }
}