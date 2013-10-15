<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="header">
    <div id="logindisplay">
        <%if (Request.IsAuthenticated)
          {%>
        <%= Html.ActionLink("Log Out", "LogOff", "Account", new { controller = "Account" }, new { @class = "postforward" })%>
        <% }
          else
          {
        %>
        <%= Html.ActionLink("Log In", "LogOn", "Account", new { controller = "Account" }, new { @class = "postforward" })%>
        <%}%>
    </div>
</div>
