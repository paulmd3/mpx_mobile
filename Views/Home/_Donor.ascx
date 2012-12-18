<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% var account = ((MPXMobile.WipService.account)ViewData["account"]); %>

<script type="text/javascript">
    $('a[href^=mailto]').live('click', function(event) {
        event.preventDefault();
        window.location.replace = $(this).attr('href');
        return false;
    });
</script>

<h2>Donor Information</h2>
<div class="panel">
    <h3>
        <%=account.title +" "+ account.lastName +", "+ account.firstName %>
        <a href="<%=Url.Action("BookMarkDonor", "Home", new {accountId = account.accountId})%>" class="forward btn_chrome">Bookmark</a>
    </h3>
    <input type="hidden" id="account" value="<%=account.accountId %>" />
    <ul>
        
        <% if (account.defaultEmailAddress != string.Empty){%>
        <li class="arrow email">
            <a class="postforward" href="mailto:<%=account.defaultEmailAddress %>">
                <%=account.defaultEmailAddress.Trim()%>
            </a>
        </li>
        <%} %>
        
        <%if (account.defaultPhoneNumber.Trim() != string.Empty) {%>
        <li class="arrow phone">
            <a class="postforward" href="tel:<%=account.defaultPhoneNumber.Trim() %>" onclick="return (navigator.userAgent.indexOf('iPhone') != -1)">
            <%=account.defaultPhoneNumber.Trim()%>
            </a>
        </li>
        <%} %>
        
        <li class="arrow address">
            <%
                var agent = Request.UserAgent;
                var direct = account.address1 + "," + account.city;

                if (agent.ToString().Contains(System.Configuration.ConfigurationSettings.AppSettings["Device"]))
                {%>
            <a class="postforward" href="http://maps.google.com/maps?q=<%=direct%>">
                <%= account.address1 %> <%=account.city %> <%= account.state %> <%= account.zipCode %> <%= account.country %>
            </a>
            <%}
                else
                { %>
            <a class="address" target="_blank" href="http://maps.google.com/maps?q=<%=direct%>">
                <%=account.address1%> <%=account.city %> <%=account.state %> <%=account.zipCode %> <%=account.country %></a>
            <% }%>
        </li>
        <%if (account.isOrganization)
          { %>
        <li class="arrow">
            <span id="titleO">Organization: Name:<%=account.organizationName%></span>
        </li>
        <li class="arrow">
            <span>JobTitle: <%=account.jobTitle%></span>
        </li>
        <%} %>
        
        <% if (account.webSite != string.Empty) {%>
            <li class="arrow website">
                <a class="postforward" target="_blank" href="http://<%=account.webSite%>">
                    <%=account.webSite%>
                </a>
            </li>
        <%} %>
    </ul>
 
    <a href="<%=Url.Action("GiftInformation", "Home", new {accountId = account.accountId})%>" class="forward grayButton">Gifts</a>

    <a href="<%=Url.Action("NotesInformation", "Home", new {accountId = account.accountId})%>" class="forward grayButton">Notes</a>
</div>