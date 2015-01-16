using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls.DragAndDrop;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.FormControls;

public partial class CMSAdminControls_UI_Macros_MacroDesignerGroup : MacroDesignerGroup
{
    #region "Variables"

    /// <summary>
    /// Drag and drop extender for the group
    /// </summary>
    protected DragAndDropExtender extDragDrop = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns the operator of the group.
    /// </summary>
    public override string GroupOperator
    {
        get
        {
            if (radOr.Checked)
            {
                return "||";
            }
            else
            {
                return "&&";
            }
        }
        set
        {
            if (value == "||")
            {
                radOr.Checked = true;
            }
            else
            {
                radAnd.Checked = true;
            }
        }
    }


    /// <summary>
    /// Returns the condition created by this boolean expression designer.
    /// </summary>
    public override string Condition
    {
        get
        {
            StoreData();
            return CurrentGroup.Condition;
        }
        set
        {
            // Parse the condition and build the tree from it
            try
            {
                MacroExpression expr = MacroExpression.ParseExpression(value, true);
                expr.MakeFlatter();
                CurrentGroup = MacroDesignerTree.BuildTreeFromExpression(expr);

                // Rebuild the designer according to the new condition
                BuildDesigner(true);
            }
            catch { }
        }
    }


    /// <summary>
    /// Gets or sets the condition represented by the designer.
    /// </summary>
    public override object Value
    {
        get
        {
            return Condition;
        }
        set
        {
            Condition = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// Gets or sets the current macro structure.
    /// </summary>
    public override MacroDesignerTree CurrentGroup
    {
        get
        {
            MacroDesignerTree group = ViewState["CurrentGroup"] as MacroDesignerTree;
            if (group == null)
            {
                group = new MacroDesignerTree();
                ViewState["CurrentGroup"] = group;
            }
            return group;
        }
        set
        {
            ViewState["CurrentGroup"] = value;
        }
    }


    /// <summary>
    /// Name of the resolver.
    /// </summary>
    public override string ResolverName
    {
        get
        {
            string clientId = QueryHelper.GetString("clientid", "");
            return ValidationHelper.GetString(SessionHelper.GetValue("ConditionBuilderResolver" + clientId), "");
        }
        set
        {
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        btnAddGroup.Click += new ImageClickEventHandler(btnAddGroup_Click);
        btnAddExp.Click += new ImageClickEventHandler(btnAddExp_Click);
        btnRemove.Click += new ImageClickEventHandler(btnRemove_Click);
        btnRemoveExpression.Click += new ImageClickEventHandler(btnRemove_Click);

        btnAddGroup.ImageUrl = GetImageUrl("/Design/Controls/MacroDesigner/AddGroup.png");
        btnAddExp.ImageUrl = GetImageUrl("/Design/Controls/MacroDesigner/AddExpression.png");
        btnRemove.ImageUrl = GetImageUrl("/CMSModules/CMS_PortalEngine/Widgets/DeleteDashboard.png");
        btnRemoveExpression.ImageUrl = GetImageUrl("/Design/Controls/MacroDesigner/Remove.png");
        btnGroupContextMenu.ImageUrl = GetImageUrl("/Design/Controls/UniGrid/Actions/Menu.png");
        btnExprContextMenu.ImageUrl = GetImageUrl("/Design/Controls/UniGrid/Actions/Menu.png");
        imgMove.ImageUrl = GetImageUrl("/Design/Controls/MacroDesigner/Move.png");

        BuildDesigner(false);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        cmcExpr.MenuControlPath = "~/CMSAdminControls/UI/Macros/MacroDesignerMenu.ascx";
        cmcGroup.MenuControlPath = "~/CMSAdminControls/UI/Macros/MacroDesignerMenu.ascx";
        cmcExprHandle.MenuControlPath = "~/CMSAdminControls/UI/Macros/MacroDesignerMenu.ascx";
        cmcGroupHeader.MenuControlPath = "~/CMSAdminControls/UI/Macros/MacroDesignerMenu.ascx";

        int groupCount = (CurrentGroup.ParentGroup == null ? 0 : CurrentGroup.ParentGroup.ChildGroups.Count);

        cmcExpr.Parameter = PrepareContextMenuParameters(CurrentGroup.IDPath, groupCount);
        cmcGroup.Parameter = PrepareContextMenuParameters(CurrentGroup.IDPath, groupCount);
        cmcExprHandle.Parameter = PrepareContextMenuParameters(CurrentGroup.IDPath, groupCount);
        cmcGroupHeader.Parameter = PrepareContextMenuParameters(CurrentGroup.IDPath, groupCount);

        bool isLeaf = CurrentGroup.IsLeaf;

        // Set correct styles
        pnlMacroGroup.CssClass = (isLeaf ? "MacroDesignerExpression" : "MacroDesignerGroup");
        pnlGroups.CssClass = (isLeaf ? "" : "MacroDesignerChildGroups");

        // Display correct controls according to type
        pnlOperator.Visible = (CurrentGroup.Position > 0);
        pnlRemoveGroup.Visible = (CurrentGroup.ParentGroup != null) && !isLeaf;
        pnlRemoveExpression.Visible = (CurrentGroup.ParentGroup != null) && isLeaf;
        pnlHandle.Visible = isLeaf;
        pnlButtons.Visible = !isLeaf;
        pnlHeader.Visible = !isLeaf;
        pnlGroups.Attributes.Add("IDPath", CurrentGroup.IDPath);

        // Display correct tooltips
        btnAddGroup.ToolTip = GetString("macrodesigner.addgroup");
        btnAddExp.ToolTip = GetString("macrodesigner.addexpression");
        btnRemove.ToolTip = GetString("macrodesigner.removegroup");
        btnRemoveExpression.ToolTip = GetString("macrodesigner.removeexpression");
        imgMove.ToolTip = GetString("macrodesigner.moveexpression");

        pnlGroupHandle.Attributes.Add("onmousedown", "return false;");
        pnlGroupHandle.Attributes.Add("onclick", "return false;");

        pnlItemHandle.Attributes.Add("onmousedown", "return false;");
        pnlItemHandle.Attributes.Add("onclick", "return false;");

        if (isLeaf)
        {
            pnlItemHandle.Attributes.Add("onmouseover", "ActivateBorder('" + pnlMacroGroup.ClientID + "', 'MacroDesignerExpression');");
            pnlItemHandle.Attributes.Add("onmouseout", "DeactivateBorder('" + pnlMacroGroup.ClientID + "', 'MacroDesignerExpression');");
        }
        else
        {
            pnlMacroGroup.CssClass += " G";

            if (CurrentGroup.Level > 0)
            {
                pnlHeader.Attributes.Add("onmouseover", "ActivateBorder('" + pnlMacroGroup.ClientID + "', 'MacroDesignerGroup');");
                pnlHeader.Attributes.Add("onmouseout", "DeactivateBorder('" + pnlMacroGroup.ClientID + "', 'MacroDesignerGroup');");
            }
            else
            {
                // First group is not movable
                pnlHeader.Attributes["style"] = "cursor: default;";
            }
        }

        // Register the scripts
        string script = @"
function ActivateBorder(elementId, className) {
  var e = document.getElementById(elementId);
  if (e != null) {
    e.className = e.className.replace(className, className + 'Active');
  }
}

function DeactivateBorder(elementId, className) {
  var e = document.getElementById(elementId);
  if (e != null) {
    e.className = e.className.replace(className + 'Active', className);
  }
}
";
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ActivateDeactivateBorder", script, true);

        // Register IDPath of the group;
        ltlScript.Text = ScriptHelper.GetScript("groupLocations['" + pnlGroups.ClientID + "'] = '" + CurrentGroup.IDPath + "'; groupLocations['" + ClientID + "'] = '" + CurrentGroup.IDPath + "';");
    }


    protected override void Render(HtmlTextWriter writer)
    {
        writer.Write("<div id=\"{0}\" class=\"MacroElement\">", ClientID);

        base.Render(writer);

        writer.Write("</div>");
    }


    protected void btnRemove_Click(object sender, ImageClickEventArgs e)
    {
        CurrentGroup.ParentGroup.RemoveGroup(CurrentGroup.Position);
        RebuildDesigner();
    }


    protected void btnAddExp_Click(object sender, ImageClickEventArgs e)
    {
        MacroDesignerGroup root = GetRootGroup();
        root.StoreData();

        MacroDesignerTree item = CurrentGroup.AddExpression();
        MacroDesignerGroup ctrl = (MacroDesignerGroup)AddGroup(item.IDPath);
        ctrl.CurrentGroup = item;

        root.BuildDesigner(true);
    }


    protected void btnAddGroup_Click(object sender, ImageClickEventArgs e)
    {
        MacroDesignerGroup root = GetRootGroup();
        root.StoreData();

        MacroDesignerTree item = CurrentGroup.AddGroup();
        AddGroup(item.IDPath);

        root.BuildDesigner(true);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Create parameters for context menu (format [up]@[down]@[parent]).
    /// </summary>
    /// <param name="idPath">IDPath</param>
    private static string PrepareContextMenuParameters(string idPath, int itemCount)
    {
        string retval = "";
        if (!string.IsNullOrEmpty(idPath))
        {
            string[] path = idPath.Split('.');
            int currentPos = ValidationHelper.GetInteger(path[path.Length - 1], 0);
            if (path.Length == 1)
            {
                string upParams = "";
                if (currentPos > 0)
                {
                    upParams = idPath + ";;" + (currentPos - 1);
                }
                string downParams = "";
                if (itemCount > currentPos + 1)
                {
                    downParams = idPath + ";;" + (currentPos + 1);
                }

                retval = upParams + "@" + downParams + "@";
            }
            else
            {
                string targetPathBase = string.Join(".", path, 0, path.Length - 2);

                string upParams = "";
                if (currentPos > 0)
                {
                    upParams = idPath + ";" + targetPathBase + "." + path[path.Length - 2] + ";" + (currentPos - 1);
                }
                string downParams = "";
                if (itemCount > currentPos + 1)
                {
                    downParams = idPath + ";" + targetPathBase + "." + path[path.Length - 2] + ";" + (currentPos + 1);
                }
                string parentParams = idPath + ";" + targetPathBase + ";" + ValidationHelper.GetInteger(path[path.Length - 2], 0);

                retval = upParams + "@" + downParams + "@" + parentParams;
            }
        }
        return ScriptHelper.GetString(retval);
    }


    /// <summary>
    /// Returns the root group.
    /// </summary>
    /// <param name="recursive">If true, BuldDesigner is called to child groups as well</param>
    public MacroDesignerGroup GetRootGroup()
    {
        MacroDesignerGroup group = this;
        while ((group.Parent != null) && (group.Parent.Parent.Parent != null) && (group.Parent.Parent.Parent is MacroDesignerGroup))
        {
            group = (MacroDesignerGroup)group.Parent.Parent.Parent;
        }
        return group;
    }


    /// <summary>
    /// Saves the current data to the MacroDesignerTree object.
    /// </summary>
    public override void StoreData()
    {
        if (CurrentGroup.IsLeaf)
        {
            CurrentGroup.GroupOperator = GroupOperator;
            if (pnlGroups.Controls.Count > 0)
            {
                MacroBoolExpression expr = (MacroBoolExpression)pnlGroups.Controls[0];
                CurrentGroup.LeftExpression = expr.LeftExpression;
                CurrentGroup.RightExpression = expr.RightExpression;
                CurrentGroup.Operator = expr.Operator;
            }
        }
        else
        {
            CurrentGroup.GroupOperator = GroupOperator;
            foreach (Control item in pnlGroups.Controls)
            {
                if (item is MacroDesignerGroup)
                {
                    MacroDesignerGroup group = (MacroDesignerGroup)item;
                    group.StoreData();
                }
            }
        }
    }


    /// <summary>
    /// Builds the designer controls structure.
    /// </summary>
    public override void BuildDesigner(bool recursive)
    {
        // Append child groups and expressions
        pnlGroups.Controls.Clear();

        if (CurrentGroup.IsLeaf)
        {
            plcNoItems.Visible = false;

            AddExpression(CurrentGroup.LeftExpression, CurrentGroup.RightExpression, CurrentGroup.Operator, CurrentGroup.IDPath);
        }
        else
        {
            if (CurrentGroup.Level == 0)
            {
                plcNoItems.Visible = true;
            }

            // Add child groups
            foreach (MacroDesignerTree child in CurrentGroup.ChildGroups)
            {
                MacroDesignerGroup ctrl = (MacroDesignerGroup)AddGroup(child.IDPath);
                ctrl.GroupOperator = child.GroupOperator;
                ctrl.CurrentGroup = child;
                if (recursive)
                {
                    ctrl.BuildDesigner(true);
                }

                plcNoItems.Visible = false;
            }

            // Drop cue
            Panel pnlCue = new Panel();
            pnlCue.ID = "pnlCue";
            pnlCue.CssClass = "MacroDesignerCue";
            pnlGroups.Controls.Add(pnlCue);

            pnlCue.Controls.Add(new LiteralControl("&nbsp;"));
            pnlCue.Style.Add("display", "none");

            // Create drag and drop extender
            extDragDrop = new DragAndDropExtender();

            extDragDrop.ID = "extDragDrop";
            extDragDrop.TargetControlID = pnlGroups.ID;
            extDragDrop.DragItemClass = "MacroElement";
            extDragDrop.DragItemHandleClass = "MacroElementHandle";
            extDragDrop.DropCueID = pnlCue.ID;
            extDragDrop.OnClientDrop = "OnDropGroup";

            pnlGroups.Controls.Add(extDragDrop);
        }
    }


    /// <summary>
    /// Rebuilds the whole structure according to the MacroDesignerTree object.
    /// </summary>
    public void RebuildDesigner()
    {
        MacroDesignerGroup root = GetRootGroup();
        root.StoreData();
        root.BuildDesigner(true);
    }


    /// <summary>
    /// Moves the group to given location.
    /// </summary>
    /// <param name="sourcePath">Position path of the source</param>
    /// <param name="targetPath">Position path of the target</param>
    /// <param name="targetPos">Position within the target</param>
    public void MoveGroup(string sourcePath, string targetPath, int targetPos)
    {
        MacroDesignerGroup root = GetRootGroup();
        root.StoreData();
        root.CurrentGroup.MoveGroup(sourcePath, targetPath, targetPos);
        root.BuildDesigner(true);
    }


    /// <summary>
    /// Loads new group control into the child panel controls collection.
    /// </summary>
    /// <param name="idPath">IDPath to ensure unique ID of the control</param>
    public Control AddGroup(string idPath)
    {
        // Add control to a controls collection
        MacroDesignerGroup group = (MacroDesignerGroup)Page.LoadUserControl("~/CMSAdminControls/UI/Macros/MacroDesignerGroup.ascx");
        group.ID = "g" + idPath.Replace(".", "_");
        pnlGroups.Controls.Add(group);

        return group;
    }


    /// <summary>
    /// Adds an expression to a child panel controls collection.
    /// </summary>
    /// <param name="left">Left part of the expression</param>
    /// <param name="right">Right part of the expression</param>
    /// <param name="op">Operator to use</param>
    /// <param name="idPath">IDPath to ensure unique ID of the control</param>
    public void AddExpression(string left, string right, string op, string idPath)
    {
        // Add control to a controls collection
        MacroBoolExpression expr = (MacroBoolExpression)Page.LoadUserControl("~/CMSAdminControls/UI/Macros/MacroBoolExpression.ascx");
        expr.LeftExpression = left;
        expr.RightExpression = right;
        expr.Operator = op;
        expr.ID = "e" + idPath.Replace(".", "_");
        expr.ResolverName = ResolverName;

        pnlGroups.Controls.Add(expr);
    }

    #endregion
}