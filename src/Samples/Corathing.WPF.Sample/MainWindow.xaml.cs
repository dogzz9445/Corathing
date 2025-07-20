using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using Corathing.WPF.Sample.ViewModels;
using LiveChartsCore.Defaults;

using LiveChartsCore;
using LiveChartsCore.Drawing;

using LiveChartsCore.SkiaSharpView.WPF;

using Microsoft.Win32;

using SkiaSharp;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace Corathing.WPF.Sample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    //private readonly IPopupService _popupService;

    public MainWindow()
    {
        InitializeComponent();

        // ViewModel 생성 및 연결
        _viewModel = new MainWindowViewModel();
        this.DataContext = _viewModel;


		//Series = new ObservableCollection<ISeries>
		//{
  //          // use the second type argument to specify the geometry to draw for every point
  //          // there are already many predefined geometries in the
  //          // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
  //          new LineSeries<ObservablePoint>
		//	{
		//	Values = ValuePixel,
		//	Fill = null,
		//	GeometrySize = 0,
  //          // use the line smoothness property to control the curve
  //          // it goes from 0 to 1
  //          // where 0 is a straight line and 1 the most curved
  //          //  LineSmoothness = 0, // mark
  //          GeometryStroke = null,
		//	Stroke = new SolidColorPaint(SKColors.Black, 1),
  //          //Fill = null
  //          }
		//};
	}

	//public IEnumerable<ISeries> Series { get; set; }
	//public Axis[] XAxes { get; set; } =
	//{
	//		new Axis
	//		{
	//			Name = "axisname",
	//			NamePaint = new SolidColorPaint { Color = SKColors.Black },
	//			 SeparatorsPaint = new SolidColorPaint
	//			{
	//				Color = SKColors.Gray,
	//				StrokeThickness = 2

	//			},
	//			 MinStep=500,
	//		}
	//	};

	//public Axis[] YAxes { get; set; } =
	//{
	//		new Axis
	//		{
	//			Name = "Brightness(level)",
	//			MinStep=64,
	//			 NamePaint = new SolidColorPaint { Color = SKColors.Black },
	//			  SeparatorsPaint = new SolidColorPaint
	//			{
	//				Color = SKColors.Gray,
	//				StrokeThickness = 2

	//			},
	//		}
	//	};
	//public ObservableCollection<RectangularSection> Sections { get; set; } = new()
	//	{
	//	   new RectangularSection
	//	   {
	//		   Xi =0,
	//		   Xj = 0,
	//		   Fill = new SolidColorPaint(new SKColor(255, 0, 0))
	//	   }
	//	};

	//private void Pixelchart_MouseDown(object sender, MouseButtonEventArgs e)
	//{
	//	//  checkboxzooom.IsChecked = false;
	//	//  Pixelchart.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.None;
	//	var chart = (CartesianChart)FindName("Pixelchart");
	//	var p = e.GetPosition(chart);
	//	var scaledPoint = chart.ScaleUIPoint(new LvcPoint((float)p.X, (float)p.Y));

	//	// where the X coordinate is in the first position
	//	var x1 = scaledPoint[0];
	//	//var y = scaledPoint[1];

	//	int near = (int)((int)x1;
	//	int idx = ValuePixel.ToList().FindIndex(x => x.X == near);

	//	this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
	//	{
	//		if (idx >= 0)
	//		{
	//			ValuePixel[idx].X.ToString();
	//			ValuePixel[idx].Y.ToString();
	//		}

	//	}));
	//}
}
