<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Frameset.aspx.cs" Inherits="CMSModules_DeviceProfile_Pages_Development_Frameset" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title>Device profiles - edit</title>
</head>
<frameset border="0" rows="<%=mHeight%>, *" id="rowsFrameset">
    <frame name="Menu" src="Header.aspx?<%=QueryHelper.EncodedQueryString%>"
        scrolling="no" frameborder="0" noresize="noresize" />
    <frame name="Content" src="General.aspx?<%=QueryHelper.EncodedQueryString%>"
        frameborder="0" noresize="noresize" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
