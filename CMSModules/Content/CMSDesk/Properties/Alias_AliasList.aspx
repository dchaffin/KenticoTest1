<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Alias_AliasList.aspx.cs"
    Inherits="CMSModules_Content_CMSDesk_Properties_Alias_AliasList" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<asp:Content runat="server" ID="cntServer" ContentPlaceHolderID="plcContent">
    <div class="PageContent">
        <cms:UniGrid ID="ugAlias" runat="server" IsLiveSite="false" OrderBy="AliasUrlPath" ShowActionsMenu="true">
            <GridActions Parameters="AliasID">
                <ug:Action Name="edit" Caption="$General.edit$" Icon="Edit.png" CommandArgument="AliasID" />
                <ug:Action Name="delete" Caption="$General.Delete$" Icon="Delete.png" Confirmation="$General.ConfirmDelete$"
                    CommandArgument="AliasID" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="AliasURLPath" Caption="$list.urlalias$" Wrap="false">
                    <Filter Type="text" Source="AliasURLPath" />
                </ug:Column>
                <ug:Column Source="##ALL##" Caption="$general.documentname$" Wrap="false" Sort="NodeName"
                    ExternalSourceName="documentName">
                    <Filter Type="text" Source="NodeName" />
                </ug:Column>
                <ug:Column Source="AliasExtensions" Caption="$doc.urls.urlextensions$" Wrap="false" />
                <ug:Column Source="CampaignDisplayName" Caption="$doc.urls.trackcampaign$" Wrap="false" />
                <ug:Column Source="DocumentCulture" Caption="$general.language$" Wrap="false" Width="100%"
                    ExternalSourceName="culture" Sort="AliasCulture" />
            </GridColumns>
            <GridOptions DisplayFilter="true" FilterLimit="0" />
        </cms:UniGrid>
    </div>
</asp:Content>
<asp:Content runat="server" ID="cntFooter" ContentPlaceHolderID="plcFooter">
    <div id="divFooter" runat="server" class="AlignRight" >
        <cms:CMSButton ID="btnClose" runat="server" OnClientClick="return CloseDialog();"
            CssClass="SubmitButton" />
    </div>
</asp:Content>
