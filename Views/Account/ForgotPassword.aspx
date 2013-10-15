<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%=System.Configuration.ConfigurationManager.AppSettings["ApplicationName"].ToString()%>
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">

        $(document).ready(function() {
            //**--Clear name on click--**//
            $('#username').click(function() {
                $('#username').val('');
                $('#username').css({ 'color': 'black' });
            })
        });

    </script>

    <%= Html.ValidationSummary("Please correct the errors and try again.") %>
    <% using (Html.BeginForm())
       { %>
    <div class="content login">
        <h2>Forgot Your Password?</h2>
        <p>
            <label for="username">Username:</label>
            <%= Html.TextBox("username", "username@example.com",  new { @class = "text-long" })%>
            <%= Html.ValidationMessage("username") %>
        </p>
        <p>
            <input type="submit" value="Submit" />
        </p>
    </div>
    <% } %>
</asp:Content>
