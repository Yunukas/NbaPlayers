using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace NbaPlayers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // the active page of player list
        private int pageNo = 1;

        // this field will hold the selection of current position 
        private static string Position = "All";

        // the API address
        const string URL = "https://www.balldontlie.io/api/v1/players?";

        // the container for players that is returned from the API request
        private static List<NbaPlayer> players = new List<NbaPlayer>();

        // the container for meta info of the response
        private static List<Meta> metaInfo = new List<Meta>();
        public MainWindow()
        {
            InitializeComponent();

            // on load, disable the previous button
            if (pageNo == 1)
                prev_button.IsEnabled = false;

            label_currentPage.Content = pageNo.ToString();
        }

        // when the grid is loaded, do our async operation
        // get players asynchronously 
        // and then get positions and fill the player list box
        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            next_button.IsEnabled = false;

            players = await GetPlayersAsync();

            next_button.IsEnabled = true;

            GetPositions();

            FillPlayerListBox();
        }

        // this will connect to the API and get player list
        private async Task<List<NbaPlayer>> GetPlayersAsync()
        {
            //List<NbaPlayer> players = new List<NbaPlayer>();

            // used by Visual Studio to create socket connections
            HttpClient client = new HttpClient();
            // Makes request to cat facts

            HttpResponseMessage response = await client.GetAsync(URL + "page=" + pageNo);
            // Not required, but checks if status code is successful. Other status codes are errors or page not found.
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                // print JSON response
                //Console.WriteLine(res);
               
                players = SerializePlayers(res);
                metaInfo = SerializeMetaInfo(res);
            }

            return players;
        }

        // this will serialize the parsed player list json string and filter with position
        private static List<NbaPlayer> SerializePlayers(string json)
        {

            List<NbaPlayer> Players = new List<NbaPlayer>();

            // read the json file
            JObject job = JObject.Parse(json);

            // get the results fragment of the json as a list of JTokens
            List<JToken> results = job["data"].Children().ToList();

            // populate the list
            foreach (JToken result in results)
            {
                NbaPlayer player = result.ToObject<NbaPlayer>();

                Players.Add(player);
            }

            string position = Position.Equals("All") ? "" : Position;

            // use LINQ to filter the player list based on position
            var playerList = Players.Where(x => x.Position.Contains(position)).Select(y => y).ToList();

            //foreach(NbaPlayer player in playerList)
            //{
            //    Console.WriteLine(player.First_Name);
            //}

            return playerList;
        }

        // this will serialize the parsed player list json string and filter with position
        private static List<Meta> SerializeMetaInfo(string json)
        {

            List<Meta> meta = new List<Meta>();

            // read the json file
            JObject job = JObject.Parse(json);

            // get the results fragment of the json as a list of JTokens
            List<JToken> results = job["meta"].Parent.ToList();

            // populate the list
            foreach (JToken result in results)
            {
                Meta m = result.ToObject<Meta>();

                meta.Add(m);
            }
            
            return meta;
        }

        // this will filter out the unique positions from the list of players
        // page 1 of the player list contains all the required positions
        private void GetPositions()
        {
            // filter out the unique positions using LINQ
            var positions = from player in players.Select(x => x.Position)
                            group player by player
                            into g
                            orderby g.First()
                            select g.First();

            // add All positions selector
            position_comboBox.Items.Add("All");

            // add the unique positions to the combobox
            foreach (string position in positions)
            {
                position_comboBox.Items.Add(position);
            }
        }

        // whenever combobox selection is changed, we will update the player list box
        private void Position_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // first reset the page number
            //pageNo = 1;

            //label_currentPage.Content = pageNo.ToString();

            // get the selected item
            Position = position_comboBox.SelectedItem.ToString();

            UpdateView();

        }

        // this method will fill the player list box with the filtered player list
        private void FillPlayerListBox()
        {
            players_listBox.Items.Clear();

            foreach (NbaPlayer player in players)
            {
                players_listBox.Items.Add(player.First_Name + " " + player.Last_Name);
            }
        }

        // when next button is clicked, the next page of player list will be queried and printed
        // also enable the previous button here
        private void Next_button_Click(object sender, RoutedEventArgs e)
        {
            ++pageNo;

            UpdateView();
        }

        // when previous button is clicked, the previous page of player list will be queried and printed
        // also disable the previous button if page no becomes 0
        private void Prev_button_Click(object sender, RoutedEventArgs e)
        {
            --pageNo;

            UpdateView();
        }


        

        private void TextBox_goToPage_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                string text = textBox_goToPage.Text;
                int num;

                // check if the text is actually a number
                if (int.TryParse(text, out num))
                {
                    if(num > 0)
                    {
                        pageNo = num;
                        UpdateView();
                    }
                    else
                    {
                        textBox_goToPage.Text = pageNo.ToString();
                        MessageBox.Show("Please input a number greater than 0");
                    }
                }
            }
        }

        // this method will update our gui
        private async void UpdateView()
        {
            if (pageNo == 1)
                prev_button.IsEnabled = false;
            else
                prev_button.IsEnabled = true;

            if (pageNo == metaInfo[0].Total_Pages)
                next_button.IsEnabled = false;
            else
                next_button.IsEnabled = true;

            label_currentPage.Content = pageNo.ToString();

            await GetPlayersAsync();

            FillPlayerListBox();

        }
    }
}
