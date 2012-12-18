<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    var gift = ((MPXMobile.WipService.getGiftSummariesResponse)ViewData["account"]);
    var accountId = Request.QueryString["accountId"].ToString();
    var donor = MPXMobile.Models.Donors.GetAccount(accountId);
%>

<h2>Gift Info for <%=donor.firstName + " " + donor.lastName %></h2>
<div class="panel gifts">
<fieldset>
<%
    if (gift.giftSummaries.Count() == 0)
    { %>
        <div id="message">
            No gift information for <%= donor.firstName %>
        </div>
        <%}
    else
    {%>
    
       <% foreach (var item in gift.giftSummaries) { %>
    <h3>
        <%= item.amount.ToString() + " " + item.currencyCode %> on <%=item.firstGiftDate.ToShortDateString()%>
    </h3>
    <ul>
        <li>Net Gift Amount: <%=item.netGiftAmount.ToString()%></li>
        <li>Conversation Amount: <%=item.conversionAmount.ToString()%></li>
        <li>Deductible:<%=item.deductible.ToString()%></li>
        <li>First Gift Amount:<%=item.firstGiftAmount.ToString()%></li>
        <li>First Gift Conversion Amount:<%=item.firstGiftConversionAmount.ToString()%></li>
        <li>Gift Count:<%=item.giftCount.ToString()%></li>
        <li>Large Gift Date:<%=item.largeGiftDate.ToString()%></li>
        <li>Last Gift Conversion Amount:<%=item.lastGiftConversionAmount.ToString()%></li>
        <li>Last Gift Date:<%=item.lastGiftDate.ToString()%></li>
        <li>Net Gift Amount:<%=item.netGiftAmount.ToString()%></li>
        <li>Related Giving:<%=item.relatedGiving.ToString()%></li>
        <li>Net Conversation Amount:<%=item.netGiftConversionAmount.ToString()%></li>
        <li>Premium Amount:<%=item.premiumAmount.ToString()%></li>
        <li>Large Gift Amount:<%=item.largeGiftAmount.ToString()%></li>
        <li>Large Gift Conversation Amount:<%=item.largeGiftConversionAmount.ToString()%></li>
        <li>Last Gift Amount:<%=item.lastGiftAmount.ToString()%></li>
    </ul>
    <%}
   }%>
</fieldset>
</div>