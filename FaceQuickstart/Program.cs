using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
namespace FaceQuickstart
{
    internal class Program
    {
        //public static _faceServiceClient;
        public static ObservableCollection<Face> list = new ObservableCollection<Face>();
        static void Main(string[] args)
        {


            //IFaceClient client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);
            string path = "C:/Users/ltorres/Documents/face.png";
            Object ts = FaceDetect(path);
        }
        public static async Task FaceDetect(string image)
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
                IFaceServiceClient _faceServiceClient = new FaceServiceClient("0979034b298f4f0cb5007e1e355fbaa5",
                "https://facedetecte.cognitiveservices.azure.com/");

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
                //await DisplayAlert("Error", f.ErrorMessage, "ok");
                Console.WriteLine(f.Message);
                Console.ReadKey();
            }
            // Catch and display all other errors.
            catch (Exception e)
            {
               // await DisplayAlert("Error", e.Message, "ok");
               Console.WriteLine(e.ToString());
                Console.ReadKey();
            }
        }
    }
}
