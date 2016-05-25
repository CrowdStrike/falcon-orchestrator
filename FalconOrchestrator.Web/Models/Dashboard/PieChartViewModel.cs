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
using System.Drawing;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Enums;

namespace FalconOrchestratorWeb.Models.Dashboard
{

    public class PieChartData
    {
        public string field { get; set; }
        public int count { get; set; }
    }

    public class PieChartUtil
    {
        private static Highcharts CreatePieChart(Data model, string name)
        {

            Highcharts chart = new Highcharts(name)

               .SetOptions(new GlobalOptions
               {
                   Global = new Global
                   {
                       UseUTC = false,
                   },
                   Colors = new[]
                                 {
                                      ColorTranslator.FromHtml("#4083BF"), //blue
                                      ColorTranslator.FromHtml("#FF5249"), //red
                                      ColorTranslator.FromHtml("#3FDB3F"), //green
                                      ColorTranslator.FromHtml("#FF8B49"), //orange
                                      ColorTranslator.FromHtml("#FFDC49"), //yellow
                                      ColorTranslator.FromHtml("#6B48C6"), //purple
                                  }
               })
              .InitChart(new Chart
              {
                  PlotShadow = false,
                  Type = ChartTypes.Pie,
                  Options3d = new ChartOptions3d
                  {
                      Enabled = false,
                      Alpha = 60,
                      Beta = 0,
                  }
              })
              .SetCredits(new Credits { Enabled = false })
              .SetTitle(new Title { Text = "" })
              .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.point.name +'</b><br/>' +  'Detections: ' + this.y + '<br/>' + 'Percentage: ' + this.percentage.toFixed(2) + ' %'; }" })
              .SetPlotOptions(new PlotOptions
              {
                  Pie = new PlotOptionsPie
                  {
                      Depth = 45,
                      AllowPointSelect = true,
                      Cursor = Cursors.Pointer,
                      DataLabels = new PlotOptionsPieDataLabels
                      {
                          Enabled = false
                      },
                      ShowInLegend = true
                  }
              })
              .SetSeries(new Series
              {
                  Type = ChartTypes.Pie,
                  Data = model
              });

            return chart;

        }

        private static Data PieChartAddDataPoints(List<PieChartData> model)
        {
            var list = new List<object[]>();

            foreach (var line in model)
            {
                list.Add(new object[] { line.field, line.count });
            }

            return new Data(list.ToArray());
        }

        public static Highcharts ExecuteQueryAndReturnPieChart(IEnumerable<PieChartData> query, string cssId)
        {

            List<PieChartData> stats = new List<PieChartData>();

            foreach (var record in query)
            {
                PieChartData line = new PieChartData() { field = record.field, count = record.count };
                stats.Add(line);
            }
            Data model = PieChartUtil.PieChartAddDataPoints(stats);
            Highcharts chart = PieChartUtil.CreatePieChart(model, cssId);
            return chart;

        }

    }

}