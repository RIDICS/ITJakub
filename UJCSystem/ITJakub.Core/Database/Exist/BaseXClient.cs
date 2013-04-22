/*
 * Language Binding for BaseX.
 * Works with BaseX 7.0 and later
 *
 * Documentation: http://docs.basex.org/wiki/Clients
 *
 * (C) BaseX Team 2005-12, BSD License
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace ITJakub.Core.Database.Exist
{
    public class Session
    {
        public readonly NetworkStream Stream;
        private readonly byte[] m_cache = new byte[4096];
        private readonly string m_ehost;
        private readonly TcpClient m_socket;
        private int m_bpos;
        private int m_bsize;
        private Dictionary<string, EventNotification> m_en;
        private TcpClient m_esocket;
        private NetworkStream m_estream;
        private string m_info = String.Empty;

        /** see readme.txt */

        public Session(string host, int port, string username, string pw)
        {
            m_socket = new TcpClient(host, port);
            Stream = m_socket.GetStream();
            m_ehost = host;
            string ts = Receive();
            Send(username);
            Send(MD5(MD5(pw) + ts));
            if (Stream.ReadByte() != 0)
            {
                throw new IOException("Access denied.");
            }
        }

        public string Info
        {
            get { return m_info; }
        }

        /** see readme.txt */

        public void Execute(string com, Stream ms)
        {
            Send(com);
            Init();
            Receive(ms);
            m_info = Receive();
            if (!Ok())
            {
                throw new IOException(m_info);
            }
        }

        /** see readme.txt */

        public String Execute(string com)
        {
            var ms = new MemoryStream();
            Execute(com, ms);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        /** see readme.txt */

        public Query Query(string q)
        {
            return new Query(this, q);
        }

        /** see readme.txt */

        public void Create(string name, Stream s)
        {
            Stream.WriteByte(8);
            Send(name);
            Send(s);
        }

        /** see readme.txt */

        public void Add(string path, Stream s)
        {
            Stream.WriteByte(9);
            Send(path);
            Send(s);
        }

        /** see readme.txt */

        public void Replace(string path, Stream s)
        {
            Stream.WriteByte(12);
            Send(path);
            Send(s);
        }

        /** see readme.txt */

        public void Store(string path, Stream s)
        {
            Stream.WriteByte(13);
            Send(path);
            Send(s);
        }

        /* Watches an event. */

        public void Watch(string name, EventNotification notify)
        {
            Stream.WriteByte(10);
            if (m_esocket == null)
            {
                int eport = Convert.ToInt32(Receive());
                m_en = new Dictionary<string, EventNotification>();
                m_esocket = new TcpClient(m_ehost, eport);
                m_estream = m_esocket.GetStream();
                string id = Receive();
                byte[] msg = Encoding.UTF8.GetBytes(id);
                m_estream.Write(msg, 0, msg.Length);
                m_estream.WriteByte(0);
                m_estream.ReadByte();
                new Thread(Listen).Start();
            }
            Send(name);
            m_info = Receive();
            if (!Ok())
            {
                throw new IOException(m_info);
            }
            m_en.Add(name, notify);
        }

        /** Listens to event socket */

        private void Listen()
        {
            while (true)
            {
                String name = readS();
                String val = readS();
                m_en[name].Update(val);
            }
        }

        /** Returns event message */

        private string readS()
        {
            var ms = new MemoryStream();
            while (true)
            {
                int b = m_estream.ReadByte();
                if (b == 0) break;
                ms.WriteByte((byte) b);
            }
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        /* Unwatches an event. */

        public void Unwatch(string name)
        {
            Stream.WriteByte(11);
            Send(name);
            m_info = Receive();
            if (!Ok())
            {
                throw new IOException(m_info);
            }
            m_en.Remove(name);
        }

        /** see readme.txt */

        /** see readme.txt */

        public void Close()
        {
            Send("exit");
            if (m_esocket != null)
            {
                m_esocket.Close();
            }
            m_socket.Close();
        }

        /** Initializes the byte transfer. */

        private void Init()
        {
            m_bpos = 0;
            m_bsize = 0;
        }

        /** Returns a single byte from the socket. */

        public byte Read()
        {
            if (m_bpos == m_bsize)
            {
                m_bsize = Stream.Read(m_cache, 0, 4096);
                m_bpos = 0;
            }
            return m_cache[m_bpos++];
        }

        /** Receives a string from the socket. */

        private void Receive(Stream ms)
        {
            while (true)
            {
                byte b = Read();
                if (b == 0) break;
                ms.WriteByte(b);
            }
        }

        /** Receives a string from the socket. */

        public string Receive()
        {
            var ms = new MemoryStream();
            Receive(ms);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        /** d1x added - ASCII version */

        public string ReceiveASCII()
        {
            var ms = new MemoryStream();
            Receive(ms);
            return Encoding.ASCII.GetString(ms.ToArray());
        }

        /** Sends strings to server. */

        public void Send(string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);
            Stream.Write(msg, 0, msg.Length);
            Stream.WriteByte(0);
        }

        /** see readme.txt */

        private void Send(Stream s)
        {
            while (true)
            {
                int t = s.ReadByte();
                if (t == -1) break;
                if (t == 0x00 || t == 0xFF) Stream.WriteByte(Convert.ToByte(0xFF));
                Stream.WriteByte(Convert.ToByte(t));
            }
            Stream.WriteByte(0);
            m_info = Receive();
            if (!Ok())
            {
                throw new IOException(m_info);
            }
        }


        /** Returns success check. */

        public bool Ok()
        {
            return Read() == 0;
        }

        /** Returns the md5 hash of a string. */

        private string MD5(string input)
        {
            var MD5 = new MD5CryptoServiceProvider();
            byte[] hash = MD5.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sb = new StringBuilder();
            foreach (byte h in hash)
            {
                sb.Append(h.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    public class Query
    {
        private readonly string id;
        private readonly Session session;
        private ArrayList cache;
        private int pos;

        /** see readme.txt */

        public Query(Session s, string query)
        {
            pos = 0;
            session = s;
            id = Exec(0, query);
        }

        /** see readme.txt */

        public void Bind(string name, string value)
        {
            Bind(name, value, "");
        }

        /** see readme.txt */

        public void Bind(string name, string value, string type)
        {
            Exec(3, id + '\0' + name + '\0' + value + '\0' + type);
        }

        /** see readme.txt */

        public void Context(string value)
        {
            Context(value, "");
        }

        /** see readme.txt */

        public void Context(string value, string type)
        {
            Exec(14, id + '\0' + value + '\0' + type);
        }

        /** see readme.txt */

        public bool More()
        {
            if (cache == null)
            {
                session.Stream.WriteByte(4);
                session.Send(id);
                cache = new ArrayList();
                while (session.Read() > 0)
                {
                    cache.Add(session.Receive());
                }
                if (!session.Ok())
                {
                    throw new IOException(session.Receive());
                }
            }
            return pos < cache.Count;
        }

        /** see readme.txt */

        public string Next()
        {
            if (More())
            {
                return cache[pos++] as string;
            }
            else
            {
                return null;
            }
        }

        /** see readme.txt */

        public string Execute()
        {
            return Exec(5, id);
        }

        /** see readme.txt */

        public string Info()
        {
            return Exec(6, id);
        }

        /** see readme.txt */

        public string Options()
        {
            return Exec(7, id);
        }

        /** see readme.txt */

        public void Close()
        {
            Exec(2, id);
        }

        /** see readme.txt */

        private string Exec(byte cmd, string arg)
        {
            session.Stream.WriteByte(cmd);
            session.Send(arg);
            string s = session.Receive();
            bool ok = session.Ok();
            if (!ok)
            {
                throw new IOException(session.Receive());
            }
            return s;
        }
    }

    public interface EventNotification
    {
        void Update(string data);
    }
}