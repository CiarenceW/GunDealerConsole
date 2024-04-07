using Receiver2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using TMPro;
using UnityEngine;

namespace GunDealer
{
    public class GunDealer : MonoBehaviour
    {
        private List<string> gunList = new List<string>();

        private int selectedGun;
        public TextMeshPro gunNameText;

        public Transform spawnPoint;
        public Transform magSpawnPoint;

        private LevelObject levelObject;

        private void Awake()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform.Find("GunSpawnPoint");
            }

            if (magSpawnPoint == null)
            {
                magSpawnPoint = transform.Find("magSpawnPoint");
            }

            if (gunNameText == null)
            {
                gunNameText = transform.Find("screen/screen_text").GetComponent<TextMeshPro>();
            }

            levelObject = FindObjectOfType<LevelObject>();

			#if !UNITY_EDITOR
			var gunNameList = ReceiverCoreScript.Instance().GetGunNames();

            for (int i = 0; i < gunNameList.Length; i++)
            {
                if (!gunNameList[i].Contains("wolfire")) gunList.Add(gunNameList[i]); //removes vanilla guns
            }

            selectedGun = UnityEngine.Random.Range(0, gunList.Count - 1);
			#endif

            UpdateGunName();
        }

        public void SpawnGun()
        {
            Instantiate(ReceiverCoreScript.Instance().GetGunPrefab(gunList[selectedGun]), spawnPoint.position + (Vector3.up * 0.3f), spawnPoint.rotation, levelObject.PlayerRoomTransform).AddRigidBody();
        }

        private void UpdateGunName()
        {
            gunNameText.text = gunList[selectedGun].Split('.')[1].Replace('_', ' ').ToUpper();
        }

        public void SpawnMatchingMag()
        {
			var rootTypes = ReceiverCoreScript.Instance().GetGunPrefab(gunList[selectedGun]).magazine_root_types;
			for (int magRootIndex = 0; magRootIndex < rootTypes.Length; magRootIndex++)
			{
				if (ReceiverCoreScript.Instance().TryGetMagazinePrefabFromRoot(rootTypes[magRootIndex], MagazineClass.StandardCapacity, out var magScript))
				{
					Instantiate(magScript, magSpawnPoint.position + new Vector3(0.05f * magRootIndex, 0.05f), magSpawnPoint.rotation, levelObject.PlayerRoomTransform).AddRigidBody();
				}
			}
        }

        public void SelectNextGun()
        {
            if (selectedGun == 0)
            {
                selectedGun = gunList.Count - 1;
                UpdateGunName();
                return;
            }
            selectedGun--;
            UpdateGunName();
        }

        public void SelectPrevGun()
        {
            if (selectedGun == gunList.Count - 1)
            {
                selectedGun = 0;
                UpdateGunName();
                return;
            }
            selectedGun++;
            UpdateGunName();
        }
    }
}
