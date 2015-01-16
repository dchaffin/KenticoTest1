<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_InternalStatus_InternalStatus_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Internal status - List"
    CodeFile="InternalStatus_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="gridElem" GridName="InternalStatus_List.xml" OrderBy="InternalStatusDisplayName"
        IsLiveSite="false" Columns="InternalStatusID, InternalStatusDisplayName, InternalStatusEnabled" />
</asp:Content>
