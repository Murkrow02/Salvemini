using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CoreGraphics;
using Foundation;
using Intents;
using IntentsUI;
using Newtonsoft.Json;
using UIKit;

namespace SalveminiAppIntentUI
{
    // As an example, this extension's Info.plist has been configured to handle interactions for INSendMessageIntent.
    // You will want to replace this or add other intents as appropriate.
    // The intents whose interactions you wish to handle must be declared in the extension's Info.plist.

    // You can test this example integration by saying things to Siri like:
    // "Send a message using <myApp>"
    public partial class IntentViewController : UIViewController, IINUIHostedViewControlling
    {
        protected IntentViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Do any required interface initialization here.
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        List<RestApi.Models.Treno> Treni = new List<RestApi.Models.Treno>();
        List<RestApi.Models.Lezione> Lezioni = new List<RestApi.Models.Lezione>();

        void getOrario()
        {
            //Get direction and station
            var defaults = new NSUserDefaults("group.com.codex.SalveminiApp", NSUserDefaultsType.SuiteName);
            defaults.AddSuite("group.com.codex.SalveminiApp");
            var classe = defaults.StringForKey(new NSString("SiriClass"));

            //Download strings
            WebClient wc = new WebClient();
            var json = wc.DownloadString("https://www.mysalvemini.me/api/orari/siri/oggi/" + classe);
            Lezioni = JsonConvert.DeserializeObject<List<RestApi.Models.Lezione>>(json);
        }

        void getTrains()
        {
            //Get direction and station
            var defaults = new NSUserDefaults("group.com.codex.SalveminiApp", NSUserDefaultsType.SuiteName);
            defaults.AddSuite("group.com.codex.SalveminiApp");
            var station = defaults.IntForKey(new NSString("SiriStation"));
            var direction = defaults.BoolForKey(new NSString("SiriDirection"));

            //Download strings
            WebClient wc = new WebClient();
            var json = wc.DownloadString("https://www.mysalvemini.me/api/orari/siri/" + station + "/" + direction + "/false");
            Treni = JsonConvert.DeserializeObject<List<RestApi.Models.Treno>>(json);
        }

        public void Configure(INInteraction interaction, INUIHostedViewContext context, Action<CGSize> completion)
        {
            // Do configuration here, including preparing views and calculating a desired size for presentation.
            if (interaction.Intent.GetType() == new SalveminiApp.TrenoIntent().GetType())
            {
                getTrains();

                var mainLayout = new UIStackView();

                //Title view
                var titleLayout = new UIStackView();

                var titlespacingview = new UIView();
                titleLayout.AddArrangedSubview(titlespacingview);

                var title = new UILabel { Text = "A seguire", Font = UIFont.BoldSystemFontOfSize(26), TextAlignment = UITextAlignment.Left };
                titleLayout.AddArrangedSubview(title);

                titleLayout.Axis = UILayoutConstraintAxis.Horizontal;
                titleLayout.TranslatesAutoresizingMaskIntoConstraints = false;
                titleLayout.Spacing = 10;

                mainLayout.AddArrangedSubview(titleLayout);



                var trainLayout = new UIStackView();

                //First Train
                bool firstExists = Treni.ElementAtOrDefault(0) != null;

                var firstlayout = new UIStackView();

                var firststartspacingview = new UIView();

                var firsticon = new UIImageView(new UIImage("train.png"));

                var firstlabel = new UILabel { Text = firstExists ? Treni[0].Partenza : null, Font = UIFont.SystemFontOfSize(19), TextAlignment = UITextAlignment.Left };

                var firstvariations = new UILabel { Text = firstExists ? Treni[0].convertPriority : null, Font = UIFont.SystemFontOfSize(12), TextColor = new UIColor(0.8f, 0.8f, 0.8f, 1), TextAlignment = UITextAlignment.Right };

                var firstendspacingview = new UIView();

                if (firstExists)
                {
                    firstlayout.AddArrangedSubview(firststartspacingview);
                    firstlayout.AddArrangedSubview(firsticon);
                    firstlayout.AddArrangedSubview(firstlabel);
                    firstlayout.AddArrangedSubview(firstvariations);
                    firstlayout.AddArrangedSubview(firstendspacingview);

                    //FirstTrainLayout settings
                    firstlayout.Axis = UILayoutConstraintAxis.Horizontal;
                    firstlayout.TranslatesAutoresizingMaskIntoConstraints = false;
                    firstlayout.Spacing = 10;

                    //Add FirstTrainLayout to view
                    trainLayout.AddArrangedSubview(firstlayout);
                }

                //Second Train
                bool secondExists = Treni.ElementAtOrDefault(1) != null;

                var secondlayout = new UIStackView();

                var secondstartspacingview = new UIView();

                var secondicon = new UIImageView(new UIImage("train.png"));

                var secondlabel = new UILabel { Text = secondExists ? Treni[1].Partenza : null, Font = UIFont.SystemFontOfSize(19), TextAlignment = UITextAlignment.Left };

                var secondvariations = new UILabel { Text = secondExists ? Treni[1].convertPriority : null, Font = UIFont.SystemFontOfSize(12), TextColor = new UIColor(0.8f, 0.8f, 0.8f, 1), TextAlignment = UITextAlignment.Right };

                var secondendspacingview = new UIView();

                if (secondExists)
                {
                    secondlayout.AddArrangedSubview(secondstartspacingview);
                    secondlayout.AddArrangedSubview(secondicon);
                    secondlayout.AddArrangedSubview(secondlabel);
                    secondlayout.AddArrangedSubview(secondvariations);
                    secondlayout.AddArrangedSubview(secondendspacingview);

                    //SecondTrainLayout settings
                    secondlayout.Axis = UILayoutConstraintAxis.Horizontal;
                    secondlayout.TranslatesAutoresizingMaskIntoConstraints = false;
                    secondlayout.Spacing = 10;

                    //Add SecondTrainLayout to view
                    trainLayout.AddArrangedSubview(secondlayout);
                }

                //Third Train
                bool thirdExists = Treni.ElementAtOrDefault(2) != null;

                var thirdlayout = new UIStackView();

                var thirdstartspacingview = new UIView();

                var thirdicon = new UIImageView(new UIImage("train.png"));

                var thirdlabel = new UILabel { Text = thirdExists ? Treni[2].Partenza : null, Font = UIFont.SystemFontOfSize(19), TextAlignment = UITextAlignment.Left };

                var thirdvariations = new UILabel { Text = thirdExists ? Treni[2].convertPriority : null, Font = UIFont.SystemFontOfSize(12), TextColor = new UIColor(0.8f, 0.8f, 0.8f, 1), TextAlignment = UITextAlignment.Right };

                var thirdendspacingview = new UIView();

                if (thirdExists)
                {
                    thirdlayout.AddArrangedSubview(thirdstartspacingview);
                    thirdlayout.AddArrangedSubview(thirdicon);
                    thirdlayout.AddArrangedSubview(thirdlabel);
                    thirdlayout.AddArrangedSubview(thirdvariations);
                    thirdlayout.AddArrangedSubview(thirdendspacingview);

                    //SecondTrainLayout settings
                    thirdlayout.Axis = UILayoutConstraintAxis.Horizontal;
                    thirdlayout.TranslatesAutoresizingMaskIntoConstraints = false;
                    thirdlayout.Spacing = 10;

                    //Add SecondTrainLayout to view
                    trainLayout.AddArrangedSubview(thirdlayout);
                }

                //Train Layout Settings
                trainLayout.Axis = UILayoutConstraintAxis.Vertical;
                trainLayout.TranslatesAutoresizingMaskIntoConstraints = false;
                trainLayout.Spacing = 15;

                //Add TrainLayout to view
                mainLayout.AddArrangedSubview(trainLayout);


                //Main Layout Settings
                mainLayout.Axis = UILayoutConstraintAxis.Vertical;
                mainLayout.Spacing = 20;
                mainLayout.TranslatesAutoresizingMaskIntoConstraints = false;

                //Add Main layout to view
                View.AddSubview(mainLayout);

                if (firstExists)
                {
                    firststartspacingview.WidthAnchor.ConstraintEqualTo(1).Active = true;
                    firstendspacingview.WidthAnchor.ConstraintEqualTo(1).Active = true;

                    firsticon.WidthAnchor.ConstraintEqualTo(22).Active = true;
                    firsticon.HeightAnchor.ConstraintEqualTo(22).Active = true;

                    firstlayout.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
                    firstlayout.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
                }

                if (secondExists)
                {
                    secondstartspacingview.WidthAnchor.ConstraintEqualTo(1).Active = true;
                    secondendspacingview.WidthAnchor.ConstraintEqualTo(1).Active = true;

                    secondicon.WidthAnchor.ConstraintEqualTo(22).Active = true;
                    secondicon.HeightAnchor.ConstraintEqualTo(22).Active = true;

                    secondlayout.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
                    secondlayout.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
                }

                if (thirdExists)
                {
                    thirdstartspacingview.WidthAnchor.ConstraintEqualTo(1).Active = true;
                    thirdendspacingview.WidthAnchor.ConstraintEqualTo(1).Active = true;

                    thirdicon.WidthAnchor.ConstraintEqualTo(22).Active = true;
                    thirdicon.HeightAnchor.ConstraintEqualTo(22).Active = true;

                    thirdlayout.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
                    thirdlayout.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
                }

                titlespacingview.WidthAnchor.ConstraintEqualTo(1).Active = true;

                titleLayout.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
                titleLayout.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;

                trainLayout.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
                trainLayout.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;

                mainLayout.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
                mainLayout.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;

                completion(DesiredSize());
            }
            else
            {
                getOrario();

                if (Lezioni == null || DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                {
                    return;
                }

                var colori = new Dictionary<string, UIColor>();

                colori.Add(Lezioni.ElementAtOrDefault(0) != null && !colori.ContainsKey(Lezioni[0].Materia) ? Lezioni[0].Materia : "nil0", new UIColor(red: 0.49f, green: 0.47f, blue: 1.00f, alpha: 1.0f));
                colori.Add(Lezioni.ElementAtOrDefault(1) != null && !colori.ContainsKey(Lezioni[1].Materia) ? Lezioni[1].Materia : "nil1", new UIColor(red: 0.36f, green: 0.69f, blue: 0.90f, alpha: 1.0f));
                colori.Add(Lezioni.ElementAtOrDefault(2) != null && !colori.ContainsKey(Lezioni[2].Materia) ? Lezioni[2].Materia : "nil2", new UIColor(red: 1.00f, green: 0.73f, blue: 0.31f, alpha: 1.0f));
                colori.Add(Lezioni.ElementAtOrDefault(3) != null && !colori.ContainsKey(Lezioni[3].Materia) ? Lezioni[3].Materia : "nil3", new UIColor(red: 1.00f, green: 0.44f, blue: 0.39f, alpha: 1.0f));
                colori.Add(Lezioni.ElementAtOrDefault(4) != null && !colori.ContainsKey(Lezioni[4].Materia) ? Lezioni[4].Materia : "nil4", new UIColor(red: 0.92f, green: 0.35f, blue: 0.92f, alpha: 1.0f));
                colori.Add(Lezioni.ElementAtOrDefault(5) != null && !colori.ContainsKey(Lezioni[5].Materia) ? Lezioni[5].Materia : "nil5", new UIColor(red: 0.28f, green: 0.92f, blue: 0.60f, alpha: 1.0f));
                colori.Add(Lezioni.ElementAtOrDefault(6) != null && !colori.ContainsKey(Lezioni[6].Materia) ? Lezioni[6].Materia : "nil6", new UIColor(red: 0.28f, green: 0.92f, blue: 0.60f, alpha: 1.0f));

                int increment = 40;
                Add(new UILabel(new CGRect(View.Bounds.X + 13, View.Bounds.Y + 5, View.Bounds.Width, 25)) { Text = "Invece oggi", Font = UIFont.BoldSystemFontOfSize(20), TextAlignment = UITextAlignment.Left, TextColor = new UIColor(red: 0.24f, green: 0.18f, blue: 0.18f, alpha: 1.0f) });
                foreach (var lesson in Lezioni)
                {
                    var cell = new UIView(new CGRect((View.Bounds.Width / 2) - ((View.Bounds.Width * 0.47) / 1), View.Bounds.Y + increment, View.Bounds.Width * 0.9f, 30 * lesson.numOre));
                    cell.BackgroundColor = colori[lesson.Materia];
                    cell.Layer.CornerRadius = lesson.numOre > 1 ? 10 : 7;
                    var label = new UILabel(new CGRect(View.Bounds.X + 5, (cell.Bounds.Height / 2) - 15, View.Bounds.Width * 0.9f, 30)) { Text = lesson.Materia, TextColor = UIColor.White, Font = UIFont.SystemFontOfSize(15), TextAlignment = UITextAlignment.Left };
                    cell.Add(label);
                    Add(cell);
                    increment += (5 + 30 * lesson.numOre);
                }

                completion(DesiredSize(increment));
            }



        }

        CGSize DesiredSize(float size = 180)
        {
            return new CGSize(this.ExtensionContext?.GetHostedViewMaximumAllowedSize().Width ?? 320, size);
        }
    }

    public class TableSource : UITableViewSource
    {

        string[] TableItems;
        string CellIdentifier = "TableCell";

        public TableSource(string[] items)
        {
            TableItems = items;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return TableItems.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
            string item = TableItems[indexPath.Row];

            //---- if there are no cells to reuse, create a new one
            if (cell == null)
            { cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier); }

            cell.TextLabel.Text = item;

            return cell;
        }
    }
}
