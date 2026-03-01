using System.Globalization;

namespace VoiceNotesAI.Converters;

public class RecordButtonColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isRecording)
            return isRecording ? Color.FromArgb("#E53935") : Color.FromArgb("#6750A4");
        return Color.FromArgb("#6750A4");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
