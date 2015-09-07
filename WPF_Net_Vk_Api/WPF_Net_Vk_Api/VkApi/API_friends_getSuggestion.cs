using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WPF_Net_Vk_Api.VkApi
{
    class API_friends_getSuggestion : ApiBase
    {
        public List<Person> response { get; set; }

        public static API_friends_getSuggestion GetRespounce(string accessToken)
        {
            string code = "return API.friends.getSuggestions({filter:\"mutual\", count : 500, fields : \"photo_50, online\"}).items;";
            string answer = Execute(accessToken, code);
            return JsonConvert.DeserializeObject<API_friends_getSuggestion>(answer);
        }
    }
}
