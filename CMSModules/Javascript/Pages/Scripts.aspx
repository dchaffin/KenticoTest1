<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Javascript_Pages_Scripts" EnableEventValidation="false"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="Scripts.aspx.cs" %>

<%@ Register Src="~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/FileSystemSelector.ascx"
    TagName="FileSystemSelector" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cms:FileSystemSelector ID="selFile" ShortID="s" runat="server" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
