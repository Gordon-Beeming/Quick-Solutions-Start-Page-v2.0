using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace Quick_Solutions_Start_Page_v2_0Control
{
    public static class DataManager
    {
        static Guid _lastWriteRequestGuid = Guid.Empty;
        static string _thisXml = string.Empty;

        static string _localSettingsPath = null;
        static string _localSettingsFile = null;
        static string _localSettingsPathFile = null;

        public static MTObservableCollection<ListDataItem> DataSource { get; set; }

        //public static IOrderedEnumerable<FileInfo> DataSourceOrdered
        //{
        //    get
        //    {
        //        return DataSource.OrderBy(o => o.Name);
        //    }
        //}

        public static void Load()
        {
            DataSource = new MTObservableCollection<ListDataItem>();
            _localSettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Quick Solutions Start Page v2.0\\";
            if (!Directory.Exists(_localSettingsPath))
                Directory.CreateDirectory(_localSettingsPath);
            _localSettingsFile = _localSettingsPath + "cache.xml";

            if (File.Exists(_localSettingsFile))
            {
                _thisXml = File.ReadAllText(_localSettingsFile);
                if (string.IsNullOrEmpty(_thisXml))
                {
                    Initialise();
                    Save();
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_thisXml);
                    List<string> pathsAdded = new List<string>();
                    foreach (XmlNode path in doc.SelectNodes("//root/paths/path"))
                    {
                        if (File.Exists(path.Attributes["fullName"].Value))
                        {
                            if (!pathsAdded.Contains(path.Attributes["fullName"].Value.ToLower()))
                            {
                                try
                                {
                                    FileInfo fi = new FileInfo(path.Attributes["fullName"].Value);
                                    DataSource.Add(new ListDataItem { FullName = fi.FullName, GroupName = path.Attributes["groupName"] == null ? string.Empty : path.Attributes["groupName"].Value, Name = fi.Name });
                                    pathsAdded.Add(path.Attributes["fullName"].Value.ToLower());
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            else
            {
                Initialise();
                Save();
            }
        }

        public static string ReadTextFromFile(string fullFilePath)
        {
            string result = string.Empty;
            if (File.Exists(fullFilePath))
            {
                FileStream fs = null;
                StreamReader sr = null;
                try
                {
                    fs = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    sr = new StreamReader(fs);
                    result = sr.ReadToEnd();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Close();
                        sr.Dispose();
                        sr = null;
                    }
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                        fs = null;
                    }
                }
            }
            return result;
        }

        private static void Initialise()
        {
            if (!Directory.Exists(_localSettingsPath))
            {
                Directory.CreateDirectory(_localSettingsPath);
            }
            _thisXml = @"<root mode='local'><paths></paths></root>";
        }

        private static bool PathExists(string pathToCheck)
        {
            bool result = false;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(_thisXml);
            foreach (XmlNode path in doc.SelectNodes("//root/paths/path"))
            {
                if (string.Compare(path.Attributes["fullName"].Value, pathToCheck, true) == 0)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static void Save()
        {
            SaveTextToFile(_localSettingsFile, _thisXml);
        }

        public static void SaveTextToFile(string fullFilePath, string content)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                sw = new StreamWriter(fs);
                sw.Write(content);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                    sw = null;
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
            }
        }

        public static void AddToCache(FileInfo fi, string groupName)
        {
            if (fi != null && !string.IsNullOrEmpty(fi.Name))
            {
                if (!PathExists(fi.FullName))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_thisXml);
                    XmlNode paths = doc.SelectSingleNode("//root/paths");
                    XmlNode xn = doc.CreateElement("path");
                    XmlAttribute fullName = doc.CreateAttribute("fullName");
                    fullName.Value = fi.FullName;
                    xn.Attributes.Append(fullName);
                    XmlAttribute groupNameAttri = doc.CreateAttribute("groupName");
                    groupNameAttri.Value = groupName;
                    xn.Attributes.Append(groupNameAttri);
                    paths.AppendChild(xn);
                    _thisXml = doc.InnerXml;

                    try
                    {
                        DataSource.Add(new ListDataItem { FullName = fi.FullName, GroupName = groupName, Name = fi.Name });
                    }
                    catch { }
                }
            }
        }

        public static string LoadPathSettings()
        {
            _localSettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Quick Solutions Start Page v2.0\\";
            _localSettingsPathFile = _localSettingsPath + "settings.txt";
            return ReadTextFromFile(_localSettingsPathFile);
        }

        public delegate void SettingsSavedDelegate();

        public static event SettingsSavedDelegate SettingsSaved;

        public static void SavePathSettings(string data)
        {
            _localSettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Quick Solutions Start Page v2.0\\";
            _localSettingsPathFile = _localSettingsPath + "settings.txt";
            SaveTextToFile(_localSettingsPathFile, data);
            if (SettingsSaved!= null)
            {
                SettingsSaved();
            }
        }

        public static List<string> ScanPaths
        {
            get
            {
                _localSettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Quick Solutions Start Page v2.0\\";
                _localSettingsPathFile = _localSettingsPath + "settings.txt";
                List<string> result = new List<string>();

                string data = ReadTextFromFile(_localSettingsPathFile);
                if (!string.IsNullOrEmpty(data))
                {
                    foreach (string line in data.Split('\r'))
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            result.Add(line.Trim());
                        }
                    }
                }
                return result;
            }
        }
    }

    public class ListDataItem
    {
        public string FullName { get; set; }

        public string Name { get; set; }

        public string GroupName { get; set; }
    }

    public class MTObservableCollection<T> : ObservableCollection<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            if (CollectionChanged != null)
                foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
                {
                    DispatcherObject dispObj = nh.Target as DispatcherObject;
                    if (dispObj != null)
                    {
                        Dispatcher dispatcher = dispObj.Dispatcher;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            try
                            {
                                dispatcher.BeginInvoke(
                                    (Action)(() => nh.Invoke(this,
                                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                                    DispatcherPriority.DataBind);
                            }
                            catch { }
                            continue;
                        }
                    }
                    nh.Invoke(this, e);
                }
        }
    }
}
