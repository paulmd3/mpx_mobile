<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<h2>Recent Donors</h2> 
<div class="panel">
    <fieldset>
<%
    var accounts = ((List<MPXMobile.WipService.account>)ViewData["RecentlyDonors"]);  
%>
    <div id="list">

    <%
        if (accounts == null || accounts.Count == 0)
        { %>
            <div id="message">No recent activity</div>
        <%
        }
        else
        { %>
        
        <ul>
        <% 
     if (accounts != null)
     {
         string[] abc = new string[300];
         var list = accounts.OrderBy(r => r.lastName);
         int i = 0;
         char letter = 'a';

         ///generate the directory, first letters of the names
         foreach (var items in list)
         {
             if (items.lastName != "")
             {
                 string name;

                 name = items.lastName;

                 letter = name[0];
                 abc[i] = letter.ToString();
                 i = i + 1;
             }
         }

         foreach (var item in list)
         {%>
        <%if (item.lastName != "")
          {
              string name;
              name = item.lastName + ", " + item.firstName;
        %>
        <%
     //first letter of the name
     letter = name[0];

     ///foreach list of the directory and verify if the letter exists
     ///abc= string array of the directory
     foreach (var ab in abc)
     {
         if (ab != null)
         {
             if (ab.ToString() == letter.ToString())
             {
                 var dictionaryLetter = letter.ToString().ToUpper();
        %>
        <li class="group">
            <%=dictionaryLetter%></li>
        <%
     int z = 0;
     foreach (var dictionary in abc)
     {
         if (dictionary == ab)
         {
             abc[z] = "";

         }
         z = z + 1;
     }
             }

         }
     }   %>
        <li><a>
            <%=Html.ActionLink(name, "DonorInformation", "Home",
                    new
                    {
                        donorId = item.accountId,

                    }, new { @class = "newforward" })%></a></li>
        <%}
         }
     }%>
    </ul>
    <%} %>
        </div>
    </fieldset>
</div>