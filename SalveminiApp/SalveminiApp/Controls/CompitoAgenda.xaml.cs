using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonkeyCache.SQLite;
using Xamarin.Forms;

namespace SalveminiApp.Controls
{
    public partial class CompitoAgenda : ContentView
    {
        public StackLayout layout;

        //Title
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(CompitoAgenda), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }

            set
            {
                SetValue(TitleProperty, value);
            }
        }

        //Description
        public static readonly BindableProperty DescProperty = BindableProperty.Create(nameof(Desc), typeof(string), typeof(CompitoAgenda), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string Desc
        {
            get
            {
                return (string)GetValue(DescProperty);
            }

            set
            {
                SetValue(DescProperty, value);
            }
        }

        //Color
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(FrameColor), typeof(string), typeof(WidgetGradient), default(string), Xamarin.Forms.BindingMode.OneWay);
        public string FrameColor
        {
            get
            {
                return (string)GetValue(ColorProperty);
            }

            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public CompitoAgenda(StackLayout layout_)
        {
            InitializeComponent();

            //Save parent layout
            layout = layout_;

            //Set dimensions
            closeFrame.WidthRequest = App.ScreenWidth * 0.058;
            closeFrame.HeightRequest = App.ScreenWidth * 0.058;
            closeFrame.CornerRadius = (float)(App.ScreenWidth * 0.058f) / 2;
            titleDesc.WidthRequest = App.ScreenWidth * 0.8;

        }

        //Update values
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            try
            {
                //Title
                if (propertyName == TitleProperty.PropertyName)
                {
                    TitleLbl.Text = Title;
                }

                //Description
                if (propertyName == DescProperty.PropertyName)
                {
                    DescLbl.HtmlText = Desc;
                }

                //Color
                if (propertyName == ColorProperty.PropertyName)
                {
                    view.BackgroundColor = Color.FromHex(FrameColor);
                    deleteButton.TextColor = Color.FromHex(FrameColor);
                }
            }
            catch
            {
                //Boh per sicurezza a volte fa cose strane
                return;
            }
        }

        public async void delete_Confirmed(object sender, EventArgs e)
        {
            try
            {
                //Get deleted compiti
                var deletedList = CacheHelper.GetCache<List<string>>("deletedCompiti");
                //If no compiti create a new list
                if (deletedList == null) deletedList = new List<string>();
                //Add a new object combining materia and compiti
                deletedList.Add(Title + Desc);
                //Add new list to cache
                Barrel.Current.Add<List<string>>("deletedCompiti", deletedList, TimeSpan.FromDays(100));
                //Remove element from list
                var animate = new Animation(d => this.HeightRequest = d, this.Height, 0);
                animate.Commit(hiddenBtn, "RemoveCompito", 16, 200);
                await Task.Delay(200);
                layout.Children.Remove(this);

            }
            catch
            {
                Costants.showToast("Non è stato possibile eliminare l'elemento");
            }

        }

        public void delete_Clicked(object sender, EventArgs e)
        {
            if (hiddenBtn.WidthRequest > 1)//Hide
                HideConfirm(300);
            if (hiddenBtn.WidthRequest < 1)//Show
                ShowConfirm(300);
        }

        public void frame_SwipedRight(object sender, SwipedEventArgs e)
        {
            if (hiddenBtn.WidthRequest > 1)//Hide
                HideConfirm(50);

        }
        public void frame_SwipedLeft(object sender, SwipedEventArgs e)
        {
            if (hiddenBtn.WidthRequest < 1)//Show
                ShowConfirm(50);
        }

        public async void ShowConfirm(uint speed)
        {
            var animate = new Animation(d => hiddenBtn.WidthRequest = d, hiddenBtn.Width, App.ScreenWidth / 4);
            animate.Commit(hiddenBtn, "ConfirmShow", 16, speed);
            await Task.Delay(TimeSpan.FromMilliseconds(speed)); await confirmTxt.FadeTo(1, speed);
        }

        public async void HideConfirm(uint speed)
        {
            await confirmTxt.FadeTo(0, speed);
            var animate = new Animation(d => hiddenBtn.WidthRequest = d, App.ScreenWidth / 4, 0);
            animate.Commit(hiddenBtn, "ConfirmHide", 16, speed);
        }

    }
}
