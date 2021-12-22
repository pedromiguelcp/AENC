using AENC.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfSharp;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Test;
using Document = iTextSharp.text.Document;
using PdfReader = iTextSharp.text.pdf.PdfReader;
using Rectangle = iTextSharp.text.Rectangle;

namespace AENC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Metric> AllMetrics = new List<Metric>();
        public List<Metric> SelectedMetrics = new List<Metric>();
        int auxswitchTab = 0;
        string filename, printer, path;
        bool allmetrics = false;

        public MainWindow()
        {
            InitializeComponent();
            FillMetricsList();
        }


        #region Metrics
        private void combobox_lvl_test_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(combobox_lvl_test.SelectedValue != null)
            {
                SelectedMetrics.Clear();
                checkboxAllMetrics.IsChecked = false;

                foreach (var selectallmetrics in AllMetrics)
                {
                    selectallmetrics.IsSelected = false;
                }

                object LvlMetric = combobox_lvl_test.SelectedValue;
                Metric MetricPattern = (Metric)LvlMetric;
                if (MetricPattern.MetricLvl == "All")
                    SelectedMetrics = AllMetrics.Where(x => x.MetricLvl != "All").ToList();
                else
                    SelectedMetrics = AllMetrics.Where(x => x.MetricLvl == MetricPattern.MetricLvl).ToList();

                dg_metrics.ItemsSource = null;
                dg_metrics.ItemsSource = SelectedMetrics;
                UploadFile.IsEnabled = true;
                btn_scancode.IsEnabled = true;
                btn_scancode.IsEnabled = false;
                lvl.Content = MetricPattern.MetricLvl;
            }
            else
            {
                UploadFile.IsEnabled = false;
                btn_scancode.IsEnabled = false;
                dg_metrics.ItemsSource = null;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var selectallmetrics in SelectedMetrics)
            {
                selectallmetrics.IsSelected = true;
            }
            dg_metrics.ItemsSource = null;
            dg_metrics.ItemsSource = SelectedMetrics;
            allmetrics = true;
        }

        private void Checkboxall_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var selectallmetrics in SelectedMetrics)
            {
                selectallmetrics.IsSelected = false;
            }
            dg_metrics.ItemsSource = null;
            dg_metrics.ItemsSource = SelectedMetrics;
            allmetrics = false;
        }
        #endregion

        #region Code Analysis
        public void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            // Create a Flow Document
            FlowDocument ObjFdoc = new FlowDocument();
            filename = "";
            Stream mystream;

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".cs"; // Default file extension
            dlg.Filter = "CS documents (.cs)|*.cs"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                if ((mystream = dlg.OpenFile()) != null)
                {
                    // Open document
                    filename = dlg.FileName;
                    OriginalCode.Text = File.ReadAllText(filename);
                    btn_scancode.IsEnabled = true;


                    /*FlowDocument contentForStoring =
                    new FlowDocument(new Paragraph(new Run(File.ReadAllText(filename))));*/

                    /*var line = OriginalCode.Document.GetLineByOffset(50);
                    var lineNumber = line.LineNumber;

                    //Select the text.
                    OriginalCode.SelectionStart = 1;
                    OriginalCode.SelectionLength = 2;

                    //Scroll the textEditor to the selected line.
                    var visualTop = OriginalCode.TextArea.TextView.GetVisualTopByDocumentLine(lineNumber);
                    OriginalCode.ScrollToVerticalOffset(visualTop);*/

                }
                lvUsers.ItemsSource = null;
            }
        }

        private void btn_scancode_Click(object sender, RoutedEventArgs e)
        {
            List<CodeSmellDetected> itemss = new List<CodeSmellDetected>();
            printer = "";
            /*itemss.Add(new CodeSmellDetected() { CodeSmellName = "dores do pe", CodeSmellClass = "John asdfas", CodeSmellFunction = "John ttt", CodeSmellLine = 1 });
            itemss.Add(new CodeSmellDetected() { CodeSmellName = "de cabeça", CodeSmellClass = "John Doe", CodeSmellFunction = "John ghkfgjk", CodeSmellLine = 2 });
            itemss.Add(new CodeSmellDetected() { CodeSmellName = "de cabeça", CodeSmellClass = "John uiiu", CodeSmellFunction = "gsa", CodeSmellLine = 3 });*/


            CodeSmeller my = new CodeSmeller(filename);

            foreach (Namespace_t var in my.namespacesArray)
            {
                if (var != null)
                {
                    foreach (Class_t var1 in var.classArray)
                    {
                        if (var1 != null)
                        {
                            if (SelectedMetrics.Where(x => x.MetricID == 7).ToList()[0].IsSelected & var1.hasToManyLines())
                            {
                                itemss.Add(new CodeSmellDetected() { CodeSmellName = "Too Many Lines", CodeSmellClass = var1.classname.Replace("()", ""), CodeSmellFunction = "---", CodeSmellLine = 1 });
                                
                            }

                            if (SelectedMetrics.Where(x => x.MetricID == 1).ToList()[0].IsSelected & var1.hasToManyMethods())
                            {
                                itemss.Add(new CodeSmellDetected() { CodeSmellName = "Too Many Methods", CodeSmellClass = var1.classname.Replace("()", ""), CodeSmellFunction = "---", CodeSmellLine = 1 });
                            }
                            foreach (Methods_t var2 in var1.methodArray)
                            {
                                if (var2 != null)
                                {
                                    List<string> unusedparam = var2.hasUnusedParameters();
                                    if (SelectedMetrics.Where(x => x.MetricID == 3).ToList()[0].IsSelected & unusedparam.Count != 0)
                                    {
                                        itemss.Add(new CodeSmellDetected() { CodeSmellName = "Unused Parameters", CodeSmellClass = var1.classname.Replace("()", ""), CodeSmellFunction = var2.functionName, CodeSmellLine = 1 });
                                    }

                                    if (SelectedMetrics.Where(x => x.MetricID == 7).ToList()[0].IsSelected & var2.hasToManyLines())
                                    {
                                        itemss.Add(new CodeSmellDetected() { CodeSmellName = "Too Many Lines", CodeSmellClass = var1.classname.Replace("()", ""), CodeSmellFunction = var2.functionName, CodeSmellLine = 1 });
                                    }

                                    if (SelectedMetrics.Where(x => x.MetricID == 2).ToList()[0].IsSelected & !var2.isTheLineToCommentRatioGood())
                                    {
                                        itemss.Add(new CodeSmellDetected() { CodeSmellName = "Bad Line to Comment Ratio", CodeSmellClass = var1.classname.Replace("()", ""), CodeSmellFunction = var2.functionName, CodeSmellLine = 1 });
                                    }

                                    if (SelectedMetrics.Where(x => x.MetricID == 5).ToList()[0].IsSelected & var2.toMuchNesting())
                                    {
                                        itemss.Add(new CodeSmellDetected() { CodeSmellName = "Too Much Nesting", CodeSmellClass = var1.classname.Replace("()", ""), CodeSmellFunction = var2.functionName, CodeSmellLine = 1 });
                                    }
                                    if (SelectedMetrics.Where(x => x.MetricID == 6).ToList()[0].IsSelected & var2.switchCaseToComplex())
                                    {
                                        itemss.Add(new CodeSmellDetected() { CodeSmellName = "High Switch Case Complexity", CodeSmellClass = var1.classname.Replace("()", ""), CodeSmellFunction = var2.functionName, CodeSmellLine = 1 });
                                    }

                                    if (SelectedMetrics.Where(x => x.MetricID == 4).ToList()[0].IsSelected & var2.hasToManyParameters())
                                    {
                                        itemss.Add(new CodeSmellDetected() { CodeSmellName = "Too Many Parameters", CodeSmellClass = var1.classname.Replace("()", ""), CodeSmellFunction = var2.functionName, CodeSmellLine = 1 });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            printer = "Code Smells found on code!\n\n";
            foreach (CodeSmellDetected var in itemss)
            {
                printer += var.CodeSmellName + " Smell found on class: " + var.CodeSmellFunction + "\n";
            }

            lvUsers.ItemsSource = itemss;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("CodeSmellName");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void Printer_Click(object sender, RoutedEventArgs e)
        {
            iTextSharp.text.Document oDoc = new iTextSharp.text.Document();
            PdfWriter.GetInstance(oDoc, new FileStream(@"C:\Users\pedro\Desktop\4Ano\TVSI\AENC.pdf", FileMode.Create));
            oDoc.Open();
            oDoc.Add(new iTextSharp.text.Paragraph(printer));
            oDoc.Close();
        }
       
        #endregion

        #region About Code Smells

        private void combobox_txt_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(combobox_txt_name.SelectedValue != null)
            {
                object Metric = combobox_txt_name.SelectedValue;
                Metric MetricPattern = (Metric)Metric;
                AboutMetrics.Text = MetricPattern.MetricDescription;
                AboutMetrics.Text += "\n\n";
                AboutMetrics.Text += MetricPattern.MetricDetails;
            }
            else
            {
                AboutMetrics.Text = null;
            }
        }

        #endregion

        #region Others
        public void FillMetricsList()
        {
            AllMetrics.Clear();
            SelectedMetrics.Clear();
            UploadFile.IsEnabled = false;
            btn_scancode.IsEnabled = false;
            printer = "";

            AllMetrics.Add(new Metric
            {
                MetricID = 1,
                MetricLvl = "2",
                MetricName = "Amount of Class Methods",
                MetricDescription = "Evaluate if the class has to many methods",
                MetricDetails = "It is good practice to avoid creating classes in code with too many associated methods. " +
                "At some point it may be a little difficult to realize the usefulness of each function within the class.",
                IsSelected = false
            });
            AllMetrics.Add(new Metric
            {
                MetricID = 2,
                MetricLvl = "2",
                MetricName = "Line to Comment Ratio",
                MetricDescription = "Evaluate if there is a good ratio between code and comments",
                MetricDetails = "Each piece of code should be properly commented for a better external perception of its functionality. " +
                "On the other hand, overly commented code can mean its poor quality.",
                IsSelected = false
            });
            AllMetrics.Add(new Metric
            {
                MetricID = 3,
                MetricLvl = "1",
                MetricName = "Parameters Usability",
                MetricDescription = "Evaluate if all function parameters are used",
                MetricDetails = "If a function has input parameters, these must be used within it, " +
                "otherwise the parameters are useless for the functionality of the function.",
                IsSelected = false
            });
            AllMetrics.Add(new Metric
            {
                MetricID = 4,
                MetricLvl = "2",
                MetricName = "Amount of Method Parameters",
                MetricDescription = "Evaluate if the method has to many parameters",
                MetricDetails = "Too many input parameters of a method is an indicator of a bad coding practice. " +
                "There are certain limits to the use of input parameters in a function and quantity is one of them.",
                IsSelected = false
            });
            AllMetrics.Add(new Metric
            {
                MetricID = 5,
                MetricLvl = "3",
                MetricName = "Method Nesting",
                MetricDescription = "Evaluate the method nesting level",
                MetricDetails = "The level of nesting in a program's code is something that can be measured to evaluate its simplicity. " +
                "Apart from certain exceptions the nesting level of a code should not be too high to be easier to maintain.",
                IsSelected = false
            });
            AllMetrics.Add(new Metric
            {
                MetricID = 6,
                MetricLvl = "3",
                MetricName = "Decision Blocks Complexity",
                MetricDescription = "Evaluate the overall complexity of decision blocks",
                MetricDetails = "The complexity of the decision blocks in the code should be as small as possible.\n " +
                "Huge complexity in these blocks brings problems of comprehension and maintenance.",
                IsSelected = false
            });
            AllMetrics.Add(new Metric
            {
                MetricID = 7,
                MetricLvl = "1",
                MetricName = "Amount of Code Lines",
                MetricDescription = "Evaluate the number of lines of code in the class and function.",
                MetricDetails = "Number of lines of code in a function or class is important when it comes to code cleanup. " +
                "Obviously, an excessively large number of lines of code make it more difficult to interpret.",
                IsSelected = false
            });
            AllMetrics.Add(new Metric
            {
                MetricID = 8,
                MetricLvl = "All",
                MetricName = "",
                IsSelected = false
            });

            combobox_lvl_test.ItemsSource = null;
            combobox_lvl_test.ItemsSource = AllMetrics.GroupBy(x => x.MetricLvl).Select(y => y.First());
            combobox_txt_name.ItemsSource = null;
            combobox_txt_name.ItemsSource = AllMetrics.Where(x => x.MetricLvl != "All").ToList();
        }

        private void Tabcontrol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (e.OriginalSource == Tabcontrol)
            {
                if (Tabcontrol.SelectedIndex == 1)
                {
                    if (dg_metrics.SelectedCells.Count() != 0 || allmetrics)
                        return;
                    if(auxswitchTab == 1)
                        auxswitchTab = 0;
                    else
                    {
                        SelectedMetrics win = new SelectedMetrics();
                        win.Show();
                        auxswitchTab = 1;
                    }
                    Tabcontrol.SelectedIndex = 0;
                }
            }
        }

        #endregion

       
    }
}
