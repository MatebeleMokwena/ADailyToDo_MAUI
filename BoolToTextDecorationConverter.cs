using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDailyToDo.TheToDo
{
    public class BoolToTextDecorationConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if ((value != null))
            {
                bool b = (bool)value;
                if (b) { return TextDecorations.Strikethrough; }
                return TextDecorations.None;
            }
            return TextDecorations.None;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
