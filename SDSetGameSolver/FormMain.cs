using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace SDSetGameSolver
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void SetStatus(string text)
        {
            labelStatus.Text = text;
        }

        private void SetStatusDetails(string text)
        {
            labelStatusDetails.Text = text;
        }

        private string FetchPageSource(string url, BackgroundWorker worker)
        {
            var source = string.Empty;

            try
            {
                var request = WebRequest.Create(url);
                var dataStream = request.GetResponse().GetResponseStream();
                
                using (var sr = new StreamReader(dataStream))
                {
                    worker.ReportProgress(0, "Downloading SET Puzzle data...");
                    source = sr.ReadToEnd();
                }
            }
            catch(Exception)
            {
                throw new Exception("Could not connect to the Set Game server!");
            }

            return source;
        }

        private List<int> ParseIntArray(string source, int pos, int expected)
        {
            var result = new List<int>();

            var i = pos - 1;
            var cs = string.Empty;
            while (true)
            {
                i++;

                if (i >= source.Length)
                {
                    throw new IndexOutOfRangeException("Failed to parse int array - reached end of source string!");
                }
                else if (result.Count > expected)
                {
                    throw new Exception("Failed to parse int array - parsed more elements than expected!");
                }

                var c = source[i];

                if (char.IsDigit(c))
                {
                    cs += c;
                    continue;
                }
                else if (char.IsWhiteSpace(c))
                {
                    continue;
                }
                else
                {
                    result.Add(int.Parse(cs));
                    cs = string.Empty;

                    if (c == ',')
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (result.Count < expected)
            {
                throw new Exception("Failed to parse int array - parsed less elements than expected!");
            }
            return result;
        }

        private SetBoard ParseBoard(string source)
        {
            var board = new SetBoard();

            var pos = source.IndexOf("board.initSetCards(");
            if (pos == -1)
            {
                throw new Exception("Board parsing failed - could not locate board cards!");
            }

            pos += "board.initSetCards(".Length;

            List<int> cardList;
            try
            {
                cardList = ParseIntArray(source, pos, 12);
            }
            catch (Exception)
            {
                throw new Exception("Board parsing failed - failed to parse board cards array!");
            }

            for (int i = 0; i < 12; i++)
            {
                board.cards[i].number = cardList[i];
            }

            return board;
        }

        private SetSolutions ParseSolutions(string source)
        {
            var solutions = new SetSolutions();

            var pos = source.IndexOf("board.initSets(");
            if (pos == -1)
            {
                throw new Exception("Solutions parsing failed - could not locate set solutions!");
            }

            pos += "board.initSets(".Length;

            List<int> solList;
            try
            {
                 solList = ParseIntArray(source, pos, 18);
            }
            catch(Exception)
            {
                throw new Exception("Solutions parsing failed - failed to parse solution array!");
            }

            for (int s = 0; s < 6; s++)
            {
                for(int c = 0; c < 3; c++)
                {
                    solutions.sols[s].cards[c].number = solList[(s * 3) + c];
                }
            }

            return solutions;
        }

        private string ParsePuzzleName(string source)
        {
            var pos = source.IndexOf("<font size=\"6\" face=\"Verdana\">");
            if (pos == -1)
            {
                return "(Unable to parse Puzzle name)";
            }

            pos += "<font size=\"6\" face=\"Verdana\">".Length;
            
            try
            {
                var i = pos - 1;
                var cs = string.Empty;
                while (true)
                {
                    i++;

                    if (i >= source.Length)
                    {
                        return "(Unable to parse Puzzle name)";
                    }

                    var c = source[i];

                    if(c == '<')
                    {
                        return cs;
                    }
                    else
                    {
                        cs += c;
                        continue;
                    }
                }
            }
            catch (Exception)
            {
                return "(Unable to parse Puzzle name)";
            }
        }

        private void AssignCardImage(Image image, int boardIndex)
        {
            switch(boardIndex)
            {
                case 0:
                    pictureBoxCard1.Image = image;
                    return;
                case 1:
                    pictureBoxCard2.Image = image;
                    return;
                case 2:
                    pictureBoxCard3.Image = image;
                    return;
                case 3:
                    pictureBoxCard4.Image = image;
                    return;
                case 4:
                    pictureBoxCard5.Image = image;
                    return;
                case 5:
                    pictureBoxCard6.Image = image;
                    return;
                case 6:
                    pictureBoxCard7.Image = image;
                    return;
                case 7:
                    pictureBoxCard8.Image = image;
                    return;
                case 8:
                    pictureBoxCard9.Image = image;
                    return;
                case 9:
                    pictureBoxCard10.Image = image;
                    return;
                case 10:
                    pictureBoxCard11.Image = image;
                    return;
                case 11:
                    pictureBoxCard12.Image = image;
                    return;
                default:
                    throw new Exception("Attempted to place board card image in non-existant board slot!");
            }
        }

        private void AssignCardSolutionImage(Image image, int solNum, int solCardNum)
        {
            switch(solNum)
            {
                case 1:
                    switch(solCardNum)
                    {
                        case 1:
                            pictureBoxs1c1.Image = image;
                            break;
                        case 2:
                            pictureBoxs1c2.Image = image;
                            break;
                        case 3:
                            pictureBoxs1c3.Image = image;
                            break;
                        default:
                            throw new Exception("Attempted to place solution image in non-existant solution cell!");
                    }
                    return;

                case 2:
                    switch (solCardNum)
                    {
                        case 1:
                            pictureBoxs2c1.Image = image;
                            break;
                        case 2:
                            pictureBoxs2c2.Image = image;
                            break;
                        case 3:
                            pictureBoxs2c3.Image = image;
                            break;
                        default:
                            throw new Exception("Attempted to place solution image in non-existant solution cell!");
                    }
                    return;

                case 3:
                    switch (solCardNum)
                    {
                        case 1:
                            pictureBoxs3c1.Image = image;
                            break;
                        case 2:
                            pictureBoxs3c2.Image = image;
                            break;
                        case 3:
                            pictureBoxs3c3.Image = image;
                            break;
                        default:
                            throw new Exception("Attempted to place solution image in non-existant solution cell!");
                    }
                    return;

                case 4:
                    switch (solCardNum)
                    {
                        case 1:
                            pictureBoxs4c1.Image = image;
                            break;
                        case 2:
                            pictureBoxs4c2.Image = image;
                            break;
                        case 3:
                            pictureBoxs4c3.Image = image;
                            break;
                        default:
                            throw new Exception("Attempted to place solution image in non-existant solution cell!");
                    }
                    return;

                case 5:
                    switch (solCardNum)
                    {
                        case 1:
                            pictureBoxs5c1.Image = image;
                            break;
                        case 2:
                            pictureBoxs5c2.Image = image;
                            break;
                        case 3:
                            pictureBoxs5c3.Image = image;
                            break;
                        default:
                            throw new Exception("Attempted to place solution image in non-existant solution cell!");
                    }
                    return;

                case 6:
                    switch (solCardNum)
                    {
                        case 1:
                            pictureBoxs6c1.Image = image;
                            break;
                        case 2:
                            pictureBoxs6c2.Image = image;
                            break;
                        case 3:
                            pictureBoxs6c3.Image = image;
                            break;
                        default:
                            throw new Exception("Attempted to place solution image in non-existant solution cell!");
                    }
                    return;

                default:
                    throw new Exception("Attempted to place solution image in non-existant solution row!");
            }
        }

        private void AssignCardSolutionBoardIndex(int boardIndex, int solNum, int solCardNum)
        {
            switch (solNum)
            {
                case 1:
                    switch (solCardNum)
                    {
                        case 1:
                            labels1c1.Text = string.Empty + boardIndex;
                            break;
                        case 2:
                            labels1c2.Text = string.Empty + boardIndex;
                            break;
                        case 3:
                            labels1c3.Text = string.Empty + boardIndex;
                            break;
                        default:
                            throw new Exception("Attempted to place solution board index in non-existant solution cell!");
                    }
                    return;

                case 2:
                    switch (solCardNum)
                    {
                        case 1:
                            labels2c1.Text = string.Empty + boardIndex;
                            break;
                        case 2:
                            labels2c2.Text = string.Empty + boardIndex;
                            break;
                        case 3:
                            labels2c3.Text = string.Empty + boardIndex;
                            break;
                        default:
                            throw new Exception("Attempted to place solution board index in non-existant solution cell!");
                    }
                    return;

                case 3:
                    switch (solCardNum)
                    {
                        case 1:
                            labels3c1.Text = string.Empty + boardIndex;
                            break;
                        case 2:
                            labels3c2.Text = string.Empty + boardIndex;
                            break;
                        case 3:
                            labels3c3.Text = string.Empty + boardIndex;
                            break;
                        default:
                            throw new Exception("Attempted to place solution board index in non-existant solution cell!");
                    }
                    return;

                case 4:
                    switch (solCardNum)
                    {
                        case 1:
                            labels4c1.Text = string.Empty + boardIndex;
                            break;
                        case 2:
                            labels4c2.Text = string.Empty + boardIndex;
                            break;
                        case 3:
                            labels4c3.Text = string.Empty + boardIndex;
                            break;
                        default:
                            throw new Exception("Attempted to place solution board index in non-existant solution cell!");
                    }
                    return;

                case 5:
                    switch (solCardNum)
                    {
                        case 1:
                            labels5c1.Text = string.Empty + boardIndex;
                            break;
                        case 2:
                            labels5c2.Text = string.Empty + boardIndex;
                            break;
                        case 3:
                            labels5c3.Text = string.Empty + boardIndex;
                            break;
                        default:
                            throw new Exception("Attempted to place solution board index in non-existant solution cell!");
                    }
                    return;

                case 6:
                    switch (solCardNum)
                    {
                        case 1:
                            labels6c1.Text = string.Empty + boardIndex;
                            break;
                        case 2:
                            labels6c2.Text = string.Empty + boardIndex;
                            break;
                        case 3:
                            labels6c3.Text = string.Empty + boardIndex;
                            break;
                        default:
                            throw new Exception("Attempted to place solution board index in non-existant solution cell!");
                    }
                    return;

                default:
                    throw new Exception("Attempted to place solution board index in non-existant solution row!");
            }
        }

        private void LoadBoard()
        {
            SetStatus("Loading...");
            SetStatusDetails("Loading board...");
            panelLoading.Show();
            backgroundWorkerLoadBoard.RunWorkerAsync();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadBoard();
        }

        private void backgroundWorkerLoadBoard_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;

            try
            {
                worker.ReportProgress(0, "Connecting to the Daily SET Puzzle servers...");
                var pageSource = FetchPageSource("http://www.puzzles.setgame.com/puzzle/set.htm", worker);

                worker.ReportProgress(0, "Parsing puzzle data...");
                var puzzleName = ParsePuzzleName(pageSource);
                var board = ParseBoard(pageSource);
                var solutions = ParseSolutions(pageSource);

                // Display Puzzle Name
                worker.ReportProgress(3, "Today's Puzzle - " + puzzleName);

                // Display solutions
                worker.ReportProgress(1, solutions);

                // Download board images
                worker.ReportProgress(0, "Downloading card images...");
                bool imagesFailedLoad = false;
                for (int i = 0; i < 12; i++)
                {
                    var card = board.cards[i];

                    string imageFileName = string.Empty;
                    if (card.number < 10)
                    {
                        imageFileName = "0" + card.number;
                    }
                    else
                    {
                        imageFileName += card.number;
                    }

                    string imageUrl = "http://www.puzzles.setgame.com/images/setcards/small/" + imageFileName + ".gif";
                    Image image;

                    worker.ReportProgress(0, "Downloading image " + imageUrl + " for board index " + i + "...");
                    try
                    {
                        var request = WebRequest.Create(imageUrl);
                        var dataStream = request.GetResponse().GetResponseStream();

                        worker.ReportProgress(0, "Downloading image " + imageFileName + "...");
                        image = Image.FromStream(dataStream);
                    }
                    catch (Exception)
                    {
                        image = null;
                        imagesFailedLoad = true;
                    }

                    if (image != null)
                    {
                        AssignCardImage((Image)image.Clone(), i);
                        for (int s = 0; s < 6; s++)
                        {
                            for (int c = 0; c < 3; c++)
                            {
                                var ca = solutions.sols[s].cards[c];
                                if (ca.number == i + 1)
                                {
                                    AssignCardSolutionImage((Image)image.Clone(), s + 1, c + 1);
                                    continue;
                                }
                            }
                        }
                    }
                }

                if(imagesFailedLoad)
                {
                    worker.ReportProgress(4);
                }
            }
            catch(Exception ex)
            {
                worker.ReportProgress(2, ex.Message);
            }
        }

        private void backgroundWorkerLoadBoard_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetStatusDetails("Finished! Here are the solutions for today's SET Puzzle:");
            panelLoading.Hide();
        }

        private void backgroundWorkerLoadBoard_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                SetStatusDetails((string)e.UserState);
            }
            else if(e.ProgressPercentage == 1)
            {
                SetSolutions sols = (SetSolutions)e.UserState;
                for(int s = 0; s < 6; s++)
                {
                    for(int c = 0; c < 3; c++)
                    {
                        var card = sols.sols[s].cards[c];

                        AssignCardSolutionBoardIndex(card.number, s + 1, c + 1);
                    }
                }
            }
            else if(e.ProgressPercentage == 2)
            {
                MessageBox.Show("Oh no - An error!" + Environment.NewLine + Environment.NewLine + (string)e.UserState + Environment.NewLine + Environment.NewLine + "The program will now exit because I don't have the time to handle every error case properly. Sorry! :(", "Whoops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            else if(e.ProgressPercentage == 3)
            {
                SetStatus((string)e.UserState);
            }
            else if(e.ProgressPercentage == 4)
            {
                MessageBox.Show("Some card images could not be downloaded!" + Environment.NewLine + Environment.NewLine + "Please make sure to check your connection and whatever." + Environment.NewLine + Environment.NewLine + "It is recommended that you restart the application to try again (because I don't have the time to handle these errors properly). Sorry!", "Heads up!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    public class SetCard
    {
        public int number;

        public SetCard()
        {
            number = -1;
        }
    }

    public class SetBoard
    {
        public SetCard[] cards;

        public SetBoard()
        {
            cards = new SetCard[12];
            for (int i = 0; i < 12; i++)
            {
                cards[i] = new SetCard();
            }
        }
    }

    public class SetSolution
    {
        public SetCard[] cards;

        public SetSolution()
        {
            cards = new SetCard[3];
            for (int i = 0; i < 3; i++)
            {
                cards[i] = new SetCard();
            }
        }
    }

    public class SetSolutions
    {
        public SetSolution[] sols;

        public SetSolutions()
        {
            sols = new SetSolution[6];
            for (int i = 0; i < 6; i++)
            {
                sols[i] = new SetSolution();
            }
        }
    }
}
