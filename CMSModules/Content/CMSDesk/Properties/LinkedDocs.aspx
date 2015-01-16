<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_LinkedDocs"
    Theme="Default" CodeFile="LinkedDocs.aspx.cs" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu id="menuElem" runat="server" handleworkflow="false" showsave="false"
        helptopicname="linked_docs" islivesite="false" />
    <cms:cmsdocumentpanel id="pnlDocInfo" runat="server" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" />
    <asp:Panel ID="pnlContainer" runat="server">
        <cms:unigrid id="gridDocs" runat="server" gridname="LinkedDocs.xml" islivesite="false"
            exportfilename="cms_document" />
    </asp:Panel>
    <script type="text/javascript">
        //<![CDATA[
        // Select item action
        function SelectItem(nodeId, parentNodeId) {
            if (nodeId != 0) {
                parent.SelectNode(nodeId);
                parent.RefreshTree(parentNodeId, nodeId);
            }
        }
        //]]>
    </script>
</asp:Content>
