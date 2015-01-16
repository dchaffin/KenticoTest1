<%@ Page Title="Search" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="CMSAdminControls_CodeMirror_dialogs_Search" %>

<asp:Content ID="plcContentContent" ContentPlaceHolderID="plcContent" runat="server">
    <table class="SHEditorDialogTable">
        <col />
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblSearchFor" runat="server" EnableViewState="false" ResourceString="cmssearchdialog.searchfor"
                    AssociatedControlID="txtSearchFor" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtSearchFor" runat="server" EnableViewState="false" Text=""
                    Width="98%" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblMatchCase" runat="server" EnableViewState="false" ResourceString="general.matchcase"
                    DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox ID="chkMatchCase" runat="server" EnableViewState="false" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="plcFooterContent" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnSearch" runat="server" EnableViewState="false" CssClass="SubmitButton"
            OnClientClick="findText(); return false;" ResourceString="general.search" />
        <cms:LocalizedButton ID="btnClose" runat="server" EnableViewState="false" CssClass="SubmitButton"
            OnClientClick="CloseDialog();" ResourceString="general.close" />
    </div>
    <script type="text/javascript" src="SearchReplace.js"></script>
    <script type="text/javascript">
        var txtSearch = document.getElementById('<%=txtSearchFor.ClientID%>');
        var chkMatchCase = document.getElementById('<%=chkMatchCase.ClientID%>');
        var btnSearch = document.getElementById('<%=btnSearch.ClientID%>');

        focusOnTextBox('<%=txtSearchFor.ClientID%>');
    </script>
</asp:Content>
