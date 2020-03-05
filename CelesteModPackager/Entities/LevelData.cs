using CelesteEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace CelesteModPackager.Entities
{
    [Serializable]
    public class LevelData
    {
        public string FilePath { get; private set; }
        public string LevelName { get; set; }
        public ObservableCollection<Checkpoint> Checkpoints { get; private set; } = new ObservableCollection<Checkpoint>();

        public LevelData( string filePath )
        {
            FilePath = filePath;
            MapElement mapElement = MapCoder.FromBinary( filePath );
            GatherCheckpoints( mapElement );
            LevelName = Path.GetFileNameWithoutExtension( filePath );
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
