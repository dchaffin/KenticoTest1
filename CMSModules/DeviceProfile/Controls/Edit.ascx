<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="CMSModules_DeviceProfile_Controls_Edit" %>
<cms:UIForm runat="server" ID="editForm" ObjectType="cms.deviceprofile" RedirectUrlAfterCreate="Frameset.aspx?profileid={%EditedObject.ID%}&saved=1" />
