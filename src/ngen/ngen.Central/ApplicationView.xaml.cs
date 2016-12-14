#region Using directives

using System.Windows;
using ngen.Data;
using ngen.Data.Model;

#endregion

namespace ngen.Central
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var db = new ngenDbContext();

            var customer = new Customer
            {
                FullName = "e2v Technologies Ltd - Chelmsford",
                ShortName = "e2v Chelmsford"
            };

            db.Customers.Add(customer);

            db.SaveChanges();
        }
    }
}