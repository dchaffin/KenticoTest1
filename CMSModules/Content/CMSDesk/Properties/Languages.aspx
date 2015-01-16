<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_Languages"
    Title="Languages" ValidateRequest="false" Theme="Default" CodeFile="Languages.aspx.cs"
    MaintainScrollPositionOnPostback="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" HandleWorkflow="false" ShowSave="false"
        HelpTopicName="doc_languages" IsLiveSite="false" />
    <cms:CMSDocumentPanel ID="pnlDocInfo" runat="server" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">
        //<![CDATA[
        // Redirect item
        function RedirectItem(nodeId, culture) {
            if (nodeId != 0) {
                if (parent != null) {
                    if (parent.parent != null) {
                        if (parent.parent.parent != null) {
                            if (parent.parent.parent.parent != null) {
                                parent.parent.parent.parent.location.href = "../../../../CMSDesk/default.aspx?section=content&action=edit&mode=editform&nodeid=" + nodeId + "&culture=" + culture;
                            }
                        }
                    }
                }
            }
        }
        //]]>
    </script>
    <asp:Panel ID="pnlContainer" runat="server">
        <cms:UniGrid ID="gridLanguages" runat="server" GridName="Languages.xml" GridLines="Horizontal"
            IsLiveSite="false" ExportFileName="cms_document" />
    </asp:Panel>
</asp:Content>
