using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

public class MenuUtility
{
#if UNITY_EDITOR && UNITY_ANDROID
		[MenuItem("Tools/GRPC/Build Proto File", false, 80)]
		public static void GenerateProtoFileAtMenu()
		{
			GenerateProtoFiles();
		}

		private static void GenerateProtoFiles()
		{
			Process.Start("Protobuild.bat");
		}

		[MenuItem("Tools/ServerData/Create ServerData Json File", false, 80)]
		public static void CreateServerDataJsonFileAtMenu()
		{
			CreateServerDataJsonFile();
		}

		private static void CreateServerDataJsonFile()
		{
			ServerInfoData serverInfo = new ServerInfoData();
			serverInfo.maintenance = false;
			serverInfo.csvFiles = null;
			
			List<string> csvFiles = new List<string>();
			
			string[] filenames = Directory.GetFiles("Assets/Resources/CSV");

			for (int i = 0; i < filenames.Length; i++)
			{
				if(filenames[i].Contains(".meta"))
				   continue;

				string file = Path.GetFileNameWithoutExtension(filenames[i]);
				if(csvFiles.Contains(file))
					continue;
				
				csvFiles.Add(file);
			}
/*			
			csvFiles.Add("dino_basic");
			csvFiles.Add("dino_monster");
			csvFiles.Add("expTable");
			csvFiles.Add("set_option");
			csvFiles.Add("string");
			csvFiles.Add("skill");
			csvFiles.Add("skilllevel");
			csvFiles.Add("sound");
			csvFiles.Add("dino_part");
			csvFiles.Add("talent");
			csvFiles.Add("random_option");
			csvFiles.Add("pure_dna_bonus");
			csvFiles.Add("effect");
			csvFiles.Add("limitvalue");
			csvFiles.Add("ani_event");
			csvFiles.Add("aniskill");
			csvFiles.Add("buff");
			csvFiles.Add("burst");
			csvFiles.Add("burst_type");
			csvFiles.Add("dino_status");
			csvFiles.Add("effectlist");
			csvFiles.Add("monsterai");
			csvFiles.Add("randomOptionTable");
			csvFiles.Add("stagemonster");
			csvFiles.Add("timechange");
*/
			serverInfo.csvFiles = csvFiles.ToArray();

			string json = JsonUtility.ToJson(serverInfo);
			string fileName = $"{Application.dataPath}/ServerInfoData.json";
			File.WriteAllText(fileName, json);
		}
		
		[MenuItem("Tools/CSV2CS", false, 80)]
		public static void CreateCSFileFromCSV()
		{
			var cSharpClass = CsvToClass.CSharpClassCodeFromCsvFile($"{Application.dataPath}/Resources/CSV/dino_part.csv");
			File.WriteAllText($"{Application.dataPath}/Resources/CSV/dino_part.cs", cSharpClass);
		}
#endif
}
