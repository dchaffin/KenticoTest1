<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Basic_ThreeStateCheckBox" CodeFile="ThreeStateCheckBox.ascx.cs" %>
<cms:LocalizedRadioButton ID="rbPositive" runat="server" GroupName="ThreeState_<%=ClientID %>" />
<cms:LocalizedRadioButton ID="rbNegative" runat="server" GroupName="ThreeState_<%=ClientID %>" CssClass="RightColumn" />
<cms:LocalizedRadioButton ID="rbNotSet" runat="server" GroupName="ThreeState_<%=ClientID %>" CssClass="RightColumn" /> 
