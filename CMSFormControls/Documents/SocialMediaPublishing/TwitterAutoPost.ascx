<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Documents_SocialMediaPublishing_TwitterAutoPost" CodeFile="TwitterAutoPost.ascx.cs" %>

<script type="text/javascript" language="javascript">
    //<![CDATA[

    function countCharactersTwitter(txt, lbl, maxvalue) {
        var length = txt.val().length;
        lbl.text(length + "/" + maxvalue);
        if (length > maxvalue) {
            lbl.css("color", "red");
        }
        else {
            lbl.css("color", "");
        }
    }

    function countCharactersTwitterInit(textareaID, labelID, maxVal) {
        var txt = jQuery("#" + textareaID);
        var lbl = jQuery("#" + labelID);
        lbl.css("line-height", "22px");
        txt.bind('keyup', function () {
            countCharactersTwitter(txt, lbl, maxVal);
        });

    }
    //]]>
</script>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
<ContentTemplate>    
    <fieldset>
        <legend><cms:LocalizedLabel runat="server" ResourceString="socialnetworking.twitter.postat"></cms:LocalizedLabel></legend>
        <cms:LocalizedLabel ID="lblTwitterMissingSetting" runat="server" CssClass="AdvancedLabel Warn" ResourceString="socialnetworking.twitter.missingsetting" Visible="false" style="position: static;"></cms:LocalizedLabel>
        <cms:LocalizedLabel ID="lblAccessDeniedWarning" runat="server" CssClass="AdvancedLabel Warn" EnableViewState="false"
            ResourceString="socialnetworking.twitter.accessdenied" Visible="false" style="position: static;"></cms:LocalizedLabel>
        <div>
            <asp:TextBox ID="txtTemplateTwitter" runat="server" TextMode="MultiLine" 
                Height="100px" Width="100%" ></asp:TextBox>
        </div>
        <div style="padding-top:7px;">
            <div class="LeftAlign">
                <cms:LocalizedCheckBox ID="chkEnabledTwitter" runat="server" ResourceString="socialnetworking.twitter.postafterpublish" />            
                <div style="padding-top:5px;">
                    <asp:Label ID="lblTwitterIsPublished" runat="server" Visible="true" EnableViewState="true" CssClass="InfoLabel"></asp:Label>
                    <asp:HyperLink ID="lnkTwitterPostURL" runat="server" Visible="false" EnableViewState="true"></asp:HyperLink>
                </div>
            </div>
            <div class="RightAlign">
                <asp:Label ID="lblCharactersCount" runat="server" Height="22px"></asp:Label>
                <asp:Button ID="btnPostAtTwitter" runat="server"  onclick="btnPostAtTwitter_Click" CssClass="LongButton" />
            </div>
            <div class="ClearBoth"></div>
        </div>
    </fieldset>
</ContentTemplate>
</cms:CMSUpdatePanel>