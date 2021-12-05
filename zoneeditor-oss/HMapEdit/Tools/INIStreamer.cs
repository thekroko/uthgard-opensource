using System.Collections;
using System.IO;

/// <summary>
/// INI Streamer
/// Supports Reading and Writing of INI files
/// Simple: Use GetItem/SetItem
/// Advanced: Use Topic, GetTopic, Header, ..
/// ©2006 Metty
/// </summary>
public class INIStreamer {
    #region Header

    private readonly ArrayList m_Header = new ArrayList();

    /// <summary>
    /// Header List
    /// Contains Comments, which stand at the beginning of the file
    /// </summary>
    public ArrayList Header {
        get { return m_Header; }
    }

    #endregion

    #region Topics

    private readonly SortedList m_Topics = new SortedList();

    /// <summary>
    /// Contains all topics of the ini file
    /// </summary>
    public SortedList Topics {
        get { return m_Topics; }
    }

    /// <summary>
    /// Represents a Topic object
    /// </summary>
    public class Topic {
        private readonly SortedList m_Items;
        private readonly string m_Name;

        /// <summary>
        /// Creates a new Topic
        /// </summary>
        /// <param name="name"></param>
        public Topic(string name) {
            m_Name = name;
            m_Items = new SortedList();
        }

        /// <summary>
        /// Name of the Topic
        /// </summary>
        public string Name {
            get { return m_Name; }
        }

        /// <summary>
        /// Items in the topic
        /// </summary>
        public SortedList Items {
            get { return m_Items; }
        }
    }

    #endregion

    #region FileName

    private string m_File;

    /// <summary>
    /// Filename
    /// </summary>
    public string File {
        get { return m_File; }
        set { m_File = value; }
    }

    #endregion

    #region Write/Read Ini

    /// <summary>
    /// Rewrites the INI file
    /// </summary>
    /// <returns>Success</returns>
    public bool WriteIni() {
        try {
            StreamWriter writer = new StreamWriter(m_File, false);
            writer.AutoFlush = true;

            foreach (string l in Header) writer.WriteLine("; " + l);

            foreach (DictionaryEntry topic in Topics) {
                writer.WriteLine("");

                Topic tpc = (Topic) topic.Value;

                writer.WriteLine(string.Format("[{0}]", tpc.Name));

                foreach (DictionaryEntry entry in tpc.Items)
                    writer.WriteLine(string.Format("{0}={1}", entry.Key, entry.Value));
            }
            writer.Close();
            writer.Dispose();
        }
        catch {
            return false;
        }

        return true;
    }

    /// <summary>
    /// (Re)Reads the INI File
    /// </summary>
    /// <returns></returns>
    public bool ReadIni() {
        string file = m_File;
        if (System.IO.File.Exists(file)) {
            StreamReader reader = new StreamReader(file);

            if (reader == null) return false;

            Header.Clear();
            m_Topics.Clear();

            //Lets read it!
            Topic lasttopic = null;
            string line;

            char[] topicsep = {'[', ']'};
            char[] itemsep = {'='};

            while ((line = reader.ReadLine()) != null) {
                //Ignore comments
                if (line.StartsWith(";")) {
                    if (lasttopic == null) if (line.Length >= 2) Header.Add(line.Remove(0, 2));
                    continue;
                }
                if (line.StartsWith("#")) continue;
                //Ignore emptyl ines
                if (line == "") continue;

                if (line.StartsWith("[")) // Its a topic?
                {
                    string[] split = line.Split(topicsep, 3); //3 - before [, between, and after ]

                    //As there is a new topic, change the topic element!
                    lasttopic = new Topic(split[1]);

                    try {
                        m_Topics.Add(lasttopic.Name, lasttopic);
                    }
                    catch {
                        lasttopic = (Topic) m_Topics[lasttopic.Name];
                    } //duplicate entry?
                }
                else //Then its a item!
                {
                    string[] split = line.Split(itemsep, 2);

                    if (split.Length < 2) //error!
                        continue;

                    try {
                        lasttopic.Items.Add(split[0], split[1]);
                    }
                    catch {} //duplicate entry?
                }
            }
            reader.Close();
            reader.Dispose();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Clears the current memory Ini file
    /// </summary>
    public void ClearIni() {
        Header.Clear();
        m_Topics.Clear();
    }

    #endregion

    #region Get/Set/Rem

    #region GetTopic(s)

    /// <summary>
    /// Gets all Topics in the INI
    /// </summary>
    /// <returns>List of Topic Objects</returns>
    public ICollection GetTopics() {
        return Topics.Values;
    }

    /// <summary>
    /// Gets the specified Topic and creates it if neccessary and specified
    /// </summary>
    /// <param name="name">The Topic</param>
    /// <returns>Topic Object/Null</returns>
    public Topic GetTopic(string name) {
        if (Topics[name] == null) return null;

        return (Topic) Topics[name];
    }

    #endregion

    #region GetItem(s)

    /// <summary>
    /// Gets all items of the specified topic
    /// </summary>
    /// <param name="topic">The Topic</param>
    /// <returns>List of Items</returns>
    public ICollection GetItems(string topic) {
        Topic tpc = GetTopic(topic);

        if (tpc == null) return new string[] {};

        return tpc.Items.Keys;
    }

    /// <summary>
    /// Gets the specified item or creates it if neccessary
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public string GetItem(string topic, string item) {
        Topic tpc = GetTopic(topic);

        if (tpc == null) return "";

        if (tpc.Items[item] == null) return "";

        return (string) tpc.Items[item];
    }

    #endregion

    #region AddTopic

    /// <summary>
    /// Adds a new topic to the INI file
    /// </summary>
    /// <param name="name">Topic Name</param>
    public Topic AddTopic(string name) {
        if (Topics[name] == null) Topics[name] = new Topic(name);

        return (Topic) Topics[name];
    }

    #endregion

    #region SetItem

    /// <summary>
    /// Adds an item to the specified topic in the INI file
    /// </summary>
    /// <param name="topic">Topic</param>
    /// <param name="item">Item</param>
    /// <param name="content">Content of the Item</param>
    public void SetItem(string topic, string item, string content) {
        Topic tpc = GetTopic(topic);

        if (tpc == null) tpc = AddTopic(topic);

        tpc.Items[item] = content;
    }

    #endregion

    #region RemTopic   

    /// <summary>
    /// Removes a topic from the INI file
    /// </summary>
    /// <param name="topic">Topic to remove</param>
    public void RemTopic(string topic) {
        if (Topics.Contains(topic)) Topics.Remove(topic);
    }

    #endregion

    #region RemItem

    /// <summary>
    /// Removes an Item from a topic
    /// </summary>
    /// <param name="topic">Topic</param>
    /// <param name="item">Item to remove</param>
    public void RemItem(string topic, string item) {
        Topic tpc = Topics[topic] as Topic;

        if (tpc == null) return;

        if (tpc.Items.Contains(item)) tpc.Items.Remove(item);
    }

    #endregion

    #endregion

    /// <summary>
    /// Initializes a new blank INI Streamer
    /// </summary>
    /// <param name="file"></param>
    public INIStreamer(string file) {
        m_File = file;

        Header.Clear();

        Header.Add("###########");
        Header.Add("Config File");
        Header.Add("Do not edit");
        Header.Add("###########");
    }
}