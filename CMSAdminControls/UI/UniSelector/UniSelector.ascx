<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_UI_UniSelector_UniSelector"
    CodeFile="UniSelector.ascx.cs" %>
<%@ Register Assembly="CMS.ExtendedControls" Namespace="CMS.ExtendedControls" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register src="~/CMSAdminControls/UI/UniControls/UniButton.ascx" tagname="UniButton" tagprefix="cms" %>
<asp:Literal ID="ltlContentBefore" runat="server" EnableViewState="false" />
<asp:Label ID="lblStatus" runat="server" EnableViewState="False" CssClass="InfoLabel" />
<asp:PlaceHolder runat="server" ID="plcButtonSelect" Visible="false" EnableViewState="false">
    <cms:LocalizedButton runat="server" ID="btnDialog" CssClass="ContentButton" /><cms:LocalizedLabel
        ID="lblDialog" runat="server" EnableViewState="false" AssociatedControlID="btnDialog"
        Display="false" />
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="plcImageSelect" Visible="false" EnableViewState="false">
    <asp:HyperLink runat="server" ID="lnkDialog" CssClass="ActionLink">
        <asp:Image runat="server" ID="imgDialog" CssClass="ActionImage" />
        <cms:LocalizedLabel ID="lblLinkDialog" CssClass="Text" runat="server" />
    </asp:HyperLink></asp:PlaceHolder><asp:PlaceHolder runat="server" ID="plcTextBoxSelect" Visible="false">
    <cms:CMSTextBox ID="txtSingleSelect" runat="server" CssClass="SelectorTextBox" ReadOnly="true" /><cms:LocalizedLabel
        ID="lblSingleSelectTxt" runat="server" EnableViewState="false" AssociatedControlID="txtSingleSelect"
        Display="false" /><cms:UniButton ID="btnSelect" runat="server" CssClass="ContentButton"
            EnableViewState="False" ImageCssClass="SelectorImageButton" /><cms:LocalizedLabel ID="lblSelect" runat="server" EnableViewState="false"
                AssociatedControlID="btnSelect" Display="false" /><cms:UniButton ID="btnEdit"
                    runat="server" CssClass="ContentButton" Visible="false" EnableViewState="False" /><cms:LocalizedLabel
                        ID="lblEdit" runat="server" EnableViewState="false" AssociatedControlID="btnEdit"
                        Display="false" Visible="false" /><cms:UniButton ID="btnNew" runat="server"
                            CssClass="ShortButton" Visible="false" EnableViewState="False" ImageCssClass="SelectorImageButton" /><cms:LocalizedLabel
                                ID="lblNew" runat="server" EnableViewState="false" AssociatedControlID="btnNew"
                                Display="false" Visible="false" /><cms:UniButton ID="btnClear" runat="server"
                                    CssClass="ContentButton" EnableViewState="False" ImageCssClass="SelectorImageButton" /><cms:LocalizedLabel ID="lblClear"
                                        runat="server" EnableViewState="false" AssociatedControlID="btnClear" Display="false" />
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="plcDropdownSelect" Visible="false">
    <cms:ExtendedDropDownList SaveItemAttributesToViewState="true" ID="drpSingleSelect"
        runat="server" CssClass="DropDownField" />
    <cms:LocalizedLabel ID="lblSingleSelectDrp" runat="server" EnableViewState="false"
        AssociatedControlID="drpSingleSelect" Display="false" /><cms:UniButton runat="server"
            ID="btnDropEdit" EnableViewState="false" Visible="false" CssClass="ContentButton"
            RenderScript="true" ImageCssClass="SelectorImageButton" /><cms:LocalizedLabel ID="lblDropEdit" runat="server" EnableViewState="false"
                AssociatedControlID="btnDropEdit" Display="false" Visible="false" /><cms:UniButton
                    ID="btnDropNew" runat="server" CssClass="ShortButton" Visible="false" EnableViewState="False" ImageCssClass="SelectorImageButton" /><cms:LocalizedLabel
                        ID="lblDropNew" runat="server" EnableViewState="false" AssociatedControlID="btnDropNew"
                        Display="false" Visible="false" /></asp:PlaceHolder>
<asp:Panel runat="server" ID="pnlGrid" CssClass="UniSelector" Visible="false">
    <asp:PlaceHolder ID="plcContextMenu" runat="server" EnableViewState="false" />
    <cms:UniGrid ID="uniGrid" ShortID="g" runat="server" ZeroRowsText="" ShowObjectMenu="false"
        ShowActionsMenu="false" RememberState="false" />
    <div id="UniSelectorSpacer" runat="server" class="UniSelectorSpacer">
    </div>
    <cms:LocalizedButton ID="btnRemoveSelected" runat="server" CssClass="LongButton"
        EnableViewState="False" OnClick="btnRemoveSelected_Click" /><cms:LocalizedLabel ID="lblRemoveSelected"
            runat="server" EnableViewState="false" AssociatedControlID="btnRemoveSelected"
            Display="false" /><cms:LocalizedButton ID="btnAddItems" runat="server" CssClass="LongButton"
                EnableViewState="False" /><cms:LocalizedLabel ID="lblAddItems" runat="server" EnableViewState="false"
                    AssociatedControlID="btnAddItems" Display="false" />
    <cms:ContextMenuButton runat="server" ID="btnMenu" />
</asp:Panel>
<asp:Literal ID="ltlContentAfter" runat="server" EnableViewState="false" />
<asp:HiddenField ID="hdnDialogSelect" runat="server" EnableViewState="false" />
<asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />
<asp:HiddenField ID="hiddenField" runat="server" EnableViewState="false" />
<asp:HiddenField ID="hiddenSelected" runat="server" EnableViewState="false" />
