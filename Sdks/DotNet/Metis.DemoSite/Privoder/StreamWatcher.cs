using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Metis.DemoSite
{
    internal class StreamWatcher : Stream
    {
        private Stream mBase;
        private MemoryStream mMemoryStream = new MemoryStream();

        public StreamWatcher(Stream stream)
        {
            this.mBase = stream;
        }

        public override void Flush()
        {
            this.mBase.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.mBase.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            mMemoryStream.Write(buffer, offset, count);
            mBase.Write(buffer, offset, count);
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(mMemoryStream.ToArray());
        }

        public byte[] ToBytes()
        {
            return mMemoryStream.ToArray();
        }

        public override long Length
        {
            get { return mMemoryStream.Length; }
        }

        #region Rest of the overrides
        public override bool CanRead
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { throw new NotImplementedException(); }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

    }
}
