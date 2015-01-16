<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_Images_ImageDialogSelector" CodeFile="ImageDialogSelector.ascx.cs" %>
<cms:localizedradiobutton id="radImageNo" runat="server" resourcestring="general.no"
    groupname="EnableImage" />
&nbsp;
<cms:localizedradiobutton id="radImageSimple" runat="server" resourcestring="forum.settings.simpledialog"
    groupname="EnableImage" />
&nbsp;
<cms:localizedradiobutton id="radImageAdvanced" runat="server" resourcestring="forum.settings.advanceddialog"
    groupname="EnableImage" />
