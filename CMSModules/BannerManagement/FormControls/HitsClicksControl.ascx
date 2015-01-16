<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HitsClicksControl.ascx.cs" Inherits="CMSModules_BannerManagement_FormControls_HitsClicksControl" %>

<cms:CMSUpdatePanel runat="server" ID="updatePanel" UpdateMode="Conditional">
    <ContentTemplate>     
        <div>
        <cms:LocalizedRadioButton ID="radUnlimited" runat="server" GroupName="NumberGroup" AutoPostBack="True" />
        </div>
        <div>
        <cms:LocalizedRadioButton ID="radAllowSpecific" runat="server" GroupName="NumberGroup" AutoPostBack="True" />
        </div>
        <asp:Panel ID="pnlSpecific" runat="server">
            <asp:TextBox ID="txtNumberLeft" runat="server" style="margin-left: 20px;"></asp:TextBox>
            <cms:LocalizedButton ID="btnAdd" runat="server" CssClass="ContentButton" EnableViewState="false" ResourceString="general.add" />
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
