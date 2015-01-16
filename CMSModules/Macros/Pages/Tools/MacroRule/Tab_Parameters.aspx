<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Macro rule properties – Parameters" Inherits="CMSModules_Macros_Pages_Tools_MacroRule_Tab_Parameters"
    Theme="Default" CodeFile="Tab_Parameters.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldEditor.ascx" TagName="FieldEditor"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:FieldEditor ID="editElem" runat="server" IsLiveSite="false" Mode="General" />
</asp:Content>
