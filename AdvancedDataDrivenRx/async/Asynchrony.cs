using System;
using System.Text;
using System.IO;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AdvancedDataDrivenRx
{
    public class Asynchrony
    {

        const string _FOLDERPATH = @"C:\Workarea\Seagate-4TB-USD-HDD-Backup\Anand\Workarea\SourceCode\AdvancedDataDrivenCSharp\AdvancedDataDrivenRx";

        public Asynchrony()
        {

        }

        public string[] IdentifyFilesFromDirectory()
        {
            string[] files = Directory.GetFiles(_FOLDERPATH + @"\TestFiles");
            return files;
        }

        public int DoFileReadLogic()
        {
            DateTime t1 = DateTime.Now;

            StringBuilder sb = new StringBuilder();

            string[] s = IdentifyFilesFromDirectory();

            foreach (var f in s)
            {
                using (FileStream fs = new FileStream(f, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        sb.AppendLine(sr.ReadToEnd());
                    }
                }
            }
            int timetaken = DateTime.Now.Subtract(t1).Milliseconds;
            Console.WriteLine(sb.ToString());
            Console.WriteLine(timetaken + " millisecs");

            return timetaken;
        }

        public async void DoFileReadLogicASync()
        {
            DateTime t1 = DateTime.Now;
            List<Task<string>> tasks;
            StringBuilder sb = new StringBuilder();

            string[] s = IdentifyFilesFromDirectory();
            tasks = new List<Task<string>>(s.Length);

            foreach (var f in s)
            {
                using (FileStream fs = new FileStream(f, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string str = await sr.ReadToEndAsync();
                        sb.AppendLine(str);
                    }
                }
            }

            int timetaken = DateTime.Now.Subtract(t1).Milliseconds;
            Console.WriteLine(sb.ToString());
            Console.WriteLine(timetaken + " millisecs");

        }
    }

    [TestFixture]
    public class AsynchronyTests
    {
        [Test]
        public void TestIdentifyFilesFromDirectory()
        {
            string[] s = new Asynchrony().IdentifyFilesFromDirectory();
            Assert.IsTrue(s != null && s.Length == 6);
        }

        [Test]
        public void TestDoFileReadLogic()
        {
            Assert.IsTrue(new Asynchrony().DoFileReadLogic() > 0);
        }

        [Test]
        public void TestDoFileReadAsyncLogic()
        {
            new Asynchrony().DoFileReadLogicASync();
            System.Threading.Thread.Sleep(5000);
            Assert.IsTrue(true);
        }
      
    }
}