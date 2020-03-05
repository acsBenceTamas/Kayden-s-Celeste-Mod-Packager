using CelesteModPackager.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using YamlDotNet.Serialization;

namespace CelesteModPackager.Entities
{
    [Serializable]
    public class VersionNumber : PropertyOwner
    {
        [YamlIgnore]
        public uint Major 
        { 
            get => _versionNumbers[ 0 ];
            set { 
                if ( _versionNumbers[ 0 ] != value )
                {
                    _versionNumbers[ 0 ] = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [YamlIgnore]
        public uint Minor
        {
            get => _versionNumbers[ 1 ];
            set
            {
                if ( _versionNumbers[ 1 ] != value )
                {
                    _versionNumbers[ 1 ] = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [YamlIgnore]
        public uint Build
        {
            get => _versionNumbers[ 2 ];
            set
            {
                if ( _versionNumbers[ 2 ] != value )
                {
                    _versionNumbers[ 2 ] = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [YamlIgnore]
        public string VersionText
        {
            get
            {
                return string.Join( '.', _versionNumbers );
            }
        }
        private readonly uint[] _versionNumbers = new uint[ 3 ];

        public VersionNumber( VersionNumber other )
        {
            other._versionNumbers.CopyTo( _versionNumbers, 0 );
        }

        public VersionNumber( uint major = 0, uint minor = 0, uint build = 0)
        {
            Major = major;
            Minor = minor;
            Build = build;
        }

        public VersionNumber( string versionAsString )
        {
            string[] numbersAsString = versionAsString.Split( '.' );
            for ( int i = 0; i < 3 && i < numbersAsString.Length; i++ )
            {
                uint number = 0;
                try
                {
                    number = Convert.ToUInt16( numbersAsString[ i ] );
                }
                catch ( Exception ex ) when ( ex is FormatException || ex is OverflowException )
                {
                    Console.WriteLine( ex );
                }
                _versionNumbers[ i ] = number;
            }
        }

        public override string ToString()
        {
            return VersionText;
        }

        public static explicit operator VersionNumber( string source )
        {
            return new VersionNumber( source );
        }
    }
}
