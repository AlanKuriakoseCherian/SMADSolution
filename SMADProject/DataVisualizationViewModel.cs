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
    public class DataVisualizationViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ProductionLine> _productionLines;
        private ProductionLine _selectedProductionLine;
        private DateTime? _startDate;
        private DateTime? _endDate;
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

        public PlotModel PlotModel
        {
            get => _plotModel;
            set
            {
                _plotModel = value;
                OnPropertyChanged(nameof(PlotModel));
            }
        }

        public ICommand LoadDataCommand { get; }

        public DataVisualizationViewModel()
        {
            // Initialize collections
            ProductionLines = new ObservableCollection<ProductionLine>();

            // Load production lines
            LoadProductionLines();

            // Setup command
            LoadDataCommand = new RelayCommand(param => LoadData());
        }

        private void LoadProductionLines()
        {
            using (var context = new SmadDbEntities())
            {
                var lines = context.ProductionLines.ToList();
                ProductionLines = new ObservableCollection<ProductionLine>(lines);
            }
        }

        private void LoadData()
        {
            if (SelectedProductionLine == null || StartDate == null || EndDate == null)
            {
                Console.WriteLine("Please select a production line and a valid date range.");
                return;
            }

            using (var context = new SmadDbEntities())
            {
                var data = context.ProductionMetrics
                    .Where(pm => pm.LineID == SelectedProductionLine.LineID
                                 && pm.MetricDate >= StartDate
                                 && pm.MetricDate <= EndDate)
                    .ToList();

                Console.WriteLine($"Data Count: {data.Count}"); // Debugging output

                if (data.Count == 0)
                {
                    Console.WriteLine("No data found for the selected line and date range.");
                    UpdatePlotModel(new List<ProductionMetric>()); // Clear the graph if no data
                    return;
                }

                // Update the PlotModel
                UpdatePlotModel(data);
            }
        }

        private void UpdatePlotModel(List<ProductionMetric> data)
        {
            try
            {
                // Initialize a new PlotModel
                var newPlotModel = new PlotModel { Title = "Production Metrics" };

                // Create and configure the line series
                var lineSeries = new LineSeries { Title = "Production Rate" };

                if (data.Count == 0)
                {
                    // Adding some test points if no real data is found
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(-2)), 50));
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(-1)), 75));
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), 100));
                }
                else
                {
                    foreach (var item in data)
                    {
                        double productionRateValue = (double)(item.ProductionRate ?? 0);
                        lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(item.MetricDate), productionRateValue));
                    }
                }

                // Configure the axes
                var dateAxis = new DateTimeAxis
                {
                    Position = AxisPosition.Bottom,
                    StringFormat = "MM/dd/yyyy",
                    Title = "Date",
                    IntervalType = DateTimeIntervalType.Days,
                    MinorIntervalType = DateTimeIntervalType.Hours,
                    IsZoomEnabled = true,
                    IsPanEnabled = true
                };

                var valueAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Production Rate",
                    IsZoomEnabled = true,
                    IsPanEnabled = true
                };

                // Clear previous axes and series, then add the new ones
                newPlotModel.Axes.Clear();
                newPlotModel.Axes.Add(dateAxis);
                newPlotModel.Axes.Add(valueAxis);
                newPlotModel.Series.Clear();
                newPlotModel.Series.Add(lineSeries);

                // Set the updated PlotModel
                PlotModel = newPlotModel;
                PlotModel.InvalidatePlot(true); // Force refresh
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while updating plot model: " + ex.Message);
            }

            OnPropertyChanged(nameof(PlotModel));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
