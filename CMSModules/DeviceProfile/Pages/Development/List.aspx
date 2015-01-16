<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    CodeFile="List.aspx.cs" Inherits="CMSModules_DeviceProfile_Pages_Development_List"
    Theme="Default" Title="Device profiles - List" %>

<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSModules/DeviceProfile/Controls/List.ascx" TagName="ProfilesList"
    TagPrefix="cms" %>
<asp:Content ID="Content6" ContentPlaceHolderID="plcContent" runat="server">
    <cms:DisabledModule runat="server" ID="ucDisabledModule" />
    <cms:ProfilesList ID="listElem" runat="server" />
</asp:Content>
