<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_AnalyticsLog"
    CodeFile="AnalyticsLog.ascx.cs" %>
<div class="DebugLog">
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <asp:GridView runat="server" ID="gridAnalytics" EnableViewState="false" GridLines="None"
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
                    <%#Eval("CodeName")%>
                </ItemTemplate>
                <FooterTemplate>
                    <div style="color:#bd6200"><strong>IP:</strong> <asp:Literal runat="server" ID="ltlIp" EnableViewState="false" /></div>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetInformation(Eval("ActivityData"), Eval("ObjectName"), Eval("Culture"), Eval("ObjectID"))%>
                </ItemTemplate>
                <FooterTemplate>
                    <div style="color:#bd6200"><strong>Agent:</strong> <asp:Literal runat="server" ID="ltlAgent" EnableViewState="false" /></div>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetCount(Eval("Count"), Eval("Value"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("SiteName")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetContext(Eval("Context"))%>
                </ItemTemplate>
                <FooterTemplate>
                    <strong>
                        <%#CMSVersion%></strong>
                </FooterTemplate>
            </asp:TemplateField>
            
        </Columns>
    </asp:GridView>
</div>
