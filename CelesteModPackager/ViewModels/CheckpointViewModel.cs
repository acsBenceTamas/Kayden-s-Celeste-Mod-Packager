using CelesteModPackager.Entities;
using CelesteModPackager.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CelesteModPackager.ViewModels
{
    public class CheckpointViewModel
    {
        public Checkpoint Checkpoint { get; set; }
        public ICommand SelectPreviewImageCommand { get; set; }

        public CheckpointViewModel( Checkpoint checkpoint )
        {
            Checkpoint = checkpoint;
            SelectPreviewImageCommand = new RelayCommand( o => SelectPreviewImage_Execute() );
        }

        private void SelectPreviewImage_Execute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Portable Network Graphics *.png|*.png"
            };
            if ( openFileDialog.ShowDialog() == true )
            {
                Checkpoint.PicturePath = openFileDialog.FileName;
            }
        }

    }
}
