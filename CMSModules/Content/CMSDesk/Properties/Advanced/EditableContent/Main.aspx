<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_Advanced_EditableContent_Main"
    Theme="Default" CodeFile="Main.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ID="cntMenu" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:CMSPageManager ID="pageManagerElem" runat="server" Visible="false" />
    <cms:editmenu ID="menuElem" runat="server" ShowApprove="true" ShowReject="true" ShowSubmitToApproval="true"
        ShowProperties="false" ShowSave="false" ShowCreateAnother="false" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel ID="pnlEditableContent" runat="server" Visible="false">
        <table style="width: 100%">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblEditControl" runat="server" EnableViewState="false" CssClass="FieldLabel"
                        ResourceString="EditableContent.EditControl" />
                </td>
                <td style="width: 100%;">
                    <asp:DropDownList ID="drpEditControl" runat="server" AutoPostBack="true" CssClass="DropDownField" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblName" runat="server" EnableViewState="false" CssClass="FieldLabel"
                        ResourceString="General.CodeName" DisplayColon="true" />
                </td>
                <td style="width: 100%;">
                    <cms:CMSTextBox ID="txtName" runat="server" CssClass="TextBoxField" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <cms:ExtendedTextArea Visible="false" ID="txtAreaContent" runat="server" Height="330px"
                        TextMode="MultiLine" Width="720px" />
                    <cms:CMSHtmlEditor Visible="false" ID="htmlContent" runat="server" Width="720px"
                        Height="350px" />
                    <cms:CMSEditableImage Visible="false" ID="imageContent" runat="server" Width="720"
                        ImageWidth="720" IsLiveSite="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblContent" runat="server" EnableViewState="false" CssClass="FieldLabel"
                        ResourceString="header.content" DisplayColon="true" Visible="false" />
                </td>
                <td>
                    <cms:CMSTextBox ID="txtContent" runat="server" Visible="false" CssClass="TextBoxField" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <script type="text/javascript">
        //<![CDATA[
        var mainUrl = '';

        // Selects specified node
        function SelectNode(nodeName, nodeType) {
            document.location.replace(mainUrl + "&nodename=" + nodeName + "&nodetype=" + nodeType);
        }

        // Selects specified after saving
        function SelectNodeAfterImageSave(nodeName, nodeType) {
            document.location.replace(mainUrl + "&nodename=" + nodeName + "&nodetype=" + nodeType + "&imagesaved=true");
        }

        // Refreshes after saving
        function RefreshNode(nodeName, nodeType, nodeId) {
            parent.frames['tree'].RefreshNode(nodeName, nodeType, nodeId);
        }

        // Opens menu for creating new item
        function CreateNew(nodetype) {
            document.location.replace(mainUrl + "&createNew=true&nodetype=" + nodetype);
        }
        //]]>
    </script>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
