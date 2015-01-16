<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_RequestLog"
    CodeFile="RequestLog.ascx.cs" %>
<%@ Register Src="ValuesTable.ascx" TagName="ValuesTable" TagPrefix="cms" %>
<div class="DebugLog">
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <asp:GridView runat="server" ID="gridCache" EnableViewState="false" GridLines="None"
        AutoGenerateColumns="false" ShowFooter="true">
        <AlternatingRowStyle CssClass="AltRow" />
        <FooterStyle CssClass="Footer" />
        <Columns>
            <asp:TemplateField>
                <ItemStyle CssClass="Index" />
                <ItemTemplate>
                    <strong>
                        <%#GetIndex()%>
                    </strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetBeginIndent(Eval("Indent"))%><%#Eval("Method")%><%#GetEndIndent(Eval("Indent"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("Parameter")%>
                </ItemTemplate>
                <FooterTemplate>
                    <strong>
                        <%#CMSVersion%></strong>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetFromStart(Eval("Time"))%>
                    <br />
                </ItemTemplate>
                <FooterTemplate>
                    <strong>
                        <%#TotalDuration.ToString("F3")%>
                    </strong>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle CssClass="Dur" />
                <ItemStyle CssClass="Dur" />
                <FooterStyle CssClass="Dur" />
                <ItemTemplate>
                    <%#GetDuration(Eval("Duration"))%><br />
                    <%#GetDurationChart(Eval("Duration"), 0.005, 0, 0)%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <cms:ValuesTable ID="tblReqC" runat="server" EnableViewState="False" />
    <cms:ValuesTable ID="tblResC" runat="server" EnableViewState="False" />
    <cms:ValuesTable ID="tblVal" runat="server" EnableViewState="False" />
</div>
