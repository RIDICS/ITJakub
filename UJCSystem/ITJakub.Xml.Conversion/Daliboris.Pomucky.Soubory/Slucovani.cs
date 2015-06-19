using System;
using System.IO;

namespace Daliboris.Pomucky.Soubory {
	public class Slucovani {

		public SlucovaniNastaveni Nastaveni { get; set; }

		public void Sloucit() {
			DirectoryInfo dis = new DirectoryInfo(Nastaveni.Slozka);

			if (dis.GetDirectories().Length == 0) {
				SloucitSouboryZAdesare(dis, dis.Parent.FullName);
			}
			else {
				foreach (DirectoryInfo di in dis.GetDirectories()) {
					SloucitSouboryZAdesare(di, di.Parent.FullName);
				}
			}

		}

		private void SloucitSouboryZAdesare(DirectoryInfo di, string strSlozka) {
			using (StreamWriter sw = new StreamWriter(Path.Combine(strSlozka, di.Name + ".txt"), false)) {
				foreach (FileInfo fi in di.GetFiles(Nastaveni.MaskaSouboru)) {
					//sw.WriteLine("<!-- " + fi.Name + "-->");
					//using(StreamReader sr = new StreamReader(fi.FullName, System.Text.Encoding.GetEncoding("Windows-1250"))) {
					using (StreamReader sr = new StreamReader(fi.FullName)) {
						string sRadek = null;
						while ((sRadek = sr.ReadLine()) != null) {
							if (!sr.EndOfStream)
								sw.WriteLine(sRadek);
							else {
								int i;
								if (!Int32.TryParse(sRadek, out i)) {
									sw.WriteLine(sRadek);
								}
							}
						}
						//sw.Write(sr.ReadToEnd());
						//sw.WriteLine();
					}
				}
			}
		}
	}
}
