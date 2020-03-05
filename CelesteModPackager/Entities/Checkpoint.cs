using CelesteModPackager.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CelesteModPackager.Entities
{
    [Serializable]
    public class Checkpoint : PropertyOwner
    {
        private string checkpointName;

        public string RoomName { get; private set; }
        public string CheckpointName
        {
            get => checkpointName; 
            set
            {
                value = HelperFunctions.MakeValidIDName( value );
                if ( checkpointName != value )
                {
                    checkpointName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Checkpoint( string levelName, string checkpointName )
        {
            RoomName = levelName;
            CheckpointName = checkpointName;
        }

    }
}
