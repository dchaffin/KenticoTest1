<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Frameset"
    EnableViewState="false" CodeFile="Frameset.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title>Online Marketing Automation - Processes</title>
</head>
<frameset border="0" rows="<%=TabsFrameHeight%>, *" id="rowsFrameset">
    <frame name="processesHeader" src="<%=headerTargetUrl%>" scrolling="no" frameborder="0"
        noresize="noresize" />
    <frame name="processesContent" src="<%=contentTargetUrl%>" frameborder="0" noresize="noresize" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
