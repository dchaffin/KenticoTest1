<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BannerContentEdit.ascx.cs" Inherits="CMSModules_BannerManagement_FormControls_BannerContentEdit" %>

<%@ Register Src="~/CMSFormControls/Macros/MacroEditor.ascx" TagName="MacroEditor" TagPrefix="cms" %>

<asp:PlaceHolder ID="plcHtml" runat="server" Visible="false">
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblHtmlContent" runat="server" ResourceString="banner.bannernew.content" DisplayColon="true" EnableViewState="false" CssClass="EditingFormLabel"
                AssociatedControlID="htmlBanner" />
        </td>
        <td style="width: 700px;">
            <cms:CMSHtmlEditor ID="htmlBanner" runat="server" CssClass="HtmlEditor" ToolbarSet="Full" Width="700px" Height="350px" />
        </td>
    </tr>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcPlain" runat="server" Visible="false">
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblPlainContent" runat="server" ResourceString="banner.bannernew.content" DisplayColon="true" EnableViewState="false" CssClass="EditingFormLabel"
                AssociatedControlID="macroEditorPlain" />
        </td>
        <td style="width: 700px;">
            <cms:MacroEditor runat="server" ID="macroEditorPlain" />
        </td>
    </tr>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcImage" runat="server">
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblImageContent" runat="server" ResourceString="banner.bannernew.image" DisplayColon="true" EnableViewState="false" CssClass="EditingFormLabel"
                AssociatedControlID="imsImage" />
        </td>
        <td>
            <cms:ImageSelector ID="imsImage" runat="server" CssClass="ImageSelector" IsLiveSite="false" UseImagePath="true" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblImgTitle" runat="server" ResourceString="banner.bannernew.imgtitle" DisplayColon="true" EnableViewState="false" CssClass="EditingFormLabel"
                AssociatedControlID="txtImgTitle" />
        </td>
        <td>
            <cms:CMSTextBox ID="txtImgTitle" runat="server" MaxLength="2000" CssClass="TextBoxField" EnableViewState="true" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblImgAlt" runat="server" ResourceString="banner.bannernew.imgalt" DisplayColon="true" EnableViewState="false" CssClass="EditingFormLabel"
                AssociatedControlID="txtImgAlt" />
        </td>
        <td>
            <cms:CMSTextBox ID="txtImgAlt" runat="server" MaxLength="2000" CssClass="TextBoxField" EnableViewState="true" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblImgClass" runat="server" ResourceString="banner.bannernew.imgclass" DisplayColon="true" EnableViewState="false" CssClass="EditingFormLabel"
                AssociatedControlID="txtImgClass" />
        </td>
        <td>
            <cms:CMSTextBox ID="txtImgClass" runat="server" MaxLength="2000" CssClass="TextBoxField" EnableViewState="true" />
        </td>
    </tr>
    <tr>
        <td>
            <cms:LocalizedLabel ID="lblImgStyle" runat="server" ResourceString="banner.bannernew.imgstyle" DisplayColon="true" EnableViewState="false" CssClass="EditingFormLabel"
                AssociatedControlID="txtImgStyle" />
        </td>
        <td>
            <cms:CMSTextBox ID="txtImgStyle" runat="server" MaxLength="2000" CssClass="TextBoxField" EnableViewState="true" />
        </td>
    </tr>
</asp:PlaceHolder>
