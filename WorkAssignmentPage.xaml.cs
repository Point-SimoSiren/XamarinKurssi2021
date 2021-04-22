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
        int eId;

        public WorkAssignmentPage(int idParam)
        {
            InitializeComponent();

            eId = idParam;

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


        async void Aloitus_Nappi_Clicked(object sender, EventArgs e)
        {
            WorkAssignment wa = (WorkAssignment)workList.SelectedItem;

            if (wa == null)
            {
                await DisplayAlert("Valinta puuttuu", "Valitse työtehtävä.", "OK");
            }
            
            
            try
            {
                Operation op = new Operation
                {
                    EmployeeID = eId,
                    WorkAssignmentID = wa.IdWorkAssignment,
                    CustomerID = wa.IdCustomer,
                    OperationType = "start",
                    Comment = "Aloitettu"
                };

                HttpClient client = new HttpClient();

                client.BaseAddress = new Uri("https://timesheetbackend2021.azurewebsites.net/");

                // Muutetaan em. data objekti Jsoniksi
                string input = JsonConvert.SerializeObject(op);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                
                // Lähetetään serialisoitu objekti back-endiin Post pyyntönä
                HttpResponseMessage message = await client.PostAsync("/api/workassignments", content);


                // Otetaan vastaan palvelimen vastaus
                string reply = await message.Content.ReadAsStringAsync();


                 //Asetetaan vastaus serialisoituna success muuttujaan
                 bool success = JsonConvert.DeserializeObject<bool>(reply);

                if (success == false)
                {
                    await DisplayAlert("Ei voida aloittaa", "Työ on jo käynnissä", "OK");
                }
                else if (success == true)
                {
                    await DisplayAlert("Työ aloitettu", "Työ on aloitettu", "OK");
                }
            }

            catch (Exception ex)
            {

                await DisplayAlert(ex.GetType().Name, ex.Message, "OK");
            }

        }

        async void Lopetus_Nappi_Clicked(object sender, EventArgs e)
        {

            WorkAssignment wa = (WorkAssignment)workList.SelectedItem;
            
            if (wa == null)
            {
                await DisplayAlert("Valinta puuttuu", "Valitse työtehtävä.", "OK");
                return;
            }

            string result = await DisplayPromptAsync("Kommentti", "Kirjoita kommentti");
            
            try
            {
                Operation op = new Operation
                {
                    EmployeeID = eId,
                    WorkAssignmentID = wa.IdWorkAssignment,
                    CustomerID = wa.IdCustomer,
                    OperationType = "stop",
                    Comment = result
                    
                };

                HttpClient client = new HttpClient();

                client.BaseAddress = new Uri("https://timesheetbackend2021.azurewebsites.net/");

                // Muutetaan em. data objekti Jsoniksi
                string input = JsonConvert.SerializeObject(op);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");


                // Lähetetään serialisoitu objekti back-endiin Post pyyntönä
                HttpResponseMessage message = await client.PostAsync("/api/workassignments", content);


                // Otetaan vastaan palvelimen vastaus
                string reply = await message.Content.ReadAsStringAsync();


                //Asetetaan vastaus serialisoituna success muuttujaan
                bool success = JsonConvert.DeserializeObject<bool>(reply);

                if (success == false)
                {
                    await DisplayAlert("Ei voida lopettaa", "Työtä ei ole aloitettu", "OK");
                }

                else if (success == true)
                {
                    await DisplayAlert("Työn päättyminen", "Työ on päättynyt", "OK");

                    await Navigation.PushAsync(new WorkAssignmentPage(eId));
                }
            }
            catch (Exception ex)
            {

                await DisplayAlert(ex.GetType().Name, ex.Message, "OK");
            }


        }
    }
}