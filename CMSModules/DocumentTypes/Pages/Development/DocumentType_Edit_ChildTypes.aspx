<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_ChildTypes"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Document Type Edit - Child Types"
    CodeFile="DocumentType_Edit_ChildTypes.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <table style="width: 100%">
        <tr>
            <td class="ColumnPanel">
                <asp:Panel ID="pnlChildren" runat="server">
                    <cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" ObjectType="cms.documenttype"
                        SelectionMode="Multiple" />
                </asp:Panel>
            </td>
            <td>
                &nbsp;
            </td>
            <td class="ColumnPanel">
                <asp:Panel ID="pnlParents" runat="server">
                    <cms:UniSelector ID="selParent" runat="server" IsLiveSite="false" ObjectType="cms.documenttype"
                        SelectionMode="Multiple" />
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
