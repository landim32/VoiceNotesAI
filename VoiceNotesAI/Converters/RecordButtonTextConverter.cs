using System.Globalization;

namespace VoiceNotesAI.Converters;

public class RecordButtonTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isRecording)
            return isRecording ? "⏹️  Parar Gravação" : "🎙️  Iniciar Gravação";
        return "🎙️  Iniciar Gravação";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
