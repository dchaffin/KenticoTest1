<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_New_NewFile"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Content - New file"
    CodeFile="NewFile.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DocumentAttachments/DirectUploader.ascx"
    TagName="DirectFileUploader" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/EditMenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ID="cntMenu" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" ShortID="m" runat="server" ShowProperties="false" ShowSpellCheck="true"
        IsLiveSite="false" />
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel ID="pnlForm" runat="server">
        <table>
            <tr>
                <td>
                    <asp:Label ID="lblUploadFile" runat="server" EnableViewState="false" />
                </td>
                <td>
                    <asp:PlaceHolder ID="plcDirect" runat="server">
                        <cms:DirectFileUploader ID="ucDirectUploader" runat="server" CheckPermissions="false" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcUploader" runat="server" Visible="false">
                        <cms:CMSFileUpload ID="FileUpload" runat="server" Width="456px" />
                    </asp:PlaceHolder>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblFileDescription" runat="server" EnableViewState="false" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtFileDescription" runat="server" MaxLength="500" TextMode="MultiLine"
                        Rows="6" Width="450px"></cms:CMSTextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
