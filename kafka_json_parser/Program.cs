using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace kafka_json_parser{
    class Program{
        static void Main(string[] args) {
            string [] file_list=files.get();
            //foreach(string file in file_list) {
            string file=@"C:\repos\atlis-json-schema\account-management\event\AccountCreated.v1.json";
            //string file=@"C:\repos\atlis-json-schema\event.v1.json";
                Task taskA = Task.Factory.StartNew(() => parse.jsonAsync(file) );
                taskA.Wait();
            //}
            Console.ReadKey();
        }//end main
    }//end class
}//end namespace 
