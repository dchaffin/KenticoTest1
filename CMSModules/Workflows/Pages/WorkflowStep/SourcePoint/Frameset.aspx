<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Frameset.aspx.cs" Inherits="CMSModules_Workflows_Pages_WorkflowStep_SourcePoint_Frameset" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head enableviewstate="false">
    <title>Workflows - Workflow Edit</title>
</head>
    <frameset border="0" rows="<%=TabsBreadFrameHeight%>, *" ID="rowsFrameset">
        <frame name="wfSourcePointMenu" src="<%=headerTargetUrl%>" scrolling="no" frameborder="0" noresize="noresize" />
        <frame name="wfSourcePointContent" src="<%=contentTargetUrl%>" frameborder="0" noresize="noresize" />
        <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
    </frameset>
</html>
