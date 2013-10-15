<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MPXMobile.Models.DonorDetail>" %>
<script type="text/javascript">
    $('a[.postforward]').bind('click', function (event) {
        //event.preventDefault();
        window.location.replace = $(this).attr('href');
        return false;
    });
</script>
<h2>
    Donor Information</h2>
<div class="panel">
    <h3>
        <%=Model.Donor.title +" "+ Model.Donor.lastName +", "+ Model.Donor.firstName %>
        <a href="<%=(ViewData["IsBookmarked"]!=null && ((bool)ViewData["IsBookmarked"]))?Url.Action("UnBookMarkDonor", "Home", new {accountId = Model.Donor.accountId}):Url.Action("BookMarkDonor", "Home", new {accountId = Model.Donor.accountId})%>"
            class="forward btn_chrome"><%if(ViewData["IsBookmarked"]!=null && ((bool)ViewData["IsBookmarked"])){Response.Write("Unbookmark");}else{Response.Write("Bookmark");} %></a><br />
            <span style="font-size: 12px; font-weight: normal; margin-top: 5px">Account Number: <%=Model.Donor.accountId.ToString() %></span>
    </h3>
    <input type="hidden" id="account" value="<%=Model.Donor.accountId %>" />
    <ul>
        <% foreach (MPXMobile.WipService.accountEmailAddress email in Model.Emails)
           {
               if (!string.IsNullOrEmpty(email.emailAddress.Trim()))
               {%>
        <li class="arrow"><a class="postforward email" href="mailto:<%=email.emailAddress.Trim() %>">
            <%=email.emailAddress.Trim()%>
        </a></li>
        <%      }
           }
           foreach (MPXMobile.WipService.accountPhoneNumber phone in Model.Phones)
           {
               if (!string.IsNullOrEmpty(phone.phoneNumber))
               {%>
        <li class="arrow"><a class="postforward phone" href="tel:<%=phone.phoneNumber.Trim().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") %>"
            onclick="return (navigator.userAgent.indexOf('iPhone') != -1)">
            <%=phone.phoneNumber.Trim()%>
        </a></li>
        <%      }
           }
           foreach (MPXMobile.WipService.accountAddress address in Model.Addresses)
           {
        %>
        <li class="arrow"><a class="postforward address" target="_blank" href="http://maps.google.com/maps?q=<%=address.address1 + ", " + address.city + " " + address.state %>">
            <%=address.address1%>
            <%=address.city%>
            <%=address.state%>
            <%=address.zipCode%>
            <%=address.country%>
        </a></li>
        <%  }
            if (Model.Donor.isOrganization && !string.IsNullOrEmpty(Model.Donor.organizationName))
            { %>
        <li class="arrow"><a id="titleO" style="background-image:none;">Organization: <%=Model.Donor.organizationName%></a>
        </li>
        <%  }
            if (!string.IsNullOrEmpty( Model.Donor.jobTitle))
            {%>
        <li class="arrow"><a style="background-image:none;">JobTitle:
            <%=Model.Donor.jobTitle%></a> </li>
        <%  }
            if (!string.IsNullOrEmpty( Model.Donor.webSite))
            {%>
        <li class="arrow website"><a class="postforward" target="_blank" href="http://<%=Model.Donor.webSite%>">
            <%=Model.Donor.webSite%>
        </a></li>
        <%} %>
    </ul>
    <a href="<%=Url.Action("GiftInformation", "Home", new {accountId = Model.Donor.accountId})%>"
        class="forward grayButton">Gifts</a> <a href="<%=Url.Action("NotesInformation", "Home", new {accountId = Model.Donor.accountId})%>"
            class="forward grayButton">Notes</a>
</div>
