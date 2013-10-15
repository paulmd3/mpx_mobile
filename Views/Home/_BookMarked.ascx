<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SortedDictionary<string, List<MPXMobile.WipService.account>>>" %>
<h2>
    Bookmarked Donors</h2>
<div class="panel">
    <fieldset>
        <div id="list">
            <%if (Model.Count == 0)
              { %>
            <div id="message">
                No donors have been bookmarked</div>
            <%}
              else
              { %>
            <ul id="donors" title="Donors">
                <%
                  foreach (var item in Model)
                  {%>
                <li class="group">
                    <%=item.Key%></li>
                <%  foreach (var acc in item.Value)
                    {
                %>
                <li>
                    <%=Html.ActionLink(acc.lastName + ", " + acc.firstName, "DonorInformation", "Home", new { donorId = acc.accountId, }, new { @class = "forward" })%>
                    <%=Html.ActionLink("Unbookmark", "UnBookMarkDonor", "Home", new { accountId = acc.accountId, }, new { @class = "forward btn_chrome", @style = "width:132px;text-align:center;" })%>
                </li>
                <%  }
                  } %>
            </ul>
            <%} %>
        </div>
    </fieldset>
</div>
