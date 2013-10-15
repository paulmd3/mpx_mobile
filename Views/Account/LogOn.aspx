<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%=System.Configuration.ConfigurationManager.AppSettings["ApplicationName"].ToString()%>
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">    
    <% Html.RenderPartial("~/Views/Account/_LogOn.ascx"); %>
</asp:Content>
