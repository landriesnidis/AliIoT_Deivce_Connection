using AliIoT_Deivce_Connection.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;

namespace AliIoT_Deivce_Connection
{
    public class AliIoTDeviceClient : MqttClient
    {
        private string ProductKey { get; set; }
        private string DeviceName { get; set; }
        private string DeviceSecret { get; set; }
        private string RegionId { get; set; }

        private string mqttUserName;
        private string mqttPassword;
        private string mqttClientId;

        public AliIoTDeviceClient(string productKey,string deviceName,string deviceSecret,string regionId = "cn-shanghai"):base(productKey + ".iot-as-mqtt." + regionId + ".aliyuncs.com")
        {
            ProductKey = productKey;
            DeviceName = deviceName;
            DeviceSecret = deviceSecret;
            RegionId = regionId;
        }

        public void Connect()
        {
            // 
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            // 
            string clientId = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            // 
            string timestamp = Convert.ToString(DateTimeOffset.Now.Millisecond);
            // 加密方式
            string signmethod = "hmacmd5";

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("productKey", ProductKey);
            dict.Add("deviceName", DeviceName);
            dict.Add("clientId", clientId);
            dict.Add("timestamp", timestamp);

            mqttUserName = DeviceName + "&" + ProductKey;
            mqttPassword = SignUtils.Sign(dict, DeviceSecret, signmethod);
            mqttClientId = clientId + "|securemode=3,signmethod=" + signmethod + ",timestamp=" + timestamp + "|";

            ProtocolVersion = MqttProtocolVersion.Version_3_1_1;
            Connect(mqttClientId, mqttUserName, mqttPassword, false, 60);
        }


    }
}
