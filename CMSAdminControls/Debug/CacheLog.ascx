<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_CacheLog"
    CodeFile="CacheLog.ascx.cs" %>
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
                    <%#HttpUtility.HtmlEncode(ValidationHelper.GetString(Eval("CacheOperation"), ""))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetInformation(Page, Eval("CacheKey"), Eval("Dependencies"), Eval("CacheValue"), Eval("CacheOPERATION"))%>
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
