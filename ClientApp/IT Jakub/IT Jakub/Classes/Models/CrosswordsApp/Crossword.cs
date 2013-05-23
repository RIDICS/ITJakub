using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace IT_Jakub.Classes.Models.CrosswordsApp {
    /// <summary>
    /// Crossword class represents Crossword, containing Fields.
    /// </summary>
    class Crossword {
        /// <summary>
        /// The size_x is count of fields in horisontal direction
        /// </summary>
        private int size_x;
        /// <summary>
        /// The size_y is count of fields in vertical direction
        /// </summary>
        private int size_y;
        /// <summary>
        /// The crossWordFields is two dimensional array of Fields in Crossword
        /// </summary>
        private CrosswordField[,] crossWordfields;
        /// <summary>
        /// The xmlCrossword is XML document definition of crossword
        /// </summary>
        private Windows.Data.Xml.Dom.XmlDocument xmlCrossword;

        /// <summary>
        /// Initializes a new instance of the <see cref="Crossword"/> class.
        /// </summary>
        /// <param name="xmlDoc">The XML document definition of Crossword.</param>
        public Crossword(Windows.Data.Xml.Dom.XmlDocument xmlDoc) {
            this.xmlCrossword = xmlDoc;

            XmlNodeList size = xmlCrossword.GetElementsByTagName("size");
            var x = size.First().Attributes.GetNamedItem("x");
            size_x = int.Parse(x.NodeValue.ToString());
            var y = size.First().Attributes.GetNamedItem("y");
            size_y = int.Parse(y.NodeValue.ToString());

            crossWordfields = new CrosswordField[size_x, size_y];

            XmlNodeList hint = xmlCrossword.GetElementsByTagName("hint");
            if (hint.Count > 0) {
                XmlNodeList hints = hint[0].ChildNodes;

                string hintText = string.Empty;

                foreach (IXmlNode item in hints) {
                    string text = item.InnerText.ToString().ToUpper();
                    text = text.Replace("\n", "");
                    text = text.Replace("\t", "");
                    if (text.Length > 0) {
                        hintText += text + "\r\n";
                    }
                }
                crossWordfields[0, 0] = new CrosswordHint(hintText);
            }
            XmlNodeList fields = xmlCrossword.GetElementsByTagName("field");
            foreach (IXmlNode field in fields) {
                string[] pos = field.Attributes.GetNamedItem("pos").NodeValue.ToString().Split('.');
                string type = field.Attributes.GetNamedItem("type").NodeValue.ToString();
                string puzzle = string.Empty;
                try {
                    puzzle = field.Attributes.GetNamedItem("puzzle").NodeValue.ToString().Trim();
                } catch (Exception e) { }
                string ch = string.Empty;
                string verticalHint = string.Empty;
                string horizontalHint = string.Empty;
                int pos_x = int.Parse(pos[0]);
                int pos_y = int.Parse(pos[1]);

                if (type == "question") {
                    var verticalElement = field.ChildNodes.Where(Item => Item.NodeName == "vertical").FirstOrDefault();
                    var horizontalElement = field.ChildNodes.Where(Item => Item.NodeName == "horizontal").FirstOrDefault();
                    if (verticalElement != null) {
                        verticalHint = verticalElement.InnerText;
                    }
                    if (horizontalElement != null) {
                        horizontalHint = horizontalElement.InnerText;
                    }
                    crossWordfields[pos_x, pos_y] = new CrosswordQuestion(verticalHint, horizontalHint);
                }
                if (type == "char") {                    
                    if (field.ChildNodes.Count > 0) {
                        ch = field.ChildNodes.First().InnerText.Trim();
                        CrosswordChar cch = new CrosswordChar(ch);
                        if (puzzle == "puzzle") {
                            cch.makePuzzle();
                        }
                        crossWordfields[pos_x, pos_y] = cch;
                    } else {
                        ch = "";
                        CrosswordChar cch = new CrosswordChar(ch);
                        if (puzzle == "puzzle") {
                            cch.makePuzzle();
                        }
                        crossWordfields[pos_x, pos_y] = new CrosswordChar(ch);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Crossword GUI.
        /// </summary>
        /// <returns></returns>
        internal ScrollViewer getGUI() {
            ScrollViewer sv = new ScrollViewer();
            StackPanel ver = new StackPanel();
            ver.Orientation = Orientation.Horizontal;
            int x = crossWordfields.GetLength(0);
            int y = crossWordfields.GetLength(1);

            for (int i = 0; i < x; i++) {
                StackPanel hor = new StackPanel();
                hor.Orientation = Orientation.Vertical;
                
                for (int j = 0; j < y; j++) {
                    if (crossWordfields[i, j] == null) {
                        IT_Jakub.Views.Controls.CrossWord.Null e = new IT_Jakub.Views.Controls.CrossWord.Null();
                        e.Margin = new Thickness(1);
                        hor.Children.Add(e);
                        continue;
                    }

                    if (crossWordfields[i,j].GetType().Equals(typeof(CrosswordChar))) {
                        CrosswordChar field = crossWordfields[i, j] as CrosswordChar;
                        IT_Jakub.Views.Controls.CrossWord.Char e = new IT_Jakub.Views.Controls.CrossWord.Char(field.getText());
                        if (field.isPuzzleChar()) {
                            e = new IT_Jakub.Views.Controls.CrossWord.Char(field.getText(), field.isPuzzleChar());
                        }
                        e.Margin = new Thickness(1);
                        hor.Children.Add(e);
                        continue;
                    }
                    if (crossWordfields[i, j].GetType().Equals(typeof(CrosswordQuestion))) {
                        IT_Jakub.Views.Controls.CrossWord.Question e = new IT_Jakub.Views.Controls.CrossWord.Question(crossWordfields[i, j].getText(),i,j);
                        e.Margin = new Thickness(1);
                        hor.Children.Add(e);
                        continue;
                    }
                    if (crossWordfields[i, j].GetType().Equals(typeof(CrosswordHint))) {
                        IT_Jakub.Views.Controls.CrossWord.Hint e = new IT_Jakub.Views.Controls.CrossWord.Hint(crossWordfields[i, j].getText(), i, j);
                        e.Margin = new Thickness(1);
                        hor.Children.Add(e);
                        continue;
                    }
                    
                }
                ver.Children.Add(hor);
            }
            sv.Content = ver;
            return sv;
            
        }

        internal XmlDocument getXML() {
            return this.xmlCrossword;
        }

    }
}
