<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_QueryLog"
    CodeFile="QueryLog.ascx.cs" %>
<%@ Import Namespace="CMS.SettingsProvider" %>
<div class="DebugLog">
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <asp:GridView runat="server" ID="gridQueries" EnableViewState="false" GridLines="None"
        AutoGenerateColumns="false" CellPadding="3" ShowFooter="true">
        <AlternatingRowStyle CssClass="AltRow" />
        <FooterStyle CssClass="Footer" />
        <Columns>
            <asp:TemplateField>
                <ItemStyle CssClass="Index" />
                <ItemTemplate>
                    <strong>
                        <%#GetIndex(Eval("IsInformation"), Eval("QueryResultsSize"), Eval("QueryParametersSize"), Eval("QueryName"), Eval("QueryText"))%>
                        <%#GetDuplicity(Eval("Duplicit"), Eval("QueryText"))%>
                        <%#GetConnectionString(Eval("ConnectionString"))%>
                    </strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetInformation(Eval("IsInformation"), Eval("ConnectionString"), Eval("ConnectionOp"), Eval("QueryName"), Eval("QueryText"), Eval("QueryParameters"), Eval("QueryParametersSize"), Eval("QueryResults"), Eval("QueryResultsSize"), MaxSize)%>
                </ItemTemplate>
                <FooterTemplate>
                    <strong>
                        <%#String.Format(ResHelper.GetString("QueryLog.Total", null, "QueryLog.Total {0} / {1} / {2}"), index, DataHelper.GetSizeString(TotalParamSize), DataHelper.GetSizeString(TotalSize))%>
                    </strong>
                </FooterTemplate>
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
            <asp:TemplateField>
                <HeaderStyle CssClass="Dur" />
                <ItemStyle CssClass="Dur" />
                <FooterStyle CssClass="Dur" />
                <ItemTemplate>
                    <%#GetDuration(Eval("QueryDuration"))%><br />
                    <%#GetDurationChart(Eval("QueryDuration"), 0.005, 0, 0)%>
                </ItemTemplate>
                <FooterTemplate>
                    <strong>
                        <%#TotalDuration.ToString("F3")%>
                    </strong>
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
