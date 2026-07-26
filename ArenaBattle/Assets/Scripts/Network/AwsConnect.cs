using UnityEngine;
using UnityEngine.UI;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using System.IO;
using System.Configuration;
public class AwsConnect : MonoBehaviour
{
	#region AWS
	private static string S3BucketName = "futtidino-data";
	private static string pathName = "game-data";
	private static string serverFileName = "ServerInfoData.json"; 
	private static IAmazonS3 _s3Client;
	//private AWSCredentials _credentials;
	private static CognitoAWSCredentials _credentials;

	public string CognitoIdentityRegion = RegionEndpoint.APSoutheast1.SystemName;
	private RegionEndpoint _CognitoIdentityRegion
	{
		get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
	}
	public static string S3Region = RegionEndpoint.APSoutheast1.SystemName;
	private static RegionEndpoint _S3Region
	{
		get { return RegionEndpoint.GetBySystemName(S3Region); }
	}

	private static AWSCredentials Credentials
	{
		get
		{
			_credentials = new CognitoAWSCredentials(
				"ap-southeast-1:6cbc861e-85ff-4260-957b-d2c8f1a94c98", // 자격 증명 풀 ID
				RegionEndpoint.APSoutheast1 // 리전
			);
			//if (_credentials == null)
				//_credentials = new BasicAWSCredentials("AKIAYA4MQWOLHPRZHJYD", "T5zoI3hIqlnepJbL+w/FUbGq29e0kBCrCDvoffNH");
			return _credentials; 
		}
	}

	private static IAmazonS3 Client
	{
		get
		{
			if (_s3Client == null)
			{
				_s3Client = new AmazonS3Client(Credentials, _S3Region);
			}
			return _s3Client;
		}
	}
	#endregion

	// Start is called before the first frame update
	void Start()
	{
		//		UnityInitializer.AttachToGameObject(this.gameObject);
	}

	public static string ReadObjectDataAsync()
	{
		GetObjectResponse response = Client.GetObject(S3BucketName, $"{pathName}/{serverFileName}");

		Stream responseStream = response.ResponseStream;

		StreamReader reader = new StreamReader(responseStream);

		string responseBody = reader.ReadToEnd(); // Now you process the response body.

		return responseBody;
	}

	public static string DownloadFileFromAWS(string fileName)
	{
		GetObjectResponse response = Client.GetObject(S3BucketName, $"{pathName}/{fileName}");

		Stream responseStream = response.ResponseStream;

		StreamReader reader = new StreamReader(responseStream);

		string responseBody = reader.ReadToEnd(); // Now you process the response body.

		return responseBody;
	}
}
