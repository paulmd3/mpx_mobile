<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SortedDictionary<string, List<MPXMobile.WipService.account>>>" %>
<h2>
    Recent Donors</h2>
<div class="panel">
    <fieldset>
        <div id="list">
            <%
                var dic = (SortedDictionary<string, List<MPXMobile.WipService.account>>)ViewData["RecentlyDonors"];
                if (dic == null || dic.Count == 0)
                { %>
            <div id="message">
                No recent activity</div>
            <% }
                else
                { %>
            <ul>
                <%  foreach (var item in dic)
                    { %>
                <li class="group">
                    <%=item.Key%></li>
                <% foreach (var acc in item.Value)
                   { %>
                <li><a>
                    <%=Html.ActionLink(acc.lastName + ", " + acc.firstName, "DonorInformation", "Home", new { donorId = acc.accountId, }, new { @class = "forward" })%></a></li>
                <%  }
                    }
                %>
            </ul>
            <%} %>
        </div>
    </fieldset>
</div>
