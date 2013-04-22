using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

namespace ITJakub.Core.Database.Exist
{
    /// <summary>
    ///     Default implementation of resolving file names within XML DB.
    /// </summary>
    public class DefaultFilenamesResolver : IFilenamesResolver
    {

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const char Separator = '-';
        private const string XmlFormatSuffix = ".xml";
        private const string ImgFormatSuffix = ".png";
        private const int IdLength = 6;
        private const int PageNumberLength = 4;

        private readonly ExistDao m_dao;

        public DefaultFilenamesResolver(ExistDao dao)
        {
            m_dao = dao;
        }

        public int GenerateUniqueID()
        {
            bool isUnique = false;
            int[] usedIds = GetAllIDs();
            int newUniqId = GenerateID();
            while (!isUnique)
            {
                int idx = Array.IndexOf(usedIds, newUniqId);
                if (idx < 0)
                {
                    isUnique = true;
                }
            }
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Generated unique ID :{0}" ,newUniqId);
            return newUniqId;
        }

        public int[] GetAllIDs()
        {
            string[] allNames = m_dao.ReadAllNames();
            var namesSet = new HashSet<int>();
            for (int i = 0; i < allNames.Length; i++)
            {
                string name = allNames[i];
                int id = GetIDFromName(name);
                namesSet.Add(id);
            }
            return namesSet.ToArray();
        }

        public string[] GetNamesFromID(int id)
        {
            string[] allNames = m_dao.ReadAllNames();
            var returnNames = new List<string>();
            foreach (string t in allNames)
            {
                string name = t;

                // remove collection name
                int startIdx = name.IndexOf('/');
                if (startIdx != -1)
                {
                    int len = name.Length - startIdx - 1;
                    name = name.Substring(startIdx + 1, len);
                }

                int foundId = GetIDFromName(name);
                if (foundId == id)
                {
                    returnNames.Add(name);
                }
            }
            return returnNames.ToArray();
        }

        public string[] GetViewsFromID(int id)
        {
            string[] names = GetNamesFromID(id);
            var views = new string[names.Length];

            for (int i = 0; i < names.Length; i++)
            {
                views[i] = GetViewFromName(names[i]);
            }
            return views;
        }

        public string[] GetAllNames()
        {
            return m_dao.ReadAllNames();
        }

        public int GetIDFromName(string name)
        {
            int endIdx = name.IndexOf(Separator);
            if (endIdx != -1)
            {
                string asStr = name.Substring(0, endIdx);
                return Int32.Parse(asStr);
            }
            else
            {
                throw new Exception(); // TODO custom exception
            }
        }

        public string GetViewFromName(string name)
        {
            int viewIdx = name.IndexOf(Separator);
            int endIdx = name.LastIndexOf(Separator);

            if (viewIdx == endIdx) // does not contain page
            {
                endIdx = name.IndexOf(XmlFormatSuffix);
                if (endIdx == -1)
                {
                    endIdx = name.IndexOf(ImgFormatSuffix);
                }
            }

            int length = endIdx - viewIdx - 1;
            if (viewIdx != -1 && endIdx != -1)
            {
                return name.Substring(viewIdx + 1, length);
            }
            else
            {
                throw new Exception(); // TODO custom exception
            }
        }


        public string GetSeparator()
        {
            return Separator.ToString();
        }

        public string ConstructXmlName(int id, DocumentView view)
        {
            return ConstructName(id, view) + XmlFormatSuffix;
        }

        public string ConstructImgName(int id, DocumentView view)
        {
            return ConstructName(id, view) + ImgFormatSuffix;
        }

        public string ConstructImgName(int id, DocumentView view, int pageNr)
        {
            string page = (pageNr == 0) ? "" : Separator + AlignNumber(pageNr, PageNumberLength);
            return ConstructName(id, view) + page + ImgFormatSuffix;
        }

        public string ConstructImgPageNameTemplate(int id, DocumentView view, string pageNumberTempl)
        {
            return ConstructName(id, view) + pageNumberTempl + ImgFormatSuffix;
        }

        /// <summary>
        ///     Generates random ID.
        /// </summary>
        /// <returns>random ID</returns>
        private int GenerateID()
        {
            int result = 0;
            int dec = 1;
            var random = new Random();
            for (int i = 0; i < IdLength; i++)
            {
                int num = random.Next(10);
                while (i == (IdLength - 1) && num == 0) // first number cannot be zero
                {
                    num = random.Next(10);
                }
                result += dec*num;
                dec *= 10;
            }
            return result;
        }

        private string ConstructName(int id, DocumentView view)
        {
            var sb = new StringBuilder();
            string idStr = AlignNumber(id, IdLength);
            sb.Append(idStr).Append(Separator).Append(view.ToString().ToLower());
            return sb.ToString();
        }

        private string AlignNumber(int number, int length)
        {
            int digits = 0;
            int res = number;
            while (res != 0)
            {
                res /= 10;
                digits++;
            }
            var sb = new StringBuilder();
            for (int i = 0; i < (length - digits); i++)
            {
                sb.Append('0');
            }
            sb.Append(number.ToString());
            return sb.ToString();
        }
    }
}