using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Runner
{
    public class FileLog : IDisposable
    {
        private bool disposedValue;
        public string FileName { get; }
        StreamWriter InputStream { get; set; }

        public FileLog(string fileName)
        {
            FileName = fileName;

            var dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            InputStream = File.CreateText(FileName);
        }

        public void WriteLine<T>(IEnumerable<T> items)
        {
            foreach (var t in items)
            {
                InputStream.WriteLine(JsonConvert.SerializeObject(t));
            }
            Save();
        }

        public void WriteLine(string msg)
        {
            InputStream.WriteLine(msg);
            Save();
        }
        public void WriteLine(string sep, params object[] msgs)
        {
            foreach (var msg in msgs)
            {
                InputStream.WriteLine(msg);
                InputStream.WriteLine(sep);
            }
            Save();
        }

        public void WriteLine<T>(T t)
        {
            InputStream.WriteLine(JsonConvert.SerializeObject(t));
            Save();
        }

        public void WriteLineNow(string msg = "")
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                InputStream.WriteLine($"{DateTime.Now}-{msg}");
            }
            else
            {
                InputStream.WriteLine(DateTime.Now);
            }
            Save();
        }

        protected void Save()
        {
            InputStream.Flush();
        }

        protected virtual void Dispose(bool disposing)
        {
            Save();
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                InputStream?.Close();
                InputStream?.Dispose();
                // TODO: 将大型字段设置为 null
                InputStream = null;
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~FileLog()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
