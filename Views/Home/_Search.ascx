<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<h2>
    Search</h2>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/search.js") %>"></script>
<script type="text/javascript">
    function searchDonors() {

        if (Validate()) {
            var href = '<%=Url.Action("SearchDonors", "Home") %>' + '?email=' + encodeURI($('#txtEmail').val())
            + '&account=' + encodeURI($('#txtAccount').val()) + '&org=' + encodeURI($('#txtOrg').val())
            + '&name=' + encodeURI($('#txtName').val()) + '&zip=' + encodeURI($('#txtZip').val());
            //_getPage(href);
            document.location.href = href;
        }
    }
</script>
<div class="panel">
    <fieldset>
        <div class="row">
            <label>
                Search</label>
            <input type="text" placeholder="search by account number" id="txtAccount" name="txtAccount" />
        </div>
    </fieldset>
</div>
<div class="panel">
    <fieldset>
        <div class="row">
            <label>
                Search</label>
            <input type="text" placeholder="search by name" id="txtName" name="txtName" />
        </div>
    </fieldset>
</div>
<div class="panel">
    <fieldset>
        <div class="row">
            <label>
                Search</label>
            <input type="text" placeholder="search by organization name " id="txtOrg" name="txtOrg" />
        </div>
    </fieldset>
</div>
<div class="panel">
    
    <fieldset>
        <div class="row">
            <label>
                Search</label>
            <input type="text" placeholder="search by email" id="txtEmail" name="txtEmail" />
        </div>
    </fieldset>
</div>
<div class="panel">
    <fieldset>
        <div class="row">
            <label>
                Search</label>
            <input type="text" placeholder="search by Zip" id="txtZip" name="txtZip" />
        </div>
    </fieldset>
    <input class="grayButton" onclick="searchDonors()" type="button" value="Search" />    
</div>