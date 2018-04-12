using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections;

namespace kafka_json_parser{
    class Program{

        static string destination_dir= @"/home/nd/atlis-kafka-cs/";
        //static string source_dir= @"C:\repos\atlis-json-schema\";        
        static string source_dir= @"/home/nd/atlis-json-schema/";
        //        static string source_dir= @"";        
        public static async Task jsonAsync(string file){
            string directoryName = Path.GetDirectoryName(file);
            string base_filenname=Path.GetFileNameWithoutExtension(file);
            string https_schema_name="https://schemas.insightglobal.net/"+base_filenname;

            string schema_text= File.ReadAllText(file);
            // schema_text=schema_text.Replace("$ref\": \"https://schemas.insightglobal.net/","$ref\":\"http://schema.dev.box/");

            try{

                NJsonSchema.Generation.JsonSchemaGeneratorSettings settings=new NJsonSchema.Generation.JsonSchemaGeneratorSettings();
                Func<JsonSchema4, JsonReferenceResolver> referenceResolverFactory = 
                    schema => new JsonReferenceResolver(
                        new NJsonSchema.JsonSchemaResolver(schema, settings)
                    );

                Console.WriteLine(file);
                var schema_object= await JsonSchema4.FromFileAsync(file,referenceResolverFactory).ConfigureAwait(true);

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

        public static bool compile(string filename,string code){
            Console.WriteLine("Compiling");
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = filename+".dll";
            //Add reference to integrated objects
            //parameters.ReferencedAssemblies.Add("data.dll");

            if(String.IsNullOrWhiteSpace(code)) {
                Console.WriteLine("No code.");
                return false;
            }
            if(String.IsNullOrWhiteSpace(filename)) {
                Console.WriteLine("No DLL name.");
                return false;
            }

            CompilerResults cr = codeProvider.CompileAssemblyFromSource(parameters, code);

        
            if( cr.Errors.Count > 0 ) {
                for( int i=0; i<cr.Output.Count; i++ )  Console.WriteLine( cr.Output[i] );
                for( int i=0; i<cr.Errors.Count; i++ )  Console.WriteLine( i.ToString() + ": " + cr.Errors[i].ToString() );
                return false;

            } else {
                // Display information about the compiler's exit code and the generated assembly.
                Console.WriteLine( "Compiler returned with result code: " + cr.NativeCompilerReturnValue.ToString() );
                Console.WriteLine( "Generated assembly name: " + cr.CompiledAssembly.FullName );
                if( cr.PathToAssembly == null )
                    Console.WriteLine( "The assembly has been generated in memory." );
                else
                    Console.WriteLine( "Path to assembly: " + cr.PathToAssembly );

                // Display temporary files information.
                if( !cr.TempFiles.KeepFiles ) Console.WriteLine( "Temporary build files were deleted." );
                else {
                    Console.WriteLine( "Temporary build files were not deleted." );
                    // Display a list of the temporary build files
                    IEnumerator enu = cr.TempFiles.GetEnumerator();                                        
                    for( int i=0; enu.MoveNext(); i++ )                                          
                        Console.WriteLine( "TempFile " + i.ToString() + ": " + (string)enu.Current );                  
                    return false;

                }
            }    
            return true;
        }



        static void Main(string[] args) {
           // string [] file_list=files.get();
            //foreach(string file in file_list) {
            string file=source_dir+@"account-management/event/AccountCreated.v1.json";
            //string file=@"C:\repos\atlis-json-schema\event.v1.json";
                Task taskA = Task.Factory.StartNew(() => jsonAsync(file,output_file) );
                taskA.Wait();
            //}
            Console.ReadKey();
        }//end main
    }//end class
}//end namespace 
