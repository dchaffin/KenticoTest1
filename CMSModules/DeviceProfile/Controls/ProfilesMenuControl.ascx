<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProfilesMenuControl.ascx.cs"
    Inherits="CMSModules_DeviceProfile_Controls_ProfilesMenuControl" %>
<%@ Register Src="~/CMSAdminControls/UI/UniMenu/UniMenuButtons.ascx" TagName="UniMenuButtons"
    TagPrefix="cms" %>
<div class="ActionButtons">
    <cms:UniMenuButtons ID="buttons" ShortID="b" runat="server" EnableViewState="false"
        AllowSelection="false" />
</div>
