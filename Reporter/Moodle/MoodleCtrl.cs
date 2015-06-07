using System.Diagnostics;
using System.Security.Policy;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReportsGenerator.DataStructures;
using ReportsGenerator.Settings;

namespace ReportsGenerator.Moodle
{
    class MoodleCtrl
    {
        public MoodleCtrl()
        {
            UpdateMoodleSettings();
        }

        public void UpdateMoodleSettings()
        {
            this.MoodleServer = MoodleSettings.Default.Server;
            this.Token = MoodleSettings.Default.Token;
        }

        private const string RequestMethod = "GET";
        public string Token
        {
            get;
            set;
        }

        public string MoodleServer
        {
            get;
            set;
        }

        private async Task<string> RequestAsync(String url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                Trace.WriteLine(DateTime.Now + url);
                request.Method = RequestMethod;
                request.Timeout = 15000;
                WebResponse response;
                int timeout = 20000;
                var ts = new CancellationTokenSource();
                var task = request.GetResponseAsync();

                CancellationToken ct = ts.Token;
                if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                {
                    response = await task;
                }
                else
                {
                    request.Abort();
                    return await RequestAsync(url);
                }

                string responseFromServer;
                using (var dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseFromServer = reader.ReadToEnd();
                }

                return responseFromServer;
            }
            catch (Exception e)
            {
                throw new ReporterException(e.Message);
            }
        }

        public const string TokenRequest = "{0}/login/token.php?username={1}&password={2}&service={3}";
        public async Task<string> GetTokenAsync(string webService, string username, string password)
        {
            string url = string.Format(TokenRequest, MoodleServer, username, password, webService);
            string response = await RequestAsync(url);
            Regex r = new Regex("\"token\":\"(.*?)\"");
            return r.Match(response).Groups[0].Value;
        }

        private async Task<string> GetFuncResult(string function, string token, string parameters)
        {
            string url = MoodleServer + "/webservice/rest/server.php?moodlewsrestformat=json&wstoken=" + token + "&wsfunction=" + function + "&" + parameters;
            return await RequestAsync(url);
        }

        private T JsonToObjectAsync<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Exception toThrow;
                try
                {
                    toThrow = new Exception(JObject.Parse(json)["message"].ToString());
                }
                catch
                {
                    toThrow = new Exception("Неправильно указан адрес серверра.");
                }
                throw toThrow;
            }
        }

        public async Task<List<User>> GetUsersByIds(IEnumerable<int> ids)
        {
            try
            {
                StringBuilder parameters = new StringBuilder();
                parameters.Append("&field=id");
                string p1 = "&values[";
                string p2 = "]=";

                int i = 0;
                foreach (int id in ids)
                {
                    parameters.Append(p1).Append(i++).Append(p2).Append(id);
                }

                string jsonStr = await GetFuncResult("core_user_get_users_by_field", Token, parameters.ToString());
                List<User> users = JsonToObjectAsync<List<User>>(jsonStr);
                return users;
            }
            catch (Exception e)
            {
                throw new ReporterException("При попытке получения пользователей по ID: \"" + ids + "\" возникла следующая ошибка:\"" + e.Message + "\"");
            }
        }

        //http://moodletest12.herokuapp.com/webservice/rest/server.php?wstoken=87697b3448991f70b8d7fe0895085fdc&wsfunction=core_enrol_get_enrolled_users&courseid=2&moodlewsrestformat=json
        public async Task<List<User>> GetEnrolUsers(int courseId)
        {
            try
            {
                StringBuilder parameters = new StringBuilder();
                parameters.Append("&courseid=").Append(courseId);

                string jsonStr = await GetFuncResult("core_enrol_get_enrolled_users", Token, parameters.ToString());
                List<User> users = JsonToObjectAsync<List<User>>(jsonStr);
                return users;
            }
            catch (Exception e)
            {
                throw new ReporterException("При попытке получения пользователя со следующим ID: \"" + courseId + "\" возникла следующая ошибка:\"" + e.Message + "\"");
            }
        }

        //http://moodletest12.herokuapp.com/webservice/rest/server.php?wstoken=87697b3448991f70b8d7fe0895085fdc&wsfunction=core_enrol_get_enrolled_users&courseid=4&options[0][name]=groupid&options[0][value]=1
        //http://www.oe-it.ru/edu/webservice/rest/server.php?wstoken=566dc30826ab9e5ddfa1835868fc6052&wsfunction=core_enrol_get_enrolled_users&courseid=4&options[0][name]=groupid&options[0][value]=1
        public async Task<List<User>> GetEnrolUsers(int courseId, int groupId)
        {
            try
            {
                Debug.WriteLine("getEnrolUsers--start for " + groupId);
                StringBuilder parameters = new StringBuilder();
                parameters.Append("&courseid=").Append(courseId).Append("&options[0][name]=groupid&options[0][value]=").Append(groupId);
                string jsonStr = await GetFuncResult("core_enrol_get_enrolled_users", Token, parameters.ToString());
                List<User> users = JsonToObjectAsync<List<User>>(jsonStr);
                Debug.WriteLine("getEnrolUsers--ok");
                return users;
            }
            catch (Exception e)
            {
                throw new ReporterException("При попытке получения пользователей курса с ID \"" + courseId + "\" и группой c ID \"" + groupId + "\" возникла следующая ошибка:\"" + e.Message + "\"");
            }
        }

        //http://moodletest12.herokuapp.com/webservice/rest/server.php?wstoken=87697b3448991f70b8d7fe0895085fdc&wsfunction=core_grades_get_grades&courseid=2&userids[0]=8&userids[1]=9
        //http://www.oe-it.ru/edu/webservice/rest/server.php?wstoken=566dc30826ab9e5ddfa1835868fc6052&wsfunction=core_grades_get_grades&courseid=2&userids[0]=42&userids[1]=76&userids[1]=1574
        public async Task<List<Activity>> GetGrades(int courseId, IEnumerable<int> usersIds)
        {
            try
            {
                StringBuilder parameters = new StringBuilder();
                parameters.Append("&courseid=").Append(courseId);
                string p1 = "&userids[";
                string p2 = "]=";

                int i = 0;
                foreach (int id in usersIds)
                {
                    parameters.Append(p1).Append(i++).Append(p2).Append(id);
                }

                string jsonStr = await GetFuncResult("core_grades_get_grades", Token, parameters.ToString());
                JObject jo = JsonToObjectAsync<JObject>(jsonStr);
                return jo.GetValue("items").ToObject<List<Activity>>();
            }
            catch (Exception e)
            {
                throw new ReporterException("При попытке получения оценок пользователей курса с ID \"" + courseId + "\" возникла следующая ошибка:\"" + e.Message + "\"");
            }
        }

        public async Task<List<Activity>> GetGrades(int courseId, IEnumerable<User> users)
        {
            try
            {
                StringBuilder parameters = new StringBuilder();
                parameters.Append("&courseid=").Append(courseId);
                string p1 = "&userids[";
                string p2 = "]=";
                int i = 0;
                foreach (User user in users)
                {
                    parameters.Append(p1).Append(i++).Append(p2).Append(user.Id);
                }

                string jsonStr = await GetFuncResult("core_grades_get_grades", Token, parameters.ToString());
                JObject jo = JsonToObjectAsync<JObject>(jsonStr);
                return jo.GetValue("items").ToObject<List<Activity>>();
            }
            catch (Exception e)
            {
                throw new ReporterException("При попытке получения оценок пользователей курса с ID \"" + courseId + "\" возникла следующая ошибка:\"" + e.Message + "\"");
            }
        }

        //http://moodletest12.herokuapp.com/webservice/rest/server.php?wstoken=87697b3448991f70b8d7fe0895085fdc&wsfunction=core_course_get_contents&courseid=2&moodlewsrestformat=json
        //http://moodletest12.herokuapp.com/webservice/rest/server.php?wstoken=87697b3448991f70b8d7fe0895085fdc&wsfunction=core_course_get_courses&options[ids][0]=2&&moodlewsrestformat=json
        public async Task<List<Course>> GetCoursesByIds(IEnumerable<int> ids)
        {
            try
            {
                StringBuilder parameters = new StringBuilder();
                string p1 = "&options[ids][";
                string p2 = "]=";

                int i = 0;
                foreach (int id in ids)
                {
                    parameters.Append(p1).Append(i++).Append(p2).Append(id);
                }

                string jsonStr = await GetFuncResult("core_course_get_courses", Token, parameters.ToString());

                return JsonToObjectAsync<List<Course>>(jsonStr);
            }
            catch (Exception e)
            {

                throw new ReporterException("При попытке получения данных о курсах с ID: \"" + string.Join(", ", ids) + "\" возникла следующая ошибка:\"" + e.Message + "\"", e);
            }
        }

        //http://moodletest12.herokuapp.com/webservice/rest/server.php?wstoken=87697b3448991f70b8d7fe0895085fdc&wsfunction=core_course_get_contents&courseid=2
        public async Task<Time[]> GetCourseContent(int courseid)
        {
            try
            {
                StringBuilder parameters = new StringBuilder();
                parameters.Append("&courseid=").Append(courseid);

                string jsonStr = await GetFuncResult("core_course_get_contents", Token, parameters.ToString());
                var times = from v in JArray.Parse(jsonStr) select new Time(v.ToObject<JObject>());
                return times.ToArray();
            }
            catch (Exception e)
            {
                throw new ReporterException("При попытке получения данных о курсе с ID: \"" + courseid + "\" возникла следующая ошибка:\"" + e.Message + "\"");
            }
        }

        //http://moodletest12.herokuapp.com/webservice/rest/server.php?wstoken=87697b3448991f70b8d7fe0895085fdc&wsfunction=core_group_get_course_groups&courseid=4
        public async Task<List<DataStructures.Group>> GetCourseGroups(int courseid)
        {
            try
            {
                StringBuilder parameters = new StringBuilder();
                parameters.Append("&courseid=").Append(courseid);

                string jsonStr = await GetFuncResult("core_group_get_course_groups", Token, parameters.ToString());
                return JsonToObjectAsync<List<DataStructures.Group>>(jsonStr);
            }
            catch (Exception e)
            {
                throw new ReporterException("При попытке получении списка групп курса с ID \"" + courseid + "\" возникла следующая ошибка:\"" + e.Message + "\"");
            }
        }
    }
}
