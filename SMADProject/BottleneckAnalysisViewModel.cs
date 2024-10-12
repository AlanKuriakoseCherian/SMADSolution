using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SMADProject.ViewModels
{
    public class BottleneckAnalysisViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ProductionLine> _productionLines;
        private ProductionLine _selectedProductionLine;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private ObservableCollection<BottleneckData> _bottleneckData;
        private PlotModel _plotModel;

        public ObservableCollection<ProductionLine> ProductionLines
        {
            get => _productionLines;
            set
            {
                _productionLines = value;
                OnPropertyChanged(nameof(ProductionLines));
            }
        }

        public ProductionLine SelectedProductionLine
        {
            get => _selectedProductionLine;
            set
            {
                _selectedProductionLine = value;
                OnPropertyChanged(nameof(SelectedProductionLine));
            }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        public ObservableCollection<BottleneckData> BottleneckData
        {
            get => _bottleneckData;
            set
            {
                _bottleneckData = value;
                OnPropertyChanged(nameof(BottleneckData));
            }
        }

        public PlotModel PlotModel
        {
            get => _plotModel;
            set
            {
                _plotModel = value;
                OnPropertyChanged(nameof(PlotModel));
            }
        }

        public ICommand AnalyzeCommand { get; }

        public BottleneckAnalysisViewModel()
        {
            // Initialize collections
            ProductionLines = new ObservableCollection<ProductionLine>();
            BottleneckData = new ObservableCollection<BottleneckData>();

            // Load production lines
            LoadProductionLines();

            // Setup command
            AnalyzeCommand = new RelayCommand(param => AnalyzeBottlenecks());
        }

        private void LoadProductionLines()
        {
            using (var context = new SmadDbEntities())
            {
                var lines = context.ProductionLines.ToList();
                ProductionLines = new ObservableCollection<ProductionLine>(lines);
            }
        }

        private void AnalyzeBottlenecks()
        {
            if (SelectedProductionLine == null || StartDate == null || EndDate == null)
            {
                Console.WriteLine("No production line or dates selected.");
                return;
            }

            using (var context = new SmadDbEntities())
            {
                try
                {
                    var data = context.ProductionMetrics
                        .Where(pm => pm.LineID == SelectedProductionLine.LineID
                                     && pm.MetricDate >= StartDate
                                     && pm.MetricDate <= EndDate)
                        .ToList();

                    Console.WriteLine($"Data count: {data.Count}");

                    if (data.Count == 0)
                    {
                        Console.WriteLine("No data found for the selected line and date range.");
                        return;
                    }

                    // Calculate total downtime in ticks
                    var totalDowntimeTicks = data.Sum(pm => (long)((pm.Downtime ?? 0) * TimeSpan.TicksPerHour));
                    var totalDowntime = new TimeSpan(totalDowntimeTicks);

                    // Generate recommendations based on analysis
                    string recommendations = GenerateRecommendations(totalDowntime);

                    // Create and populate BottleneckData collection
                    BottleneckData.Clear();
                    BottleneckData.Add(new BottleneckData
                    {
                        LineName = SelectedProductionLine.LineName,
                        BottleneckTime = totalDowntime,
                        TotalDowntime = totalDowntime,
                        Recommendations = recommendations
                    });

                    // Update the PlotModel
                    UpdatePlotModel(data);
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log them)
                    Console.WriteLine($"Error during analysis: {ex.Message}");
                }
            }
        }

        private string GenerateRecommendations(TimeSpan totalDowntime)
        {
            // Implement your recommendation logic here
            return $"Recommendations based on total downtime of {totalDowntime.TotalHours} hours."; // Placeholder
        }

        private void UpdatePlotModel(List<ProductionMetric> data)
        {
            PlotModel.Series.Clear();

            // Create BarSeries based on data
            var barSeries = new BarSeries
            {
                Title = "Downtime Analysis",
                ItemsSource = data.Select(pm => new BarItem((double)(pm.Downtime ?? 0))).ToList()
            };

            PlotModel.Series.Add(barSeries);

            // Add axes
            var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom };
            foreach (var pm in data)
            {
                categoryAxis.Labels.Add(pm.MetricDate.ToString("MM/dd/yyyy")); // Adjust the date format as needed
            }

            PlotModel.Axes.Clear();
            PlotModel.Axes.Add(categoryAxis);
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Downtime (Hours)" });

            OnPropertyChanged(nameof(PlotModel));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
