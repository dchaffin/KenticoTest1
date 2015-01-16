<%@ Page Title="Search" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" AutoEventWireup="true" CodeFile="EditSource.aspx.cs" Inherits="CMSAdminControls_CodeMirror_dialogs_EditSource" %>

<asp:Content ID="plcContentContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSTextBox runat="server" ID="txtSource" TextMode="MultiLine" Width="800px"
        Height="550px" CssClass="SHEditorDialogCodeTextBox NORTL" />

    <script type="text/javascript" src="SearchReplace.js"></script>
    <script type="text/javascript">
        var txtSource = document.getElementById('<%=txtSource.ClientID%>');

        setInterval('doResize();', 500);
        getSource();
        focusOnTextBox();
    </script>
</asp:Content>
<asp:Content ID="plcFooterContent" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnOk" runat="server" CssClass="SubmitButton" OnClientClick="setSource(); CloseDialog();"
            ResourceString="general.ok" />
        <cms:LocalizedButton ID="btnCancel" runat="server" CssClass="SubmitButton" OnClientClick="CloseDialog();"
            ResourceString="general.cancel" />
    </div>
</asp:Content>
