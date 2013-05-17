using IT_Jakub.Classes.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace IT_Jakub.Classes.Models.CrosswordsApp {
    class CrossWordSolution {

        public long userId;
        public DateTime dateTime { get; set; }
        public string text;
        public Command command;
        public User creator { get; set; }
        private Command endSolution;
        public double percentage { get; set; }
        public string PercentageText { get; set; }

        public CrossWordSolution(Command c, User creator, Command endSolution) {
            string pattern = "^.+SolutionFinal\\((?<date>\\d{1,2}\\.\\s\\d{1,2}\\.\\s\\d{1,4}\\s\\d{1,2}:\\d{1,2}:\\d{1,2}),\\s(?<user>\\d+),\\s(?<solution>[\\s\\S]*)\\)$";
            Regex r = new Regex(pattern);
            var matches = r.Match(c.CommandText);
            
            string dateTime = matches.Groups["date"].ToString();
            DateTime date = DateTime.Parse(dateTime);

            long userId = long.Parse(matches.Groups["user"].ToString());
            
            string solution = matches.Groups["solution"].ToString();

            this.endSolution = endSolution;
            this.command = c;
            this.userId = userId;
            this.dateTime = date;
            this.text = solution;
            this.creator = creator;
            this.calculatePercentage();
        }

        private void calculatePercentage() {
            string pattern = "^.+SolutionEnd\\((?<solution>[\\s\\S]*)\\)$";
            Regex r = new Regex(pattern);
            var matches = r.Match(endSolution.CommandText);

            string endSolutionText = matches.Groups["solution"].ToString();


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(endSolutionText);

            XmlNodeList size = xmlDoc.GetElementsByTagName("size");
            var x = size.First().Attributes.GetNamedItem("x");
            var size_x = int.Parse(x.NodeValue.ToString());
            var y = size.First().Attributes.GetNamedItem("y");
            var size_y = int.Parse(y.NodeValue.ToString());

            string[,] EndSolutionArray = new String[size_x, size_y];

            XmlNodeList fields = xmlDoc.GetElementsByTagName("field");
            foreach (IXmlNode field in fields) {
                string[] pos = field.Attributes.GetNamedItem("pos").NodeValue.ToString().Split('.');
                string type = field.Attributes.GetNamedItem("type").NodeValue.ToString();
                string ch = string.Empty;
                string verticalHint = string.Empty;
                string horizontalHint = string.Empty;
                int pos_x = int.Parse(pos[0]);
                int pos_y = int.Parse(pos[1]);

                if (type == "char") {
                    if (field.ChildNodes.Count > 0) {
                        ch = field.ChildNodes.First().InnerText.Trim();
                        EndSolutionArray[pos_x, pos_y] = ch;
                    } else {
                        ch = "";
                        EndSolutionArray[pos_x, pos_y] = ch;
                    }
                }
            }

            xmlDoc.LoadXml(text);

            size = xmlDoc.GetElementsByTagName("size");
            x = size.First().Attributes.GetNamedItem("x");
            size_x = int.Parse(x.NodeValue.ToString());
            y = size.First().Attributes.GetNamedItem("y");
            size_y = int.Parse(y.NodeValue.ToString());

            string[,] userSolutionArray = new String[size_x, size_y];

            fields = xmlDoc.GetElementsByTagName("field");
            foreach (IXmlNode field in fields) {
                string[] pos = field.Attributes.GetNamedItem("pos").NodeValue.ToString().Split('.');
                string type = field.Attributes.GetNamedItem("type").NodeValue.ToString();
                string ch = string.Empty;
                string verticalHint = string.Empty;
                string horizontalHint = string.Empty;
                int pos_x = int.Parse(pos[0]);
                int pos_y = int.Parse(pos[1]);

                if (type == "char") {
                    if (field.ChildNodes.Count > 0) {
                        ch = field.ChildNodes.First().InnerText.Trim();
                        userSolutionArray[pos_x, pos_y] = ch;
                    } else {
                        ch = "";
                        userSolutionArray[pos_x, pos_y] = ch;
                    }
                }
            }

            double points = 0;
            double maxPoints = 0;
            for (int i = 0; i < EndSolutionArray.GetLength(0); i++) {
                for (int j = 0; j < EndSolutionArray.GetLength(1); j++) {
                    if (EndSolutionArray[i,j] != null) {
                        maxPoints++;
                        if (EndSolutionArray[i, j] == userSolutionArray[i, j]) {
                            points++;
                        }
                    }
                }
            }

            double pointIsPercentage = 100 / maxPoints;
            percentage = points * pointIsPercentage;
            percentage = Math.Round(percentage, 0);
            PercentageText = percentage + " %";
        }
    }
}
