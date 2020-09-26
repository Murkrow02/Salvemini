using System;
using System.Collections.Generic;
using CocosSharp;
using Xamarin.Forms;

namespace SalveminiApp.FlappyMimmo
{
    public class GameOverLayer : CCLayerColor
    {

        //string scoreMessage = string.Empty;
        CCLabel scoreLabel;
        public GameOverLayer(int score) : base(CCColor4B.Black)
        {
            scoreLabel = new CCLabel(String.Format("Punteggio: {0}", score), "Arial", 40, CCLabelFormat.SystemFont);
            scoreLabel.Color = CCColor3B.White;
            AddChild(scoreLabel);
        }

        CCLabel homeLabel = new CCLabel("Torna alla home", "Arial", 40, CCLabelFormat.SystemFont);

        protected override void AddedToScene()
        {
            base.AddedToScene();

            var background = new CCSprite("textGameOver");
            background.Position = new CCPoint(ContentSize.Width / 2, ContentSize.Height / 2);
            AddChild(background);
            scoreLabel.Position = new CCPoint(ContentSize.Width / 2, ContentSize.Height / 2 + background.ContentSize.Height);

            homeLabel.Color = CCColor3B.White;
            homeLabel.Position = new CCPoint(VisibleBoundsWorldspace.MaxX / 2, 140);
            AddChild(homeLabel);

            var startLabel = new CCLabel("Tocca per ricominciare", "Arial", 40, CCLabelFormat.SystemFont);
            startLabel.Color = CCColor3B.White;
            startLabel.Position = new CCPoint(ContentSize.Width / 2, ContentSize.Height / 2 - background.ContentSize.Height);
            AddChild(startLabel);

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesBegan = OnTouchesBegan;
            AddEventListener(touchListener, this);
        }

        private void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                foreach (var touch in touches)
                {
                    CCScene newGameScene = new CCScene(GameView);
                    var transitionToNewGame = new CCTransitionProgressInOut(0.7f, newGameScene);
                    if (homeLabel.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                    {
                        MessagingCenter.Send<App>((App)Application.Current, "CloseGame");
                    }
                    else
                    {
                        newGameScene.AddLayer(new SingleGameLayer());
                    }
                    Director.ReplaceScene(transitionToNewGame);
                }
            }
        }
    }
}
