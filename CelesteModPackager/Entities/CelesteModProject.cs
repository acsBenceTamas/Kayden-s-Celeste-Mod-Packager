using CelesteModPackager.Helpers;
using System;
using System.Collections.ObjectModel;

namespace CelesteModPackager.Entities
{
    [Serializable]
    public class CelesteModProject : PropertyOwner
    {
        private string _userName = "UnknownCreator";
        private bool _isCodeMod = false;
        private bool _createEnglishTxt = false;
        private string _campaignName = "Unknown Campaign";

        public EverestMetadata EverestMetadata { get; private set; }
        public string ProjectName
        {
            get => EverestMetadata.ProjectMetadata.Name;
            set
            {
                value = HelperFunctions.MakeValidIDName( value );
                if ( value != EverestMetadata.ProjectMetadata.Name )
                {
                    EverestMetadata.ProjectMetadata.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool IsCodeMod
        {
            get => _isCodeMod;
            set
            {
                if ( value != _isCodeMod )
                {
                    _isCodeMod = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool CreateEnglishTxt
        {
            get => _createEnglishTxt; 
            set
            {
                if ( _createEnglishTxt != value )
                {
                    _createEnglishTxt = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                value = HelperFunctions.MakeValidIDName( value );
                if ( value != _userName )
                {
                    _userName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string MapsPath { get { return @$"Maps\{UserName}\{ProjectName}"; } }
        public ObservableCollection<LevelData> SelectedLevels { get; private set; } = new ObservableCollection<LevelData>();
        public string CampaignName
        {
            get => _campaignName; 
            set
            {
                value = HelperFunctions.MakeValidValueName( value );
                if ( _campaignName != value )
                {
                    _campaignName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public CelesteModProject()
        {
            EverestMetadata = new EverestMetadata( "UnnamedProject" );
        }

        public CelesteModProject( EverestMetadata everestMetadata )
        {
            EverestMetadata = everestMetadata;
            IsCodeMod = everestMetadata.DLL != string.Empty && everestMetadata.DLL != null;
        }

        public void AddDependency( string dependency, VersionNumber version )
        {
            EverestMetadata.Dependencies.Add( new ProjectMetadata( dependency, version ) );
        }

        public void AddDependency( ProjectMetadata dependency )
        {
            EverestMetadata.Dependencies.Add( dependency );
        }
    }
}
