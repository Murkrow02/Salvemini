using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using CocosSharp;


namespace SalveminiApp.FlappyMimmo
{
    public class GamePage : ContentPage
    {
        CocosSharpView gameView;

        public GamePage()
        {
            BackgroundColor = Color.Black;
            gameView = new CocosSharpView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                // Set the game world dimensions
                DesignResolution = new Size(750, 1400),
                // Set the method to call once the view has been initialised
                ViewCreated = LoadGame
            };

            Content = gameView;

            MessagingCenter.Subscribe<App>(this, "CloseGame", (sender) => { Navigation.PopModalAsync(); });
        }

        protected override void OnDisappearing()
        {
            if (gameView != null)
            {
                gameView.Paused = true;
            }

            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (gameView != null)
                gameView.Paused = false;
        }

        void LoadGame(object sender, EventArgs e)
        {
            var nativeGameView = sender as CCGameView;

            if (nativeGameView != null)
            {
                var contentSearchPaths = new List<string>();
                CCSizeI viewSize = nativeGameView.ViewSize;
                CCSizeI designResolution = nativeGameView.DesignResolution;
                nativeGameView.ResolutionPolicy = CCViewResolutionPolicy.NoBorder;
                contentSearchPaths.Add("Images");

                CCAudioEngine.SharedEngine.BackgroundMusicVolume = 0.5f;

                nativeGameView.ContentManager.SearchPaths = contentSearchPaths;

                CCScene gameScene = new CCScene(nativeGameView);
                gameScene.AddLayer(new SingleGameLayer());
                nativeGameView.RunWithScene(gameScene);
            }
        }
    }
}
