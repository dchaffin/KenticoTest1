<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSWebParts/Newsletters/CustomSubscriptionForm.ascx.cs" Inherits="CMSWebParts_Newsletters_CustomSubscriptionForm" %>
<%@ Register Src="~/CMSFormControls/Captcha/SecurityCode.ascx" TagName="SecurityCode" TagPrefix="cms" %>
<asp:Label ID="lblError" runat="server" EnableViewState="false" Visible="false" />
<asp:Label ID="lblInfo" runat="server" EnableViewState="false" Visible="false" />
<asp:Panel ID="pnlSubscription" runat="server" DefaultButton="btnSubmit" CssClass="Subscription">
    <cms:DataForm ID="formElem" runat="server" />
    <asp:PlaceHolder runat="server" ID="plcNwsList">
        <div class="NewsletterList">
            <asp:CheckBoxList runat="server" ID="chklNewsletters" CssClass="NewsletterList" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcCaptcha">
        <div class="CaptchaLabel">
            <cms:LocalizedLabel ID="lblCaptcha" runat="server" EnableViewState="false" />
        </div>
        <div class="Captcha">
            <cms:SecurityCode ID="scCaptcha" GenerateNumberEveryTime="false" runat="server" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="pnlButtonSubmit" runat="server">
        <cms:LocalizedButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" CssClass="SubscriptionButton"
            EnableViewState="false" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="pnlImageSubmit" runat="server">
        <asp:ImageButton ID="btnImageSubmit" runat="server" OnClick="btnSubmit_Click" EnableViewState="false" />
    </asp:PlaceHolder>
</asp:Panel>