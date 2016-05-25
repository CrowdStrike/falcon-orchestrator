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
using FalconOrchestrator.DAL;
using FalconOrchestratorWeb.Areas.Admin.Models;

namespace FalconOrchestratorWeb.Areas.Admin.Repository
{
    public class ResponderRepository
    {
        private FalconOrchestratorDB context;

        private MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Responder, ResponderViewModel>();
            cfg.CreateMap<ResponderViewModel, Responder>();
        });

        public ResponderRepository()
        {
            this.context = new FalconOrchestratorDB();
        }


        public IEnumerable<ResponderViewModel> GetList()
        {
            var data = context.Responders.ToList();
            return config.CreateMapper().Map<List<Responder>, List<ResponderViewModel>>(data);
        }

        public void Create(ResponderViewModel model)
        {
            Responder data = config.CreateMapper().Map<ResponderViewModel, Responder>(model);
            context.Responders.Add(data);
            context.SaveChanges();
        }

        public ResponderViewModel Get(int? id)
        {
            Responder data = context.Responders.Find(id);
            return config.CreateMapper().Map<Responder, ResponderViewModel>(data);
        }

        public void Update(ResponderViewModel model)
        {
            Responder data = context.Responders.Find(model.ResponderId);
            data.EmailAddress = model.EmailAddress;
            data.PhoneNumber = model.PhoneNumber;
            data.FirstName = model.FirstName;
            data.LastName = model.LastName;
            context.SaveChanges();
        }

        public void Delete(int? id)
        {
            Responder data = context.Responders.Find(id);
            context.Responders.Remove(data);
            context.SaveChanges();
        }
    }
}