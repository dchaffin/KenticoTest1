<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_HeaderTab"
    Theme="Default" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Page Template Edit - Header" CodeFile="PageTemplate_HeaderTab.aspx.cs" EnableEventValidation="false" %>

<%@ Register Assembly="CMS.UIControls" Namespace="CMS.UIControls" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ObjectLockingPanel runat="server" ID="pnlObjectLocking">
        <asp:Literal ID="ltlScript" runat="server" />
        <table style="width: 100%">
            <tr>
                <td style="vertical-align: top;">
                    <asp:Label ID="lblTemplateHeader" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:ExtendedTextArea ID="txtTemplateHeader" runat="server" EnableViewState="false"
                        EditorMode="Advanced" Width="98%" Height="270" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:LocalizedCheckBox runat="server" ID="chkAllowInherit" ResourceString="template.allowheaderinherit" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:LocalizedCheckBox runat="server" ID="chkInheritParent" ResourceString="template.inheritheaderparent" />
                </td>
            </tr>
        </table>
    </cms:ObjectLockingPanel>
</asp:Content>
