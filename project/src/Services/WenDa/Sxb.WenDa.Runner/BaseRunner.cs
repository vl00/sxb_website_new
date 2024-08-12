using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Runner
{
    public abstract class BaseRunner<T>
    {
        public string ClassName => typeof(T).Name;

        public void Run()
        {
            WriteLine($"{ClassName} Start!");

            Running();

            WriteLine($"{ClassName} End!");
        }

        protected abstract void Running();

        public static void WriteLine<T>(T t)
        {
            var msg = JsonConvert.SerializeObject(t);
            WriteLine(msg);
        }

        public static void WriteLine<T>(IEnumerable<T> data)
        {
            //var msg = JsonConvert.SerializeObject(data);
            //WriteLine(msg)
            foreach (var item in data)
            {
                WriteLine(JsonConvert.SerializeObject(item));
            };
        }

        public static void WriteLine(string msg, params object?[]? arg)
        {
            //Debug.WriteLine(msg);
            //Trace.WriteLine(msg);
            Console.WriteLine(msg, arg);
        }

        public void WriteToFile<T>(string filename, IEnumerable<T> items)
        {
            using StreamWriter inputStream = File.CreateText(filename);
            //var bom = Encoding.Default.GetBytes($"\uFEFF");
            //inputStream.Write(bom);

            foreach (var t in items)
            {
                inputStream.WriteLine(JsonConvert.SerializeObject(t));
            }

            inputStream.Flush();
            inputStream.Close();

            WriteLine("写入文件完成, filename={0}", filename);
        }

        public IEnumerable<T> LoadFromFile<T>(string filename)
        {
            if (File.Exists(filename))
            {
                using var reader = File.OpenText(filename);

                string line;
                //前后顺序不能变, 读完最后一行, EndOfStream=true
                while (!reader.EndOfStream && (line = reader.ReadLine()) != null)
                {
                    yield return JsonConvert.DeserializeObject<T>(line);
                }
            }
        }

        public void WriteToFile(string filename, string content)
        {
            using StreamWriter inputStream = File.CreateText(filename);

            inputStream.Write(content);

            inputStream.Flush();
            inputStream.Close();

            WriteLine("写入文件完成, filename={0}", filename);
        }
    }
}
