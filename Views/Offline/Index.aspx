<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Offline Mode
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function () {
            $('#dim').css('height', $(document).height());

            Bookmarks.list();
        });
    </script>

    <script id="bookmarks_list_template" type="text/x-jquery-tmpl">
        <li data-donor-id="${id}">
            <a onclick="donorDetails(this)">
                <strong>${last_name}, ${first_name}</strong>
            </a>
        </li>
    </script>

    <script id="bookmark_details_template" type="text/x-jquery-tmpl">
        <div class="card">
            <span>Contact Info</span>
            <h3>${last_name}, ${first_name}</h3>
            <ul>
                <li class="arrow email"><a href="mailto:${email}">${email}</a></li>
                <li class="arrow phone"><a href="tel:${phone}">${phone}</a></li>
                <li class="arrow address"><a href="http://maps.google.com/maps?q=${address}">${address}</a></li>
                <li class="arrow website"><a target=_blank href="${url}">${url}</a></li>
            </ul>
        </div>
    </script>

    <h2 id="status">Bookmarks</h2>

    <div class="panel" style="position: relative">
        <fieldset>
            <div id="list">
                <ul id="__donors">
                    <li class="group">A</li>
                </ul>
            </div>
        </fieldset>
    </div>

</asp:Content>
