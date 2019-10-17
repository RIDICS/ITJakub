using System.IO;

namespace Vokabular.ImportTestData.App
{
    public class DataProvider
    {
        private readonly TextWriter m_output;
        private readonly TextReader m_input;

        public DataProvider(TextWriter output, TextReader input)
        {
            m_output = output;
            m_input = input;
        }

        public TextWriter Output => m_output;

        public string GetString(string label)
        {
            m_output.WriteLine(label);
            var data = m_input.ReadLine();
            return data;
        }

        public int GetNumber(string label)
        {
            var data = GetString(label);
            return string.IsNullOrWhiteSpace(data) ? 0 : int.Parse(data);
        }
    }
}