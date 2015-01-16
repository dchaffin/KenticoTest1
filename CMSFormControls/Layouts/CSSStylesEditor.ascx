<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CSSStylesEditor.ascx.cs"
    Inherits="CMSFormControls_Layouts_CSSStylesEditor" %>
<div class="CssStylesEditorWrapperClass">
    <asp:Panel runat="server" ID="pnlWrapper">
        <asp:PlaceHolder runat="server" ID="plcCssLink">
            <asp:Panel runat="server" ID="pnlLink">
                <cms:LocalizedLinkButton runat="server" ID="lnkStyles" EnableViewState="false" ResourceString="general.addcss" />
            </asp:Panel>
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="pnlEditCSS">
            <cms:LocalizedLabel runat="server" ID="lblLayoutCSS" EnableViewState="false" ResourceString="templatedesigner.section.styles"
                DisplayColon="true" /><br />
            <cms:ExtendedTextArea ID="txtLayoutCSS" runat="server" EditorMode="Advanced" Language="CSS"
                Width="98%" Height="200px" />
        </asp:Panel>
    </asp:Panel>
</div>
