<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_System_Deployment"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - Deployment"
    CodeFile="System_Deployment.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncBackground ID="backgroundElem" runat="server" />
        <div class="AsyncLogArea">
            <div>
                <asp:Panel ID="pnlAsyncBody" runat="server" CssClass="PageBody">
                    <asp:Panel ID="pnlTitleAsync" runat="server" CssClass="PageHeader">
                        <cms:PageTitle ID="titleElemAsync" runat="server" SetWindowTitle="false" />
                    </asp:Panel>
                    <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                        <cms:AsyncControl ID="ctlAsync" runat="server" />
                    </asp:Panel>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" GroupingText="Deployment" ID="pnlDeployment">
        <asp:Label runat="server" ID="lblDeploymentInfo" EnableViewState="false" />
        <div style="padding: 10px 0px 10px 0px;">
            <cms:LocalizedButton ID="btnSaveAll" CssClass="XXXLongSubmitButton" runat="server"
                OnClick="btnSaveAll_Click" ResourceString="Deployment.SaveAll" EnableViewState="false" />
        </div>
    </asp:Panel>
    <br />
    <asp:Panel runat="server" GroupingText="Source control" ID="pnlSourceControl">
        <div style="padding-bottom: 10px">
            <asp:Label runat="server" ID="lblSourceControlInfo" EnableViewState="false" />
        </div>
        <div class="SourceControlPanelItem">
            <cms:LocalizedCheckBox ID="chkSaveLayouts" ResourceString="Deployment.SaveLayouts"
                runat="server" />
        </div>
        <div class="SourceControlPanelItem">
            <cms:LocalizedCheckBox ID="chkSavePageTemplate" ResourceString="Deployment.SavePageTemplates"
                runat="server" />
        </div>
        <div class="SourceControlPanelItem">
            <cms:LocalizedCheckBox ID="chkSaveTransformation" ResourceString="Deployment.SaveTansformations"
                runat="server" />
        </div>
        <div class="SourceControlPanelItem">
            <cms:LocalizedCheckBox ID="chkSaveWebpartLayout" ResourceString="Deployment.SaveWebpartLayouts"
                runat="server" />
        </div>
        <div class="SourceControlPanelItem">
            <cms:LocalizedCheckBox ID="chkSaveWebpartContainer" ResourceString="Deployment.SaveWebpartContainers"
                runat="server" />
        </div>
        <div class="SourceControlPanelItem">
            <cms:LocalizedCheckBox ID="chkSaveCSS" ResourceString="Deployment.SaveCSS" runat="server"
                EnableViewState="false" />
        </div>
        <div style="padding-top: 10px">
            <cms:LocalizedLabel runat="server" ID="lblApplyChanges" EnableViewState="false" ResourceString="Deployment.ApplyChangesInfo" />&nbsp;<cms:LocalizedLabel
                runat="server" ID="lblSynchronization" EnableViewState="false" ResourceString="Deployment.SynchronizeChangesInfo"
                Visible="false" />
        </div>
        <div style="padding: 10px 0px 10px 0px;">
            <cms:LocalizedButton ID="btnSourceControl" CssClass="XXXLongSubmitButton" runat="server"
                OnClick="btnSourceControl_Click" ResourceString="Deployment.SourceControlStore"
                EnableViewState="false" />&nbsp;<cms:LocalizedButton runat="server" ID="btnSynchronize" OnClick="btnSynchronize_Click"
                    Visible="false" EnableViewState="false" CssClass="XXXLongSubmitButton" ResourceString="Deployment.SynchronizeChanges" />
        </div>
    </asp:Panel>
</asp:Content>
