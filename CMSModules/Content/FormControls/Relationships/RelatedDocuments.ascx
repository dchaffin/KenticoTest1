<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RelatedDocuments.ascx.cs"
    Inherits="CMSModules_Content_FormControls_Relationships_RelatedDocuments" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<div class="RelatedDocuments">
    <asp:Panel ID="pnlNewLink" runat="server" Style="margin-bottom: 5px;">
        <cms:CMSImage ID="imgNewRelationship" runat="server" ImageAlign="AbsMiddle" CssClass="NewItemImage"
            EnableViewState="false" /><cms:LocalizedHyperlink ID="lnkNewRelationship" runat="server" EnableViewState="false"
            ResourceString="Relationship.AddRelatedDocument" />
    </asp:Panel>
    <div>
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
                <cms:UniGrid ID="UniGridRelationship" runat="server" GridName="~/CMSModules/Content/FormControls/Relationships/RelatedDocuments_List.xml"
                    OrderBy="RelationshipDisplayName" IsLiveSite="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
    <asp:HiddenField ID="hdnSelectedNodeId" runat="server" Value="" />
</div>
