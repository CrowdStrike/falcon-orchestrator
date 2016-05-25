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
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Enums;

namespace FalconOrchestratorWeb.Models.Dashboard
{
    public class BarChartData
    {
        public string field { get; set; }
        public int count { get; set; }
    }

    public class BarChartMultiSeriesData
    {
        public string field { get; set; }
        public int count { get; set; }
        public int count2 { get; set; }
    }

    public class BarChartUtil
    {

        public static Highcharts CreateBarChart(IEnumerable<BarChartData> model, string cssId)
        {
            List<string> xValues = new List<string>();
            var yValues = new List<object[]>();

            foreach (var line in model)
            {
                xValues.Add(line.field);
                yValues.Add(new object[] { line.count });
            }

            Highcharts chart = new Highcharts(cssId)
           .InitChart(new Chart { DefaultSeriesType = ChartTypes.Bar })
           .SetTitle(new Title { Text = "" })
           .SetXAxis(new XAxis
           {
               Categories = xValues.ToArray(),
               Title = new XAxisTitle { Text = "" }
           })
           .SetYAxis(new YAxis
           {
               Type = AxisTypes.Logarithmic,
               Title = new YAxisTitle { Text = "" }
           })
           .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.series.name +'</b>: '+ this.y; }" })
           .SetPlotOptions(new PlotOptions
           {
               Bar = new PlotOptionsBar
               {
                   DataLabels = new PlotOptionsBarDataLabels { Enabled = false }
               }
           })
           .SetCredits(new Credits { Enabled = false })
           .SetSeries(new[]
                {
                    new Series { Name= "Detections", Data = new Data(yValues.ToArray())}
                });

            return chart;
        }

        public static Highcharts CreateMultiSeriesBarChart(IEnumerable<BarChartMultiSeriesData> model, string cssId)
        {
            List<string> xValues = new List<string>();
            var yValues = new List<object[]>();
            var yValues2 = new List<object[]>();

            foreach (var line in model)
            {
                xValues.Add(line.field);
                yValues.Add(new object[] { line.count });
                yValues2.Add(new object[] { line.count2 });
            }

            Highcharts chart = new Highcharts(cssId)
           .InitChart(new Chart { DefaultSeriesType = ChartTypes.Bar })
           .SetTitle(new Title { Text = "" })
           .SetXAxis(new XAxis
           {
               Categories = xValues.ToArray(),
               Title = new XAxisTitle { Text = "" }
           })
           .SetYAxis(new YAxis
           {
               Type = AxisTypes.Logarithmic,
               Title = new YAxisTitle { Text = "" }
           })
           .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.series.name +'</b>: '+ this.y; }" })
           .SetPlotOptions(new PlotOptions
           {
               Bar = new PlotOptionsBar
               {
                   DataLabels = new PlotOptionsBarDataLabels { Enabled = false }
               }
           })
           .SetCredits(new Credits { Enabled = false })
           .SetSeries(new[]
                {
                    new Series { Name= "Detections", Data = new Data(yValues.ToArray())},
                    new Series {Name = "Distinct Users", Data = new Data(yValues2.ToArray())}
                });

            return chart;
        }
    }

}