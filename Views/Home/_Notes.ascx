<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

 <script type="text/javascript">

     $(function() {
        $(".deleteNote").click(function() {
             if (confirm('Delete note?') == true) {
                 //var account = $('#account').val();
                 var note = $(this).attr("rel");
                 document.location = '<%=Url.Action("DeleteNote", "Home") %>' + '?noteId=' + note;
             }
         });
     });

     $('a[href^=mailto]').click(function(event) {
         event.preventDefault();
         window.location.replace = $(this).attr('href');
         return false;
     });

    </script>
<% 
    var accountId = Convert.ToInt64(Request.QueryString["accountId"]);
    var donor = new MPXMobile.Models.ApiServiceDonors().GetAccount(accountId); 
%>

<h2>Notes for <%=donor.firstName + " " + donor.lastName %></h2> 

<a class="newforward grayButton" href="<%=Url.Action("InsertNotesInformation", "Home")%>" style=" margin-top:20px;" >Add a Note</a>
<div class="panel notes">
<fieldset>

<%
    var notes = ((MPXMobile.WipService.getAccountNotesResponse)ViewData["notes"]);
    if (notes.accountNotes.Count() == 0)
{ %>
    <div id="message">
        No notes for <%=donor.title + " " + donor.firstName %>
    </div>
<%
} else {%>

<ul>
<%
    foreach (var item in notes.accountNotes.OrderByDescending(p=>p.addDate))
    {          
%>
    <li>
        <% try { %>
        <%=item.note.ToString()%><br />
        <em class="note-info"><%=item.noteType%> note on <%=item.addDate.ToShortDateString()%></em>
        
        <%//=item.nextActionNote.ToString()%>
        
        <%var urlI1 = "InsertNotesInformation?noteId="+item.noteId;%>
        
        <% if (item.note.ToString().ToLower().Split(':').First().Contains(HttpContext.Current.User.Identity.Name.ToLower())) {%>
            <a class="forward btn_chrome" href="<%=Url.Action(urlI1, "Home")%>" >Edit</a>
            <a class="forward btn_chrome deleteNote" rel="<%=item.noteId %>">Delete</a>
        <%}%>
                
      <%}
        catch (Exception ex)
        {
            MPXMobile.Controllers.BaseController e = new MPXMobile.Controllers.BaseController();
            e.ErrorLog(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ERRORPATH"]), ex.Message);
        } %>
    </li>
    <%}%>
</ul>    
<%} %>
</fieldset>
</div>