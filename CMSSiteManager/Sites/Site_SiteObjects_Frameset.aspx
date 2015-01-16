<%@ Page Language="C#" AutoEventWireup="True" CodeFile="Site_SiteObjects_Frameset.aspx.cs"
    Inherits="CMSSiteManager_Sites_Site_SiteObjects_Frameset" Theme="Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server" enableviewstate="false">
    <title>Site Manager - Site Objects</title>
</head>
<frameset border="0" cols="126, *" id="colsFrameset" runat="server">
    <frame name="menu" id="menu" runat="server" scrolling="no" frameborder="0"
        noresize="noresize" />
    <frame name="content"  frameborder="0"
        noresize="noresize" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
