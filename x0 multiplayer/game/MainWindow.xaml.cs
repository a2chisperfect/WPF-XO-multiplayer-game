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
using game.pages;
using Message;
using System.Net;
using System.Threading;

namespace game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        Button[,] buttons;
        GameClient client;
        GamePage gamePage;
        MainPage mainPage;
        LoginPage loginPage;

        public MainWindow()
        {
            InitializeComponent();
            client = new GameClient();
            gamePage = new GamePage();
            mainPage = new MainPage();
            loginPage = new LoginPage();

            loginPage.btnConnect.Click += btnConnect_Click;
            mainPage.btnSearch.Click += btnSearch_Click;
            mainPage.btnQuit.Click += btnQuit_Click;
            mainPage.btnCancelSearch.Click += btnCancelSearch_Click;

            MainFrame.Navigate(loginPage);
            MainFrame.Navigating += MainFrame_Navigating;
            CreateField();
            client.gameSession.GameStartedHandler += gameSession_GameStartedHandler;
            client.gameSession.TurnChangedHandler += gameSession_TurnChangedHandler;
            client.gameSession.ChoiceDoneHandler += gameSession_ChoiceDoneHandler;
            client.gameSession.MatchResultHandler += gameSession_MatchResultHandler;
            gamePage.fieldGid.AddHandler(Button.ClickEvent, new RoutedEventHandler(Button_Click));
        }

        void gameSession_MatchResultHandler(object sender, MatchResultEventArgs e)
        {
            Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                string caption, message;
                caption = "Reuslt:";
                if(e.result == MatchResult.Win)
                {
                    message = "Win";
                }
                else if(e.result ==MatchResult.Lose)
                {
                    message ="Lose";
                }
                else{
                     message ="Draw";
                }
                var tmp = MessageBox.Show(this, message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
                    if(tmp == MessageBoxResult.OK|| tmp== MessageBoxResult.Cancel)
                    {
                        MainFrame.Navigate(mainPage);
                    }
            });
        }

        void gameSession_ChoiceDoneHandler(object sender, ChoiceEventArgs e)
        {
           Dispatcher.BeginInvoke((ThreadStart)delegate()
           {
               ((Button)buttons[e.X, e.Y]).Content = e.symbol;
           });
        }

        void gameSession_TurnChangedHandler(object sender, TurnEventArgs e)
        {
            Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                if (e.turn == Turn.Your)
                {
                    foreach (var button in buttons)
                    {
                        button.IsEnabled = true;
                    }
                    gamePage.tbTurn.Text = "YOUR TURN";
                    gamePage.tbTurn.Foreground = Brushes.Green;
                }
                else
                {
                    foreach (var button in buttons)
                    {
                        button.IsEnabled = false;
                    }
                    gamePage.tbTurn.Text = "ENEMY TURN";
                    gamePage.tbTurn.Foreground = Brushes.Red;
                }
            });
        }

        void gameSession_GameStartedHandler(object sender, GameStartedEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke((ThreadStart)delegate() { gamePage.tbYou.Text = e.Your.Name; gamePage.tbEnemy.Text = e.Enemy.Name; MainFrame.Navigate(gamePage); });
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        void btnCancelSearch_Click(object sender, RoutedEventArgs e)
        {
            client.StopSearching();
            mainPage.btnSearch.IsHitTestVisible = true;
            mainPage.btnQuit.IsHitTestVisible = true;
            mainPage.SearchPannel.Visibility = System.Windows.Visibility.Hidden;
        }

        void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            client.SendQuitMessage();
        }

        void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            client.StartSearching();
            mainPage.btnSearch.IsHitTestVisible = false;
            mainPage.btnQuit.IsHitTestVisible = false;
            mainPage.SearchPannel.Visibility = System.Windows.Visibility.Visible;
        }

        void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.Connect(loginPage.Login.Text, IPAddress.Loopback, 11000, this);
                MainFrame.Navigate(mainPage);
                client.ReciveMessageAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void MainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if(e.Content is GamePage)
            {
                ClearButtons();
            }
            if (e.Content is MainPage)
            {
                mainPage.btnSearch.IsHitTestVisible = true;
                mainPage.btnQuit.IsHitTestVisible = true;
                mainPage.SearchPannel.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void CreateField()
        {
            buttons = new Button[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    buttons[i,j] = new Button();
                    Grid.SetRow(buttons[i, j], i + 1);
                    Grid.SetColumn(buttons[i, j], j);
                    gamePage.fieldGid.Children.Add(buttons[i, j]);
                }
            }
        }
        private void ClearButtons()
        {
            foreach (var b in buttons)
            {
                b.Content = "";
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int x = Grid.GetRow(e.OriginalSource as Button);
            int y = Grid.GetColumn(e.OriginalSource as Button);
            client.MyChoice(new Point(x-1, y));
          
        }

       
    }
}
