using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using LumenWorks.Framework.IO.Csv;
using CsvReader = LumenWorks.Framework.IO.Csv.CsvReader;

namespace Activity_Count
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum Fields
        {
            ActivityName, // major one for this application
            ActivityCategoryName,
            ActivityStartDateMs,
            ActivityStartDate,
            ActivityEndDateMs,
            ActivityEndDate,
            ActivityDurationMs,
            ActivityDuration
        };
        
        private Dictionary<string, int> _activityCount
            = new Dictionary<string, int>();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private string SerializeCsv(CsvReader csv)
        {
            string results = "";
            
            int fieldCount = csv.FieldCount;
            string[] headers = csv.GetFieldHeaders();
            while (csv.ReadNextRecord()) {
                for (int i = 0; i < fieldCount; i++) {
                    results += csv[i] + " ";
                }
                results += "\n";
            }

            return results;
        }

        private void CountActivities(CsvReader csv)
        {
            int fieldCount = csv.FieldCount;
            string[] headers = csv.GetFieldHeaders();
            while (csv.ReadNextRecord()) {
                string activityName = csv[(int)Fields.ActivityName];
                if (_activityCount.ContainsKey(activityName)) {
                    _activityCount[activityName]++;
                } else {
                    _activityCount.Add(activityName, 1);
                }
            }
        }

        private string CountToString()
        {
            string results = "";

            IOrderedEnumerable<KeyValuePair<string, int>> sortedDict 
                = _activityCount.OrderByDescending(x => x.Value);            
            foreach (KeyValuePair<string, int> item in sortedDict) {
                results += item.Key + " " + item.Value;
                results += "\n";
            }
            
            return results;
        }
        
        private void ButtonCountActivities_OnClick(object sender, RoutedEventArgs e)
        {

            dynamic csv;
            string countString = "";
            using (csv = new CachedCsvReader(new StreamReader(@"Config\data.csv"), true)) {
                CountActivities(csv);
                countString = CountToString();
                labelDone.Content = "Done counting activities!";
            }


            //string csvString = SerializeCsv(csv);
            //MessageBox.Show(csvString);
            System.IO.File.WriteAllText(@"output.txt", countString);
        }
    }
}