<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ProjectManagement_Controls_UI_Projecttaskstatus_Edit" CodeFile="Edit.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
    
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<table style="vertical-align: top">
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblTaskStatusDisplayName" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.displayname" AssociatedControlID="txtTaskStatusDisplayName" />
        </td>
        <td>
            <cms:LocalizableTextBox ID="txtTaskStatusDisplayName" runat="server" CssClass="TextBoxField" MaxLength="200"
                EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvTaskStatusDisplayName" runat="server" Display="Dynamic"
                ControlToValidate="txtTaskStatusDisplayName:textbox" ValidationGroup="vgProjecttaskstatus" EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblTaskStatusName" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.codename" AssociatedControlID="txtTaskStatusName" />
        </td>
        <td>
            <cms:CodeName ID="txtTaskStatusName" runat="server" CssClass="TextBoxField" MaxLength="200"
                EnableViewState="false" />
            <cms:CMSRequiredFieldValidator ID="rfvTaskStatusName" runat="server" Display="Dynamic"
                ControlToValidate="txtTaskStatusName" ValidationGroup="vgProjecttaskstatus" EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblTaskStatusColor" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.projectstatus.edit.color" AssociatedControlID="colorPicker" />
        </td>
        <td>
            <cms:ColorPicker ID="colorPicker" runat="server"  />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblTaskStatusIcon" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.projectstatus.edit.icon" AssociatedControlID="txtTaskStatusIcon" />
        </td>
        <td>
            <cms:CMSTextBox ID="txtTaskStatusIcon" runat="server" CssClass="TextBoxField" MaxLength="450"
                EnableViewState="false" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblTaskStatusIsNotStarted" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.projectstatus.isnotstartedstatus" AssociatedControlID="chkTaskStatusIsNotStarted" />
        </td>
        <td>
            <asp:CheckBox ID="chkTaskStatusIsNotStarted" runat="server" EnableViewState="false"
                CssClass="CheckBoxMovedLeft" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblTaskStatusIsFinished" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="pm.projectstatus.isfinishstatus" AssociatedControlID="chkTaskStatusIsFinished" />
        </td>
        <td>
            <asp:CheckBox ID="chkTaskStatusIsFinished" runat="server" EnableViewState="false"
                CssClass="CheckBoxMovedLeft" />
        </td>
    </tr>
    <tr>
        <td class="FieldLabel">
            <cms:LocalizedLabel ID="lblTaskStatusEnabled" runat="server" EnableViewState="false" DisplayColon="true"
                ResourceString="general.enabled" AssociatedControlID="chkTaskStatusEnabled" />
        </td>
        <td>
            <asp:CheckBox ID="chkTaskStatusEnabled" runat="server" EnableViewState="false" Checked="true"
                CssClass="CheckBoxMovedLeft" />
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false" CssClass="SubmitButton"
                OnClick="btnOk_Click" ValidationGroup="vgProjecttaskstatus" />
        </td>
    </tr>
</table>