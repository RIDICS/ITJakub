using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Daliboris.Pomucky.Xml;
using DPXT = Daliboris.Pomucky.Xml.Transformace;
using Ujc.Ovj.Xml.Info;

namespace Daliboris.Slovniky
{
    public class MockDictionary : Slovnik
    {
        public override void SeskupitHeslaPismene(string inputFile, string outputFile)
        {
            File.Copy(inputFile, outputFile);
        }

        public override void UpravitHraniceHesloveStati(string inputFile, string outputFile)
        {
            File.Copy(inputFile, outputFile);
        }

        public override void KonsolidovatHeslovouStat(string inputFile, string outputFile)
        {
            File.Copy(inputFile, outputFile);
        }

        public override void UpravitOdkazy(string inputFile, string outputFile)
        {
            File.Copy(inputFile, outputFile);
        }
    }
}
