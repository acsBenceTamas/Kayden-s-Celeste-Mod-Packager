using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CelesteModPackager.Helpers
{
    public static class HelperFunctions
    {
        private static Regex ParseNameRegex = new Regex( @"^(?:(?<order>\d+)(?<side>[ABCHX]?)\-)?(?<name>.+?)(?:\-(?<sideAlt>[ABCHX]?))?$", RegexOptions.Compiled );
        private static Dictionary<string, Match> ParseNameCache = new Dictionary<string, Match>();
        private static Dictionary<string, char> ParseNameModes = new Dictionary<string, char>() {
            { "A", 'A' },
            { "B", 'B' },
            { "H", 'B' },
            { "C", 'C' },
            { "X", 'C' },
        };

        public static string MakeValidIDName( string value )
        {
            value = Regex.Replace( value, "[\\-\\.\\s]", "_" );
            value = Regex.Replace( value, "[^\\w\\d_]", "" );
            return value;
        }

        public static string MakeValidValueName( string value )
        {
            value = Regex.Replace( value, "[=]", "" );
            return value;
        }

        public static void ParseSide( string sid, out char side )
        {
            int indexOfSlash = sid.Replace( '\\', '/' ).LastIndexOf( '/' );
            if ( indexOfSlash != -1 )
                sid = sid.Substring( indexOfSlash + 1 );
            if ( sid.EndsWith( ".bin" ) )
                sid = sid.Substring( 0, sid.Length - 4 );

            Match match;
            if ( !ParseNameCache.TryGetValue( sid, out match ) )
                ParseNameCache[ sid ] = match = ParseNameRegex.Match( sid );

            string rawSide = match.Groups[ "side" ].Value;
            if ( string.IsNullOrEmpty( rawSide ) )
                rawSide = match.Groups[ "sideAlt" ].Value;
            string rawName = match.Groups[ "name" ].Value;

            if ( !string.IsNullOrEmpty( rawName ) && string.IsNullOrEmpty( rawSide ) && ParseNameModes.ContainsKey( rawName ) )
            {
                rawSide = rawName;
            }

            if ( !ParseNameModes.TryGetValue( rawSide, out side ) )
                side = 'A';
        }
    }
}
