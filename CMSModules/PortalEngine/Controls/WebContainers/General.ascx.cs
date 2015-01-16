using System;

using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.CMSHelper;

public partial class CMSModules_PortalEngine_Controls_WebContainers_General : CMSPreviewControl
{
    #region "Variables"

    private const string WP_CHAR = "□";

    private int previewState = 0;
    protected bool startWithFullScreen = false;

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

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterResizeHeaders();

        previewState = GetPreviewStateFromCookies(CMSPreviewControl.WEBPARTCONTAINER);

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

        bool hide = !(BrowserHelper.IsSafari() || BrowserHelper.IsChrome());
        if (hide)
        {
            pnlContainer.CssClass += " Hidden ";
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

        codeElem.Editor.ParentElementID = ParentClientID;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        startWithFullScreen = ((previewState != 0) && editMenuElem.ObjectManager.IsObjectChecked());
        RegisterInitScripts(pnlContainer.ClientID, editMenuElem.MenuPanel.ClientID, startWithFullScreen);
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

    #endregion
}
