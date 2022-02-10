using System;
using System.Collections.Generic;
using System.Text;

namespace RESTApiWithAzureFunction
{
    public class PersonData
    {
        public int ID { get; set; }
        public string first_name { get; set; }
        public string pref_lang  { get; set; }
        public int Age { get; set; }
    }
    public class CreatePerson
    {
        public int ID { get; set; }
        public string first_name { get; set; }
        public string pref_lang { get; set; }
        public int Age { get; set; }
    }
    public class UpdatePerson
    {
        public string first_name { get; set; }
        public string pref_lang { get; set; }
        public int Age { get; set; }
    }
}
