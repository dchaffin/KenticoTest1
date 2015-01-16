<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Macros_Controls_UI_MacroRule_Edit"
    CodeFile="Edit.ascx.cs" %>
<cms:UIForm runat="server" ID="EditForm" ObjectType="cms.macrorule" DefaultFormLayout="FieldSets"
    RedirectUrlAfterCreate="Frameset.aspx?ruleid={%EditedObject.ID%}&saved=1&module={?module?}&displaytitle={?displaytitle?}">
</cms:UIForm>
