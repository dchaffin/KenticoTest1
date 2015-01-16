<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Theme="Default"
    AutoEventWireup="true" CodeFile="Workflow_Step_Footer.aspx.cs" Inherits="CMSModules_Workflows_Workflow_Step_Footer"
    Title="Workflow Edit - Step Footer" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <div class="PageFooterLine">
        <div class="FloatRight">
            <cms:LocalizedButton ID="btnClose" runat="server" CssClass="SubmitButton" Text="{$general.close$}" />
        </div>
    </div>
</asp:Content>
