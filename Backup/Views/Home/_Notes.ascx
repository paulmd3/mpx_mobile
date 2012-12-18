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
    var accountId = Request.QueryString["accountId"].ToString();
    var donor = MPXMobile.Models.Donors.GetAccount(accountId); 
%>

<h2>Notes for <%=donor.firstName + " " + donor.lastName %></h2> 
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
    foreach (var item in notes.accountNotes.OrderByDescending(p=>p.nextDate))
    {          
%>
    <li>
        <% try { %>
        <%=item.note.ToString()%><br />
        <em class="note-info"><%=item.noteType.ToString()%> note on <%//=item.nextDate.Value.ToShortDateString()%></em>
        
        <%//=item.nextActionNote.ToString()%>
        
        <%var urlI1 = "InsertNotesInformation?noteId="+item.noteId+"&accountId=" + Request.QueryString["accountId"];%>
        
        <% if (item.note.ToString().Split(':').First().Contains(HttpContext.Current.User.Identity.Name)) {%>
            <a class="forward btn_chrome" href="<%=Url.Action(urlI1, "Home")%>" >Edit</a>
            <a class="forward btn_chrome deleteNote" rel="<%=item.noteId %>">Delete</a>
        <%}%>
                
      <%}
        catch (Exception ex)
        {
            MPXMobile.Controllers.BaseController e = new MPXMobile.Controllers.BaseController();
            e.ErrorLog(Server.MapPath(System.Configuration.ConfigurationSettings.AppSettings["ERRORPATH"]), ex.Message);
        } %>
    </li>
    <%}%>
</ul>    
<%} %>
</fieldset>
</div>

<%var urlI = "InsertNotesInformation?accountId=" + Request.QueryString["accountId"];%>
<a class="newforward grayButton" href="<%=Url.Action(urlI, "Home")%>" >Add a Note</a>