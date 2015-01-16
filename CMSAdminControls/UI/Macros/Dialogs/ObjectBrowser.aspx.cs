using System;
using System.Text;

using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.GlobalHelper;

[Title(Text = "Macro browser")]
public partial class CMSAdminControls_UI_Macros_Dialogs_ObjectBrowser : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        editorElem.Editor.Height = 30;
        editorElem.Editor.ShowToolbar = false;
        editorElem.MixedMode = false;
        editorElem.Editor.DynamicHeight = true;

        treeElem.VirtualMode = chkVirtual.Checked;
        treeElem.ContextResolver.CheckSecurity = false;
        treeElem.MacroExpression = editorElem.Text;
        treeElem.OnNodeClickHandler = "SelectItem";

        string macro = editorElem.Text;

        txtOutput.Text = treeElem.ContextResolver.ResolveMacros("{%" + macro + "%}");

        StringBuilder sb = new StringBuilder();

        sb.Append(
@"
function SelectItem(macro) { 
    ", editorElem.Editor.EditorID, @".setValue('", macro, @".' + macro);
}
"
        );

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SelectItem", ScriptHelper.GetScript(sb.ToString()));
    }


    protected void btnRefresh_Click(object sender, EventArgs e)
    {
    }
}
