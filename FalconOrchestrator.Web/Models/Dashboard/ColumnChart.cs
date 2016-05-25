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
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Enums;

namespace FalconOrchestratorWeb.Models.Dashboard
{
    public class ColumnChartData
    {
        public string field { get; set; }
        public int count { get; set; }
    }

    public class StackedColumnChartData
    {
        public string xValue { get; set; }
        public string seriesName { get; set; }
        public int count { get; set; }
    }

    public class SeriesData
    {
        public string seriesName { get; set; }
        public List<object[]> counts { get; set; }
    }


    public class ColumnChartUtil
    {

        public static Highcharts CreateColumnChart(IEnumerable<ColumnChartData> model, string cssId)
        {
            List<string> xValues = new List<string>();
            var yValues = new List<object[]>();

            foreach (var line in model)
            {
                xValues.Add(line.field);
                yValues.Add(new object[] { line.count });
            }

            Highcharts chart = new Highcharts(cssId)
           .InitChart(new Chart { DefaultSeriesType = ChartTypes.Column })
           .SetTitle(new Title { Text = "" })
           .SetXAxis(new XAxis
           {
               Categories = xValues.ToArray(),
               Title = new XAxisTitle { Text = "" }
           })
           .SetYAxis(new YAxis
           {
               Type = AxisTypes.Linear,
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

        public static Highcharts CreateStacked(IEnumerable<StackedColumnChartData> model, string cssId)
        {

            //string[] categories = model.OrderByDescending(x => x.count).Select(x => x.xValue).Distinct().ToArray();
            string[] categories = model.Select(x => x.xValue).Distinct().ToArray();
            string[] series = model.Select(x => x.seriesName).Distinct().ToArray();
            List<SeriesData> final = new List<SeriesData>();

            foreach (string line in series)
            {
                SeriesData _data = new SeriesData();
                _data.seriesName = line;
                List<object[]> countsHolder = new List<object[]>();

                for (int i = 0; i < categories.Count(); i++)
                {
                    int count = model.Where(x => x.seriesName.Equals(line) && x.xValue.Equals(categories[i])).Select(x => x.count).FirstOrDefault();
                    countsHolder.Add(new object[] { count });
                }
                _data.counts = countsHolder;

                final.Add(_data);
            }

            Highcharts chart = new Highcharts(cssId)
           .InitChart(new Chart { DefaultSeriesType = ChartTypes.Column })
           .SetTitle(new Title { Text = "" })
           .SetXAxis(new XAxis
           {
               Categories = categories,
               Title = new XAxisTitle { Text = "" }
           })
           .SetYAxis(new YAxis
           {
               Type = AxisTypes.Linear,
               Title = new YAxisTitle { Text = "" }
           })
           .SetTooltip(new Tooltip { Formatter = "function() { return '<b>'+ this.series.name +'</b>: '+ this.y; }" })
           .SetPlotOptions(new PlotOptions
           {
               Column = new PlotOptionsColumn
               {
                   Stacking = Stackings.Normal
               }
           })
          .SetCredits(new Credits { Enabled = false });

            List<Series> seriesList = new List<Series>();
            foreach (SeriesData line in final)
            {
                seriesList.Add(new Series { Name = line.seriesName, Data = new Data(line.counts.ToArray()) });

            }
            chart.SetSeries(seriesList.ToArray());
            return chart;
        }
    }
}