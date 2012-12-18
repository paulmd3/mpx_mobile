<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%var agent = Request.UserAgent; %>
    <h2>Search</h2>



<script type="text/javascript">
  
    function searchDonors() {    
        var keyword = $('#txtSearch').val();

        if (keyword == '') {
            alert('Enter a term to search');
        }
        else {
            $('#loading').html('Searching...').show();
            var href = '<%=Url.Action("SearchDonors", "Home") %>' + '?keyword=' + encodeURI(keyword);

<%if (!agent.ToString().Contains(System.Configuration.ConfigurationSettings.AppSettings["Device"])) {%>            

            document.location = href;

<%} else {%>

            getPage(href);

<%}%>
        };
    }

</script>

<div class="panel">
    <fieldset>
        <div class="row">
            <label>Search</label>
            <input type="text" placeholder="search by name or email" id="txtSearch" name="txtSearch" />
        </div>
    </fieldset>
    <input class="forward grayButton" onclick="searchDonors()" type="button" value="Search" />
</div>