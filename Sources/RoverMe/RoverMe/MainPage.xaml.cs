using RoverMe.Shared.Commands;
using RoverMe.Shared.Network;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RoverMe
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ControllerCommand Controller { get; set; }
        public MainPage()
        {
            this.InitializeComponent();

        }


        //socketClient = SocketClient.Instance
        //        var controllerCommand = new ControllerCommand(socketClient);

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Controller = new ControllerCommand(SocketClient.Instance);
        }
    }
}
