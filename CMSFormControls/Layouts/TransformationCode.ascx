<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TransformationCode.ascx.cs"
    Inherits="CMSFormControls_Layouts_TransformationCode" %>
<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<script type="text/javascript" language="javascript">
    function AddCssHandlerScript() {
        document.getElementById('editCss').style.display = 'block';
        document.getElementById('cssLink').style.display = 'none';
        if (window.ResizeHeaderButtons != null) {
            ResizeHeaderButtons();
        }
        return false;
    }
</script>
<div class="LayoutCodeControl">
    <div class="LayoutControlHeaderInfo">
        <div class="FloatLeft">
            <cms:LocalizedLabel runat="server" ID="lblType" ResourceString="DocumentType_Edit_Transformation_Edit.TransformType" />&nbsp;
            <asp:DropDownList runat="server" ID="drpType" AutoPostBack="true" OnSelectedIndexChanged="drpTransformationType_SelectedIndexChanged"
                EnableViewState="true" CssClass="ShortDropDownList" />
        </div>
        <div class="FloatRight" style="padding-top: 4px;">
            <cms:LocalizedHyperlink ID="lnkHelp" runat="server" EnableViewState="false" />
        </div>
        <div class="FloatRight">
            <cms:Help ID="helpElem" runat="server" EnableViewState="false" />
        </div>
        <div class="ClearBoth">
        </div>
    </div>
    <asp:Panel runat="server" ID="pnlDirectives" CssClass="NORTL CodeDirectives">
        <asp:Label runat="server" ID="lblDirectives" EnableViewState="false" />
    </asp:Panel>
    <cms:CMSHtmlEditor runat="server" ID="tbWysiwyg" Width="99%" Height="300" Visible="false" />
    <cms:MacroEditor runat="server" ID="txtCode" ShortID="e" Visible="false" />
</div>
<div class="LayoutCSSControl">
    <asp:PlaceHolder runat="server" ID="plcCssLink">
        <div id="cssLink">
            <cms:LocalizedLinkButton runat="server" ID="lnkStyles" EnableViewState="false" ResourceString="general.addcss"
                OnClientClick="return AddCssHandlerScript();" />
        </div>
    </asp:PlaceHolder>
    <div id="editCss" style="<%=(plcCssLink.Visible ? "display: none": "")%>">
        <cms:LocalizedLabel runat="server" ID="lblCSS" ResourceString="Container_Edit.ContainerCSS"
            DisplayColon="true" EnableViewState="false" />
        <cms:ExtendedTextArea ID="txtCSS" runat="server" EnableViewState="false" EditorMode="Advanced"
            Language="CSS" Width="99%" Height="200px" />
    </div>
</div>
