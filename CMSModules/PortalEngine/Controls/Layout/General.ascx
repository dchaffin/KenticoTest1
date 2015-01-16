<%@ Control Language="C#" AutoEventWireup="true" CodeFile="General.ascx.cs" Inherits="CMSModules_PortalEngine_Controls_Layout_General" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/PortalEngine/FormControls/WebPartLayouts/WebPartLayoutSelector.ascx"
    TagPrefix="cms" TagName="LayoutSelector" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Locking/ObjectEditPanel.ascx" TagName="ObjectEditPanel"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Macros/MacroEditor.ascx" TagName="MacroEditor"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlBody" CssClass="TabsPageBody SimpleHeader">
    <asp:Panel ID="pnlTab" runat="server" CssClass="TabsPageContent">
        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
            <ContentTemplate>
                <cms:ObjectEditPanel runat="server" ID="editMenuElem" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <asp:Panel ID="pnlFormArea" runat="server" CssClass="PreviewDefaultContent">
            <div class="PageContent WebPartLayoutContent">
                <asp:PlaceHolder runat="server" Visible="true" ID="plcContent">
                    <cms:UIForm runat="server" ID="EditForm" ObjectType="cms.webpartlayout" DefaultFieldLayout="Inline"
                        RedirectUrlAfterCreate="">
                        <LayoutTemplate>
                            <table>
                                <asp:PlaceHolder runat="server" ID="plcValues" Visible="false">
                                    <tr>
                                        <td class="FieldLabel">
                                            <cms:LocalizedLabel runat="server" ID="lblDisplayName" ResourceString="general.displayname"
                                                DisplayColon="true" />
                                        </td>
                                        <td>
                                            <cms:LocalizableTextBox ID="txtDisplayName" runat="server" MaxLength="100" Field="WebPartLayoutDisplayName" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="FieldLabel">
                                            <cms:LocalizedLabel runat="server" ID="lblCodeName" ResourceString="general.codename"
                                                DisplayColon="true" />
                                        </td>
                                        <td>
                                            <cms:CodeName runat="server" ID="txtCodeName" Field="WebPartLayoutCodeName" />
                                        </td>
                                    </tr>
                                    <asp:PlaceHolder runat="server" ID="plcDescription" Visible="false">
                                        <tr>
                                            <td>
                                                <cms:LocalizedLabel ID="lblDescription" runat="server" ResourceString="general.description"
                                                    DisplayColon="true" />
                                            </td>
                                            <td>
                                                <cms:LocalizableTextBox ID="txtDescription" runat="server" MaxLength="450" Field="WebPartLayoutDescription"
                                                    TextMode="MultiLine" CssClass="TextAreaField" />
                                            </td>
                                        </tr>
                                    </asp:PlaceHolder>
                                </asp:PlaceHolder>
                                <tr>
                                    <td style="white-space: nowrap;">
                                        <cms:LocalizedLabel runat="server" ID="lblCode" ResourceString="webparteditlayoutedit.lblcode"
                                            DisplayColon="true" EnableViewState="false" />
                                    </td>
                                    <td class="MaxWidth">                                       
                                        <cms:FormField runat="server" ID="fCode" Field="WebPartLayoutCode">
                                            <cms:MacroEditor runat="server" ID="etaCode" Field="WebPartLayoutCode" />
                                        </cms:FormField>
                                    </td>
                                </tr>
                                <asp:PlaceHolder runat="server" ID="plcCssLink">
                                    <tr id="cssLink">
                                        <td class="FieldLabel">
                                        </td>
                                        <td>
                                            <cms:LocalizedLinkButton runat="server" ID="lnkStyles" EnableViewState="false" ResourceString="general.addcss"
                                                OnClientClick="document.getElementById('editCss').style.display = 'table-row'; document.getElementById('cssLink').style.display = 'none';ResizeHeaderButtons(); return false;" />
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                                <tr id="editCss" style="<%=(plcCssLink.Visible ? "display: none": "")%>">
                                    <td class="FieldLabel">
                                        <cms:LocalizedLabel runat="server" ID="lblCSS" ResourceString="Container_Edit.ContainerCSS"
                                            DisplayColon="true" EnableViewState="false" />
                                    </td>
                                    <td colspan="3">
                                        <cms:FormField runat="server" ID="FormField1" Field="WebPartLayoutCss">
                                            <cms:MacroEditor runat="server" ID="etaCSS" />
                                        </cms:FormField>
                                    </td>
                                </tr>
                            </table>
                        </LayoutTemplate>
                    </cms:UIForm>
                    <asp:HiddenField runat="server" ID="hidRefresh" Value="0" />
                </asp:PlaceHolder>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
<asp:HiddenField ID="hdnClose" runat="server" EnableViewState="false" />
