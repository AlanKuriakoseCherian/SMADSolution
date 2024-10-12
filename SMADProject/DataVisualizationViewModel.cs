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
                    // Optional: You can call UpdatePlotModel with some test data here
                    UpdatePlotModel(new List<ProductionMetric>()); // Clear the graph if no data
                    return;
                }

                // Update the PlotModel
                UpdatePlotModel(data);
            }
        }

        private void UpdatePlotModel(List<ProductionMetric> data)
        {
            PlotModel = new PlotModel { Title = "Production Metrics" };
            var lineSeries = new LineSeries { Title = "Production Rate" };

            // If data is empty, you can add test points for verification
            if (data.Count == 0)
            {
                // Hardcoded test data for verification
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(-2)), 50));
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddDays(-1)), 75));
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), 100));
            }
            else
            {
                foreach (var item in data)
                {
                    // Convert ProductionRate to double and handle nullable type
                    double productionRateValue = (double)(item.ProductionRate ?? 0);
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(item.MetricDate), productionRateValue));
                }
            }

            PlotModel.Series.Clear();
            PlotModel.Series.Add(lineSeries);
            OnPropertyChanged(nameof(PlotModel));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
