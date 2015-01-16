using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_Dialogs_General_WidthHeightSelector : CMSUserControl
{
    #region "Variables"

    private bool mShowLabels = true;
    private bool mShowActions = true;
    private bool mVerticalLayout = true;
    private string mLockImg = null;
    private string mUnLockImg = null;
    private string mRefreshImg = null;
    private string mCustomRefreshCode = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the CSS Class of the Width & Height TextBoxes.
    /// </summary>
    public string TextBoxesClass
    {
        get
        {
            return txtWidth.CssClass;
        }
        set
        {
            txtHeight.CssClass = value;
            txtWidth.CssClass = value;
        }
    }


    /// <summary>
    /// Returns TextBox object for width.
    /// </summary>
    public TextBox WidthTextBox
    {
        get
        {
            return txtWidth;
        }
    }


    /// <summary>
    /// Returns TextBox object for height.
    /// </summary>
    public TextBox HeightTextBox
    {
        get
        {
            return txtHeight;
        }
    }


    /// <summary>
    /// Gets or sets custom refresh button code.
    /// </summary>
    public string CustomRefreshCode
    {
        get
        {
            return mCustomRefreshCode;
        }
        set
        {
            mCustomRefreshCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether the aspect ratio is locked.
    /// </summary>
    public bool Locked
    {
        get
        {
            return ValidationHelper.GetBoolean(hdnLocked.Value, true);
        }
        set
        {
            hdnLocked.Value = value.ToString();
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "lock" + ClientID, ScriptHelper.GetScript("var lock" + ClientID + " = " + (value ? "true" : "false") + ";"));
            imgLock.ImageUrl = (value ? mLockImg : mUnLockImg);
        }
    }


    /// <summary>
    /// Indicates if labes should be displayed.
    /// </summary>
    public bool ShowLabels
    {
        get
        {
            return mShowLabels;
        }
        set
        {
            mShowLabels = value;
        }
    }


    /// <summary>
    /// Indicates if actions should be visible.
    /// </summary>
    public bool ShowActions
    {
        get
        {
            return mShowActions;
        }
        set
        {
            mShowActions = value;
        }
    }


    /// <summary>
    /// Indicates if vertical layout should be used.
    /// </summary>
    public bool VerticalLayout
    {
        get
        {
            return mVerticalLayout;
        }
        set
        {
            mVerticalLayout = value;
        }
    }


    /// <summary>
    /// Object width.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(txtWidth.Text, 0);
        }
        set
        {
            hdnWidth.Value = value.ToString();
            txtWidth.Text = value.ToString();
        }
    }


    /// <summary>
    /// Object height.
    /// </summary>
    public int Height
    {
        get
        {
            return ValidationHelper.GetInteger(txtHeight.Text, 0);
        }
        set
        {
            hdnHeight.Value = value.ToString();
            txtHeight.Text = value.ToString();
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        mLockImg = GetImageUrl("Design/Controls/Dialogs/lock.png");
        mUnLockImg = GetImageUrl("Design/Controls/Dialogs/unlock.png");
        mRefreshImg = GetImageUrl("Design/Controls/UniGrid/Actions/undo.png");
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        lblWidth.Visible = ShowLabels;
        lblHeight.Visible = ShowLabels;

        imgLock.Visible = mShowActions;
        imgRefresh.Visible = mShowActions;

        imgLock.ImageUrl = (Locked ? mLockImg : mUnLockImg);
        imgRefresh.ImageUrl = mRefreshImg;

        imgLock.ToolTip = GetString("dialogs.lockratio");
        imgRefresh.ToolTip = GetString("dialogs.refreshsize");

        if (String.IsNullOrEmpty(CustomRefreshCode))
        {
            imgRefresh.Attributes.Add("onClick", GetRefreshJS());
        }
        else
        {
            imgRefresh.OnClientClick = CustomRefreshCode;
        }
        imgLock.Attributes.Add("onClick", GetLockJS());

        txtWidth.Attributes["onKeyUp"] = GetWidthKeyUpS();
        txtHeight.Attributes["onKeyUp"] = GetHeightKeyUpS();
        txtWidth.Attributes["onKeyDown"] = "return OnlyNumbers(event, true);";
        txtHeight.Attributes["onKeyDown"] = "return OnlyNumbers(event, true);";

        hdnWidth.Value = Width.ToString();
        hdnHeight.Value = Height.ToString();

        if (VerticalLayout)
        {
            lblWidth.Attributes.Add("style", "width:80px;");
            lblHeight.Attributes.Add("style", "width:80px;");
            txtWidth.Attributes.Add("style", "margin-bottom:5px;");
            ltlBreak.Text = "<br />";
        }
        else
        {
            lblWidth.Attributes.Add("style", "width:80px;");
            if (CultureHelper.IsUICultureRTL())
            {
                lblHeight.Attributes.Add("style", "width:80px;padding-right:20px;");
            }
            else
            {
                lblHeight.Attributes.Add("style", "width:80px;padding-left:20px;");
            }
        }

        InitializeControlScripts();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes and registers all the necessary JavaScript.
    /// </summary>
    private void InitializeControlScripts()
    {
        string locked = "var lock" + ClientID + " = " + (Locked ? "true" : "false") + ";";

        ScriptHelper.RegisterOnlyNumbersScript(Page);
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "lock" + ClientID, ScriptHelper.GetScript(locked));
    }


    /// <summary>
    /// Returns regresh Javascript function.
    /// </summary>
    private string GetRefreshJS()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("var txtWidth = document.getElementById('" + txtWidth.ClientID + "');");
        builder.Append("var txtHeight = document.getElementById('" + txtHeight.ClientID + "');");
        builder.Append("var hdnWidth = document.getElementById('" + hdnWidth.ClientID + "');");
        builder.Append("var hdnHeight = document.getElementById('" + hdnHeight.ClientID + "');");
        builder.Append("if ((txtWidth!= null)&&(hdnWidth != null)) {");
        builder.Append("txtWidth.value = hdnWidth.value;");
        builder.Append("}");
        builder.Append("if ((txtHeight!= null)&&(hdnHeight != null)) {");
        builder.Append("txtHeight.value = hdnHeight.value;");
        builder.Append("} return false;");

        return builder.ToString();
    }


    /// <summary>
    /// Returns width textbox KeuUP Javascript function.
    /// </summary>
    private string GetWidthKeyUpS()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("var txtWidth = document.getElementById('" + txtWidth.ClientID + "');");
        builder.Append("var txtHeight = document.getElementById('" + txtHeight.ClientID + "');");
        builder.Append("var hdnWidth = document.getElementById('" + hdnWidth.ClientID + "');");
        builder.Append("var hdnHeight = document.getElementById('" + hdnHeight.ClientID + "');");
        builder.Append("if (lock" + ClientID + ") {");
        builder.Append("if ((txtWidth!= null)&&(hdnWidth != null)&&(txtHeight!= null)&&(hdnHeight != null)) {");
        builder.Append("var val = parseInt(parseInt(txtWidth.value,10)*(parseInt(hdnHeight.value,10)/parseInt(hdnWidth.value,10)),10);");
        builder.Append("if (isNaN(val)) {txtHeight.value = 0; } else {txtHeight.value = val;}");
        builder.Append("}}");

        return builder.ToString();
    }


    /// <summary>
    /// Returns height textbox KeuUP Javascript function.
    /// </summary>
    private string GetHeightKeyUpS()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("var txtWidth = document.getElementById('" + txtWidth.ClientID + "');");
        builder.Append("var txtHeight = document.getElementById('" + txtHeight.ClientID + "');");
        builder.Append("var hdnWidth = document.getElementById('" + hdnWidth.ClientID + "');");
        builder.Append("var hdnHeight = document.getElementById('" + hdnHeight.ClientID + "');");
        builder.Append("if (lock" + ClientID + ") {");
        builder.Append("if ((txtWidth!= null)&&(hdnWidth != null)&&(txtHeight!= null)&&(hdnHeight != null)) {");
        builder.Append("var val = parseInt(parseInt(txtHeight.value,10)*(parseInt(hdnWidth.value,10)/parseInt(hdnHeight.value,10)),10);");
        builder.Append("if (isNaN(val)) {txtWidth.value = 0; } else {txtWidth.value = val;}");
        builder.Append("}}");

        return builder.ToString();
    }


    /// <summary>
    /// Returns lock Javascript function.
    /// </summary>
    private string GetLockJS()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("var hdnLockElem = document.getElementById('" + hdnLocked.ClientID + "');");
        builder.Append("if (hdnLockElem != null) {");
        builder.Append("if (lock" + ClientID + " == true) {");
        builder.Append("lock" + ClientID + " = false;");
        builder.Append("hdnLockElem.value = false;");
        builder.Append("this.src = '" + ResolveUrl(mUnLockImg) + "'");
        builder.Append("} else {");
        builder.Append("var hdnWidth = document.getElementById('" + hdnWidth.ClientID + "');");
        builder.Append("var hdnHeight = document.getElementById('" + hdnHeight.ClientID + "');");
        builder.Append("var txtWidth = document.getElementById('" + txtWidth.ClientID + "');");
        builder.Append("var txtHeight = document.getElementById('" + txtHeight.ClientID + "');");
        builder.Append("if ((hdnWidth != null) && (hdnHeight != null) && (txtWidth != null) && (txtHeight != null)) {");
        builder.Append("hdnWidth.value = txtWidth.value;");
        builder.Append("hdnHeight.value = txtHeight.value;");
        builder.Append("lock" + ClientID + " = true;");
        builder.Append("hdnLockElem.value = true;");
        builder.Append("this.src = '" + ResolveUrl(mLockImg) + "'");
        builder.Append("}");
        builder.Append("}");
        builder.Append("} return false;");

        return builder.ToString();
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Validates the user input.
    /// </summary>
    public bool Validate()
    {
        return ((txtWidth.Text.Trim() == "") || ValidationHelper.IsInteger(txtWidth.Text.Trim())) &&
               ((txtHeight.Text.Trim() == "") || ValidationHelper.IsInteger(txtHeight.Text.Trim()));
    }

    #endregion
}