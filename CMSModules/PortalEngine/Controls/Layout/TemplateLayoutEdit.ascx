<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_Controls_Layout_TemplateLayoutEdit"
    CodeFile="TemplateLayoutEdit.ascx.cs" %>
<%@ Register Src="~/CMSModules/PortalEngine/FormControls/PageLayouts/PageLayoutSelector.ascx"
    TagName="LayoutSelector" TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="File" Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" %>
<%@ Register TagPrefix="cms" TagName="PageLayoutCode" Src="~/CMSFormControls/Layouts/PageLayoutCode.ascx" %>
<%@ Register Src="~/CMSModules/Objects/Controls/Locking/ObjectEditPanel.ascx" TagName="ObjectEditPanel"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="CSSStylesEditor" Src="~/CMSFormControls/Layouts/CSSStylesEditor.ascx" %>
<asp:Panel runat="server" ID="pnlBody">
    <script type="text/javascript">
        function RefreshWOpener(w) {
            if (w.refreshPageOnClose) {
                window.location = window.location.href + '&refreshParent=1';
            }
        }

        function RefreshAfterDelete() {
            if (wopener.parent.RefreshAfterDelete) {
                wopener.parent.RefreshAfterDelete();
            }
        }
    </script>
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Always">
        <ContentTemplate>
            <cms:ObjectEditPanel runat="server" ID="editMenuElem" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <asp:Panel ID="pnlContainer" runat="server" CssClass="PreviewDefaultContent">
        <asp:Panel runat="server" ID="pnlType" CssClass="PageHeaderLine">
            <table>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="radShared" GroupName="LayoutGroup" />&nbsp;
                    </td>
                    <td>
                        <cms:LayoutSelector runat="server" ID="drpLayout" IsLiveSite="false" OnChanged="selectShared_Changed" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:RadioButton runat="server" ID="radCustom" GroupName="LayoutGroup" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <div class="LayoutForm">
            <cms:UIForm runat="server" ID="EditFormTemplate" ObjectType="cms.pagetemplate" DefaultFieldLayout="Inline"
                EnabledByLockState="False">
                <SecurityCheck Resource="CMS.Design" Permission="Design" />
                <LayoutTemplate>
                    <cms:FormField runat="server" ID="fCode" Field="PageTemplateLayout">
                        <cms:PageLayoutCode runat="server" ID="codeElem" CodeColumn="PageTemplateLayout"
                            TypeColumn="PageTemplateLayoutType" />
                    </cms:FormField>
                    <cms:FormField runat="server" ID="fCSS" Field="PageTemplateCSS">
                        <cms:CSSStylesEditor runat="server" ID="cssEditor" />
                    </cms:FormField>
                </LayoutTemplate>
            </cms:UIForm>
            <cms:UIForm runat="server" ID="EditFormDeviceLayout" ObjectType="cms.templatedevicelayout"
                DefaultFieldLayout="Inline" EnabledByLockState="False">
                <SecurityCheck Resource="CMS.Design" Permission="Design" />
                <LayoutTemplate>
                    <cms:FormField runat="server" ID="fDCode" Field="LayoutCode">
                        <cms:PageLayoutCode runat="server" ID="deviceCode" CodeColumn="LayoutCode" TypeColumn="LayoutType" />
                    </cms:FormField>
                    <cms:FormField runat="server" ID="dDCSS" Field="LayoutCSS">
                        <cms:CSSStylesEditor runat="server" ID="cssDeviceEditor" />
                    </cms:FormField>
                </LayoutTemplate>
            </cms:UIForm>
            <cms:UIForm runat="server" ID="EditFormLayout" ObjectType="cms.layout" DefaultFieldLayout="Inline"
                RedirectUrlAfterCreate="PageLayout_Frameset.aspx?layoutid={%EditedObject.ID%}&saved=1"
                EnabledByLockState="False">
                <SecurityCheck Resource="CMS.Design" Permission="Design" />
                <LayoutTemplate>
                    <cms:CMSUpdatePanel runat="server" ID="pnlServer" UpdateMode="Always">
                        <ContentTemplate>
                            <cms:FormCategory runat="server" ID="pnlGeneral" ResourceString="general.general"
                                DefaultFieldLayout="TwoColumns">
                                <cms:FormField runat="server" ID="fDisplayName" Field="LayoutDisplayName" FormControl="LocalizableTextBox"
                                    ResourceString="general.displayname" DisplayColon="true" />
                                <cms:FormField runat="server" ID="fName" Field="LayoutCodeName" FormControl="CodeName"
                                    ResourceString="general.codename" DisplayColon="true" />
                                <cms:FormField runat="server" ID="fDescription" Field="LayoutDescription" FormControl="TextAreaControl"
                                    ResourceString="general.description" DisplayColon="true" />
                                <cms:FormField runat="server" ID="fThumbnailGUID" Field="LayoutThumbnailGUID" Layout="Inline">
                                    <tr>
                                        <td class="FieldLabel">
                                            <cms:FormLabel ID="lblMetaFile" runat="server" EnableViewState="true" ResourceString="general.thumbnail"
                                                DisplayColon="true" />
                                        </td>
                                        <td>
                                            <cms:File ID="ucFile" runat="server" Enabled="true" />
                                        </td>
                                    </tr>
                                </cms:FormField>
                                <cms:FormField runat="server" ID="fIsConverible" Field="LayoutIsConvertible" FormControl="CheckboxControl"
                                    DisplayColon="true" ResourceString="pagelayout.isconvertible" />
                                <cms:FormField runat="server" ID="fZoneCount" Field="LayoutZoneCount" FormControl="TextBoxControl"
                                    DisplayColon="true" ResourceString="pagelayout.zonecount" />
                            </cms:FormCategory>
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                    <cms:FormCategory runat="server" ID="pnlLayout" ResourceString="general.layout" DefaultFieldLayout="Inline">
                        <cms:FormField runat="server" ID="fLayoutCode" Field="LayoutCode">
                            <cms:PageLayoutCode runat="server" ID="codeLayoutElem" CodeColumn="LayoutCode" TypeColumn="LayoutType" />
                        </cms:FormField>
                        <cms:FormField runat="server" ID="fLayoutCSS" Field="LayoutCSS">
                            <cms:CSSStylesEditor runat="server" ID="cssLayoutEditor" />
                        </cms:FormField>
                    </cms:FormCategory>
                </LayoutTemplate>
            </cms:UIForm>
        </div>
    </asp:Panel>
</asp:Panel>
<asp:HiddenField ID="hdnWOpenerRefreshed" runat="server" />
<asp:HiddenField ID="hdnClose" runat="server" EnableViewState="false" />
