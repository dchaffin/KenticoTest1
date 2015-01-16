<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_Newsletters_Tools_Subscribers_Subscriber_General"
    Theme="Default" Title="Tools - Newsletter subscriber edit" CodeFile="Subscriber_General.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:DataForm ID="formCustomFields" runat="server" ClassName="newsletter.subscriber"
        IsLiveSite="false" />
</asp:Content>
