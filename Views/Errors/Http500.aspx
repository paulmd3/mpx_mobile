<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Server Error</title>

    <style type="text/css">
        body{background-color:#F2F9FC;font-size:.75em;font-family:Helvetica-Neue,Helvetica,Sans-Serif;margin:0 auto;padding:0;color:#696969;width:500px;}
        h1 {color: #000; font-size: 17px}
        #logo { margin: auto; text-align: center; margin-top: 1em;}
        .message {margin-top: 3em; border-radius: 3px; padding: 1em; border: 1px solid #EDEDED; background: #fff}
    </style>
</head>
<body>
    <div class="message">
        <h1>Server Error</h1>
        <p>Try your request again or <a href="javascript: history.go(-1)">go back to the previous page</a>.</p>
    </div>
    <div id="logo">
        <img alt="MPX" src="/Content/images/orangeleap.gif" />
    </div>
</body>
</html>
