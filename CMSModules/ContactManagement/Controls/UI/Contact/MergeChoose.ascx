<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MergeChoose.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Contact_MergeChoose" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Contact/Filter.ascx" TagPrefix="cms" TagName="ContactFilter" %>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:LocalizedLabel ID="lblTitle" runat="server" ResourceString="om.contact.choosemerge"
    EnableViewState="false" CssClass="BoldInfoLabel" DisplayColon="true" />
<cms:UniGrid runat="server" ID="gridElem" ObjectType="om.contactlist" OrderBy="ContactLastName"
    Columns="ContactID,ContactLastName,ContactFirstName,ContactEmail,ContactStatusID,ContactCountryID,ContactSiteID"
    IsLiveSite="false" ShowObjectMenu="false" HideFilterButton="true" RememberState="false"> 
    <GridColumns>
        <ug:Column Source="ContactFirstName" Caption="$om.contact.firstname$" Wrap="false">
        </ug:Column>
        <ug:Column Source="ContactLastName" Caption="$om.contact.lastname$" Wrap="false">
        </ug:Column>
        <ug:Column Source="ContactEmail" Caption="$general.emailaddress$" Wrap="false">
        </ug:Column>
        <ug:Column Source="ContactStatusID" AllowSorting="false" ExternalSourceName="#transform: om.contactstatus.contactstatusdisplayname"
            Caption="$om.contactstatus$" Wrap="false">
        </ug:Column>
        <ug:Column Source="ContactCountryID" AllowSorting="false" ExternalSourceName="#transform: cms.country.countrydisplayname"
            Caption="$general.country$" Wrap="false">
        </ug:Column>
        <ug:Column Source="ContactSiteID" Name="sitename" AllowSorting="false" ExternalSourceName="#sitenameorglobal"
            Caption="$general.sitename$" Wrap="false" Localize="true">
        </ug:Column>
        <ug:Column Source="ContactGUID" Visible="false">
            <Filter Type="custom" Path="~/CMSModules/ContactManagement/Controls/UI/Contact/Filter.ascx" />
        </ug:Column>
        <ug:Column Width="100%" />
    </GridColumns>
    <GridOptions DisplayFilter="true" ShowSelection="true" />
</cms:UniGrid>
<asp:Panel ID="pnlButton" runat="server" class="PanelButton">
    <cms:LocalizedButton ID="btnMergeSelected" runat="server" CssClass="LongSubmitButton"
        ResourceString="om.contact.mergeselected" />
    <cms:LocalizedButton ID="btnMergeAll" runat="server" CssClass="LongSubmitButton"
        ResourceString="om.contact.mergeall" />
</asp:Panel>
<asp:HiddenField ID="hdnIdentifier" runat="server" EnableViewState="false" />
