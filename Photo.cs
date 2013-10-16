using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PhotoSort
{
    /// <summary>
    /// This class describes a single photo - its location, the image and 
    /// the metadata extracted from the image.
    /// </summary>
    public class Photo
    {

        public bool Print { get; set; }

        public Photo(string path)
        {
            _path = path;
            _source = new Uri(path);
            //   _image = BitmapFrame.Create(_source);

            //using (var stream = File.OpenRead(path))
            //{
            //    var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            //    _image = decoder.Frames.FirstOrDefault();
            //}

        }

        public override string ToString()
        {
            return _source.ToString();
        }

        private string _path;

        private Uri _source;
        public string Source { get { return _path; } }

        private BitmapFrame _image;
        public BitmapFrame Image
        {
            get
            {
                if (_image == null)
                {
                    using (var stream = File.OpenRead(_path))
                    {
                        var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        _image = decoder.Frames.FirstOrDefault();
                    }
                }
                return _image;
            }
            set { _image = value; }
        }



    }

    /// <summary>
    /// This class represents a collection of photos in a directory.
    /// </summary>
    public class PhotoCollection : ObservableCollection<Photo>
    {
        DirectoryInfo _directory;
        DirectoryInfo diPrint;
        DirectoryInfo diTrash;

        public PhotoCollection() { }

        public PhotoCollection(string path) : this(new DirectoryInfo(path)) { }

        public PhotoCollection(DirectoryInfo directory)
        {
            _directory = directory;
            Update();
        }

        public string Path
        {
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    _directory = new DirectoryInfo(value);
                    Update();
                }

            }
            get { return _directory.FullName; }
        }

        public DirectoryInfo Directory
        {
            set
            {
                _directory = value;
                Update();
            }
            get { return _directory; }
        }

        private void Update()
        {
            diPrint = new DirectoryInfo(System.IO.Path.Combine(_directory.FullName, "Print"));
            diTrash = new DirectoryInfo(System.IO.Path.Combine(_directory.FullName, "Trash"));

            this.Clear();
            try
            {
                foreach (FileInfo f in _directory.GetFiles("*.jpg"))
                {
                    var photo = new Photo(f.FullName);
                    Add(photo);

                    if (File.Exists(System.IO.Path.Combine(diPrint.FullName, f.Name)))
                        photo.Print = true;
                }
            }
            catch (DirectoryNotFoundException)
            {
                System.Windows.MessageBox.Show("No Such Directory");
            }
        }

        public void Delete(int index)
        {
            if (index < 0 || index > this.Count)
                return;

            if (!diTrash.Exists)
                diTrash.Create();

            Photo photoToDelete = this[index];
            if (photoToDelete != null)
            {

                this.Remove(photoToDelete);

                photoToDelete.Image = null;
                FileInfo fi = new FileInfo(photoToDelete.Source);
                Task.Run(() =>
                {

                    var dest = System.IO.Path.Combine(diTrash.FullName, fi.Name);
                    File.Move(fi.FullName, dest);
                });
            }
        }


        internal void Print(int index)
        {
            if (index < 0 || index > this.Count)
                return;

            if (!diPrint.Exists)
                diPrint.Create();


            Photo photoToPrint = this[index];
            if (photoToPrint != null)
            {
                if (photoToPrint.Print)
                    return;

                photoToPrint.Print = true;
                FileInfo fi = new FileInfo(photoToPrint.Source);
                Task.Run(() =>
                {

                    var dest = System.IO.Path.Combine(diPrint.FullName, fi.Name);
                    File.Copy(fi.FullName, dest);
                });
            }
        }
    }
}
