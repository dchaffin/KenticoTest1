<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_FormBuilder_FormBuilder"
    CodeFile="FormBuilder.ascx.cs" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FormBuilder/ControlSelector.ascx"
    TagName="ControlSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FormBuilder/FieldSelector.ascx"
    TagName="FieldSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FormBuilder/FormPreview.ascx"
    TagName="FormPreview" TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<asp:Panel ID="pnlFormBuilder" runat="server">
    <cms:CMSPanel ID="pnlControlSelector" runat="server" Style="width: 100%;">
        <cms:ControlSelector runat="server" />
    </cms:CMSPanel>
    <cms:CMSPanel ID="pnlFieldSelector" runat="server" Style="float: left; width: 10%; height:400px;">
        <cms:FieldSelector ID="fieldSelector" runat="server" />
    </cms:CMSPanel>
    <cms:CMSPanel ID="pnlFormPreview" runat="server" Style="float: right; width: 89%;">
        <cms:FormPreview runat="server" />
    </cms:CMSPanel>
</asp:Panel>
