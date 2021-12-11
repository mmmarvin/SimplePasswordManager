using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SimplePasswordManager
{
    public class UsernamePassword
    {
        public string Label { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class PasswordManager : Subject
    {
        class PasswordFileHeader
        {
            private static readonly char[] HEADER_VALUE = { (char)0x01, (char)0x02, (char)0x03, (char)0x04 };

            public char[] Header { get; private set; } = new char[4];

            public static void Write(StreamWriter writer)
            {
                for(int i = 0; i < 4; ++i)
                {
                    writer.BaseStream.WriteByte(Convert.ToByte(HEADER_VALUE[i]));
                }
            }

            public bool Verify()
            {
                for(int i = 0; i < 4; ++i)
                {
                    if(Header[i] != HEADER_VALUE[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private static readonly char[] DATA_HEADER_VALUE = { (char)0x11, (char)0x12, (char)0x13, (char)0x14 };
        private static readonly byte[] SALT = { 0x2c, 0x00, 0x9c, 0xba, 0x2d, 0xe5, 0x44, 0x70, 0x89, 0xfe, 0x90, 0xa3, 0xb1, 0xcc, 0x32, 0xca };

        public enum ELoadResult
        {
            Success,
            FileNotSet,
            FileDoesNotExist,
            InvalidFile,
            InvalidPassword,
            NeedPassword,
            ReadError
        }

        public PasswordManager(string filename)
        {
            m_filename = filename;
        }

        ~PasswordManager()
        {
            m_filename = null;
            m_usernamePasswords = null;
            foreach(var item in m_usernamePasswords)
            {
                item.Value.Clear();
            }
            m_usernamePasswords.Clear();
        }

        public bool Save()
        {
            if(m_filename == null || m_filename.Length == 0)
            {
                return false;
            }

            using(var file_writer = new StreamWriter(new FileStream(m_filename, FileMode.Create), Encoding.ASCII))
            {
                // write the header
                PasswordFileHeader.Write(file_writer);

                // write if we have a master password
                if(m_masterPassword != null)
                {
                    file_writer.BaseStream.WriteByte(Convert.ToByte(0x01));
                }
                else
                {
                    file_writer.BaseStream.WriteByte(Convert.ToByte(0x00));
                }

                using (var ms = new MemoryStream())
                {
                    // write passwords to memory stream
                    using (var sw = new StreamWriter(ms, Encoding.ASCII))
                    {
                        // write the headers
                        for (int i = 0; i < 4; ++i)
                        {
                            sw.BaseStream.WriteByte(Convert.ToByte(DATA_HEADER_VALUE[i]));
                        }

                        // write the json data
                        sw.WriteLine("[");
                        bool first_item1 = true;
                        foreach (var category_pair in m_usernamePasswords)
                        {
                            if (!first_item1)
                            {
                                sw.WriteLine(",");
                            }
                            else
                            {
                                first_item1 = false;
                            }

                            sw.WriteLine("{");
                            sw.WriteLine("\"category\": \"" + CheckForSpecialCharacters(category_pair.Key) + "\",");
                            sw.WriteLine("\"list\": [");

                            bool first_item2 = true;
                            foreach (var entry in category_pair.Value)
                            {
                                if (!first_item2)
                                {
                                    sw.WriteLine(",");
                                }
                                else
                                {
                                    first_item2 = false;
                                }

                                sw.WriteLine("{");
                                sw.WriteLine("\"label\": \"" + CheckForSpecialCharacters(entry.Label) + "\",");
                                sw.WriteLine("\"username\": \"" + CheckForSpecialCharacters(entry.Username) + "\",");
                                sw.WriteLine("\"password\": \"" + CheckForSpecialCharacters(entry.Password) + "\"");
                                sw.Write("}");
                            }
                            if (category_pair.Value.Count > 0)
                            {
                                sw.WriteLine("");
                            }
                            sw.WriteLine("]");
                            sw.Write("}");
                        }
                        if (m_usernamePasswords.Count > 0)
                        {
                            sw.WriteLine("");
                        }
                        sw.WriteLine("]");
                    }
                    
                    if(m_masterPassword != null)
                    {
                        using(var aes = Aes.Create())
                        {
                            aes.Key = m_masterPassword;

                            // write the IV
                            file_writer.BaseStream.Write(aes.IV, 0, aes.IV.Length);

                            // encrypt the data
                            byte[] encrypted_data = null;
                            try
                            {
                                encrypted_data = EncryptData(Encoding.ASCII.GetString(ms.ToArray()), aes.Key, aes.IV);
                            }
                            catch(Exception)
                            {
                                return false;
                            }

                            // write to the file
                            if(encrypted_data == null)
                            {
                                return false;
                            }
                            file_writer.BaseStream.Write(encrypted_data, 0, encrypted_data.Length);
                        }
                    }
                    else
                    {
                        var data_array = ms.ToArray();
                        file_writer.BaseStream.Write(data_array, 0, data_array.Length);
                    }
                }
            }

            return true;
        }

        public ELoadResult Load()
        {
            // primary checks
            if(m_filename == null || m_filename.Length == 0)
            {
                return ELoadResult.FileNotSet;
            }
            if(!File.Exists(m_filename))
            {
                return ELoadResult.FileDoesNotExist;
            }

            using (var reader = new StreamReader(new FileStream(m_filename, FileMode.Open), Encoding.ASCII))
            {
                // read the header
                var header = new PasswordFileHeader();
                header.Header[0] = Convert.ToChar(reader.BaseStream.ReadByte());
                header.Header[1] = Convert.ToChar(reader.BaseStream.ReadByte());
                header.Header[2] = Convert.ToChar(reader.BaseStream.ReadByte());
                header.Header[3] = Convert.ToChar(reader.BaseStream.ReadByte());
                if(!header.Verify())
                {
                    return ELoadResult.InvalidFile;
                }

                // read to check if the file is password protected
                var has_password = Convert.ToChar(reader.BaseStream.ReadByte()) == 0x01;
                if(has_password && m_masterPassword == null)
                {
                    return ELoadResult.NeedPassword;
                }

                byte[] iv = null;
                if(has_password)
                {
                    iv = new byte[16];
                    if(reader.BaseStream.Read(iv, 0, 16) != 16)
                    {
                        return ELoadResult.ReadError;
                    }
                }

                using (var ms = new MemoryStream())
                {
                    byte[] buffer = new byte[256];
                    var read = reader.BaseStream.Read(buffer, 0, 256);
                    while(read != 0)
                    {
                        ms.Write(buffer, 0, read);
                        read = reader.BaseStream.Read(buffer, 0, 256);
                    }

                    byte[] raw_data = ms.ToArray();
                    string data = null;
                    if(has_password)
                    {
                        try
                        {
                            data = DecryptData(raw_data, m_masterPassword, iv);
                            if(data == null)
                            {
                                return ELoadResult.ReadError;
                            }
                        }
                        catch(Exception)
                        {
                            return ELoadResult.ReadError;
                        }

                        // check that the headers match
                        for(int i = 0; i < 4; ++i)
                        {
                            if(data[i] != DATA_HEADER_VALUE[i])
                            {
                                return ELoadResult.InvalidPassword;
                            }
                        }
                        data = data.Substring(4);
                    }
                    else
                    {
                        data = Encoding.ASCII.GetString(raw_data, 4, raw_data.Length - 4);
                    }

                    try
                    {
                        var json_array = JArray.Parse(data);
                        foreach(var json_item in json_array)
                        {
                            if(json_item["category"] == null)
                            {
                                return ELoadResult.ReadError;
                            }

                            var new_list = new List<UsernamePassword>();
                            m_usernamePasswords.Add(json_item["category"].ToString(), new_list);
                            foreach(var json_password_item in json_item["list"])
                            {
                                if(json_password_item["label"] == null || json_password_item["username"] == null || json_password_item["password"] == null)
                                {
                                    return ELoadResult.ReadError;
                                }
                                new_list.Add(new UsernamePassword() { Label = json_password_item["label"].ToString(), Username = json_password_item["username"].ToString(), Password = json_password_item["password"].ToString() });
                            }
                        }
                    }
                    catch(Exception)
                    {
                        return ELoadResult.ReadError;
                    }
                }
            }
            
            NotifyObservers("Load", new LoadEvent() { UsernamePasswords = m_usernamePasswords });
            return ELoadResult.Success;
        }

        public bool SetMasterPassword(string master_password)
        {
            if(master_password == null || master_password.Length == 0 || master_password.Length > 1024)
            {
                return false;
            }

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(master_password, SALT, 100000);
            m_masterPassword = pbkdf2.GetBytes(16);

            return true;
        }

        public bool CreateNewCategory(string category)
        {
            if(!m_usernamePasswords.ContainsKey(category))
            {
                m_usernamePasswords.Add(category, new List<UsernamePassword>());

                Save();
                NotifyObservers("AddCategory", new CategoryEvent() { Category = category });

                return true;
            }

            return false;
        }

        public List<string> GetCategoryList()
        {
            var ret = new List<string>();
            foreach (var item in m_usernamePasswords)
            {
                ret.Add(item.Key);
            }

            return ret;
        }

        public List<UsernamePassword> GetUsernameAndPasswords(string category)
        {
            if(m_usernamePasswords.ContainsKey(category))
            {
                return m_usernamePasswords[category];
            }

            return null;
        }

        public bool RemoveCategory(string category)
        {
            var ret = m_usernamePasswords.Remove(category);

            Save();
            NotifyObservers("RemoveCategory", new CategoryEvent() { Category = category });

            return ret;
        }

        public bool AddEntry(string category, string label, string username, string password)
        {
            if(m_usernamePasswords.ContainsKey(category))
            {
                var entry = new UsernamePassword() { Label = label, Username = username, Password = password };
                m_usernamePasswords[category].Add(entry);

                Save();
                NotifyObservers("AddPassword", new PasswordEvent() { Category = category, UsernamePassword = entry });

                return true;
            }

            return false;
        }

        public bool UpdateEntry(string category, int index, string label, string username, string password)
        {
            if(m_usernamePasswords.ContainsKey(category) && m_usernamePasswords[category].Count > index)
            {
                m_usernamePasswords[category][index].Label = label;
                m_usernamePasswords[category][index].Username = username;
                m_usernamePasswords[category][index].Password = password;

                Save();
                NotifyObservers("UpdatePassword", new PasswordUpdateEvent() { Category = category, UsernamePassword = m_usernamePasswords[category][index], Index = index });

                return true;
            }

            return false;
        }

        public bool RemoveEntry(string category, int index)
        {
            if(m_usernamePasswords.ContainsKey(category) && m_usernamePasswords[category].Count > index)
            {
                var entry = m_usernamePasswords[category][index];
                m_usernamePasswords[category].RemoveAt(index);

                Save();
                NotifyObservers("RemovePassword", new PasswordUpdateEvent() { Category = category, UsernamePassword = entry, Index = index });

                return true;
            }

            return false;
        }

        public int Count
        {
            get
            {
                int ret = 0;
                foreach(var pair in m_usernamePasswords)
                {
                    ret += pair.Value.Count;
                }

                return ret;
            }
        }

        public bool IsPasswordSet
        {
            get
            {
                return m_masterPassword != null;
            }
        }

        static private byte[] EncryptData(string data, byte[] key, byte[] iv)
        {
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentException("Invalid data");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentException("Invalid key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentException("Invalid iv");
            }

            byte[] ret = null;
            using(var aes_alg = Aes.Create())
            {
                aes_alg.Key = key;
                aes_alg.IV = iv;
                aes_alg.Mode = CipherMode.CBC;
                aes_alg.Padding = PaddingMode.PKCS7;

                using(var ms = new MemoryStream())
                {
                    using(var cs = new CryptoStream(ms, aes_alg.CreateEncryptor(aes_alg.Key, aes_alg.IV), CryptoStreamMode.Write))
                    {
                        using(var sw = new StreamWriter(cs, Encoding.ASCII))
                        {
                            sw.Write(data);
                        }

                        ret = ms.ToArray();
                    }
                }
            }

            return ret;
        }

        static private string DecryptData(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentException("Invalid data");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentException("Invalid key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentException("Invalid iv");
            }

            string ret = null;
            using (var aes_alg = Aes.Create())
            {
                aes_alg.Key = key;
                aes_alg.IV = iv;
                aes_alg.Mode = CipherMode.CBC;
                aes_alg.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream(data))
                {
                    using (var cs = new CryptoStream(ms, aes_alg.CreateDecryptor(aes_alg.Key, aes_alg.IV), CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs, Encoding.ASCII))
                        {
                            ret = sr.ReadToEnd();
                        }
                    }
                }
            }

            return ret;
        }

        private string CheckForSpecialCharacters(string s)
        {
            string ret = s;
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms, Encoding.ASCII))
                {
                    foreach (var c in s)
                    {
                        switch (c)
                        {
                            case '\\':
                                sw.Write("\\\\");
                                break;
                            case '\'':
                                sw.Write("\\\'");
                                break;
                            case '\"':
                                sw.Write("\\\"");
                                break;
                            default:
                                sw.Write(c);
                                break;
                        }
                    }
                }

                ret = Encoding.ASCII.GetString(ms.ToArray());
            }

            return ret;
        }

        private string                                      m_filename = null;
        private byte[]                                      m_masterPassword = null;
        private Dictionary<string, List<UsernamePassword>>  m_usernamePasswords = new Dictionary<string, List<UsernamePassword>>();
    }
}
