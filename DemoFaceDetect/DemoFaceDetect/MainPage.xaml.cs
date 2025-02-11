﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DemoFaceDetect
{
    public partial class MainPage : ContentPage
    {
        private readonly IFaceServiceClient _faceServiceClient;
        ObservableCollection<Face> list = new ObservableCollection<Face>();

        public MainPage()
        {
            InitializeComponent();

            _faceServiceClient = new FaceServiceClient("0979034b298f4f0cb5007e1e355fbaa5",
                "https://facedetecte.cognitiveservices.azure.com/face/v1.0/");

            //IFaceClient client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);
            list = new ObservableCollection<Face>();
            FacesListView.ItemsSource = list;

        }

        private async void btnGetFoto_Clicked(object sender, EventArgs e)
        {
            var file = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Please pick photo"
            });

            var stream = await file.OpenReadAsync();

            // MinhaImagem.Source = ImageSource.FromStream(() => stream);
            await FaceDetect(file.FullPath);

            MinhaImagem.Source = ImageSource.FromStream(() =>
            {
                var stream2 = file.OpenReadAsync();
                stream2.Dispose();

                return stream;

            });

        }
        private async void TakePicture(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
            {
                await DisplayAlert("Ops", "Nenhuma câmera detectada.", "OK");

                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(
                new StoreCameraMediaOptions
                {
                    CompressionQuality = 75,
                    CustomPhotoSize = 50,
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 2000,
                    DefaultCamera = CameraDevice.Front,
                    SaveToAlbum = false,
                    Directory = "Demo"

                });

            if (file == null)
                return;

            await FaceDetect(file.AlbumPath);

            MinhaImagem.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();

                
                return stream;

            });
        }

        public async Task FaceDetect(string image)
        {
            IEnumerable<FaceAttributeType> faceAttributes =
                new FaceAttributeType[]
                {
                    FaceAttributeType.Gender,
                    FaceAttributeType.Age,
                    FaceAttributeType.Smile,
                    FaceAttributeType.Emotion,
                    FaceAttributeType.Glasses,
                };

            list.Clear();

            // Call the Face API.
            try
            {
                using (Stream imageFileStream = File.OpenRead(image))
                {
                     var faces = await _faceServiceClient.DetectAsync(imageFileStream,
                        returnFaceId: true,
                        returnFaceLandmarks: false,
                        returnFaceAttributes: faceAttributes);

                    //Add Faces in List
                    foreach (var face in faces)
                    {
                        list.Add(face);
                    }
                    
                }
            }
            // Catch and display Face API errors.
            catch (FaceAPIException f)
            {
                await DisplayAlert("Error", f.ErrorMessage, "ok");
            }
            // Catch and display all other errors.
            catch (Exception e)
            {
                await DisplayAlert("Error", e.Message, "ok");
            }
        }
    }
}
