using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAdminControls_UI_System_RequireScript : CMSUserControl
{
    #region "Variables"

    private bool mUseFileStrings = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if the control should use the string from resource file.
    /// </summary>
    public bool UseFileStrings
    {
        get
        {
            return mUseFileStrings;
        }
        set
        {
            mUseFileStrings = value;
            ScriptTitle.UseFileStrings = value;
        }
    }

    #endregion


    #region "Control events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptTitle.TitleText = GetString("RequireScript.Title");
        lblInfo.Text = GetString("RequireScript.Information");
        ScriptTitle.TitleImage = GetImageUrl("Design/Controls/RequireScript/javascript.png");
        btnContinue.Text = GetString("RequireScript.Continue");
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns localized string.
    /// </summary>
    /// <param name="stringName">String to localize</param>
    public override string GetString(string stringName)
    {
        if (UseFileStrings)
        {
            return ResHelper.GetFileString(stringName);
        }
        else
        {
            return base.GetString(stringName);
        }
    }

    #endregion
}