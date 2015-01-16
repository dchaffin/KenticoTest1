<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LocalizableTextArea.ascx.cs"
    Inherits="CMSFormControls_System_LocalizableTextArea" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<cms:LocalizableTextBox ID="txtDescription" runat="server" MaxLength="450" TextMode="MultiLine"
    CssClass="TextAreaField" />
