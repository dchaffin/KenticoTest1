<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_PublicStatus_PublicStatus_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Public status - List"
    CodeFile="PublicStatus_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="gridElem" GridName="PublicStatus_List.xml" OrderBy="PublicStatusDisplayName"
        IsLiveSite="false" Columns="PublicStatusID,PublicStatusDisplayName,PublicStatusEnabled" />
</asp:Content>
