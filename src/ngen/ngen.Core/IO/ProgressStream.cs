#region Using directives

using System;
using System.ComponentModel;
using System.IO;

#endregion

namespace ngen.Core.IO
{
    // A modified version of the ProgressStream from http://blogs.msdn.com/b/paolos/archive/2010/05/25/large-message-transfer-with-wcf-adapters-part-1.aspx
    // This class allows progress changed events to be raised from the blob upload/download.
    public class ProgressStream : Stream
    {
        #region Public Constructor

        public ProgressStream(Stream file, string fileName = "n/a")
        {
            stream = file;
            _totalLength = file.Length;
            _bytesTransferred = 0;
            FileName = fileName;
        }

        #endregion

        #region Public Handler

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        #endregion

        #region Private Fields

        private readonly Stream stream;
        private int _bytesTransferred;
        private long _totalLength;

        #endregion

        #region Public Properties

        public string FileName { get; }

        public override bool CanRead
        {
            get { return stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return stream.CanWrite; }
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override void Close()
        {
            stream.Close();
        }

        public override long Length
        {
            get { return stream.Length; }
        }

        public override long Position
        {
            get { return stream.Position; }
            set { stream.Position = value; }
        }

        #endregion

        #region Public Methods

        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = stream.Read(buffer, offset, count);
            _bytesTransferred += result;
            if (ProgressChanged != null)
            {
                try
                {
                    var percentComplete = ((double)_bytesTransferred/_totalLength)*100;

                    OnProgressChanged(new ProgressChangedEventArgs(Convert.ToInt32(percentComplete), _totalLength));
                    //ProgressChanged(this, new ProgressChangedEventArgs(bytesTransferred, totalLength));
                }
                catch (Exception)
                {
                    ProgressChanged = null;
                    throw;
                }
            }
            return result;
        }

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _totalLength = value;
            //this.stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
            _bytesTransferred += count;
            {
                try
                {
                    OnProgressChanged(new ProgressChangedEventArgs(_bytesTransferred, _totalLength));
                    //ProgressChanged(this, new ProgressChangedEventArgs(bytesTransferred, totalLength));
                }
                catch (Exception)
                {
                    ProgressChanged = null;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            stream.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}