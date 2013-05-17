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
    class Crossword {
        private int size_x;
        private int size_y;
        private CrosswordField[,] crossWordfields;
        private Windows.Data.Xml.Dom.XmlDocument xmlCrossword;
        
        public Crossword(Windows.Data.Xml.Dom.XmlDocument xmlDoc) {
                this.xmlCrossword = xmlDoc;

                XmlNodeList size = xmlCrossword.GetElementsByTagName("size");
                var x = size.First().Attributes.GetNamedItem("x");
                size_x = int.Parse(x.NodeValue.ToString());
                var y = size.First().Attributes.GetNamedItem("y");
                size_y = int.Parse(y.NodeValue.ToString());

                crossWordfields = new CrosswordField[size_x, size_y];

                XmlNodeList fields = xmlCrossword.GetElementsByTagName("field");
                foreach (IXmlNode field in fields) {
                    string[] pos = field.Attributes.GetNamedItem("pos").NodeValue.ToString().Split('.');
                    string type = field.Attributes.GetNamedItem("type").NodeValue.ToString();
                    string ch = string.Empty;
                    string verticalHint = string.Empty;
                    string horizontalHint = string.Empty;
                    int pos_x = int.Parse(pos[0]);
                    int pos_y = int.Parse(pos[1]);

                    if (type == "hint") {
                        var verticalElement = field.ChildNodes.Where(Item => Item.NodeName == "vertical").FirstOrDefault();
                        var horizontalElement = field.ChildNodes.Where(Item => Item.NodeName == "horizontal").FirstOrDefault();
                        if (verticalElement != null) {
                            verticalHint = verticalElement.InnerText;
                        }
                        if (horizontalElement != null) {
                            horizontalHint = horizontalElement.InnerText;
                        }
                        crossWordfields[pos_x,pos_y] = new CrosswordHint(verticalHint, horizontalHint);
                    }
                    if (type == "char") {
                        if (field.ChildNodes.Count > 0) {
                            ch = field.ChildNodes.First().InnerText.Trim();
                            crossWordfields[pos_x, pos_y] = new CrosswordChar(ch);
                        } else {
                            ch = "";
                            crossWordfields[pos_x, pos_y] = new CrosswordChar(ch);
                        }
                    }
                }
        }

        internal ScrollViewer getUI() {
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
                        Button e = new Button();
                        e.Width = 40;
                        e.Height = 40;
                        e.Content = "";
                        e.Margin = new Thickness(1);
                        e.IsEnabled = false;
                        hor.Children.Add(e);
                        continue;
                    }

                    if (crossWordfields[i,j].GetType().Equals(typeof(CrosswordChar))) {
                        IT_Jakub.Views.Controls.CrossWord.Char e = new IT_Jakub.Views.Controls.CrossWord.Char(crossWordfields[i, j].getText());
                        e.Margin = new Thickness(1);
                        hor.Children.Add(e);
                        continue;
                    }
                    if (crossWordfields[i, j].GetType().Equals(typeof(CrosswordHint))) {
                        IT_Jakub.Views.Controls.CrossWord.Hint e = new IT_Jakub.Views.Controls.CrossWord.Hint(crossWordfields[i, j].getText(),i,j);
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

    }
}
