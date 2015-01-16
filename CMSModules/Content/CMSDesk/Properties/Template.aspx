<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Properties_Template"
    Theme="Default" CodeFile="Template.aspx.cs" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/Help.ascx" TagName="Help" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/PortalEngine/FormControls/PageTemplates/PageTemplateLevels.ascx"
    TagName="PageTemplateLevel" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.UIControls" Assembly="CMS.UIControls" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" HandleWorkflow="false" HelpTopicName="template"
        IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel ID="pnlContent" runat="server" CssClass="PropertiesPanel">
        <asp:Panel runat="server" ID="pnlActions" CssClass="NodePermissions">
            <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
                <ContentTemplate>
                    <asp:HiddenField runat="server" ID="hdnSelected" />
                    <div style="padding: 5px;">
                        <cms:LocalizedRadioButton runat="server" ID="radInherit" ResourceString="Template.Inherit"
                            GroupName="Template" AutoPostBack="true" OnCheckedChanged="RadChanged" /><br />
                        <cms:LocalizedRadioButton runat="server" ID="radAllCultures" ResourceString="Template.AllCultures"
                            GroupName="Template" AutoPostBack="true" OnCheckedChanged="RadChanged" /><br />
                        <cms:LocalizedRadioButton runat="server" ID="radThisCulture" ResourceString="Template.ThisCulture"
                            GroupName="Template" AutoPostBack="true" OnCheckedChanged="RadChanged" /><br />
                        <br />
                        <cms:CMSTextBox ID="txtTemplate" runat="server" ReadOnly="True" CssClass="SelectorTextBox"
                            Width="350" /><cms:CMSButton ID="btnSelect" runat="server" CssClass="ContentButton"
                                EnableViewState="false" OnClick="btnSelect_Click" />
                    </div>
                    <div style="padding: 5px;">
                        <table cellspacing="0" cellpadding="0">
                            <cms:UIPlaceHolder ID="plcUISave" runat="server" ModuleName="CMS.Content" ElementName="Template.SaveAsNew">
                                <tr>
                                    <td>
                                        <asp:LinkButton ID="btnSave" runat="server" CssClass="MenuItemEdit" EnableViewState="false">
                                            <cms:CMSImage ID="imgSave" runat="server" />
                                            <cms:LocalizedLabel runat="server" ID="lblSave" ResourceString="PageProperties.Save" />
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </cms:UIPlaceHolder>
                            <cms:UIPlaceHolder ID="plcUIClone" runat="server" ModuleName="CMS.Content" ElementName="Template.CloneAdHoc">
                                <tr>
                                    <td>
                                        <asp:LinkButton ID="btnClone" runat="server" CssClass="MenuItemEdit" OnClick="btnClone_Click"
                                            EnableViewState="false">
                                            <cms:CMSImage ID="imgClone" runat="server" />
                                            <cms:LocalizedLabel runat="server" ID="lblClone" ResourceString="PageProperties.Clone" />
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </cms:UIPlaceHolder>
                            <cms:UIPlaceHolder ID="plcUIEdit" runat="server" ModuleName="CMS.Content" ElementName="Template.EditProperties">
                                <tr>
                                    <td>
                                        <asp:LinkButton ID="btnEditTemplateProperties" runat="server" CssClass="MenuItemEdit"
                                            EnableViewState="false">
                                            <cms:CMSImage ID="imgEditTemplateProperties" runat="server" />
                                            <cms:LocalizedLabel runat="server" ID="lblEditTemplate" ResourceString="PageProperties.EditTemplateProperties" />
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </cms:UIPlaceHolder>
                        </table>
                    </div>
                </ContentTemplate>
            </cms:CMSUpdatePanel>
        </asp:Panel>
        <br />
        <cms:UIPlaceHolder ID="plcUILevels" runat="server" ModuleName="CMS.Content" ElementName="Template.InheritContent">
            <asp:Panel runat="server" ID="pnlInherits" CssClass="NodePermissions">
                <cms:PageTemplateLevel runat="server" ID="inheritElem"></cms:PageTemplateLevel>
            </asp:Panel>
        </cms:UIPlaceHolder>
    </asp:Panel>
</asp:Content>
