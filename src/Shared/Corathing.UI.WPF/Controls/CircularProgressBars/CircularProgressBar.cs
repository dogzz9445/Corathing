using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata;

namespace Corathing.UI.WPF.Controls.CircularProgressBars;

public partial class CircularProgressBar : ProgressBar, INotifyPropertyChanged
{
    #region Constants Brushes
    private static readonly SolidColorBrush DefaultHighlightBrush =
        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#23FF23"));
    private static readonly SolidColorBrush DefaultShadowBrush =
        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#242424"));

    #endregion

    #region Public Properties
    public static readonly DependencyProperty HighlightStrokeProperty =
        DependencyProperty.Register(
            nameof(HighlightStroke),
            typeof(Brush),
            typeof(CircularProgressBar),
            new PropertyMetadata(DefaultHighlightBrush)
            );

    public Brush HighlightStroke
    {
        get => (Brush)GetValue(HighlightStrokeProperty);
        set => SetValue(HighlightStrokeProperty, value);
    }

    public static readonly DependencyProperty ShadowStrokeProperty =
        DependencyProperty.Register(
            nameof(ShadowStroke),
            typeof(Brush),
            typeof(CircularProgressBar),
            new PropertyMetadata(DefaultShadowBrush))
        ;

    public Brush ShadowStroke
    {
        get => (Brush)GetValue(ShadowStrokeProperty);
        set => SetValue(ShadowStrokeProperty, value);
    }


    public static readonly DependencyProperty SweepDirectionProperty =
        DependencyProperty.Register(
            nameof(SweepDirection),
            typeof(System.Windows.Media.SweepDirection),
            typeof(CircularProgressBar),
            new PropertyMetadata(System.Windows.Media.SweepDirection.Counterclockwise));

    public System.Windows.Media.SweepDirection SweepDirection
    {
        get => (System.Windows.Media.SweepDirection)GetValue(SweepDirectionProperty);
        set => SetValue(SweepDirectionProperty, value);
    }

    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register(
            nameof(Radius),
            typeof(double),
            typeof(CircularProgressBar),
            new PropertyMetadata(50.0, (s,e) => OnRadiusChanged(s)));

    public double Radius
    {
        get => (double)GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    // Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register(
            nameof(Angle),
            typeof(double),
            typeof(CircularProgressBar),
            new PropertyMetadata(0.0));

    public double Angle
    {
        get => (double)GetValue(AngleProperty);
        set => SetValue(AngleProperty, value);
    }

    // Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty StartAngleProperty =
        DependencyProperty.Register(
            nameof(StartAngle),
            typeof(double),
            typeof(CircularProgressBar),
            new PropertyMetadata(0.0));

    public double StartAngle
    {
        get => (double)GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }


    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(
            nameof(Duration),
            typeof(double),
            typeof(CircularProgressBar),
            new PropertyMetadata(100.0));

    public double Duration
    {
        get => (double)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    // Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HighlightThicknessProperty =
        DependencyProperty.Register(
            nameof(HighlightThickness),
            typeof(double),
            typeof(CircularProgressBar),
            new PropertyMetadata(10.0));

    public double HighlightThickness
    {
        get => (double)GetValue(HighlightThicknessProperty);
        set => SetValue(HighlightThicknessProperty, value);
    }

    // Using a DependencyProperty as the backing store for InnerThickness.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ShadowThicknessProperty =
        DependencyProperty.Register(
            nameof(ShadowThickness),
            typeof(double),
            typeof(CircularProgressBar),
            new PropertyMetadata(10.0));

    public double ShadowThickness
    {
        get => (double)GetValue(ShadowThicknessProperty);
        set => SetValue(ShadowThicknessProperty, value);
    }

    #endregion

    #region Implement INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    public void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool SetProperty<T>(ref T member, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(member, value))
            return false;
        member = value;
        RaisePropertyChanged(propertyName);
        return true;
    }
    #endregion


    #region Internal Properties
    // https://stackoverflow.com/questions/33015016/hidden-public-property-in-wpf-control

    private Point _highlightStartPoint;
    public Point HighlightStartPoint { get => _highlightStartPoint; set => SetProperty(ref _highlightStartPoint, value); }

    private Point _highlightPoint;
    public Point HighlightPoint { get => _highlightPoint; set => SetProperty(ref _highlightPoint, value); }

    private double _diameter;
    public double Diameter { get => _diameter; set => SetProperty(ref _diameter, value); }

    private Thickness _marginOffset;
    public Thickness StorkeMargin { get => _marginOffset; set => SetProperty(ref _marginOffset, value); }

    private Thickness _highlightStorkeMargin;
    public Thickness HighlightStrokeMargin { get => _highlightStorkeMargin; set => SetProperty(ref _highlightStorkeMargin, value); }

    public static readonly DependencyProperty HighlightSizeProperty =
        DependencyProperty.Register(
            nameof(HighlightSize),
            typeof(Size),
            typeof(CircularProgressBar),
            new FrameworkPropertyMetadata(new Size(0, 0)));

    [Browsable(false)]
    public Size HighlightSize
    {
        get => (Size)GetValue(HighlightSizeProperty);
        set => SetValue(HighlightSizeProperty, value);
    }

    private Thickness _shadowStorkeMargin;
    public Thickness ShadowStorkeMargin { get => _shadowStorkeMargin; set => SetProperty(ref _shadowStorkeMargin, value); }

    private double _highlightRadius;
    public double HighlightRadius { get => _highlightRadius; set => SetProperty(ref _highlightRadius, value); }

    private double _highlightDiameter;
    public double HighlightDiameter { get => _highlightDiameter; set => SetProperty(ref _highlightDiameter, value); }

    private double _shadowDiameter;
    public double ShadowDiameter { get => _shadowDiameter; set => SetProperty(ref _shadowDiameter, value); }
    #endregion

    public CircularProgressBar()
    {
        HighlightStartPoint = new Point(0, 0);
        HighlightSize = new Size(0, 0);
        SweepDirection = SweepDirection.Clockwise;

        ValueChanged += CircularProgressBar_ValueChanged;
        SizeChanged += CircularProgressBar_SizeChanged;
    }

    private static void OnRadiusChanged(object sender)
    {
        if (sender is CircularProgressBar bar)
        {
            var marginOffset = Math.Abs(bar.HighlightThickness - bar.ShadowThickness) / 2;
            if (bar.HighlightThickness >= bar.ShadowThickness)
            {
                bar.HighlightStrokeMargin = new Thickness((bar.HighlightThickness)/ 2);
                bar.ShadowStorkeMargin = new Thickness(marginOffset);
            }
            else
            {
                bar.HighlightStrokeMargin = new Thickness(marginOffset + (bar.HighlightThickness / 2));
                bar.ShadowStorkeMargin = new Thickness(0);
            }
            bar.HighlightDiameter = (bar.Radius - bar.HighlightStrokeMargin.Top) * 2;
            bar.ShadowDiameter = (bar.Radius - bar.ShadowStorkeMargin.Top) * 2;
            bar.HighlightRadius = bar.HighlightDiameter / 2;
            bar.HighlightStartPoint = new Point(bar.HighlightRadius + bar.HighlightStrokeMargin.Left, bar.HighlightStrokeMargin.Top);
            bar.HighlightSize = new Size(bar.HighlightRadius, bar.HighlightRadius);

            bar.RaisePropertyChanged(nameof(Radius));
        }
    }

    private void CircularProgressBar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is CircularProgressBar bar)
        {
            var diameter = Math.Min(bar.ActualWidth, bar.ActualHeight);

            bar.Radius = diameter / 2;
            bar.Diameter = diameter;
        }
    }

    private void CircularProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        CircularProgressBar bar = sender as CircularProgressBar;
        double currentAngle = bar.Angle;
        double targetAngle = e.NewValue / bar.Maximum * 359.999;

        DoubleAnimation anim = new DoubleAnimation(currentAngle, targetAngle, TimeSpan.FromMilliseconds(Duration));
        bar.BeginAnimation(AngleProperty, anim, HandoffBehavior.SnapshotAndReplace);
    }
}
