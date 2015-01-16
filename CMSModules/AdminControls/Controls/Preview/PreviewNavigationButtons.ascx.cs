using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.CMSHelper;

public partial class CMSModules_AdminControls_Controls_Preview_PreviewNavigationButtons : CMSPreviewControl
{
    #region "Properties"

    /// <summary>
    /// If true, preview controls values are loaded from cache (even when postback)
    /// </summary>
    public bool SetControls
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SetControls"), false);
        }
    }


    /// <summary>
    /// Indicates whether show panel separator (used in vertical mode)
    /// </summary>
    public bool ShowPanelSeparator
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPanelSeparator"), false);
        }
    }


    /// <summary>
    /// Gets or sets the key under which the preview state is stored in the cookies
    /// </summary>
    public String CookiesPreviewStateName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CookiesPreviewStateName"), String.Empty);
        }
        set
        {
            SetValue("CookiesPreviewStateName", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ucButtons.PreviewURLSuffix = PreviewURLSuffix;
        ucButtons.SetControls = SetControls;
        ucButtons.PreviewObjectName = PreviewObjectName;
        ucButtons.DefaultAliasPath = DefaultAliasPath;
        ucButtons.DialogMode = DialogMode;
        ucButtons.PreviewInitialized = PreviewInitialized;
        ucButtons.IgnoreSessionValues = IgnoreSessionValues;
        ucButtons.DefaultPreviewPath = DefaultPreviewPath;

        int previewValue = GetPreviewStateFromCookies(CookiesPreviewStateName);

        if (previewValue == 2)
        {
            pnlMain.CssClass = pnlMain.CssClass.Replace("Vertical", "Horizontal");
        }

        imgVertical.Attributes["src"] = UIHelper.GetImageUrl(Page, "Design/Controls/SplitView/splitviewvertical.png");
        imgVertical.Attributes["Title"] = GetString("splitmode.verticallayout");

        imgNoSplit.Attributes["src"] = UIHelper.GetImageUrl(Page, "Design/Controls/SplitView/nosplit.png");
        imgNoSplit.Attributes["Title"] = GetString("splitmode.closelayout");

        imgHorizontal.Attributes["src"] = UIHelper.GetImageUrl(Page, "Design/Controls/SplitView/splitviewhorizontal.png");
        imgHorizontal.Attributes["Title"] = GetString("splitmode.horizontallayout");

        imgVertical.Attributes["OnClick"] = "changeSelected('vertical');performToolbarAction('vertical');return false;";
        imgHorizontal.Attributes["OnClick"] = "changeSelected('horizontal');performToolbarAction('horizontal');return false;";
        imgNoSplit.Attributes["OnClick"] = "changeSelected('nosplit');performToolbarAction('split');return false;";

        if (previewValue == 1)
        {
            divVertical.Attributes["class"] = "Button Selected";
            divHorizontal.Attributes["class"] = "Button";
        }

        if (previewValue == 2)
        {
            pnlButtons.Attributes["class"] = "Buttons";
            divHorizontal.Attributes["class"] = "Button Selected";
            divVertical.Attributes["class"] = "Button";
        }

        pnlHeaderSeparator.Visible = ShowPanelSeparator;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        String script = @"function changeSelected(action) {
                            var horizontalElem = document.getElementById('" + divHorizontal.ClientID + @"');
                            var verticalElem = document.getElementById('" + divVertical.ClientID + @"');
                            var nosplit = document.getElementById('" + divNoSplit.ClientID + @"');
                            if (action == 'horizontal') {
                                verticalElem.className = 'Button';                                
                                horizontalElem.className = 'Button Selected';
                            }
                            if (action == 'vertical') {
                                verticalElem.className = 'Button Selected';
                                horizontalElem.className = 'Button';
                            }
                            if (action == 'nosplit') {
                                nosplit.className = 'Button Selected';
                                horizontalElem.className = 'Button';
                                verticalElem.className = 'Button';
                            }
                        }";

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "ChangeSelected", ScriptHelper.GetScript(script));
    }

    #endregion
}
