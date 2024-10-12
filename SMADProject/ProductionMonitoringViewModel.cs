using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SMADProject.ViewModels
{
    public class ProductionMonitoringViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ProductionMetric> _productionMetrics;
        private ObservableCollection<ProductionLine> _productionLines;
        private ProductionLine _selectedProductionLine;
        private DateTime? _startDate;
        private DateTime? _endDate;

        public ObservableCollection<ProductionMetric> ProductionMetrics
        {
            get => _productionMetrics;
            set
            {
                _productionMetrics = value;
                OnPropertyChanged(nameof(ProductionMetrics));
            }
        }

        public ObservableCollection<ProductionLine> ProductionLines
        {
            get => _productionLines;
            set
            {
                _productionLines = value;
                OnPropertyChanged(nameof(ProductionLines));
            }
        }

        private PlotModel _plotModel;
        public PlotModel PlotModel
        {
            get => _plotModel;
            set
            {
                _plotModel = value;
                OnPropertyChanged(nameof(PlotModel));
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

        public ICommand FilterCommand { get; }

        public ProductionMonitoringViewModel()
        {
            // Initialize collections
            ProductionMetrics = new ObservableCollection<ProductionMetric>();
            ProductionLines = new ObservableCollection<ProductionLine>();

            // Load data
            LoadProductionLines();
            LoadProductionMetrics();

            // Setup commands
            FilterCommand = new RelayCommand(param => FilterMetrics());
        }

        private void LoadProductionLines()
        {
            using (var context = new SmadDbEntities())
            {
                var lines = context.ProductionLines.ToList();
                ProductionLines = new ObservableCollection<ProductionLine>(lines);
            }
        }

        private void LoadProductionMetrics()
        {
            using (var context = new SmadDbEntities())
            {
                var metrics = context.ProductionMetrics.ToList();
                ProductionMetrics = new ObservableCollection<ProductionMetric>(metrics);
            }
        }

        private void FilterMetrics()
        {
            using (var context = new SmadDbEntities())
            {
                var query = context.ProductionMetrics.AsQueryable();

                if (SelectedProductionLine != null)
                {
                    query = query.Where(m => m.LineID == SelectedProductionLine.LineID);
                }

                if (StartDate.HasValue)
                {
                    query = query.Where(m => m.MetricDate >= StartDate.Value);
                }

                if (EndDate.HasValue)
                {
                    query = query.Where(m => m.MetricDate <= EndDate.Value);
                }

                var filteredMetrics = query.ToList();
                ProductionMetrics = new ObservableCollection<ProductionMetric>(filteredMetrics);
                UpdatePlotModel();
            }
        }

        private void UpdatePlotModel()
        {
            var plotModel = new PlotModel { Title = "Production Metrics" };
            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "dd-MM-yyyy",
                Title = "Date"
            };
            plotModel.Axes.Add(dateAxis);

            var productionRateSeries = new LineSeries { Title = "Production Rate" };
            foreach (var metric in ProductionMetrics)
            {
                productionRateSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(metric.MetricDate), (double)metric.ProductionRate));
            }
            plotModel.Series.Add(productionRateSeries);

            var efficiencySeries = new LineSeries { Title = "Efficiency (%)" };
            foreach (var metric in ProductionMetrics)
            {
                efficiencySeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(metric.MetricDate), (double)metric.Efficiency));
            }
            plotModel.Series.Add(efficiencySeries);

            PlotModel = plotModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
