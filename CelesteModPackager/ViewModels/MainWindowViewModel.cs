using CelesteModPackager.Entities;
using CelesteModPackager.Helpers;
using Microsoft.Win32;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Input;
using YamlDotNet.Serialization;
using System;

namespace CelesteModPackager.ViewModels
{
    class MainWindowViewModel : PropertyOwner
    {
        private const string _projectExtension = "cmp";
        private bool _hasUnsavedChanges = false;
        private readonly MainWindow View;
        private CelesteModProject _project;
        private LevelData _selectedMap;

        private bool HasUnsavedChanges
        {
            get => _hasUnsavedChanges;
            set
            {
                if ( _hasUnsavedChanges != value )
                {
                    _hasUnsavedChanges = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged( "CustomTitle" );
                }
            }
        }
        public CelesteModProject Project
        {
            get => _project;
            set
            {
                _project = value;
                HasUnsavedChanges = false;
                NotifyPropertyChanged();
                NotifyPropertyChanged( "CustomTitle" );
                NotifyPropertyChanged( "CheckpointSetupVisibility" );
                NotifyPropertyChanged( "EnglishTxtSetupVisibility" );
                _project.PropertyChanged += ( o, e ) => {
                    if ( e.PropertyName == "CreateEnglishTxt" )
                    {
                        NotifyPropertyChanged( "EnglishTxtSetupVisibility" );
                    }
                    else if ( e.PropertyName == "CreatePreviewImages" )
                    {
                        NotifyPropertyChanged( "PreviewImageSetupVisibility" );
                    }
                };
            }
        }
        public ProjectMetadata NewDependency { get; set; } = new ProjectMetadata( "NewDependency", new VersionNumber( 0, 0, 0 ) );
        public string CustomTitle
        {
            get
            {
                return $"Celeste Mod Packager | {Project.ProjectName}" + ( HasUnsavedChanges ? "*" : "" );
            }
        }

        public LevelData SelectedMap
        {
            get => _selectedMap; set
            {
                if ( _selectedMap != value )
                {
                    _selectedMap = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged( "SelectedMapName" );
                    NotifyPropertyChanged( "CheckpointSetupVisibility" );
                }
            }
        }

        public string SelectedMapName
        {
            get
            {
                if ( SelectedMap != null ) return Path.GetFileName( SelectedMap.FilePath );
                return "None Selected";
            }
        }

        public Visibility CheckpointSetupVisibility
        {
            get
            {
                return ( SelectedMap != null ) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility EnglishTxtSetupVisibility
        {
            get
            {
                return ( Project.CreateEnglishTxt ) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility PreviewImageSetupVisibility
        {
            get
            {
                return ( Project.CreatePreviewImages ) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public ICommand AddMapFilesCommand { get; set; }
        public ICommand RemoveMapFilesCommand { get; set; }
        public ICommand AddDependencyCommand { get; set; }
        public ICommand RemoveDependenciesCommand { get; set; }
        public ICommand CreateNewProjectCommand { get; set; }
        public ICommand PackageProjectCommand { get; set; }
        public ICommand ImportProjectCommand { get; set; }
        public ICommand SaveProjectCommand { get; set; }
        public ICommand LoadProjectCommand { get; set; }
        public ICommand ExportEnglishTxtCommand { get; set; }
        public ICommand BrowseDLLCommand { get; set; }
        public ICommand SelectLevelPreviewImageCommand { get; set; }
        public ICommand ImportDependencyCommand { get; set; }

        public MainWindowViewModel( MainWindow mainWindow )
        {
            View = mainWindow;
            Project = new CelesteModProject();
            AddMapFilesCommand = new RelayCommand( o => AddMapFiles_Execute() );
            RemoveMapFilesCommand = new RelayCommand( o => RemoveMapFiles_Execute(), o => RemoveMapFiles_CanExecute() );
            AddDependencyCommand = new RelayCommand( o => AddDependency_Execute() );
            RemoveDependenciesCommand = new RelayCommand( o => RemoveDependency_Execute(), o => RemoveDependency_CanExecute() );
            CreateNewProjectCommand = new RelayCommand( o => CreateNewProject_Execute() );
            SaveProjectCommand = new RelayCommand( o => SaveProject_Execute() );
            LoadProjectCommand = new RelayCommand( o => LoadProject_Execute() );
            PackageProjectCommand = new RelayCommand( o => PackageProject_Execute(), o => PackageProject_CanExecute() );
            ImportProjectCommand = new RelayCommand( o => ImportProject_Execute() );
            ExportEnglishTxtCommand = new RelayCommand( o => ExportEnglishTxt_Execute(), o => Project.CreateEnglishTxt );
            BrowseDLLCommand = new RelayCommand( o => BrowseDLL_Execute(), o => Project.IsCodeMod );
            SelectLevelPreviewImageCommand = new RelayCommand( o => SelectLevelPreviewImage_Execute() );
            ImportDependencyCommand = new RelayCommand( o => ImportDependency_Execute() );
        }

        public void AddMapFiles_Execute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Binary files *.bin|*.bin"
            };
            if ( openFileDialog.ShowDialog() == true )
            {
                foreach ( string fileName in openFileDialog.FileNames )
                {
                    if ( !Project.SelectedLevels.Any( ( levelData ) => { return levelData.FilePath == fileName; } ) )
                    {
                        Project.SelectedLevels.Add( new LevelData( fileName ) );
                    }
                }
                if ( openFileDialog.FileNames.Length > 0 )
                {
                    HasUnsavedChanges = true;
                }
            }
        }

        public void RemoveMapFiles_Execute()
        {
            LevelData[] itemsToRemove = new LevelData[ View.selectedMapsListView.SelectedItems.Count ];
            for ( int i = 0; i < View.selectedMapsListView.SelectedItems.Count; i++ )
            {
                itemsToRemove[ i ] = (LevelData)View.selectedMapsListView.SelectedItems[ i ];
            }
            foreach ( LevelData item in itemsToRemove )
            {
                Project.SelectedLevels.Remove( item );
            }
            HasUnsavedChanges = true;
        }

        public bool RemoveMapFiles_CanExecute()
        {
            return View.selectedMapsListView != null && View.selectedMapsListView.SelectedItems.Count > 0;
        }

        public void AddDependency_Execute()
        {
            Project.AddDependency( new ProjectMetadata( NewDependency ) );
            HasUnsavedChanges = true;
        }

        public void RemoveDependency_Execute()
        {
            ProjectMetadata[] itemsToRemove = new ProjectMetadata[ View.dependenciesListView.SelectedItems.Count ];
            for ( int i = 0; i < View.dependenciesListView.SelectedItems.Count; i++ )
            {
                itemsToRemove[ i ] = (ProjectMetadata)View.dependenciesListView.SelectedItems[ i ];
            }
            foreach ( ProjectMetadata item in itemsToRemove )
            {
                Project.EverestMetadata.Dependencies.Remove( item );
            }
            HasUnsavedChanges = true;
        }

        public bool RemoveDependency_CanExecute()
        {
            return View.dependenciesListView != null && View.dependenciesListView.SelectedItems.Count > 0;

        }

        private void CreateNewProject_Execute()
        {
            Project = new CelesteModProject();
        }

        private void SaveProject_Execute()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = $"Celeste Mod Project *.{_projectExtension}|*.{_projectExtension}"
            };
            if ( saveFileDialog.ShowDialog() == true )
            {
                SaveProjectToFile( saveFileDialog.FileName );
            }
        }

        private void LoadProject_Execute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = $"Celeste Mod Project *.{_projectExtension}|*.{_projectExtension}"
            };
            if ( openFileDialog.ShowDialog() == true )
            {
                FileInfo file = new FileInfo( openFileDialog.FileName );
                IFormatter formatter = new BinaryFormatter();
                using ( FileStream stream = file.OpenRead() )
                {
                    Project = formatter.Deserialize( stream ) as CelesteModProject;
                }
                HasUnsavedChanges = false;
            }
        }

        private void SaveProjectToFile( string fileName )
        {
            FileInfo file = new FileInfo( fileName );
            if ( !file.Directory.Exists )
            {
                file.Directory.Create();
            }
            IFormatter formatter = new BinaryFormatter();
            using ( FileStream stream = file.OpenWrite() )
            {
                formatter.Serialize( stream, Project );
            }
            HasUnsavedChanges = false;
        }

        private void PackageProject_Execute()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = $"Celeste Mod Project *.{_projectExtension}|*.{_projectExtension}"
            };
            if ( saveFileDialog.ShowDialog() == true )
            {
                SaveProjectToFile( saveFileDialog.FileName );
                string root = Path.GetDirectoryName( saveFileDialog.FileName );

                if ( Project.CreateEnglishTxt )
                {
                    ExportEnglishTxt( Path.Combine( root, @"Dialog/English.txt" ) );
                }

                if ( Project.CreatePreviewImages )
                {
                    PackageCheckpointImages( root );
                }

                ISerializer serializer = new SerializerBuilder()
                    .ConfigureDefaultValuesHandling( DefaultValuesHandling.OmitNull )
                    .ConfigureDefaultValuesHandling( DefaultValuesHandling.OmitDefaults )
                    .Build();

                string yaml = serializer.Serialize( new EverestMetadata[] { Project.EverestMetadata } );
                byte[] data = new UTF8Encoding( true ).GetBytes( yaml );

                FileInfo everestYaml = new FileInfo( Path.Combine( root, "everest.yaml" ) );
                if ( everestYaml.Exists )
                {
                    everestYaml.Delete();
                }
                using ( FileStream stream = everestYaml.OpenWrite() )
                {
                    stream.Write( data, 0, data.Length );
                }

                string mapsRoot = Path.Combine( root, Project.MapsPath );

                DirectoryInfo mapsDirectory = new DirectoryInfo( mapsRoot );
                if ( !mapsDirectory.Exists )
                {
                    mapsDirectory.Create();
                }

                CopySelectedMaps( root );
                if ( Project.IsCodeMod )
                {
                    FileInfo DLL = new FileInfo( Project.EverestMetadata.GlobalDLL );
                    if ( DLL.Exists )
                    {
                        string newPath = Path.Combine( root, Path.GetFileName( DLL.Name ) );
                        File.Delete( newPath );
                        DLL.CopyTo( newPath );
                    }
                }

                HasUnsavedChanges = false;
            }
        }

        private void PackageCheckpointImages( string root )
        {
            foreach ( LevelData level in Project.SelectedLevels )
            {
                string levelName = Path.GetFileNameWithoutExtension( level.FilePath );
                string destinationFolder = Path.Combine( root, "Graphics/Atlases/Checkpoints", Project.UserName, Project.ProjectName, levelName, level.Side.ToString() );
                if ( level.PreviewImagePath != null && level.PreviewImagePath != string.Empty )
                {

                    FileInfo file = new FileInfo( level.PreviewImagePath );

                    if ( file.Exists )
                    {
                        CopyPicture( destinationFolder, file, "start.png" );
                    }
                }
                foreach ( Checkpoint checkpoint in level.Checkpoints )
                {
                    if ( checkpoint.PicturePath == string.Empty ) continue;

                    FileInfo file = new FileInfo( checkpoint.PicturePath );

                    if ( !file.Exists ) continue;

                    CopyPicture( destinationFolder, file, checkpoint.RoomName + ".png" );
                }
            }
        }

        private static void CopyPicture( string destinationFolder, FileInfo file, string destinationFileName )
        {
            string destinationName = Path.Combine( destinationFolder, destinationFileName );

            DirectoryInfo directory = new DirectoryInfo( destinationFolder );
            if ( !directory.Exists )
            {
                directory.Create();
            }

            File.Delete( destinationName );
            file.CopyTo( destinationName );
        }

        private void CopySelectedMaps( string targetDir )
        {
            for ( int i = 0; i < Project.SelectedLevels.Count; i++ )
            {
                string relativeFileName = Path.Combine( Project.MapsPath, Path.GetFileName( Project.SelectedLevels[ i ].FilePath ) );
                FileInfo fileInfo = new FileInfo( Project.SelectedLevels[ i ].FilePath );
                if ( fileInfo.Exists )
                {
                    try
                    {
                        fileInfo.CopyTo( Path.Combine( targetDir, relativeFileName ), true );
                    }
                    catch ( IOException e )
                    {
                        MessageBox.Show( e.Message, "Error copying files" );
                    }
                }
            }
        }

        private bool PackageProject_CanExecute()
        {
            bool properCodeModCheck = !Project.IsCodeMod || ( Project.EverestMetadata.GlobalDLL != null && Project.EverestMetadata.GlobalDLL != string.Empty );
            bool properLevelModCheck = Project.IsCodeMod || Project.UserName != string.Empty && Project.SelectedLevels.Count > 0;
            return Project.ProjectName != string.Empty && properCodeModCheck && properLevelModCheck;
        }

        private void BrowseDLL_Execute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Dynamic Link Library *.dll|*.dll"
            };
            if ( openFileDialog.ShowDialog() == true )
            {
                Project.EverestMetadata.GlobalDLL = openFileDialog.FileName;
            }
        }

        private void ImportProject_Execute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Everest YAML everest.yaml|everest.yaml"
            };
            if ( openFileDialog.ShowDialog() == true )
            {
                FileInfo fileInfo = new FileInfo( openFileDialog.FileName );

                IDeserializer deserializer = new DeserializerBuilder().Build();
                string yaml;
                using ( StreamReader streamReader = new StreamReader( fileInfo.OpenRead() ) )
                {
                    yaml = streamReader.ReadToEnd();
                }
                EverestMetadata[] data = null;
                try
                {
                    data = deserializer.Deserialize<EverestMetadata[]>( yaml );
                }
                catch ( Exception e )
                {

                }

                if ( data != null && data.Length > 0 )
                {
                    Project = new CelesteModProject( data[ 0 ] );
                    HasUnsavedChanges = false;
                }
                else
                {
                    MessageBox.Show( "Could not read yaml file. Check if it isn't malformed!", "Error" );
                    return;
                }

                ImportProjectInfoFromFolder( fileInfo.Directory );
            }
        }

        private void ImportProjectInfoFromFolder( DirectoryInfo directory )
        {
            List<DirectoryInfo> subDirs = new List<DirectoryInfo>( directory.GetDirectories() );

            DirectoryInfo mapsDirectory = subDirs.Find( ( info ) => info.Name == "Maps" );
            if ( mapsDirectory == null )
            {
                if ( !Project.IsCodeMod )
                {
                    MessageBox.Show( "No Maps folder was found beside the selected everest.yaml", "Info" );
                }
                return;
            }

            if ( mapsDirectory.GetDirectories().Length == 0 )
            {
                MessageBox.Show( "Maps folder has no sub folders. Binary file collection aborted.", "Warning" );
                return;
            }
            Project.UserName = mapsDirectory.GetDirectories()[ 0 ].Name;

            if ( mapsDirectory.GetDirectories()[ 0 ].GetDirectories().Length == 0 )
            {
                MessageBox.Show( $"Maps/{Project.UserName} folder has no sub folders. Binary file collection aborted.", "Warning" );
                return;
            }
            Project.ProjectName = mapsDirectory.GetDirectories()[ 0 ].GetDirectories()[ 0 ].Name;
            foreach ( FileInfo mapFile in mapsDirectory.GetDirectories()[ 0 ].GetDirectories()[ 0 ].GetFiles() )
            {
                if ( mapFile.Extension == ".bin" )
                {
                    Project.SelectedLevels.Add( new LevelData( mapFile.FullName ) );
                }
            }
        }

        private void ExportEnglishTxt( string fileName )
        {
            string campaignRootID = $"{Project.UserName}_{Project.ProjectName}";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append( $"\n\t{campaignRootID}={Project.CampaignName}" );
            foreach ( LevelData level in Project.SelectedLevels )
            {
                string levelID = $"{campaignRootID}_{Path.GetFileNameWithoutExtension( level.FilePath ).Replace( '-', '_' ).Replace( ' ', '_' )}";
                stringBuilder.Append( $"\n\t{levelID}={level.LevelName}" );
                foreach ( Checkpoint checkpoint in level.Checkpoints )
                {
                    stringBuilder.Append( $"\n\t{levelID}_{checkpoint.RoomName.Replace( '-', '_' ).Replace( ' ', '_' )}={checkpoint.CheckpointName}" );
                }
                stringBuilder.Append( $"\n\tPOEM_{levelID}_{level.Side}={level.Poem}" );
                stringBuilder.Append( '\n' );
            }

            FileInfo file = new FileInfo( fileName );
            if ( file.Exists )
            {
                file.Delete();
            }
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }

            using ( FileStream stream = file.OpenWrite() )
            {
                byte[] data = new UTF8Encoding( true ).GetBytes( stringBuilder.ToString() );
                stream.Write( data );
            }
        }

        private void ExportEnglishTxt_Execute()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = $"Text Document *.txt|*.txt";
            if ( saveFileDialog.ShowDialog() == true )
            {
                ExportEnglishTxt( saveFileDialog.FileName );
            }
        }

        private void SelectLevelPreviewImage_Execute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Portable Network Graphics *.png|*.png"
            };
            if ( openFileDialog.ShowDialog() == true )
            {
                SelectedMap.PreviewImagePath = openFileDialog.FileName;
            }
        }

        private void ImportDependency_Execute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Everest YAML everest.yaml|everest.yaml"
            };
            if (openFileDialog.ShowDialog() == true )
            {
                FileInfo fileInfo = new FileInfo( openFileDialog.FileName );

                IDeserializer deserializer = new DeserializerBuilder().Build();
                string yaml;
                using ( StreamReader streamReader = new StreamReader( fileInfo.OpenRead() ) )
                {
                    yaml = streamReader.ReadToEnd();
                }
                EverestMetadata[] data = null;
                try
                {
                    data = deserializer.Deserialize<EverestMetadata[]>( yaml );
                }
                catch ( Exception e )
                {

                }

                foreach ( EverestMetadata metadata in data )
                {
                    Project.AddDependency( new ProjectMetadata( metadata.Name, metadata.Version ) );
                }
            }
        }
    }
}
