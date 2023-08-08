using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora_App.Models
{
    public class AgoraModel
    {
        public string appid { get; set; }
        public string cname { get; set; }
        public string uid { get; set; }
        public string ip { get; set; }
        public string time { get; set; }
        public string time_in_seconds { get; set; }
        public string[]  privileges { get; set; }
    }
    public class Priviledges
    {
        public string join_channel { get; set; }
        public string publish_audio { get; set; }
        public string publish_video { get; set; }
    }
}
