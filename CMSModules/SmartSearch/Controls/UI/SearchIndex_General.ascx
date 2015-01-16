<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_General"
    CodeFile="SearchIndex_General.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="SearchIndex_StopWordsCustomAnalyzer.ascx" TagName="StopCustomControl"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Panel ID="pnlBody" runat="server">
    <asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        <cms:DisabledModule runat="server" ID="ucDisabledModule" />
        <table style="width: 100%">
            <tr>
                <td style="vertical-align: top; width: 100%;">
                    <table>
                        <tr>
                            <td class="FieldLabel">
                                <cms:LocalizedLabel runat="server" ID="lblDisplayName" EnableViewState="false" ResourceString="general.displayname"
                                    DisplayColon="true" />
                            </td>
                            <td>
                                <cms:LocalizableTextBox ID="txtDisplayName" runat="server" CssClass="TextBoxField"
                                    MaxLength="200" />
                                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" Display="Dynamic"
                                    ControlToValidate="txtDisplayName:textbox" ValidationGroup="Required" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel">
                                <cms:LocalizedLabel runat="server" ID="lblCodeName" EnableViewState="false" ResourceString="general.codename"
                                    DisplayColon="true" />
                            </td>
                            <td>
                                <cms:CodeName ID="txtCodeName" runat="server" CssClass="TextBoxField" MaxLength="200" />
                                <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" Display="Dynamic"
                                    ControlToValidate="txtCodeName" ValidationGroup="Required" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel">
                                <cms:LocalizedLabel runat="server" ID="lblType" EnableViewState="false" ResourceString="srch.index.type"
                                    DisplayColon="true" />
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblTypeValue" EnableViewState="false"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel">
                                <cms:LocalizedLabel runat="server" ID="lblAnalyzer" EnableViewState="false" ResourceString="srch.index.analyzer"
                                    DisplayColon="true" />
                            </td>
                            <td>
                                <asp:DropDownList ID="drpAnalyzer" runat="server" CssClass="DropDownFieldSmall">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <cms:StopCustomControl runat="server" ID="stopCustomControl" />
                        <tr>
                            <td class="FieldLabel">
                                <cms:LocalizedLabel runat="server" ID="lblBatchSize" EnableViewState="false" ResourceString="srch.index.batchsize"
                                    DisplayColon="true" />
                            </td>
                            <td>
                                <cms:CMSTextBox ID="txtBatchSize" runat="server" CssClass="TextBoxFieldSmall" MaxLength="8" />
                                <cms:CMSRequiredFieldValidator ID="rfvBatchSize" runat="server" Display="Dynamic"
                                    ControlToValidate="txtBatchSize" ValidationGroup="Required" EnableViewState="false" />
                            </td>
                        </tr>
                        <%--      
// Community indexing is not yet supported
  <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblCommunity" EnableViewState="false" ResourceString="srch.index.community"
                    DisplayColon="true" />
            </td>
            <td>
                <asp:CheckBox ID="chkCommunity" runat="server" />
            </td>
        </tr>--%>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" ValidationGroup="Required" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="vertical-align: top">
                    <div style="padding-right: 30px;">
                        <asp:Panel runat="server" ID="pnlInfo">
                            <div style="padding: 10px">
                                <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Timer ID="timRefresh" runat="server" Interval="3000" EnableViewState="false" />
                                        <table cellspacing="2" cellpadding="3">
                                            <tr>
                                                <td class="FieldLabel">
                                                    <cms:LocalizedLabel runat="server" ID="lblNumberOfItemsText" EnableViewState="false"
                                                        ResourceString="srch.general.numberofitems" DisplayColon="true" />
                                                </td>
                                                <td class="FieldLabel">
                                                    <cms:LocalizedLabel runat="server" ID="lblNumberOfItemsValue" EnableViewState="false" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="FieldLabel">
                                                    <cms:LocalizedLabel runat="server" ID="lblIndexFileSizeText" EnableViewState="false"
                                                        ResourceString="srch.general.filesize" DisplayColon="true" />
                                                </td>
                                                <td class="FieldLabel">
                                                    <cms:LocalizedLabel runat="server" ID="lblIndexFileSizeValue" EnableViewState="false" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="FieldLabel">
                                                    <cms:LocalizedLabel runat="server" ID="lblIndexStatusText" EnableViewState="false"
                                                        ResourceString="srch.index.indexstatus" DisplayColon="true" />
                                                </td>
                                                <td class="FieldLabel">
                                                    <cms:LocalizedLabel runat="server" ID="lblIndexStatusValue" EnableViewState="false" />
                                                    <asp:Literal runat="server" ID="ltrProgress" EnableViewState="false" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="FieldLabel">
                                                    <cms:LocalizedLabel runat="server" ID="lblIndexIsOptimizedText" EnableViewState="false"
                                                        ResourceString="srch.index.isoptimized" DisplayColon="true" />
                                                </td>
                                                <td class="FieldLabel">
                                                    <cms:LocalizedLabel runat="server" ID="lblIndexIsOptimizedValue" EnableViewState="false" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="FieldLabel">
                                                    <cms:LocalizedLabel runat="server" ID="lblLastUpdateText" EnableViewState="false"
                                                        ResourceString="srch.index.lastupdate" DisplayColon="true" />
                                                </td>
                                                <td class="FieldLabel">
                                                    <cms:LocalizedLabel runat="server" ID="lblLastUpdateValue" EnableViewState="false" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="FieldLabel">
                                                    <cms:LocalizedLabel runat="server" ID="lblLastRebuildTimeText" EnableViewState="false"
                                                        ResourceString="srch.index.lastrebuild" DisplayColon="true" />
                                                </td>
                                                <td class="FieldLabel">
                                                    <cms:LocalizedLabel runat="server" ID="lblLastRebuildTimeValue" EnableViewState="false" />
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </cms:CMSUpdatePanel>
                            </div>
                        </asp:Panel>
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
