using BepInEx;
using Receiver2;
using Receiver2ModdingKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunDealer
{
	[BepInPlugin("CiarenceW-GunDealer", "GunDealer", "1.0.0")]
	[BepInProcess("receiver2.exe")]
	internal class GunDealerManager : BaseUnityPlugin
    {
		private GunDealer gunDealerConsole;

        private void Awake()
        {
			var attribute = this.GetBepInAttribute();

            Logger.LogInfo($"GunDealer {attribute.GUID} version {attribute.Version} Awaken wokeism woke");

            ReceiverEvents.StartListening(ReceiverEventTypeVoid.PlayerInitialized, new UnityEngine.Events.UnityAction<ReceiverEventTypeVoid>(InstantiateConsole));

			Receiver2ModdingKit.ModdingKitEvents.AddTaskAtCoreStartup(new Receiver2ModdingKit.ModdingKitEvents.StartupAction(FindGunDealerAssetBundle));
        }

		private void FindGunDealerAssetBundle()
		{
			string current_directory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
			Debug.Log(current_directory);
			if (Directory.Exists(current_directory)) //don't need to check but whatevs
			{
				var files_in_directory = Directory.GetFiles(current_directory);
				for (int i = 0; i < files_in_directory.Length; i++)
				{
					string fileName = files_in_directory[i];
					if (fileName.Contains(SystemInfo.operatingSystemFamily.ToString().ToLower()))
					{
						AssetBundle assetBundle = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault((AssetBundle bundle) => bundle.name.Contains(Path.GetFileNameWithoutExtension(fileName)));
						if (assetBundle == null)
						{
							assetBundle = AssetBundle.LoadFromFile(fileName);
							if (assetBundle == null)
							{
								Debug.LogWarning("Failed to load AssetBundle " + fileName);
								return;
							}
						}
						foreach (string asset_name in assetBundle.GetAllAssetNames())
						{
							GameObject gameObject = assetBundle.LoadAsset<GameObject>(asset_name);
							if (gameObject != null)
							{
								if (gameObject.TryGetComponent<GunDealer>(out gunDealerConsole))
								{
									Debug.Log("Found gun dealer bundle");
									return;
								}
							}
						}
					}
				}
				if (gunDealerConsole == null)
				{
					Debug.LogError("mdoel replacer is null!!!! SHITTTTTT!!!!!!!!!!!");
				}
			}
			else Debug.LogError("direrctort DOESNT FUCKING EDXIST");
		}

        private void InstantiateConsole(ReceiverEventTypeVoid ev)
        {
            if (ReceiverCoreScript.Instance().game_mode.GetGameMode() == GameMode.ReceiverMall)
            {
				Instantiate(gunDealerConsole, new Vector3(-0.17f, 1f, 15.1491f), new Quaternion(0f, 1f, 0f, 0f));
            }
			else if (Receiver2ModdingKit.Extensions.TryFindObjectOfType<GunDealerSpawnPoint>(out var result))
			{
				Instantiate(gunDealerConsole, result.transform.position, result.transform.rotation);
			}
        }
    }
}
