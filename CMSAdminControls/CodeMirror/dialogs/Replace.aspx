<%@ Page Title="Replace" Language="C#" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Theme="Default" AutoEventWireup="true" CodeFile="Replace.aspx.cs" Inherits="CMSAdminControls_CodeMirror_dialogs_Replace" %>

<asp:Content ID="plcContentContent" ContentPlaceHolderID="plcContent" runat="server">
    <table class="SHEditorDialogTable">
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblFindWhat" runat="server" EnableViewState="false" ResourceString="cmsreplacedialog.findwhat"
                    DisplayColon="true" AssociatedControlID="txtFindWhat" />
            </td>
            <td>
                <cms:CMSTextBox runat="server" ID="txtFindWhat" EnableViewState="false" Text="" Width="98%" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblReplaceWith" runat="server" EnableViewState="false" ResourceString="cmsreplacedialog.replacewith"
                    DisplayColon="true" AssociatedControlID="txtReplaceWith" />
            </td>
            <td>
                <cms:CMSTextBox runat="server" ID="txtReplaceWith" EnableViewState="false" Text="" Width="98%" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblReplaceMode" runat="server" EnableViewState="false" ResourceString="cmsreplacedialog.replacemode"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizedRadioButtonList ID="rblReplaceMode" runat="server" EnableViewState="false"
                    RepeatDirection="Horizontal" UseResourceStrings="true">
                    <asp:ListItem Text="cmsreplacedialog.replacemode.replace" Value="down" Selected="True" />
                    <asp:ListItem Text="cmsreplacedialog.replacemode.replaceall" Value="up" />
                </cms:LocalizedRadioButtonList>
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
        <cms:LocalizedButton ID="btnFind" runat="server" EnableViewState="false" CssClass="SubmitButton"
            OnClientClick="findText(); return false;" ResourceString="general.find" />
        <cms:LocalizedButton ID="btnReplace" runat="server" EnableViewState="false" CssClass="SubmitButton"
            OnClientClick="replaceText(); return false;" ResourceString="general.replace" />
        <cms:LocalizedButton ID="btnClose" runat="server" EnableViewState="false" CssClass="SubmitButton"
            OnClientClick="CloseDialog();" ResourceString="general.close" />
    </div>
    <script type="text/javascript" src="SearchReplace.js"></script>
    <script type="text/javascript">
        var txtReplace = document.getElementById('<%=txtReplaceWith.ClientID%>');
        var rbAll = document.forms[0]['<%=rblReplaceMode.UniqueID%>'][1];
        var txtSearch = document.getElementById('<%=txtFindWhat.ClientID%>');
        var chkMatchCase = document.getElementById('<%=chkMatchCase.ClientID%>');
        var btnSearch = document.getElementById('<%=btnFind.ClientID%>');

        focusOnTextBox('<%=txtFindWhat.ClientID%>');
    </script>
</asp:Content>
