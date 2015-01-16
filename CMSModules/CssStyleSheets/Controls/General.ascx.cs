using System;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_CssStyleSheets_Controls_General : CMSPreviewControl
{
    #region "Variables"

    protected bool startWithFullScreen = true;
    private CssStylesheetInfo si = null;
    private int previewState = 0;
    private int cssStylesheetId = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if the page is in tab mode.
    /// </summary>
    public virtual bool TabMode
    {
        get
        {
            return QueryHelper.GetBoolean("tabmode", false);
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return editMenuElem.MessagesPlaceHolder;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        fDisplayName.Label.Visible = false;
        fName.Label.Visible = false;

        txtCssStylesheetText.Editor.EditorMode = EditorModeEnum.Advanced;
        txtCssStylesheetText.Editor.Language = LanguageEnum.CSS;
        txtCssStylesheetText.Editor.ShowBookmarks = true;
        txtCssStylesheetText.Editor.Width = Unit.Percentage(100);
        txtCssStylesheetText.Editor.Height = Unit.Pixel(385);
        txtCssStylesheetText.Editor.RegularExpression = @"\s*/\*\s*#\s*([a-zA-Z_0-9-/\+\*.=~\!@\$%\^&\(\[\]\);:<>\?\s]*)\s*#\s*\*/";
        txtCssStylesheetText.Editor.EnablePositionMember = true;
        txtCssStylesheetText.Editor.EnableSections = true;

        si = CMSContext.EditedObject as CssStylesheetInfo;

        base.OnInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        RegisterResizeHeaders();

        previewState = GetPreviewStateFromCookies(CMSPreviewControl.CSSSTYLESHEET);

        if (previewState != 0)
        {
            txtCssStylesheetText.TopOffset = 40;
        }

        // Add preview action
        HeaderAction preview = new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.LinkButton,
            Text = GetString("general.preview"),
            OnClientClick = "performToolbarAction('split');return false;",
            ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/Preview.png"),
            SmallImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/16/Preview.png"),
            Visible = (previewState == 0),
            Tooltip = GetString("preview.tooltip")
        };
        editMenuElem.ObjectEditMenu.AddExtraAction(preview);
        editMenuElem.ObjectEditMenu.PreviewMode = true;
        editMenuElem.MenuPanel.CssClass = "PreviewMenu";
        editMenuElem.ObjectManager.OnAfterAction += new EventHandler<SimpleObjectManagerEventArgs>(ObjectManager_OnAfterAction);

        if ((si != null) && (si.StylesheetID != 0))
        {
            cssStylesheetId = si.StylesheetID;
        }
        else
        {
            cssStylesheetId = QueryHelper.GetInteger("cssstylesheetid", 0);
        }

        bool hide = !(BrowserHelper.IsSafari() || BrowserHelper.IsChrome());
        if (hide)
        {
            pnlContainer.CssClass += " Hidden ";
        }

        // Initialize selected line
        if (!RequestHelper.IsPostBack())
        {
            string script = @"
$j(document).ready(function () {
    setTimeout(function() {
        var cmCSS = " + txtCssStylesheetText.Editor.EditorID + @";
        if(cmCSS != null) {
            cmCSS.setCursor(" + (QueryHelper.GetInteger("line", 0) - 1) + @");
        }
    }, 50);
});";
            // Register client script
            ScriptHelper.RegisterStartupScript(this, typeof(string), "JumpToLine", script, true);
        }

        if (CMSObjectHelper.UseCheckinCheckout)
        {
            pnlContainer.CssClass = "PreviewDefaultContentLarge";
        }
        else
        {
            pnlContainer.CssClass = "PreviewDefaultContent";
        }

        // Register action script for dialog purposes
        if (DialogMode)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "PreviewHierarchyPerformAction", ScriptHelper.GetScript("function actionPerformed(action) { if (action == 'saveandclose') { document.getElementById('" + hdnClose.ClientID + "').value = '1'; }; " + editMenuElem.ObjectManager.GetJSFunction(ComponentEvents.SAVE, null, null) + "; }"));
        }
    }


    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if (e.ActionName == ComponentEvents.SAVE)
        {
            if (DialogMode)
            {
                string script = String.Empty;
                string selector = QueryHelper.GetString("selectorid", string.Empty);
                if (!string.IsNullOrEmpty(selector))
                {
                    // Selects newly created container in the UniSelector
                    script = string.Format(@"if (wopener && wopener.US_SelectNewValue_{0}) {{ wopener.US_SelectNewValue_{0}('{1}'); }}",
                                      selector, QueryHelper.GetInteger("cssstylesheetid", -1));
                }

                if (ValidationHelper.GetBoolean(hdnClose.Value, false))
                {
                    script += "; CloseDialog();";
                }

                if (script != String.Empty)
                {
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "UpdateSelector", ScriptHelper.GetScript(script));
                }
            }

            if (txtCssStylesheetText.Editor.ShowBookmarks)
            {
                // Refresh bookmarks 
                txtCssStylesheetText.Editor.ExtractBookmarks();
                String scriptBookmark = String.Format("RefreshBookmarks({0}, {1});", txtCssStylesheetText.Editor.EditorID, txtCssStylesheetText.Editor.GetPreformattedBookmarks());
                ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "RefreshBookmarks", ScriptHelper.GetScript(scriptBookmark));
            }

            RegisterRefreshScript();
        }

        if (DialogMode)
        {
            switch (e.ActionName)
            {
                case ComponentEvents.SAVE:
                case ComponentEvents.CHECKOUT:
                case ComponentEvents.UNDO_CHECKOUT:
                case ComponentEvents.CHECKIN:
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "wopenerRefresh", ScriptHelper.GetScript("if (wopener && wopener.refresh) { wopener.refresh(); }"));
                    break;
            }
        }

        if ((TabMode || !DialogMode) && (e.ActionName != ComponentEvents.CHECKOUT))
        {
            ScriptHelper.RefreshTabHeader(Page, null);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        startWithFullScreen = ((previewState != 0) && editMenuElem.ObjectManager.IsObjectChecked());

        txtCssStylesheetText.Editor.AutoSize = true;
        txtCssStylesheetText.Editor.ParentElementID = ParentClientID;
        RegisterInitScripts(pnlContainer.ClientID, editMenuElem.MenuPanel.ClientID, startWithFullScreen);

        base.OnPreRender(e);
    }

    #endregion
}
