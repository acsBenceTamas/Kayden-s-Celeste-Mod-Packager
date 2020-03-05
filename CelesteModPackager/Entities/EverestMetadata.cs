using CelesteModPackager.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using YamlDotNet.Serialization;

namespace CelesteModPackager.Entities
{
    [Serializable]
    public class EverestMetadata : PropertyOwner
    {
        private string _DLL;

        [YamlMember( Order = 1 )]
        public string Name { get => ProjectMetadata.Name; set => ProjectMetadata.Name = value; }
        [YamlMember( Order = 2 )]
        public string Version { get => ProjectMetadata.Version.VersionText; set => ProjectMetadata.Version = new VersionNumber( value ); }
        [YamlMember( Order = 3 )]
        public string DLL
        {
            get => _DLL; set
            {
                if ( value != _DLL)
                {
                    _DLL = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [YamlIgnore]
        public ProjectMetadata ProjectMetadata { get; set; }
        [YamlMember( Order = 4 )]
        public ObservableCollection<ProjectMetadata> Dependencies { get; private set; } = new ObservableCollection<ProjectMetadata>();

        public EverestMetadata( string projectName )
        {
            ProjectMetadata = new ProjectMetadata( projectName, new VersionNumber() );
        }

        public EverestMetadata( )
        {
            ProjectMetadata = new ProjectMetadata();
        }
    }
}
