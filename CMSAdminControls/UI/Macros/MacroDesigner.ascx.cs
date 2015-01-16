using System;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.CMSHelper;
using CMS.UIControls;

public partial class CMSAdminControls_UI_Macros_MacroDesigner : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Returns resulting condition.
    /// </summary>
    public override object Value
    {
        get
        {
            switch (ValidationHelper.GetInteger(hdnSelTab.Value, 0))
            {
                case 1:
                    return designerElem.Value;

                case 2:
                    return editorElem.Text;

                default:
                    return ruleElem.Value;
            }
        }
        set
        {
            string val = ValidationHelper.GetString(value, "");
            if (val.EndsWithCSafe("%}"))
            {
                val = val.Substring(0, val.Length - 2);
            }
            if (val.StartsWithCSafe("{%"))
            {
                val = val.Substring(2);
            }

            string condition = ruleElem.ConditionFromExpression(val);
            hdnCondition.Value = condition;

            int tab = ValidationHelper.GetInteger(CookieHelper.GetValue(CookieName.MacroDesignerTab), 0);

            if (val == "")
            {
                if (tab == 1)
                {
                    ShowDesigner();
                }
                else if (tab == 2)
                {
                    ShowCode();
                }
                else
                {
                    ShowRuleEditor();
                }
            }
            else
            {
                if (condition != val)
                {
                    // If condition is different from whole expression, that it's rule editor
                    ShowRuleEditor();
                    ruleElem.Value = val;
                }
                else
                {
                    designerElem.Condition = condition;
                    editorElem.Text = condition;

                    // Show code tab by default if it's not a rule expression
                    if (tab == 1)
                    {
                        ShowDesigner();
                    }
                    else
                    {
                        ShowCode();
                    }
                }
            }
        }
    }


    /// <summary>
    /// Returns the condition created by this boolean expression designer.
    /// </summary>
    private string Condition
    {
        get
        {
            switch (ValidationHelper.GetInteger(hdnSelTab.Value, 0))
            {
                case 1:
                    return designerElem.Condition;

                case 2:
                    return editorElem.Text;

                default:
                    return ruleElem.GetCondition();
            }
        }
    }


    /// <summary>
    /// Returns the editor object.
    /// </summary>
    public ExtendedTextArea Editor
    {
        get
        {
            return editorElem.Editor;
        }
    }


    /// <summary>
    /// Gets or sets name(s) of the Macro rule category(ies) which should be displayed in Rule designer. Items should be separated by semicolon.
    /// </summary>
    public string RuleCategoryNames
    {
        get
        {
            return ruleElem.RuleCategoryNames;
        }
        set
        {
            ruleElem.RuleCategoryNames = value;
        }
    }


    /// <summary>
    /// Determines which rules to display. 0 means all rules, 1 means only rules which does not require context, 2 only rules which require context.
    /// </summary>
    public int DisplayRuleType
    {
        get
        {
            return ruleElem.DisplayRuleType;
        }
        set
        {
            ruleElem.DisplayRuleType = value;
        }
    }


    /// <summary>
    /// Name of the resolver to use.
    /// </summary>
    public override string ResolverName
    {
        get
        {
            return editorElem.ResolverName;
        }
        set
        {
            editorElem.ResolverName = value;
            designerElem.ResolverName = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        editorElem.Editor.UseSmallFonts = true;

        btnShowCode.Click += btnShowCode_Click;
        btnShowDesigner.Click += btnShowDesigner_Click;
        btnShowRuleEditor.Click += btnShowRuleEditor_Click;
        btnMoveGroup.Click += btnMoveGroup_Click;

        string[,] tabs = new string[3, 5];
        tabs[0, 0] = GetString("macrodesigner.ruleeditor");
        tabs[0, 1] = "if (GetSelected() != 0) { if (DisplayWarning()) {" + ControlsHelper.GetPostBackEventReference(btnShowRuleEditor, null) + "; } else { SelTab(GetSelected(), '_self', '') }} return false;";
        tabs[0, 2] = "";
        tabs[0, 3] = GetString("macrodesigner.codetooltip");
        tabs[1, 0] = GetString("macrodesigner.designer");
        tabs[1, 1] = "if (GetSelected() != 1) { if (DisplayWarning()) {" + ControlsHelper.GetPostBackEventReference(btnShowDesigner, null) + "; } else { SelTab(GetSelected(), '_self', '') }} return false;";
        tabs[1, 2] = "";
        tabs[1, 3] = GetString("macrodesigner.designertooltip");
        tabs[2, 0] = GetString("macrodesigner.code");
        tabs[2, 1] = "if (GetSelected() != 2) { if (DisplayWarning()) {" + ControlsHelper.GetPostBackEventReference(btnShowCode, null) + "; } else { SelTab(GetSelected(), '_self', '') }} return false;";
        tabs[2, 2] = "";
        tabs[2, 3] = GetString("macrodesigner.codetooltip");

        tabsElem.UseClientScript = true;
        tabsElem.UrlTarget = "_self";
        tabsElem.Tabs = tabs;

        // Register move script
        string script = string.Format(@"
function OnDropGroup(source, target) {{ 
  var container = target.get_container(); 
  var item = target.get_droppedItem();
  var targetPos = target.get_position(); 

  var hidden = document.getElementById('{0}')
  if (hidden != null) {{
    hidden.value = groupLocations[item.id] + ';' + groupLocations[container.id] + ';' + targetPos;
    {1}; 
  }}
}}

function MoveDesignerGroup(params, noOperationMessage) {{ 
  if (params != '') {{
    var hidden = document.getElementById('{0}')
    if (hidden != null) {{
      hidden.value = params;
      {1}; 
    }}
  }} else {{
     alert(noOperationMessage);
  }}
}}

function GetSelected() {{ 
  return document.getElementById('{2}').value;
}}

function DisplayWarning() {{ 
  if (GetSelected() == 0) {{
    return confirm({3});
  }}
  return true;
}}
", hdnMove.ClientID, ControlsHelper.GetPostBackEventReference(btnMoveGroup, null), hdnSelTab.ClientID, ScriptHelper.GetString(GetString("macrodesigner.switchfromruleeditor")));

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "DesignerOnDropGroup", script, true);
        ltlScript.Text = ScriptHelper.GetScript("var groupLocations = new Array();");

        SetHelp(ValidationHelper.GetInteger(hdnSelTab.Value, 0));
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        SelectTab(ValidationHelper.GetInteger(hdnSelTab.Value, 0));
    }


    protected void btnMoveGroup_Click(object sender, EventArgs e)
    {
        string[] moveParams = hdnMove.Value.Split(';');
        if (moveParams.Length == 3)
        {
            int targetPos = ValidationHelper.GetInteger(moveParams[2], 0);
            MoveGroup(moveParams[0], moveParams[1], targetPos);
        }
    }


    protected void btnShowRuleEditor_Click(object sender, EventArgs e)
    {
        ShowRuleEditor();
    }


    protected void btnShowDesigner_Click(object sender, EventArgs e)
    {
        ShowDesigner();
    }


    protected void btnShowCode_Click(object sender, EventArgs e)
    {
        ShowCode();
    }


    private void ShowRuleEditor()
    {
        SaveValue();
        if (hdnCondition.Value != ruleElem.GetCondition())
        {
            // Erase the designer when the condition is different
            ruleElem.Value = "";
        }

        HideAll();
        pnlRuleEditor.Visible = true;

        SelectTab(0);
    }


    private void ShowDesigner()
    {
        try
        {
            SaveValue();
            designerElem.Condition = hdnCondition.Value;

            HideAll();
            pnlDesigner.Visible = true;

            SelectTab(1);
        }
        catch (Exception ex)
        {
            ScriptHelper.Alert(Page, ex.Message);
            tabsElem.SelectedTab = ValidationHelper.GetInteger(hdnSelTab.Value, 0);
        }
    }


    private void ShowCode()
    {
        SaveValue();
        editorElem.Text = hdnCondition.Value;

        HideAll();
        pnlEditor.Visible = true;

        SelectTab(2);
    }


    private void SaveValue()
    {
        if (RequestHelper.IsPostBack())
        {
            switch (ValidationHelper.GetInteger(hdnSelTab.Value, 0))
            {
                case 1:
                    hdnCondition.Value = designerElem.Condition;
                    break;

                case 2:
                    hdnCondition.Value = editorElem.Text;
                    break;

                default:
                    ruleElem.RuleTree.AutoIndent();
                    hdnCondition.Value = ruleElem.GetCondition();
                    break;
            }
        }
    }


    private void HideAll()
    {
        pnlRuleEditor.Visible = false;
        pnlDesigner.Visible = false;
        pnlEditor.Visible = false;
    }


    private void SelectTab(int num)
    {
        tabsElem.SelectedTab = num;
        hdnSelTab.Value = num.ToString();

        SetHelp(num);

        CookieHelper.SetValue(CookieName.MacroDesignerTab, num.ToString(), DateTime.Now.AddDays(1));
    }


    private void SetHelp(int num)
    {
        switch (num)
        {
            case 1:
                ((CMSPage)this.Page).SetHelp("macros_designer", "helpTopic");
                break;

            case 2:
                ((CMSPage)this.Page).SetHelp("macros_code", "helpTopic");
                break;

            default:
                ((CMSPage)this.Page).SetHelp("macros_rule_designer", "helpTopic");
                break;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Rebuilds the designer according to a text in the editor.
    /// </summary>
    public void RebuildDesigner()
    {
        designerElem.Condition = editorElem.Text;
    }


    /// <summary>
    /// Moves the group within the designer
    /// </summary>
    /// <param name="sourcePath">Source path</param>
    /// <param name="targetPath">Target path</param>
    /// <param name="targetPos">Target position</param>
    public void MoveGroup(string sourcePath, string targetPath, int targetPos)
    {
        designerElem.StoreData();
        designerElem.MoveGroup(sourcePath, targetPath, targetPos);
    }

    #endregion
}