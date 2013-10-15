<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<fieldset>
    <legend>Insert Notes </legend>
    <%
        var noteTypes = new List<MPXMobile.Models.NoteTypes>();

        string text = string.Empty;
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

        SelectList note = new SelectList(noteTypes, "noteTypeValue", "description");
        var noteId = Request.QueryString["noteId"];

        if (noteId != null)
        {
            var notes = MPXMobile.Models.Notes.GetAccountNotes(Session["donorNotes"].ToString());

            try
            {
                var id = Convert.ToInt64(noteId);
                var noteCurrent = notes.accountNotes.Where(n => n.noteId == id).First();

                if (noteCurrent != null)
                {
                    var texts = noteCurrent.note.Split(':');
                    if (!texts[0].ToLower().Contains(HttpContext.Current.User.Identity.Name.ToLower()))
                    { 
    %>
    <div>
        <p>
            You can only edit your own notes.</p>
    </div>
    <%
                            return;
                        }
                        note = new SelectList(noteTypes, "noteTypeValue", "description", noteCurrent.noteType);

                        if (texts.Length > 1)
                        {
                            text = texts[1].Trim();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MPXMobile.Controllers.BaseController e = new MPXMobile.Controllers.BaseController();
                    e.ErrorLog(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ERRORPATH"]), ex.Message);
    %>
    <div>
        <p>
            You can only edit your own notes.</p>
    </div>
    <%
                    return;
                }
            }
    %>
    <% using (Html.BeginForm())
       {                        
    %>
    <div>
        <p>
            <label for="firstname">
                Type:</label>
            <%= Html.DropDownList("type",note)%>
        </p>
        <p>
            <label for="lastname">
                Note:</label>
            <%= Html.TextArea("note", text, new { @class = "text-long" })%>
        </p>
        <p>
            <input id="Submit" class="forward" type="submit" value="Go" />
        </p>
        <% if (noteId != null)
           { %>
        <input type="hidden" name="noteId" value="<%=noteId %>" />
        <%} %>
    </div>
    <% } %>
</fieldset>
