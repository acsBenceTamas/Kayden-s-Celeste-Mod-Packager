using System.Text.RegularExpressions;

namespace CelesteModPackager.Helpers
{
    public static class HelperFunctions
    {
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
    }
}
