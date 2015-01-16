<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Alternative Forms - New" Inherits="CMSModules_SystemTables_Pages_Development_AlternativeForms_New"
    Theme="default" CodeFile="New.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm ID="form" runat="server" ObjectType="CMS.AlternativeForm">
        <LayoutTemplate>
            <cms:FormField runat="server" ID="fDispName" Field="FormDisplayName" FormControl="LocalizableTextBox"
                ResourceString="general.displayname" DisplayColon="true" />
            <cms:FormField runat="server" ID="fName" Field="FormName" FormControl="CodeName"
                ResourceString="general.codename" DisplayColon="true" />
            <cms:FormField runat="server" ID="fInherits" Field="FormHideNewParentFields" FormControl="CheckBoxControl"
                ResourceString="alternativeforms.hidenewparentfields" DisplayColon="true" />
            <asp:PlaceHolder runat="server" ID="pnlCombineUserSettings" Visible="false">
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblCombineUserSettings" runat="server" EnableViewState="false"
                            DisplayColon="true" AssociatedControlID="chkCombineUserSettings" CssClass="EditingFormLabel" ResourceString="altform.combineusersettings" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkCombineUserSettings" runat="server" EnableViewState="false" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <cms:FormSubmit runat="server" ID="fSubmit" />
        </LayoutTemplate>
    </cms:UIForm>
</asp:Content>
