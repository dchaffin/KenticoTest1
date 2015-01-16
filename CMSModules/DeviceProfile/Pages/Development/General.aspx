<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    CodeFile="General.aspx.cs" Inherits="CMSModules_DeviceProfile_Pages_Development_General"
    Theme="Default" Title="Device profiles - Edit" %>

<%@ Register Src="~/CMSModules/DeviceProfile/Controls/Edit.ascx" TagName="ProfilesEdit"
    TagPrefix="cms" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="server">
    <cms:ProfilesEdit ID="editElem" runat="server" />
</asp:Content>
