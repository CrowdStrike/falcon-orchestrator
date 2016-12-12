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
using FalconOrchestratorWeb.Models.ViewModels;
using FalconOrchestratorWeb.Utility;
using System.Threading.Tasks;
using AutoMapper;
using FalconOrchestrator.DAL;

namespace FalconOrchestratorWeb.Repository
{
    public class TicketingRepository
    {
        private FalconOrchestratorDB context;

        private MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TicketViewModel, Ticket>()
                .ForMember(x => x.Severity, y => y.Ignore())
                .ForMember(x => x.TicketRecipient, y => y.Ignore())
                .ForMember(x => x.AccountTickets, y => y.Ignore())
                .ForMember(x => x.DeviceTickets, y => y.Ignore())
                .ForMember(x => x.DetectionTickets, y => y.Ignore())     
                .ForMember(x => x.CompletionDate, y => y.MapFrom(v => v.CompletionDate != null ? Convert.ToDateTime(v.CompletionDate).ToUniversalTime() : (DateTime?)null))
                .ForMember(x => x.DispatchDate, y => y.MapFrom(v => v.DispatchDate != null ? Convert.ToDateTime(v.DispatchDate).ToUniversalTime() : (DateTime?)null));

            cfg.CreateMap<Ticket, TicketViewModel>()
              .ForMember(x => x.DispatchDate, y => y.MapFrom(v => v.DispatchDate != null ? Convert.ToDateTime(v.DispatchDate).ToLocalTime() : (DateTime?)null));

            cfg.CreateMap<Asset, Account>()
                .ForMember(x => x.AccountId, y => y.Ignore())              
                .ForMember(x => x.Detections, y => y.Ignore())
                .ForMember(x => x.OrganizationalUnit, y => y.Ignore())
                .ForMember(x => x.LastLogon, y => y.Ignore());
            cfg.CreateMap<Account, Asset>();

        });

        public TicketingRepository()
        {
            context = new FalconOrchestratorDB();
        }

        public TicketingRepository(FalconOrchestratorDB db)
        {
            context = db;
        }

        public List<TicketViewModel> GetList()
        {
            List<Ticket> tickets = context.Tickets.ToList();
            var result = new List<TicketViewModel>();
            foreach (Ticket line in tickets)
            {
                TicketViewModel model = config.CreateMapper().Map<Ticket, TicketViewModel>(line);
                model.Assignee = line.TicketRecipient.Title;
                model.Asset = new Asset();
                model.Asset.AccountName = line.AccountTickets.Select(x => x.Account.AccountName).Single();
                model.Asset.Hostname = line.DeviceTickets.Select(x => x.Device.Hostname).Single();
                model.Asset.Domain = line.DeviceTickets.Select(x => x.Device.Domain).Single();
                result.Add(model);
            }
            return result;
        }

        public async Task Create(TicketViewModel model)
        {

            //Map view model to domain model and create ticket record
            Ticket ticket = config.CreateMapper().Map<TicketViewModel, Ticket>(model);
            context.Tickets.Add(ticket);
            context.SaveChanges();
            context.Tickets.Attach(ticket);
         
            //if ticket is associated to detection, create detection ticket records for each associated detect
            if (model.DetectionId != null)
            {
                CreateDetectionTickets(Convert.ToInt32(model.DetectionId), ticket.TicketId);
            }

            //Check if device already exists, if so get deviceId, if not create a new one and return that device Id
            int deviceId = GetDeviceId(model.Asset);

            //Create a device ticket record
            DeviceTicket dt = new DeviceTicket();
            dt.DeviceId = deviceId;
            dt.TicketId = ticket.TicketId;
            context.DeviceTickets.Add(dt);

            //Check if account already exists and whether it has been modified, if modified update attributes if does not exists, create a new account
            int accountId = GetAccountId(model.Asset);

            AccountTicket at = new AccountTicket();
            at.AccountId = accountId;
            at.TicketId = ticket.TicketId;
            context.AccountTickets.Add(at);
            context.SaveChanges();

            await TicketNotification(model, ticket);

        }

        public TicketViewModel Get(int? id)
        {
            Ticket ticket = context.Tickets.Find(id);
            TicketViewModel model = config.CreateMapper().Map<Ticket, TicketViewModel>(ticket);
            Account account = ticket.AccountTickets.Select(x => x.Account).Single();
            
            model.DetectionId = ticket.DetectionTickets.Select(x => x.DetectionId).FirstOrDefault().ToString();     
            model.Asset = config.CreateMapper().Map<Account, Asset>(account);
            model.Asset.Hostname = ticket.DeviceTickets.Select(x => x.Device.Hostname).Single();
            model.Asset.Domain = ticket.DeviceTickets.Select(x => x.Device.Domain).Single();
            model.DispatchDate = ticket.DispatchDate.HasValue ? ticket.DispatchDate.Value.ToLocalTime().ToString("MMMM d, yyyy h:mm tt") : null;
            model.CompletionDate = ticket.CompletionDate.HasValue ? ticket.CompletionDate.Value.ToLocalTime().ToString("MMMM d, yyyy h:mm tt") : null;
            return model;
        }

        public async Task Update(TicketViewModel model)
        {
            Ticket ticket = context.Tickets.Find(model.TicketId);

            //if ticket is associated to detections, handle status updates accordingly
            if(model.DetectionId != null)
            {
                UpdateDetectionStatuses(model, ticket);
            }

            //need to update ticket attributes after detection status updates as to get current DB stored values first
            ticket.Comment = model.Comment;
            ticket.CompletionDate = model.CompletionDate != null ? Convert.ToDateTime(model.CompletionDate).ToUniversalTime() : (DateTime?)null;
            ticket.DispatchDate = model.DispatchDate != null ? Convert.ToDateTime(model.DispatchDate).ToUniversalTime() : (DateTime?)null;
            ticket.ExternalTicket = model.ExternalTicket;
            ticket.SeverityId = model.SeverityId;
            ticket.TicketRecipientId = model.TicketRecipientId;
            context.SaveChanges();

            //Manage changes to account attributes
            GetAccountId(model.Asset);

            //Manage changes to device attributes
            GetDeviceId(model.Asset);

            await TicketNotification(model, ticket);
        }

        public void Delete(int? id)
        {
            Ticket ticket = context.Tickets.Find(id);

            foreach (var line in ticket.DetectionTickets.ToList())
            {
                //set status of each detection to open and closed date to null
                line.Detection.StatusId = 1;
                line.Detection.ClosedDate = null;
                context.DetectionTickets.Remove(line);
            }

            //remove any associated device tickets
            ticket.DeviceTickets.ToList().ForEach(x => context.DeviceTickets.Remove(x));

            //remove any associated account tickets
            ticket.AccountTickets.ToList().ForEach(x => context.AccountTickets.Remove(x));

            //remove ticket record
            context.Tickets.Remove(ticket);
            context.SaveChanges();

        }



        private async Task TicketNotification(TicketViewModel model, Ticket ticket)
        {
            //If notification is enabled
            if (model.SendNotification)
            {
                List<string> ccList = new List<string>();
                Notification notify = new Notification(ticket, model);

                if (model.NotifyUser)
                    ccList.Add(model.Asset.EmailAddress);

                if (model.NotifyManager)
                    ccList.Add(model.Asset.Manager);

                await notify.SendEmail(ccList);
            }
        }




        private void CreateDetectionTickets(int detectionId, int ticketId)
        {
            Detection detection = context.Detections.Find(detectionId);

            // there can be multiple detection records associated to one Falcon UI based detection, detections with the same falcon host link url should be grouped and handled together
            List<Detection> associated = context.Detections.Where(x => x.FalconHostLink.Equals(detection.FalconHostLink)).ToList();
            foreach (Detection line in associated)
            {
                DetectionTicket dt = new DetectionTicket();
                dt.TicketId = ticketId;
                dt.DetectionId = line.DetectionId;
                context.DetectionTickets.Add(dt);

                //set the status of each detection to remediation
                line.StatusId = 4;
                context.SaveChanges();
            }
        }

        private int GetDeviceId(Asset asset)
        {
            int deviceId;
           
            if (context.Devices.Any(x => x.Hostname.Equals(asset.Hostname)))
            {
                Device device = context.Devices.Where(x => x.Hostname.Equals(asset.Hostname)).FirstOrDefault();
                bool modified = DeviceAttributesModified(device, asset);
                if (modified)
                {
                    device.Hostname = asset.Hostname;
                    device.Domain = asset.Domain;
                    context.SaveChanges();
                }

                deviceId = device.DeviceId;
            }

            else
            {
                Device device = new Device();
                device.Hostname = asset.Hostname;
                device.Domain = asset.Domain;
                context.Devices.Add(device);
                context.SaveChanges();
                deviceId = device.DeviceId;
            }

            return deviceId;
        }

        private int GetAccountId(Asset asset)
        {
            int accountId;
            //Account already exists in database
            if (context.Accounts.Any(x => x.AccountName.Equals(asset.AccountName)))
            {
                Account account = context.Accounts.Where(x => x.AccountName.Equals(asset.AccountName)).FirstOrDefault();

                //if any of the account attributes have been modified, update the model with new values
                bool modified = AccountAttributesModified(account, asset);
                if (modified)
                {
                    account.City = asset.City;
                    account.Country = asset.Country;
                    account.Department = asset.Department;
                    account.JobTitle = asset.JobTitle;
                    account.EmailAddress = asset.EmailAddress;
                    account.FirstName = asset.FirstName;
                    account.LastName = asset.LastName;
                    account.JobTitle = asset.JobTitle;
                    account.Manager = asset.Manager;
                    account.PhoneNumber = asset.PhoneNumber;
                    account.StateProvince = asset.StateProvince;
                    account.StreetAddress = asset.StreetAddress;
                    account.Timestamp = DateTime.UtcNow;       
                    context.SaveChanges();
                }

                accountId = account.AccountId;
            }

            //Account does not exist, create a new one
            else
            {
                Account account = config.CreateMapper().Map<Asset, Account>(asset);
                account.Timestamp = DateTime.UtcNow;
                context.Accounts.Add(account);
                context.SaveChanges();
                accountId = account.AccountId;
            }

            return accountId;
        }

        private Boolean AccountAttributesModified(Account data, Asset model)
        {

           bool match = (data.City == model.City) && (data.Country == model.Country) && (data.Department == model.Department)
                && (data.EmailAddress == model.EmailAddress) && (data.FirstName == model.FirstName) && (data.LastName == model.LastName)
                && (data.Manager == model.Manager) && (data.PhoneNumber == model.PhoneNumber) && (data.StateProvince == model.StateProvince)
                && (data.StreetAddress == model.StreetAddress) && (data.JobTitle == model.JobTitle);

           //if all attributes are the same still, return a false (has not been modified)
           if (match == true)
               return false;

           return true;
        }
        private Boolean DeviceAttributesModified(Device data, Asset model)
        {

            bool match = (data.Hostname == model.Hostname) && (data.Domain == model.Domain);

            if (match == true)
                return false;

            return true;
        }


        private void UpdateDetectionStatuses(TicketViewModel model, Ticket ticket)
        {
            //Get list of detections associated to ticket
            List<Detection> detections = context.DetectionTickets.Where(x => x.TicketId == model.TicketId).Select(x => x.Detection).ToList();
            System.Diagnostics.Debug.WriteLine(ticket.CompletionDate);
            System.Diagnostics.Debug.WriteLine(model.CompletionDate);
            //When re-opening a ticket after it has already been closed, set status of all associated detection events back to remediation
            if (ticket.CompletionDate.HasValue && String.IsNullOrEmpty(model.CompletionDate))
            {
                foreach (Detection line in detections)
                {
                    //set status of detection events back to remediation & closed date to null
                    line.StatusId = 4;
                    line.ClosedDate = null;
                    context.SaveChanges();
                }
            }

            //When ticket has a completion date set, update all associated detections to a status of closed with a completion date
            if (!String.IsNullOrEmpty(model.CompletionDate))
            {
                foreach (Detection line in detections)
                {
                    line.StatusId = 6;
                    line.ClosedDate = Convert.ToDateTime(model.CompletionDate);
                }
                context.SaveChanges();
            }

        }

    }
}