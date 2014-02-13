

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server" language="C#" >

    protected void Button1_Click(object sender, EventArgs e)
    {

        //System.Net.Mail.MailAddress a = new System.Net.Mail.MailAddress("pankaj.malik@skysoft.net.in");
        //System.Net.Mail.MailAddress a2 = new System.Net.Mail.MailAddress("pankaj.malik@skysoft.net.in");
        //System.Net.Mail.MailMessage s = new System.Net.Mail.MailMessage(a2, a);
        //s.Subject = "title";
        //s.Body = "test";


        //s.IsBodyHtml = true;
        //System.Net.Mail.SmtpClient k = new System.Net.Mail.SmtpClient();
        //k.Host = "mail.nysolutions.com";
        //k.Timeout = 99999;
        //k.Send(s);
        //k.Port = 25;
        //s.Dispose();









        string smtpServer = Convert.ToString(DotNetNuke.Common.Globals.HostSettings["SMTPServer"]);
        string smtpAuthentication = Convert.ToString(DotNetNuke.Common.Globals.HostSettings["SMTPAuthentication"]);
        string smtpUsername = Convert.ToString(DotNetNuke.Common.Globals.HostSettings["SMTPUsername"]);
        string smtpPassword = Convert.ToString(DotNetNuke.Common.Globals.HostSettings["SMTPPassword"]);
        string xResult = DotNetNuke.Services.Mail.Mail.SendMail("support@nysolutions.com", "pankaj.malik@skysoft.net.in", "", "",
        DotNetNuke.Services.Mail.MailPriority.Normal, "This is the Subject!",
        DotNetNuke.Services.Mail.MailFormat.Text, System.Text.Encoding.UTF8, "This is the Body", "",
        smtpServer, smtpAuthentication, smtpUsername, smtpPassword);




    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
   
     <asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click"  />
    </div>
    </form>
</body>
</html>