using CelesteEditor;
using CelesteModPackager.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace CelesteModPackager.Entities
{
    [Serializable]
    public class LevelData : PropertyOwner
    {
        private string _previewImagePath = string.Empty;
        private string _levelName = string.Empty;
        private string _poem = string.Empty;
        private char _side;

        public string FilePath { get; private set; }
        public string AreaName { get; set; }
        public char Side { get => _side; set => _side = value; }
        public string LevelName
        {
            get => _levelName;
            set
            {
                if ( value != _levelName )
                {
                    _levelName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Poem
        {
            get => _poem;
            set
            {
                if ( value != _poem )
                {
                    _poem = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string PreviewImagePath
        {
            get => _previewImagePath;
            set
            {
                if ( value != _previewImagePath )
                {
                    _previewImagePath = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<Checkpoint> Checkpoints { get; private set; } = new ObservableCollection<Checkpoint>();

        public LevelData( string filePath )
        {
            FilePath = filePath;
            MapElement mapElement = MapCoder.FromBinary( filePath );
            GatherCheckpoints( mapElement );
            LevelName = Path.GetFileNameWithoutExtension( filePath );
            HelperFunctions.ParseSide( filePath, out _side );
        }

        private void GatherCheckpoints( MapElement rootElement )
        {
            MapElement levels = GetChildWithName( rootElement, "levels" );

            foreach ( MapElement level in levels.Children )
            {
                MapElement entities = GetChildWithName( level, "entities" );

                MapElement checkpoint = GetChildWithName( entities, "checkpoint" );
                if ( checkpoint == null ) continue;

                string levelName = level.Attr( "name" );
                Checkpoints.Add( new Checkpoint( levelName, levelName ) );
            }
        }

        private static MapElement GetChildWithName( MapElement rootElement, string name )
        {
            return rootElement.Children.Find( ( element ) => { return element.Name == name; } );
        }
    }
}
