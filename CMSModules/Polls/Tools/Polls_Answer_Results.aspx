<%@ Page Language="C#" AutoEventWireup="true"
 CodeFile="Polls_Answer_Results.aspx.cs" Inherits="CMSModules_Polls_Tools_Polls_Answer_Results"
 MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" Title="Answer Results" Theme="default" EnableEventValidation="false" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
<div class="PageContent">
    <asp:PlaceHolder ID="plcBizForm" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnClose" runat="server" CssClass="SubmitButton" EnableViewState="false"
            OnClientClick="return CloseDialog();" ResourceString="general.close" />
    </div>
</asp:Content>