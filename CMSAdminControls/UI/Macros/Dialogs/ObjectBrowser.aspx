<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_Macros_Dialogs_ObjectBrowser"
    Title="Untitled Page" ValidateRequest="false" Theme="default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    CodeFile="ObjectBrowser.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/Trees/MacroTree.ascx" TagName="MacroTree" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagName="MacroEditor" TagPrefix="cms" %>
<asp:Content runat="server" ContentPlaceHolderID="plcBeforeContent">
    <div class="PageHeaderLine">
        <cms:MacroEditor ID="editorElem" runat="server" Text="CMSContext.Current" />
        <cms:LocalizedButton runat="server" ID="btnRefresh" ResourceString="general.Refresh"
            OnClick="btnRefresh_Click" />
        <cms:LocalizedCheckBox runat="server" ID="chkVirtual" Checked="true" Text="Virtual mode" />
    </div>
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:MacroTree ID="treeElem" runat="server" />
    <br />
    <br />
    <asp:TextBox runat="server" ID="txtOutput" TextMode="MultiLine" Height="400" Width="800" />
</asp:Content>
