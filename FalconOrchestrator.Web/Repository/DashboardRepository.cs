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
using FalconOrchestratorWeb.Models.Dashboard;
using FalconOrchestrator.DAL;

namespace FalconOrchestratorWeb.Repository
{
    public class DashboardRepository
    {

        public static string AverageDetectionsPerDay()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.v_DetectionAverages.Select(x => x.Daily).FirstOrDefault().Value.ToString();
            }
        }

        public static string AverageDetectionsPerWeek()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.v_DetectionAverages.Select(x => x.Weekly).FirstOrDefault().Value.ToString();
            }
        }

        public static string AverageDetectionsPerMonth()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.v_DetectionAverages.Select(x => x.Monthly).FirstOrDefault().Value.ToString();
            }
        }

        public static List<PieChartData> DetectionsBySeverity()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                List<PieChartData> model = db.Detections
                     .GroupBy(x => x.Severity.SeverityType)
                     .Select(y => new PieChartData { field = y.Key, count = y.Count() })
                     .OrderByDescending(z => z.count).ToList();

                return model;
            }
        }

        public static List<PieChartData> DetectionsByStatus()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                List<PieChartData> model = db.Detections
                         .GroupBy(x => x.Status.StatusType)
                         .Select(y => new PieChartData { field = y.Key, count = y.Count() })
                         .OrderByDescending(z => z.count).ToList();

                return model;
            }
        }

        public static List<BarChartData> DetectionsByType()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                List<BarChartData> model = db.Detections
                                 .GroupBy(x => x.Name)
                                 .Select(y => new BarChartData { field = y.Key, count = y.Count() })
                                 .OrderByDescending(z => z.count).ToList();
                return model;
            }
        }

        public static List<BarChartData> DetectionsByTaxonomy()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                List<BarChartData> model = db.DetectionTaxonomies
                                 .GroupBy(x => x.Taxonomy.Description)
                                 .Select(y => new BarChartData { field = y.Key, count = y.Count() })
                                 .OrderByDescending(z => z.count).ToList();
                return model;
            }
        }

        public static List<BarChartMultiSeriesData> DetectionsByUserDepatment()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                List<BarChartMultiSeriesData> model = db.Detections
                         .Where(z => z.Account.Department != null)
                         .GroupBy(x => x.Account.Department)
                         .Select(y => new BarChartMultiSeriesData
                         {
                             field = y.Key,
                             count = y.Count(),
                             count2 = y.Select(x => x.AccountId).Distinct().Count()
                         })
                         .OrderByDescending(z => z.count).ToList();
                return model;
            }
        }

        public static List<BarChartMultiSeriesData> DetectionsByUserCountry()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                List<BarChartMultiSeriesData> model = db.Detections
                         .GroupBy(x => x.Account.Country)
                         .Select(y => new BarChartMultiSeriesData { field = y.Key, count = y.Count(), count2 = y.Select(x => x.AccountId).Distinct().Count() })
                         .Where(z => z.field != null)
                         .OrderByDescending(z => z.count).ToList();
                return model;
            }
        }

        public static List<BarChartMultiSeriesData> DetectionsByUserJobTitle()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                List<BarChartMultiSeriesData> model = db.Detections
                         .GroupBy(x => x.Account.JobTitle)
                         .Select(y => new BarChartMultiSeriesData { field = y.Key, count = y.Count(), count2 = y.Select(x => x.AccountId).Distinct().Count() })
                         .Where(z => z.field != null)
                         .OrderByDescending(z => z.count).ToList();
                return model;
            }
        }

        public static StackedColumnChartData[] DetectionsByHandlerAndStatus()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                StackedColumnChartData[] model = db.Detections
                        .GroupBy(x => new { FullName = x.Responder.Equals(null) ? "Unassigned" : x.Responder.FirstName + " " + x.Responder.LastName, x.Status.StatusType })
                        .Select(y => new StackedColumnChartData { xValue = y.Key.FullName, seriesName = y.Key.StatusType, count = y.Count() }).ToArray();
                return model;
            }
        }
    }
}