<%@ Control Language="C#" AutoEventWireup="false" CodeFile="LayoutBinding.ascx.cs"
    Inherits="CMSModules_DeviceProfile_Controls_LayoutBinding" %>
<div class="DeviceProfileLayoutMapping">
    <div class="FlatItem">
        <div class="SelectorEnvelope DeviceProfileSourceLayout" style="overflow: hidden">
            <div class="SelectorFlatImage">
                <asp:Image ID="SourceLayoutThumbnail" runat="server"/>
            </div>
            <asp:Label ID="SourceLayoutDisplayNameLabel" runat="server" CssClass="SelectorFlatText"></asp:Label>
        </div>
    </div>
    <div class="MappingDirection">
    </div>
    <div class="FlatItem" title="<%= HTMLHelper.HTMLEncode(GetString("device_profile.layoutmapping.sethint")) %>">
        <div class="SelectorEnvelope" style="overflow: hidden" id="TargetLayoutItemControl" runat="server">
            <% if (TargetLayout != null)
               { %>
            <img src="<%= GetImageUrl("Design/Controls/UniGrid/Actions/delete.png") %>" alt="<%= HTMLHelper.HTMLEncode(GetString("device_profile.layoutmapping.unset")) %>"
                title="<%= HTMLHelper.HTMLEncode(GetString("device_profile.layoutmapping.unset")) %>"
                class="RemoveButton" onclick="jQuery.Event(event).stopPropagation(); Client_UnsetTargetLayout({sourceLayoutId:<%= SourceLayout.LayoutId.ToString("D") %>});" />
            <% } %>
            <div class="SelectorFlatImage">
                <asp:Image ID="TargetLayoutThumbnail" runat="server"/>
            </div>
            <asp:Label ID="TargetLayoutDisplayNameLabel" runat="server" CssClass="SelectorFlatText"></asp:Label>
        </div>
    </div>
    <div style="clear: both">
    </div>
</div>
