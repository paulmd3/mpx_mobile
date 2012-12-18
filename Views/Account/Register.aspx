<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    X Nonprofit MPX-Register
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.ValidationSummary("Account creation was unsuccessful. Please correct the errors and try again.") %>
    <% using (Html.BeginForm())
       { %>
    <div>
        <fieldset>
            <legend>Account Information</legend>
            <h2>
                Create a New Account</h2>
            <p>
                Use the form below to create a new account.
            </p>
            <p>
                Passwords are required to be a minimum of
                <%=Html.Encode(ViewData["PasswordLength"])%>
                characters in length.
            </p>
            <p>
                <label for="firstname">
                    First Name:</label>
                <%= Html.TextBox("firstname", "", new { @class = "text-long" })%>
                <%= Html.ValidationMessage("firstname")%>
            </p>
            <p>
                <label for="lastname">
                    Last Name:</label>
                <%= Html.TextBox("lastname", "", new { @class = "text-long" })%>
                <%= Html.ValidationMessage("lastname")%>
            </p>
            <p>
                <label for="email">
                    Email:</label>
                <%= Html.TextBox("email", "", new { @class = "text-long" })%>
                <%= Html.ValidationMessage("email") %>
            </p>
            <p>
                <label for="password">
                    Password:</label>
                <%= Html.Password("password", "", new { @class = "text-long" })%>
                <%= Html.ValidationMessage("password") %>
            </p>
            <p>
                <label for="confirmPassword">
                    Confirm password:</label>
                <%= Html.Password("confirmPassword", "", new { @class = "text-long" })%>
                <%= Html.ValidationMessage("confirmPassword") %>
            </p>
            <p>
                <input id="btnSubmit1" type="submit" value="Register" />
            </p>
        </fieldset>
    </div>
    <% } %>
</asp:Content>
