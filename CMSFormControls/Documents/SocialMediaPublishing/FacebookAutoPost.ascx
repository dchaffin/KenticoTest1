<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Documents_SocialMediaPublishing_FacebookAutoPost" CodeFile="FacebookAutoPost.ascx.cs" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
<ContentTemplate>
    
    <fieldset>
        <legend><cms:LocalizedLabel runat="server" ResourceString="socialnetworking.facebook.postat"></cms:LocalizedLabel></legend>    
        <cms:LocalizedLabel ID="lblFacebookMissingSetting" runat="server" CssClass="AdvancedLabel Warn" EnableViewState="false"
            ResourceString="socialnetworking.facebook.missingsetting" Visible="false" style="position: static;"></cms:LocalizedLabel>
        <cms:LocalizedLabel ID="lblExpirationWarning" runat="server" CssClass="AdvancedLabel Warn" EnableViewState="false"
            ResourceString="socialnetworking.facebook.gotosettings" Visible="false" style="position: static;"></cms:LocalizedLabel>
        <cms:LocalizedLabel ID="lblAccessDeniedWarning" runat="server" CssClass="AdvancedLabel Warn" EnableViewState="false"
            ResourceString="socialnetworking.facebook.accessdenied" Visible="false" style="position: static;"></cms:LocalizedLabel>
        <div>
            <asp:TextBox ID="txtTemplateFacebook" runat="server" TextMode="MultiLine" 
                Height="100px" Width="100%" ></asp:TextBox>
        </div>
        <div style="padding-top:7px;">
            <div class="LeftAlign">
                <cms:LocalizedCheckBox ID="chkEnabledFacebook" runat="server" ResourceString="socialnetworking.facebook.postafterpublish" />
                <div style="padding-top:5px;">
                    <asp:Label ID="lblFacebookIsPublished" runat="server" Visible="true" EnableViewState="false" CssClass="InfoLabel"></asp:Label>
                    <asp:HyperLink ID="lnkFacebookPostURL" runat="server" Visible="false" EnableViewState="false"></asp:HyperLink>
                </div>
            </div>
            <div class="RightAlign">
                <asp:Button ID="btnPostAtFacebook" runat="server" onclick="btnPostAtFacebook_Click" CssClass="LongButton" />
            </div>
            <div class="ClearBoth"></div>
        </div>
    </fieldset>
</ContentTemplate>
</cms:CMSUpdatePanel>