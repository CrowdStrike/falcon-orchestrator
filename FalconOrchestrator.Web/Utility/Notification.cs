//<Falcon Orchestrator provides automated workflow and response capabilities>
//    Copyright(C) 2016 CrowdStrike

//   This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or(at your option) any later version.

//   This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net.Mail;
using FalconOrchestrator.DAL;
using FalconOrchestratorWeb.Models.ViewModels;

namespace FalconOrchestratorWeb.Utility
{
    public class Notification
    {
        private Ticket ticket;
        private string severity;
        private string recipientAddress;
        private AppConfiguration config;

        public Notification(Ticket _ticket, TicketViewModel model)
        {
            config = new AppConfiguration(System.Configuration.ConfigurationManager.AppSettings["CryptoKey"]);
            ticket = _ticket;

            //Severity and TicketRecipient navigation properties are not being loaded properly, this is a temp workaround
            FalconOrchestratorDB db = new FalconOrchestratorDB();
            severity = db.Severities.Where(x => x.SeverityId == ticket.SeverityId).Select(y => y.SeverityType).Single();
            recipientAddress = db.TicketRecipients.Where(x => x.TicketRecipientId == ticket.TicketRecipientId).Select(y => y.EmailAddress).Single();
            db.Dispose();
        }

        private string TemplateReplace(string templatePath)
        {
            StringBuilder sb = new StringBuilder(templatePath);
            Account account = ticket.AccountTickets.Select(x => x.Account).Single();

            return sb.Replace("{{Priority}}", severity)
                     .Replace("{{Hostname}}", ticket.DeviceTickets.Select(x => x.Device.Hostname).Single())
                     .Replace("{{Username}}", account.AccountName)
                     .Replace("{{FirstName}}", account.FirstName ?? string.Empty)
                     .Replace("{{LastName}}", account.LastName ?? string.Empty)
                     .Replace("{{Department}}", account.Department ?? string.Empty)
                     .Replace("{{JobTitle}}", account.JobTitle ?? string.Empty)
                     .Replace("{{EmailAddress}}", account.EmailAddress ?? string.Empty)
                     .Replace("{{Manager}}", account.Manager ?? string.Empty)
                     .Replace("{{Country}}", account.Country ?? string.Empty)
                     .Replace("{{City}}", account.City ?? string.Empty)
                     .Replace("{{PhoneNumber}}", account.PhoneNumber ?? string.Empty)
                     .Replace("{{StateProvince}}", account.StateProvince ?? string.Empty)
                     .Replace("{{StreetAddress}}", account.StreetAddress ?? string.Empty)
                     .Replace("{{ExternalTicket}}", ticket.ExternalTicket ?? string.Empty)
                     .Replace("{{Comment}}", ticket.Comment ?? string.Empty).ToString();
        }

        public async Task SendEmail(List<string> ccList = null)
        {
            MailAddress from = new MailAddress(config.EMAIL_SENDER);
            MailAddress to = new MailAddress(recipientAddress);

            string subject = TemplateReplace(config.EMAIL_TICKET_SUBJECT);
            string template = File.ReadAllText(config.EMAIL_TICKET_TEMPLATE_PATH);
            string body = TemplateReplace(template);

            SmtpClient smtp = new SmtpClient
            {
                Host = config.EMAIL_SERVER,
                Port = Convert.ToInt32(config.EMAIL_PORT),
                EnableSsl = Convert.ToBoolean(config.EMAIL_SSL),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(config.EMAIL_USERNAME, config.EMAIL_PASSWORD)
            };

            using (MailMessage message = new MailMessage(from, to))
            {
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;

                //If a high or above priority, set email messave to high priority
                if (ticket.SeverityId > 4)
                {
                    message.Priority = MailPriority.High;
                }

                //If cc propery is not empty, CC them on the notification
                if (!String.IsNullOrEmpty(config.EMAIL_CC))
                {
                    foreach (string line in config.EMAIL_CC.Split(','))
                    {
                        message.CC.Add(new MailAddress(line));
                    }
                }

                if (ccList != null)
                {
                    ccList.ForEach(x => message.CC.Add(new MailAddress(x)));
                }
                await smtp.SendMailAsync(message);
            }
        }
    }
}