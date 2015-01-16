<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TimeControl.ascx.cs" Inherits="CMSFormControls_Basic_TimeControl" %>
<%@ Register Src="~/CMSFormControls/Basic/TextBoxControl.ascx" TagName="TextBoxControl"
    TagPrefix="cms" %>
<cms:TextBoxControl ID="txtHour" runat="server" MaxLength="2" CssClass="SuperSmallTextBox"
    FilterType="Numbers" />
&nbsp;:&nbsp;<cms:TextBoxControl ID="txtMinute" runat="server" MaxLength="2" CssClass="SuperSmallTextBox"
    FilterType="Numbers" />
&nbsp;<cms:LocalizedDropDownList ID="drpAmPm" runat="server" />
