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

namespace Corathing.UI.WPF.Controls.CircularProgressBars;

public partial class CircularProgressBar : ProgressBar, INotifyPropertyChanged
{
    #region Public Properties
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
    public static readonly DependencyProperty StrokeThicknessProperty =
        DependencyProperty.Register(
            nameof(StrokeThickness),
            typeof(double),
            typeof(CircularProgressBar),
            new PropertyMetadata(10.0));

    public double StrokeThickness
    {
        get => (double)GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    // Using a DependencyProperty as the backing store for InnerThickness.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty InnerThicknessProperty =
        DependencyProperty.Register(
            nameof(InnerThickness),
            typeof(double),
            typeof(CircularProgressBar),
            new PropertyMetadata(10.0));

    public double InnerThickness
    {
        get => (double)GetValue(InnerThicknessProperty);
        set => SetValue(InnerThicknessProperty, value);
    }

    #endregion

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    //public void SetProperty<T>(ref T  )
    #endregion


    #region Internal Properties
    // https://stackoverflow.com/questions/33015016/hidden-public-property-in-wpf-control

    public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register(
        nameof(StartPoint),
        typeof(Point),
        typeof(CircularProgressBar),
        new PropertyMetadata(new Point(0, 0)));

    [Browsable(false)]
    public Point StartPoint
    {
        get => (Point)GetValue(StartPointProperty);
        set => SetValue(StartPointProperty, value);
    }

    public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
        nameof(Diameter),
        typeof(double),
        typeof(CircularProgressBar),
        new PropertyMetadata(0.0));

    public double Diameter
    {
        get;
        set;
    }
    public Thickness HighlightStrokeMargin { get; set; }
    public Size HighlightSize { get; set; }
    public Thickness ShadowStorkeMargin { get; set; }
    public double OuterRadius { get; set; }
    public double MidRadius { get; set; }
    public double InnerRadius { get; set; }

    #endregion

    public CircularProgressBar()
    {
        ValueChanged += CircularProgressBar_ValueChanged;
        SizeChanged += CircularProgressBar_SizeChanged;
    }

    private static void OnRadiusChanged(object sender)
    {
        if (sender is CircularProgressBar bar)
        {
            var marginOffset = Math.Max(bar.StrokeThickness, bar.InnerThickness) / 2;
            bar.Diameter = bar.Radius * 2;
            bar.HighlightStrokeMargin = new Thickness(bar.StrokeThickness - marginOffset);
            bar.ShadowStorkeMargin = new Thickness(bar.InnerThickness / 2 - marginOffset);
            bar.MidRadius = bar.Radius - (bar.StrokeThickness - marginOffset);
            bar.HighlightSize = new Size(bar.Radius - (bar.StrokeThickness - marginOffset), bar.Radius - (bar.StrokeThickness - marginOffset));
            bar.StartPoint = new Point(bar.Radius - (bar.StrokeThickness - marginOffset), 0);
            //bar.RaiseEvent(new RoutedEventArgs());
        }
    }

    private void CircularProgressBar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is CircularProgressBar bar)
        {
            var diameter = Math.Min(bar.ActualWidth, bar.ActualHeight);

            bar.Radius = diameter / 2;
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
