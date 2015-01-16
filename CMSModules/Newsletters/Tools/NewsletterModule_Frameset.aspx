<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Newsletters_Tools_NewsletterModule_Frameset"
    CodeFile="NewsletterModule_Frameset.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title>Newsletter module</title>
</head>
<frameset border="0" cols="126, *" frameborder="0" runat="server" id="colsFrameset">
    <frame name="newslettersMenu" src="Header.aspx"  scrolling="no" noresize="noresize"
        frameborder="0" id="configurationMenu" runat="server" />
    <frame name="newslettersContent" src="Newsletters/Newsletter_List.aspx" noresize="noresize"
        frameborder="0" scrolling="auto" id="configurationContent" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
