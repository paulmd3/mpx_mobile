<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%
    var accounts = ((List<MPXMobile.WipService.account>)ViewData["donors"]);
%>
<fieldset>
    <legend>Donors</legend>
    <div id="list">
    <%if (accounts.Count == 0) { %>
        <div id="message">
            No results found, please try again
        </div>
    <%} else { %>
        <ul id="donors" title="Donors">
            <% 
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
        int count = 1;
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
            <%if (count % 2 == 0)
              { %>
            <li><a>
                <%=Html.ActionLink(name, "DonorInformation", "Home",
                new
                {
                    donorId = item.accountId,

                }, new { @class = "forward" })%></a></li>
            <%} %>
            <% else
        {%>
            <li class="sec"><a>
                <%=Html.ActionLink(name, "DonorInformation", "Home",
                new
                {

                    donorId = item.accountId,

                }, new { @class = "forward" })%></a></li>
            <%} %>
            <%}
              count = count + 1;
        } %>
        </ul>
        <%} %>
    </div>
</fieldset>