<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSSiteManager_Development_Cultures_Culture_Edit_Frameset"
    CodeFile="Culture_Edit_Frameset.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title>Cultures</title>
</head>
<frameset border="0" rows="<%=TabsBreadHeadFrameHeight%>, *" id="rowsFrameset">
    <frame name="menu" src="Culture_Edit_Header.aspx?cultureID=<%=QueryHelper.GetInteger("cultureID", 0)%> "
        scrolling="no" frameborder="0" noresize="noresize" />
    <frame name="content" src="Culture_Edit_General.aspx?cultureID=<%=QueryHelper.GetInteger("cultureID", 0)%>&saved=<%=QueryHelper.GetInteger("saved", 0)%>  "
        frameborder="0" noresize="noresize" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
