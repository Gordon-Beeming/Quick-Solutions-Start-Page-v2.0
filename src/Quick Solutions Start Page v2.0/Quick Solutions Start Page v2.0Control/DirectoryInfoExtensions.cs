using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;

namespace Quick_Solutions_Start_Page_v2_0Control
{
    public static class DirectoryInfoExtensions
    {
        //public delegate void FileFoundDelegate(FileInfo fi);
        //public static void GetFiles(this DirectoryInfo di, string searchPattern, FileFoundDelegate callback,string groupName)
        //{
        //    BackgroundWorker bw = new BackgroundWorker();
        //    bw.DoWork += (sv, ev) =>
        //    {
        //        GetFiles(di, searchPattern, callback, groupName, true);
        //    };
        //    bw.RunWorkerAsync();
        //}
        //private static void GetFiles(this DirectoryInfo di, string searchPattern, FileFoundDelegate callback,string groupName, bool rootLevel = false)
        //{
        //    if (rootLevel)
        //    {
        //        BackgroundWorker bwFiles = new BackgroundWorker();
        //        bwFiles.DoWork += (sv, ev) =>
        //        {
        //            FindFiles(di, searchPattern, callback, groupName);
        //        };
        //        bwFiles.RunWorkerAsync();
        //    }
        //    else
        //    {
        //        FindFiles(di, searchPattern, callback, groupName);
        //    }
        //    foreach (DirectoryInfo d in di.GetDirectories())
        //    {
        //        if (rootLevel)
        //        {
        //            BackgroundWorker bwFiles = new BackgroundWorker();
        //            bwFiles.DoWork += (sv, ev) =>
        //            {
        //                d.GetFiles(searchPattern, callback, groupName, false);
        //            };
        //            bwFiles.RunWorkerAsync();
        //        }
        //        else
        //        {
        //            d.GetFiles(searchPattern, callback, groupName, false);
        //        }
        //    }
        //}
        //private static void FindFiles(DirectoryInfo di, string searchPattern, FileFoundDelegate callback, string groupName)
        //{
        //    foreach (FileInfo fi in searchPattern == null ? di.GetFiles() : di.GetFiles(searchPattern, SearchOption.AllDirectories))
        //    {
        //        DataManager.AddToCache(fi, groupName);
        //        callback(fi);
        //    }
        //}
        public delegate void FileFoundDelegate(FileInfo fi);

        public static void GetFiles(this DirectoryInfo di, string searchPattern, FileFoundDelegate callback, string groupName)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (sv, ev) =>
            {
                //                ProcessStartInfo processInfo;
                //                Process process = new Process();
                //                processInfo = new ProcessStartInfo("cmd.exe");//, "/c dir " + di.FullName.TrimEnd('\\') + "\\*.sln /S /B");
                //                processInfo.CreateNoWindow = false;
                //                processInfo.UseShellExecute = false;
                //                processInfo.RedirectStandardOutput = true;
                //                processInfo.RedirectStandardError = true;
                //                processInfo.RedirectStandardInput = true;
                //                process.StartInfo = processInfo;
                //                process.Start();
                ////                string data = process.StandardOutput.ReadToEnd();
                //                process.EnableRaisingEvents = true;
                //                process.BeginOutputReadLine();
                //                process.OutputDataReceived += (svv, evv) =>
                //                {
                //                };
                //                process.StandardInput.WriteLine("dir " + di.FullName.TrimEnd('\\') + "\\*.sln /S /B");
                //                process.WaitForExit();
                //                process.Close();
                ProcessStartInfo startInfo = new ProcessStartInfo(@"cmd.exe");
                startInfo.CreateNoWindow = true;
                startInfo.ErrorDialog = false;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                Read(process.StandardOutput, callback, groupName);
                //Read(process.StandardError);
                process.StandardInput.WriteLine("dir " + di.FullName.TrimEnd('\\') + "\\*.sln /S /B");
                process.WaitForExit();
                string e = string.Empty;
            };
            bw.RunWorkerAsync();
        }

        private static void Read(StreamReader reader, FileFoundDelegate callback, string groupName)
        {
            new System.Threading.Thread(() =>
            {
                StringBuilder currentString = new StringBuilder();
                while (true)
                {
                    int current;
                    while ((current = reader.Read()) >= 0)
                    {
                        currentString.Append((char)current);
                        //Console.Write((char)current);
                        if ((char)current == '\n')
                        {
                            if (File.Exists(currentString.ToString().Trim()))
                            {
                                FileInfo fi = new FileInfo(currentString.ToString().Trim());
                                DataManager.AddToCache(fi, groupName);
                                callback(fi);
                            }
                            currentString = new StringBuilder();
                        }
                    }
                }
            }).Start();
        }
        //public static int ExecuteCommand(string Command, int Timeout)
        //{
        //    int ExitCode;
        //    ProcessStartInfo ProcessInfo;
        //    Process Process;
        //    ProcessInfo = new ProcessStartInfo("cmd.exe", "/C " + Command);
        //    ProcessInfo.CreateNoWindow = true;
        //    ProcessInfo.UseShellExecute = false;
        //    Process = Process.Start(ProcessInfo);
        //    Process.WaitForExit(Timeout);
        //    ExitCode = Process.ExitCode;
        //    Process.Close();
        //    return ExitCode;
        //}
    }
}