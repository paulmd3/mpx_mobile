<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MPXMobile.Models.GiftInfo>" %>
<h2>
    Gift Info for
    <%=Model.Donor.firstName + " " + Model.Donor.lastName%></h2>
<div class="panel gifts">
    <fieldset>
        <%
            if (Model.Gifts.Count() == 0)
            { %>
        <div id="message">
            No gift information for
            <%= Model.Donor.firstName%>
        </div>
        <%}
            else
            {
                foreach (var year in Model.Gifts)
                {
        %>
        <h3>
            <%= year.Key %>
        </h3>
        <% foreach (var item in year.Value)
           {%>
        <ul>
            <li>Gift Annual ID:
                <%=item.giftAnnualId %></li>
            <li>Gift Amount:
                <%=item.amount.ToString("N2")%></li>
            <li>Currency Code:
                <%=item.currencyCode.ToString()%></li>
            <li>Conversion Amount:
                <%=item.conversionAmount.ToString("N2")%></li>
            <li>Net Gift Amount:
                <%=item.netGiftAmount.ToString("N2")%></li>
            <li>Net Conversion Amount:
                <%=item.netGiftConversionAmount.ToString("N2")%></li>
            <li>Deductible:
                <%=item.deductible.ToString()%></li>
            <li>Gift Count:
                <%=item.giftCount.ToString()%></li>
            <li>Related Giving:
                <%=item.relatedGiving.ToString()%></li>
            <li>Premium Amount:
                <%=item.premiumAmount.ToString("N2")%></li>
        </ul>
        <br />
        <%}
                    }
            }%>
    </fieldset>
</div>
