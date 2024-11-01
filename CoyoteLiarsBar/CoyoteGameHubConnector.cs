using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoyoteGameHubSDK
{
    public class CoyoteGameHubConnector
    {
        private string controllerUrl;
        private string clientId;

        private string EntryStrength;
        private string EntryFire;

        public CoyoteGameHubConnector(string connectCode)
        {
            // 初始化
            // 解析连接码
            var parts = connectCode.Split('@');
            if (parts.Length == 2)
            {
                clientId = parts[0];
                controllerUrl = parts[1];
            }
            else
            {
                clientId = "all";
                controllerUrl = connectCode;
            }

            EntryStrength = controllerUrl + "/api/v2/game/" + clientId + "/strength";
            EntryFire = controllerUrl + "/api/v2/game/" + clientId + "/action/fire";
        }

        public Task<HttpResponseMessage> SendHttpRequest(string url, string method, string data)
        {
            // 发送HTTP请求
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(new HttpMethod(method), url)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            var ret = httpClient.SendAsync(request);

            return ret;
        }

        /// <summary>
        /// 增加当前强度
        /// </summary>
        /// <param name="strength">要增加的强度值</param>
        public Task<HttpResponseMessage> AddStrength(int strength) => SendHttpRequest(EntryStrength, "POST", "strength.add=" + strength);

        /// <summary>
        /// 减少当前强度
        /// </summary>
        /// <param name="strength">要减少的强度值</param>
        public Task<HttpResponseMessage> SubStrength(int strength) => SendHttpRequest(EntryStrength, "POST", "strength.sub=" + strength);

        /// <summary>
        /// 设置当前强度
        /// </summary>
        /// <param name="strength">要设置的强度值</param>
        public Task<HttpResponseMessage> SetStrength(int strength) => SendHttpRequest(EntryStrength, "POST", "strength.set=" + strength);

        /// <summary>
        /// 增加随机强度
        /// </summary>
        /// <param name="strength">要增加的随机强度值</param>
        public Task<HttpResponseMessage> AddRandomStrength(int strength) => SendHttpRequest(EntryStrength, "POST", "randomStrength.add=" + strength);

        /// <summary>
        /// 减少随机强度
        /// </summary>
        /// <param name="strength">要减少的随机强度值</param>
        public Task<HttpResponseMessage> SubRandomStrength(int strength) => SendHttpRequest(EntryStrength, "POST", "randomStrength.sub=" + strength);

        /// <summary>
        /// 设置随机强度
        /// </summary>
        /// <param name="strength">要设置的随机强度值</param>
        public Task<HttpResponseMessage> SetRandomStrength(int strength) => SendHttpRequest(EntryStrength, "POST", "randomStrength.set=" + strength);

        /// <summary>
        /// 一键开火指令
        /// </summary>
        /// <param name="strength">一键开火强度</param>
        /// <param name="timeMs">持续时间（毫秒）</param>
        public Task<HttpResponseMessage> ActionFire(int strength, int timeMs) => SendHttpRequest(EntryFire, "POST", "strength=" + strength + "&time=" + timeMs);
    }
}
