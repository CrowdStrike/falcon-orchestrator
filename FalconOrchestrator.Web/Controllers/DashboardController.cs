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


using System.Web.Mvc;
using FalconOrchestratorWeb.Models.Dashboard;
using FalconOrchestratorWeb.Repository;

namespace FalconOrchestratorWeb.Controllers
{
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            DashboardViewModel model = new DashboardViewModel()
            {
                DetectionsBySeverity = PieChartUtil.ExecuteQueryAndReturnPieChart(DashboardRepository.DetectionsBySeverity(),"detects_by_severity"),
                DetectionsByStatus = PieChartUtil.ExecuteQueryAndReturnPieChart(DashboardRepository.DetectionsByStatus(),"detects_by_status"),
                DetectionsByType = BarChartUtil.CreateBarChart(DashboardRepository.DetectionsByType(), "detects_by_type"),
                DetectionsByUserDepartment = BarChartUtil.CreateMultiSeriesBarChart(DashboardRepository.DetectionsByUserDepatment(), "detects_by_user_department"),
                DetectionsByUserCountry = BarChartUtil.CreateMultiSeriesBarChart(DashboardRepository.DetectionsByUserCountry(), "detects_by_user_country"),
                DetectionsByUserJobTitle = BarChartUtil.CreateMultiSeriesBarChart(DashboardRepository.DetectionsByUserJobTitle(), "detects_by_user_jobtitle"),
                DetectionsByHandlerAndStatus = ColumnChartUtil.CreateStacked(DashboardRepository.DetectionsByHandlerAndStatus(), "detects_by_handler_and_status"),
                DetectionsByTaxonomy = BarChartUtil.CreateBarChart(DashboardRepository.DetectionsByTaxonomy(),"detects_by_taxonomy"),
                AverageDetectionsPerDay = DashboardRepository.AverageDetectionsPerDay(),
                AverageDetectionsPerWeek = DashboardRepository.AverageDetectionsPerWeek(),
                AverageDetectionsPerMonth = DashboardRepository.AverageDetectionsPerMonth()
            };
            return View(model);
        }
    }
}