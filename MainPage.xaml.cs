using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FlashCard
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        //List of question that will display in the card
        private readonly List<(string Question, string Answer, string Image, string Theme)> flashCards = new()
        {
            ("1. What is the capital of Japan?", "Tokyo", "japan.png", "Geography"),
            ("2. Who wrote 'Romeo and Juliet'?", "William Shakespeare", "shakes.png", "Literature"),
            ("3. What planet is known as the Red Planet?", "Mars", "solar.png", "Space"),
            ("4. In which country did the Olympic Games originate?", "Greece", "olympic.png", "History"),
            ("5. Who was the first person to step on the Moon?", "Neil Armstrong", "astro.png", "Astronomy"),
            ("6. Which European country is famous for the Colosseum?", "Italy", "rome.png", "Geography"),
            ("7. Who invented the light bulb?", "Thomas Edison", "bulb.png", "History"),
            ("8. Who painted the Mona Lisa?", "Leonardo da Vinci", "lisa.png", "Art"),
            ("9. Who painted “Starry Night”?", "Vincent van Gogh", "night.png", "Art"),
            ("10. Who is the author of “Pride and Prejudice”?", "Jane Austen", "pride.png", "Literature")
        };
        //Shows the flashcard behavior
        private int current;
        private bool _isShowingAnswer;
        private readonly Random _random = new();

        //Display flashcard contents
        private string _displayText;
        private string _displayImage;
        private string _topLabel;

        //Bound to UI for showing question/answer
        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnPropertyChanged();
                }
            }
        }
        //Bound to UI for showing flashcard image
        public string DisplayImage
        {
            get => _displayImage;
            set
            {
                if (_displayImage != value)
                {
                    _displayImage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasImage));
                }
            }
        }
        //Bound to UI for top label
        public string TopLabel
        {
            get => _topLabel;
            set
            {
                if (_topLabel != value)
                {
                    _topLabel = value;
                    OnPropertyChanged();
                }
            }
        }
        //Used to toggle image visibility in the UI
        public bool HasImage => !string.IsNullOrEmpty(DisplayImage);

        //The Commands form the Xaml
        public ICommand NextCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ShuffleCommand { get; private set; }
        public ICommand FlipCommand { get; private set; }
        
        //Loads all UI elements from XAML. And initialize commans
        public MainPage()
        {
            InitializeComponent();

            NextCommand = new Command(NextCard);
            BackCommand = new Command(PreviousCard);
            ShuffleCommand = new Command(ShuffleCard);
            FlipCommand = new Command(FlipCard);

            BindingContext = this;

            current = 0;
            _isShowingAnswer = false;
            UpdateCardDisplay();
        }

        //Always show the image, question and answer. Also the top label  
        private void UpdateCardDisplay()
        {
            DisplayText = _isShowingAnswer ? flashCards[current].Answer : flashCards[current].Question;

            DisplayImage = flashCards[current].Image;

            TopLabel = flashCards[current].Theme;
        }
        //Show the next flashcard in the list
        private void NextCard()
        {
            current = (current + 1) % flashCards.Count;
            _isShowingAnswer = false;
            UpdateCardDisplay();
        }
        //Show the previous flashcard in the list
        private void PreviousCard()
        {
            current = (current - 1 + flashCards.Count) % flashCards.Count;
            _isShowingAnswer = false;
            UpdateCardDisplay();
        }
        //Shuffle: jump to a random flashcard
        private void ShuffleCard()
        {
            current = _random.Next(flashCards.Count);
            _isShowingAnswer = false;
            UpdateCardDisplay();
        }
        //Flip the current flashcard (toggle between question and answer)
        private void FlipCard()
        {
            _isShowingAnswer = !_isShowingAnswer;
            UpdateCardDisplay();
        }

        //Handles card flipping animation when tapped
        private async void OnFlipAnimation(object sender, EventArgs e)
        {
            await CardBorder.RotateYTo(90, 150, Easing.CubicIn);
            FlipCard();
            await CardBorder.RotateYTo(0, 150, Easing.CubicOut);
        }
        //For INotifyPropertyChanged to notify UI about data updates
        public event PropertyChangedEventHandler PropertyChanged;

        //Helper method to raise PropertyChanged events
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
