using InBodyPDFExtractor.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace InBodyPDFExtractor.Convertors;

[ValueConversion(typeof(object), typeof(Visibility))]
internal class NullToInvisibleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return Visibility.Collapsed;
        }
        else
        {
            if (value is string valString)
                return string.IsNullOrEmpty(valString) ? Visibility.Collapsed : Visibility.Visible;
            else return Visibility.Visible;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

[ValueConversion(typeof(JobStatus), typeof(SolidColorBrush))]
internal class JobStatusToSolidColorBrushConverter : IValueConverter
{
    public JobStatusToSolidColorBrushConverter()
    {
        _solidColorBrushs.Add(JobStatus.NotStart, new SolidColorBrush(Colors.Transparent));
        _solidColorBrushs.Add(JobStatus.Running, new SolidColorBrush(Colors.LightGoldenrodYellow));
        _solidColorBrushs.Add(JobStatus.Finish, new SolidColorBrush(Colors.GreenYellow));
        _solidColorBrushs.Add(JobStatus.Error, new SolidColorBrush(Colors.OrangeRed));
    }

    private Dictionary<JobStatus, SolidColorBrush> _solidColorBrushs = new();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is JobStatus valJS)
        {
            return _solidColorBrushs[valJS];
        }
        else
        {
            throw new ArgumentException("Only JobStatus source object is supported");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}