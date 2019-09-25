using System.Net;

namespace EVarlik.PushBullet
{
    public class PushBulletManager
    {
        public void SendPush()
        {
            var url = "https://api.pushbullet.com/v2/pushes";
            var request = (HttpWebRequest)WebRequest.Create(url);



        }

    }
}