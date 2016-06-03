using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TextToSpeech
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void btnTexttoSpeech_Click(object sender, RoutedEventArgs e)
        {
            readText(xContent.Text);
        }

        private async void readText(string text)
        {
            var voice = SpeechSynthesizer.AllVoices;

            using (var speech = new SpeechSynthesizer())
            {
                speech.Voice = voice.First(gender => gender.Gender == VoiceGender.Male);
                var stock = await speech.SynthesizeTextToStreamAsync(text);
                audioPlayer.SetSource(stock, stock.ContentType);
                audioPlayer.Play();
            }
        }
    }
}
