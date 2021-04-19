using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.ObjectModel;
using XamarinTimesheet2021.Models;

namespace XamarinTimesheet2021
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkAssignmentPage : ContentPage
    {
        public WorkAssignmentPage(int id)
        {
            InitializeComponent();

            //Annetaan latausilmoitus
            työ_lataus.Text = "Ladataan työtehtäviä...";

            LoadDataFromRestAPI();

            async void LoadDataFromRestAPI()
            {
                try
                {

                    workList.ItemsSource = new List<string> { "Ladataan", "Työtehtäviä", "palvelimelta..." };

                    HttpClient client = new HttpClient();

                    client.BaseAddress = new Uri("https://timesheetbackend2021.azurewebsites.net/");
                    string json = await client.GetStringAsync("/api/workassignments/");

                    IEnumerable<WorkAssignment> works = JsonConvert.DeserializeObject<WorkAssignment[]>(json);
                    ObservableCollection<WorkAssignment> dataa = new ObservableCollection<WorkAssignment>(works);
                    workList.ItemsSource = dataa;

                    //Poistetaan latausilmoitus
                    työ_lataus.Text = "";
                }
                catch (Exception e)
                {
                    await DisplayAlert("Virhe", e.Message.ToString(), "SELVÄ!");
                }
            }

        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }

        private void Aloitus_Nappi_Clicked(object sender, EventArgs e)
        {

        }

        private void Lopetus_Nappi_Clicked(object sender, EventArgs e)
        {

        }
    }
}