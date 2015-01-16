<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_FormControls_Pages_Development_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Form User Control Edit"
    CodeFile="Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagPrefix="cms"
    TagName="FileSystemSelector" %>
<%@ Register Src="~/CMSFormControls/SelectModule.ascx" TagPrefix="cms" TagName="SelectModule" %>
<%@ Register Src="~/CMSFormControls/System/UserControlTypeSelector.ascx" TagPrefix="cms"
    TagName="TypeSelector" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagName="File"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div style="width: 680px">
        <asp:Panel ID="pnlGeneral" runat="server" CssClass="FieldContentTable">
            <table class="FormControlsWrapper">
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel ID="lblDisplayName" runat="server" CssClass="ContentLabel" EnableViewState="false"
                            ResourceString="general.displayname" DisplayColon="true" />
                    </td>
                    <td>
                        <cms:LocalizableTextBox ID="tbDisplayName" runat="server" CssClass="TextBoxField"
                            MaxLength="100" />
                        <br />
                        <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="tbDisplayName:textbox"
                            Display="Dynamic" EnableViewState="false"></cms:CMSRequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel ID="lblCodeName" runat="server" CssClass="ContentLabel" EnableViewState="false"
                            ResourceString="general.codename" DisplayColon="true" />
                    </td>
                    <td>
                        <cms:CodeName ID="tbCodeName" runat="server" CssClass="TextBoxField" MaxLength="100" />
                        <br />
                        <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" EnableViewState="false"
                            ControlToValidate="tbCodeName" Display="dynamic"></cms:CMSRequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel" style="vertical-align: top">
                        <cms:LocalizedLabel ID="lblType" runat="server" CssClass="ContentLabel" EnableViewState="false"
                            ResourceString="formcontrols.type" DisplayColon="true" />
                    </td>
                    <td>
                        <cms:TypeSelector ID="drpTypeSelector" runat="server" CssClass="DropDownField" IncludeAllItem="false" />
                    </td>
                </tr>
                <asp:PlaceHolder ID="plcFileName" runat="server">
                    <tr>
                        <td class="FieldLabel" style="vertical-align: top">
                            <cms:LocalizedLabel ID="lblFileName" runat="server" CssClass="ContentLabel" EnableViewState="false"
                                ResourceString="general.filename" DisplayColon="true" />
                        </td>
                        <td>
                            <cms:FileSystemSelector ID="tbFileName" runat="server" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcFormControls" runat="server" Visible="false">
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel ID="lblInheritedWebPart" runat="server" CssClass="ContentLabel"
                                EnableViewState="false" ResourceString="DevelopmentWebPartGeneral.InheritedWebPart"
                                DisplayColon="true" />
                        </td>
                        <td>
                            <cms:CMSTextBox ID="txtParent" runat="server" Enabled="false" CssClass="TextBoxField" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblDescription" EnableViewState="false" DisplayColon="true"
                            ResourceString="General.Description" />
                    </td>
                    <td>
                        <cms:LocalizableTextBox TextMode="MultiLine" CssClass="TextAreaField" runat="server"
                            ID="txtDescription" />
                    </td>
                </tr>
                <asp:PlaceHolder runat="server" ID="plcThumbnail" Visible="false">
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblIcon" EnableViewState="false" DisplayColon="true"
                                ResourceString="General.Thumbnail" />
                        </td>
                        <td>
                            <cms:File ID="UploadFile" runat="server" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcDevelopment">
                    <tr>
                        <td class="FieldLabel">
                            <cms:LocalizedLabel runat="server" ID="lblModule" EnableViewState="false" ResourceString="General.Module" />
                        </td>
                        <td>
                            <cms:SelectModule ID="drpModule" runat="server" DisplayNone="true" DisplayAllModules="true"
                                IsLiveSite="false" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblHighPriority" EnableViewState="false" DisplayColon="true"
                            ResourceString="formcontrols.highpriority" />
                    </td>
                    <td>
                        <cms:LocalizedCheckBox ID="chkHighPriority" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        <asp:Panel runat="server" ID="pnlControlScope" CssClass="FieldContentTable">
            <table class="FormControlsWrapper">
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblUseControlFor" EnableViewState="false"
                            DisplayColon="true" ResourceString="development.formcontrols.usefor" />
                    </td>
                    <td>
                        <table class="BorderedWrapper FormControlTable">
                            <tr>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkForText" runat="server" ResourceString="Development_FormUserControl_Edit.lblForText" />
                                </td>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkForDateTime" runat="server" ResourceString="Development_FormUserControl_Edit.lblForDateTime" />
                                </td>
                            </tr>
                            <tr>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkForLongText" runat="server" ResourceString="Development_FormUserControl_Edit.lblForLongText" />
                                </td>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkForBoolean" runat="server" ResourceString="Development_FormUserControl_Edit.lblForBoolean" />
                                </td>
                            </tr>
                            <tr>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkForDecimal" runat="server" ResourceString="Development_FormUserControl_Edit.lblForDecimal" />
                                </td>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkForGuid" runat="server" ResourceString="templatedesigner.fieldtypes.guid" />
                                </td>
                            </tr>
                            <tr>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkForInteger" runat="server" ResourceString="Development_FormUserControl_Edit.lblForInteger" />
                                </td>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkForFile" runat="server" ResourceString="Development_FormUserControl_Edit.lblForFile" />
                                </td>
                            </tr>
                            <tr>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkForLongInteger" runat="server" ResourceString="Development_FormUserControl_Edit.lblForLongInteger" />
                                </td>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkForDocAtt" runat="server" ResourceString="Development_FormUserControl_Edit.lblForDocAtt" />
                                </td>
                            </tr>
                            <tr>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkForVisibility" runat="server" ResourceString="Development_FormUserControl_Edit.lblForVisibility" />
                                </td>
                                <td width="50%">
                                    &nbsp;
                                </td>
                            </tr>
                        </table>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <cms:LocalizedLabel runat="server" ID="lblShowControlIn" EnableViewState="false"
                            DisplayColon="true" ResourceString="development.formcontrols.showin" />
                    </td>
                    <td>
                        <table class="BorderedWrapper FormControlTable">
                            <tr>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkShowInDocumentTypes" runat="server" ResourceString="Development_FormUserControl_Edit.lblShowInDocumentTypes" />
                                </td>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkForBizForms" runat="server" AutoPostBack="True" OnCheckedChanged="chkForBizForms_CheckedChanged"
                                        ResourceString="Development_FormUserControl_Edit.lblForBizForms" />
                                </td>
                            </tr>
                            <tr>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkShowInCustomTables" runat="server" ResourceString="Development_FormUserControl_Edit.lblShowInCustomTables" />
                                </td>
                                <td width="50%" rowspan="4" style="vertical-align: top">
                                    <table>
                                        <tr>
                                            <td>
                                                <cms:LocalizedLabel ID="lblDataType" runat="server" EnableViewState="false" ResourceString="Development_FormUserControl_Edit.lblDataType"
                                                    DisplayColon="true" />
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpDataType" runat="server" CssClass="SmallDropDown" AutoPostBack="True"
                                                    OnSelectedIndexChanged="drpDataType_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="vertical-align: top">
                                                <cms:LocalizedLabel ID="lblColumnSize" runat="server" EnableViewState="false" ResourceString="Development_FormUserControl_Edit.lblColumnSize"
                                                    DisplayColon="true" />
                                            </td>
                                            <td>
                                                <cms:CMSTextBox ID="tbColumnSize" runat="server" CssClass="SmallTextBox">0</cms:CMSTextBox>
                                                <cms:LocalizedLabel ID="lblErrorSize" runat="server" CssClass="ErrorLabel" Visible="False"
                                                    EnableViewState="false" ResourceString="Development_FormUserControl_Edit.lblErrorSize" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkShowInSystemTables" runat="server" ResourceString="Development_FormUserControl_Edit.lblShowInSystemTables" />
                                </td>
                            </tr>
                            <tr>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkShowInReports" runat="server" ResourceString="Development_FormUserControl_Edit.lblShowInReports" />
                                </td>
                            </tr>
                            <tr>
                                <td width="50%">
                                    <cms:LocalizedCheckBox ID="chkShowInControls" runat="server" ResourceString="Development_FormUserControl_Edit.lblShowInControls" />
                                </td>
                            </tr>
                        </table>
                        <br />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOK_Click" CssClass="SubmitButton"
            EnableViewState="false" ResourceString="general.ok" />
    </div>
</asp:Content>
