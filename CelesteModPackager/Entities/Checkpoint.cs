using CelesteModPackager.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CelesteModPackager.Entities
{
    [Serializable]
    public class Checkpoint : PropertyOwner
    {
        private string _checkpointName;
        private string _picturePath = string.Empty;

        public string RoomName { get; private set; }
        public string CheckpointName
        {
            get => _checkpointName;
            set
            {
                value = HelperFunctions.MakeValidIDName( value );
                if ( _checkpointName != value )
                {
                    _checkpointName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string PicturePath
        {
            get => _picturePath; 
            set
            {
                if ( _picturePath != value )
                {
                    _picturePath = value;
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
