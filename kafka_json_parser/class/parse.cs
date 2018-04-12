using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NJsonSchema;
using Newtonsoft.Json.Schema;
using NJsonSchema.CodeGeneration.CSharp;

namespace kafka_json_parser{
    public static class parse {
        static string destination_dir= @"C:\repos\atlis_cs_project\";
        static string source_dir= @"C:\repos\atlis-json-schema\";        
//        static string source_dir= @"";        
        public static async Task jsonAsync(string file){
            string directoryName = Path.GetDirectoryName(file);
            string base_filenname=Path.GetFileNameWithoutExtension(file);
            string https_schema_name="https://schemas.insightglobal.net/"+base_filenname;

            string schema_text= File.ReadAllText(file);
            schema_text=schema_text.Replace("$ref\": \"https://schemas.insightglobal.net/","$ref\":\"http://schema.dev.box/");
                
            try{
                
               NJsonSchema.Generation.JsonSchemaGeneratorSettings settings=new NJsonSchema.Generation.JsonSchemaGeneratorSettings();
               Func<JsonSchema4, JsonReferenceResolver> referenceResolverFactory = schema => new JsonReferenceResolver(new NJsonSchema.JsonSchemaResolver(schema, settings));
                
               Console.WriteLine(file);
               var schema_object= await JsonSchema4.FromUrlAsync("http://schema.dev.box/account-management/event/AccountCreated.v1.json").ConfigureAwait(true);

               // var schema_object= await JsonSchema4.FromUrlAsync(https_schema_name,referenceResolverFactory).ConfigureAwait(false);

                //settings


                //generate c# class
                var generator = new CSharpGenerator(schema_object);             
                var cs_file = generator.GenerateFile();

                string sub_path=directoryName.Substring(source_dir.Length-1);
                string dest_filename=String.Format("{0}\\{1}\\{2}.{3}",destination_dir,sub_path,base_filenname,"cs");
                
                //create directory
                string dest_file_dir= Path.GetDirectoryName(dest_filename);
                DirectoryInfo di = Directory.CreateDirectory(dest_file_dir);
                //write file
                File.WriteAllText(dest_filename, cs_file);
             } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(schema_text);

            }
        }
    }
}
