using System;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using Plugin.Media;
using SalveminiApp.RestApi;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.IO;

namespace SalveminiApp.AreaVip
{
    public partial class CreaAvviso : ContentPage
    {
        List<ImagesToUpload> ImagesToUpload = new List<ImagesToUpload>();
        public string imageStatus = "view"; //delete or view

        public CreaAvviso()
        {
            InitializeComponent();
        }

        async void Crea_Clicked(object sender, System.EventArgs e)
        {
            string imageList = "";
            createBtn.IsEnabled = false;

            //Reset checks
            titoloEntry.ErrorText = "";

            //Checks
            if (string.IsNullOrEmpty(title.Text))
            {
                titoloEntry.HasError = true;
                titoloEntry.ErrorText = "Inserisci un titolo";
                createBtn.IsEnabled = true;
                return;
            }
            titoloEntry.HasError = false;

            //PROGRESS DIALOG
            using (IProgressDialog progress = UserDialogs.Instance.Progress("Caricamento avviso", null, null, true, MaskType.Black))
            {

                progress.PercentComplete = 10;

                //CARICAMENTO ALLEGATI
                if (ImagesToUpload != null)
                {
                    //ALLEGATI PROPORTION
                    progress.Title = "Caricamento allegati";
                    progress.PercentComplete = 15;

                    int imageIndex = 1;
                    foreach (ImagesToUpload currentImage in ImagesToUpload)
                    {
                        byte[] byteArray = File.ReadAllBytes(currentImage.mediaFile.Path);
                        Stream convertedStream = new MemoryStream(byteArray);
                        var nomeImmagine = (title.Text + Preferences.Get("UserId", 0).ToString() + DateTime.Now.ToString("yyyyMMddHHmmss")).Trim().Replace(' ', '_');
                        bool caricaImmagine = await App.Immagini.uploadImages(convertedStream, nomeImmagine, "avvisi");


                        if (caricaImmagine == true)
                        {
                            if (currentImage == ImagesToUpload[ImagesToUpload.Count - 1])
                            {
                                imageList = imageList + nomeImmagine;
                            }
                            else
                            {
                                imageList = imageList + nomeImmagine + ",";
                            }
                        }
                        else
                        {
                            await DisplayAlert("Ooops", "Si è verificato un errore durante il caricamento delle immagini, riprova o prova con una differente", "Ok");
                            createBtn.IsEnabled = true;

                            return;
                        }
                        progress.Title = "Caricamento allegati " + imageIndex + " di " + ImagesToUpload.Count;
                        progress.PercentComplete += 5;
                        imageIndex++;
                    }


                }
                else
                {
                    imageList = null;
                }

                //ALLEGATI PROPORTION
                progress.Title = "Caricamento avviso";
                progress.PercentComplete = 90;

                var avviso = new RestApi.Models.Avvisi();
                avviso.Immagini = imageList;

                avviso.Titolo = title.Text;
                avviso.Descrizione = desc.Text;
                avviso.SendNotification = pushSwitch.IsToggled;
                avviso.idCreatore = Preferences.Get("UserId", 0);

                //Upload avviso
                var result = await App.Avvisi.PostAvviso(avviso);
                await DisplayAlert(result[0], result[1], "Ok");
                if (result[0] == "Successo")
                    await Navigation.PopAsync();
            }

            createBtn.IsEnabled = true;
        }


        async void choosePhoto(object sender, System.EventArgs e)
        {
            if (ImagesToUpload.Count >= 10)
            {
                return;
            }
            else
            {
                MainPage.isSelectingImage = true;
                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        await DisplayAlert("Attenzione", "Non ci hai dato il permesso di accedere alle tue foto :(", "Ok");
                    MainPage.isSelectingImage = false;
                    return;
                    }
                    var choosenImage = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                    {
                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                        CompressionQuality = 80

                    });

                    if (choosenImage == null)
                    {
                    MainPage.isSelectingImage = false;
                    return;
                    }

                var choosenImageToUpload = new ImagesToUpload();


                choosenImageToUpload.imageSource = ImageSource.FromStream(() =>
                {
                    var streams = choosenImage.GetStreamWithImageRotatedForExternalStorage();
                    choosenImageToUpload.mediaFile = choosenImage;
                    return streams;
                });

                ImagesToUpload.Add(choosenImageToUpload);
                imageAttach.FlowItemsSource = null;
                imageAttach.FlowItemsSource = ImagesToUpload;
                MainPage.isSelectingImage = false;
            }
        }

        void detailedPicture(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var button = sender as DLToolkit.Forms.Controls.FlowListView;
            ImagesToUpload selectedImage = button.FlowLastTappedItem as ImagesToUpload;
            if (imageStatus == "delete")
            {
                var findImage = ImagesToUpload.Where(x => x == selectedImage).ToList();
                ImagesToUpload.Remove(findImage[0]);
                imageAttach.FlowItemsSource = null;
                imageAttach.FlowItemsSource = ImagesToUpload;
            }
            if (imageStatus == "view")
            {
                //   Navigation.PushPopupAsync(new PopUps.FullImage(selectedImage.imageSource));
            }
        }

        async void deleteOrView(object sender, System.EventArgs e)
        {
            bool canAnimate = true;

            //DELETE
            if (imageStatus == "delete" && canAnimate == true)
            {
                //DELETE
                canAnimate = false;
                await deleteView.ScaleTo(0.8, 150);
                deleteView.Text = "fas-trash";
                deleteView.TextColor = Color.FromHex("e80000");
                await deleteView.ScaleTo(1, 150);
                imageStatus = "view";
                canAnimate = true;
            }
            else if (imageStatus == "view" && canAnimate == true)
            {
                //VIEW
                canAnimate = false;
                await deleteView.ScaleTo(0.8, 150);
                deleteView.Text = "fas-eye";
                deleteView.TextColor = Styles.Apple;
                await deleteView.ScaleTo(1, 150);
                imageStatus = "delete";
                canAnimate = true;
            }
        }


    }
}
