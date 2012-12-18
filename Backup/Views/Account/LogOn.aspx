<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%=System.Configuration.ConfigurationSettings.AppSettings["ApplicationName"].ToString()%>
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">

    <% var agent = Request.UserAgent;
        if (agent.ToString().Contains(System.Configuration.ConfigurationSettings.AppSettings["Device"]))
        {
    %>
    
    <script type="text/javascript">
        $(function() {
            $("form").submit(function() {
                $('#loading').html('Authenticating...').show();
            });
        });
    </script>
    
    <% 
        }%>
    <%= Html.ValidationSummary("Login attempt was unsuccessful. Please correct the errors and try again.") %>
    <% using (Html.BeginForm())
       { %>
    <div class="content login">
        <h2>Log In</h2>
        <p>
            Enter your account information.
        </p>
        <p>
            <label for="username">
                Username:</label>
            <%= Html.TextBox("username", "juan.pablo@morphodev.com",  new { @class = "text-long", placeholder = "email@example.com" })%>
            <%= Html.ValidationMessage("username") %>
        </p>
        <p>
            <label for="password">
                Password:</label>
            <%= Html.Password("password", "", new { @class = "text-long" })%>
            <%= Html.ValidationMessage("password") %>
        </p>
        <p>
            <%= Html.CheckBox("rememberMe") %>
            <label class="inline" for="rememberMe">
                Remember me?</label>
        </p>
        <p>
            <input type="submit" value="Log On" />
        </p>
        <p>
            <%= Html.ActionLink("Forgot your password?", "ForgotPassword", "Account")%>
        </p>
    </div>
    <% } %>


</asp:Content>
