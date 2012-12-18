<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Validation Error
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Validation Error</h2>
    
    <p>Please make sure that your entries do not contain HTML characters like &lt; or &gt;.</p>
    <p><a href="javascript: history.go(-1);">Go back</a></p>

</asp:Content>
