<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SettingsKeyControlSelector.ascx.cs"
    Inherits="CMSModules_Settings_Controls_SettingsKeyControlSelector" %>
<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagPrefix="cms"
    TagName="FileSystemSelector" %>
<%@ Register Src="~/CMSModules/FormControls/FormControls/FormControlSelector.ascx"
    TagName="FormControlSelector" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlChoice" runat="server">
            <cms:LocalizedRadioButton ID="rdbFormControl" GroupName="rdbControlChoice" runat="server" Checked="true"
                AutoPostBack="true" ResourceString="objecttype.cms_formusercontrol" OnCheckedChanged="rdbFormControl_CheckedChanged" />
            <cms:LocalizedRadioButton ID="rdbFileSystem" GroupName="rdbControlChoice" runat="server"
                AutoPostBack="true" ResourceString="settingskey.filesystempath" OnCheckedChanged="rdbFormControl_CheckedChanged" />
        </asp:Panel>
        <asp:Panel ID="pnlFileSystem" runat="server" Visible="false">
            <cms:FileSystemSelector ID="selectFileName" runat="server" SelectedPathPrefix="~/CMSFormControls" />
        </asp:Panel>
        <asp:Panel ID="pnlFormControl" runat="server" Visible="true">
            <cms:FormControlSelector ID="selectFormControl" runat="server" ShowInheritedControls="true"
                ReturnColumnName="UserControlCodeName" AllowEmptyValue="true"  />
        </asp:Panel>
    </ContentTemplate>
</cms:CMSUpdatePanel>
