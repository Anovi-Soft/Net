using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace WPF_Net_Vk_Api.VkApi
{
    class AsyncListOfPotencialFriends : ApiBase
    {
        public class ExecuteClass
        {
            public Dictionary<string, int> response { get; set; }
        }
        private string accessToken;

        private Thread thread;
        public bool IsWork
        {
            get
            {
                return thread != null;
            }
        }
        public bool IsEmpty
        {
            get
            {
                return lstItems.Count == 0;
            }
        }

        private static object _syncLst = new object(); 
        private List<Person> lstItems = new List<Person>();
        public Person[] LstItems
        {
            get { lock (_syncLst) 
            {
                Person[] tmp = new Person[lstItems.Count];
                lstItems.CopyTo(tmp);
                lstItems.Clear(); 
                return tmp; 
            }}
        }

        public ExecuteClass GetMutual(List<Person> mutual)
        {
            string code = "return {";
            foreach (Person id in mutual)
                code += String.Format("\"{0}\":API.users.get(<user_ids : \"{0}\", fields : \"counters\">)@.counters@.mutual_friends[0], ", id.id).Replace("<", "{").Replace(">", "}");
            code += "};";
            string answer = Execute(accessToken, code);
            return JsonConvert.DeserializeObject<ExecuteClass>(answer);
        }

        public void Start(string accessToken)
        {
            this.accessToken = accessToken;
            this.thread = new Thread(ThreadFinder);
            this.thread.Start();
        }

        private void ThreadFinder()
        {
            var allShortPersons = API_friends_getSuggestion.GetRespounce(accessToken);
            for (int i = 0; i < allShortPersons.response.Count; i += 25)
            {
                int end = allShortPersons.response.Count - i;
                end = end < 25 ? end : 25;
                var partShortPersons = allShortPersons.response.GetRange(i, end);
                var partPersons = GetMutual(partShortPersons);
                partShortPersons.ForEach(delegate(Person person){   person.mutual_friends = partPersons.response[person.id];    });
                lock (_syncLst)
                {
                    lstItems.AddRange(partShortPersons);
                }
            }
            thread = null;
        }
    }
}
