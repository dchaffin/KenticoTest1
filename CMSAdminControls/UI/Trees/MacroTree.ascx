<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Trees_MacroTree"
    EnableViewState="false" CodeFile="MacroTree.ascx.cs" %>
<asp:Panel runat="server" ID="pnlPriorityTree" EnableViewState="false">
    <cms:UITreeView runat="server" ID="treeElemPriority" ShortID="tp" ShowLines="true"
        PopulateNodesFromClient="true" OnTreeNodePopulate="treeElemPriority_TreeNodePopulate"
        EnableViewState="false" />
</asp:Panel>
<cms:UITreeView runat="server" ID="treeElem" ShortID="t" ShowLines="true" PopulateNodesFromClient="true"
    OnTreeNodePopulate="treeElem_TreeNodePopulate" EnableViewState="false" />
