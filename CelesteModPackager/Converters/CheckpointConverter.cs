using CelesteModPackager.Entities;
using CelesteModPackager.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace CelesteModPackager.Converters
{
    [ValueConversion( typeof( Checkpoint ), typeof( CheckpointViewModel ) )]
    public class CheckpointConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( value.GetType() == typeof( Checkpoint ) )
            {
                return new CheckpointViewModel( (Checkpoint)value );
            }
            else
            {
                throw new ArgumentException( "Must be a Checkpoint.", "value" );
            }
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( value.GetType() == typeof( CheckpointViewModel ) )
            {
                return ( (CheckpointViewModel)value ).Checkpoint;
            }
            else
            {
                throw new ArgumentException( "Must be a CheckpointViewModel.", "value" );
            }
        }
    }
}
