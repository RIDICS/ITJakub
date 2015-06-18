using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Daliboris.OOXML.Word
{
	public class Styles : ICollection<Style>
	{
		private SortedDictionary<string, Style> mgdcStyly = new SortedDictionary<string, Style>();
		//System.Data.DataSet mdsStyly = new System.Data.DataSet();
		DataTable mdtStyly;
		private string mstrDefaultStyleID = null;

		//private Dictionary<string, string> mgdcID2Name = new Dictionary<string, string>();
		//private Dictionary<string, string> mgdcName2ID = new Dictionary<string, string>();
		//private Dictionary<string, string> mgdcUIName2ID = new Dictionary<string, string>();

		public string DefaultLanguage { get; set; }

		public Styles()
		{
			InicializovatTabulkuStylu();
		}


		private void InicializovatTabulkuStylu()
		{

			mdtStyly = new DataTable("Styly");
			mdtStyly.CaseSensitive = true;
			mdtStyly.Locale = new CultureInfo("cs-cz");
			DataColumn[] keys = new DataColumn[1];
			DataColumn column;


			column = mdtStyly.Columns.Add("StylID", System.Type.GetType("System.String"));
			keys[0] = column;

			mdtStyly.Columns.Add("Nazev", System.Type.GetType("System.String"));
			mdtStyly.Columns.Add("NazevUI", System.Type.GetType("System.String"));
		}

		public Style DefaultStyle(StyleType typ)
		{
			Style stVychoziStyl = null;


			//po konverzi se špatně pojmenují styly a označí výchozí styly
			foreach (Style st in this)
			{
				if (st.IsDefault && st.Type == typ)
				{
					if (st.Type == StyleType.OdstavcovyStyl)
					{
						if (st.ID == "Normln" || st.Name == "Normal" || st.Name == "Výchozí") //Výchozí pro OpenOffice.org, ODT
						{
							stVychoziStyl = st;
							break;
						}
					}
					else if (st.Type == StyleType.ZnakovyStyl)
					{
						if (st.ID == "Standardnpsmoodstavce" || st.Name == "Default Paragraph Font" || st.Name == "NormalCharacter")
						{
							stVychoziStyl = st;
							break;
						}
					}
				}
			}
			if (stVychoziStyl != null)
				return stVychoziStyl;

			foreach (Style st in this)
			{
				if (st.IsDefault && st.Type == typ)
				{
					stVychoziStyl = st;
					break;
				}
			}

			if (stVychoziStyl != null)
				return stVychoziStyl;

			foreach (Style st in this)
			{
				if (st.Type == StyleType.OdstavcovyStyl && st.Type == typ)
				{
					if (st.ID == "Normln" || st.Name == "Normal" || st.Name == "Výchozí") //Výchozí pro OpenOffice.org, ODT
					{
						stVychoziStyl = st;
						break;
					}
				}
				else if (st.Type == StyleType.ZnakovyStyl && st.Type == typ)
				{
					if (st.ID == "Standardnpsmoodstavce" || st.Name == "Default Paragraph Font" || st.Name == "NormalCharacter")
					{
						stVychoziStyl = st;
						break;
					}
				}

			}

			if (stVychoziStyl == null)
			{
				stVychoziStyl = new Style();
				stVychoziStyl.Type = typ;
				stVychoziStyl.Name = "Výchozí styly (automatický)";
				stVychoziStyl.UIName = stVychoziStyl.Name;
				stVychoziStyl.ID = "DDD_Vychozi_styl_automaticky";
				stVychoziStyl.IsDefault = true;
				stVychoziStyl.IsCustom = true;
			}

			return stVychoziStyl;
		}

		private string StyleIDByName(string strName)
		{
			return NajdiStyleID(String.Format("Nazev = '{0}'", strName)); ;
		}

		private string NajdiStyleID(string strFiltr)
		{
			DataView dv = new DataView(mdtStyly);
			dv.RowFilter = strFiltr;
			if (dv.Count != 1)
				return null;
			return dv[0]["StylID"].ToString();
		}

		private string StyleIDByUIName(string strUIName)
		{
			return NajdiStyleID(String.Format("NazevUI = '{0}'", strUIName));

		}

		/// <summary>
		/// Najde styl na základě jeho názvu v uživatelském prostředí.
		/// </summary>
		/// <param name="strUIName"></param>
		/// <returns></returns>
		public Style StyleByUIName(string strUIName)
		{
			string sID = StyleIDByUIName(strUIName);
			if (sID == null)
				return null;
			return mgdcStyly[sID];
			/*
			if (!mgdcUIName2ID.ContainsValue(strUIName))
				return null;
			return mgdcStyly[mgdcUIName2ID[strUIName]];
			*/
		}

		/// <summary>
		/// Najde styl na základě jeho názvu: nejprve hledá podle názvu uživatelského rozhraní,
		/// poté hledá podle identifikátoru (který přiděluje aplikace MS Word).
		/// </summary>
		/// <param name="strName"></param>
		/// <returns></returns>
		public Style StyleByName(string strName)
		{
			string sID = StyleIDByUIName(strName);
			if (sID == null)
				return null;
			return mgdcStyly[sID];

			/*
			if (!mgdcID2Name.ContainsValue(strName))
				return null;
			return mgdcStyly[mgdcName2ID[strName]];
				*/
		}

		public Style StyleByID(string strID)
		{
			if (!mgdcStyly.ContainsKey(strID))
				return null;
			return mgdcStyly[strID];
		}

		/// <summary>
		/// Zda seznam stylů obsahuje styl s daným identifikátorem (který přiděluje aplikace MS Word).
		/// </summary>
		/// <param name="strID">Jedinečný identifikátor stylu.</param>
		/// <returns>Vrací <value>true</value>, pokud seznam styl obsahuje, jinak vrací <value>false</value>. </returns>
		public bool ContainsStyleID(string strID)
		{
			return mgdcStyly.ContainsKey(strID);
		}

		#region ICollection<Style> Members

		public void Add(Style item)
		{
			/*
			mgdcID2Name.Add(item.ID, item.Name);
			mgdcUIName2ID.Add(item.UIName, item.ID);
			mgdcName2ID.Add(item.Name, item.ID);
			*/
			if (item.Language == null && item.BasenOnStyleID == null)
				item.Language = DefaultLanguage;
			PridatStylDoTabulky(item);
			mgdcStyly.Add(item.ID, item);
		}

		public void Clear()
		{
			if (mdtStyly != null)
				mdtStyly.DataSet.Clear();
			mgdcStyly.Clear();
			/*
			mgdcName2ID.Clear();
			mgdcUIName2ID.Clear();
			mgdcID2Name.Clear();
				*/
		}

		public bool Contains(Style item)
		{
			return mgdcStyly.ContainsValue(item);
		}

		public void CopyTo(Style[] array, int arrayIndex)
		{
			mgdcStyly.Values.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return mgdcStyly.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(Style item)
		{
			string sID = item.ID;
			if (mgdcStyly.Remove(sID))
			{
				OdstranitStylZTabulky(sID);
				return true;
			}
			else
				return false;
			/*
			mgdcID2Name.Remove(item.ID);
			mgdcName2ID.Remove(item.Name);
			mgdcUIName2ID.Remove(item.UIName);
			return mgdcStyly.Remove(item.ID);
				*/
		}

		private void PridatStylDoTabulky(Style item)
		{
			DataRowView drv = mdtStyly.DefaultView.AddNew();
			drv.BeginEdit();
			drv["StylID"] = item.ID;
			drv["Nazev"] = item.Name;
			drv["NazevUI"] = item.UIName;
			drv.EndEdit();

		}

		private void OdstranitStylZTabulky(string sID)
		{
			DataView dv = new DataView(mdtStyly);
			dv.AllowDelete = true;
			dv.RowFilter = String.Format("StylID = '{0}'", sID);
			if (dv.Count == 1)
				dv.Delete(0);
		}

		#endregion

		/// <summary>
		/// Vrací kolekci stylů podle typu.
		/// </summary>
		/// <param name="stTyp">Typ stylu, který se má vrátit</param>
		/// <returns>Vrací kolekci stylů podle typu.</returns>
		public IEnumerable<Style> GetStylesByType(StyleType stTyp)
		{
			foreach (Style item in this)
			{
				if (item.Type == stTyp)
					yield return item;
			}
		}

		#region IEnumerable<Style> Members

		public IEnumerator<Style> GetEnumerator()
		{
			return mgdcStyly.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
