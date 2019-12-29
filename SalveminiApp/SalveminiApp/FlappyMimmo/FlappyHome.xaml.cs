using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace SalveminiApp.FlappyMimmo
{
    public partial class FlappyHome : ContentPage
    {
        public FlappyHome()
        {
            InitializeComponent();

            //Save Flappy default image in documents
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            byte[] imageBytes;
            for (int i = 1; i < 4; i++)
            {
                var filename = Path.Combine(documents, "default" + i + ".png");
                bool doesExist = File.Exists(filename);

                if (!doesExist)
                {
                    var image = new Image { Source = "default" + i + ".png" };
                    StreamImageSource streamImageSource = (StreamImageSource)image.Source;
                    System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
                    Task<Stream> task = streamImageSource.Stream(cancellationToken);
                    Stream stream = task.Result;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        imageBytes = ms.ToArray();
                    }
                    File.WriteAllBytes(filename, imageBytes);
                }

            }

            //Set Sizes
            buttonFrame.WidthRequest = App.ScreenWidth / 6;
            buttonFrame.HeightRequest = App.ScreenWidth / 6;
            buttonFrame.CornerRadius = (float)(App.ScreenWidth / 6) / 2;
            singleImage.WidthRequest = App.ScreenWidth * 0.35;
            singleImage.HeightRequest = App.ScreenWidth * 0.35;
            boardImage.WidthRequest = App.ScreenWidth * 0.58;
            boardImage.HeightRequest = App.ScreenWidth * 0.35;
        }

        void Play_Tapped(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new GamePage());
        }

        void Shop_Tapped(object sender, EventArgs e)
        {
            Navigation.PushPopupAsync(new Shop());
        }

        void Close_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
