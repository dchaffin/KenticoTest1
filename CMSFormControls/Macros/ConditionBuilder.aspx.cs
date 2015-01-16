using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.CMSHelper;

public partial class CMSFormControls_Macros_ConditionBuilderDialog : DesignerPage
{
    string clientId = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterESCScript = false;

        clientId = QueryHelper.GetString("clientid", "");

        SetTitle("CMSModules/CMS_DocumentLibrary/Properties.png", GetString("conditionbuilder.title"), "macros_rule_designer", "helpTopic");

        btnInsert.Click += new EventHandler(btnInsert_Click);

        designerElem.RuleCategoryNames = QueryHelper.GetString("module", "");
        designerElem.DisplayRuleType = QueryHelper.GetInteger("ruletype", 0);

        // Set correct  resolver to the control
        string resolverName = ValidationHelper.GetString(SessionHelper.GetValue("ConditionBuilderResolver" + clientId), "");
        if (!string.IsNullOrEmpty(resolverName))
        {
            designerElem.ResolverName = resolverName;
        }

        if (!RequestHelper.IsPostBack())
        {
            string condition = MacroResolver.RemoveDataMacroBrackets(ValidationHelper.GetString(SessionHelper.GetValue("ConditionBuilderCondition" + clientId), ""));
            designerElem.Value = condition;
        }
    }


    protected void btnInsert_Click(object sender, EventArgs e)
    {
        // Clean-up the session
        SessionHelper.Remove("ConditionBuilderCondition" + clientId);
        SessionHelper.Remove("ConditionBuilderResolver" + clientId);

        try
        {
            string text = ValidationHelper.GetString(designerElem.Value, "");
            ltlScript.Text = ScriptHelper.GetScript("wopener.InsertMacroCondition" + clientId + "(" + ScriptHelper.GetString(text) + "); CloseDialog();");
        }
        catch { }
    }
}