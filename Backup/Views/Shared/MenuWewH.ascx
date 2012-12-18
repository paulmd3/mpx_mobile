<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
function newmenu(cmenu) 
{
    $('#home').className = '';
    $('#password').className = '';
    $('#search').className = '';
    $('#bookmarked').className = '';
    $('#forgot').className = '';
    $('#'+ cmenu).className = 'selected';
}
</script>

<ul id="menu">
    <%if (System.Threading.Thread.CurrentPrincipal.Identity.Name == (System.Configuration.ConfigurationSettings.AppSettings["Admin"]))
      { %>
    <% if (Request.Url.ToString().Contains("Register"))
       { %>
    <li id="home" class="selected">
        <%= Html.ActionLink("New User", "Register", "Account")%></li>
    <%}
       else
       { %>
    <li id="register" onclick="newmenu('home');">
        <%= Html.ActionLink("New User", "Register", "Account")%></li>
    <%} %>
    <%} %>
    
    <li id="search"<% if (Request.Url.ToString().Contains("Search")) { %> class="selected" <%} %>>
        <%= Html.ActionLink("Search Donors", "Search", "Home")%>
    </li>
    <li id="bookmarked"<% if (Request.Url.ToString().Contains("Bookmark")) { %> class="selected" <%} %>>
        <%= Html.ActionLink("Bookmarked", "BookmarkedDonors", "Home")%>
     </li>
    <li id="recently"<% if (Request.Url.ToString().Contains("Recently")){ %> class="selected" <%} %>>
        <%= Html.ActionLink("Recent", "RecentlyDonors", "Home")%>
    </li>
    <li id="password"<% if (Request.Url.ToString().Contains("ChangePassword")) { %> class="selected" <%} %>>
        <%= Html.ActionLink("Account", "ChangePassword", "Account")%>
    </li>
    <li class="logout">
        <% Html.RenderPartial("LogOnUserControl"); %>
    </li>
</ul>
