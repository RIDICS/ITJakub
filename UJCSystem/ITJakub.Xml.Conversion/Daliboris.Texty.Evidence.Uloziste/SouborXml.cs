using System.IO;

namespace Daliboris.Texty.Evidence.Uloziste
{
    public class SouborXml
    {
        private string mstrCestaKSouboru;
        private string mstrCestaKZaloze;

        public string CestaKZaloze
        {
            get { return mstrCestaKZaloze; }
            set { mstrCestaKZaloze = value; }
        }

        public string CestaKSouboru
        {
            get { return mstrCestaKSouboru; }
            set { mstrCestaKSouboru = value; }
        }
        public SouborXml(string strCestaKSouboru) {
            mstrCestaKSouboru = strCestaKSouboru;
        }


        public void Zalohuj(string strCestaKZaloze) {
            mstrCestaKZaloze = strCestaKZaloze;
            Zalohuj();
        }
        public void Zalohuj() {
					if(File.Exists(mstrCestaKSouboru))
            File.Copy(mstrCestaKSouboru, mstrCestaKZaloze, true);
        }
        public void ObnovZalohu() {
					if(File.Exists(mstrCestaKZaloze))
            File.Copy(mstrCestaKZaloze, mstrCestaKSouboru, true);
        }

			public void UlozitJako(string strCestaKSouboru) {

			}

    }

}
