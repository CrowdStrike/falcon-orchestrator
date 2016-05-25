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


using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FalconOrchestratorWeb.Areas.Admin.Models;
using FalconOrchestrator.DAL;

namespace FalconOrchestratorWeb.Areas.Admin.Repository
{
    public class RecipientRepository
    {
        private FalconOrchestratorDB context;

        private MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TicketRecipient, RecipientViewModel>();
            cfg.CreateMap<RecipientViewModel, TicketRecipient>();
        });

        public RecipientRepository()
        {
            this.context = new FalconOrchestratorDB();
        }


        public IEnumerable<RecipientViewModel> GetList()
        {
            var data = context.TicketRecipients.ToList();
            return config.CreateMapper().Map<List<TicketRecipient>, List<RecipientViewModel>>(data);
        }

        public void Create(RecipientViewModel model)
        {
            TicketRecipient data = config.CreateMapper().Map<RecipientViewModel, TicketRecipient>(model);
            context.TicketRecipients.Add(data);
            context.SaveChanges();
        }

        public RecipientViewModel Get(int? id)
        {
            TicketRecipient data = context.TicketRecipients.Find(id);
            return config.CreateMapper().Map<TicketRecipient, RecipientViewModel>(data);
        }

        public void Update(RecipientViewModel model)
        {
            TicketRecipient data = context.TicketRecipients.Find(model.TicketRecipientId);
            data.EmailAddress = model.EmailAddress;
            data.PhoneNumber = model.PhoneNumber;
            data.Title = model.Title;
            context.SaveChanges();
        }

        public void Delete(int? id)
        {
            TicketRecipient data = context.TicketRecipients.Find(id);
            context.TicketRecipients.Remove(data);
            context.SaveChanges();
        }
    }
}