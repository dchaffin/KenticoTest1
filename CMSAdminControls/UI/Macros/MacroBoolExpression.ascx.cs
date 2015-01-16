using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.CMSHelper;

public partial class CMSAdminControls_UI_Macros_MacroBoolExpression : MacroBoolExpression
{
    #region "Properties"

    /// <summary>
    /// Enables or disables the control.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return drpOperator.Enabled;
        }
        set
        {
            drpOperator.Enabled = value;
            leftOperand.Enabled = value;
            rightOperand.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the left part of the expression.
    /// </summary>
    public override string LeftExpression
    {
        get
        {
            return leftOperand.Text;
        }
        set
        {
            leftOperand.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the left part of the expression.
    /// </summary>
    public override string RightExpression
    {
        get
        {
            return rightOperand.Text;
        }
        set
        {
            rightOperand.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the left part of the expression.
    /// </summary>
    public override string Operator
    {
        get
        {
            return (string.IsNullOrEmpty(drpOperator.SelectedValue) ? "--" : drpOperator.SelectedValue);
        }
        set
        {
            EnsureOperators();
            drpOperator.SelectedValue = value.ToLowerCSafe();
            pnlRightOperand.Style["display"] = (value.EndsWithCSafe("--") ? "none" : "block");
        }
    }


    /// <summary>
    /// Resolver to use.
    /// </summary>
    public override string ResolverName
    {
        get
        {
            return leftOperand.ResolverName;
        }
        set
        {
            leftOperand.ResolverName = value;
            rightOperand.ResolverName = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureOperators();

        leftOperand.Editor.UseSmallFonts = true;
        rightOperand.Editor.UseSmallFonts = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string script = @"
function ShowHideRightOperand(elementId, drpId) {
  var ddl = document.getElementById(drpId);
  var e = document.getElementById(elementId);
  if ((e != null) && (ddl != null)) {
    if ((ddl.value == '--') || (ddl.value == '!--')) {
      e.style.display = 'none';
    } else {
      e.style.display = 'block';
    }
  }
}";

        // Hide right side for unary operators
        if (drpOperator.SelectedValue.EndsWithCSafe("--"))
        {
            pnlRightOperand.Style.Add("display", "none");
        }

        drpOperator.Attributes.Add("onchange", "ShowHideRightOperand('" + pnlRightOperand.ClientID + "', '" + drpOperator.ClientID + "');");
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ShowHideRightOperand", script, true);
    }

    #endregion


    /// <summary>
    /// Loads the DDL with operators.
    /// </summary>
    private void EnsureOperators()
    {
        if (drpOperator.Items.Count == 0)
        {
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.istrue"), "--"));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.isfalse"), "!--"));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.equals"), "=="));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.notequals"), "!="));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.greaterthan"), ">"));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.lessthan"), "<"));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.greaterorequal"), ">="));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.lessorequal"), "<="));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.contains"), "contains"));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.notcontains"), "!contains"));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.endswith"), "endswith"));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.notendswith"), "!endswith"));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.startswith"), "startswith"));
            drpOperator.Items.Add(new ListItem(GetString("macrodesigner.notstartswith"), "!startswith"));
        }
    }
}