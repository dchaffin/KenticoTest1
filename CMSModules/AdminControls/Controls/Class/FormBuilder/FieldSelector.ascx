<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FieldSelector.ascx.cs" Inherits="CMSModules_AdminControls_Controls_Class_FormBuilder_FieldSelector" %>
<ajaxToolkit:ReorderList ID="lstFields"  runat="server" AllowReorder="true" DragHandleAlignment="Left" ClientIDMode="AutoID">
    <ItemTemplate>
        <%# Eval("Text") %>
    </ItemTemplate>
    <DragHandleTemplate>
        <div class="dragHandle">x</div>
    </DragHandleTemplate>
</ajaxToolkit:ReorderList>