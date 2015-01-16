<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Content_CMSDesk_PublishArchive" Title="Publishes or archives multiple documents"
    ValidateRequest="false" Theme="Default" CodeFile="PublishArchive.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:asyncbackground id="backgroundElem" runat="server" />
        <div class="AsyncLogArea">
            <div>
                <asp:Panel ID="pnlAsyncBody" runat="server" CssClass="PageBody">
                    <asp:Panel ID="pnlTitleAsync" runat="server" CssClass="PageHeader">
                        <cms:pagetitle id="titleElemAsync" runat="server" />
                    </asp:Panel>
                    <asp:Panel ID="pnlCancel" runat="server" CssClass="PageHeaderLine">
                        <cms:localizedbutton runat="server" id="btnCancel" resourcestring="General.Cancel"
                            cssclass="SubmitButton" />
                    </asp:Panel>
                    <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                        <cms:asynccontrol id="ctlAsync" runat="server" />
                    </asp:Panel>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server" EnableViewState="false">
    <asp:Panel runat="server" ID="pnlContent" CssClass="PageContent" EnableViewState="false">
        <asp:Panel ID="pnlPublish" runat="server" EnableViewState="false">
            <cms:localizedlabel id="lblQuestion" runat="server" cssclass="ContentLabel" enableviewstate="false"
                font-bold="true" />
            <br />
            <br id="brSeparator" runat="server" />
            <asp:Panel ID="pnlDocList" runat="server" Visible="false" CssClass="ScrollableList"
                EnableViewState="false">
                <asp:Label ID="lblDocuments" runat="server" CssClass="ContentLabel" EnableViewState="false" />
            </asp:Panel>
            <br />
            <asp:PlaceHolder ID="plcCheck" runat="server" EnableViewState="false">
                <asp:PlaceHolder ID="plcAllCultures" runat="server">
                    <cms:localizedcheckbox id="chkAllCultures" runat="server" cssclass="ContentCheckbox"
                        enableviewstate="false" checked="true" />
                </asp:PlaceHolder>
                <cms:localizedcheckbox id="chkUnderlying" runat="server" cssclass="ContentCheckbox"
                    enableviewstate="false" checked="true" />
                <asp:PlaceHolder ID="plcUndoCheckOut" runat="server">
                    <cms:localizedcheckbox id="chkUndoCheckOut" runat="server" cssclass="ContentCheckbox"
                        enableviewstate="false" resourcestring="content.undocheckedoutdocs" />
                </asp:PlaceHolder>
            </asp:PlaceHolder>
            <br />
            <br />
            <cms:localizedbutton id="btnOk" runat="server" cssclass="SubmitButton" onclick="btnOK_Click"
                resourcestring="general.yes" enableviewstate="false" />
            <cms:localizedbutton id="btnNo" runat="server" cssclass="SubmitButton" onclick="btnNo_Click"
                resourcestring="general.no" enableviewstate="false" />
        </asp:Panel>
    </asp:Panel>
    <script type="text/javascript">
        //<![CDATA[

        // Display the document
        function SelectNode(nodeId) {
            if (parent != null) {
                if (parent.SelectNode != null) {
                    parent.SelectNode(nodeId);
                }
                RefreshTree(nodeId);
            }
        }

        function RefreshTree(nodeId) {
            if (parent != null) {
                if (parent.RefreshTree != null) {
                    parent.RefreshTree(nodeId, nodeId);
                }
            }

        }
        //]]>                        
    </script>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
