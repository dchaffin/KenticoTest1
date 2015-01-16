using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSAdminControls_EditingFormControl : CMSModalPage
{
    #region "Variables"

    protected string selectorId = "";
    protected string controlPanelId = "";
    protected string selectorPanelId = "";

    private ContextResolver mMacroResolver = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Default resolver.
    /// </summary>
    protected ContextResolver MacroResolver
    {
        get
        {
            if (mMacroResolver == null)
            {
                // Init resolver
                mMacroResolver = ResolverFactory.GetResolver(QueryHelper.GetText("resolverName", null));
                mMacroResolver.Context.VirtualMode = true;
            }

            return mMacroResolver;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get query string parameters
        selectorId = QueryHelper.GetText("selectorid", "");
        controlPanelId = QueryHelper.GetText("controlPanelId", "");
        selectorPanelId = QueryHelper.GetText("selectorpanelid", "");

        // Initialize UI
        CurrentMaster.Title.TitleText = GetString("EditingFormControl.TitleText");
        CurrentMaster.Title.TitleImage = GetImageUrl("Design/Controls/EditingFormControl/title.png");

        btnOk.Text = GetString("General.Save");
        btnRemove.Text = GetString("editingformcontrol.removemacros");

        btnOk.OnClientClick = "setValueToParent(" + ScriptHelper.GetString(selectorId) + ", " + ScriptHelper.GetString(controlPanelId) + ", " + ScriptHelper.GetString(selectorPanelId) + "); return CloseDialog();";
        btnRemove.OnClientClick = "removeMacro(" + ScriptHelper.GetString(selectorId) + ", " + ScriptHelper.GetString(controlPanelId) + ", " + ScriptHelper.GetString(selectorPanelId) + "); return CloseDialog();";

        btnCancel.Text = GetString("General.Cancel");

        ScriptHelper.RegisterClientScriptBlock(this.Page, typeof(string), "SetNestedControlValue", @"
function setValueToParent(selId, controlPanelId, selPanelId) {
    wopener.setNestedControlValue(selId, controlPanelId, trimNewLines(" + macroEditor.Editor.GetValueGetterCommand() + @"), selPanelId); 
}", true);

        macroEditor.Resolver = MacroResolver;
        macroEditor.MixedMode = true;
        macroEditor.Editor.Language = LanguageEnum.HTMLMixed;
        macroEditor.Editor.Width = new Unit("97%");
        macroEditor.Editor.Height = new Unit("255px");
        macroEditor.Editor.FullScreenParentElementID = "divContent";

        macroSelector.Resolver = MacroResolver;
        macroSelector.ExtendedTextAreaElem = macroEditor.Editor.EditorID;
        macroSelector.TextAreaID = macroEditor.Editor.ClientID;
        macroSelector.ShowMacroTreeAbove = true;
    }


    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);
        writer.Write(ScriptHelper.GetScript(macroEditor.Editor.GetValueSetterCommand("wopener.getNestedControlValue('" + selectorId + "')")));
    }
}