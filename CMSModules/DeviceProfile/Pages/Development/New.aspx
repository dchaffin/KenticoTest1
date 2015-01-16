<%@ Page Title="Device profiles - Edit" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="true" CodeFile="New.aspx.cs" Theme="Default" Inherits="CMSModules_DeviceProfile_Pages_Development_New" %>

<%@ Register Src="~/CMSModules/DeviceProfile/Controls/Edit.ascx" TagName="ProfilesEdit"
    TagPrefix="cms" %>
<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="server">
    <cms:ProfilesEdit ID="editElem" runat="server" />
</asp:Content>
