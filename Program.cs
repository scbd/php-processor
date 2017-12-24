using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PhpProcessor
{
	class Program
	{
		static Regex JobOpeningID   = new Regex(@"Job Opening ID: \d+ Application Date:");
		static Regex PageXofY       = new Regex(@"Page\s\d+\sof\s\d+$");
		static Regex EndsWithColumn = new Regex(@".*:$");

		static string WorkingDirectory = ".";
        static string Outfilepath  { get { return Path.Combine(WorkingDirectory, "_output.txt"); } }

		//============================================
		//
		//
		//============================================
		static void Main(string[] args)
		{
            if(args.Length>0)
                WorkingDirectory = args[0];

            if(!Path.IsPathRooted(WorkingDirectory))
                WorkingDirectory = Path.Combine(Environment.CurrentDirectory, WorkingDirectory);

            WorkingDirectory = Path.GetFullPath(WorkingDirectory);

            if(!Directory.Exists(WorkingDirectory)) {
                Console.Error.WriteLine("Invalid working directory: " + WorkingDirectory);
                Environment.Exit(-1);
            }

            Console.WriteLine("Working Directory: {0}", WorkingDirectory);

			if(File.Exists(Outfilepath))
				File.Delete(Outfilepath);

            File.CreateText(Outfilepath).Close();

			Write("Filename            ".Trim());
			Write("Name                ".Trim());
			Write("UncsStatus          ".Trim());
			Write("StartOfAppointment  ".Trim());
			Write("UNEntity            ".Trim());
			Write("DateOfBirth         ".Trim());
			Write("Nationality         ".Trim());
			Write("Gender              ".Trim());
			Write("OtherNationalities  ".Trim());

			Write("Job1_Title          ".Trim());
			Write("Job1_NameOfEmployer ".Trim());
			Write("Job1_FromTo         ".Trim());
			Write("Job2_Title          ".Trim());
			Write("Job2_NameOfEmployer ".Trim());
			Write("Job2_FromTo         ".Trim());
			Write("Job3_Title          ".Trim());
			Write("Job3_NameOfEmployer ".Trim());
			Write("Job3_FromTo         ".Trim());
			Write("Job4_Title          ".Trim());
			Write("Job4_NameOfEmployer ".Trim());
			Write("Job4_FromTo         ".Trim());

			Write("English (S/W)".Trim());
			Write("French (S/W) ".Trim());
			Write("Spanish (S/W)".Trim());
			Write("Arabic (S/W) ".Trim());
			Write("Chinese (S/W)".Trim());
			Write("Russian (S/W)".Trim());
			Write("AllLanguages ".Trim());

			Write("Educ1_Degree        ".Trim());
			Write("Educ1_Obtained      ".Trim());
			Write("Educ1_FieldOfStudy  ".Trim());
			Write("Educ1_Title         ".Trim());
			Write("Educ2_Degree        ".Trim());
			Write("Educ2_Obtained      ".Trim());
			Write("Educ2_FieldOfStudy  ".Trim());
			Write("Educ2_Title         ".Trim());
			Write("Educ3_Degree        ".Trim());
			Write("Educ3_Obtained      ".Trim());
			Write("Educ3_FieldOfStudy  ".Trim());
			Write("Educ3_Title         ".Trim());
			Write("Educ4_Degree        ".Trim());
			Write("Educ4_Obtained      ".Trim());
			Write("Educ4_FieldOfStudy  ".Trim());
			Write("Educ4_Title         ".Trim());

            WriteLine();

			foreach(string sFilePath in Directory.GetFiles(WorkingDirectory, "*.txt"))
			{
				if(Path.GetFileName(sFilePath) == Path.GetFileName(Outfilepath))
					continue;

				List<string> data = Load(sFilePath).ToList();

                if(!data[0].Contains("Job Opening"))
                {
                    Console.WriteLine("SKIP:       {0} => Probably not a PHP! ", Path.GetFileName(sFilePath));
                    continue;
                }

                Console.WriteLine("Processing: {0}", Path.GetFileName(sFilePath));


				var qEducation1 = data.ExtractSection(new Regex(@"^Name of Institution:$"), new Regex(@"^Name of Institution:"), new Regex(@"^Name of Institution:$"), new Regex(@"^Employment$")); 
				var qEducation2 = data.ExtractSection(new Regex(@"^Name of Institution:$"), new Regex(@"^Name of Institution:"), new Regex(@"^Name of Institution:$"), new Regex(@"^Employment$")); 
				var qEducation3 = data.ExtractSection(new Regex(@"^Name of Institution:$"), new Regex(@"^Name of Institution:"), new Regex(@"^Name of Institution:$"), new Regex(@"^Employment$")); 
				var qEducation4 = data.ExtractSection(new Regex(@"^Name of Institution:$"), new Regex(@"^Name of Institution:"), new Regex(@"^Name of Institution:$"), new Regex(@"^Employment$")); 
                
				var qJob1 = data.ExtractSection(new Regex(@"^Job Title:$"), new Regex(@"^Job Title:"), new Regex(@"^Job Title:$"), new Regex(@"^Languages$")); 
				var qJob2 = data.ExtractSection(new Regex(@"^Job Title:$"), new Regex(@"^Job Title:"), new Regex(@"^Job Title:$"), new Regex(@"^Languages$")); 
				var qJob3 = data.ExtractSection(new Regex(@"^Job Title:$"), new Regex(@"^Job Title:"), new Regex(@"^Job Title:$"), new Regex(@"^Languages$")); 
				var qJob4 = data.ExtractSection(new Regex(@"^Job Title:$"), new Regex(@"^Job Title:"), new Regex(@"^Job Title:$"), new Regex(@"^Languages$")); 

				var Filename           = new List<string>() { "Filename:", Path.GetFileName(sFilePath) };
				var Name               = data.ExtractSection(new Regex(@"^Personal History Profile for$"),    EndsWithColumn, new Regex(@"^User Profile as Indicated at Time of Application$"));
				var UncsStatus         = data.ExtractSection(new Regex(@"^Applicant's UNCS Status:$"),        EndsWithColumn);
				var StartOfAppointment = data.ExtractSection(new Regex(@"^Start date of appointment:$"),      EndsWithColumn);
				var UNEntity           = data.ExtractSection(new Regex(@"^UN Entity:$"),                      EndsWithColumn);
				var DateOfBirth        = data.ExtractSection(new Regex(@"^Date of Birth:$"),                  EndsWithColumn);
				var Nationality        = data.ExtractSection(new Regex(@"^Country of Nationality:$"),         EndsWithColumn);
				var oGender            = data.ExtractSection(new Regex(@"^Gender:$"),                         EndsWithColumn);
				var OtherNationalities = data.ExtractSection(new Regex(@"^Other Nationalities \(if any\):$"), EndsWithColumn, new Regex(@"^Address$"));

				var LanguageSection       = data.ExtractSection(new Regex(@"^Languages$"),   new Regex(@"^Read$"), new Regex(@"^UN Training$"));
				var Languages_Columns     = LanguageSection.SingleOrDefault(o=>o.StartsWith("Language ")).Split(' ').ToList();

				var Index_Read  = Languages_Columns.IndexOf("Read");
				var index_Speak = Languages_Columns.IndexOf("Speak");
				var index_Write = Languages_Columns.IndexOf("Write");

				List<string> English = new List<string>() { "English"            };
				List<string> French  = new List<string>() { "French"             };
				List<string> Spanish = new List<string>() { "Spanish"            };
				List<string> Arabic  = new List<string>() { "Arabic"             };
				List<string> Russian = new List<string>() { "Russian"            };
				List<string> Chinese = new List<string>() { "Chinese (Mandarin)" };

				English.AddRange(LanguageSection.SeekLevels("English",            index_Speak, index_Write));
				French .AddRange(LanguageSection.SeekLevels("French",             index_Speak, index_Write));
				Spanish.AddRange(LanguageSection.SeekLevels("Spanish",            index_Speak, index_Write));
				Arabic .AddRange(LanguageSection.SeekLevels("Arabic",             index_Speak, index_Write));
				Russian.AddRange(LanguageSection.SeekLevels("Russian",            index_Speak, index_Write));
				Chinese.AddRange(LanguageSection.SeekLevels("Chinese (Mandarin)", index_Speak, index_Write));

				var OtherLangues = new List<string>();

				foreach(var row in LanguageSection){

					var parts = row.Split(' ');
					var lang  =  parts.FirstOrDefault();

					if(parts.Length!=7)	continue;
					if(lang == "English")	continue;
					if(lang == "French" )	continue;
					if(lang == "Spanish")	continue;
					if(lang == "Arabic" )	continue;
					if(lang == "Russian")	continue;
					if(lang == "Chinese")	continue;

					OtherLangues.Add("|");
					OtherLangues.Add(lang);
					OtherLangues.AddRange((new string[] { row }).SeekLevels(lang, index_Speak, index_Write));
				}
	
				var Job1_Title          = qJob1.ExtractSection(new Regex(@"^Job Title:$"),                             EndsWithColumn); 
				var Job1_NameOfEmployer = qJob1.ExtractSection(new Regex(@"^Name of Employer \(Type of Business\):$"), EndsWithColumn); 
				var Job1_FromTo         = qJob1.ExtractSection(new Regex(@"^From / To:$"),                             EndsWithColumn, new Regex("^Type of contract")); 
			
				var Job2_Title          = qJob2.ExtractSection(new Regex(@"^Job Title:$"),                             EndsWithColumn); 
				var Job2_NameOfEmployer = qJob2.ExtractSection(new Regex(@"^Name of Employer \(Type of Business\):$"), EndsWithColumn); 
				var Job2_FromTo         = qJob2.ExtractSection(new Regex(@"^From / To:$"),                             EndsWithColumn, new Regex("^Type of contract")); 
			
				var Job3_Title          = qJob3.ExtractSection(new Regex(@"^Job Title:$"),                             EndsWithColumn); 
				var Job3_NameOfEmployer = qJob3.ExtractSection(new Regex(@"^Name of Employer \(Type of Business\):$"), EndsWithColumn); 
				var Job3_FromTo         = qJob3.ExtractSection(new Regex(@"^From / To:$"),                             EndsWithColumn, new Regex("^Type of contract")); 
			
				var Job4_Title          = qJob4.ExtractSection(new Regex(@"^Job Title:$"),                             EndsWithColumn); 
				var Job4_NameOfEmployer = qJob4.ExtractSection(new Regex(@"^Name of Employer \(Type of Business\):$"), EndsWithColumn); 
				var Job4_FromTo         = qJob4.ExtractSection(new Regex(@"^From / To:$"),                             EndsWithColumn, new Regex("^Type of contract")); 


			    var Educ1_Obtained      = qEducation1.ExtractSection(new Regex(@"^Degree Obtained: \w+$"),          EndsWithColumn).Select(o=>o.Replace("Degree Obtained: ", "")).ToList(); 
			    var Educ1_Degree        = qEducation1.ExtractSection(new Regex(@"^Degree obtained:$"),              EndsWithColumn);
			    var Educ1_FieldOfStudy  = qEducation1.ExtractSection(new Regex(@"^Specialization:$"),               EndsWithColumn);
			    var Educ1_Title         = qEducation1.ExtractSection(new Regex(@"^Title in English or French:$"),   EndsWithColumn);

			    var Educ2_Obtained      = qEducation2.ExtractSection(new Regex(@"^Degree Obtained: \w+$"),          EndsWithColumn).Select(o=>o.Replace("Degree Obtained: ", "")).ToList(); 
			    var Educ2_Degree        = qEducation2.ExtractSection(new Regex(@"^Degree obtained:$"),              EndsWithColumn);
			    var Educ2_FieldOfStudy  = qEducation2.ExtractSection(new Regex(@"^Specialization:$"),               EndsWithColumn);
			    var Educ2_Title         = qEducation2.ExtractSection(new Regex(@"^Title in English or French:$"),   EndsWithColumn);

			    var Educ3_Obtained      = qEducation3.ExtractSection(new Regex(@"^Degree Obtained: \w+$"),          EndsWithColumn).Select(o=>o.Replace("Degree Obtained: ", "")).ToList(); 
			    var Educ3_Degree        = qEducation3.ExtractSection(new Regex(@"^Degree obtained:$"),              EndsWithColumn);
			    var Educ3_FieldOfStudy  = qEducation3.ExtractSection(new Regex(@"^Specialization:$"),               EndsWithColumn);
			    var Educ3_Title         = qEducation3.ExtractSection(new Regex(@"^Title in English or French:$"),   EndsWithColumn);

			    var Educ4_Obtained      = qEducation4.ExtractSection(new Regex(@"^Degree Obtained: \w+$"),          EndsWithColumn).Select(o=>o.Replace("Degree Obtained: ", "")).ToList(); 
			    var Educ4_Degree        = qEducation4.ExtractSection(new Regex(@"^Degree obtained:$"),              EndsWithColumn);
			    var Educ4_FieldOfStudy  = qEducation4.ExtractSection(new Regex(@"^Specialization:$"),               EndsWithColumn);
			    var Educ4_Title         = qEducation4.ExtractSection(new Regex(@"^Title in English or French:$"),   EndsWithColumn);

				Write(Filename            );
				Write(Name                );
				Write(UncsStatus          );
				Write(StartOfAppointment  );
				Write(UNEntity            );
				Write(DateOfBirth         );
				Write(Nationality         );
				Write(oGender             );
				Write(OtherNationalities  );

				Write(Job1_Title          );
				Write(Job1_NameOfEmployer );
				Write(Job1_FromTo         );
				Write(Job2_Title          );
				Write(Job2_NameOfEmployer );
				Write(Job2_FromTo         );
				Write(Job3_Title          );
				Write(Job3_NameOfEmployer );
				Write(Job3_FromTo         );
				Write(Job4_Title          );
				Write(Job4_NameOfEmployer );
				Write(Job4_FromTo         );

				Write(English );
				Write(French  );
				Write(Spanish );
				Write(Arabic  );
				Write(Chinese );
				Write(Russian );
				Write(OtherLangues);

			    Write(Educ1_Degree      );
			    Write(Educ1_Obtained,   skip:0);
			    Write(Educ1_FieldOfStudy);
			    Write(Educ1_Title       );
			    Write(Educ2_Degree      );
			    Write(Educ2_Obtained,   skip:0);
			    Write(Educ2_FieldOfStudy);
			    Write(Educ2_Title       );
			    Write(Educ3_Degree      );
			    Write(Educ3_Obtained,   skip:0);
			    Write(Educ3_FieldOfStudy);
			    Write(Educ3_Title       );
			    Write(Educ4_Degree      );
			    Write(Educ4_Obtained,   skip:0);
			    Write(Educ4_FieldOfStudy);
			    Write(Educ4_Title       );

				WriteLine();
			}
		}

		//============================================
		//
		//
		//============================================
		private static void Write(string text)
		{
			text = text.Replace("\t", " ").Replace("\r", "").Replace("\n", " ")+"\t";

			File.AppendAllText(Outfilepath, text, Encoding.UTF8);
			//Console.Write(text);
		}
		//============================================
		//
		//
		//============================================
		private static void WriteLine()
		{
			File.AppendAllText(Outfilepath, Environment.NewLine, Encoding.UTF8);
			//Console.WriteLine();
		}
		
		//============================================
		//
		//
		//============================================
		private static void Write(IEnumerable<string> data, int skip=1, int take=int.MaxValue, string sep=" ")
		{
			string sText = "";

			var qValues = data.Skip(skip).Take(take);

			if(qValues.Any())
				sText = qValues.Aggregate((r,v)=>r+sep+v);

			Write(sText);
		}
		
		//============================================
		//
		//
		//============================================
		private static IEnumerable<string> Load(string sFilePath)
		{
			IEnumerable<string> qLines = File.ReadAllLines(sFilePath, Encoding.UTF8);

			qLines = qLines.Select(o=>o.Trim());
			qLines = qLines.Where (o=>o!="");
			qLines = qLines.Where (o=>!PageXofY.IsMatch(o));
			qLines = qLines.Where ((o,i)=>i==0 || !JobOpeningID.IsMatch(o));

			return qLines;
		}
	}

	static class extention
	{

		//============================================
		//
		//
		//============================================
		public static IEnumerable<string> SeekLevels(this IEnumerable<string> lines, string value, params int [] indexes)
		{
			List<string> result = new List<string>();

			string line  = lines.SingleOrDefault(o=>o.StartsWith(value+" ")) ?? "";

			if(line!="")
				line = line.Substring(value.Length).Trim();

			var levels = ("LANGUAGE " + line).Split(' ');

			foreach(var index in indexes)
			{
				if(result.Count>0)
					result.Add("/");

				if(levels.Length>index)
					result.Add(levels[index]);
				else
					result.Add("NA");
			}
			
			return result;
		}

		//============================================
		//
		//
		//============================================
		public static List<string> ExtractSection(this List<string> lines, Regex starts, params Regex [] ends)
		{
			int iStart = -1;
			int iEnds  = -1;

			for(int i=0; i<lines.Count; ++i)
			{
				if(iStart>=0 && iEnds<0 && ends.Any(o=>o.IsMatch(lines[i])))
					iEnds  = i;

				if(iStart <0 && (starts==null || starts.IsMatch(lines[i])))
					iStart = i;

				if(iStart>=0 && iEnds>=0)
					break;
			}

			List<string> oExtractedLines = new List<string>();

			if(iStart>=0)
			{
				if(iEnds<0) 
					iEnds = lines.Count;

				for(int i=iStart; i<iEnds; ++i)
				{
					string sLine = lines[iStart];

					lines.RemoveAt(iStart);
					oExtractedLines.Add(sLine);
				}
			}

			return oExtractedLines;
		}
	}
}
