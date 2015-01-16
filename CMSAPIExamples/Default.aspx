<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Theme="Default"
    Inherits="CMSAPIExamples_Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Title="CMS API Examples" %>

<%@ Register Src="~/CMSAdminControls/UI/Header.ascx" TagName="Header" TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="paneHeader" runat="server" Direction="North" RenderAs="Div"
                Resizable="False">
                <Template>
                    <cms:Header runat="server" ID="headerElem" IsAPIExampleHeader="True" />
                </Template>
            </cms:UILayoutPane>
            <cms:UILayoutPane ID="cmsdesktop" runat="server" Direction="Center" RenderAs="Iframe" />
        </Panes>
    </cms:UILayout>
</asp:Content>
