<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SortedDictionary<string, List<MPXMobile.WipService.account>>>" %>
<fieldset>
    <legend>Donors</legend>
    <div id="list">
        <%if (Model.Count == 0)
          { %>
        <div id="message">
            No results found, please try again
        </div>
        <%}
          else
          { %>
        <ul id="donors" title="Donors">
            <% 
              int count = 1;
              string style = string.Empty;
              foreach (var list in Model)
              {
            %>
            <li class="group">
                <%=list.Key%></li>
            <%            
                  foreach (var acc in list.Value)
                  {
                      if (count % 2 != 0)
                      {
                          style = "class='sec'";
                      }
                      count++;
            %>
            <li <%=style %>>
                <%=Html.ActionLink(acc.lastName + ", " + acc.firstName, "DonorInformation", "Home", new { donorId = acc.accountId, }, new { @class = "forward" })%>
                <span style="font-size: 12px; font-weight: normal; margin-top: 5px">Account Number:
                    <%=Html.Encode(acc.accountId)%>&nbsp;&nbsp;&nbsp;Address:
                    <%=Html.Encode(acc.city + ", " + acc.state)%>
                    <% if (acc.isOrganization && !string.IsNullOrEmpty(acc.organizationName))
                       { %>
                    &nbsp;&nbsp;&nbsp;Organization:
                    <%=Html.Encode(acc.organizationName)%>
                    <%}%>
                </span></li>
            <%
                  }
              } %>
        </ul>
        <%} %>
    </div>
</fieldset>
