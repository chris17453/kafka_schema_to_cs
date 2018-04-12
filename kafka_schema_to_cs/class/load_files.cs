using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kafka_json_parser{
    public static class files{

        static string source_dir= @"C:\repos\atlis-json-schema\";
        static string file_type = "*.json";
        
        public static string[] get(){
             string[] files = Directory.GetFiles(source_dir,file_type,SearchOption.AllDirectories);
            return files;
        }//end get_files


    }
}
