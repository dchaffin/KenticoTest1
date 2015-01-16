<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Detail.aspx.cs" Inherits="Views_Global_NewsMVC_Detail"
    MasterPageFile="Root.master" %>

<asp:Content ID="cntMain" ContentPlaceHolderID="plcMain" runat="Server">
    <div class="innerContent">
        <h1>
            MVC Example</h1>
        <p>
            This is a sample for the page rendered by MVC. It displays the news detail. The detail is provided by <strong>NewsMVC</strong> controller and <strong>~/Views/Global/NewsMVC/Detail.aspx</strong> view and routed through MVC URL pattern <strong>/NewsMVC/Detail/{id}</strong> added as document alias to the same document as the main document. This way, there is no need to provide separate documents for detail view.
        </p>
        <div class="LightGradientBox ">
            <h2>
                <%=Document.GetValue("NewsTitle")%></h2>
            <p>
                <%=Document.GetValue("NewsText")%></p>
            <p>
                <a href="<%=ResolveUrl("~/newsmvc/list/")%>">Back to the list of news</a>
            </p>
        </div>
    </div>
    </div>
</asp:Content>
