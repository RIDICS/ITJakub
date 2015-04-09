using System;
using System.Collections.Generic;
using Daliboris.Texty.Evidence.Rozhrani;
using Daliboris.Texty.Export.Rozhrani;
using NLog;

namespace Daliboris.Texty.Export
{
	public abstract class ExportBase
	{

		private static Logger _logger = LogManager.GetCurrentClassLogger();

		public IExportNastaveni Nastaveni { get; set; }

		public void Exportuj(IExportNastaveni emnNastaveni) {
			Nastaveni = emnNastaveni;
			Exportuj();
		}

		public abstract void Exportuj();
		public abstract void Exportuj(IEnumerable<IPrepis> prpPrepisy);

		public void Zaloguj(string zprava)
		{
			Zaloguj(zprava, false);
		}

		public void Zaloguj(string format, object arg0, bool chyba)
		{
			object[] args = new object[1];
			args[0] = arg0;
			Zaloguj(format, args, chyba);
		}
		public void Zaloguj(string format, object arg0, object arg1, bool chyba)
		{
			object[] args = new object[2];
			args[0] = arg0;
			args[1] = arg1;
			Zaloguj(format, args, chyba);

		}
		public void Zaloguj(string format, object arg0, object arg1, object arg2, bool chyba)
		{
			object[] args = new object[1];
			args[0] = arg0;
			args[1] = arg1;
			args[2] = arg2;
			Zaloguj(format, args, chyba);
		}

		public void Zaloguj(string format, object[] args, bool chyba)
		{
			Zaloguj(String.Format(format, args), chyba);
		}


		public void Zaloguj(string zprava, bool chyba)
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			ConsoleColor foregroundColor = Console.ForegroundColor;
			if (!Nastaveni.VypisovatVse && !chyba) return;
			if(chyba)
			{
				Console.BackgroundColor = ConsoleColor.Yellow;
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine(zprava);
			}
			_logger.Debug(zprava);

			if (chyba)
			{
				Console.ForegroundColor = foregroundColor;
				Console.BackgroundColor = backgroundColor;
			}

		}


	}
}