<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DocumentCultureFilter.ascx.cs"
    Inherits="CMSModules_Content_Controls_Filters_DocumentCultureFilter" %>
<%@ Register Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" TagName="SiteCultureSelector"
    TagPrefix="cms" %>
<asp:DropDownList ID="drpLanguage" runat="server" CssClass="LongDropDownList" />
<cms:SiteCultureSelector runat="server" ID="cultureElem" IsLiveSite="false" />
