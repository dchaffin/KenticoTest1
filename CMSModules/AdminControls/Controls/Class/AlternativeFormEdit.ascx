<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_AlternativeFormEdit"
    CodeFile="AlternativeFormEdit.ascx.cs" %>
<cms:UIForm ID="form" runat="server" ObjectType="CMS.AlternativeForm">
    <LayoutTemplate>
        <cms:FormField runat="server" ID="fDispName" Field="FormDisplayName" FormControl="LocalizableTextBox"
            ResourceString="general.displayname" DisplayColon="true" />
        <cms:FormField runat="server" ID="fName" Field="FormName" FormControl="CodeName"
            ResourceString="general.codename" DisplayColon="true" />
        <cms:FormField runat="server" ID="fInherits" Field="FormHideNewParentFields" ToolTipResourceString="alternativeforms.hidenewparentfields.tooltip" FormControl="CheckBoxControl"
            ResourceString="alternativeforms.hidenewparentfields" DisplayColon="true" />
        <cms:FormSubmit runat="server" ID="fSubmit" />
    </LayoutTemplate>
</cms:UIForm>
