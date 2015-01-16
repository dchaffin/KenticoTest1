<%@ Page Language="C#" AutoEventWireup="True" Inherits="CMSModules_Chat_Pages_Tools_ChatRoom_Frameset" CodeFile="Frameset.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/TabsFrameset.ascx" TagName="TabsFrameset" TagPrefix="cms" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title>Chat room properties</title>
</head>
<cms:TabsFrameset ID="frm" runat="server" ContentUrl="Edit.aspx" />
</html>
