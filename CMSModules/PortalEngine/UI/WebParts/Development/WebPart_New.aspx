<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_WebParts_Development_WebPart_New"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Web parts - New"
    CodeFile="WebPart_New.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/SelectWebpart.ascx"
    TagName="SelectWebpart" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagPrefix="cms"
    TagName="FileSystemSelector" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcSiteSelector" runat="server">
    <br />
    <cms:LocalizedRadioButton runat="server" ID="radNewWebPart" ResourceString="developmentwebparteditnewwepart"
        GroupName="wpSelect" Checked="true" OnCheckedChanged="radNewWebPart_CheckedChanged"
        AutoPostBack="true" />
    <br />
    <cms:LocalizedRadioButton runat="server" ID="radInherited" GroupName="wpSelect" ResourceString="Development-WebPart_Edit.Inherited"
        OnCheckedChanged="radNewWebPart_CheckedChanged" AutoPostBack="true" />
    <br />
    <br />
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel ID="PanelUsers" runat="server">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <asp:PlaceHolder ID="plcTable" runat="server">
                    <table>
                        <tr>
                            <td class="FieldLabel">
                                <cms:LocalizedLabel ID="lbWebPartDisplaytName" runat="server" EnableViewState="False"
                                    ResourceString="general.displayname" DisplayColon="true" />
                            </td>
                            <td>
                                <cms:LocalizableTextBox ID="txtWebPartDisplayName" runat="server" CssClass="TextBoxField"
                                    MaxLength="95" />
                                <cms:CMSRequiredFieldValidator ID="rfvWebPartDisplayName" runat="server" EnableViewState="false"
                                    ControlToValidate="txtWebPartDisplayName:textbox" Display="dynamic"></cms:CMSRequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel" style="vertical-align: top">
                                <cms:LocalizedLabel ID="lbWebPartName" runat="server" EnableViewState="False" ResourceString="general.codename"
                                    DisplayColon="true" />
                            </td>
                            <td>
                                <cms:CodeName ID="txtWebPartName" runat="server" CssClass="TextBoxField" MaxLength="95" />
                                <cms:CMSRequiredFieldValidator ID="rfvWebPartName" runat="server" EnableViewState="false"
                                    ControlToValidate="txtWebPartName" Display="dynamic"></cms:CMSRequiredFieldValidator>
                            </td>
                        </tr>
                        <asp:PlaceHolder ID="plcFileName" runat="server">
                            <tr>
                                <td>
                                    <cms:LocalizedLabel ID="lblWebPartFileName" runat="server" ResourceString="general.filepath"
                                        DisplayColon="true" />
                                </td>
                                <td>
                                    <cms:FileSystemSelector ID="FileSystemSelector" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <cms:LocalizedCheckBox runat="server" ID="chkGenerateFiles" ResourceString="webpart.generatefiles" Checked="true" />
                                </td>
                            </tr>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plcWebparts" runat="server" Visible="false">
                            <tr>
                                <td>
                                    <asp:Label ID="lblWebpartList" runat="server" />
                                </td>
                                <td>
                                    <cms:SelectWebpart ID="webpartSelector" runat="server" />
                                </td>
                            </tr>
                        </asp:PlaceHolder>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <cms:FormSubmitButton ID="btnOk" runat="server" CssClass="SubmitButton" EnableViewState="false"
                                    OnClick="btnOK_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>
