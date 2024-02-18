namespace Flyingdarts.Backend.Games.X01.Api.Requests.WebRPC
{
    public class RTCIceCandidate
    {
        public string Address { get; set; }
        public string Candidate { get; set; }
        public string Component { get; set; }
        public string Foundation { get; set; }
        public int Port { get; set; }
        public long Priority { get; set; }
        public string Protocol { get; set; }
        public string RelatedAddress { get; set; }
        public int RelatedPort { get; set; }
        public int SdpMLineIndex { get; set; }
        public string SdpMid { get; set; }
        public string TcpType { get; set; }
        public string Type { get; set; }
        public string UsernameFragment { get; set; }

        public RTCIceCandidate() { }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static RTCIceCandidate FromJson(string json)
        {
            return JsonSerializer.Deserialize<RTCIceCandidate>(json)!;
        }
    }
}
