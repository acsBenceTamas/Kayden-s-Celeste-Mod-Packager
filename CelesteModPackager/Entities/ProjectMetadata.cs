using CelesteModPackager.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace CelesteModPackager.Entities
{
    [Serializable]
    public class ProjectMetadata : PropertyOwner
    {
        private string _projectName;
        private VersionNumber _version;

        public string Name
        {
            get => _projectName;
            set
            {
                value = HelperFunctions.MakeValidIDName( value );
                if ( value != _projectName )
                {
                    _projectName = value;
                }
                NotifyPropertyChanged();
            }
        }
        [YamlMember(typeof(string))]
        public VersionNumber Version
        {
            get => _version;
            set
            {
                if ( value != _version )
                {
                    _version = value;
                }
                NotifyPropertyChanged();
            }
        }
        [YamlIgnore]
        public bool IsValid
        {
            get
            {
                return Name != string.Empty && ( (Version.Major + Version.Minor) > 0 );
            }
        }

        public ProjectMetadata( string projectName, string version ) :
            this( projectName, new VersionNumber( version ) )
        { }

        public ProjectMetadata( string projectName, VersionNumber version )
        {
            Name = projectName;
            Version = version;
        }

        public ProjectMetadata( ProjectMetadata other )
        {
            Name = other.Name;
            Version = new VersionNumber( other.Version );
        }

        public ProjectMetadata()
        {
            Version = new VersionNumber();
        }

        public override string ToString()
        {
            return $"{Name} v{Version}";
        }
    }
}
