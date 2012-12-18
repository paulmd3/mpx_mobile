<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    var bookList = ((List<MPXMobile.Models.BookmarkedDonor>)ViewData["BookmarkedDonors"]);

    List<MPXMobile.WipService.account> accounts = new List<MPXMobile.WipService.account>();

    foreach (var item in bookList)
    {
        var account = MPXMobile.Models.Donors.GetAccount(item.Donor_ID.ToString());
        accounts.Add(account);
    }
%>
<h2>
    Bookmarked Donors</h2>
<div class="panel">
    <fieldset>
        <div id="list">
            <%if (bookList.Count == 0)
              { %>
            No donors have been bookmarked
            <%}
              else
              { %>
            <ul id="donors" title="Donors">
                <% 
                    ///array of alph directory
                    string[] abc = new string[300];
                    var list = accounts.OrderBy(r => r.lastName);
                    int i = 0;

                    ///Begin letter
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

                <script type="text/javascript">
                    var b = Bookmark("<%= item.accountId %>", "<%= item.lastName %>", "<%= item.firstName %>", "<%= item.firstName %>", "<%= item.defaultPhoneNumber.Trim() %>", "", "<%= item.defaultEmailAddress %>", "<%= item.webSite %>");
                    Bookmarks.add(b);
                </script>

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
                    foreach (var listDirectory in abc)
                    {
                        if (listDirectory != null)
                        {
                            if (listDirectory.ToString() == letter.ToString())
                            {
                                var dictionaryLetter = letter.ToString().ToUpper();
                %>
                <li class="group">
                    <%=dictionaryLetter%></li>
                <%
                    int z = 0;
                    foreach (var dictionary in abc)
                    {
                        if (dictionary == listDirectory)
                        {
                            abc[z] = "";
                        }
                        z = z + 1;
                    }
                        }
                    }
                }   %>
                <li>
                    <%=Html.ActionLink(name, "DonorInformation", "Home",
                    new
                    {
                        donorId = item.accountId,

                    }, new { @class = "newforward" })%></li>
                <%}
            } %>
            </ul>
            <%} %>
        </div>
    </fieldset>
</div>
