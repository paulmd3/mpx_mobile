<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    InsertNotesInformation
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <fieldset>
        <legend>Insert Notes </legend>
        <%var agent = Request.UserAgent;    %>
        <%
            var noteTypes = new List<MPXMobile.Models.NoteTypes>();

            ///Read XML file
            System.Xml.XmlTextReader xtr = new System.Xml.XmlTextReader(Server.MapPath("/NoteTypes.xml"));
            xtr.WhitespaceHandling = System.Xml.WhitespaceHandling.None;
            System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
            xDoc.Load(xtr);
            var z = xDoc.SelectSingleNode("types").ChildNodes.Count;


            ///Load the Types
            for (int i = 1; i < z; i++)
            {
                var notes = new MPXMobile.Models.NoteTypes();
                notes.description = xDoc.SelectSingleNode("// types/type[" + i + "]/noteTypeValue").InnerText;
                notes.noteTypeValue = xDoc.SelectSingleNode("// types/type[" + i + "]/noteTypeValue").InnerText;
                noteTypes.Add(notes);
            }
    
        %>
        <% using (Html.BeginForm())
           {
               string donorId = Request.QueryString["accountId"];
               string noteId = (Request.QueryString["noteId"]);
               Session.Add("donorNotes", donorId);
        %>
        <div>
            <p>
                <label for="firstname">
                    Type:</label>
                <%SelectList note = new SelectList(noteTypes, "noteTypeValue", "description");
                %>
                <%= Html.DropDownList("type",note)%>
            </p>
            <p>
                <label for="lastname">
                    Note:</label>
                <%= Html.TextArea("note", "", new { @class = "text-long" })%>
            </p>
            <p>
                <%
                    if (noteId == null)
                    { %>
                <%if (!agent.ToString().Contains(System.Configuration.ConfigurationSettings.AppSettings["Device"]))
                  { %><input id="btnSubmit" type="submit" value="Go" onclick="javascript:var type = $('#type').val(); var note=$('#note').val();var donorId=request.querystring('accountId');
            
                    document.location = '<%=Url.Action("InsertNotesInformation", "Home") %>'+'?type=' + type+'?note=' + note+'?donorId=' + donorId; " />
                <%}
                  else
                  {%>
                <input class="forward" id="btnSubmit" type="submit" value="Go" onclick="javascript:var type = $('#type').val(); var note=$('#note').val();var donorId=request.querystring('accountId');
                
                 $.get('/Home/InsertNotesInformation?type=' + type+'?note='+note+'?donorId='+donorId,null, function(data) {
                 $('body').html(data);
             
         );
             
                    };" />
                <%} %>
                <%} %>
                <% else
                    {%>
                <%if (!agent.ToString().Contains(System.Configuration.ConfigurationSettings.AppSettings["Device"]))
                  { %><input id="Submit1" type="submit" value="Go" onclick="javascript:var type = $('#type').val(); var note=$('#note').val();var donorId=request.querystring('accountId');var noteId=request.querystring('noteId');
            
                    document.location = '<%=Url.Action("EditNotesInformation", "Home") %>'+'?type=' + type+'?note=' + note+'?donorId=' + donorId+'?noteId='+noteId; " />
                <%}
                  else
                  {%>
                <input class="forward" id="Submit2" type="submit" value="Go" onclick="javascript:var type = $('#type').val(); var note=$('#note').val();var donorId=request.querystring('accountId');var noteId=request.querystring('noteId');
                
                 $.get('/Home/EditNotesInformation?type=' + type+'?note='+note+'?donorId='+donorId+'?noteId='+noteId,null, function(data) {
                 $('body').html(data);
             
         );
             
                    };" />
                <%} %>
                <%} %>
            </p>
        </div>
        <% } %>
    </fieldset>
</asp:Content>
