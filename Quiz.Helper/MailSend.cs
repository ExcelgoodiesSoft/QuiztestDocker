using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Helper
{
    public static class MailSend
    {
        public static int SendEmail(string ToMail, string CC, string Subject, string BodyContent, List<string> Attachment)
        {
            string EmailID = System.Configuration.ConfigurationManager.AppSettings["EmailID"].ToString();
            string Password = System.Configuration.ConfigurationManager.AppSettings["Password"].ToString();
            try
            {
                MailMessage mail = new MailMessage();
                mail = new MailMessage();
                mail.To.Add(ToMail);
                string MailList = "";
                string[] ToMuliId = ToMail.Split(',');
                foreach (string ToEMailIds in ToMuliId) { MailList = MailList + ToEMailIds; }
                if (CC != "")
                    mail.CC.Add(CC);
                mail.From = new MailAddress(EmailID, "Alert Mail");
                mail.SubjectEncoding = Encoding.UTF8;
                mail.Subject = Subject;
                mail.BodyEncoding = Encoding.UTF8;
                mail.Body = BodyContent;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = System.Configuration.ConfigurationManager.AppSettings["Host"].ToString();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Port"]);
                if (Password != "")
                {
                    smtp.Credentials = new System.Net.NetworkCredential(EmailID, Password);
                    smtp.EnableSsl = true;
                }
                else
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Timeout = 60000;
                }
                if (Attachment.Count > 0)
                {
                    for (int i = 0; i < Attachment.Count; i++)
                    {
                        Attachment attachmentfile = new Attachment(Attachment[i]);
                        mail.Attachments.Add(attachmentfile);
                    }
                }
                smtp.Send(mail);
                if (mail.Attachments != null)
                {
                    for (Int32 I = mail.Attachments.Count - 1; I >= 0; I--)
                    {
                        mail.Attachments[I].Dispose();
                    }
                    mail.Attachments.Clear();
                    mail.Attachments.Dispose();
                }
                mail.Dispose();
                mail = null;
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void FailedMail(string Subject, string mailBody)
        {
            try
            {

               // string mailSend = System.Configuration.ConfigurationManager.AppSettings["IsMailSend"].ToString();
                string FromEmail = System.Configuration.ConfigurationManager.AppSettings["FromEmail"].ToString();
                string Password = System.Configuration.ConfigurationManager.AppSettings["Password"].ToString();

                
                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                    mail.To.Add("Maheswaran@excelgoodies.com");
                    mail.From = new MailAddress(FromEmail, "EG");
                    mail.SubjectEncoding = Encoding.UTF8;
                    mail.Subject = Subject;
                    mail.BodyEncoding = Encoding.UTF8;
                    mail.Body = mailBody;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(FromEmail, Password);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                
            }
            catch (Exception ex)
            {

            }
        }
    }
}
