<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_General"
    Theme="Default" CodeFile="WebPart_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>

<%@ Register Src="~/CMSAdminControls/UI/Selectors/LoadGenerationSelector.ascx" TagName="LoadGenerationSelector"
    TagPrefix="uc1" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagName="File" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/PortalEngine/Controls/WebParts/SelectWebpart.ascx"
    TagName="SelectWebpart" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagPrefix="cms"
    TagName="FileSystemSelector" %>
<%@ Register Src="~/CMSFormControls/SelectModule.ascx" TagPrefix="cms" TagName="SelectModule" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:Literal ID="ltlScript" runat="server" />
    <table>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblDisplayName" runat="server" EnableViewState="False" ResourceString="general.displayname"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtWebPartDisplayName" runat="server" CssClass="TextBoxField" MaxLength="95" />
                <cms:CMSRequiredFieldValidator ID="rfvWebPartDisplayName" runat="server" EnableViewState="false"
                    ControlToValidate="txtWebPartDisplayName:textbox" Display="dynamic"></cms:CMSRequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lbWebPartName" runat="server" EnableViewState="False" ResourceString="general.codename"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:CodeName ID="txtWebPartName" runat="server" CssClass="TextBoxField" MaxLength="95" />
                <cms:CMSRequiredFieldValidator ID="rfvWebPartName" runat="server" EnableViewState="false"
                    ControlToValidate="txtWebPartName" Display="dynamic"></cms:CMSRequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <asp:Label ID="lbWebPartCategory" runat="server" EnableViewState="False" />
            </td>
            <td>
                <cms:SelectWebpart ID="categorySelector" runat="server" ShowWebparts="false" EnableCategorySelection="true" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <asp:Label ID="lblWebPartType" runat="server" EnableViewState="False" />
            </td>
            <td>
                <asp:DropDownList ID="drpWebPartType" runat="server" CssClass="DropDownField" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblWebPartFileName" runat="server" ResourceString="general.filename"
                    DisplayColon="true" />
            </td>
            <td>
                <asp:PlaceHolder ID="plcInheritedName" runat="server" Visible="false">
                    <cms:CMSTextBox ID="txtInheritedName" runat="server" Enabled="false" CssClass="TextBoxField" />
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcFileSystemSelector" runat="server">
                    <cms:FileSystemSelector ID="FileSystemSelector" runat="server" />
                </asp:PlaceHolder>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblWebPartDescription" runat="server" ResourceString="general.description"
                    DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtWebPartDescription" runat="server" CssClass="TextAreaField" TextMode="MultiLine" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <asp:Label ID="lblUploadFile" runat="server" />
            </td>
            <td>
                <cms:File ID="attachmentFile" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel ID="lblSkipInsertProperties" runat="server" ResourceString="webpart.SkipInsertProperties"
                    DisplayColon="true" AssociatedControlID="chkSkipInsertProperties" />
            </td>
            <td>
                <asp:CheckBox ID="chkSkipInsertProperties" runat="server" />
            </td>
        </tr>
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
            <tr>
                <td class="FieldLabel">
                    <asp:Label runat="server" ID="lblLoadGeneration" EnableViewState="false" />
                </td>
                <td>
                    <uc1:LoadGenerationSelector ID="drpGeneration" runat="server" />
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
</asp:Content>
